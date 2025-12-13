using PostEZ.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostEZ.Main
{
    public partial class Message : Form
    {
        private System.Threading.Timer? autoRefreshTimer;
        private Panel? usersListPanel = null;
        private Panel? chatPanel = null;
        private TextBox? txtMessage = null;
        private Button? btnSend = null;
        private Label? lblChatWith = null;
        private string? selectedUserToChat = null;

        public Message()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private async void Message_Load(object sender, EventArgs e)
        {
            try
            {
                await Login.LoadFromUrl("https://pminmod.site/doannt106/logo.png", pic_logo);

                SetupUsersListPanel();
                SetupChatPanel();

                await LoadOnlineUsers();

                StartAutoRefreshMessages();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo: " + ex.Message, "Lỗi");
            }
        }

        // ============================================
        // SETUP UI - DANH SÁCH USERS ONLINE
        // ============================================
        private void SetupUsersListPanel()
        {
            usersListPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = gb_PerOnline.Size,
                AutoScroll = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White
            };
            gb_PerOnline.Controls.Add(usersListPanel);
            gb_PerOnline.Text = "👥 Users Online";
        }

        // ============================================
        // SETUP UI - KHUNG CHAT
        // ============================================
        private void SetupChatPanel()
        {
            gb_chat.Text = "💬 Chat";
            lblChatWith = new Label
            {
                Text = "Chọn một user để chat",
                Location = new Point(10, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DodgerBlue
            };
            gb_chat.Controls.Add(lblChatWith);

            chatPanel = new Panel
            {
                Location = new Point(10, 50),
                Size = new Size(gb_chat.Width - 25, gb_chat.Height - 120),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            gb_chat.Controls.Add(chatPanel);

            txtMessage = new TextBox
            {
                Location = new Point(10, chatPanel.Bottom + 10),
                Size = new Size(gb_chat.Width - 110, 30),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Nhập tin nhắn..."
            };
            gb_chat.Controls.Add(txtMessage);

            btnSend = new Button
            {
                Text = "📤 Gửi",
                Location = new Point(txtMessage.Right + 5, txtMessage.Top),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Click += BtnSend_Click;
            gb_chat.Controls.Add(btnSend);
        }

        // ============================================
        // TỰ ĐỘNG REFRESH MESSAGES VÀ USERS MỖI 2-3 GIÂY
        // ============================================
        private void StartAutoRefreshMessages()
        {
            autoRefreshTimer?.Dispose();

            autoRefreshTimer = new System.Threading.Timer(async _ =>
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(async () =>
                    {
                        try
                        {
                            await LoadOnlineUsers();

                            if (!string.IsNullOrEmpty(selectedUserToChat))
                            {
                                await LoadMessages(selectedUserToChat, silentMode: true);
                            }
                        }
                        catch
                        {

                        }
                    }));
                }
            }, null, 5000, 5000);
        }

        // ============================================
        // LOAD DANH SÁCH USERS ONLINE
        // ============================================
        private async Task LoadOnlineUsers()
        {
            try
            {
                var request = new
                {
                    action = "get_online_users",
                    username = Load_Data.LoginData.username,
                    request_id = Load_Data.GenerateRandomString(4)
                };

                bool success = Load_Data.SendJson(request);
                if (!success)
                {
                    MessageBox.Show("Không thể kết nối server!", "Lỗi");
                    return;
                }

                bool received = await Load_Data.WaitForServerResponse(
                    () => Load_Data.GetOnlineUsersResponse.request_id != null &&
                          Load_Data.GetOnlineUsersResponse.request_id.Contains("ServerHaha")
                );

                if (received && Load_Data.GetOnlineUsersResponse.accept)
                {
                    DisplayOnlineUsers(Load_Data.GetOnlineUsersResponse.users);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi");
            }
        }

        private void DisplayOnlineUsers(List<string>? users)
        {
            if (usersListPanel == null) return;

            usersListPanel.Controls.Clear();

            if (users == null || users.Count == 0)
            {
                Label lblNoUsers = new Label
                {
                    Text = "Không có user nào online",
                    AutoSize = true,
                    Location = new Point(10, 10),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Gray
                };
                usersListPanel.Controls.Add(lblNoUsers);
                return;
            }

            int yPosition = 10;
            foreach (var user in users)
            {
                if (user == Load_Data.LoginData.username)
                    continue;

                Button btnUser = new Button
                {
                    Text = $"👤 {user}",
                    Location = new Point(10, yPosition),
                    Size = new Size(usersListPanel.Width - 25, 40),
                    Font = new Font("Segoe UI", 9),
                    TextAlign = ContentAlignment.MiddleLeft,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    BackColor = Color.WhiteSmoke,
                    Tag = user
                };
                btnUser.FlatAppearance.BorderColor = Color.LightGray;
                btnUser.Click += async (s, e) =>
                {
                    selectedUserToChat = user;
                    if (lblChatWith != null)
                    {
                        lblChatWith.Text = $"💬 Chat với: {user}";
                    }
                    if (btnSend != null)
                    {
                        btnSend.Enabled = true;
                    }
                    await LoadMessages(user);
                };

                usersListPanel.Controls.Add(btnUser);
                yPosition += 45;
            }
        }

        // ============================================
        // LOAD TIN NHẮN
        // ============================================
        private async Task LoadMessages(string withUser, bool silentMode = false)
        {
            try
            {
                var request = new
                {
                    action = "get_messages",
                    from_user = Load_Data.LoginData.username,
                    to_user = withUser,
                    request_id = Load_Data.GenerateRandomString(4)
                };

                bool success = Load_Data.SendJson(request);
                if (!success && !silentMode)
                {
                    MessageBox.Show("Không thể kết nối server!", "Lỗi");
                    return;
                }

                bool received = await Load_Data.WaitForServerResponse(
                    () => Load_Data.GetMessagesResponse.request_id != null &&
                          Load_Data.GetMessagesResponse.request_id.Contains("ServerHaha"),
                    timeoutSeconds: 1
                );

                if (received && Load_Data.GetMessagesResponse.accept)
                {
                    DisplayMessages(Load_Data.GetMessagesResponse.messages);
                }
            }
            catch (Exception ex)
            {
                if (!silentMode)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi");
                }
            }
        }

        private void DisplayMessages(List<Load_Data.Data_MessageJson>? messages)
        {
            if (chatPanel == null) return;

            int currentScroll = chatPanel.VerticalScroll.Value;
            bool wasAtBottom = (chatPanel.VerticalScroll.Value >= chatPanel.VerticalScroll.Maximum - chatPanel.Height);

            chatPanel.Controls.Clear();

            if (messages == null || messages.Count == 0)
            {
                Label lblNoMessages = new Label
                {
                    Text = "Chưa có tin nhắn nào. Hãy gửi tin nhắn đầu tiên!",
                    AutoSize = true,
                    Location = new Point(10, 10),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Gray
                };
                chatPanel.Controls.Add(lblNoMessages);
                return;
            }

            int yPosition = 10;
            foreach (var message in messages)
            {
                bool isMyMessage = message.from_user == Load_Data.LoginData.username;
                Panel msgPanel = CreateMessagePanel(message, yPosition, isMyMessage);
                chatPanel.Controls.Add(msgPanel);
                yPosition += msgPanel.Height + 10;
            }
            if (wasAtBottom)
            {
                chatPanel.AutoScrollPosition = new Point(0, chatPanel.VerticalScroll.Maximum);
            }
        }

        private Panel CreateMessagePanel(Load_Data.Data_MessageJson message, int yPosition, bool isMyMessage)
        {
            Panel panel = new Panel
            {
                Location = new Point(isMyMessage ? chatPanel!.Width - 270 : 10, yPosition),
                Width = 250,
                AutoSize = false,
                Height = 60,
                BackColor = isMyMessage ? Color.FromArgb(220, 240, 255) : Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblContent = new Label
            {
                Text = message.content,
                Location = new Point(10, 10),
                MaximumSize = new Size(230, 0),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black
            };
            panel.Controls.Add(lblContent);

            Label lblTime = new Label
            {
                Text = FormatTimestamp(message.timestamp),
                Location = new Point(10, lblContent.Bottom + 5),
                AutoSize = true,
                Font = new Font("Segoe UI", 7),
                ForeColor = Color.Gray
            };
            panel.Controls.Add(lblTime);

            panel.Height = Math.Max(60, lblTime.Bottom + 10);

            return panel;
        }

        private string FormatTimestamp(string timestamp)
        {
            try
            {
                if (long.TryParse(timestamp, out long unixTime))
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTime);
                    DateTime localTime = dateTimeOffset.LocalDateTime;
                    return localTime.ToString("HH:mm dd/MM");
                }
                return timestamp;
            }
            catch
            {
                return timestamp;
            }
        }

        // ============================================
        // GỬI TIN NHẮN
        // ============================================
        private async void BtnSend_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessage?.Text) || selectedUserToChat == null)
            {
                return;
            }

            try
            {
                if (btnSend != null)
                {
                    btnSend.Enabled = false;
                }

                var request = new
                {
                    action = "send_message",
                    from_user = Load_Data.LoginData.username,
                    to_user = selectedUserToChat,
                    content = txtMessage?.Text,
                    request_id = Load_Data.GenerateRandomString(4)
                };

                bool success = Load_Data.SendJson(request);
                if (!success)
                {
                    MessageBox.Show("Không thể kết nối server!", "Lỗi");
                    return;
                }

                bool received = await Load_Data.WaitForServerResponse(
                    () => Load_Data.SendMessageResponse.request_id != null &&
                          Load_Data.SendMessageResponse.request_id.Contains("ServerHaha"),
                    timeoutSeconds: 5
                );

                if (received && Load_Data.SendMessageResponse.accept)
                {
                    if (txtMessage != null)
                    {
                        txtMessage.Clear();
                    }
                    await LoadMessages(selectedUserToChat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi");
            }
            finally
            {
                if (btnSend != null)
                {
                    btnSend.Enabled = true;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            autoRefreshTimer?.Dispose();
            base.OnFormClosing(e);
        }

        private void btn_main_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
