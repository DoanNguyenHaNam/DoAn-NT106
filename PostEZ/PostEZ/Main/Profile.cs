using PostEZ.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label; // Fix conflict

namespace PostEZ.Main
{
    public partial class Profile : Form
    {
        private readonly string _username;
        private Button? btnChangeAvatar; // Nút thay ảnh đại diện
        private const string UPLOAD_API_URL = "http://160.191.245.144/doanNT106/upload.php";

        public Profile(string username)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            _username = username;
            
            lb_bio.AutoSize = false;
            lb_bio.Size = new Size(220, 80);
            lb_bio.TextAlign = ContentAlignment.MiddleCenter;
            lb_bio.Padding = new Padding(6);

            lb_name.AutoSize = false;
            lb_name.Size = new Size(220, 40);
            lb_name.TextAlign = ContentAlignment.MiddleCenter;
            lb_name.Padding = new Padding(6);

            // Tạo nút thay ảnh (ẩn ban đầu)
            CreateChangeAvatarButton();
        }

        private void CreateChangeAvatarButton()
        {
            btnChangeAvatar = new Button
            {
                Text = "📷 Thay ảnh",
                Size = new Size(pic_avatar.Width, 30),
                Location = new Point(pic_avatar.Left, pic_avatar.Bottom - 30),
                BackColor = Color.FromArgb(180, 0, 0, 0), // Màu đen trong suốt
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Visible = false
            };
            btnChangeAvatar.FlatAppearance.BorderSize = 0;
            btnChangeAvatar.Click += BtnChangeAvatar_Click;

            // Thêm vào form (cùng container với pic_avatar)
            if (pic_avatar.Parent != null)
            {
                pic_avatar.Parent.Controls.Add(btnChangeAvatar);
                btnChangeAvatar.BringToFront();
            }

            // Events để hiện/ẩn nút khi hover
            pic_avatar.MouseEnter += (s, e) => btnChangeAvatar.Visible = true;
            pic_avatar.MouseLeave += (s, e) => 
            {
                if (!btnChangeAvatar.ClientRectangle.Contains(btnChangeAvatar.PointToClient(Cursor.Position)))
                {
                    btnChangeAvatar.Visible = false;
                }
            };
            btnChangeAvatar.MouseLeave += (s, e) =>
            {
                if (!pic_avatar.ClientRectangle.Contains(pic_avatar.PointToClient(Cursor.Position)))
                {
                    btnChangeAvatar.Visible = false;
                }
            };
        }

        private async void BtnChangeAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Chọn ảnh đại diện";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    // Kiểm tra kích thước
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length > 5 * 1024 * 1024) // 5MB
                    {
                        MessageBox.Show("Ảnh quá lớn! Tối đa 5MB.", "Lỗi");
                        return;
                    }

                    try
                    {
                        btnChangeAvatar.Text = "⏳ Đang tải...";
                        btnChangeAvatar.Enabled = false;

                        // Upload ảnh lên server
                        string? uploadedUrl = await UploadAvatarToServer(filePath);
                        
                        if (uploadedUrl == null)
                        {
                            MessageBox.Show("Upload ảnh thất bại!", "Lỗi");
                            return;
                        }

                        // Gửi request cập nhật avatar về server qua TCP
                        bool updateSuccess = await UpdateAvatarOnServer(uploadedUrl);
                        
                        if (!updateSuccess)
                        {
                            MessageBox.Show("Cập nhật avatar trên server thất bại!", "Lỗi");
                            return;
                        }

                        // Cập nhật local
                        Load_Data.InformationUser.avatar_url = uploadedUrl;
                        
                        // Cập nhật UI
                        await Login.LoadFromUrl(uploadedUrl, pic_avatar, showError: false);
                        MessageBox.Show("Cập nhật ảnh đại diện thành công!", "Thông báo");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message, "Lỗi");
                    }
                    finally
                    {
                        btnChangeAvatar.Text = "📷 Thay ảnh";
                        btnChangeAvatar.Enabled = true;
                        btnChangeAvatar.Visible = false;
                    }
                }
            }
        }

        private async Task<string?> UploadAvatarToServer(string filePath)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(5);

                    using (var form = new MultipartFormDataContent())
                    {
                        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
                        var fileContent = new ByteArrayContent(fileBytes);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                        form.Add(fileContent, "file", Path.GetFileName(filePath));
                        form.Add(new StringContent(Load_Data.LoginData.username), "username");
                        form.Add(new StringContent("avatar"), "type");

                        var response = await httpClient.PostAsync(UPLOAD_API_URL, form);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseText = await response.Content.ReadAsStringAsync();
                            var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseText);
                            
                            if (jsonResponse?.success == true)
                            {
                                return jsonResponse?.url?.ToString();
                            }
                        }
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> UpdateAvatarOnServer(string avatarUrl)
        {
            try
            {
                // Tạo request update avatar
                Load_Data.UpdateAvatar = new Load_Data.Data_UpdateAvatarJson
                {
                    action = "update_user_avatar",
                    username = Load_Data.LoginData.username,
                    avatar_url = avatarUrl,
                    request_id = Load_Data.GenerateRandomString(4)
                };

                // Gửi request
                bool sent = Load_Data.SendJson(Load_Data.UpdateAvatar);
                if (!sent)
                {
                    return false;
                }

                // Đợi phản hồi từ server
                bool received = await Load_Data.WaitForServerResponse(
                    () => Load_Data.UpdateAvatar.request_id != null && Load_Data.UpdateAvatar.request_id.Contains("ServerHaha"),
                    timeoutSeconds: 15
                );

                if (!received)
                {
                    return false;
                }

                // Kiểm tra kết quả
                return Load_Data.UpdateAvatar.accept;
            }
            catch
            {
                return false;
            }
        }

        private async void Profile_Load(object sender, EventArgs e)
        {
            // Đặt form ở giữa màn hình
            this.StartPosition = FormStartPosition.CenterScreen;
            
            await Login.LoadFromUrl("https://pminmod.site/doannt106/logo.png", pic_logo);
            MakeCircular(pic_avatar);
            
            // Load thông tin user (luôn request mới từ server để cập nhật)
            await LoadUserInfo();
            
            // Load bài đăng của user
            await LoadUserPosts();
        }

        private async Task LoadUserInfo()
        {
            try
            {
                // Request từ server để lấy thông tin mới nhất (không dùng cache)
                Load_Data.InformationUser = new Load_Data.Data_InformationUserJson
                {
                    action = "get_user_info",
                    username = _username,
                    request_id = Load_Data.GenerateRandomString(4)
                };

                bool success = Load_Data.SendJson(Load_Data.InformationUser);
                if (!success)
                {
                    MessageBox.Show("Không thể kết nối server!", "Lỗi");
                    return;
                }

                bool received = await Load_Data.WaitForServerResponse(
                    () => Load_Data.InformationUser.request_id != null && Load_Data.InformationUser.request_id.Contains("ServerHaha"),
                    timeoutSeconds: 15
                );

                if (!received)
                {
                    MessageBox.Show("Server không phản hồi!", "Lỗi");
                    return;
                }

                if (Load_Data.InformationUser.accept)
                {
                    UpdateUserInfoUI();
                }
                else
                {
                    MessageBox.Show("Không thể tải thông tin: " + Load_Data.InformationUser.error, "Lỗi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi");
            }
        }

        private void UpdateUserInfoUI()
        {
            if (Load_Data.InformationUser == null) return;

            lb_name.Text = Load_Data.InformationUser.username;
            lb_bio.Text = string.IsNullOrEmpty(Load_Data.InformationUser.bio) 
                ? "Chưa có tiểu sử" 
                : Load_Data.InformationUser.bio;
            lb_count.Text = "Số bài đăng: " + Load_Data.InformationUser.count_posts;
            lb_follower.Text = "Follower: " + Load_Data.InformationUser.count_followers;

            // Load avatar
            if (!string.IsNullOrEmpty(Load_Data.InformationUser.avatar_url))
            {
                _ = Login.LoadFromUrl(Load_Data.InformationUser.avatar_url, pic_avatar, showError: false);
            }
        }

        private Panel? postsScrollPanel = null;

        private async Task LoadUserPosts()
        {
            // Tạo scroll panel nếu chưa có
            if (postsScrollPanel == null)
            {
                postsScrollPanel = new Panel
                {
                    Location = new Point(0, 0),
                    Size = gb_posted.Size,
                    AutoScroll = true,
                    BorderStyle = BorderStyle.None
                };
                gb_posted.Controls.Add(postsScrollPanel);
            }
            else
            {
                postsScrollPanel.Controls.Clear();
            }

            // Kiểm tra có posts không
            if (Load_Data.InformationUser?.posts_user == null || Load_Data.InformationUser.posts_user.Count == 0)
            {
                Label lblNoPosts = new Label
                {
                    Text = "Chưa có bài đăng nào",
                    Location = new Point(10, 10),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Gray
                };
                postsScrollPanel.Controls.Add(lblNoPosts);
                return;
            }

            // Tạo từng post (đơn giản hóa, không có ảnh/video)
            int yPosition = 10;
            foreach (var post in Load_Data.InformationUser.posts_user)
            {
                GroupBox gbPost = CreateSimplePostBox(post, yPosition);
                postsScrollPanel.Controls.Add(gbPost);
                yPosition += gbPost.Height + 10;
            }
        }

        private GroupBox CreateSimplePostBox(Load_Data.Data_PostJson post, int yPosition)
        {
            GroupBox gb = new GroupBox
            {
                Text = post.timestamp,
                Location = new Point(10, yPosition),
                Width = postsScrollPanel!.Width - 30,
                AutoSize = false,
                Height = 80
            };

            Label lblContent = new Label
            {
                Text = post.content,
                Location = new Point(10, 20),
                MaximumSize = new Size(gb.Width - 20, 0),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
            gb.Controls.Add(lblContent);

            // Tự động điều chỉnh height
            gb.Height = lblContent.Bottom + 15;

            return gb;
        }

        void MakeCircular(PictureBox pb)
        {
            var gp = new GraphicsPath();
            gp.AddEllipse(0, 0, pb.Width, pb.Height);
            pb.Region = new Region(gp);
        }

        private void pic_logo_Click(object sender, EventArgs e)
        {

        }

        private void btn_main_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btn_profile_Click(object sender, EventArgs e)
        {
            // Refresh profile
            await RefreshProfile();
        }

        private async Task RefreshProfile()
        {
            try
            {
                btn_profile.Enabled = false;
                btn_profile.Text = "Đang tải...";

                await LoadUserInfo();
                await LoadUserPosts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi refresh: " + ex.Message, "Lỗi");
            }
            finally
            {
                btn_profile.Enabled = true;
                btn_profile.Text = "Trang cá nhân";
            }
        }

        private void gb_posted_Enter(object sender, EventArgs e)
        {
            
        }

        private void btn_makepost_Click(object sender, EventArgs e)
        {
            CreatePost createPost = new CreatePost();
            this.Hide();
            createPost.ShowDialog();
            this.Show();
            
            // Refresh sau khi tạo bài mới
            _ = RefreshProfile();
        }
    }
}
