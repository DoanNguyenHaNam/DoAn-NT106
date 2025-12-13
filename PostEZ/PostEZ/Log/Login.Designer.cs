namespace PostEZ.Log
{
    partial class Login
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
            groupBox1 = new GroupBox();
            lb_signup = new Label();
            label3 = new Label();
            label2 = new Label();
            btn_login = new Button();
            tb_password = new TextBox();
            label1 = new Label();
            tb_username = new TextBox();
            pic_logo = new PictureBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_logo).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lb_signup);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(btn_login);
            groupBox1.Controls.Add(tb_password);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(tb_username);
            groupBox1.Location = new Point(403, 68);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(290, 315);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            // 
            // lb_signup
            // 
            lb_signup.AutoSize = true;
            lb_signup.Location = new Point(190, 232);
            lb_signup.Name = "lb_signup";
            lb_signup.Size = new Size(79, 15);
            lb_signup.TabIndex = 6;
            lb_signup.Text = "Tạo tài khoản";
            lb_signup.Click += lb_signup_Click;
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
            // btn_login
            // 
            btn_login.Font = new Font("Segoe UI", 12F);
            btn_login.Location = new Point(92, 185);
            btn_login.Name = "btn_login";
            btn_login.Size = new Size(110, 35);
            btn_login.TabIndex = 3;
            btn_login.Text = "Login";
            btn_login.UseVisualStyleBackColor = true;
            btn_login.Click += btn_login_Click;
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
            label1.Size = new Size(108, 28);
            label1.TabIndex = 1;
            label1.Text = "Đăng nhập";
            // 
            // tb_username
            // 
            tb_username.Font = new Font("Segoe UI", 12F);
            tb_username.Location = new Point(6, 100);
            tb_username.Name = "tb_username";
            tb_username.Size = new Size(278, 29);
            tb_username.TabIndex = 0;
            tb_username.TextChanged += tb_username_TextChanged;
            // 
            // pic_logo
            // 
            pic_logo.Location = new Point(97, 112);
            pic_logo.Name = "pic_logo";
            pic_logo.Size = new Size(240, 240);
            pic_logo.TabIndex = 1;
            pic_logo.TabStop = false;
            pic_logo.Click += pictureBox1_Click;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pic_logo);
            Controls.Add(groupBox1);
            Name = "Login";
            Text = "Login";
            Load += Login_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pic_logo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Button btn_login;
        private TextBox tb_password;
        private Label label1;
        private TextBox tb_username;
        private PictureBox pic_logo;
        private Label label2;
        private Label label3;
        private Label lb_signup;
    }
}