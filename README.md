<h1 align="center">POSTEZ</h1>

<p align="center">
  <img src="https://raw.githubusercontent.com/DoanNguyenHaNam/DoAn-NT106/main/Sources_NotNecessery/Logo.png" alt="POSTEZ Logo" width="180" height="auto">
</p>

<h4 align="center">Một ứng dụng của Socket</h4>

<p align="center">
  <a href="#features">Tính năng</a> •
  <a href="#installation">Cài đặt & Khởi chạy</a> •
  <a href="#cre">Creator</a>
</p>

---

## 🚀 Tính năng

POSTEZ được thiết kế nhằm tạo không gian trao đổi thông tin, bàn luận về các vấn đề nhanh, tiện.

* **Tạo Bài Viết Đơn Giản** — Giao diện trực quan cho phép người dùng đăng tải nội dung (văn bản, hình ảnh, video ngắn) một cách nhanh chóng.
* **Tương Tác Tối Giản** — Chỉ bao gồm các tương tác cơ bản: **Thích (Like)** và **Bình luận (Comment)**, giúp nội dung trở nên quan trọng hơn.
* **Trang Cá nhân** — Tổng hợp tất cả bài viết của người dùng, có thể tùy chỉnh ảnh đại diện.
* **Message** — Hỗ trợ nhắn tin cơ bản

## 🛠️ Cài đặt & Khởi chạy

Bạn có thể chạy POSTEZ theo hai cách: sử dụng tệp thực thi đã đóng gói sẵn, hoặc chạy trực tiếp từ mã nguồn.

### 1. Dành cho Người dùng Cuối (Native Windows APP)

Phương pháp này đơn giản nhất, không yêu cầu cài đặt Node.js hay các công cụ phát triển khác.

1.  **Tải xuống:** Truy cập trang [Latest Release](https://github.com/DoanNguyenHaNam/DoAn-NT106/releases/latest) và tải xuống tệp cài đặt **POSTEZvX.X.exe**.
2.  **Cài đặt:** Chạy tệp `.exe` đã tải xuống và làm theo hướng dẫn.
3.  **Khởi chạy:** Mở ứng dụng POSTEZ từ Desktop hoặc Start Menu.

### 2. Dành cho Lập trình viên (Từ Mã Nguồn)

Phương pháp này cho phép bạn tùy chỉnh, phát triển và chạy ứng dụng trên các nền tảng khác (Windows, macOS, Linux) bằng cách sử dụng **Node.js** (phiên bản $18+$).

1.  **Clone Repository:** Mở Terminal (hoặc Git Bash) và sao chép mã nguồn:
    ```bash
    git clone [https://github.com/DoanNguyenHaNam/DoAn-NT106.git](https://github.com/DoanNguyenHaNam/DoAn-NT106.git)
    cd DoAn-NT106
    ```
### 2. Dành cho Lập trình viên (Từ Mã Nguồn - Python Server)

Phương pháp này cho phép bạn tùy chỉnh, phát triển và chạy ứng dụng Backend (Server) trên các nền tảng (Windows, macOS, Linux) bằng cách sử dụng **Python** (phiên bản **$3.9+$** được yêu cầu).

#### **Bước A: Cấu hình Môi trường Python**

1.  **Cài đặt Python:** Đảm bảo bạn đã cài đặt **Python 3.9 trở lên**.
2.  **Clone Repository:** Mở Terminal (hoặc Command Prompt/Git Bash) và sao chép mã nguồn:
    ```bash
    git clone [https://github.com/DoanNguyenHaNam/DoAn-NT106.git](https://github.com/DoanNguyenHaNam/DoAn-NT106.git)
    cd DoAn-NT106/Server
    ```
    > **Lưu ý:** Chỉnh sửa ip và port phù hợp.
3.  **Cài đặt Phụ thuộc:** Cài đặt các thư viện cần thiết bằng cách sử dụng môi trường ảo (Virtual Environment - venv):
    ```bash
    # 1. Tạo môi trường ảo (Chỉ cần làm lần đầu)
    python3 -m venv venv
    
    # 2. Kích hoạt môi trường ảo
    # Trên Linux/macOS:
    source venv/bin/activate
    # Trên Windows (Command Prompt):
    venv\Scripts\activate
    
    ```
4.  **Cấu hình Dữ liệu:** Đảm bảo thư mục **`DB`** tồn tại trong thư mục gốc của dự án. Server sẽ tự tạo thư mục này nếu cần.
   
---

## 📄 Creator
* **Github Account** DoanNguyenHaNam
  
### Liên hệ
* **Tên Sinh viên - MSSV - Nhóm:** Đoàn Nguyễn Hà Nam - 24521100 - Nhóm 14
* **GitHub Repository:** [https://github.com/DoanNguyenHaNam/DoAn-NT106]

