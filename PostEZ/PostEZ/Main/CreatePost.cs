using PostEZ.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostEZ.Main
{
    public partial class CreatePost : Form
    {
        private string? selectedImagePath = null;
        private string? selectedVideoPath = null;
        private string? youtubeVideoUrl = null;
        private string? uploadedImageUrl = null;
        private string? uploadedVideoUrl = null;

        private const string UPLOAD_API_URL = "http://160.191.245.144/doanNT106/upload.php";

        public CreatePost()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private async void CreatePost_Load(object sender, EventArgs e)
        {
            await Login.LoadFromUrl("https://pminmod.site/doannt106/logo.png", pic_logo);
            ResetForm();
        }

        private void ResetForm()
        {
            tb_content.Clear();
            selectedImagePath = null;
            selectedVideoPath = null;
            youtubeVideoUrl = null;
            uploadedImageUrl = null;
            uploadedVideoUrl = null;

            pic_preview.Image = null;
            pic_preview.BackColor = Color.WhiteSmoke;

            lb_image_status.Text = "Chưa chọn ảnh";
            lb_image_status.ForeColor = Color.Gray;
            lb_video_status.Text = "Chưa chọn video";
            lb_video_status.ForeColor = Color.Gray;

            btn_remove_image.Visible = false;
            btn_remove_video.Visible = false;
        }

        private void btn_main_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_profile_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ============================================
        // CHỌN ẢNH
        // ============================================
        private void btn_select_image_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Chọn ảnh";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.webp|All Files|*.*";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = openFileDialog.FileName;

                    FileInfo fileInfo = new FileInfo(selectedImagePath);
                    if (fileInfo.Length > 10 * 1024 * 1024)
                    {
                        MessageBox.Show("Ảnh quá lớn! Tối đa 10MB.", "Lỗi");
                        selectedImagePath = null;
                        return;
                    }

                    try
                    {
                        pic_preview.Image = Image.FromFile(selectedImagePath);
                        pic_preview.SizeMode = PictureBoxSizeMode.Zoom;
                        lb_image_status.Text = $"✅ {Path.GetFileName(selectedImagePath)} ({fileInfo.Length / 1024}KB)";
                        lb_image_status.ForeColor = Color.Green;
                        btn_remove_image.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể load ảnh: " + ex.Message, "Lỗi");
                        selectedImagePath = null;
                    }
                }
            }
        }

        // ============================================
        // XÓA ẢNH ĐÃ CHỌN
        // ============================================
        private void btn_remove_image_Click(object sender, EventArgs e)
        {
            selectedImagePath = null;
            uploadedImageUrl = null;
            pic_preview.Image = null;
            pic_preview.BackColor = Color.WhiteSmoke;
            lb_image_status.Text = "Chưa chọn ảnh";
            lb_image_status.ForeColor = Color.Gray;
            btn_remove_image.Visible = false;
        }

        // ============================================
        // CHỌN VIDEO (2 OPTIONS)
        // ============================================
        private void btn_select_video_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "YES = Upload video từ máy tính\nNO = Nhập link YouTube",
                "Chọn loại video",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                SelectVideoFile();
            }
            else if (result == DialogResult.No)
            {
                InputYouTubeLink();
            }
        }

        private void SelectVideoFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Chọn video";
                openFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mov;*.wmv;*.mkv;*.flv|All Files|*.*";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedVideoPath = openFileDialog.FileName;
                    youtubeVideoUrl = null;

                    FileInfo fileInfo = new FileInfo(selectedVideoPath);
                    if (fileInfo.Length > 100 * 1024 * 1024) // 100MB
                    {
                        MessageBox.Show($"Video quá lớn! Tối đa 100MB.\nFile hiện tại: {fileInfo.Length / 1024 / 1024}MB", "Lỗi");
                        selectedVideoPath = null;
                        return;
                    }

                    lb_video_status.Text = $"✅ {Path.GetFileName(selectedVideoPath)} ({fileInfo.Length / 1024 / 1024}MB)";
                    lb_video_status.ForeColor = Color.Green;
                    btn_remove_video.Visible = true;
                }
            }
        }

        private void InputYouTubeLink()
        {
            string url = PromptForYouTubeUrl();
            
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (url.Contains("youtube.com") || url.Contains("youtu.be"))
                {
                    youtubeVideoUrl = url;
                    selectedVideoPath = null;
                    lb_video_status.Text = "✅ YouTube: " + url;
                    lb_video_status.ForeColor = Color.Green;
                    btn_remove_video.Visible = true;
                }
                else
                {
                    MessageBox.Show("URL không hợp lệ! Chỉ chấp nhận YouTube.", "Lỗi");
                }
            }
        }

        private string PromptForYouTubeUrl()
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Nhập URL YouTube",
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Text = "YouTube URL:", Width = 100 };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 440 };
            Button confirmation = new Button() { Text = "OK", Left = 280, Width = 80, Top = 80, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Hủy", Left = 370, Width = 80, Top = 80, DialogResult = DialogResult.Cancel };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        // ============================================
        // XÓA VIDEO ĐÃ CHỌN
        // ============================================
        private void btn_remove_video_Click(object sender, EventArgs e)
        {
            selectedVideoPath = null;
            youtubeVideoUrl = null;
            uploadedVideoUrl = null;
            lb_video_status.Text = "Chưa chọn video";
            lb_video_status.ForeColor = Color.Gray;
            btn_remove_video.Visible = false;
        }

        // ============================================
        // UPLOAD FILE LÊN SERVER (PHP API)
        // ============================================
        private async Task<string?> UploadFileToServer(string filePath, string fileType)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(10);

                    using (var form = new MultipartFormDataContent())
                    {
                        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
                        var fileContent = new ByteArrayContent(fileBytes);

                        string contentType = fileType == "image" ? "image/jpeg" : "video/mp4";
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                        form.Add(fileContent, "file", Path.GetFileName(filePath));
                        form.Add(new StringContent(Load_Data.LoginData.username), "username");
                        form.Add(new StringContent(fileType), "type");

                        var response = await httpClient.PostAsync(UPLOAD_API_URL, form);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseText = await response.Content.ReadAsStringAsync();
                            var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseText);
                            
                            if (jsonResponse?.success == true)
                            {
                                return jsonResponse?.url?.ToString();
                            }
                            else
                            {
                                MessageBox.Show("Server lỗi: " + jsonResponse?.error, "Lỗi");
                                return null;
                            }
                        }
                        else
                        {
                            string errorBody = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Upload thất bại: {response.StatusCode}\n{errorBody}", "Lỗi");
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi upload: {ex.Message}", "Lỗi");
                return null;
            }
        }

        // ============================================
        // ĐĂNG BÀI
        // ============================================
        private async void btn_post_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_content.Text))
            {
                MessageBox.Show("Vui lòng nhập nội dung bài viết!", "Lỗi");
                return;
            }

            // Disable button để tránh click nhiều lần
            btn_post.Enabled = false;
            btn_post.Text = "Đang xử lý...";

            try
            {
                // 1. Upload ảnh nếu có
                if (!string.IsNullOrEmpty(selectedImagePath))
                {
                    lb_image_status.Text = "⏳ Đang upload ảnh...";
                    lb_image_status.ForeColor = Color.Orange;
                    
                    uploadedImageUrl = await UploadFileToServer(selectedImagePath, "image");
                    
                    if (uploadedImageUrl == null)
                    {
                        lb_image_status.Text = "❌ Upload ảnh thất bại!";
                        lb_image_status.ForeColor = Color.Red;
                        btn_post.Enabled = true;
                        btn_post.Text = "Đăng bài";
                        return;
                    }
                    
                    lb_image_status.Text = "✅ Upload ảnh thành công!";
                    lb_image_status.ForeColor = Color.Green;
                }

                // 2. Upload video nếu có
                if (!string.IsNullOrEmpty(selectedVideoPath))
                {
                    lb_video_status.Text = "⏳ Đang upload video...";
                    lb_video_status.ForeColor = Color.Orange;
                    
                    uploadedVideoUrl = await UploadFileToServer(selectedVideoPath, "video");
                    
                    if (uploadedVideoUrl == null)
                    {
                        lb_video_status.Text = "❌ Upload video thất bại!";
                        lb_video_status.ForeColor = Color.Red;
                        btn_post.Enabled = true;
                        btn_post.Text = "Đăng bài";
                        return;
                    }
                    
                    lb_video_status.Text = "✅ Upload video thành công!";
                    lb_video_status.ForeColor = Color.Green;
                }
                else if (!string.IsNullOrEmpty(youtubeVideoUrl))
                {
                    uploadedVideoUrl = youtubeVideoUrl;
                }

                Load_Data.CreatePost.action = "post_data";
                Load_Data.CreatePost.username = Load_Data.LoginData.username;
                Load_Data.CreatePost.content = tb_content.Text;
                Load_Data.CreatePost.image_url = uploadedImageUrl ?? "";
                Load_Data.CreatePost.video_url = uploadedVideoUrl ?? "";
                Load_Data.CreatePost.request_id = Load_Data.GenerateRandomString(4);

                bool success = Load_Data.SendJson(Load_Data.CreatePost);
                if (!success)
                {
                    MessageBox.Show("Không thể gửi dữ liệu tới server!", "Lỗi");
                    return;
                }

                bool received = await Load_Data.WaitForServerResponse(
                    () => Load_Data.CreatePost.request_id != null && Load_Data.CreatePost.request_id.Contains("ServerHaha"),
                    timeoutSeconds: 1
                );

                if (!received)
                {
                    MessageBox.Show("Server không phản hồi kịp thời (get_feed). Vui lòng thử lại sau!", "Lỗi");
                    return;
                }
                else
                {
                    if (Load_Data.CreatePost.error == "")
                    {
                        MessageBox.Show("Upload file thành công!\n\n" +
                            $"Ảnh: {(uploadedImageUrl != null ? "✅" : "❌")}\n" +
                            $"Video: {(uploadedVideoUrl != null ? "✅" : "❌")}\n\n" +
                            "Bạn có thể gửi request TCP để tạo bài đăng.",
                            "Thông báo"
                        );
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi đăng bài: " + Load_Data.CreatePost.error, "Lỗi");
                        return;
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi");
            }
            finally
            {
                btn_post.Enabled = true;
                btn_post.Text = "Đăng bài";
            }
        }
    }
}
