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
            lb_username = new Label();
            lb_name = new Label();
            pic_avatar = new PictureBox();
            gb_posted = new GroupBox();
            btn_makepost = new Button();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_logo).BeginInit();
            gb_profile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_avatar).BeginInit();
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
            // 
            // btn_main
            // 
            btn_main.Location = new Point(7, 145);
            btn_main.Name = "btn_main";
            btn_main.Size = new Size(122, 34);
            btn_main.TabIndex = 1;
            btn_main.Text = "Trang chủ";
            btn_main.UseVisualStyleBackColor = true;
            // 
            // pic_logo
            // 
            pic_logo.Location = new Point(6, 13);
            pic_logo.Name = "pic_logo";
            pic_logo.Size = new Size(123, 123);
            pic_logo.TabIndex = 0;
            pic_logo.TabStop = false;
            // 
            // gb_profile
            // 
            gb_profile.Controls.Add(lb_username);
            gb_profile.Controls.Add(lb_name);
            gb_profile.Controls.Add(pic_avatar);
            gb_profile.Location = new Point(153, 12);
            gb_profile.Name = "gb_profile";
            gb_profile.Size = new Size(222, 426);
            gb_profile.TabIndex = 7;
            gb_profile.TabStop = false;
            // 
            // lb_username
            // 
            lb_username.AutoSize = true;
            lb_username.Font = new Font("Segoe UI", 12F);
            lb_username.Location = new Point(6, 247);
            lb_username.Name = "lb_username";
            lb_username.Size = new Size(81, 21);
            lb_username.TabIndex = 2;
            lb_username.Text = "Username";
            // 
            // lb_name
            // 
            lb_name.AutoSize = true;
            lb_name.Font = new Font("Segoe UI", 12F);
            lb_name.Location = new Point(6, 226);
            lb_name.Name = "lb_name";
            lb_name.Size = new Size(52, 21);
            lb_name.TabIndex = 1;
            lb_name.Text = "Name";
            // 
            // pic_avatar
            // 
            pic_avatar.Location = new Point(6, 13);
            pic_avatar.Name = "pic_avatar";
            pic_avatar.Size = new Size(210, 210);
            pic_avatar.TabIndex = 0;
            pic_avatar.TabStop = false;
            // 
            // gb_posted
            // 
            gb_posted.Location = new Point(381, 74);
            gb_posted.Name = "gb_posted";
            gb_posted.Size = new Size(407, 364);
            gb_posted.TabIndex = 8;
            gb_posted.TabStop = false;
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
            // 
            // Profile
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btn_makepost);
            Controls.Add(gb_posted);
            Controls.Add(gb_profile);
            Controls.Add(groupBox1);
            Name = "Profile";
            Text = "Profile";
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pic_logo).EndInit();
            gb_profile.ResumeLayout(false);
            gb_profile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pic_avatar).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Button btn_profile;
        private Button btn_main;
        private PictureBox pic_logo;
        private GroupBox gb_profile;
        private Label lb_username;
        private Label lb_name;
        private PictureBox pic_avatar;
        private GroupBox gb_posted;
        private Button btn_makepost;
    }
}