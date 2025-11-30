using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using System.Text.RegularExpressions;
using PostEZ.Main;

namespace PostEZ.Log
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
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
            LoadFromUrl("https://pminmod.site/doannt106/logo.png", pic_logo);
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

            // Regex:
            // ^[a-zA-Z0-9]{4,20}$
            // ^ : Bắt đầu chuỗi
            // [a-zA-Z0-9] : Chỉ cho phép chữ cái (thường, hoa) và số
            // {4,20} : Độ dài tối thiểu 4, tối đa 20 ký tự
            // $ : Kết thúc chuỗi

            string pattern = @"^[a-zA-Z0-9]{4,20}$";

            return Regex.IsMatch(username, pattern);
        }
        public static bool CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            // Regex phức hợp cho mật khẩu
            // ^                     : Bắt đầu chuỗi
            // (?=.*[0-9])           : Phải chứa ÍT NHẤT một chữ số (0-9)
            // (?=.*[!@#$%^&*()_+}{":?/><.,;`~]) : Phải chứa ÍT NHẤT một ký tự đặc biệt (Bạn có thể thêm/bớt ký tự đặc biệt tùy ý)
            // .{6,20}               : Tổng độ dài từ 6 đến 20 ký tự
            // $                     : Kết thúc chuỗi

            string pattern = @"^(?=.*[0-9])(?=.*[!@#$%^&*()_+{}\[\]:""|\\;'.?/<>,]).{6,20}$";

            return Regex.IsMatch(password, pattern);
        }

        private void btn_login_Click(object sender, EventArgs e)
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
            if (tb_username.Text == "Admin123" && tb_password.Text == "Admin123!")
            {
                MessageBox.Show("Đăng nhập thành công!", "Thành công");
                Dashboard dashboardForm = new Dashboard();
                this.Hide();
                dashboardForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Lỗi");
            }
        }
    }
}