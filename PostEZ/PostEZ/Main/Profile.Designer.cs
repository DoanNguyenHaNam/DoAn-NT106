namespace PostEZ.Main
{
    partial class Profile
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
            gb_profile = new GroupBox();
            lb_follower = new Label();
            lb_bio = new Label();
            lb_name = new Label();
            pic_avatar = new PictureBox();
            gb_posted = new GroupBox();
            btn_makepost = new Button();
            lb_count = new Label();
            btn_refresh = new Button();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_logo).BeginInit();
            gb_profile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_avatar).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btn_refresh);
            groupBox1.Controls.Add(btn_profile);
            groupBox1.Controls.Add(btn_main);
            groupBox1.Controls.Add(pic_logo);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(135, 426);
            groupBox1.TabIndex = 6;
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
            pic_logo.Click += pic_logo_Click;
            // 
            // gb_profile
            // 
            gb_profile.Controls.Add(lb_follower);
            gb_profile.Controls.Add(lb_bio);
            gb_profile.Controls.Add(lb_name);
            gb_profile.Controls.Add(pic_avatar);
            gb_profile.Location = new Point(153, 12);
            gb_profile.Name = "gb_profile";
            gb_profile.Size = new Size(222, 426);
            gb_profile.TabIndex = 7;
            gb_profile.TabStop = false;
            // 
            // lb_follower
            // 
            lb_follower.AutoSize = true;
            lb_follower.Font = new Font("Segoe UI", 12F);
            lb_follower.Location = new Point(6, 276);
            lb_follower.Name = "lb_follower";
            lb_follower.Size = new Size(77, 21);
            lb_follower.TabIndex = 11;
            lb_follower.Text = "Follower: ";
            // 
            // lb_bio
            // 
            lb_bio.AutoSize = true;
            lb_bio.Font = new Font("Segoe UI", 12F);
            lb_bio.Location = new Point(6, 306);
            lb_bio.Name = "lb_bio";
            lb_bio.Size = new Size(32, 21);
            lb_bio.TabIndex = 2;
            lb_bio.Text = "Bio";
            // 
            // lb_name
            // 
            lb_name.AutoSize = true;
            lb_name.Font = new Font("Segoe UI", 15F);
            lb_name.Location = new Point(6, 13);
            lb_name.Name = "lb_name";
            lb_name.Size = new Size(64, 28);
            lb_name.TabIndex = 1;
            lb_name.Text = "Name";
            // 
            // pic_avatar
            // 
            pic_avatar.Location = new Point(6, 62);
            pic_avatar.Name = "pic_avatar";
            pic_avatar.Size = new Size(210, 210);
            pic_avatar.TabIndex = 0;
            pic_avatar.TabStop = false;
            // 
            // gb_posted
            // 
            gb_posted.Location = new Point(381, 98);
            gb_posted.Name = "gb_posted";
            gb_posted.Size = new Size(407, 340);
            gb_posted.TabIndex = 8;
            gb_posted.TabStop = false;
            gb_posted.Enter += gb_posted_Enter;
            // 
            // btn_makepost
            // 
            btn_makepost.Font = new Font("Segoe UI", 12F);
            btn_makepost.Location = new Point(381, 25);
            btn_makepost.Name = "btn_makepost";
            btn_makepost.Size = new Size(407, 43);
            btn_makepost.TabIndex = 9;
            btn_makepost.Text = "Post";
            btn_makepost.UseVisualStyleBackColor = true;
            btn_makepost.Click += btn_makepost_Click;
            // 
            // lb_count
            // 
            lb_count.AutoSize = true;
            lb_count.Font = new Font("Segoe UI", 12F);
            lb_count.Location = new Point(381, 74);
            lb_count.Name = "lb_count";
            lb_count.Size = new Size(99, 21);
            lb_count.TabIndex = 10;
            lb_count.Text = "Số bài đăng: ";
            // 
            // btn_refresh
            // 
            btn_refresh.Location = new Point(7, 225);
            btn_refresh.Name = "btn_refresh";
            btn_refresh.Size = new Size(122, 34);
            btn_refresh.TabIndex = 3;
            btn_refresh.Text = "Refresh";
            btn_refresh.UseVisualStyleBackColor = true;
            // 
            // Profile
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lb_count);
            Controls.Add(btn_makepost);
            Controls.Add(gb_posted);
            Controls.Add(gb_profile);
            Controls.Add(groupBox1);
            Name = "Profile";
            Text = "Profile";
            Load += Profile_Load;
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pic_logo).EndInit();
            gb_profile.ResumeLayout(false);
            gb_profile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pic_avatar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private Button btn_profile;
        private Button btn_main;
        private PictureBox pic_logo;
        private GroupBox gb_profile;
        private Label lb_bio;
        private Label lb_name;
        private PictureBox pic_avatar;
        private GroupBox gb_posted;
        private Button btn_makepost;
        private Label lb_follower;
        private Label lb_count;
        private Button btn_refresh;
    }
}