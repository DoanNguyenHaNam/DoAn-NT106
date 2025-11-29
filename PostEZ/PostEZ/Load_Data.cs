using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PostEZ.Log;

namespace PostEZ
{
    public partial class Load_Data : Form
    {
        public Load_Data()
        {
            InitializeComponent();
        }
        private bool _isRunning = true;
        private TcpClient _client;
        private NetworkStream _stream;
        private StreamReader _reader;
        private String _host = "163.61.110.135";
        private int _port = 12000;
        public bool _LoginSucces = false;

        public void ConnectTCP()
        {
            _client = new TcpClient();
            _client.Connect(_host, _port); // Kết nối chặn (blocking)
            _stream = _client.GetStream();
            _reader = new StreamReader(_stream, Encoding.UTF8);

            // Bắt đầu loop nhận dữ liệu (chạy ngầm)
            Task.Run(ReceiveLoop);
        }
        private async Task ReceiveLoop()
        {
            while (_isRunning)
            {
                string line = await _reader.ReadLineAsync();
                if (line == null)
                {
                    Console.WriteLine("[DISCONNECTED] Server closed connection");
                    break;
                }

                try
                {
                    JObject resp = JObject.Parse(line);

                    // ===============================================
                    // BƯỚC 1: XÁC ĐỊNH HEADER (Tên Key Cấp Cao Nhất)
                    // ===============================================
                    // Lấy JProperty đầu tiên (Header/Action)
                    JProperty headerProp = resp.Properties().FirstOrDefault();

                    if (headerProp == null)
                    {
                        Console.WriteLine("[PARSE ERROR] Response is an empty JSON object.");
                        continue;
                    }

                    string header = headerProp.Name; // Ví dụ: "Login"
                    JToken body = headerProp.Value;  // Body/Payload, ví dụ: { "UserName": "...", "Equal": 1 }

                    if (header == "Login")
                    {
                        // Xử lý Response Login
                        // Kiểm tra xem đây có phải là response Login thành công hay không
                        int equalValue = body["Equal"]?.Value<int>() ?? 0;
                        string username = body["UserName"]?.Value<string>() ?? "";

                    }
                    else if (header == "Signup")
                    {
                        // Xử lý Response Signup
                        // ...
                    }
                    else
                    {
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RECEIVE ERROR] {ex.Message}");
                }
            }
        }
        private void Load_Data_Load(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            this.Hide();
            loginForm.ShowDialog();
            this.Show();
        }
    }
}
