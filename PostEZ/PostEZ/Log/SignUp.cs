using PostEZ;
using PostEZ.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostEZ.Log
{
    public partial class SignUp : Form
    {
        public SignUp()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void SignUp_Load(object sender, EventArgs e)
        {
            Login.LoadFromUrl("https://pminmod.site/doannt106/logo.png", pic_logo);
        }

        private void lb_login_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            this.Hide();
            loginForm.ShowDialog();
        }

        private async void btn_signup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_username.Text) || string.IsNullOrEmpty(tb_password.Text) || string.IsNullOrEmpty(tb_repass.Text) || string.IsNullOrEmpty(tb_phone.Text) || string.IsNullOrEmpty(tb_mail.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi");
                return;
            }
            if (!Login.CheckUsername(tb_username.Text))
            {
                MessageBox.Show("Tên đăng nhập không hợp lệ. Vui lòng sử dụng 4-20 ký tự chữ cái và số, không dùng bất cứ kí tự đặc biệt nào (!@#$%...)", "Lỗi");
                return;
            }
            if (!Login.CheckPassword(tb_password.Text))
            {
                MessageBox.Show("Mật khẩu không hợp lệ. Vui lòng sử dụng 6-20 ký tự, bao gồm ít nhất một chữ số và một ký tự đặc biệt (!@#$%...)", "Lỗi");
                return;
            }
            if (tb_password.Text != tb_repass.Text)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp", "Lỗi");
                return;
            }
            if ("@".All(c => !tb_mail.Text.Contains(c)) || tb_mail.Text.Length < 5)
            {
                MessageBox.Show("Địa chỉ email không hợp lệ", "Lỗi");
                return;
            }
            if (!tb_phone.Text.All(c => char.IsDigit(c)) || tb_phone.Text.Length < 10 || tb_phone.Text.Length > 10)
            {
                MessageBox.Show("Số điện thoại không hợp lệ", "Lỗi");
                return;
            }
            Load_Data.SignupData = new Load_Data.Data_SignupJson
            {
                action = "signup_data",
                username = tb_username.Text,
                password = tb_password.Text,
                email = tb_mail.Text,
                phone = tb_phone.Text,
                request_id = Load_Data.GenerateRandomString(4)
            };
            bool success = Load_Data.SendJson(Load_Data.SignupData);

            if (!success)
            {
                MessageBox.Show("Không thể gửi dữ liệu tới server!", "Lỗi");
                return;
            }

            bool received = await Load_Data.WaitForServerResponse(
                () => Load_Data.SignupData.request_id != null && Load_Data.SignupData.request_id.Contains("ServerHaha")
            );

            if (!received)
            {
                MessageBox.Show("Server không phản hồi kịp thời. Vui lòng thử lại sau!", "Lỗi");
                return;
            }

            MessageBox.Show(Load_Data.SignupData.error, "Thông báo");

            if (Load_Data.SignupData.accept)
            {
                Login login = new Login();
                this.Hide();
                login.ShowDialog();
            }

        }
    }
}
