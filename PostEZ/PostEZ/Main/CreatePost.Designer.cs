namespace PostEZ.Main
{
    partial class CreatePost
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
            groupBox2 = new GroupBox();
            btn_profile = new Button();
            btn_main = new Button();
            pic_logo = new PictureBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_logo).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btn_profile);
            groupBox1.Controls.Add(pic_logo);
            groupBox1.Controls.Add(btn_main);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(135, 426);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            groupBox2.Location = new Point(153, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(635, 426);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            // 
            // btn_profile
            // 
            btn_profile.Location = new Point(7, 194);
            btn_profile.Name = "btn_profile";
            btn_profile.Size = new Size(122, 34);
            btn_profile.TabIndex = 12;
            btn_profile.Text = "Trang cá nhân";
            btn_profile.UseVisualStyleBackColor = true;
            // 
            // btn_main
            // 
            btn_main.Location = new Point(7, 154);
            btn_main.Name = "btn_main";
            btn_main.Size = new Size(122, 34);
            btn_main.TabIndex = 11;
            btn_main.Text = "Trang chủ";
            btn_main.UseVisualStyleBackColor = true;
            // 
            // pic_logo
            // 
            pic_logo.Location = new Point(6, 22);
            pic_logo.Name = "pic_logo";
            pic_logo.Size = new Size(123, 123);
            pic_logo.TabIndex = 10;
            pic_logo.TabStop = false;
            // 
            // CreatePost
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "CreatePost";
            Text = "CreatePost";
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pic_logo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button btn_profile;
        private PictureBox pic_logo;
        private Button btn_main;
    }
}