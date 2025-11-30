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
            tb_find = new TextBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_logo).BeginInit();
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
            gb_poss.Location = new Point(153, 48);
            gb_poss.Name = "gb_poss";
            gb_poss.Size = new Size(469, 390);
            gb_poss.TabIndex = 6;
            gb_poss.TabStop = false;
            // 
            // gb_info
            // 
            gb_info.Location = new Point(628, 12);
            gb_info.Name = "gb_info";
            gb_info.Size = new Size(160, 426);
            gb_info.TabIndex = 7;
            gb_info.TabStop = false;
            // 
            // tb_find
            // 
            tb_find.Location = new Point(153, 25);
            tb_find.Name = "tb_find";
            tb_find.Size = new Size(469, 23);
            tb_find.TabIndex = 8;
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
    }
}