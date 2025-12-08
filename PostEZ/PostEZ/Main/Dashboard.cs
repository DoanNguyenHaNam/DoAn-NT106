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
using static PostEZ.Load_Data;

namespace PostEZ.Main
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private async void Dashboard_Load(object sender, EventArgs e)
        {
            await Login.LoadFromUrl("https://pminmod.site/doannt106/logo.png", pic_logo);
            Load_Data.getFeedResponse = new GetFeedResponse
            {
                action = "get_feed",
                count = 10,
                request_id = Load_Data.GenerateRandomString(4)
            };
            bool success = Load_Data.SendJson(Load_Data.getFeedResponse);
            if (!success)
            {
                MessageBox.Show("Không thể gửi dữ liệu tới server!", "Lỗi");
                return;
            }

            int i = 0;

            while (!Load_Data.getFeedResponse.request_id.Contains("ServerHaha"))
            {
                i += 1;
                if (i >= 100)
                {
                    MessageBox.Show("Server không phản hồi kịp thời. Vui lòng thử lại sau!", "Lỗi");
                    return;
                }
                await Task.Delay(100);

                continue;
            }


            if (Load_Data.getFeedResponse.accept)
            {
                Load_Data.Posts = Load_Data.getFeedResponse.posts;
                await LoadPostsAsync();
            }
            else
            {
                MessageBox.Show("Đã gặp lỗi trong quá trình tải bài đăng", "Thông báo");
            }

        }

        //=============================================
        //|||             BẮT ĐẦU TẠO POST          |||
        //=============================================
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

            // FIX: Kiểm tra null trước khi foreach
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

        // Kiểm tra URL có phải YouTube không
        private bool IsYouTubeUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return url.Contains("youtube.com") ||
                   url.Contains("youtu.be") ||
                   url.Contains("youtube-nocookie.com");
        }

        // Kiểm tra URL có phải từ server (160.191.245.144) không
        private bool IsServerVideoUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return url.Contains("160.191.245.144");
        }

        // Kiểm tra URL có hợp lệ không (chỉ cho phép YouTube hoặc Server)
        private bool IsValidVideoUrl(string? url)
        {
            return IsYouTubeUrl(url) || IsServerVideoUrl(url);
        }

        // Lấy YouTube Video ID từ URL
        private string? GetYouTubeVideoId(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            // Xử lý URL dạng: https://www.youtube.com/watch?v=VIDEO_ID
            var match = Regex.Match(url, @"(?:youtube\.com\/watch\?v=|youtu\.be\/)([a-zA-Z0-9_-]{11})");
            if (match.Success)
                return match.Groups[1].Value;

            // Xử lý URL dạng: https://www.youtube.com/embed/VIDEO_ID
            match = Regex.Match(url, @"youtube\.com\/embed\/([a-zA-Z0-9_-]{11})");
            if (match.Success)
                return match.Groups[1].Value;

            return null;
        }

        private async Task<GroupBox> CreatePostGroupBoxAsync(Data_PostJson post, int yPosition)
        {
            GroupBox gb_eachpost = new GroupBox
            {
                Text = post.username,
                Location = new Point(10, yPosition),
                Width = scrollPanel!.Width - 50,
                AutoSize = false,
                Height = 100
            };
            int currentY = 20;

            // Timestamp
            Label lblTime = new Label
            {
                Text = post.timestamp,
                Location = new Point(10, currentY),
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };
            gb_eachpost.Controls.Add(lblTime);
            currentY += lblTime.Height + 5;

            // Content
            if (!string.IsNullOrEmpty(post.content))
            {
                Label lblContent = new Label
                {
                    Text = post.content,
                    Location = new Point(10, currentY),
                    MaximumSize = new Size(gb_eachpost.Width - 30, 0),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9)
                };
                gb_eachpost.Controls.Add(lblContent);
                currentY += lblContent.Height + 10;
            }

            // Image
            if (!string.IsNullOrEmpty(post.image_url))
            {
                PictureBox picImage = new PictureBox
                {
                    Location = new Point(10, currentY),
                    Width = gb_eachpost.Width - 30,
                    Height = 200,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BorderStyle = BorderStyle.FixedSingle
                };
                gb_eachpost.Controls.Add(picImage);

                await LoadImageAsync(post.image_url, picImage);

                currentY += picImage.Height + 10;
            }

            // Video - CHỈNH HẾT VỀ WEBVIEW2
            if (!string.IsNullOrEmpty(post.video_url))
            {
                // Kiểm tra URL có hợp lệ không
                if (IsValidVideoUrl(post.video_url))
                {
                    Panel videoPanel = new Panel
                    {
                        Location = new Point(10, currentY),
                        Width = gb_eachpost.Width - 30,
                        Height = 250,
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.Black
                    };

                    // Tạo label để load video khi click
                    Label lblLoadVideo = new Label
                    {
                        Text = "▶ Nhấn để phát video",
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        AutoSize = false,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Width = videoPanel.Width,
                        Height = videoPanel.Height,
                        Cursor = Cursors.Hand,
                        Tag = post.video_url
                    };

                    lblLoadVideo.Click += async (s, e) =>
                    {
                        Label? lbl = s as Label;
                        if (lbl != null && !string.IsNullOrEmpty(lbl.Tag?.ToString()))
                        {
                            string? videoUrl = lbl.Tag?.ToString();
                            if (string.IsNullOrEmpty(videoUrl)) return;

                            videoPanel.Controls.Clear();

                            // Tạo WebView2
                            WebView2 webView = new WebView2
                            {
                                Dock = DockStyle.Fill
                            };

                            videoPanel.Controls.Add(webView);

                            try
                            {
                                await webView.EnsureCoreWebView2Async(null);

                                string videoHtml = "";

                                // Kiểm tra loại video
                                if (IsYouTubeUrl(videoUrl))
                                {
                                    // YOUTUBE: Dùng iframe embed - FIX Error 153
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
        body {{ background: #000; color: #fff; display: flex; align-items: center; justify-content: center; min-height: 100vh; }}
        .video-card {{ width: 100%; max-width: 900px; cursor: pointer; position: relative; border-radius: 8px; overflow: hidden; }}
        .video-card::before {{ content: """"; display: block; padding-bottom: 56.25%; }}
        .video-card img, .video-card iframe {{ position: absolute; inset: 0; width: 100%; height: 100%; object-fit: cover; display: block; border: 0; }}
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
                                    // VIDEO TỪ SERVER: Dùng HTML5 video tag
                                    videoHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ background: black; overflow: hidden; display: flex; align-items: center; justify-content: center; height: 100vh; }}
        video {{ max-width: 100%; max-height: 100%; object-fit: contain; }}
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
                                    Height = videoPanel.Height
                                };
                                videoPanel.Controls.Clear();
                                videoPanel.Controls.Add(lblError);
                            }
                        }
                    };

                    videoPanel.Controls.Add(lblLoadVideo);
                    gb_eachpost.Controls.Add(videoPanel);
                    currentY += videoPanel.Height + 10;
                }
                else
                {
                    // URL không hợp lệ - hiển thị cảnh báo
                    Label lblInvalidVideo = new Label
                    {
                        Text = "⚠️ Video từ nguồn không được phép",
                        Location = new Point(10, currentY),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.Orange
                    };
                    gb_eachpost.Controls.Add(lblInvalidVideo);
                    currentY += lblInvalidVideo.Height + 10;
                }
            }

            gb_eachpost.Height = currentY + 10;
            return gb_eachpost;
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

        private void btn_main_Click(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void btn_profile_Click(object sender, EventArgs e)
        {
            Profile profileForm = new Profile(Load_Data.LoginData.username);
            this.Hide();
            profileForm.ShowDialog();
        }

        private void gb_info_Enter(object sender, EventArgs e)
        {

        }

        private void lb_logout_Click(object sender, EventArgs e)
        {
            Load_Data.LoginData.accept = false;
            this.Close();
        }

        private void lb_username_Click(object sender, EventArgs e)
        {

        }

        private System.Threading.Timer? searchTimer;

        private void tb_find_TextChanged(object sender, EventArgs e)
        {
            // Hủy timer cũ nếu có
            searchTimer?.Dispose();

            // Tạo timer mới, delay 300ms
            searchTimer = new System.Threading.Timer(_ =>
            {
                // Đảm bảo toàn bộ logic chạy trên UI thread
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(async () =>
                    {
                        string searchText = tb_find.Text.Trim();

                        // Thực hiện tìm kiếm
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

            // Temporarily store original posts
            var originalPosts = Load_Data.Posts;
            Load_Data.Posts = filteredPosts;
            await LoadPostsAsync();
            Load_Data.Posts = originalPosts;
        }
    }
}