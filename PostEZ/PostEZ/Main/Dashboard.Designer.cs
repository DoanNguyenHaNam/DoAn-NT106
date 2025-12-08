namespace PostEZ.Main
{
    partial class Dashboard
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
            btn_profile = new Button();
            btn_main = new Button();
            pic_logo = new PictureBox();
            gb_poss = new GroupBox();
            gb_info = new GroupBox();
            lb_logout = new Label();
            lb_countfollower = new Label();
            lb_countpost = new Label();
            lb_mail = new Label();
            lb_username = new Label();
            tb_find = new TextBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_logo).BeginInit();
            gb_info.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btn_profile);
            groupBox1.Controls.Add(btn_main);
            groupBox1.Controls.Add(pic_logo);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(135, 426);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            // 
            // btn_profile
            // 
            btn_profile.Location = new Point(7, 185);
            btn_profile.Name = "btn_profile";
            btn_profile.Size = new Size(122, 34);
            btn_profile.TabIndex = 2;
            btn_profile.Text = "Trang cá nhân";
            btn_profile.UseVisualStyleBackColor = true;
            btn_profile.Click += btn_profile_Click;
            // 
            // btn_main
            // 
            btn_main.Location = new Point(7, 145);
            btn_main.Name = "btn_main";
            btn_main.Size = new Size(122, 34);
            btn_main.TabIndex = 1;
            btn_main.Text = "Trang chủ";
            btn_main.UseVisualStyleBackColor = true;
            btn_main.Click += btn_main_Click;
            // 
            // pic_logo
            // 
            pic_logo.Location = new Point(6, 13);
            pic_logo.Name = "pic_logo";
            pic_logo.Size = new Size(123, 123);
            pic_logo.TabIndex = 0;
            pic_logo.TabStop = false;
            // 
            // gb_poss
            // 
            gb_poss.Location = new Point(153, 64);
            gb_poss.Name = "gb_poss";
            gb_poss.Size = new Size(469, 374);
            gb_poss.TabIndex = 6;
            gb_poss.TabStop = false;
            // 
            // gb_info
            // 
            gb_info.Controls.Add(lb_logout);
            gb_info.Controls.Add(lb_countfollower);
            gb_info.Controls.Add(lb_countpost);
            gb_info.Controls.Add(lb_mail);
            gb_info.Controls.Add(lb_username);
            gb_info.Location = new Point(628, 12);
            gb_info.Name = "gb_info";
            gb_info.Size = new Size(160, 426);
            gb_info.TabIndex = 7;
            gb_info.TabStop = false;
            gb_info.Enter += gb_info_Enter;
            // 
            // lb_logout
            // 
            lb_logout.AutoSize = true;
            lb_logout.Font = new Font("Segoe UI", 12F);
            lb_logout.Location = new Point(6, 220);
            lb_logout.Name = "lb_logout";
            lb_logout.Size = new Size(80, 21);
            lb_logout.TabIndex = 4;
            lb_logout.Text = "Đăng xuất";
            lb_logout.Click += lb_logout_Click;
            // 
            // lb_countfollower
            // 
            lb_countfollower.AutoSize = true;
            lb_countfollower.Font = new Font("Segoe UI", 9F);
            lb_countfollower.Location = new Point(6, 107);
            lb_countfollower.Name = "lb_countfollower";
            lb_countfollower.Size = new Size(101, 15);
            lb_countfollower.TabIndex = 3;
            lb_countfollower.Text = "Số người theo dõi";
            // 
            // lb_countpost
            // 
            lb_countpost.AutoSize = true;
            lb_countpost.Font = new Font("Segoe UI", 9F);
            lb_countpost.Location = new Point(6, 80);
            lb_countpost.Name = "lb_countpost";
            lb_countpost.Size = new Size(69, 15);
            lb_countpost.TabIndex = 2;
            lb_countpost.Text = "Số bài đăng";
            // 
            // lb_mail
            // 
            lb_mail.AutoSize = true;
            lb_mail.Font = new Font("Segoe UI", 9F);
            lb_mail.Location = new Point(6, 52);
            lb_mail.Name = "lb_mail";
            lb_mail.Size = new Size(30, 15);
            lb_mail.TabIndex = 1;
            lb_mail.Text = "Mail";
            // 
            // lb_username
            // 
            lb_username.AutoSize = true;
            lb_username.Font = new Font("Segoe UI", 12F);
            lb_username.Location = new Point(6, 21);
            lb_username.Name = "lb_username";
            lb_username.Size = new Size(81, 21);
            lb_username.TabIndex = 0;
            lb_username.Text = "Username";
            // 
            // tb_find
            // 
            tb_find.Location = new Point(153, 25);
            tb_find.Name = "tb_find";
            tb_find.Size = new Size(469, 23);
            tb_find.TabIndex = 8;
            tb_find.TextChanged += tb_find_TextChanged;
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tb_find);
            Controls.Add(gb_info);
            Controls.Add(gb_poss);
            Controls.Add(groupBox1);
            Name = "Dashboard";
            Text = "Dashboard";
            Load += Dashboard_Load;
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pic_logo).EndInit();
            gb_info.ResumeLayout(false);
            gb_info.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private Button btn_profile;
        private Button btn_main;
        private PictureBox pic_logo;
        private GroupBox gb_poss;
        private GroupBox gb_info;
        private TextBox tb_find;
        private Label lb_countpost;
        private Label lb_mail;
        private Label lb_username;
        private Label lb_logout;
        private Label lb_countfollower;
    }
}