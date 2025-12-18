<?php
/**
 * upload.php
 * Simple file upload handler for PostEZ
 * Supports: avatar, post images, post videos
 * Compatible with C# client expecting "success" field
 */

// CORS headers
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type');
header('Content-Type: application/json; charset=utf-8');

// Handle OPTIONS request (CORS preflight)
if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit();
}

// Configuration
define('UPLOAD_DIR', 'DB/uploads/');
define('AVATAR_DIR', 'DB/USER/avatar/');
define('POST_IMAGES_DIR', 'DB/POST/images/');
define('POST_VIDEOS_DIR', 'DB/POST/videos/');
define('MAX_FILE_SIZE', 100 * 1024 * 1024); // 100MB
define('SERVER_URL', 'http://160.191.245.144');

$ALLOWED_IMAGE_EXTENSIONS = ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp'];
$ALLOWED_VIDEO_EXTENSIONS = ['mp4', 'avi', 'mov', 'wmv', 'flv', 'webm', 'mkv'];

// Create directories if not exist
$directories = [UPLOAD_DIR, AVATAR_DIR, POST_IMAGES_DIR, POST_VIDEOS_DIR];
foreach ($directories as $dir) {
    if (!is_dir($dir)) {
        mkdir($dir, 0777, true);
        error_log("Created directory: $dir");
    }
}

// Log function
function logMessage($message, $level = 'INFO') {
    $timestamp = date('Y-m-d H:i:s');
    $logEntry = "[$timestamp] [$level] $message\n";
    error_log($logEntry);
    file_put_contents('upload.log', $logEntry, FILE_APPEND);
}

// Error response - COMPATIBLE with C# client
function sendError($code, $message) {
    http_response_code($code);
    echo json_encode([
        'success' => false,  // ← C# expects "success" field
        'error' => $message
    ], JSON_UNESCAPED_UNICODE);
    logMessage("Error: $message", 'ERROR');
    exit();
}

// Success response - COMPATIBLE with C# client
function sendSuccess($data) {
    http_response_code(200);
    echo json_encode(array_merge(['success' => true], $data), JSON_UNESCAPED_UNICODE);
    exit();
}

// GET request - status check
if ($_SERVER['REQUEST_METHOD'] === 'GET') {
    sendSuccess([
        'message' => 'Upload server is running',
        'server_url' => SERVER_URL,
        'max_file_size' => MAX_FILE_SIZE
    ]);
}

// POST request - file upload
if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    sendError(405, 'Method Not Allowed');
}

// Check if file was uploaded
if (!isset($_FILES['file'])) {
    sendError(400, 'No file field in form data');
}

$file = $_FILES['file'];
$uploadType = isset($_POST['type']) ? $_POST['type'] : 'general';
$username = isset($_POST['username']) ? $_POST['username'] : 'anonymous';

logMessage("Upload request - User: $username, Type: $uploadType, File: {$file['name']}", 'INFO');

// Validate file upload
if ($file['error'] !== UPLOAD_ERR_OK) {
    $errorMessages = [
        UPLOAD_ERR_INI_SIZE => 'File too large (server limit)',
        UPLOAD_ERR_FORM_SIZE => 'File too large (form limit)',
        UPLOAD_ERR_PARTIAL => 'File was only partially uploaded',
        UPLOAD_ERR_NO_FILE => 'No file was uploaded',
        UPLOAD_ERR_NO_TMP_DIR => 'Missing temporary folder',
        UPLOAD_ERR_CANT_WRITE => 'Failed to write file to disk',
        UPLOAD_ERR_EXTENSION => 'File upload stopped by extension'
    ];
    $errorMsg = isset($errorMessages[$file['error']]) 
        ? $errorMessages[$file['error']] 
        : 'Unknown upload error';
    sendError(400, $errorMsg);
}

// Check file size
if ($file['size'] > MAX_FILE_SIZE) {
    sendError(413, 'File too large (max ' . (MAX_FILE_SIZE / (1024*1024)) . 'MB)');
}

if ($file['size'] == 0) {
    sendError(400, 'File is empty');
}

// Get file info
$originalFilename = basename($file['name']);
$fileExtension = strtolower(pathinfo($originalFilename, PATHINFO_EXTENSION));
$fileSize = $file['size'];
$tmpPath = $file['tmp_name'];

// Determine save directory and validate extension based on type
switch ($uploadType) {
    case 'avatar':
        $saveDir = AVATAR_DIR;
        if (!in_array($fileExtension, $ALLOWED_IMAGE_EXTENSIONS)) {
            sendError(400, 'Invalid image format for avatar. Allowed: ' . implode(', ', $ALLOWED_IMAGE_EXTENSIONS));
        }
        break;
    
    case 'image':  // ← CreatePost.cs gửi "image"
    case 'post_image':
        $saveDir = POST_IMAGES_DIR;
        if (!in_array($fileExtension, $ALLOWED_IMAGE_EXTENSIONS)) {
            sendError(400, 'Invalid image format. Allowed: ' . implode(', ', $ALLOWED_IMAGE_EXTENSIONS));
        }
        break;
    
    case 'video':  // ← CreatePost.cs gửi "video"
    case 'post_video':
        $saveDir = POST_VIDEOS_DIR;
        if (!in_array($fileExtension, $ALLOWED_VIDEO_EXTENSIONS)) {
            sendError(400, 'Invalid video format. Allowed: ' . implode(', ', $ALLOWED_VIDEO_EXTENSIONS));
        }
        break;
    
    default:
        $saveDir = UPLOAD_DIR;
        $allExtensions = array_merge($ALLOWED_IMAGE_EXTENSIONS, $ALLOWED_VIDEO_EXTENSIONS);
        if (!in_array($fileExtension, $allExtensions)) {
            sendError(400, 'Invalid file format');
        }
        break;
}

// Generate unique filename
$timestamp = round(microtime(true) * 1000);
$newFilename = $username . '_' . $timestamp . '.' . $fileExtension;
$filePath = $saveDir . $newFilename;

// Move uploaded file
if (!move_uploaded_file($tmpPath, $filePath)) {
    sendError(500, 'Failed to save file');
}

// Set proper permissions
chmod($filePath, 0644);

// Generate URL - MATCH với server path thực tế
$fileUrl = SERVER_URL . '/doanNT106/' . str_replace('\\', '/', $filePath);

logMessage("File uploaded successfully: $filePath ($fileSize bytes) by $username - Type: $uploadType - URL: $fileUrl", 'INFO');

// Send success response - COMPATIBLE with C# client
sendSuccess([
    'message' => 'File uploaded successfully',
    'filename' => $newFilename,
    'original_filename' => $originalFilename,
    'url' => $fileUrl,
    'size' => $fileSize,
    'type' => $uploadType
]);
?>
