namespace PostEZ
{
    partial class Load_Data
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btn_login = new Button();
            btn_close = new Button();
            tb_serverip = new TextBox();
            btn_connect = new Button();
            pic_logo = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pic_logo).BeginInit();
            SuspendLayout();
            // 
            // btn_login
            // 
            btn_login.Font = new Font("Segoe UI", 12F);
            btn_login.Location = new Point(490, 327);
            btn_login.Name = "btn_login";
            btn_login.Size = new Size(122, 41);
            btn_login.TabIndex = 0;
            btn_login.Text = "Đăng nhập";
            btn_login.UseVisualStyleBackColor = true;
            btn_login.Click += btn_login_Click;
            // 
            // btn_close
            // 
            btn_close.Font = new Font("Segoe UI", 12F);
            btn_close.Location = new Point(490, 280);
            btn_close.Name = "btn_close";
            btn_close.Size = new Size(122, 41);
            btn_close.TabIndex = 1;
            btn_close.Text = "Tắt ứng dụng";
            btn_close.UseVisualStyleBackColor = true;
            btn_close.Click += btn_close_Click;
            // 
            // tb_serverip
            // 
            tb_serverip.Font = new Font("Segoe UI", 12F);
            tb_serverip.Location = new Point(421, 187);
            tb_serverip.Name = "tb_serverip";
            tb_serverip.Size = new Size(256, 29);
            tb_serverip.TabIndex = 2;
            tb_serverip.Text = "160.191.245.144";
            // 
            // btn_connect
            // 
            btn_connect.Font = new Font("Segoe UI", 12F);
            btn_connect.Location = new Point(490, 233);
            btn_connect.Name = "btn_connect";
            btn_connect.Size = new Size(122, 41);
            btn_connect.TabIndex = 3;
            btn_connect.Text = "Kết nối";
            btn_connect.UseVisualStyleBackColor = true;
            btn_connect.Click += btn_connect_Click;
            // 
            // pic_logo
            // 
            pic_logo.Location = new Point(97, 112);
            pic_logo.Name = "pic_logo";
            pic_logo.Size = new Size(240, 240);
            pic_logo.TabIndex = 4;
            pic_logo.TabStop = false;
            // 
            // Load_Data
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pic_logo);
            Controls.Add(btn_connect);
            Controls.Add(tb_serverip);
            Controls.Add(btn_close);
            Controls.Add(btn_login);
            Name = "Load_Data";
            Text = "Load Data";
            Load += Load_Data_Load;
            ((System.ComponentModel.ISupportInitialize)pic_logo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_login;
        private Button btn_close;
        private TextBox tb_serverip;
        private Button btn_connect;
        private PictureBox pic_logo;
    }
}
