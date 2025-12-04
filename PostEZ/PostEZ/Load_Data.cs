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

        public static List<Data_PostJson> Posts = new List<Data_PostJson>();
        public static List<Data_CommentPreviousPostClickJson> Comments = new List<Data_CommentPreviousPostClickJson>();
        public static Data_LoginJson LoginData = new Data_LoginJson();
        public static Data_SignupJson SignupData = new Data_SignupJson();
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
        void TcpConnect()
        {
            try
            {
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(_host), _port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(serverEndPoint);
                while (_isRunning)
                {
                    // đọc json được gửi từ server đến

                }
            }
            catch (Exception ex)
            {
                return;
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
        }

        public class Data_SignupJson
        {
            public string action { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string email { get; set; }
            public bool accept { get; set; }
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
        }

    }
}