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
from typing import Dict, Any, Optional

HOST = "0.0.0.0"
PORT = 13579
ENC = "utf-8-sig"  # <<-- use utf-8-sig so BOM is removed automatically

VERBOSE_TO_CONSOLE = True
WRITE_LOG_FILE = True
LOG_FILENAME = "server_verbose.log"

# Simple in-memory stores
USERS = {
    "nam": {"username": "nam", "password": "123", "email": "nam@example.com"},
}
POSTS = []
COMMENTS = {}
POST_ID_SEQ = 1
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

# ------------------ Handlers ------------------

def handle_login_data(req: Dict[str, Any]):
    # Accept only username == "Admin123" and password == "Admin123!"
    username = req.get("username") or req.get("UserName") or req.get("User") or ""
    password = req.get("password") or req.get("Password") or req.get("Pass") or ""
    username = "" if username is None else str(username)
    password = "" if password is None else str(password)

    log(f"Login attempt: username='{username}', password='{password}'", level="INFO")
    accept = (username == "Admin123" and password == "Admin123!")
    resp = {
        "action": "login_data",
        "username": username,
        "password": password,
        "accept": accept
    }
    return resp

def handle_signup_data(req: Dict[str, Any]):
    username = req.get("username") or req.get("UserName") or ""
    password = req.get("password") or req.get("Password") or ""
    email = req.get("email") or req.get("Email") or ""
    username = "" if username is None else str(username)
    password = "" if password is None else str(password)
    email = "" if email is None else str(email)

    with LOCK:
        if username and username not in USERS:
            USERS[username] = {"username": username, "password": password, "email": email}
            accept = True
        else:
            accept = False

    resp = {
        "action": "signup_data",
        "username": username,
        "password": password,
        "email": email,
        "accept": accept
    }
    return resp

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

    with LOCK:
        pid = POST_ID_SEQ
        POST_ID_SEQ += 1
        post = {
            "action": "post_data",
            "username": username,
            "id": pid,
            "content": content,
            "image_url": image_url,
            "video_url": video_url,
            "timestamp": str(int(time.time())),
            "accept": True
        }
        POSTS.insert(0, post)
    return post

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

ACTION_HANDLERS = {
    "login": handle_login_data,
    "login_data": handle_login_data,
    "signup": handle_signup_data,
    "signup_data": handle_signup_data,
    "createpost": handle_create_post,
    "create_post": handle_create_post,
    "post_data": handle_create_post,
    "getfeed": handle_create_post,  # placeholder (not used)
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
                    resp_obj["request_id"] = rid

                log(f"Responding to {addr} with:\n{pretty(resp_obj)}", level="INFO")
                send_json_fileobj(f, resp_obj, addr=addr)

    except Exception as e:
        log(f"Exception with client {addr}: {e}", level="ERROR")
        traceback.print_exc()
    log(f"Thread exiting for {addr}", level="INFO")

def start_server():
    # truncate log file on start
    if WRITE_LOG_FILE:
        try:
            with open(LOG_FILENAME, "w", encoding="utf-8") as f:
                f.write("")  # clear
        except Exception:
            pass

    log(f"Listening on {HOST}:{PORT}", level="INFO")
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

if __name__ == "__main__":
    start_server()
