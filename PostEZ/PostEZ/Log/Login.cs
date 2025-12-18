using PostEZ;
using PostEZ.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PostEZ.Load_Data;

namespace PostEZ.Log
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        public static async Task<bool> LoadFromUrl(string imageUrl, PictureBox pictureBox, bool showError = true)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                    using (var ms = new MemoryStream(imageBytes))
                    {
                        pictureBox.Image = Image.FromStream(ms);
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (showError)
                {
                    MessageBox.Show($"Không thể tải ảnh:\n{ex.Message}", "Lỗi");
                }
                return false;
            }
        }
        private async void Login_Load(object sender, EventArgs e)
        {
            LoadFromUrl("https://raw.githubusercontent.com/DoanNguyenHaNam/DoAn-NT106/main/Sources_NotNecessery/Logo.png", pic_logo);
        }

        private void lb_signup_Click(object sender, EventArgs e)
        {
            SignUp signUpForm = new SignUp();
            this.Hide();
            signUpForm.ShowDialog();
        }

        public static bool CheckUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            string pattern = @"^[a-zA-Z0-9]{4,20}$";

            return Regex.IsMatch(username, pattern);
        }
        public static bool CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            string pattern = @"^(?=.*[0-9])(?=.*[!@#$%^&*()_+{}\[\]:""|\\;'.?/<>,]).{6,20}$";

            return Regex.IsMatch(password, pattern);
        }

        private async void btn_login_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_username.Text) || string.IsNullOrWhiteSpace(tb_password.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi");
                return;
            }
            if (!CheckUsername(tb_username.Text))
            {
                MessageBox.Show("Tên đăng nhập không hợp lệ. Vui lòng sử dụng 4-20 ký tự chữ cái và số, không dùng bất cứ kí tự đặc biệt nào (!@#$%...)", "Lỗi");
                return;
            }
            if (!CheckPassword(tb_password.Text))
            {
                MessageBox.Show("Mật khẩu không hợp lệ. Mật khẩu phải từ 6-20 ký tự và bao gồm ít nhất một chữ số và một ký tự đặc biệt (!@#$%...)", "Lỗi");
                return;
            }

            Load_Data.LoginData = new Load_Data.Data_LoginJson
            {
                action = "login_data",
                username = tb_username.Text,
                password = tb_password.Text,
                request_id = Load_Data.GenerateRandomString(4)
            };
            bool success = Load_Data.SendJson(Load_Data.LoginData);

            if (!success)
            {
                MessageBox.Show("Không thể gửi dữ liệu tới server!", "Lỗi");
                return;
            }

            bool received = await Load_Data.WaitForServerResponse(
                () => Load_Data.LoginData.request_id != null && Load_Data.LoginData.request_id.Contains("ServerHaha")
            );

            if (!received)
            {
                MessageBox.Show("Server không phản hồi kịp thời. Vui lòng thử lại sau!", "Lỗi");
                return;
            }

            MessageBox.Show(Load_Data.LoginData.error, "Thông báo");

            if (Load_Data.LoginData.accept)
            {


                Dashboard dashboardForm = new Dashboard();
                this.Hide();
                dashboardForm.ShowDialog();
                this.Show();
            }
        }

        private void tb_username_TextChanged(object sender, EventArgs e)
        {

        }
    }
}