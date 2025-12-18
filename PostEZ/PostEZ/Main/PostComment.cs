using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostEZ.Main
{
    public partial class PostComment : Form
    {
        private readonly int _postId;
        private readonly string _postUsername;
        private readonly string _postContent;
        
        private Panel? commentsPanel = null;
        private TextBox? txtComment = null;
        private Button? btnSendComment = null;
        private Label? lblCommentCount = null;
        
        public PostComment(int postId, string postUsername, string postContent)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            _postId = postId;
            _postUsername = postUsername;
            _postContent = postContent;
            
            InitializeCommentUI();
        }

        private void InitializeCommentUI()
        {
            this.Text = $"Bình luận - Bài viết của {_postUsername}";
            
            GroupBox gbPost = new GroupBox
            {
                Text = $"Bài viết của {_postUsername}",
                Location = new Point(153, 12),
                Size = new Size(635, 100),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            
            Label lblPostContent = new Label
            {
                Text = _postContent.Length > 200 ? _postContent.Substring(0, 200) + "..." : _postContent,
                Location = new Point(10, 25),
                MaximumSize = new Size(615, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black
            };
            gbPost.Controls.Add(lblPostContent);
            this.Controls.Add(gbPost);
            
            lblCommentCount = new Label
            {
                Text = "💬 Đang tải comments...",
                Location = new Point(153, 122),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DodgerBlue
            };
            this.Controls.Add(lblCommentCount);
            
            commentsPanel = new Panel
            {
                Location = new Point(153, 150),
                Size = new Size(635, 230),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(commentsPanel);
            
            txtComment = new TextBox
            {
                Location = new Point(153, 390),
                Size = new Size(525, 40),
                Multiline = true,
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Nhập bình luận của bạn..."
            };
            this.Controls.Add(txtComment);
            
            btnSendComment = new Button
            {
                Text = "📤 Gửi",
                Location = new Point(688, 390),
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSendComment.FlatAppearance.BorderSize = 0;
            btnSendComment.Click += BtnSendComment_Click;
            this.Controls.Add(btnSendComment);
            
            btn_main.Click += (s, e) => this.Close();
            btn_profile.Click += (s, e) => this.Close();
        }

        private async void PostComment_Load(object sender, EventArgs e)
        {
            await PostEZ.Log.Login.LoadFromUrl("https://raw.githubusercontent.com/DoanNguyenHaNam/DoAn-NT106/main/Sources_NotNecessery/Logo.png", pic_logo);
            await LoadComments();
        }

        private async Task LoadComments()
        {
            try
            {
                if (lblCommentCount != null)
                {
                    lblCommentCount.Text = "💬 Đang tải comments...";
                }

                var request = new
                {
                    action = "get_comments",
                    post_id = _postId,
                    request_id = Load_Data.GenerateRandomString(4)
                };

                bool success = Load_Data.SendJson(request);
                if (!success)
                {
                    MessageBox.Show("Không thể kết nối server!", "Lỗi");
                    return;
                }

                bool received = await Load_Data.WaitForServerResponse(
                    () => Load_Data.GetCommentsResponse.request_id != null && 
                          Load_Data.GetCommentsResponse.request_id.Contains("ServerHaha"),
                    timeoutSeconds: 10
                );

                if (received && Load_Data.GetCommentsResponse.accept)
                {
                    DisplayComments(Load_Data.GetCommentsResponse.comments);
                    
                    if (lblCommentCount != null)
                    {
                        lblCommentCount.Text = $"💬 {Load_Data.GetCommentsResponse.count} bình luận";
                    }
                }
                else
                {
                    if (lblCommentCount != null)
                    {
                        lblCommentCount.Text = "💬 0 bình luận";
                    }
                    
                    Label lblNoComments = new Label
                    {
                        Text = "Chưa có bình luận nào. Hãy là người đầu tiên!",
                        AutoSize = true,
                        Location = new Point(10, 10),
                        Font = new Font("Segoe UI", 10),
                        ForeColor = Color.Gray
                    };
                    commentsPanel?.Controls.Add(lblNoComments);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải comments: " + ex.Message, "Lỗi");
            }
        }

        private void DisplayComments(List<Load_Data.Data_CommentJson>? comments)
        {
            if (commentsPanel == null) return;
            
            commentsPanel.Controls.Clear();
            
            if (comments == null || comments.Count == 0)
            {
                Label lblNoComments = new Label
                {
                    Text = "Chưa có bình luận nào. Hãy là người đầu tiên!",
                    AutoSize = true,
                    Location = new Point(10, 10),
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Gray
                };
                commentsPanel.Controls.Add(lblNoComments);
                return;
            }

            int yPosition = 10;
            foreach (var comment in comments)
            {
                Panel commentPanel = CreateCommentPanel(comment, yPosition);
                commentsPanel.Controls.Add(commentPanel);
                yPosition += commentPanel.Height + 10;
            }
        }

        private Panel CreateCommentPanel(Load_Data.Data_CommentJson comment, int yPosition)
        {
            Panel panel = new Panel
            {
                Location = new Point(5, yPosition),
                Width = commentsPanel!.Width - 25,
                AutoSize = false,
                Height = 70,
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblUsername = new Label
            {
                Text = $"👤 {comment.username}",
                Location = new Point(10, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DodgerBlue
            };
            panel.Controls.Add(lblUsername);

            Label lblTime = new Label
            {
                Text = FormatTimestamp(comment.timestamp),
                Location = new Point(panel.Width - 150, 8),
                Width = 140,
                TextAlign = ContentAlignment.TopRight,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };
            panel.Controls.Add(lblTime);

            Label lblContent = new Label
            {
                Text = comment.content,
                Location = new Point(10, 30),
                MaximumSize = new Size(panel.Width - 20, 0),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black
            };
            panel.Controls.Add(lblContent);

            panel.Height = Math.Max(70, lblContent.Bottom + 10);

            return panel;
        }

        private string FormatTimestamp(string timestamp)
        {
            try
            {
                if (long.TryParse(timestamp, out long unixTime))
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTime);
                    DateTime localTime = dateTimeOffset.LocalDateTime;
                    
                    TimeSpan diff = DateTime.Now - localTime;
                    
                    if (diff.TotalMinutes < 1)
                        return "Vừa xong";
                    if (diff.TotalMinutes < 60)
                        return $"{(int)diff.TotalMinutes} phút trước";
                    if (diff.TotalHours < 24)
                        return $"{(int)diff.TotalHours} giờ trước";
                    if (diff.TotalDays < 7)
                        return $"{(int)diff.TotalDays} ngày trước";
                    
                    return localTime.ToString("dd/MM/yyyy HH:mm");
                }
                return timestamp;
            }
            catch
            {
                return timestamp;
            }
        }

        private async void BtnSendComment_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtComment?.Text))
            {
                MessageBox.Show("Vui lòng nhập nội dung bình luận!", "Thông báo");
                return;
            }

            try
            {
                if (btnSendComment != null)
                {
                    btnSendComment.Enabled = false;
                    btnSendComment.Text = "⏳ Đang gửi...";
                }

                var request = new
                {
                    action = "add_comment",
                    post_id = _postId,
                    username = Load_Data.LoginData.username,
                    content = txtComment?.Text,
                    request_id = Load_Data.GenerateRandomString(4)
                };

                bool success = Load_Data.SendJson(request);
                if (!success)
                {
                    MessageBox.Show("Không thể kết nối server!", "Lỗi");
                    return;
                }

                bool received = await Load_Data.WaitForServerResponse(
                    () => Load_Data.AddCommentResponse.request_id != null && 
                          Load_Data.AddCommentResponse.request_id.Contains("ServerHaha"),
                    timeoutSeconds: 10
                );

                if (received && Load_Data.AddCommentResponse.accept)
                {
                    if (txtComment != null)
                    {
                        txtComment.Clear();
                    }
                    await LoadComments();

                    MessageBox.Show("Đã thêm bình luận!", "Thành công");
                }
                else if (received)
                {
                    MessageBox.Show(Load_Data.AddCommentResponse.error, "Lỗi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi");
            }
            finally
            {
                if (btnSendComment != null)
                {
                    btnSendComment.Enabled = true;
                    btnSendComment.Text = "📤 Gửi";
                }
            }
        }
    }
}
