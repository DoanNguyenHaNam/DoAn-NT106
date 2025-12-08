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
    // BACKGROUND SERVICE - CHẠY NGẦM, KHÔNG HIỂN THỊ
    // ============================================
    public partial class Load_Data : Form
    {
        public Load_Data()
        {
            InitializeComponent();
        }

        // ============================================
        // TCP CONNECTION VARIABLES
        // ============================================
        private static bool _isRunning = true;
        private static TcpClient _client;
        private static NetworkStream _stream;
        private static StreamReader _reader;
        private static StreamWriter _writer;
        private static readonly object _writeLock = new object();  // ← STATIC
        private static string _host = "160.191.245.144";
        private static int _port = 13579;



        // ============================================
        // GLOBAL DATA - LƯU TẤT CẢ DỮ LIỆU TỪ SERVER
        // ============================================

        public static List<Data_PostJson> Posts = new List<Data_PostJson>();
        public static List<Data_CommentPreviousPostClickJson> Comments = new List<Data_CommentPreviousPostClickJson>();
        public static Data_LoginJson LoginData = new Data_LoginJson();
        public static Data_SignupJson SignupData = new Data_SignupJson();
        public static GetFeedResponse getFeedResponse = new GetFeedResponse();
        public static Data_PreviousPostClickJson PreviousPostClickData = new Data_PreviousPostClickJson();


        // ============================================
        // CONNECTION - KẾT NỐI VỚI SERVER
        // ============================================
        private async void Load_Data_Load(object sender, EventArgs e)
        {
            try
            {
                Thread TcpThread = new Thread(new ThreadStart(TcpConnect));
                TcpThread.IsBackground = true;
                TcpThread.Start();
            }
            catch (Exception ex)
            {
                return;
            }
            Login login = new Login();
            this.Hide();
            login.ShowDialog();
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
                    string line = _reader.ReadLine();   // server gửi JSON + '\n'
                    if (line == null)
                    {
                        // server đóng kết nối
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
                default:
                    // Xử lý các hành động khác nếu cần
                    break;
            }
        }

        public static bool IsConnected()
        { 
            try
            {
                // TcpClient.Connected có thể trả true dù socket đã bị ngắt ở server side,
                // nên kiểm tra thêm stream
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
                    // _writer được tạo trong TcpConnect với AutoFlush = true,
                    // nhưng vẫn gọi Flush() để an toàn
                    _writer.WriteLine(json);
                    _writer.Flush();
                }

                return true;
            }
            catch (Exception ex)
            {
                // nếu muốn log, dùng PostEZ.Log hoặc Debug.WriteLine
                // LogManager.Error(ex); // ví dụ
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

            public string error { get; set; }

            public string request_id { get; set; }
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random random = new Random();

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)])
              .ToArray());
        }

    }
}