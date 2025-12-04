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
            SuspendLayout();
            // 
            // btn_login
            // 
            btn_login.Font = new Font("Segoe UI", 12F);
            btn_login.Location = new Point(351, 146);
            btn_login.Name = "btn_login";
            btn_login.Size = new Size(122, 41);
            btn_login.TabIndex = 0;
            btn_login.Text = "Login";
            btn_login.UseVisualStyleBackColor = true;
            btn_login.Click += btn_login_Click;
            // 
            // btn_close
            // 
            btn_close.Font = new Font("Segoe UI", 12F);
            btn_close.Location = new Point(351, 204);
            btn_close.Name = "btn_close";
            btn_close.Size = new Size(122, 41);
            btn_close.TabIndex = 1;
            btn_close.Text = "Tắt ứng dụng";
            btn_close.UseVisualStyleBackColor = true;
            btn_close.Click += btn_close_Click;
            // 
            // Load_Data
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btn_close);
            Controls.Add(btn_login);
            Name = "Load_Data";
            Text = "Load Data";
            Load += Load_Data_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btn_login;
        private Button btn_close;
    }
}
