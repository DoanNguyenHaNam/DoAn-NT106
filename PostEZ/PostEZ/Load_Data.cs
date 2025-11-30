using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PostEZ.Log;
using Newtonsoft.Json;

namespace PostEZ
{
    // ============================================
    // BACKGROUND SERVICE - CHẠY NGẦM, KHÔNG HIỂN THỊ
    // ============================================
    public partial class Load_Data : Form
    {
        // ============================================
        // SINGLETON PATTERN - CHỈ CÓ 1 INSTANCE DUY NHẤT
        // ============================================
        private static Load_Data _instance;
        public static Load_Data Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new Load_Data();
                }
                return _instance;
            }
        }

        public Load_Data()
        {
            InitializeComponent();

            // ============================================
            // QUAN TRỌNG: ẨN FORM, KHÔNG HIỂN THỊ TRÊN TASKBAR
            // ============================================
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Opacity = 0; // Trong suốt hoàn toàn
            this.FormBorderStyle = FormBorderStyle.None;
            this.Width = 0;
            this.Height = 0;
        }

        // ============================================
        // TCP CONNECTION VARIABLES
        // ============================================
        private bool _isRunning = true;
        private TcpClient _client;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private string _host = "163.61.110.135";
        private int _port = 12000;

        // ============================================
        // GLOBAL DATA - LƯU TẤT CẢ DỮ LIỆU TỪ SERVER
        // ============================================

        // User đang login
        public static UserInfo CurrentUser { get; set; } = new UserInfo();

        // Danh sách tất cả posts (Dashboard)
        public static List<PostData> AllPosts { get; set; } = new List<PostData>();

        // Danh sách posts của user hiện tại (Profile)
        public static List<PostData> MyPosts { get; set; } = new List<PostData>();

        // Danh sách messages
        public static List<MessageData> Messages { get; set; } = new List<MessageData>();

        // Danh sách conversations
        public static List<ConversationData> Conversations { get; set; } = new List<ConversationData>();

        // ============================================
        // EVENTS - CÁC FORM KHÁC SUBSCRIBE ĐỂ NHẬN THÔNG BÁO
        // ============================================

        // Event khi nhận Login response
        public event Action<bool, string> OnLoginResponse; // (success, message)

        // Event khi nhận Signup response
        public event Action<bool, string> OnSignupResponse; // (success, message)

        // Event khi nhận danh sách posts mới
        public event Action<List<PostData>> OnPostsUpdated;

        // Event khi có post mới được tạo
        public event Action<PostData> OnNewPost;

        // Event khi nhận messages mới
        public event Action<List<MessageData>> OnMessagesUpdated;

        // Event khi có message mới
        public event Action<MessageData> OnNewMessage;

        // Event khi có lỗi kết nối
        public event Action<string> OnConnectionError;

        // ============================================
        // KẾT NỐI TCP VÀ BẮT ĐẦU NHẬN DỮ LIỆU
        // ============================================
        public async Task<bool> ConnectTCP()
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(_host, _port);
                _stream = _client.GetStream();
                _reader = new StreamReader(_stream, Encoding.UTF8);
                _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };

                Console.WriteLine($"✅ [CONNECTED] Đã kết nối tới {_host}:{_port}");

                // Bắt đầu loop nhận dữ liệu (chạy ngầm)
                Task.Run(ReceiveLoop);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [CONNECTION ERROR] {ex.Message}");
                OnConnectionError?.Invoke($"Không thể kết nối tới server: {ex.Message}");
                return false;
            }
        }

        // ============================================
        // GỬI REQUEST TỚI SERVER
        // ============================================
        public void SendRequest(string action, object data)
        {
            try
            {
                // Format: { "Action": { ...data... } }
                var request = new Dictionary<string, object>
                {
                    [action] = data
                };

                string json = JsonConvert.SerializeObject(request);
                _writer.WriteLine(json);

                Console.WriteLine($"📤 [SENT] {action}: {json}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SEND ERROR] {ex.Message}");
            }
        }

        // ============================================
        // VÒNG LẶP NHẬN DỮ LIỆU LIÊN TỤC TỪ SERVER
        // ============================================
        private async Task ReceiveLoop()
        {
            while (_isRunning)
            {
                try
                {
                    string line = await _reader.ReadLineAsync();
                    if (line == null)
                    {
                        Console.WriteLine("⚠️ [DISCONNECTED] Server đóng kết nối");
                        OnConnectionError?.Invoke("Mất kết nối với server");
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    Console.WriteLine($"📥 [RECEIVED] {line}");

                    // Parse JSON response
                    JObject resp = JObject.Parse(line);
                    JProperty headerProp = resp.Properties().FirstOrDefault();

                    if (headerProp == null)
                    {
                        Console.WriteLine("⚠️ [PARSE ERROR] Empty JSON");
                        continue;
                    }

                    string action = headerProp.Name;
                    JToken body = headerProp.Value;

                    // ============================================
                    // XỬ LÝ RESPONSE THEO TỪNG ACTION
                    // ============================================
                    ProcessResponse(action, body);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"❌ [JSON ERROR] {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ [RECEIVE ERROR] {ex.Message}");
                    OnConnectionError?.Invoke($"Lỗi nhận dữ liệu: {ex.Message}");
                    break;
                }
            }
        }

        // ============================================
        // XỬ LÝ RESPONSE TỪ SERVER
        // ============================================
        private void ProcessResponse(string action, JToken body)
        {
            switch (action)
            {
                case "Login":
                    HandleLoginResponse(body);
                    break;

                case "Signup":
                    HandleSignupResponse(body);
                    break;

                case "GetPosts":
                    HandleGetPostsResponse(body);
                    break;

                case "CreatePost":
                    HandleCreatePostResponse(body);
                    break;

                case "GetMyPosts":
                    HandleGetMyPostsResponse(body);
                    break;

                case "GetMessages":
                    HandleGetMessagesResponse(body);
                    break;

                case "SendMessage":
                    HandleSendMessageResponse(body);
                    break;

                case "NewMessage":
                    HandleNewMessageNotification(body);
                    break;

                case "NewPost":
                    HandleNewPostNotification(body);
                    break;

                default:
                    Console.WriteLine($"⚠️ [UNKNOWN ACTION] {action}");
                    break;
            }
        }

        // ============================================
        // HANDLERS CHO TỪNG LOẠI RESPONSE
        // ============================================

        private void HandleLoginResponse(JToken body)
        {
            int equal = body["Equal"]?.Value<int>() ?? 0;
            string username = body["UserName"]?.Value<string>() ?? "";
            string message = body["Message"]?.Value<string>() ?? "";

            if (equal == 1)
            {
                // Login thành công
                CurrentUser.Username = username;
                CurrentUser.FullName = body["FullName"]?.Value<string>() ?? "";
                CurrentUser.Email = body["Email"]?.Value<string>() ?? "";
                CurrentUser.AvatarUrl = body["AvatarUrl"]?.Value<string>() ?? "";
                CurrentUser.IsLoggedIn = true;

                Console.WriteLine($"✅ [LOGIN] Success: {username}");
                OnLoginResponse?.Invoke(true, message);
            }
            else
            {
                // Login thất bại
                Console.WriteLine($"❌ [LOGIN] Failed: {message}");
                OnLoginResponse?.Invoke(false, message);
            }
        }

        private void HandleSignupResponse(JToken body)
        {
            int equal = body["Equal"]?.Value<int>() ?? 0;
            string message = body["Message"]?.Value<string>() ?? "";

            bool success = equal == 1;
            Console.WriteLine($"{(success ? "✅" : "❌")} [SIGNUP] {message}");
            OnSignupResponse?.Invoke(success, message);
        }

        private void HandleGetPostsResponse(JToken body)
        {
            try
            {
                // Format: { "Equal": 1, "Posts": [ {...}, {...} ] }
                int equal = body["Equal"]?.Value<int>() ?? 0;
                if (equal == 1)
                {
                    var postsArray = body["Posts"]?.ToObject<List<PostData>>();
                    if (postsArray != null)
                    {
                        AllPosts = postsArray;
                        Console.WriteLine($"✅ [GET POSTS] Nhận {AllPosts.Count} posts");
                        OnPostsUpdated?.Invoke(AllPosts);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [GET POSTS ERROR] {ex.Message}");
            }
        }

        private void HandleCreatePostResponse(JToken body)
        {
            int equal = body["Equal"]?.Value<int>() ?? 0;
            string message = body["Message"]?.Value<string>() ?? "";

            if (equal == 1)
            {
                Console.WriteLine($"✅ [CREATE POST] {message}");
                // Tự động refresh posts
                SendRequest("GetPosts", new { });
                SendRequest("GetMyPosts", new { Username = CurrentUser.Username });
            }
            else
            {
                Console.WriteLine($"❌ [CREATE POST] {message}");
            }
        }

        private void HandleGetMyPostsResponse(JToken body)
        {
            try
            {
                int equal = body["Equal"]?.Value<int>() ?? 0;
                if (equal == 1)
                {
                    var postsArray = body["Posts"]?.ToObject<List<PostData>>();
                    if (postsArray != null)
                    {
                        MyPosts = postsArray;
                        Console.WriteLine($"✅ [GET MY POSTS] Nhận {MyPosts.Count} posts");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [GET MY POSTS ERROR] {ex.Message}");
            }
        }

        private void HandleGetMessagesResponse(JToken body)
        {
            try
            {
                int equal = body["Equal"]?.Value<int>() ?? 0;
                if (equal == 1)
                {
                    var messagesArray = body["Messages"]?.ToObject<List<MessageData>>();
                    if (messagesArray != null)
                    {
                        Messages = messagesArray;
                        Console.WriteLine($"✅ [GET MESSAGES] Nhận {Messages.Count} messages");
                        OnMessagesUpdated?.Invoke(Messages);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [GET MESSAGES ERROR] {ex.Message}");
            }
        }

        private void HandleSendMessageResponse(JToken body)
        {
            int equal = body["Equal"]?.Value<int>() ?? 0;
            string message = body["Message"]?.Value<string>() ?? "";
            Console.WriteLine($"{(equal == 1 ? "✅" : "❌")} [SEND MESSAGE] {message}");
        }

        private void HandleNewMessageNotification(JToken body)
        {
            // Server push message mới real-time
            try
            {
                var newMessage = body.ToObject<MessageData>();
                if (newMessage != null)
                {
                    Messages.Add(newMessage);
                    Console.WriteLine($"🔔 [NEW MESSAGE] From {newMessage.FromUser}");
                    OnNewMessage?.Invoke(newMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [NEW MESSAGE ERROR] {ex.Message}");
            }
        }

        private void HandleNewPostNotification(JToken body)
        {
            // Server push post mới real-time
            try
            {
                var newPost = body.ToObject<PostData>();
                if (newPost != null)
                {
                    AllPosts.Insert(0, newPost); // Thêm vào đầu list
                    Console.WriteLine($"🔔 [NEW POST] From {newPost.Username}");
                    OnNewPost?.Invoke(newPost);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [NEW POST ERROR] {ex.Message}");
            }
        }

        // ============================================
        // FORM EVENTS
        // ============================================
        private async void Load_Data_Load(object sender, EventArgs e)
        {
            // Kết nối tới server ngay khi form load
            bool connected = await ConnectTCP();

            if (connected)
            {
                // Hiển thị Login form
                Login loginForm = new Login();
                loginForm.Show();
            }
            else
            {
                MessageBox.Show("Không thể kết nối tới server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Đóng kết nối TCP
            _isRunning = false;
            _writer?.Close();
            _reader?.Close();
            _stream?.Close();
            _client?.Close();

            Console.WriteLine("🔴 [DISCONNECTED] Đóng kết nối TCP");
        }
    }

    // ============================================
    // DATA MODELS
    // ============================================

    public class UserInfo
    {
        public string Username { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public bool IsLoggedIn { get; set; } = false;
    }

    public class PostData
    {
        public int PostId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string Content { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MessageData
    {
        public int MessageId { get; set; }
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class ConversationData
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}