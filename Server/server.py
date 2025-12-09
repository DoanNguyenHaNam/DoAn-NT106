#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
server.py
Verbose TCP JSON server (BOM-safe).
- Accepts line-delimited JSON (each JSON ends with '\n').
- Uses encoding 'utf-8-sig' for makefile so BOM is removed on read.
- Also strips any leading U+FEFF before json.loads as extra safety.
- Responds in the same JSON shapes your C# client expects (login_data, post_data, etc.).
"""

import socket
import threading
import json
import traceback
import time
import atexit
import signal
import sys
import os
from typing import Dict, Any, Optional

HOST = "0.0.0.0"
PORT = 13579
ENC = "utf-8-sig"  # <<-- use utf-8-sig so BOM is removed automatically

VERBOSE_TO_CONSOLE = True
WRITE_LOG_FILE = True
LOG_FILENAME = "server_verbose.log"

# Database file paths
DB_DIR = "DB"
USERS_FILE = os.path.join(DB_DIR, "users.json")
SIGNUP_FILE = os.path.join(DB_DIR, "signup.json")
POSTS_FILE = os.path.join(DB_DIR, "posts.json")
COMMENTS_FILE = os.path.join(DB_DIR, "comments.json")
LIKES_FILE = os.path.join(DB_DIR, "likes.json")  # ← THÊM FILE LIKES

# Simple in-memory stores
USERS = {
    "nam": {"username": "nam", "password": "123", "email": "nam@example.com"},
}
USER_SIGNUP = {}
try:
    with open(SIGNUP_FILE, 'r', encoding='utf-8') as f:
        USER_SIGNUP = json.load(f)
except Exception:
    USER_SIGNUP = {}

# Load existing data from files
try:
    with open(USERS_FILE, 'r', encoding='utf-8') as f:
        loaded_users = json.load(f)
        USERS.update(loaded_users)
except Exception:
    pass

POSTS = []
try:
    with open(POSTS_FILE, 'r', encoding='utf-8') as f:
        POSTS = json.load(f)
except Exception:
    pass

COMMENTS = {}
try:
    with open(COMMENTS_FILE, 'r', encoding='utf-8') as f:
        COMMENTS = json.load(f)
except Exception:
    pass

LIKES = {}  # ← THÊM LIKES: {post_id: [username1, username2, ...]}
try:
    with open(LIKES_FILE, 'r', encoding='utf-8') as f:
        LIKES = json.load(f)
except Exception:
    pass

POST_ID_SEQ = 1
if POSTS:
    POST_ID_SEQ = max([p.get("id", 0) for p in POSTS]) + 1

LOCK = threading.Lock()

_log_lock = threading.Lock()

def log(msg, level="INFO", addr=None):
    t = time.strftime("%Y-%m-%d %H:%M:%S", time.localtime())
    a = f" {addr}" if addr is not None else ""
    line = f"{t} [{level}]{a} {msg}"
    with _log_lock:
        if VERBOSE_TO_CONSOLE:
            print(line)
        if WRITE_LOG_FILE:
            try:
                with open(LOG_FILENAME, "a", encoding="utf-8") as f:
                    f.write(line + "\n")
            except Exception:
                pass

def pretty(obj):
    try:
        return json.dumps(obj, ensure_ascii=False, indent=2)
    except Exception:
        return str(obj)

def save_all_data():
    """Save all data to JSON files when server shuts down"""
    log("Saving all data to files...", level="INFO")
    
    # Create db directory if it doesn't exist
    if not os.path.exists(DB_DIR):
        os.makedirs(DB_DIR)
    
    try:
        with LOCK:
            # Save users
            with open(USERS_FILE, 'w', encoding='utf-8') as f:
                json.dump(USERS, f, ensure_ascii=False, indent=2)
            log(f"Saved {len(USERS)} users to {USERS_FILE}", level="INFO")
            
            # Save signup data
            with open(SIGNUP_FILE, 'w', encoding='utf-8') as f:
                json.dump(USER_SIGNUP, f, ensure_ascii=False, indent=2)
            log(f"Saved signup data to {SIGNUP_FILE}", level="INFO")
            
            # Save posts
            with open(POSTS_FILE, 'w', encoding='utf-8') as f:
                json.dump(POSTS, f, ensure_ascii=False, indent=2)
            log(f"Saved {len(POSTS)} posts to {POSTS_FILE}", level="INFO")
            
            # Save comments
            with open(COMMENTS_FILE, 'w', encoding='utf-8') as f:
                json.dump(COMMENTS, f, ensure_ascii=False, indent=2)
            log(f"Saved comments to {COMMENTS_FILE}", level="INFO")
            
            # Save likes
            with open(LIKES_FILE, 'w', encoding='utf-8') as f:
                json.dump(LIKES, f, ensure_ascii=False, indent=2)
            log(f"Saved likes to {LIKES_FILE}", level="INFO")
            
        log("All data saved successfully!", level="INFO")
    except Exception as e:
        log(f"Error saving data: {e}", level="ERROR")
        traceback.print_exc()

def send_json_fileobj(f, obj: Dict[str, Any], addr=None):
    try:
        s = json.dumps(obj, ensure_ascii=False)
        f.write(s + "\n")
        f.flush()
        log(f"Sent JSON to client: {s}", level="DEBUG", addr=addr)
    except Exception as e:
        log(f"Failed to send to client: {e}", level="ERROR", addr=addr)

# ------------------ Handlers ------------------

def handle_login_data(req: Dict[str, Any]):
    # Accept only username == "Admin123" and password == "Admin123!"
    username = req.get("username") or req.get("UserName") or req.get("User") or ""
    request_id = req.get("request_id") or ""
    password = req.get("password") or req.get("Password") or req.get("Pass") or ""
    username = "" if username is None else str(username)
    password = "" if password is None else str(password)
    error = "NO"
    with LOCK:
        log(f"Login attempt: username='{username}', password='{password}'", level="INFO")
        if username in USER_SIGNUP:
            user_dir = "DB/user"
            if not os.path.exists(user_dir):
                os.makedirs(user_dir)
                log(f"Created directory: {user_dir}", level="INFO")
                    
            # Lưu file user riêng
            user_file = os.path.join(user_dir, f"{username}.json")
            with open(user_file, 'r', encoding='utf-8') as f:
                user_data = json.load(f)
            if password == user_data.get("password", ""):
                accept = True
                error = "Đăng Nhập Thành Công"
            else:
                accept = False
                error = "Sai Mật Khẩu"
        else:
            accept = (username == "Admin123" and password == "Admin123!")
            if accept:
                error = "Đăng Nhập Thành Công"
            else:
                error = "Tên đăng nhập không tồn tại"
        resp = {
            "action": "login_data",
            "username": username,
            "password": password,
            "error": error,
            "accept": accept,
            "request_id": f"ServerHaha_{request_id}"
        }
    return resp

def handle_signup_data(req: Dict[str, Any]):
    username = req.get("username") or req.get("UserName") or ""
    password = req.get("password") or req.get("Password") or ""
    email = req.get("email") or req.get("Email") or ""
    phone = req.get("phone") or req.get("Phone") or ""
    username = "" if username is None else str(username)
    password = "" if password is None else str(password)
    email = "" if email is None else str(email)
    phone = "" if phone is None else str(phone)

    error = "NO"
    with LOCK:
        log(f"Signup attempt: username='{username}', email='{email}', phone='{phone}'", level="INFO")
        
        # Kiểm tra username đã tồn tại chưa
        if username in USER_SIGNUP:
            accept = False
            error = "Tên đăng nhập đã tồn tại"
        else:
            # Kiểm tra email đã được đăng ký chưa
            email_exists = False
            phone_exists = False
            for user_data in USER_SIGNUP.values():
                if email and user_data.get("email") == email:
                    email_exists = True
                    break
                if phone and user_data.get("phone") == phone:
                    phone_exists = True
                    break
            
            if email_exists:
                accept = False
                error = "Email đã được đăng ký"
            elif phone_exists:
                accept = False
                error = "Số điện thoại đã được đăng ký"
            else:
                # Đăng ký thành công
                USER_SIGNUP[username] = {
                    "username": username,
                    "email": email,
                    "phone": phone
                }
                accept = True
                error = "Đăng Ký Thành Công"
                USER_SAVEDATA={
                    "username": username,
                    "password": password,
                    "email": email,
                    "phone": phone,
                    "bio": "Hello",
                    "avatar_url": "http://160.191.245.144/doanNT106/DB/USER/avatar/5.jpg",
                    "posts_user": [],
                    "count_posts": 0,
                    "count_followers": 0
                }
                try:
                    # Tạo thư mục DB/user nếu chưa tồn tại
                    user_dir = "DB/user"
                    if not os.path.exists(user_dir):
                        os.makedirs(user_dir)
                        log(f"Created directory: {user_dir}", level="INFO")
                    
                    # Lưu file user riêng
                    user_file = os.path.join(user_dir, f"{username}.json")
                    with open(user_file, 'w', encoding='utf-8') as f:
                        json.dump(USER_SAVEDATA, f, ensure_ascii=False, indent=2)
                    log(f"Saved user data to {user_file}", level="INFO")
                    
                    # Lưu signup.json
                    with open(SIGNUP_FILE, 'w', encoding='utf-8') as f:
                        json.dump(USER_SIGNUP, f, ensure_ascii=False, indent=2)
                    log(f"Saved signup data to {SIGNUP_FILE}", level="INFO")
                    
                except Exception as e:
                    log(f"Error saving user data for '{username}': {e}", level="ERROR")
                    traceback.print_exc()
    resp = {
        "action": "signup_data",
        "username": username,
        "password": password,
        "email": email,
        "phone": phone,
        "error": error,
        "accept": accept
    }
    return resp

VIOLATION_WORDS = [
    "đụ", "địt", "lồn", "cặc", "buồi", "chó", "óc chó", "đồ chó", 
    "súc vật", "đĩ", "con đĩ", "mẹ mày", "bố mày", "cmm", "dmm", 
    "vcl", "vl", "cc", "clgt", "fuck", "shit", "bitch", "ass", 
    "damn", "hell", "dick", "pussy", "motherfucker", "asshole"
]

def check_violation(text):
    """Kiểm tra nội dung có chứa từ vi phạm không"""
    if not text:
        return False, ""
    
    text_lower = text.lower()
    for word in VIOLATION_WORDS:
        if word.lower() in text_lower:
            return True, word
    return False, ""

def handle_create_post(req: Dict[str, Any]):
    global POST_ID_SEQ
    username = req.get("username") or req.get("UserName") or ""
    content = req.get("content") or req.get("Content") or ""
    image_url = req.get("image_url") or req.get("imageUrl") or ""
    video_url = req.get("video_url") or req.get("videoUrl") or ""
    username = "" if username is None else str(username)
    content = "" if content is None else str(content)
    image_url = "" if image_url is None else str(image_url)
    video_url = "" if video_url is None else str(video_url)

    # Kiểm tra từ vi phạm
    has_violation, violation_word = check_violation(content)
    if has_violation:
        return {
            "action": "post_data",
            "username": username,
            "id": 0,
            "content": content,
            "image_url": image_url,
            "video_url": video_url,
            "timestamp": str(int(time.time())),
            "enabled": False,
            "error": f"Nội dung chứa từ vi phạm: '{violation_word}'. Bài đăng không được phép.",
            "accept": False,
            "request_id": ""
        }

    # Validate video_url - chỉ cho phép YouTube hoặc IP máy chủ 160.191.245.144
    if video_url:
        is_youtube = "youtube.com" in video_url.lower() or "youtu.be" in video_url.lower()
        is_server = "160.191.245.144" in video_url
        
        if not (is_youtube or is_server):
            return {
                "action": "post_data",
                "username": username,
                "id": 0,
                "content": content,
                "image_url": image_url,
                "video_url": video_url,
                "timestamp": str(int(time.time())),
                "enabled": False,
                "error": "Video URL không hợp lệ. Chỉ chấp nhận link YouTube hoặc máy chủ 160.191.245.144",
                "accept": False,
                "request_id": ""
            }

    with LOCK:
        pid = POST_ID_SEQ
        POST_ID_SEQ += 1
        
        # Tạo post mới với enabled = True (hiển thị)
        post = {
            "action": "post_data",
            "username": username,
            "id": pid,
            "content": content,
            "image_url": image_url,
            "video_url": video_url,
            "timestamp": str(int(time.time())),
            "enabled": True,
            "like_count": 0,  # ← THÊM like_count
            "comment_count": 0,  # ← THÊM comment_count
            "error": "",
            "accept": True,
            "request_id": ""
        }
        
        # ========================================
        # THÊM VÀO CUỐI POSTS (không phải đầu)
        # ========================================
        POSTS.append(post)  # ← Đổi từ insert(0) thành append()
        log(f"Created new post with id={pid} by user '{username}'", level="INFO")
        
        # ========================================
        # GHI NGAY VÀO posts.json
        # ========================================
        try:
            if not os.path.exists(DB_DIR):
                os.makedirs(DB_DIR)
            
            with open(POSTS_FILE, 'w', encoding='utf-8') as f:
                json.dump(POSTS, f, ensure_ascii=False, indent=2)
            log(f"Saved posts.json immediately after creating post {pid}", level="INFO")
        except Exception as e:
            log(f"Error saving posts.json: {e}", level="ERROR")
            traceback.print_exc()
        
        # ========================================
        # CẬP NHẬT VÀ GHI NGAY VÀO FILE USER
        # ========================================
        user_dir = "DB/user"
        user_file = os.path.join(user_dir, f"{username}.json")
        
        if os.path.exists(user_file):
            try:
                # Đọc dữ liệu user hiện tại
                with open(user_file, 'r', encoding='utf-8') as f:
                    user_data = json.load(f)
                
                # Migrate posts sang posts_user nếu cần
                if "posts" in user_data and "posts_user" not in user_data:
                    user_data["posts_user"] = user_data.pop("posts")
                
                # Đảm bảo có posts_user (dạng list)
                if "posts_user" not in user_data:
                    user_data["posts_user"] = []
                
                # ========================================
                # THÊM TOÀN BỘ THÔNG TIN POST VÀO posts_user
                # ========================================
                post_info = {
                    "id": pid,
                    "content": content,
                    "image_url": image_url,
                    "video_url": video_url,
                    "timestamp": post["timestamp"],
                    "enabled": True
                }
                
                # Kiểm tra xem post_id đã tồn tại chưa
                existing_post = next((p for p in user_data["posts_user"] if isinstance(p, dict) and p.get("id") == pid), None)
                if not existing_post:
                    user_data["posts_user"].append(post_info)  # ← Thêm vào cuối
                
                # Cập nhật count_posts
                user_data["count_posts"] = len(user_data["posts_user"])
                
                # GHI NGAY VÀO FILE USER
                with open(user_file, 'w', encoding='utf-8') as f:
                    json.dump(user_data, f, ensure_ascii=False, indent=2)
                
                log(f"Updated user data for '{username}' immediately: added post {pid}, total posts: {user_data['count_posts']}", level="INFO")
                
            except Exception as e:
                log(f"Error updating user data for '{username}': {e}", level="ERROR")
                traceback.print_exc()
        else:
            log(f"User file not found: {user_file}, post created but user data not updated", level="WARN")
    
    return post

def handle_get_feed(req: Dict[str, Any]):
    """Trả về posts theo yêu cầu - mặc định 10 posts gần nhất, hoặc tất cả nếu all=true"""
    limit = req.get("limit") or 10
    get_all = req.get("all") or req.get("All") or False
    show_disabled = req.get("show_disabled") or False  # ← Thêm tùy chọn hiển thị post bị ẩn
    
    # Chuyển đổi limit sang int
    try:
        limit = int(limit)
    except:
        limit = 10
    
    with LOCK:
        # Sắp xếp POSTS theo id giảm dần (ID lớn nhất → nhỏ nhất)
        sorted_posts = sorted(POSTS, key=lambda p: p.get("id", 0), reverse=True)
        
        # Lọc post theo enabled (chỉ hiển thị post có enabled=True)
        if not show_disabled:
            sorted_posts = [p for p in sorted_posts if p.get("enabled", True)]
        
        # Cập nhật like_count và comment_count cho mỗi post
        for post in sorted_posts:
            pid = str(post.get("id"))
            post["like_count"] = len(LIKES.get(pid, []))
            post["comment_count"] = len(COMMENTS.get(pid, []))
        
        if get_all:
            # Trả về tất cả posts
            recent_posts = sorted_posts
            log(f"Sending ALL {len(recent_posts)} posts to client", level="INFO")
        else:
            # Lấy số lượng posts theo limit
            recent_posts = sorted_posts[:limit] if len(sorted_posts) >= limit else sorted_posts[:]
            log(f"Sending {len(recent_posts)} recent posts (limit={limit}) to client", level="INFO")
    
    # Trả về mảng posts
    resp = {
        "action": "get_feed",
        "posts": recent_posts,
        "count": len(recent_posts),
        "accept": True,
        "error": ""
    }
    return resp

def handle_previous_post_click(req: Dict[str, Any]):
    post_id = req.get("post_id") or req.get("PostId") or req.get("id")
    chosen = None
    with LOCK:
        if post_id is None and POSTS:
            chosen = POSTS[0]
        else:
            pid = str(post_id)
            for p in POSTS:
                if str(p.get("id")) == pid:
                    chosen = p
                    break
    if chosen is None:
        return {
            "action": "previous_post_click",
            "username": "",
            "post_id": post_id or "",
            "content": "",
            "image_url": "",
            "video_url": "",
            "timestamp": "",
            "enabled": False,
            "accept": False
        }
    resp = {
        "action": "previous_post_click",
        "username": chosen.get("username", ""),
        "post_id": str(chosen.get("id", "")),
        "content": chosen.get("content", ""),
        "image_url": chosen.get("image_url", ""),
        "video_url": chosen.get("video_url", ""),
        "timestamp": chosen.get("timestamp", ""),
        "enabled": chosen.get("enabled", True),  # ← Thêm enabled
        "accept": True
    }
    return resp

def handle_comment_previous_post_click(req: Dict[str, Any]):
    post_id = req.get("post_id") or req.get("PostId") or req.get("postId")
    username = req.get("username") or req.get("UserName") or ""
    comment_content = req.get("comment_content") or req.get("commentContent") or req.get("content") or ""
    if post_id is None:
        return {
            "action": "comment_previous_post_click",
            "username": username,
            "post_id": "",
            "cmt_id": "",
            "comment_content": comment_content,
            "timestamp": "",
            "accept": False
        }
    with LOCK:
        lst = COMMENTS.setdefault(str(post_id), [])
        cmt_id = len(lst) + 1
        c = {
            "action": "comment_previous_post_click",
            "username": username,
            "post_id": str(post_id),
            "cmt_id": str(cmt_id),
            "comment_content": comment_content,
            "timestamp": str(int(time.time())),
            "accept": True
        }
        lst.append(c)
    return c

def handle_update_user_avatar(req: Dict[str, Any]):
    """Cập nhật avatar_url của user"""
    username = req.get("username") or req.get("UserName") or ""
    new_avatar_url = req.get("avatar_url") or req.get("avatarUrl") or ""
    username = "" if username is None else str(username)
    new_avatar_url = "" if new_avatar_url is None else str(new_avatar_url)
    
    if not username:
        return {
            "action": "update_user_avatar",
            "username": "",
            "avatar_url": "",
            "error": "Username không được để trống",
            "accept": False
        }
    
    if not new_avatar_url:
        return {
            "action": "update_user_avatar",
            "username": username,
            "avatar_url": "",
            "error": "Avatar URL không được để trống",
            "accept": False
        }
    
    with LOCK:
        user_dir = "DB/user"
        user_file = os.path.join(user_dir, f"{username}.json")
        
        # Kiểm tra file có tồn tại không
        if not os.path.exists(user_file):
            log(f"User file not found: {user_file}", level="WARN")
            return {
                "action": "update_user_avatar",
                "username": username,
                "avatar_url": "",
                "error": "User không tồn tại",
                "accept": False
            }
        
        try:
            # Đọc dữ liệu user hiện tại
            with open(user_file, 'r', encoding='utf-8') as f:
                user_data = json.load(f)
            
            # Lưu giá trị cũ để log
            old_avatar_url = user_data.get("avatar_url", "")
            
            # Cập nhật avatar_url
            user_data["avatar_url"] = new_avatar_url
            
            # GHI NGAY VÀO FILE USER
            with open(user_file, 'w', encoding='utf-8') as f:
                json.dump(user_data, f, ensure_ascii=False, indent=2)
            
            log(f"Updated avatar_url for '{username}': '{old_avatar_url}' -> '{new_avatar_url}'", level="INFO")
            
            return {
                "action": "update_user_avatar",
                "username": username,
                "avatar_url": new_avatar_url,
                "error": "Cập nhật avatar thành công",
                "accept": True
            }
            
        except Exception as e:
            log(f"Error updating avatar for '{username}': {e}", level="ERROR")
            traceback.print_exc()
            return {
                "action": "update_user_avatar",
                "username": username,
                "avatar_url": "",
                "error": f"Lỗi cập nhật avatar: {str(e)}",
                "accept": False
            }

def handle_like_post(req: Dict[str, Any]):
    """Thêm/bỏ like cho bài đăng"""
    post_id = req.get("post_id") or req.get("PostId") or req.get("id")
    username = req.get("username") or req.get("UserName") or ""
    
    if not post_id:
        return {
            "action": "like_post",
            "post_id": "",
            "username": username,
            "liked": False,
            "like_count": 0,
            "error": "Post ID không được để trống",
            "accept": False
        }
    
    if not username:
        return {
            "action": "like_post",
            "post_id": str(post_id),
            "username": "",
            "liked": False,
            "like_count": 0,
            "error": "Username không được để trống",
            "accept": False
        }
    
    with LOCK:
        pid = str(post_id)
        
        # Tìm post trong POSTS
        post = None
        for p in POSTS:
            if str(p.get("id")) == pid:
                post = p
                break
        
        if not post:
            return {
                "action": "like_post",
                "post_id": pid,
                "username": username,
                "liked": False,
                "like_count": 0,
                "error": "Bài đăng không tồn tại",
                "accept": False
            }
        
        # Khởi tạo danh sách like cho post nếu chưa có
        if pid not in LIKES:
            LIKES[pid] = []
        
        # Toggle like
        if username in LIKES[pid]:
            # Bỏ like
            LIKES[pid].remove(username)
            liked = False
            log(f"User '{username}' unliked post {pid}", level="INFO")
        else:
            # Thêm like
            LIKES[pid].append(username)
            liked = True
            log(f"User '{username}' liked post {pid}", level="INFO")
        
        # Cập nhật like_count trong post
        like_count = len(LIKES[pid])
        post["like_count"] = like_count
        
        # Lưu ngay vào file
        try:
            with open(LIKES_FILE, 'w', encoding='utf-8') as f:
                json.dump(LIKES, f, ensure_ascii=False, indent=2)
            
            with open(POSTS_FILE, 'w', encoding='utf-8') as f:
                json.dump(POSTS, f, ensure_ascii=False, indent=2)
            
            log(f"Saved likes and posts after like action", level="INFO")
        except Exception as e:
            log(f"Error saving likes: {e}", level="ERROR")
        
        return {
            "action": "like_post",
            "post_id": pid,
            "username": username,
            "liked": liked,
            "like_count": like_count,
            "error": "Thành công" if liked else "Đã bỏ like",
            "accept": True
        }

def handle_add_comment(req: Dict[str, Any]):
    """Thêm comment vào bài đăng"""
    post_id = req.get("post_id") or req.get("PostId") or req.get("id")
    username = req.get("username") or req.get("UserName") or ""
    content = req.get("content") or req.get("comment_content") or req.get("commentContent") or ""
    
    if not post_id:
        return {
            "action": "add_comment",
            "post_id": "",
            "username": username,
            "comment_id": "",
            "content": content,
            "timestamp": "",
            "error": "Post ID không được để trống",
            "accept": False
        }
    
    if not username:
        return {
            "action": "add_comment",
            "post_id": str(post_id),
            "username": "",
            "comment_id": "",
            "content": content,
            "timestamp": "",
            "error": "Username không được để trống",
            "accept": False
        }
    
    if not content:
        return {
            "action": "add_comment",
            "post_id": str(post_id),
            "username": username,
            "comment_id": "",
            "content": "",
            "timestamp": "",
            "error": "Nội dung comment không được để trống",
            "accept": False
        }
    
    with LOCK:
        pid = str(post_id)
        
        # Tìm post trong POSTS
        post = None
        for p in POSTS:
            if str(p.get("id")) == pid:
                post = p
                break
        
        if not post:
            return {
                "action": "add_comment",
                "post_id": pid,
                "username": username,
                "comment_id": "",
                "content": content,
                "timestamp": "",
                "error": "Bài đăng không tồn tại",
                "accept": False
            }
        
        # Khởi tạo danh sách comment cho post nếu chưa có
        if pid not in COMMENTS:
            COMMENTS[pid] = []
        
        # Tạo comment mới
        comment_id = len(COMMENTS[pid]) + 1
        timestamp = str(int(time.time()))
        
        comment = {
            "comment_id": str(comment_id),
            "username": username,
            "content": content,
            "timestamp": timestamp
        }
        
        COMMENTS[pid].append(comment)
        
        # Cập nhật comment_count trong post
        comment_count = len(COMMENTS[pid])
        post["comment_count"] = comment_count
        
        log(f"User '{username}' commented on post {pid}: '{content}'", level="INFO")
        
        # Lưu ngay vào file
        try:
            with open(COMMENTS_FILE, 'w', encoding='utf-8') as f:
                json.dump(COMMENTS, f, ensure_ascii=False, indent=2)
            
            with open(POSTS_FILE, 'w', encoding='utf-8') as f:
                json.dump(POSTS, f, ensure_ascii=False, indent=2)
            
            log(f"Saved comments and posts after add comment", level="INFO")
        except Exception as e:
            log(f"Error saving comments: {e}", level="ERROR")
        
        return {
            "action": "add_comment",
            "post_id": pid,
            "username": username,
            "comment_id": str(comment_id),
            "content": content,
            "timestamp": timestamp,
            "comment_count": comment_count,
            "error": "Thêm comment thành công",
            "accept": True
        }

def handle_get_comments(req: Dict[str, Any]):
    """Lấy danh sách comments của một bài đăng"""
    post_id = req.get("post_id") or req.get("PostId") or req.get("id")
    
    if not post_id:
        return {
            "action": "get_comments",
            "post_id": "",
            "comments": [],
            "count": 0,
            "error": "Post ID không được để trống",
            "accept": False
        }
    
    with LOCK:
        pid = str(post_id)
        
        # Lấy comments của post
        comments = COMMENTS.get(pid, [])
        
        log(f"Loaded {len(comments)} comments for post {pid}", level="INFO")
        
        return {
            "action": "get_comments",
            "post_id": pid,
            "comments": comments,
            "count": len(comments),
            "error": "",
            "accept": True
        }

def handle_get_user_info(req: Dict[str, Any]):
    """Trả về thông tin user từ file DB/user/{username}.json"""
    username = req.get("username") or req.get("UserName") or req.get("user") or ""
    username = "" if username is None else str(username)
    
    if not username:
        return {
            "action": "get_user_info",
            "username": "",
            "password": "",
            "email": "",
            "phone": "",
            "bio": "",
            "avatar_url": "",
            "posts_user": [],
            "count_posts": 0,
            "count_followers": 0,
            "error": "Username không được để trống",
            "accept": False
        }
    
    with LOCK:
        user_dir = "DB/user"
        user_file = os.path.join(user_dir, f"{username}.json")
        
        # Kiểm tra file có tồn tại không
        if not os.path.exists(user_file):
            log(f"User file not found: {user_file}", level="WARN")
            return {
                "action": "get_user_info",
                "username": username,
                "password": "",
                "email": "",
                "phone": "",
                "bio": "",
                "avatar_url": "",
                "posts_user": [],
                "count_posts": 0,
                "count_followers": 0,
                "error": "User không tồn tại",
                "accept": False
            }
        
        try:
            # Đọc dữ liệu user từ file
            with open(user_file, 'r', encoding='utf-8') as f:
                user_data = json.load(f)
            
            # Kiểm tra và migrate từ "posts" sang "posts_user" nếu cần
            if "posts" in user_data and "posts_user" not in user_data:
                user_data["posts_user"] = user_data.pop("posts")
                log(f"Migrated 'posts' to 'posts_user' for {username}", level="INFO")
            
            # Kiểm tra xem có count_followers chưa, nếu chưa thì thêm vào
            if "count_followers" not in user_data:
                user_data["count_followers"] = 0
                log(f"Added count_followers to {user_file}", level="INFO")
            
            # Đảm bảo có posts_user
            if "posts_user" not in user_data:
                user_data["posts_user"] = []
                log(f"Added posts_user to {user_file}", level="INFO")
            
            # Lưu lại file nếu có thay đổi
            if "posts" in user_data or "count_followers" not in user_data or "posts_user" not in user_data:
                with open(user_file, 'w', encoding='utf-8') as f:
                    json.dump(user_data, f, ensure_ascii=False, indent=2)
                log(f"Updated user file {user_file}", level="INFO")
            
            log(f"Loaded user info for '{username}'", level="INFO")
            
            # Trả về thông tin user theo đúng thứ tự
            resp = {
                "action": "get_user_info",
                "username": user_data.get("username", ""),
                "password": user_data.get("password", ""),
                "email": user_data.get("email", ""),
                "phone": user_data.get("phone", ""),
                "bio": user_data.get("bio", ""),
                "avatar_url": user_data.get("avatar_url", ""),
                "posts_user": user_data.get("posts_user", []),
                "count_posts": user_data.get("count_posts", 0),
                "count_followers": user_data.get("count_followers", 0),
                "error": "",
                "accept": True
            }
            return resp
            
        except Exception as e:
            log(f"Error loading user data for '{username}': {e}", level="ERROR")
            traceback.print_exc()
            return {
                "action": "get_user_info",
                "username": username,
                "password": "",
                "email": "",
                "phone": "",
                "bio": "",
                "avatar_url": "",
                "posts_user": [],
                "count_posts": 0,
                "count_followers": 0,
                "error": f"Lỗi đọc dữ liệu user: {str(e)}",
                "accept": False
            }

ACTION_HANDLERS = {
    "login": handle_login_data,
    "login_data": handle_login_data,
    "signup": handle_signup_data,
    "signup_data": handle_signup_data,
    "createpost": handle_create_post,
    "create_post": handle_create_post,
    "post_data": handle_create_post,
    "getfeed": handle_get_feed,
    "get_feed": handle_get_feed,
    "getuserinfo": handle_get_user_info,
    "get_user_info": handle_get_user_info,
    "user_info": handle_get_user_info,
    "updateuseravatar": handle_update_user_avatar,
    "update_user_avatar": handle_update_user_avatar,
    "likepost": handle_like_post,
    "like_post": handle_like_post,
    "addcomment": handle_add_comment,
    "add_comment": handle_add_comment,
    "getcomments": handle_get_comments,
    "get_comments": handle_get_comments,
    "previouspostclick": handle_previous_post_click,
    "previous_post_click": handle_previous_post_click,
    "commentpreviouspostclick": handle_comment_previous_post_click,
    "comment_previous_post_click": handle_comment_previous_post_click,
}

def process_request(jobj: Dict[str, Any], addr=None):
    action = jobj.get("action") or jobj.get("Action") or jobj.get("ActionName")
    if not action:
        log(f"Missing action in request: {pretty(jobj)}", level="WARN", addr=addr if addr else None)
        return {"action": "error", "message": "missing action", "accept": False}

    akey = str(action).strip().lower()
    handler = ACTION_HANDLERS.get(akey)
    if not handler:
        log(f"Unknown action '{action}' from client. Full request: {pretty(jobj)}", level="WARN", addr=addr if addr else None)
        return {"action": "error", "message": f"unknown action: {action}", "accept": False}

    try:
        log(f"Calling handler for action '{action}'", level="INFO", addr=addr if addr else None)
        resp = handler(jobj)
        if not isinstance(resp, dict):
            log(f"Handler for {action} returned non-dict: {resp}", level="ERROR", addr=addr if addr else None)
            return {"action": "error", "message": "handler error", "accept": False}
        return resp
    except Exception as ex:
        traceback.print_exc()
        log(f"Exception in handler for {action}: {ex}", level="ERROR", addr=addr if addr else None)
        return {"action": "error", "message": str(ex), "accept": False}

def client_thread(conn: socket.socket, addr):
    thr = threading.current_thread().name
    log(f"New client connected: {addr}", level="INFO")
    try:
        with conn:
            # IMPORTANT: use encoding 'utf-8-sig' so BOM is removed when reading
            f = conn.makefile(mode="rw", encoding=ENC, newline="\n")

            while True:
                line = f.readline()
                if not line:
                    log(f"Client disconnected: {addr}", level="INFO")
                    break

                raw = line.rstrip("\n")
                log(f"Received raw line from {addr}: {raw}", level="DEBUG")
                # Extra check: remove any BOM char if still present
                if raw.startswith('\ufeff'):
                    log(f"Detected BOM at start of line from {addr}; stripping it", level="INFO")
                    raw = raw.lstrip('\ufeff')

                if raw.strip() == "":
                    log("Received empty/blank line, skipping", level="DEBUG")
                    continue

                try:
                    jobj = json.loads(raw)
                    log(f"Parsed JSON from {addr}:\n{pretty(jobj)}", level="INFO")
                except Exception as e:
                    log(f"JSON PARSE ERROR: {e} -- raw: {raw}", level="WARN")
                    send_json_fileobj(f, {"action": "error", "message": "invalid json", "accept": False}, addr=addr)
                    continue

                resp_obj = process_request(jobj, addr=addr)
                # copy request_id if provided
                rid = jobj.get("request_id") or jobj.get("RequestId") or jobj.get("requestId")
                if rid is not None:
                    resp_obj["request_id"] = "ServerHaha_"+rid

                log(f"Responding to {addr} with:\n{pretty(resp_obj)}", level="INFO")
                send_json_fileobj(f, resp_obj, addr=addr)

    except Exception as e:
        log(f"Exception with client {addr}: {e}", level="ERROR")
        traceback.print_exc()
    log(f"Thread exiting for {addr}", level="INFO")

def signal_handler(signum, frame):
    """Handle shutdown signals"""
    log(f"Received signal {signum}, shutting down...", level="INFO")
    save_all_data()
    sys.exit(0)

def start_server():
    # Register cleanup handlers
    atexit.register(save_all_data)
    signal.signal(signal.SIGINT, signal_handler)
    signal.signal(signal.SIGTERM, signal_handler)
    
    # truncate log file on start
    if WRITE_LOG_FILE:
        try:
            with open(LOG_FILENAME, "w", encoding="utf-8") as f:
                f.write("")  # clear
        except Exception:
            pass

    log(f"Listening on {HOST}:{PORT}", level="INFO")
    log(f"Data will be auto-saved to {DB_DIR}/ on shutdown", level="INFO")
    
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    s.bind((HOST, PORT))
    s.listen(100)
    try:
        while True:
            conn, addr = s.accept()
            t = threading.Thread(target=client_thread, args=(conn, addr), daemon=True)
            t.start()
    except KeyboardInterrupt:
        log("Shutting down (KeyboardInterrupt)", level="INFO")
    finally:
        s.close()
        save_all_data()

if __name__ == "__main__":
    start_server()