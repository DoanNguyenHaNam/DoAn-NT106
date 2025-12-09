# upload_server.py
from flask import Flask, request, jsonify
from werkzeug.utils import secure_filename
import os
from datetime import datetime

app = Flask(__name__)

# Cấu hình
UPLOAD_FOLDER = '/DB/SaveData'  # Thư mục lưu file (nơi web server phục vụ)
ALLOWED_EXTENSIONS = {'png', 'jpg', 'jpeg', 'gif', 'bmp', 'mp4', 'avi', 'mov', 'wmv', 'mkv'}
MAX_IMAGE_SIZE = 10 * 1024 * 1024  # 10MB
MAX_VIDEO_SIZE = 100 * 1024 * 1024  # 100MB

app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER
app.config['MAX_CONTENT_LENGTH'] = MAX_VIDEO_SIZE

# Tạo thư mục nếu chưa có
os.makedirs(UPLOAD_FOLDER, exist_ok=True)

def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS

@app.route('/upload', methods=['POST'])
def upload_file():
    try:
        # Kiểm tra file có trong request không
        if 'file' not in request.files:
            return jsonify({'success': False, 'error': 'No file part'}), 400
        
        file = request.files['file']
        username = request.form.get('username', 'anonymous')
        file_type = request.form.get('type', 'unknown')  # image hoặc video
        
        if file.filename == '':
            return jsonify({'success': False, 'error': 'No selected file'}), 400
        
        if not allowed_file(file.filename):
            return jsonify({'success': False, 'error': 'File type not allowed'}), 400
        
        # Tạo tên file unique
        timestamp = datetime.now().strftime('%Y%m%d_%H%M%S')
        original_filename = secure_filename(file.filename)
        filename = f"{username}_{timestamp}_{original_filename}"
        
        # Lưu file
        file_path = os.path.join(app.config['UPLOAD_FOLDER'], filename)
        file.save(file_path)
        
        # Trả về URL để truy cập file
        file_url = f"http://160.191.245.144/uploads/{filename}"
        
        return jsonify({
            'success': True,
            'url': file_url,
            'filename': filename,
            'type': file_type
        }), 200
        
    except Exception as e:
        return jsonify({'success': False, 'error': str(e)}), 500

@app.route('/health', methods=['GET'])
def health_check():
    return jsonify({'status': 'OK', 'message': 'Upload server is running'}), 200

if __name__ == '__main__':
    # Chạy trên port 13580, cho phép truy cập từ mọi IP
    app.run(host='0.0.0.0', port=80, debug=False)