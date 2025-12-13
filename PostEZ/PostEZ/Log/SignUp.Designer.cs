namespace PostEZ.Log
{
    partial class SignUp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pic_logo = new PictureBox();
            groupBox1 = new GroupBox();
            lb_login = new Label();
            label6 = new Label();
            tb_phone = new TextBox();
            label5 = new Label();
            tb_mail = new TextBox();
            label4 = new Label();
            tb_repass = new TextBox();
            label3 = new Label();
            label2 = new Label();
            btn_signup = new Button();
            tb_password = new TextBox();
            label1 = new Label();
            tb_username = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pic_logo).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // pic_logo
            // 
            pic_logo.Location = new Point(102, 112);
            pic_logo.Name = "pic_logo";
            pic_logo.Size = new Size(240, 240);
            pic_logo.TabIndex = 3;
            pic_logo.TabStop = false;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lb_login);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(tb_phone);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(tb_mail);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(tb_repass);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(btn_signup);
            groupBox1.Controls.Add(tb_password);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(tb_username);
            groupBox1.Location = new Point(408, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(290, 426);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            // 
            // lb_login
            // 
            lb_login.AutoSize = true;
            lb_login.Location = new Point(195, 395);
            lb_login.Name = "lb_login";
            lb_login.Size = new Size(89, 15);
            lb_login.TabIndex = 7;
            lb_login.Text = "Đã có tài khoản";
            lb_login.Click += lb_login_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Enabled = false;
            label6.Location = new Point(6, 283);
            label6.Name = "label6";
            label6.Size = new Size(76, 15);
            label6.TabIndex = 11;
            label6.Text = "Số điện thoại";
            // 
            // tb_phone
            // 
            tb_phone.Font = new Font("Segoe UI", 12F);
            tb_phone.Location = new Point(6, 301);
            tb_phone.Name = "tb_phone";
            tb_phone.Size = new Size(278, 29);
            tb_phone.TabIndex = 10;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Enabled = false;
            label5.Location = new Point(6, 233);
            label5.Name = "label5";
            label5.Size = new Size(38, 15);
            label5.TabIndex = 9;
            label5.Text = "Gmail";
            // 
            // tb_mail
            // 
            tb_mail.Font = new Font("Segoe UI", 12F);
            tb_mail.Location = new Point(6, 251);
            tb_mail.Name = "tb_mail";
            tb_mail.Size = new Size(278, 29);
            tb_mail.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Enabled = false;
            label4.Location = new Point(6, 183);
            label4.Name = "label4";
            label4.Size = new Size(104, 15);
            label4.TabIndex = 7;
            label4.Text = "Nhập lại mật khẩu";
            // 
            // tb_repass
            // 
            tb_repass.Font = new Font("Segoe UI", 12F);
            tb_repass.Location = new Point(6, 201);
            tb_repass.Name = "tb_repass";
            tb_repass.PasswordChar = '*';
            tb_repass.Size = new Size(278, 29);
            tb_repass.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Enabled = false;
            label3.Location = new Point(6, 132);
            label3.Name = "label3";
            label3.Size = new Size(57, 15);
            label3.TabIndex = 5;
            label3.Text = "Mật khẩu";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Enabled = false;
            label2.Location = new Point(6, 82);
            label2.Name = "label2";
            label2.Size = new Size(86, 15);
            label2.TabIndex = 4;
            label2.Text = "Tên đăng nhập";
            // 
            // btn_signup
            // 
            btn_signup.Font = new Font("Segoe UI", 12F);
            btn_signup.Location = new Point(94, 347);
            btn_signup.Name = "btn_signup";
            btn_signup.Size = new Size(110, 35);
            btn_signup.TabIndex = 3;
            btn_signup.Text = "Sign Up";
            btn_signup.UseVisualStyleBackColor = true;
            btn_signup.Click += btn_signup_Click;
            // 
            // tb_password
            // 
            tb_password.Font = new Font("Segoe UI", 12F);
            tb_password.Location = new Point(6, 150);
            tb_password.Name = "tb_password";
            tb_password.PasswordChar = '*';
            tb_password.Size = new Size(278, 29);
            tb_password.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F);
            label1.Location = new Point(94, 29);
            label1.Name = "label1";
            label1.Size = new Size(84, 28);
            label1.TabIndex = 1;
            label1.Text = "Đăng ký";
            // 
            // tb_username
            // 
            tb_username.Font = new Font("Segoe UI", 12F);
            tb_username.Location = new Point(6, 100);
            tb_username.Name = "tb_username";
            tb_username.Size = new Size(278, 29);
            tb_username.TabIndex = 0;
            // 
            // SignUp
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pic_logo);
            Controls.Add(groupBox1);
            Name = "SignUp";
            Text = "SignUp";
            Load += SignUp_Load;
            ((System.ComponentModel.ISupportInitialize)pic_logo).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pic_logo;
        private GroupBox groupBox1;
        private Label label4;
        private TextBox tb_repass;
        private Label label3;
        private Label label2;
        private Button btn_signup;
        private TextBox tb_password;
        private Label label1;
        private TextBox tb_username;
        private Label label6;
        private TextBox tb_phone;
        private Label label5;
        private TextBox tb_mail;
        private Label lb_login;
    }
}