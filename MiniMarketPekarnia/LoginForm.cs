using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace MiniMarketPekarnia
{
    public partial class LoginForm : Form
    {
        private bool isPasswordVisible = false;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnTogglePassword;
        private Panel leftPanel;
        private Panel rightPanel;

        public LoginForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            SetupForm();
        }

        private void SetupForm()
        {
            // Левая панель с градиентом
            leftPanel = new Panel
            {
                Size = new Size(500, 600),
                Location = new Point(0, 0)
            };
            leftPanel.Paint += LeftPanel_Paint;

            // Правая панель
            rightPanel = new Panel
            {
                Size = new Size(500, 600),
                Location = new Point(500, 0),
                BackColor = Color.White
            };

            // Заголовок на левой панели
            Label lblWelcome = new Label
            {
                Text = "Добро пожаловать в",
                Font = new Font("Segoe UI", 20, FontStyle.Regular),
                ForeColor = Color.White,
                Size = new Size(400, 50),
                Location = new Point(50, 200),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblAppName = new Label
            {
                Text = "Минимаркет\nПекарня",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(400, 150),
                Location = new Point(50, 250),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Иконка на левой панели
            PictureBox pbIcon = new PictureBox
            {
                Size = new Size(100, 100),
                Location = new Point(200, 80),
                BackColor = Color.Transparent
            };
            pbIcon.Paint += PbIcon_Paint;

            leftPanel.Controls.AddRange(new Control[] { lblWelcome, lblAppName, pbIcon });

            // Заголовок формы входа
            Label lblLoginTitle = new Label
            {
                Text = "Вход в систему",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Size = new Size(400, 50),
                Location = new Point(50, 80),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblSubTitle = new Label
            {
                Text = "Введите ваши учетные данные",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(150, 150, 150),
                Size = new Size(400, 30),
                Location = new Point(50, 130),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Поле логина
            Panel pnlUsername = CreateTextBoxPanel("Логин", "Введите логин", out txtUsername, 180);
            pnlUsername.Location = new Point(50, 180);

            // Поле пароля
            Panel pnlPassword = CreateTextBoxPanel("Пароль", "Введите пароль", out txtPassword, 250);
            pnlPassword.Location = new Point(50, 250);
            txtPassword.PasswordChar = '●';
            txtPassword.UseSystemPasswordChar = true;

            // Кнопка показа пароля
            btnTogglePassword = new Button
            {
                Text = "👁",
                Location = new Point(410, 260),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Gray,
                Cursor = Cursors.Hand
            };
            btnTogglePassword.FlatAppearance.BorderSize = 0;
            btnTogglePassword.Click += BtnTogglePassword_Click;

            // Кнопка входа
            btnLogin = new Button
            {
                Text = "ВОЙТИ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(50, 320),
                Size = new Size(400, 50),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            // Кнопка регистрации
            btnRegister = new Button
            {
                Text = "СОЗДАТЬ АККАУНТ",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Location = new Point(50, 390),
                Size = new Size(400, 45),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            btnRegister.Click += BtnRegister_Click;

            // Кнопка закрытия
            Button btnClose = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(460, 10),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(150, 150, 150),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Application.Exit();

            rightPanel.Controls.AddRange(new Control[] {
                lblLoginTitle, lblSubTitle, pnlUsername, pnlPassword,
                btnTogglePassword, btnLogin, btnRegister, btnClose
            });

            this.Controls.Add(leftPanel);
            this.Controls.Add(rightPanel);
        }

        private Panel CreateTextBoxPanel(string labelText, string placeholder, out TextBox textBox, int y)
        {
            Panel panel = new Panel
            {
                Size = new Size(400, 60),
                Location = new Point(50, y),
                BackColor = Color.White
            };

            Label label = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(0, 0),
                Size = new Size(400, 20)
            };

            textBox = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(0, 25),
                Size = new Size(400, 30),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            Panel line = new Panel
            {
                Size = new Size(400, 2),
                Location = new Point(0, 55),
                BackColor = Color.FromArgb(200, 200, 200)
            };

            panel.Controls.AddRange(new Control[] { label, textBox, line });
            return panel;
        }

        private void LeftPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, 500, 600);

            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                Color.FromArgb(44, 62, 80),
                Color.FromArgb(52, 152, 219),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, rect);
            }
        }

        private void PbIcon_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(Color.White, 4))
            {
                // Рисуем хлеб
                g.DrawArc(pen, 10, 20, 80, 40, 0, 180);
                g.DrawLine(pen, 10, 40, 90, 40);
                g.DrawLine(pen, 30, 25, 30, 40);
                g.DrawLine(pen, 50, 20, 50, 40);
                g.DrawLine(pen, 70, 25, 70, 40);
            }
        }

        private void BtnTogglePassword_Click(object sender, EventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;
            txtPassword.UseSystemPasswordChar = !isPasswordVisible;
            btnTogglePassword.Text = isPasswordVisible ? "🔒" : "👁";
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowCustomMessage("Ошибка", "Введите логин и пароль!", MessageType.Warning);
                return;
            }

            if (!DatabaseHelper.TestConnection())
            {
                ShowCustomMessage("Ошибка", "Нет подключения к базе данных!", MessageType.Error);
                return;
            }

            string hashedPassword = HashPassword(password);

            string query = @"SELECT u.UserID, u.Username, u.FullName, u.RoleID, r.RoleName 
                            FROM Users u 
                            INNER JOIN Roles r ON u.RoleID = r.RoleID 
                            WHERE u.Username = @username AND u.PasswordHash = @password AND u.IsActive = 1";

            SqlParameter[] parameters = {
                new SqlParameter("@username", username),
                new SqlParameter("@password", hashedPassword)
            };

            DataTable result = DatabaseHelper.ExecuteQuery(query, parameters);

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                CurrentUser.UserID = Convert.ToInt32(row["UserID"]);
                CurrentUser.Username = row["Username"].ToString();
                CurrentUser.FullName = row["FullName"].ToString();
                CurrentUser.RoleID = Convert.ToInt32(row["RoleID"]);
                CurrentUser.Role = row["RoleName"].ToString();

                this.Hide();
                MainForm mainForm = new MainForm();
                mainForm.Show();
            }
            else
            {
                ShowCustomMessage("Ошибка", "Неверный логин или пароль!", MessageType.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.ShowDialog();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void ShowCustomMessage(string title, string message, MessageType type)
        {
            CustomMessageBox.Show(title, message, type);
        }
    }
}