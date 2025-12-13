namespace PostEZ.Main
{
    partial class Message
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
            pic_logo = new PictureBox();
            btn_main = new Button();
            gb_PerOnline = new GroupBox();
            gb_chat = new GroupBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_logo).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(pic_logo);
            groupBox1.Controls.Add(btn_main);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(135, 426);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            // 
            // pic_logo
            // 
            pic_logo.Location = new Point(6, 22);
            pic_logo.Name = "pic_logo";
            pic_logo.Size = new Size(123, 123);
            pic_logo.TabIndex = 10;
            pic_logo.TabStop = false;
            // 
            // btn_main
            // 
            btn_main.Location = new Point(7, 154);
            btn_main.Name = "btn_main";
            btn_main.Size = new Size(122, 34);
            btn_main.TabIndex = 11;
            btn_main.Text = "Thoát";
            btn_main.UseVisualStyleBackColor = true;
            btn_main.Click += btn_main_Click;
            // 
            // gb_PerOnline
            // 
            gb_PerOnline.Location = new Point(153, 12);
            gb_PerOnline.Name = "gb_PerOnline";
            gb_PerOnline.Size = new Size(186, 426);
            gb_PerOnline.TabIndex = 8;
            gb_PerOnline.TabStop = false;
            // 
            // gb_chat
            // 
            gb_chat.Location = new Point(345, 12);
            gb_chat.Name = "gb_chat";
            gb_chat.Size = new Size(443, 426);
            gb_chat.TabIndex = 9;
            gb_chat.TabStop = false;
            // 
            // Message
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(gb_chat);
            Controls.Add(gb_PerOnline);
            Controls.Add(groupBox1);
            Name = "Message";
            Text = "Message";
            Load += Message_Load;
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pic_logo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox gb_PerOnline;
        private GroupBox gb_chat;
        private PictureBox pic_logo;
        private Button btn_main;
    }
}