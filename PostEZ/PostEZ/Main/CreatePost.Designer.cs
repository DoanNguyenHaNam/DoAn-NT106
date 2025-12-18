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
            btn_profile = new Button();
            pic_logo = new PictureBox();
            btn_main = new Button();
            groupBox2 = new GroupBox();
            label1 = new Label();
            btn_post = new Button();
            btn_remove_video = new Button();
            lb_video_status = new Label();
            btn_select_video = new Button();
            btn_remove_image = new Button();
            lb_image_status = new Label();
            pic_preview = new PictureBox();
            btn_select_image = new Button();
            tb_content = new TextBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_logo).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_preview).BeginInit();
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
            // btn_profile
            // 
            btn_profile.Location = new Point(7, 194);
            btn_profile.Name = "btn_profile";
            btn_profile.Size = new Size(122, 34);
            btn_profile.TabIndex = 12;
            btn_profile.Text = "Trang cá nhân";
            btn_profile.UseVisualStyleBackColor = true;
            btn_profile.Click += btn_profile_Click;
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
            btn_main.Enabled = false;
            btn_main.Location = new Point(7, 154);
            btn_main.Name = "btn_main";
            btn_main.Size = new Size(122, 34);
            btn_main.TabIndex = 11;
            btn_main.Text = "Trang chủ";
            btn_main.UseVisualStyleBackColor = true;
            btn_main.Click += btn_main_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(btn_post);
            groupBox2.Controls.Add(btn_remove_video);
            groupBox2.Controls.Add(lb_video_status);
            groupBox2.Controls.Add(btn_select_video);
            groupBox2.Controls.Add(btn_remove_image);
            groupBox2.Controls.Add(lb_image_status);
            groupBox2.Controls.Add(pic_preview);
            groupBox2.Controls.Add(btn_select_image);
            groupBox2.Controls.Add(tb_content);
            groupBox2.Location = new Point(153, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(635, 426);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Tạo bài đăng mới";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F);
            label1.ForeColor = Color.Gray;
            label1.Location = new Point(21, 270);
            label1.Name = "label1";
            label1.Size = new Size(590, 19);
            label1.TabIndex = 9;
            label1.Text = "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━";
            // 
            // btn_post
            // 
            btn_post.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            btn_post.Location = new Point(414, 330);
            btn_post.Name = "btn_post";
            btn_post.Size = new Size(197, 69);
            btn_post.TabIndex = 8;
            btn_post.Text = "✅ Đăng bài";
            btn_post.UseVisualStyleBackColor = true;
            btn_post.Click += btn_post_Click;
            // 
            // btn_remove_video
            // 
            btn_remove_video.Font = new Font("Segoe UI", 10F);
            btn_remove_video.ForeColor = Color.Red;
            btn_remove_video.Location = new Point(487, 330);
            btn_remove_video.Name = "btn_remove_video";
            btn_remove_video.Size = new Size(124, 29);
            btn_remove_video.TabIndex = 7;
            btn_remove_video.Text = "❌ Xóa video";
            btn_remove_video.UseVisualStyleBackColor = true;
            btn_remove_video.Visible = false;
            btn_remove_video.Click += btn_remove_video_Click;
            // 
            // lb_video_status
            // 
            lb_video_status.Font = new Font("Segoe UI", 10F);
            lb_video_status.ForeColor = Color.Gray;
            lb_video_status.Location = new Point(21, 296);
            lb_video_status.Name = "lb_video_status";
            lb_video_status.Size = new Size(447, 23);
            lb_video_status.TabIndex = 6;
            lb_video_status.Text = "Chưa chọn video";
            // 
            // btn_select_video
            // 
            btn_select_video.Font = new Font("Segoe UI", 12F);
            btn_select_video.Location = new Point(487, 289);
            btn_select_video.Name = "btn_select_video";
            btn_select_video.Size = new Size(124, 29);
            btn_select_video.TabIndex = 5;
            btn_select_video.Text = "🎥 Chọn video";
            btn_select_video.UseVisualStyleBackColor = true;
            btn_select_video.Click += btn_select_video_Click;
            // 
            // btn_remove_image
            // 
            btn_remove_image.Font = new Font("Segoe UI", 10F);
            btn_remove_image.ForeColor = Color.Red;
            btn_remove_image.Location = new Point(204, 208);
            btn_remove_image.Name = "btn_remove_image";
            btn_remove_image.Size = new Size(106, 36);
            btn_remove_image.TabIndex = 4;
            btn_remove_image.Text = "❌ Xóa ảnh";
            btn_remove_image.UseVisualStyleBackColor = true;
            btn_remove_image.Visible = false;
            btn_remove_image.Click += btn_remove_image_Click;
            // 
            // lb_image_status
            // 
            lb_image_status.Font = new Font("Segoe UI", 10F);
            lb_image_status.ForeColor = Color.Gray;
            lb_image_status.Location = new Point(21, 247);
            lb_image_status.Name = "lb_image_status";
            lb_image_status.Size = new Size(447, 23);
            lb_image_status.TabIndex = 3;
            lb_image_status.Text = "Chưa chọn ảnh";
            // 
            // pic_preview
            // 
            pic_preview.BackColor = Color.WhiteSmoke;
            pic_preview.BorderStyle = BorderStyle.FixedSingle;
            pic_preview.Location = new Point(21, 154);
            pic_preview.Name = "pic_preview";
            pic_preview.Size = new Size(160, 90);
            pic_preview.SizeMode = PictureBoxSizeMode.Zoom;
            pic_preview.TabIndex = 2;
            pic_preview.TabStop = false;
            // 
            // btn_select_image
            // 
            btn_select_image.Font = new Font("Segoe UI", 12F);
            btn_select_image.Location = new Point(204, 154);
            btn_select_image.Name = "btn_select_image";
            btn_select_image.Size = new Size(106, 36);
            btn_select_image.TabIndex = 1;
            btn_select_image.Text = "📷 Chọn ảnh";
            btn_select_image.UseVisualStyleBackColor = true;
            btn_select_image.Click += btn_select_image_Click;
            // 
            // tb_content
            // 
            tb_content.Font = new Font("Segoe UI", 11F);
            tb_content.Location = new Point(21, 44);
            tb_content.Multiline = true;
            tb_content.Name = "tb_content";
            tb_content.PlaceholderText = "Bạn đang nghĩ gì?";
            tb_content.ScrollBars = ScrollBars.Vertical;
            tb_content.Size = new Size(590, 96);
            tb_content.TabIndex = 0;
            // 
            // CreatePost
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "CreatePost";
            Text = "Tạo bài đăng mới";
            Load += CreatePost_Load;
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pic_logo).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pic_preview).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button btn_profile;
        private PictureBox pic_logo;
        private Button btn_main;
        private Button btn_post;
        private Button btn_remove_video;
        private Label lb_video_status;
        private Button btn_select_video;
        private Button btn_remove_image;
        private Label lb_image_status;
        private PictureBox pic_preview;
        private Button btn_select_image;
        private TextBox tb_content;
        private Label label1;
    }
}