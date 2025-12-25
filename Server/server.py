#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
server.py
Verbose TCP JSON server (BOM-safe) with SQLite Database.
- Accepts line-delimited JSON (each JSON ends with '\n').
- Uses encoding 'utf-8-sig' for makefile so BOM is removed on read.
- Also strips any leading U+FEFF before json.loads as extra safety.
- Responds in the same JSON shapes your C# client expects (login_data, post_data, etc.).
- Uses SQLite database for persistent storage.
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
import random
import sqlite3
from typing import Dict, Any, Optional

HOST = "0.0.0.0"
PORT = 13579
ENC = "utf-8-sig"

def get_local_ip():
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        s.connect(("8.8.8.8", 80))
        local_ip = s.getsockname()[0]
        s.close()
        return local_ip
    except Exception:
        return "127.0.0.1"

IP_INPUT = get_local_ip()
print(f"Server IP: {IP_INPUT}")

VERBOSE_TO_CONSOLE = True
WRITE_LOG_FILE = True
LOG_FILENAME = "server_verbose.log"

# Database configuration
DB_DIR = "DB"
DB_PATH = os.path.join(DB_DIR, "postez.db")

# ============================================
# LOGGING UTILITIES
# ============================================
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

def send_json_fileobj(f, obj: Dict[str, Any], addr=None):
    try:
        s = json.dumps(obj, ensure_ascii=False)
        f.write(s + "\n")
        f.flush()
        log(f"Sent JSON to client: {s}", level="DEBUG", addr=addr)
    except Exception as e:
        log(f"Failed to send to client: {e}", level="ERROR", addr=addr)

# ============================================
# DATABASE CONNECTION AND INITIALIZATION
# ============================================

def get_db_connection():
    """Tạo kết nối đến SQLite database"""
    conn = sqlite3.connect(DB_PATH, check_same_thread=False)
    conn.row_factory = sqlite3.Row
    return conn

def init_database():
    """Khởi tạo database và các bảng cần thiết"""
    if not os.path.exists(DB_DIR):
        os.makedirs(DB_DIR)
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    # Bảng users
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS users (
            username TEXT PRIMARY KEY,
            password TEXT NOT NULL,
            email TEXT UNIQUE NOT NULL,
            phone TEXT,
            bio TEXT DEFAULT 'Hello',
            avatar_url TEXT,
            count_posts INTEGER DEFAULT 0,
            count_followers INTEGER DEFAULT 0,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
        )
    ''')
    
    # Bảng posts
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS posts (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT NOT NULL,
            content TEXT NOT NULL,
            image_url TEXT,
            video_url TEXT,
            enabled BOOLEAN DEFAULT 1,
            timestamp TEXT NOT NULL,
            like_count INTEGER DEFAULT 0,
            comment_count INTEGER DEFAULT 0,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (username) REFERENCES users(username)
        )
    ''')
    
    # Bảng comments
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS comments (
            comment_id INTEGER PRIMARY KEY AUTOINCREMENT,
            post_id INTEGER NOT NULL,
            username TEXT NOT NULL,
            content TEXT NOT NULL,
            timestamp TEXT NOT NULL,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (post_id) REFERENCES posts(id) ON DELETE CASCADE,
            FOREIGN KEY (username) REFERENCES users(username)
        )
    ''')
    
    # Bảng likes
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS likes (
            like_id INTEGER PRIMARY KEY AUTOINCREMENT,
            post_id INTEGER NOT NULL,
            username TEXT NOT NULL,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            UNIQUE(post_id, username),
            FOREIGN KEY (post_id) REFERENCES posts(id) ON DELETE CASCADE,
            FOREIGN KEY (username) REFERENCES users(username)
        )
    ''')
    
    # Bảng messages
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS messages (
            message_id INTEGER PRIMARY KEY AUTOINCREMENT,
            from_user TEXT NOT NULL,
            to_user TEXT NOT NULL,
            content TEXT NOT NULL,
            timestamp TEXT NOT NULL,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (from_user) REFERENCES users(username),
            FOREIGN KEY (to_user) REFERENCES users(username)
        )
    ''')
    
    conn.commit()
    conn.close()
    log("Database initialized successfully", level="INFO")

# Initialize database on startup
init_database()

# ============================================
# SESSION MANAGEMENT
# ============================================
ACTIVE_SESSIONS = {}
SESSION_LOCK = threading.Lock()

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

def send_json_fileobj(f, obj: Dict[str, Any], addr=None):
    try:
        s = json.dumps(obj, ensure_ascii=False)
        f.write(s + "\n")
        f.flush()
        log(f"Sent JSON to client: {s}", level="DEBUG", addr=addr)
    except Exception as e:
        log(f"Failed to send to client: {e}", level="ERROR", addr=addr)

# ============================================
# DATABASE CONNECTION AND INITIALIZATION
# ============================================

def get_db_connection():
    """Tạo kết nối đến SQLite database"""
    conn = sqlite3.connect(DB_PATH, check_same_thread=False)
    conn.row_factory = sqlite3.Row
    return conn

def init_database():
    """Khởi tạo database và các bảng cần thiết"""
    if not os.path.exists(DB_DIR):
        os.makedirs(DB_DIR)
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    # Bảng users
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS users (
            username TEXT PRIMARY KEY,
            password TEXT NOT NULL,
            email TEXT UNIQUE NOT NULL,
            phone TEXT,
            bio TEXT DEFAULT 'Hello',
            avatar_url TEXT,
            count_posts INTEGER DEFAULT 0,
            count_followers INTEGER DEFAULT 0,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
        )
    ''')
    
    # Bảng posts
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS posts (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT NOT NULL,
            content TEXT NOT NULL,
            image_url TEXT,
            video_url TEXT,
            enabled BOOLEAN DEFAULT 1,
            timestamp TEXT NOT NULL,
            like_count INTEGER DEFAULT 0,
            comment_count INTEGER DEFAULT 0,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (username) REFERENCES users(username)
        )
    ''')
    
    # Bảng comments
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS comments (
            comment_id INTEGER PRIMARY KEY AUTOINCREMENT,
            post_id INTEGER NOT NULL,
            username TEXT NOT NULL,
            content TEXT NOT NULL,
            timestamp TEXT NOT NULL,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (post_id) REFERENCES posts(id) ON DELETE CASCADE,
            FOREIGN KEY (username) REFERENCES users(username)
        )
    ''')
    
    # Bảng likes
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS likes (
            like_id INTEGER PRIMARY KEY AUTOINCREMENT,
            post_id INTEGER NOT NULL,
            username TEXT NOT NULL,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            UNIQUE(post_id, username),
            FOREIGN KEY (post_id) REFERENCES posts(id) ON DELETE CASCADE,
            FOREIGN KEY (username) REFERENCES users(username)
        )
    ''')
    
    # Bảng messages
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS messages (
            message_id INTEGER PRIMARY KEY AUTOINCREMENT,
            from_user TEXT NOT NULL,
            to_user TEXT NOT NULL,
            content TEXT NOT NULL,
            timestamp TEXT NOT NULL,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (from_user) REFERENCES users(username),
            FOREIGN KEY (to_user) REFERENCES users(username)
        )
    ''')
    
    conn.commit()
    conn.close()
    log("Database initialized successfully", level="INFO")

# Initialize database on startup
init_database()

# ============================================
# SESSION MANAGEMENT
# ============================================
ACTIVE_SESSIONS = {}
SESSION_LOCK = threading.Lock()

# LOCK = threading.Lock()
# _log_lock = threading.Lock()

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

def send_json_fileobj(f, obj: Dict[str, Any], addr=None):
    try:
        s = json.dumps(obj, ensure_ascii=False)
        f.write(s + "\n")
        f.flush()
        log(f"Sent JSON to client: {s}", level="DEBUG", addr=addr)
    except Exception as e:
        log(f"Failed to send to client: {e}", level="ERROR", addr=addr)

# ============================================
# DATABASE HELPER FUNCTIONS
# ============================================

def dict_from_row(row):
    """Chuyển sqlite3.Row thành dictionary"""
    if row is None:
        return None
    return {key: row[key] for key in row.keys()}

# ============================================
# HANDLERS WITH SQLITE
# ============================================

def handle_login_data(req: Dict[str, Any], addr=None):
    """Handle login với SQLite database"""
    username = req.get("username") or req.get("UserName") or req.get("User") or ""
    request_id = req.get("request_id") or ""
    password = req.get("password") or req.get("Password") or req.get("Pass") or ""
    username = "" if username is None else str(username)
    password = "" if password is None else str(password)
    error = "NO"
    
    log(f"Login attempt: username='{username}'", level="INFO")
    
    # Admin account bypass
    if username == "Admin123" and password == "Admin123!":
        with SESSION_LOCK:
            if username in ACTIVE_SESSIONS:
                existing_session = ACTIVE_SESSIONS[username]
                error = f"⚠️ Tài khoản đang được đăng nhập từ {existing_session['addr']}"
                accept = False
            else:
                ACTIVE_SESSIONS[username] = {
                    "addr": str(addr) if addr else "unknown",
                    "login_time": time.time()
                }
                error = "Đăng Nhập Thành Công"
                accept = True
        
        return {
            "action": "login_data",
            "username": username,
            "password": password,
            "error": error,
            "accept": accept,
            "request_id": f"ServerHaha_{request_id}"
        }
    
    # Check database
    conn = get_db_connection()
    cursor = conn.cursor()
    
    cursor.execute('SELECT * FROM users WHERE username = ? AND password = ?', 
                   (username, password))
    user = cursor.fetchone()
    conn.close()
    
    if not user:
        error = "Tên đăng nhập hoặc mật khẩu không đúng"
        accept = False
    else:
        # Check session
        with SESSION_LOCK:
            if username in ACTIVE_SESSIONS:
                existing_session = ACTIVE_SESSIONS[username]
                log(f"User '{username}' already logged in from {existing_session['addr']}", level="WARN")
                error = f"⚠️ Tài khoản đang được đăng nhập từ {existing_session['addr']}"
                accept = False
            else:
                ACTIVE_SESSIONS[username] = {
                    "addr": str(addr) if addr else "unknown",
                    "login_time": time.time()
                }
                log(f"User '{username}' logged in successfully", level="INFO")
                error = "Đăng Nhập Thành Công"
                accept = True
    
    return {
        "action": "login_data",
        "username": username,
        "password": password,
        "error": error,
        "accept": accept,
        "request_id": f"ServerHaha_{request_id}"
    }

def handle_logout(req: Dict[str, Any]):
    """Xử lý logout - xóa session"""
    username = req.get("username") or req.get("UserName") or ""
    
    with SESSION_LOCK:
        if username in ACTIVE_SESSIONS:
            del ACTIVE_SESSIONS[username]
            log(f"User '{username}' logged out", level="INFO")
            return {
                "action": "logout",
                "username": username,
                "error": "Đăng xuất thành công",
                "accept": True
            }
        else:
            return {
                "action": "logout",
                "username": username,
                "error": "Session không tồn tại",
                "accept": False
            }

def handle_signup_data(req: Dict[str, Any]):
    """Đăng ký user mới vào SQLite"""
    username = req.get("username") or req.get("UserName") or ""
    password = req.get("password") or req.get("Password") or ""
    email = req.get("email") or req.get("Email") or ""
    phone = req.get("phone") or req.get("Phone") or ""
    
    username = "" if username is None else str(username)
    password = "" if password is None else str(password)
    email = "" if email is None else str(email)
    phone = "" if phone is None else str(phone)
    
    log(f"Signup attempt: username='{username}', email='{email}'", level="INFO")
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    # Kiểm tra username đã tồn tại
    cursor.execute('SELECT username FROM users WHERE username = ?', (username,))
    if cursor.fetchone():
        conn.close()
        return {
            "action": "signup_data",
            "username": username,
            "password": password,
            "email": email,
            "phone": phone,
            "error": "Tên đăng nhập đã tồn tại",
            "accept": False
        }
    
    # Kiểm tra email đã tồn tại
    cursor.execute('SELECT username FROM users WHERE email = ?', (email,))
    if cursor.fetchone():
        conn.close()
        return {
            "action": "signup_data",
            "username": username,
            "password": password,
            "email": email,
            "phone": phone,
            "error": "Email đã được đăng ký",
            "accept": False
        }
    
    # Kiểm tra phone đã tồn tại
    if phone:
        cursor.execute('SELECT username FROM users WHERE phone = ?', (phone,))
        if cursor.fetchone():
            conn.close()
            return {
                "action": "signup_data",
                "username": username,
                "password": password,
                "email": email,
                "phone": phone,
                "error": "Số điện thoại đã được đăng ký",
                "accept": False
            }
    
    # Tạo avatar ngẫu nhiên
    random_avatar_num = random.randint(1, 5)
    avatar_url = f"http://{IP_INPUT}/doanNT106/DB/USER/avatar/{random_avatar_num}.jpg"
    
    # Insert user mới
    try:
        cursor.execute('''
            INSERT INTO users (username, password, email, phone, bio, avatar_url)
            VALUES (?, ?, ?, ?, ?, ?)
        ''', (username, password, email, phone, "Hello", avatar_url))
        
        conn.commit()
        conn.close()
        
        log(f"User '{username}' registered successfully", level="INFO")
        
        return {
            "action": "signup_data",
            "username": username,
            "password": password,
            "email": email,
            "phone": phone,
            "error": "Đăng Ký Thành Công",
            "accept": True
        }
    except Exception as e:
        conn.close()
        log(f"Error registering user '{username}': {e}", level="ERROR")
        return {
            "action": "signup_data",
            "username": username,
            "password": password,
            "email": email,
            "phone": phone,
            "error": f"Lỗi đăng ký: {str(e)}",
            "accept": False
        }

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
    """Tạo bài đăng mới trong SQLite"""
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
            "error": f"Nội dung chứa từ vi phạm: '{violation_word}'",
            "accept": False,
            "request_id": ""
        }
    
    # Validate video URL
    if video_url:
        is_youtube = "youtube.com" in video_url.lower() or "youtu.be" in video_url.lower()
        is_server = IP_INPUT in video_url
        
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
                "error": f"Video URL không hợp lệ",
                "accept": False,
                "request_id": ""
            }
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    timestamp = str(int(time.time()))
    
    try:
        cursor.execute('''
            INSERT INTO posts (username, content, image_url, video_url, timestamp, enabled)
            VALUES (?, ?, ?, ?, ?, ?)
        ''', (username, content, image_url, video_url, timestamp, 1))
        
        post_id = cursor.lastrowid
        
        # Cập nhật count_posts của user
        cursor.execute('''
            UPDATE users 
            SET count_posts = (SELECT COUNT(*) FROM posts WHERE username = ?)
            WHERE username = ?
        ''', (username, username))
        
        conn.commit()
        conn.close()
        
        log(f"Created post {post_id} by user '{username}'", level="INFO")
        
        return {
            "action": "post_data",
            "username": username,
            "id": post_id,
            "content": content,
            "image_url": image_url,
            "video_url": video_url,
            "timestamp": timestamp,
            "enabled": True,
            "like_count": 0,
            "comment_count": 0,
            "error": "",
            "accept": True,
            "request_id": ""
        }
    except Exception as e:
        conn.close()
        log(f"Error creating post: {e}", level="ERROR")
        return {
            "action": "post_data",
            "username": username,
            "id": 0,
            "content": content,
            "image_url": image_url,
            "video_url": video_url,
            "timestamp": timestamp,
            "enabled": False,
            "error": f"Lỗi tạo bài đăng: {str(e)}",
            "accept": False,
            "request_id": ""
        }

def handle_get_feed(req: Dict[str, Any]):
    """Lấy danh sách posts từ SQLite"""
    limit = req.get("limit") or 10
    get_all = req.get("all") or req.get("All") or False
    show_disabled = req.get("show_disabled") or False
    
    try:
        limit = int(limit)
    except:
        limit = 10
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    if show_disabled:
        cursor.execute('SELECT * FROM posts ORDER BY id DESC')
    else:
        cursor.execute('SELECT * FROM posts WHERE enabled = 1 ORDER BY id DESC')
    
    posts = cursor.fetchall()
    conn.close()
    
    # Chuyển đổi sang list of dicts
    posts_list = []
    for post in posts:
        post_dict = dict_from_row(post)
        posts_list.append({
            "action": "post_data",
            "username": post_dict["username"],
            "id": post_dict["id"],
            "content": post_dict["content"],
            "image_url": post_dict["image_url"] or "",
            "video_url": post_dict["video_url"] or "",
            "timestamp": post_dict["timestamp"],
            "enabled": bool(post_dict["enabled"]),
            "like_count": post_dict["like_count"],
            "comment_count": post_dict["comment_count"],
            "error": "",
            "accept": True,
            "request_id": ""
        })
    
    if not get_all:
        posts_list = posts_list[:limit]
    
    log(f"Sending {len(posts_list)} posts to client", level="INFO")
    
    return {
        "action": "get_feed",
        "posts": posts_list,
        "count": len(posts_list),
        "accept": True,
        "error": ""
    }

def handle_previous_post_click(req: Dict[str, Any]):
    """Lấy thông tin một post cụ thể"""
    post_id = req.get("post_id") or req.get("PostId") or req.get("id")
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    if post_id is None:
        cursor.execute('SELECT * FROM posts ORDER BY id DESC LIMIT 1')
    else:
        cursor.execute('SELECT * FROM posts WHERE id = ?', (post_id,))
    
    post = cursor.fetchone()
    conn.close()
    
    if not post:
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
    
    post_dict = dict_from_row(post)
    return {
        "action": "previous_post_click",
        "username": post_dict["username"],
        "post_id": str(post_dict["id"]),
        "content": post_dict["content"],
        "image_url": post_dict["image_url"] or "",
        "video_url": post_dict["video_url"] or "",
        "timestamp": post_dict["timestamp"],
        "enabled": bool(post_dict["enabled"]),
        "accept": True
    }

def handle_comment_previous_post_click(req: Dict[str, Any]):
    """Legacy handler - chuyển sang handle_add_comment"""
    return handle_add_comment(req)

def handle_like_post(req: Dict[str, Any]):
    """Thêm/bỏ like cho bài đăng"""
    post_id = req.get("post_id") or req.get("PostId") or req.get("id")
    username = req.get("username") or req.get("UserName") or ""
    
    if not post_id or not username:
        return {
            "action": "like_post",
            "post_id": str(post_id) if post_id else "",
            "username": username,
            "liked": False,
            "like_count": 0,
            "error": "Post ID hoặc username không hợp lệ",
            "accept": False
        }
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    # Kiểm tra post có tồn tại không
    cursor.execute('SELECT id FROM posts WHERE id = ?', (post_id,))
    if not cursor.fetchone():
        conn.close()
        return {
            "action": "like_post",
            "post_id": str(post_id),
            "username": username,
            "liked": False,
            "like_count": 0,
            "error": "Bài đăng không tồn tại",
            "accept": False
        }
    
    # Kiểm tra đã like chưa
    cursor.execute('SELECT like_id FROM likes WHERE post_id = ? AND username = ?', 
                   (post_id, username))
    existing_like = cursor.fetchone()
    
    if existing_like:
        # Bỏ like
        cursor.execute('DELETE FROM likes WHERE post_id = ? AND username = ?', 
                       (post_id, username))
        liked = False
        log(f"User '{username}' unliked post {post_id}", level="INFO")
    else:
        # Thêm like
        cursor.execute('INSERT INTO likes (post_id, username) VALUES (?, ?)', 
                       (post_id, username))
        liked = True
        log(f"User '{username}' liked post {post_id}", level="INFO")
    
    # Cập nhật like_count trong bảng posts
    cursor.execute('SELECT COUNT(*) as count FROM likes WHERE post_id = ?', (post_id,))
    like_count = cursor.fetchone()["count"]
    
    cursor.execute('UPDATE posts SET like_count = ? WHERE id = ?', (like_count, post_id))
    
    conn.commit()
    conn.close()
    
    return {
        "action": "like_post",
        "post_id": str(post_id),
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
            "cmt_id": "",
            "content": content,
            "comment_content": content,
            "timestamp": "",
            "error": "Post ID không hợp lệ",
            "accept": False
        }
    
    if not username:
        return {
            "action": "add_comment",
            "post_id": str(post_id),
            "username": "",
            "comment_id": "",
            "cmt_id": "",
            "content": content,
            "comment_content": content,
            "timestamp": "",
            "error": "Username không hợp lệ",
            "accept": False
        }
    
    if not content:
        return {
            "action": "add_comment",
            "post_id": str(post_id),
            "username": username,
            "comment_id": "",
            "cmt_id": "",
            "content": "",
            "comment_content": "",
            "timestamp": "",
            "error": "Nội dung comment không được để trống",
            "accept": False
        }
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    # Kiểm tra post tồn tại
    cursor.execute('SELECT id FROM posts WHERE id = ?', (post_id,))
    if not cursor.fetchone():
        conn.close()
        return {
            "action": "add_comment",
            "post_id": str(post_id),
            "username": username,
            "comment_id": "",
            "cmt_id": "",
            "content": content,
            "comment_content": content,
            "timestamp": "",
            "error": "Bài đăng không tồn tại",
            "accept": False
        }
    
    timestamp = str(int(time.time()))
    
    # Thêm comment
    cursor.execute('''
        INSERT INTO comments (post_id, username, content, timestamp)
        VALUES (?, ?, ?, ?)
    ''', (post_id, username, content, timestamp))
    
    comment_id = cursor.lastrowid
    
    # Cập nhật comment_count
    cursor.execute('SELECT COUNT(*) as count FROM comments WHERE post_id = ?', (post_id,))
    comment_count = cursor.fetchone()["count"]
    
    cursor.execute('UPDATE posts SET comment_count = ? WHERE id = ?', (comment_count, post_id))
    
    conn.commit()
    conn.close()
    
    log(f"User '{username}' commented on post {post_id}", level="INFO")
    
    return {
        "action": "add_comment",
        "post_id": str(post_id),
        "username": username,
        "comment_id": str(comment_id),
        "cmt_id": str(comment_id),
        "content": content,
        "comment_content": content,
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
            "error": "Post ID không hợp lệ",
            "accept": False
        }
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    cursor.execute('''
        SELECT comment_id, username, content, timestamp 
        FROM comments 
        WHERE post_id = ? 
        ORDER BY comment_id ASC
    ''', (post_id,))
    
    comments = cursor.fetchall()
    conn.close()
    
    comments_list = []
    for comment in comments:
        comments_list.append({
            "comment_id": str(comment["comment_id"]),
            "cmt_id": str(comment["comment_id"]),
            "username": comment["username"],
            "content": comment["content"],
            "comment_content": comment["content"],
            "timestamp": comment["timestamp"]
        })
    
    log(f"Loaded {len(comments_list)} comments for post {post_id}", level="INFO")
    
    return {
        "action": "get_comments",
        "post_id": str(post_id),
        "comments": comments_list,
        "count": len(comments_list),
        "error": "",
        "accept": True
    }

def handle_get_user_info(req: Dict[str, Any]):
    """Lấy thông tin user từ SQLite - KHÔNG trả password"""
    username = req.get("username") or req.get("UserName") or req.get("user") or ""
    username = "" if username is None else str(username)
    
    if not username:
        return {
            "action": "get_user_info",
            "username": "",
            "email": "",
            "phone": "",
            "bio": "",
            "avatar_url": "",
            "posts_user": [],
            "count_posts": 0,
            "count_followers": 0,
            "error": "Username không hợp lệ",
            "accept": False
        }
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    cursor.execute('SELECT * FROM users WHERE username = ?', (username,))
    user = cursor.fetchone()
    
    if not user:
        conn.close()
        return {
            "action": "get_user_info",
            "username": username,
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
    
    # Lấy posts của user
    cursor.execute('''
        SELECT id, content, image_url, video_url, timestamp, enabled 
        FROM posts 
        WHERE username = ? 
        ORDER BY id DESC
    ''', (username,))
    
    posts = cursor.fetchall()
    conn.close()
    
    posts_list = []
    for post in posts:
        posts_list.append({
            "id": post["id"],
            "content": post["content"],
            "image_url": post["image_url"] or "",
            "video_url": post["video_url"] or "",
            "timestamp": post["timestamp"],
            "enabled": bool(post["enabled"])
        })
    
    log(f"Loaded user info for '{username}' (password hidden)", level="INFO")
    
    return {
        "action": "get_user_info",
        "username": user["username"],
        "email": user["email"] or "",
        "phone": user["phone"] or "",
        "bio": user["bio"] or "Hello",
        "avatar_url": user["avatar_url"] or "",
        "posts_user": posts_list,
        "count_posts": user["count_posts"],
        "count_followers": user["count_followers"],
        "error": "",
        "accept": True
    }

def handle_update_user_avatar(req: Dict[str, Any]):
    """Cập nhật avatar_url của user"""
    username = req.get("username") or req.get("UserName") or ""
    new_avatar_url = req.get("avatar_url") or req.get("avatarUrl") or ""
    
    if not username or not new_avatar_url:
        return {
            "action": "update_user_avatar",
            "username": username,
            "avatar_url": "",
            "error": "Username hoặc avatar_url không hợp lệ",
            "accept": False
        }
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    try:
        cursor.execute('UPDATE users SET avatar_url = ? WHERE username = ?', 
                       (new_avatar_url, username))
        
        if cursor.rowcount == 0:
            conn.close()
            return {
                "action": "update_user_avatar",
                "username": username,
                "avatar_url": "",
                "error": "User không tồn tại",
                "accept": False
            }
        
        conn.commit()
        conn.close()
        
        log(f"Updated avatar for '{username}' to '{new_avatar_url}'", level="INFO")
        
        return {
            "action": "update_user_avatar",
            "username": username,
            "avatar_url": new_avatar_url,
            "error": "Cập nhật avatar thành công",
            "accept": True
        }
    except Exception as e:
        conn.close()
        log(f"Error updating avatar: {e}", level="ERROR")
        return {
            "action": "update_user_avatar",
            "username": username,
            "avatar_url": "",
            "error": f"Lỗi cập nhật avatar: {str(e)}",
            "accept": False
        }

# ============================================
# MESSENGER HANDLERS
# ============================================

def handle_get_online_users(req: Dict[str, Any]):
    """Lấy danh sách users đang online"""
    username = req.get("username") or ""
    
    with SESSION_LOCK:
        online_users = list(ACTIVE_SESSIONS.keys())
        log(f"User '{username}' requested online users: {online_users}", level="INFO")
        
        return {
            "action": "get_online_users",
            "users": online_users,
            "count": len(online_users),
            "error": "",
            "accept": True
        }

def handle_send_message(req: Dict[str, Any]):
    """Gửi tin nhắn giữa users"""
    from_user = req.get("from_user") or req.get("username") or "" 
    to_user = req.get("to_user") or ""
    content = req.get("content") or ""
    
    if not from_user or not to_user:
        return {
            "action": "send_message",
            "message_id": "",
            "from_user": from_user,
            "to_user": to_user,
            "content": content,
            "timestamp": "",
            "error": "From_user và to_user không hợp lệ",
            "accept": False
        }
    
    if not content:
        return {
            "action": "send_message",
            "message_id": "",
            "from_user": from_user,
            "to_user": to_user,
            "content": "",
            "timestamp": "",
            "error": "Nội dung tin nhắn không được để trống",
            "accept": False
        }
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    timestamp = str(int(time.time()))
    
    try:
        cursor.execute('''
            INSERT INTO messages (from_user, to_user, content, timestamp)
            VALUES (?, ?, ?, ?)
        ''', (from_user, to_user, content, timestamp))
        
        message_id = cursor.lastrowid
        conn.commit()
        conn.close()
        
        log(f"Message sent from '{from_user}' to '{to_user}'", level="INFO")
        
        return {
            "action": "send_message",
            "message_id": str(message_id),
            "from_user": from_user,
            "to_user": to_user,
            "content": content,
            "timestamp": timestamp,
            "error": "Gửi tin nhắn thành công",
            "accept": True
        }
    except Exception as e:
        conn.close()
        log(f"Error sending message: {e}", level="ERROR")
        return {
            "action": "send_message",
            "message_id": "",
            "from_user": from_user,
            "to_user": to_user,
            "content": content,
            "timestamp": timestamp,
            "error": f"Lỗi gửi tin nhắn: {str(e)}",
            "accept": False
        }

def handle_get_messages(req: Dict[str, Any]):
    """Lấy tất cả tin nhắn giữa 2 users"""
    from_user = req.get("from_user") or req.get("username") or ""
    to_user = req.get("to_user") or ""
    
    if not from_user or not to_user:
        return {
            "action": "get_messages",
            "messages": [],
            "count": 0,
            "error": "From_user và to_user không hợp lệ",
            "accept": False
        }
    
    conn = get_db_connection()
    cursor = conn.cursor()
    
    cursor.execute('''
        SELECT message_id, from_user, to_user, content, timestamp
        FROM messages
        WHERE (from_user = ? AND to_user = ?) OR (from_user = ? AND to_user = ?)
        ORDER BY message_id ASC
    ''', (from_user, to_user, to_user, from_user))
    
    messages = cursor.fetchall()
    conn.close()
    
    messages_list = []
    for msg in messages:
        messages_list.append({
            "message_id": str(msg["message_id"]),
            "from_user": msg["from_user"],
            "to_user": msg["to_user"],
            "content": msg["content"],
            "timestamp": msg["timestamp"]
        })
    
    log(f"Loaded {len(messages_list)} messages between '{from_user}' and '{to_user}'", level="INFO")
    
    return {
        "action": "get_messages",
        "messages": messages_list,
        "count": len(messages_list),
        "error": "",
        "accept": True
    }

# ============================================
# ACTION HANDLERS MAPPING
# ============================================

ACTION_HANDLERS = {
    "login": handle_login_data,
    "login_data": handle_login_data,
    "logout": handle_logout,
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
    "getonlineusers": handle_get_online_users,
    "get_online_users": handle_get_online_users,
    "sendmessage": handle_send_message,
    "send_message": handle_send_message,
    "getmessages": handle_get_messages,
    "get_messages": handle_get_messages,
}

def process_request(jobj: Dict[str, Any], addr=None):
    action = jobj.get("action") or jobj.get("Action") or jobj.get("ActionName")
    if not action:
        log(f"Missing action in request: {pretty(jobj)}", level="WARN", addr=addr)
        return {"action": "error", "message": "missing action", "accept": False}

    akey = str(action).strip().lower()
    handler = ACTION_HANDLERS.get(akey)
    if not handler:
        log(f"Unknown action '{action}'", level="WARN", addr=addr)
        return {"action": "error", "message": f"unknown action: {action}", "accept": False}

    try:
        log(f"Calling handler for action '{action}'", level="INFO", addr=addr)
        if akey in ["login", "login_data"]:
            resp = handler(jobj, addr)
        else:
            resp = handler(jobj)
        if not isinstance(resp, dict):
            log(f"Handler for {action} returned non-dict", level="ERROR", addr=addr)
            return {"action": "error", "message": "handler error", "accept": False}
        return resp
    except Exception as ex:
        traceback.print_exc()
        log(f"Exception in handler for {action}: {ex}", level="ERROR", addr=addr)
        return {"action": "error", "message": str(ex), "accept": False}

def client_thread(conn: socket.socket, addr):
    thr = threading.current_thread().name
    log(f"New client connected: {addr}", level="INFO")
    
    current_user = None
    
    try:
        with conn:
            f = conn.makefile(mode="rw", encoding=ENC, newline="\n")

            while True:
                line = f.readline()
                if not line:
                    log(f"Client disconnected: {addr}", level="INFO")
                    break

                raw = line.rstrip("\n")
                log(f"Received raw line from {addr}: {raw}", level="DEBUG")
                
                if raw.startswith('\ufeff'):
                    log(f"Detected BOM, stripping it", level="INFO")
                    raw = raw.lstrip('\ufeff')

                if raw.strip() == "":
                    log("Received empty line, skipping", level="DEBUG")
                    continue

                try:
                    jobj = json.loads(raw)
                    log(f"Parsed JSON from {addr}:\n{pretty(jobj)}", level="INFO")
                except Exception as e:
                    log(f"JSON PARSE ERROR: {e} -- raw: {raw}", level="WARN")
                    send_json_fileobj(f, {"action": "error", "message": "invalid json", "accept": False}, addr=addr)
                    continue

                resp_obj = process_request(jobj, addr=addr)
                
                # Track login
                action = jobj.get("action", "").lower()
                if action in ["login", "login_data"] and resp_obj.get("accept"):
                    current_user = jobj.get("username")
                    log(f"Tracked current_user: {current_user}", level="INFO")
                
                rid = jobj.get("request_id") or jobj.get("RequestId") or jobj.get("requestId")
                if rid is not None:
                    resp_obj["request_id"] = "ServerHaha_"+rid

                log(f"Responding to {addr} with:\n{pretty(resp_obj)}", level="INFO")
                send_json_fileobj(f, resp_obj, addr=addr)

    except Exception as e:
        log(f"Exception with client {addr}: {e}", level="ERROR")
        traceback.print_exc()
    finally:
        # Xóa session khi disconnect
        if current_user:
            with SESSION_LOCK:
                if current_user in ACTIVE_SESSIONS:
                    del ACTIVE_SESSIONS[current_user]
                    log(f"Removed session for '{current_user}'", level="INFO")
        log(f"Thread exiting for {addr}", level="INFO")

def signal_handler(signum, frame):
    """Handle shutdown signals"""
    log(f"Received signal {signum}, shutting down...", level="INFO")
    sys.exit(0)

def start_server():
    signal.signal(signal.SIGINT, signal_handler)
    signal.signal(signal.SIGTERM, signal_handler)
    
    if WRITE_LOG_FILE:
        try:
            with open(LOG_FILENAME, "w", encoding="utf-8") as f:
                f.write("")
        except Exception:
            pass

    log(f"=== PostEZ Server Started ===", level="INFO")
    log(f"Listening on {HOST}:{PORT}", level="INFO")
    log(f"Database: {DB_PATH}", level="INFO")
    log(f"Server IP: {IP_INPUT}", level="INFO")
    
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
        log("Server stopped", level="INFO")

if __name__ == "__main__":
    start_server()