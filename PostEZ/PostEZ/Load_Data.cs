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
using System.Net;

namespace PostEZ
{
    // ============================================
    // ||         BACKGROUND SERVICE             ||
    // ============================================
    public partial class Load_Data : Form
    {
        public Load_Data()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        // ============================================
        // TCP CONNECTION VARIABLES
        // ============================================
        private static bool _isRunning = true;
        private static TcpClient _client;
        private static NetworkStream _stream;
        private static StreamReader _reader;
        private static StreamWriter _writer;
        private static readonly object _writeLock = new object();
        private static string _host = "160.191.245.144";
        private static int _port = 13579;



        // ============================================
        // GLOBAL DATA
        // ============================================

        public static List<Data_PostJson> Posts = new List<Data_PostJson>();
        public static List<Data_CommentPreviousPostClickJson> Comments = new List<Data_CommentPreviousPostClickJson>();
        public static Data_LoginJson LoginData = new Data_LoginJson();
        public static Data_SignupJson SignupData = new Data_SignupJson();
        public static GetFeedResponse getFeedResponse = new GetFeedResponse();
        public static Data_PreviousPostClickJson PreviousPostClickData = new Data_PreviousPostClickJson();
        public static Data_InformationUserJson InformationUser = new Data_InformationUserJson();
        public static Data_PostJson CreatePost = new Data_PostJson();
        public static Data_UpdateAvatarJson UpdateAvatar = new Data_UpdateAvatarJson();
        public static Data_LikePostJson LikePostResponse = new Data_LikePostJson();
        public static Data_AddCommentJson AddCommentResponse = new Data_AddCommentJson();
        public static Data_GetCommentsJson GetCommentsResponse = new Data_GetCommentsJson();
        public static Data_GetOnlineUsersJson GetOnlineUsersResponse = new Data_GetOnlineUsersJson();
        public static Data_GetMessagesJson GetMessagesResponse = new Data_GetMessagesJson();
        public static Data_SendMessageJson SendMessageResponse = new Data_SendMessageJson();
        public static Data_LogoutJson LogoutResponse = new Data_LogoutJson();


        // ============================================
        // CONNECTION
        // ============================================
        private async void Load_Data_Load(object sender, EventArgs e)
        {
            Login.LoadFromUrl("https://pminmod.site/doannt106/logo.png", pic_logo);
            btn_login.Hide();
            btn_close.Hide();
        }
        static void TcpConnect()
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(_host, _port);

                _stream = _client.GetStream();
                _reader = new StreamReader(_stream, Encoding.UTF8);
                _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };

                while (_isRunning)
                {
                    string line = _reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    HandleServerJson(line);

                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private static void HandleServerJson(string json)
        {
            var obj = JObject.Parse(json);
            string action = (string)obj["action"];
            switch (action)
            {
                case "post_data":
                    var postData = JsonConvert.DeserializeObject<Data_PostJson>(json);
                    if (postData != null)
                    {
                        CreatePost = postData;
                        Posts.Add(postData);
                    }
                    break;
                case "comment_previous_post_click":
                    var commentData = JsonConvert.DeserializeObject<Data_CommentPreviousPostClickJson>(json);
                    if (commentData != null)
                    {
                        Comments.Add(commentData);
                    }
                    break;
                case "login_data":
                    var loginData = JsonConvert.DeserializeObject<Data_LoginJson>(json);
                    if (loginData != null)
                    {
                        LoginData = loginData;
                    }
                    break;
                case "signup_data":
                    var signupData = JsonConvert.DeserializeObject<Data_SignupJson>(json);
                    if (signupData != null)
                    {
                        SignupData = signupData;
                    }
                    break;
                case "get_feed":
                    var getFeedData = JsonConvert.DeserializeObject<GetFeedResponse>(json);
                    if (getFeedData != null)
                    {
                        getFeedResponse = getFeedData;
                    }
                    break;
                case "previous_post_click":
                    var previousPostClickData = JsonConvert.DeserializeObject<Data_PreviousPostClickJson>(json);
                    if (previousPostClickData != null)
                    {
                        PreviousPostClickData = previousPostClickData;
                    }
                    break;
                case "get_user_info":
                    var userInfoData = JsonConvert.DeserializeObject<Data_InformationUserJson>(json);
                    if (userInfoData != null)
                    {
                        InformationUser = userInfoData;
                    }
                    break;
                case "update_user_avatar":
                    var updateAvatarData = JsonConvert.DeserializeObject<Data_UpdateAvatarJson>(json);
                    if (updateAvatarData != null)
                    {
                        UpdateAvatar = updateAvatarData;
                    }
                    break;
                case "like_post":
                    var likeData = JsonConvert.DeserializeObject<Data_LikePostJson>(json);
                    if (likeData != null)
                    {
                        LikePostResponse = likeData;
                    }
                    break;
                case "add_comment":
                    var addCommentData = JsonConvert.DeserializeObject<Data_AddCommentJson>(json);
                    if (addCommentData != null)
                    {
                        AddCommentResponse = addCommentData;
                    }
                    break;
                case "get_comments":
                    var getCommentsData = JsonConvert.DeserializeObject<Data_GetCommentsJson>(json);
                    if (getCommentsData != null)
                    {
                        GetCommentsResponse = getCommentsData;
                    }
                    break;
                case "get_online_users":
                    var onlineUsersData = JsonConvert.DeserializeObject<Data_GetOnlineUsersJson>(json);
                    if (onlineUsersData != null)
                    {
                        GetOnlineUsersResponse = onlineUsersData;
                    }
                    break;

                case "get_messages":
                    var messagesData = JsonConvert.DeserializeObject<Data_GetMessagesJson>(json);
                    if (messagesData != null)
                    {
                        GetMessagesResponse = messagesData;
                    }
                    break;

                case "send_message":
                    var sendMsgData = JsonConvert.DeserializeObject<Data_SendMessageJson>(json);
                    if (sendMsgData != null)
                    {
                        SendMessageResponse = sendMsgData;
                    }
                    break;

                case "logout":
                    var logoutData = JsonConvert.DeserializeObject<Data_LogoutJson>(json);
                    if (logoutData != null)
                    {
                        LogoutResponse = logoutData;
                    }
                    break;

                default:
                    MessageBox.Show("Không nhận diện được hành động từ server: " + action);
                    break;
            }
        }

        public static bool IsConnected()
        {
            try
            {
                return _client != null && _client.Connected &&
                       _stream != null && _stream.CanWrite &&
                       _writer != null;
            }
            catch
            {
                return false;
            }
        }
        public static bool SendJson(object data)
        {
            if (data == null) return false;

            try
            {
                if (!IsConnected()) return false;

                string json = JsonConvert.SerializeObject(data);

                lock (_writeLock)
                {
                    _writer.WriteLine(json);
                    _writer.Flush();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool SendRawJsonString(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return false;

            try
            {
                if (!IsConnected()) return false;

                lock (_writeLock)
                {
                    _writer.WriteLine(json);
                    _writer.Flush();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            this.Hide();
            login.ShowDialog();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public class Data_LoginJson
        {
            public string action { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public bool accept { get; set; }
            public string error { get; set; }
            public string request_id { get; set; }
        }

        public class Data_SignupJson
        {
            public string action { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string error { get; set; }
            public bool accept { get; set; }
            public string request_id { get; set; }
        }

        public class Data_PostJson
        {
            public string action { get; set; }
            public string username { get; set; }
            public int id { get; set; }
            public string content { get; set; }
            public string image_url { get; set; }
            public string video_url { get; set; }
            public string timestamp { get; set; }
            public bool accept { get; set; }
            public bool enabled { get; set; }
            public int like_count { get; set; }
            public int comment_count { get; set; }
            public string error { get; set; }
            public string request_id { get; set; }
        }

        public class Data_InformationUserJson
        {
            public string action { get; set; }
            public string username { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string bio { get; set; }
            public string avatar_url { get; set; }
            public int count_posts { get; set; }
            public int count_followers { get; set; }
            public List<Data_PostJson> posts_user { get; set; }
            public bool accept { get; set; }
            public string error { get; set; }
            public string request_id { get; set; }
        }

        public class GetFeedResponse
        {
            public string action { get; set; }
            public List<Data_PostJson> posts { get; set; }
            public int count { get; set; }
            public bool accept { get; set; }
            public string error { get; set; }
            public string request_id { get; set; }
        }

        public class Data_CommentPreviousPostClickJson
        {
            public string action { get; set; }
            public string username { get; set; }
            public string post_id { get; set; }
            public string cmt_id { get; set; }
            public string comment_content { get; set; }
            public string timestamp { get; set; }
            public bool accept { get; set; }

            public string error { get; set; }

            public string request_id { get; set; }
        }

        public class Data_PreviousPostClickJson
        {
            public string action { get; set; }
            public string username { get; set; }
            public string post_id { get; set; }
            public string content { get; set; }
            public string image_url { get; set; }
            public string video_url { get; set; }
            public string timestamp { get; set; }
            public bool accept { get; set; }
            public bool enabled { get; set; }
            public string error { get; set; }

            public string request_id { get; set; }
        }

        public class Data_CreateNewPost
        {
            public string action { get; set; }
            public string username { get; set; }
            public string content { get; set; }
            public string image_url { get; set; }
            public string video_url { get; set; }
            public bool accept { get; set; }
            public bool delete_post { get; set; }
            public bool reply_post { get; set; }
            public bool enabled { get; set; }
            public string error { get; set; }

            public string request_id { get; set; }
        }

        public class Data_UpdateAvatarJson
        {
            public string action { get; set; }
            public string username { get; set; }
            public string avatar_url { get; set; }
            public string error { get; set; }
            public bool accept { get; set; }
            public string request_id { get; set; }
        }

        public class Data_LikePostJson
        {
            public string action { get; set; }
            public string post_id { get; set; }
            public string username { get; set; }
            public bool liked { get; set; }
            public int like_count { get; set; }
            public string error { get; set; }
            public bool accept { get; set; }
            public string request_id { get; set; }
        }

        public class Data_CommentJson
        {
            public string comment_id { get; set; }
            public string username { get; set; }
            public string content { get; set; }
            public string timestamp { get; set; }
        }

        public class Data_AddCommentJson
        {
            public string action { get; set; }
            public string post_id { get; set; }
            public string username { get; set; }
            public string comment_id { get; set; }
            public string content { get; set; }
            public string timestamp { get; set; }
            public int comment_count { get; set; }
            public string error { get; set; }
            public bool accept { get; set; }
            public string request_id { get; set; }
        }

        public class Data_GetCommentsJson
        {
            public string action { get; set; }
            public string post_id { get; set; }
            public List<Data_CommentJson> comments { get; set; }
            public int count { get; set; }
            public string error { get; set; }
            public bool accept { get; set; }
            public string request_id { get; set; }
        }

        public class Data_MessageJson
        {
            public string message_id { get; set; }
            public string from_user { get; set; }
            public string to_user { get; set; }
            public string content { get; set; }
            public string timestamp { get; set; }
        }

        public class Data_GetOnlineUsersJson
        {
            public string action { get; set; }
            public List<string> users { get; set; }
            public string error { get; set; }
            public bool accept { get; set; }
            public string request_id { get; set; }
        }

        public class Data_GetMessagesJson
        {
            public string action { get; set; }
            public List<Data_MessageJson> messages { get; set; }
            public int count { get; set; }
            public string error { get; set; }
            public bool accept { get; set; }
            public string request_id { get; set; }
        }

        public class Data_SendMessageJson
        {
            public string action { get; set; }
            public string message_id { get; set; }
            public string from_user { get; set; }
            public string to_user { get; set; }
            public string content { get; set; }
            public string timestamp { get; set; }
            public string error { get; set; }
            public bool accept { get; set; }
            public string request_id { get; set; }
        }

        public class Data_LogoutJson
        {
            public string action { get; set; }
            public string username { get; set; }
            public string error { get; set; }
            public bool accept { get; set; }
            public string request_id { get; set; }
        }

        public static async Task<bool> WaitForServerResponse(Func<bool> checkCondition, int timeoutSeconds = 10)
        {
            int maxAttempts = timeoutSeconds * 10;
            int attempts = 0;

            while (!checkCondition())
            {
                attempts++;
                if (attempts >= maxAttempts)
                {
                    return false;
                }
                await Task.Delay(100);
            }

            return true;
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random random = new Random();

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)])
              .ToArray());
        }

        public static async Task<bool> Logout()
        {
            try
            {
                LogoutResponse = new Data_LogoutJson
                {
                    action = "logout",
                    username = LoginData.username,
                    request_id = GenerateRandomString(4)
                };

                bool sent = SendJson(LogoutResponse);
                if (!sent) return false;

                bool received = await WaitForServerResponse(
                    () => LogoutResponse.request_id != null && LogoutResponse.request_id.Contains("ServerHaha"),
                    timeoutSeconds: 5
                );

                return received && LogoutResponse.accept;
            }
            catch
            {
                return false;
            }
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            if (tb_serverip.Text != "")
            {
                _host = tb_serverip.Text;
            }
            try
            {
                Thread TcpThread = new Thread(new ThreadStart(TcpConnect));
                TcpThread.IsBackground = true;
                TcpThread.Start();
                Task.Delay(1000).Wait();
                if (IsConnected())
                {
                    Login login = new Login();
                    this.Hide();
                    login.ShowDialog();
                    this.Close();
                    MessageBox.Show("Kết nối đến server thành công!");
                }
                else
                {
                    MessageBox.Show("Kết nối đến server thất bại!");
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối đến server: " + ex.Message);
                return;
            }
        }
    }
}
