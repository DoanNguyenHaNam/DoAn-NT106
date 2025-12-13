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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.VisualBasic;
using static PostEZ.Load_Data;

namespace PostEZ.Main
{
    public partial class Dashboard : Form
    {
        private System.Threading.Timer? autoRefreshTimer;

        public Dashboard()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private async void Dashboard_Load(object sender, EventArgs e)
        {
            await Login.LoadFromUrl("https://pminmod.site/doannt106/logo.png", pic_logo);


            // ========================================
            // REQUEST: Lấy danh sách bài đăng
            // ========================================
            Load_Data.getFeedResponse = new GetFeedResponse
            {
                action = "get_feed",
                count = 10,
                request_id = Load_Data.GenerateRandomString(4)
            };

            bool success = Load_Data.SendJson(Load_Data.getFeedResponse);
            if (!success)
            {
                MessageBox.Show("Không thể tải dữ liệu từ server!", "Lỗi");
                return;
            }

            bool received = await Load_Data.WaitForServerResponse(
                () => Load_Data.getFeedResponse.request_id != null && Load_Data.getFeedResponse.request_id.Contains("ServerHaha")
            );

            if (!received)
            {
                MessageBox.Show("Server không phản hồi kịp thời. Vui lòng thử lại sau!", "Lỗi");
                return;
            }

            if (Load_Data.getFeedResponse.accept)
            {
                Load_Data.Posts = Load_Data.getFeedResponse.posts;
                await LoadPostsAsync();


                // ========================================
                // REQUEST 1: Lấy thông tin user
                // ========================================
                Load_Data.InformationUser = new Data_InformationUserJson
                {
                    action = "get_user_info",
                    username = Load_Data.LoginData.username, // Thêm username để server biết lấy info của ai
                    request_id = Load_Data.GenerateRandomString(4)
                };

                bool success2 = Load_Data.SendJson(Load_Data.InformationUser);
                if (!success2)
                {
                    MessageBox.Show("Không thể gửi dữ liệu tới server!", "Lỗi");
                    return;
                }

                bool received2 = await Load_Data.WaitForServerResponse(
                    () => Load_Data.InformationUser.request_id != null && Load_Data.InformationUser.request_id.Contains("ServerHaha")
                );

                if (!received2)
                {
                    MessageBox.Show("Server không phản hồi kịp thời (get_user_info). Vui lòng thử lại sau!", "Lỗi");
                    return;
                }

                if (Load_Data.InformationUser.accept)
                {
                    await LoadInforUserInToGroupBox();
                }
                else
                {
                    MessageBox.Show("Không thể tải thông tin người dùng: " + Load_Data.InformationUser.error, "Lỗi");
                }

                // ========================================
                // BẬT AUTO-REFRESH (Tùy chọn - Comment dòng dưới nếu không muốn)
                // ========================================
                StartAutoRefresh();

            }
            else
            {
                MessageBox.Show("Đã gặp lỗi trong quá trình tải bài đăng: " + Load_Data.getFeedResponse.error, "Thông báo");
            }
        }

        private void StartAutoRefresh()
        {
            autoRefreshTimer?.Dispose();
            autoRefreshTimer = new System.Threading.Timer(async _ =>
            {
                if (this.Visible && !this.Modal)
                {
                    this.BeginInvoke(new Action(async () =>
                    {
                        await RefreshFeedSilently();
                    }));
                }
            }, null, 30000, 30000);
        }

        private async Task RefreshFeedSilently()
        {
            try
            {
                Load_Data.getFeedResponse = new GetFeedResponse
                {
                    action = "get_feed",
                    count = 10,
                    request_id = Load_Data.GenerateRandomString(4)
                };

                bool success = Load_Data.SendJson(Load_Data.getFeedResponse);
                if (!success) return;

                bool received = await Load_Data.WaitForServerResponse(
                    () => Load_Data.getFeedResponse.request_id != null && Load_Data.getFeedResponse.request_id.Contains("ServerHaha"),
                    timeoutSeconds: 10
                );

                if (received && Load_Data.getFeedResponse.accept)
                {
                    Load_Data.Posts = Load_Data.getFeedResponse.posts;
                    await LoadPostsAsync();

                    await RefreshUserInfo();
                }
            }
            catch
            {
                
            }
        }

        //=============================================
        //|||             BẮT ÐẦU TẠO POST          |||
        //=============================================
        private async Task LoadInforUserInToGroupBox()
        {
            if (Load_Data.InformationUser == null)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => LoadInforUserInToGroupBox()));
                return;
            }

            lb_username.Text = "User: " + Load_Data.InformationUser.username;
            lb_mail.Text = "Email: " + Load_Data.InformationUser.email;
            lb_countpost.Text = "Số bài dang: " + Load_Data.InformationUser.count_posts;
            lb_countfollower.Text = "Người theo dõi: " + Load_Data.InformationUser.count_followers;
            btn_main.Enabled = true;
            btn_profile.Enabled = true;
            btn_messenge.Enabled = true;
            lb_logout.Enabled = true;
        }

        private Panel? scrollPanel = null;

        private async Task LoadPostsAsync()
        {
            if (scrollPanel == null)
            {
                scrollPanel = new Panel
                {
                    Location = new Point(0, 0),
                    Size = gb_poss.Size,
                    AutoScroll = true,
                    BorderStyle = BorderStyle.None
                };
                gb_poss.Controls.Add(scrollPanel);
            }
            else
            {
                if (scrollPanel.InvokeRequired)
                {
                    scrollPanel.Invoke((MethodInvoker)(() => scrollPanel.Controls.Clear()));
                }
                else
                {
                    scrollPanel.Controls.Clear();
                }
            }

            if (Load_Data.Posts == null || Load_Data.Posts.Count == 0)
            {
                Label lblNoPosts = new Label
                {
                    Text = "Chưa có bài đăng nào",
                    Location = new Point(10, 10),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Gray
                };
                scrollPanel.Controls.Add(lblNoPosts);
                return;
            }

            int yPosition = 10;

            foreach (var post in Load_Data.Posts)
            {
                GroupBox gb_eachpost = await CreatePostGroupBoxAsync(post, yPosition);
                scrollPanel.Controls.Add(gb_eachpost);

                yPosition += gb_eachpost.Height + 10;
            }
        }

        private bool IsYouTubeUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return url.Contains("youtube.com") ||
                   url.Contains("youtu.be") ||
                   url.Contains("youtube-nocookie.com");
        }

        private bool IsServerVideoUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return url.Contains("160.191.245.144");
        }

        private bool IsValidVideoUrl(string? url)
        {
            return IsYouTubeUrl(url) || IsServerVideoUrl(url);
        }

        private string? GetYouTubeVideoId(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var match = Regex.Match(url, @"(?:youtube\.com\/watch\?v=|youtu\.be\/)([a-zA-Z0-9_-]{11})");
            if (match.Success)
                return match.Groups[1].Value;

            match = Regex.Match(url, @"youtube\.com\/embed\/([a-zA-Z0-9_-]{11})");
            if (match.Success)
                return match.Groups[1].Value;

            return null;
        }

        private async Task<GroupBox> CreatePostGroupBoxAsync(Data_PostJson post, int yPosition)
        {
            // ========================================
            // TẠO CARD CHO BÀI ĐĂNG
            // ========================================
            Panel cardPanel = new Panel
            {
                Location = new Point(10, yPosition),
                Width = scrollPanel!.Width - 30,
                AutoSize = false,
                Height = 100,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(15)
            };

            cardPanel.Paint += (s, e) =>
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    var rect = new Rectangle(2, 2, cardPanel.Width - 4, cardPanel.Height - 4);
                    var path = GetRoundedRectangle(rect, 10);
                    e.Graphics.FillPath(shadowBrush, path);
                }
                
                using (var cardBrush = new SolidBrush(Color.White))
                using (var borderPen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    var rect = new Rectangle(0, 0, cardPanel.Width - 1, cardPanel.Height - 1);
                    var path = GetRoundedRectangle(rect, 8);
                    e.Graphics.FillPath(cardBrush, path);
                    e.Graphics.DrawPath(borderPen, path);
                }
            };

            GroupBox gb_eachpost = new GroupBox
            {
                Location = new Point(0, 0),
                Width = cardPanel.Width - 30,
                AutoSize = false,
                Height = 100,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
            gb_eachpost.Paint += (s, e) =>
            {
                e.Graphics.Clear(Color.White);
            };

            int currentY = 10;

            // ========================================
            // HEADER: AVATAR + USERNAME + TIMESTAMP
            // ========================================
            Panel headerPanel = new Panel
            {
                Location = new Point(10, currentY),
                Width = gb_eachpost.Width - 20,
                Height = 50,
                BackColor = Color.White
            };

            PictureBox picAvatar = new PictureBox
            {
                Location = new Point(0, 0),
                Size = new Size(45, 45),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            
            System.Drawing.Drawing2D.GraphicsPath avatarPath = new System.Drawing.Drawing2D.GraphicsPath();
            avatarPath.AddEllipse(0, 0, picAvatar.Width, picAvatar.Height);
            picAvatar.Region = new Region(avatarPath);
            
            _ = LoadUserAvatarAsync(post.username, picAvatar);
            
            headerPanel.Controls.Add(picAvatar);

            Label lblUsername = new Label
            {
                Text = post.username,
                Location = new Point(55, 5),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                BackColor = Color.White
            };
            headerPanel.Controls.Add(lblUsername);

            Label lblTime = new Label
            {
                Text = FormatTimestamp(post.timestamp),
                Location = new Point(55, 27),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                BackColor = Color.White
            };
            headerPanel.Controls.Add(lblTime);

            gb_eachpost.Controls.Add(headerPanel);
            currentY += headerPanel.Height + 10;

            // ========================================
            // CONTENT
            // ========================================
            if (!string.IsNullOrEmpty(post.content))
            {
                Label lblContent = new Label
                {
                    Text = post.content,
                    Location = new Point(15, currentY),
                    MaximumSize = new Size(gb_eachpost.Width - 30, 0),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(60, 60, 60),
                    BackColor = Color.White
                };
                gb_eachpost.Controls.Add(lblContent);
                currentY += lblContent.Height + 15;
            }

            // ========================================
            // IMAGE
            // ========================================
            if (!string.IsNullOrEmpty(post.image_url))
            {
                Panel imageContainer = new Panel
                {
                    Location = new Point(15, currentY),
                    Width = gb_eachpost.Width - 30,
                    Height = ((gb_eachpost.Width - 30) / 16) * 9,
                    BackColor = Color.White
                };
                
                PictureBox picImage = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.White
                };
                
                imageContainer.Controls.Add(picImage);
                gb_eachpost.Controls.Add(imageContainer);

                await LoadImageAsync(post.image_url, picImage);

                currentY += imageContainer.Height + 15;
            }

            // ========================================
            // VIDEO
            // ========================================
            if (!string.IsNullOrEmpty(post.video_url))
            {
                if (IsValidVideoUrl(post.video_url))
                {
                    Panel videoPanel = new Panel
                    {
                        Location = new Point(15, currentY),
                        Width = gb_eachpost.Width - 30,
                        Height = ((gb_eachpost.Width - 30) / 16) * 9,
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.White
                    };

                    Label lblLoadVideo = new Label
                    {
                        Text = "▶️ Nhấn để phát video",
                        ForeColor = Color.FromArgb(70, 130, 180),
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        AutoSize = false,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Width = videoPanel.Width,
                        Height = videoPanel.Height,
                        Cursor = Cursors.Hand,
                        BackColor = Color.FromArgb(240, 248, 255),
                        Tag = post.video_url
                    };

                    lblLoadVideo.MouseEnter += (s, e) =>
                    {
                        lblLoadVideo.BackColor = Color.FromArgb(220, 235, 255);
                        lblLoadVideo.ForeColor = Color.FromArgb(50, 100, 150);
                    };
                    lblLoadVideo.MouseLeave += (s, e) =>
                    {
                        lblLoadVideo.BackColor = Color.FromArgb(240, 248, 255);
                        lblLoadVideo.ForeColor = Color.FromArgb(70, 130, 180);
                    };

                    lblLoadVideo.Click += async (s, e) =>
                    {
                        Label? lbl = s as Label;
                        if (lbl != null && !string.IsNullOrEmpty(lbl.Tag?.ToString()))
                        {
                            string? videoUrl = lbl.Tag?.ToString();
                            if (string.IsNullOrEmpty(videoUrl)) return;

                            videoPanel.Controls.Clear();

                            WebView2 webView = new WebView2
                            {
                                Dock = DockStyle.Fill
                            };

                            videoPanel.Controls.Add(webView);

                            try
                            {
                                await webView.EnsureCoreWebView2Async(null);

                                string videoHtml = "";

                                if (IsYouTubeUrl(videoUrl))
                                {
                                    string? videoId = GetYouTubeVideoId(videoUrl);

                                    if (!string.IsNullOrEmpty(videoId))
                                    {
                                        videoHtml = $@"
                                            <!DOCTYPE html>
                                            <html>
                                            <head>
                                                <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                                                <style>
                                                    * {{ margin: 0; padding: 0; box-sizing: border-box; }}
                                                    body {{ background: #fff; color: #000; display: flex; align-items: center; justify-content: center; min-height: 100vh; }}
                                                    .video-card {{ width: 100%; max-width: 900px; cursor: pointer; position: relative; border-radius: 0; overflow: hidden; }}
                                                    .video-card::before {{ content: """"; display: block; padding-bottom: 56.25%; }}
                                                    .video-card img, .video-card iframe {{ position: absolute; inset: 0; width: 100%; height: 100%; object-fit: contain; display: block; border: 0; }}
                                                    .play {{
                                                        position: absolute; left: 50%; top: 50%; transform: translate(-50%, -50%); width: 76px; height: 76px;
                                                        border-radius: 50%; background: rgba(0, 0, 0, 0.6); display: flex; align-items: center; justify-content: center;
                                                    }}
                                                    .play svg {{ width: 36px; height: 36px; fill: #fff; margin-left: 6px; }}
                                                </style>
                                            </head>
                                            <body>
                                                <div id=""videoRoot"" class=""video-card"" role=""button"" tabindex=""0"" aria-label=""Open on YouTube""></div>

                                                <script>
                                                    const videoId = ""{videoId}"";
                                                    const openDirectOnClick = true;

                                                    const root = document.getElementById('videoRoot');

                                                    const img = document.createElement('img');
                                                    img.alt = 'YouTube thumbnail';
                                                    img.loading = 'lazy';

                                                    const thumbs = [
                                                        `https://i.ytimg.com/vi/${{videoId}}/maxresdefault.jpg`,
                                                        `https://i.ytimg.com/vi/${{videoId}}/sddefault.jpg`,
                                                        `https://i.ytimg.com/vi/${{videoId}}/hqdefault.jpg`,
                                                        `https://i.ytimg.com/vi/${{videoId}}/mqdefault.jpg`,
                                                        `https://i.ytimg.com/vi/${{videoId}}/default.jpg`
                                                    ];

                                                    (function tryThumb(i = 0) {{
                                                        if (i >= thumbs.length) {{ img.src = ''; return; }}
                                                        const test = new Image();
                                                        test.onload = () => {{ img.src = thumbs[i]; }};
                                                        test.onerror = () => tryThumb(i + 1);
                                                        test.src = thumbs[i] + '?r=' + Date.now();
                                                    }})();

                                                    const play = document.createElement('div');
                                                    play.className = 'play';
                                                    play.innerHTML = `<svg viewBox=""0 0 68 48"" xmlns=""http://www.w3.org/2000/svg"" focusable=""false"" aria-hidden=""true"">
                                                        <path d=""M66.52 7.07c-0.76-2.83-3-5.06-5.83-5.83C56.03 0 34 0 34 0s-22.03 0-26.69 1.24c-2.83.77-5.07 3-5.83 5.83C0 11.73 0 24 0 24s0 12.27 1.48 16.93c.76 2.83 3 5.06 5.83 5.83C11.97 48 34 48 34 48s22.03 0 26.69-1.24c2.83-.77 5.07-3 5.83-5.83C68 36.27 68 24 68 24s0-12.27-1.48-16.93z"" />
                                                        <path d=""M45 24L27 14v20z"" fill=""#FF0000""/>
                                                    </svg>`;

                                                    root.appendChild(img);
                                                    root.appendChild(play);

                                                    function openYoutube() {{
                                                        const url = `https://www.youtube.com/watch?v=${{videoId}}`;
                                                        window.open(url, '_blank', 'noopener,noreferrer');
                                                    }}

                                                    function tryEmbedAutoplay() {{
                                                        const iframe = document.createElement('iframe');
                                                        iframe.src = `https://www.youtube-nocookie.com/embed/${{videoId}}?autoplay=1&mute=1&rel=0&modestbranding=1`;
                                                        iframe.setAttribute('allow', 'accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share');
                                                        iframe.loading = 'lazy';
                                                        iframe.allowFullscreen = true;
                                                        root.innerHTML = '';
                                                        root.appendChild(iframe);
                                                    }}

                                                    root.addEventListener('click', function() {{
                                                        if (openDirectOnClick) return openYoutube();
                                                        tryEmbedAutoplay();
                                                    }});

                                                    root.addEventListener('keydown', function(e) {{
                                                        if (e.key === 'Enter' || e.key === ' ') {{
                                                            e.preventDefault();
                                                            if (openDirectOnClick) openYoutube(); else tryEmbedAutoplay();
                                                        }}
                                                    }});
                                                </script>
                                            </body>
                                            </html>
                                        ";
                                    }
                                }
                                else if (IsServerVideoUrl(videoUrl))
                                {
                                    videoHtml = $@"
                                        <!DOCTYPE html>
                                        <html>
                                        <head>
                                            <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                                            <style>
                                                * {{ margin: 0; padding: 0; box-sizing: border-box; }}
                                                body {{ background: white; overflow: hidden; display: flex; align-items: center; justify-content: center; height: 100vh; }}
                                                video {{ width: 100%; height: 100%; object-fit: contain; }}
                                            </style>
                                        </head>
                                        <body>
                                            <video controls autoplay>
                                                <source src=""{videoUrl}"" type=""video/mp4"">
                                                <source src=""{videoUrl}"" type=""video/webm"">
                                                <source src=""{videoUrl}"" type=""video/ogg"">
                                                Trình duyệt không hỗ trợ video.
                                            </video>
                                        </body>
                                        </html>
                                    ";
                                }

                                if (!string.IsNullOrEmpty(videoHtml))
                                {
                                    webView.CoreWebView2.NavigateToString(videoHtml);
                                }
                                else
                                {
                                    throw new Exception("Không thể tạo video player");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Không thể phát video: " + ex.Message, "Lỗi");

                                Label lblError = new Label
                                {
                                    Text = "❌ Lỗi: " + ex.Message,
                                    ForeColor = Color.Red,
                                    AutoSize = false,
                                    TextAlign = ContentAlignment.MiddleCenter,
                                    Width = videoPanel.Width,
                                    Height = videoPanel.Height,
                                    BackColor = Color.White
                                };
                                videoPanel.Controls.Clear();
                                videoPanel.Controls.Add(lblError);
                            }
                        }
                    };

                    videoPanel.Controls.Add(lblLoadVideo);
                    gb_eachpost.Controls.Add(videoPanel);
                    currentY += videoPanel.Height + 15;
                }
                else
                {
                    Label lblInvalidVideo = new Label
                    {
                        Text = "🚫 Video từ nguồn không được phép",
                        Location = new Point(15, currentY),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.Orange,
                        BackColor = Color.White
                    };
                    gb_eachpost.Controls.Add(lblInvalidVideo);
                    currentY += lblInvalidVideo.Height + 15;
                }
            }

            // ========================================
            // ACTION BUTTONS
            // ========================================
            Panel actionPanel = new Panel
            {
                Location = new Point(10, currentY),
                Width = gb_eachpost.Width - 20,
                Height = 45,
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.None
            };

            actionPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(250, 250, 250)))
                {
                    var rect = new Rectangle(0, 0, actionPanel.Width, actionPanel.Height);
                    var path = GetRoundedRectangle(rect, 8);
                    e.Graphics.FillPath(brush, path);
                }
            };

            Button btnLike = new Button
            {
                Text = $"🤍 {post.like_count}",
                Location = new Point(0, 5),
                Size = new Size(120, 30),
                Tag = post.id,
                Font = new Font("Segoe UI Emoji", 9), // <- use emoji-capable font
                ForeColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLike.FlatAppearance.BorderColor = Color.LightGray;
            btnLike.Click += btn_like_Click;
            actionPanel.Controls.Add(btnLike);

            Button btnComment = new Button
            {
                Text = $"💬 {post.comment_count}",
                Location = new Point(150, 8),
                Size = new Size(130, 32),
                Tag = post.id,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnComment.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            btnComment.FlatAppearance.BorderSize = 1;
            btnComment.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 248, 255);
            btnComment.Click += btn_comment_Click;
            
            // Hover effect
            btnComment.MouseEnter += (s, e) =>
            {
                btnComment.ForeColor = Color.FromArgb(0, 123, 255);
            };
            btnComment.MouseLeave += (s, e) =>
            {
                btnComment.ForeColor = Color.FromArgb(100, 100, 100);
            };
            
            actionPanel.Controls.Add(btnComment);

            gb_eachpost.Controls.Add(actionPanel);
            currentY += actionPanel.Height + 15;

            gb_eachpost.Height = currentY;
            cardPanel.Height = currentY + 20;
            
            cardPanel.Controls.Add(gb_eachpost);
            
            // HACK
            GroupBox wrapper = new GroupBox
            {
                Location = cardPanel.Location,
                Width = cardPanel.Width,
                Height = cardPanel.Height,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
            wrapper.Paint += (s, e) => e.Graphics.Clear(Color.White);
            wrapper.Controls.Add(cardPanel);
            cardPanel.Location = new Point(0, 0);
            
            return wrapper;
        }

        // ========================================
        // HELPER
        // ========================================
        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int diameter = radius * 2;
            var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);

            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        // ========================================
        // HELPER
        // ========================================
        private async Task LoadUserAvatarAsync(string username, PictureBox pictureBox)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(3);
                    string defaultAvatarUrl = "http://160.191.245.144/doanNT106/DB/USER/avatar/5.jpg";
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(defaultAvatarUrl);
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        pictureBox.Image = Image.FromStream(ms);
                    }
                }
            }
            catch
            {
                pictureBox.BackColor = Color.WhiteSmoke;
                Label lblInitial = new Label
                {
                    Text = username.Length > 0 ? username.Substring(0, 1).ToUpper() : "?",
                    ForeColor = Color.Gray,
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent
                };
                pictureBox.Controls.Add(lblInitial);
            }
        }

        // ========================================
        // HELPER
        // ========================================
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

        // ========================================
        // HANDLER NÚT LIKE
        // ========================================
        private async void btn_like_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int postId = (int)btn.Tag;

            try
            {
                btn.Enabled = false;

                var request = new
                {
                    action = "like_post",
                    post_id = postId,
                    username = Load_Data.LoginData.username,
                    request_id = Load_Data.GenerateRandomString(4)
                };

                bool success = Load_Data.SendJson(request);
                if (!success)
                {
                    MessageBox.Show("Không thể kết nối server!", "Lỗi");
                    return;
                }

                bool received = await Load_Data.WaitForServerResponse(
                    () => Load_Data.LikePostResponse.request_id != null && Load_Data.LikePostResponse.request_id.Contains("ServerHaha")
                );

                if (received && Load_Data.LikePostResponse.accept)
                {
                    if (Load_Data.LikePostResponse.liked)
                    {
                        btn.Text = $"🤍 {Load_Data.LikePostResponse.like_count}";
                        btn.ForeColor = Color.Red;
                    }
                    else
                    {
                        btn.Text = $"🤍 {Load_Data.LikePostResponse.like_count}";
                        btn.ForeColor = Color.Gray;
                    }
                }
                else if (received)
                {
                    MessageBox.Show(Load_Data.LikePostResponse.error, "Lỗi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi");
            }
            finally
            {
                btn.Enabled = true;
            }
        }

        // ========================================
        // HANDLER NÚT COMMENT
        // ========================================
        private void btn_comment_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int postId = (int)btn.Tag;

            try
            {
                var post = Load_Data.Posts.FirstOrDefault(p => p.id == postId);
                if (post == null)
                {
                    MessageBox.Show("Không tìm thấy bài viết!", "Lỗi");
                    return;
                }

                PostComment commentForm = new PostComment(postId, post.username, post.content);
                commentForm.ShowDialog();

                _ = RefreshFeed();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi");
            }
        }

        private async Task LoadImageAsync(string imageUrl, PictureBox pictureBox)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                    using (var ms = new MemoryStream(imageBytes))
                    {
                        pictureBox.Image = Image.FromStream(ms);
                    }
                }
            }
            catch
            {
                pictureBox.BackColor = Color.LightGray;
                Label lblError = new Label
                {
                    Text = "❌ Không tải được ảnh",
                    ForeColor = Color.Red,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Width = pictureBox.Width,
                    Height = pictureBox.Height
                };
                pictureBox.Controls.Add(lblError);
            }
        }

        //=============================================
        //|||             KẾT THÚC TẠO POST         |||
        //=============================================

        private async void btn_main_Click(object sender, EventArgs e)
        {
            await RefreshFeed();
        }

        private async Task RefreshFeed()
        {
            try
            {
                btn_main.Enabled = false;
                btn_main.Text = "Đang tải...";
                Load_Data.getFeedResponse = new GetFeedResponse
                {
                    action = "get_feed",
                    count = 10,
                    request_id = Load_Data.GenerateRandomString(4)
                };

                bool success = Load_Data.SendJson(Load_Data.getFeedResponse);
                if (!success)
                {
                    MessageBox.Show("Không thể kết nối server!", "Lỗi");
                    return;
                }

                bool received = await Load_Data.WaitForServerResponse(
                    () => Load_Data.getFeedResponse.request_id != null && Load_Data.getFeedResponse.request_id.Contains("ServerHaha")
                );

                if (!received)
                {
                    MessageBox.Show("Server không phản hồi!", "Lỗi");
                    return;
                }

                if (Load_Data.getFeedResponse.accept)
                {
                    Load_Data.Posts = Load_Data.getFeedResponse.posts;
                    await LoadPostsAsync();

                    await RefreshUserInfo();
                }
                else
                {
                    MessageBox.Show("Lỗi: " + Load_Data.getFeedResponse.error, "Lỗi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi refresh: " + ex.Message, "Lỗi");
            }
            finally
            {
                btn_main.Enabled = true;
                btn_main.Text = "Trang chủ";
            }
        }

        private async Task RefreshUserInfo()
        {
            try
            {
                Load_Data.InformationUser = new Data_InformationUserJson
                {
                    action = "get_user_info",
                    username = Load_Data.LoginData.username,
                    request_id = Load_Data.GenerateRandomString(4)
                };

                bool success = Load_Data.SendJson(Load_Data.InformationUser);
                if (!success) return;

                bool received = await Load_Data.WaitForServerResponse(
                    () => Load_Data.InformationUser.request_id != null && Load_Data.InformationUser.request_id.Contains("ServerHaha")
                );

                if (received && Load_Data.InformationUser.accept)
                {
                    await LoadInforUserInToGroupBox();
                }
            }
            catch
            {

            }
        }

        private async void btn_profile_Click(object sender, EventArgs e)
        {
            Profile profileForm = new Profile(Load_Data.LoginData.username);
            this.Hide();
            profileForm.ShowDialog();
            this.Show();
            await RefreshUserInfo();
        }

        private void gb_info_Enter(object sender, EventArgs e)
        {

        }

        private async void lb_logout_Click(object sender, EventArgs e)
        {
            try
            {
                bool logoutSuccess = await Load_Data.Logout();
                
                if (logoutSuccess)
                {
                    MessageBox.Show("Đăng xuất thành công!", "Thông báo");
                }
                
                Load_Data.LoginData.accept = false;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng xuất: " + ex.Message, "Lỗi");
                Load_Data.LoginData.accept = false;
                this.Close();
            }
        }

        private void lb_username_Click(object sender, EventArgs e)
        {

        }

        private System.Threading.Timer? searchTimer;

        private void tb_find_TextChanged(object sender, EventArgs e)
        {
            searchTimer?.Dispose();
            searchTimer = new System.Threading.Timer(_ =>
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(async () =>
                    {
                        string searchText = tb_find.Text.Trim();

                        if (string.IsNullOrWhiteSpace(searchText))
                        {
                            await LoadPostsAsync();
                        }
                        else
                        {
                            await FilterAndDisplayPosts(searchText);
                        }
                    }));
                }
            }, null, 300, Timeout.Infinite);
        }

        private async Task FilterAndDisplayPosts(string searchText)
        {
            if (Load_Data.Posts == null || Load_Data.Posts.Count == 0)
            {
                await LoadPostsAsync();
                return;
            }

            var filteredPosts = Load_Data.Posts
                .Where(post =>
                    (!string.IsNullOrEmpty(post.content) && post.content.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(post.username) && post.username.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0))
                .ToList();

            var originalPosts = Load_Data.Posts;
            Load_Data.Posts = filteredPosts;
            await LoadPostsAsync();
            Load_Data.Posts = originalPosts;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            autoRefreshTimer?.Dispose();
            searchTimer?.Dispose();
            base.OnFormClosing(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Message message = new Message();
            this.Hide();
            message.ShowDialog();
            this.Show();
        }
    }
}
