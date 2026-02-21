using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace MiniMarketPekarnia
{
    public partial class RegisterForm : Form
    {
        private TextBox txtFullName;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Button btnRegister;
        private Button btnCancel;
        private CheckBox chkShowPassword;
        private Panel leftPanel;
        private Panel rightPanel;
        private bool isPasswordVisible = false;

        public RegisterForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            SetupForm();
        }

        private void SetupForm()
        {
            // Левая панель с градиентом
            leftPanel = new Panel
            {
                Size = new Size(450, 600),
                Location = new Point(0, 0)
            };
            leftPanel.Paint += LeftPanel_Paint;

            // Правая панель
            rightPanel = new Panel
            {
                Size = new Size(550, 600),
                Location = new Point(450, 0),
                BackColor = Color.White
            };

            // Иконка на левой панели
            PictureBox pbIcon = new PictureBox
            {
                Size = new Size(120, 120),
                Location = new Point(165, 120),
                BackColor = Color.Transparent
            };
            pbIcon.Paint += PbIcon_Paint;

            // Текст на левой панели
            Label lblWelcome = new Label
            {
                Text = "Добро пожаловать!",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(400, 40),
                Location = new Point(25, 280),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblSubText = new Label
            {
                Text = "Присоединяйтесь к нам\nи получайте бонусы",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(220, 220, 220),
                Size = new Size(400, 50),
                Location = new Point(25, 330),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblAlreadyHaveAccount = new Label
            {
                Text = "Уже есть аккаунт?",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 200, 200),
                Size = new Size(200, 20),
                Location = new Point(125, 450),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };

            Button btnLogin = new Button
            {
                Text = "Войти",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(330, 447),
                Size = new Size(80, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += (s, e) =>
            {
                this.Close();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
            };

            leftPanel.Controls.AddRange(new Control[] {
                pbIcon, lblWelcome, lblSubText, lblAlreadyHaveAccount, btnLogin
            });

            // Заголовок формы регистрации
            Label lblRegisterTitle = new Label
            {
                Text = "Регистрация",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Size = new Size(400, 50),
                Location = new Point(50, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblSubTitle = new Label
            {
                Text = "Создайте новый аккаунт",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(150, 150, 150),
                Size = new Size(400, 30),
                Location = new Point(50, 90),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Поле ФИО
            Panel pnlFullName = CreateTextBoxPanel("Полное имя", "Введите ваше полное имя", out txtFullName, 140);
            pnlFullName.Location = new Point(50, 140);

            // Поле Логин
            Panel pnlUsername = CreateTextBoxPanel("Логин", "Введите логин", out txtUsername, 210);
            pnlUsername.Location = new Point(50, 210);

            // Поле Пароль
            Panel pnlPassword = CreateTextBoxPanel("Пароль", "Введите пароль", out txtPassword, 280);
            pnlPassword.Location = new Point(50, 280);
            txtPassword.PasswordChar = '●';
            txtPassword.UseSystemPasswordChar = true;

            // Поле Подтверждение пароля
            Panel pnlConfirmPassword = CreateTextBoxPanel("Подтверждение пароля", "Повторите пароль", out txtConfirmPassword, 350);
            pnlConfirmPassword.Location = new Point(50, 350);
            txtConfirmPassword.PasswordChar = '●';
            txtConfirmPassword.UseSystemPasswordChar = true;

            // Кнопка показа пароля
            Button btnTogglePassword = new Button
            {
                Text = "👁",
                Location = new Point(460, 292),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Gray,
                Cursor = Cursors.Hand
            };
            btnTogglePassword.FlatAppearance.BorderSize = 0;
            btnTogglePassword.Click += (s, e) =>
            {
                isPasswordVisible = !isPasswordVisible;
                txtPassword.UseSystemPasswordChar = !isPasswordVisible;
                txtConfirmPassword.UseSystemPasswordChar = !isPasswordVisible;
                btnTogglePassword.Text = isPasswordVisible ? "🔒" : "👁";
            };

            // Кнопка показать пароль для подтверждения
            Button btnToggleConfirmPassword = new Button
            {
                Text = "👁",
                Location = new Point(460, 362),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Gray,
                Cursor = Cursors.Hand
            };
            btnToggleConfirmPassword.FlatAppearance.BorderSize = 0;
            btnToggleConfirmPassword.Click += (s, e) =>
            {
                txtConfirmPassword.UseSystemPasswordChar = !txtConfirmPassword.UseSystemPasswordChar;
            };

            // Чекбокс "Показать пароли"
            CheckBox chkShowAllPasswords = new CheckBox
            {
                Text = "Показать пароли",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(50, 410),
                Size = new Size(150, 25),
                Cursor = Cursors.Hand
            };
            chkShowAllPasswords.CheckedChanged += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar = !chkShowAllPasswords.Checked;
                txtConfirmPassword.UseSystemPasswordChar = !chkShowAllPasswords.Checked;
            };

            // Кнопка регистрации
            btnRegister = new Button
            {
                Text = "ЗАРЕГИСТРИРОВАТЬСЯ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(50, 450),
                Size = new Size(250, 50),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            // Кнопка отмены
            btnCancel = new Button
            {
                Text = "ОТМЕНА",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Location = new Point(320, 450),
                Size = new Size(150, 50),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(149, 165, 166),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(149, 165, 166);
            btnCancel.Click += (s, e) => this.Close();

            // Кнопка закрытия
            Button btnClose = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(510, 10),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(150, 150, 150),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            rightPanel.Controls.AddRange(new Control[] {
                lblRegisterTitle, lblSubTitle,
                pnlFullName, pnlUsername, pnlPassword, pnlConfirmPassword,
                btnTogglePassword, btnToggleConfirmPassword, chkShowAllPasswords,
                btnRegister, btnCancel, btnClose
            });

            this.Controls.Add(leftPanel);
            this.Controls.Add(rightPanel);
        }

        private Panel CreateTextBoxPanel(string labelText, string placeholder, out TextBox textBox, int y)
        {
            Panel panel = new Panel
            {
                Size = new Size(450, 60),
                Location = new Point(50, y),
                BackColor = Color.White
            };

            Label label = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(0, 0),
                Size = new Size(450, 20)
            };

            textBox = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(0, 25),
                Size = new Size(420, 30),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            Panel line = new Panel
            {
                Size = new Size(450, 2),
                Location = new Point(0, 55),
                BackColor = Color.FromArgb(200, 200, 200)
            };

            panel.Controls.AddRange(new Control[] { label, textBox, line });
            return panel;
        }

        private void LeftPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, 450, 600);

            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                Color.FromArgb(44, 62, 80),
                Color.FromArgb(52, 152, 219),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, rect);
            }

            // Добавляем декоративные элементы
            using (Pen pen = new Pen(Color.FromArgb(255, 255, 255, 30), 2))
            {
                for (int i = 0; i < 5; i++)
                {
                    int y = 100 + i * 100;
                    g.DrawArc(pen, -50, y, 200, 80, 0, 180);
                }
            }
        }

        private void PbIcon_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Рисуем иконку пользователя
            using (Pen pen = new Pen(Color.White, 4))
            {
                // Голова
                g.DrawEllipse(pen, 35, 20, 50, 50);
                // Тело
                g.DrawArc(pen, 20, 60, 80, 60, 0, 180);
                // Руки
                g.DrawLine(pen, 20, 70, 5, 90);
                g.DrawLine(pen, 100, 70, 115, 90);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // Валидация
            if (string.IsNullOrEmpty(fullName))
            {
                ShowError("Введите полное имя!");
                txtFullName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(username))
            {
                ShowError("Введите логин!");
                txtUsername.Focus();
                return;
            }

            if (username.Length < 3)
            {
                ShowError("Логин должен содержать не менее 3 символов!");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Введите пароль!");
                txtPassword.Focus();
                return;
            }

            if (password.Length < 6)
            {
                ShowError("Пароль должен содержать не менее 6 символов!");
                txtPassword.Focus();
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Пароли не совпадают!");
                txtConfirmPassword.Focus();
                return;
            }

            // Проверка существования пользователя
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
            SqlParameter[] checkParams = { new SqlParameter("@username", username) };
            int exists = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParams));

            if (exists > 0)
            {
                ShowError("Пользователь с таким логином уже существует!");
                return;
            }

            // Регистрация
            string hashedPassword = HashPassword(password);
            string insertQuery = @"INSERT INTO Users (Username, PasswordHash, FullName, RoleID, IsActive) 
                                  VALUES (@username, @password, @fullName, 2, 1)";

            SqlParameter[] insertParams = {
                new SqlParameter("@username", username),
                new SqlParameter("@password", hashedPassword),
                new SqlParameter("@fullName", fullName)
            };

            int result = DatabaseHelper.ExecuteNonQuery(insertQuery, insertParams);

            if (result > 0)
            {
                // Показываем сообщение об успехе
                ShowSuccess("Регистрация успешна! Теперь вы можете войти в систему.");

                // Закрываем форму регистрации и открываем форму входа
                this.Close();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
            }
            else
            {
                ShowError("Ошибка при регистрации!");
            }
        }

        private void ShowError(string message)
        {
            using (Form errorForm = new Form
            {
                Size = new Size(350, 180),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.White
            })
            {
                Panel header = new Panel
                {
                    Size = new Size(350, 40),
                    Location = new Point(0, 0),
                    BackColor = Color.FromArgb(231, 76, 60)
                };

                Label lblTitle = new Label
                {
                    Text = "Ошибка",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(10, 8),
                    Size = new Size(330, 24)
                };

                Label lblMessage = new Label
                {
                    Text = message,
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(50, 50, 50),
                    Location = new Point(20, 60),
                    Size = new Size(310, 60),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                Button btnOk = new Button
                {
                    Text = "OK",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Location = new Point(130, 130),
                    Size = new Size(90, 30),
                    BackColor = Color.FromArgb(231, 76, 60),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnOk.FlatAppearance.BorderSize = 0;
                btnOk.Click += (s, ev) => errorForm.Close();

                header.Controls.Add(lblTitle);
                errorForm.Controls.AddRange(new Control[] { header, lblMessage, btnOk });
                errorForm.ShowDialog();
            }
        }

        private void ShowSuccess(string message)
        {
            using (Form successForm = new Form
            {
                Size = new Size(350, 180),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.White
            })
            {
                Panel header = new Panel
                {
                    Size = new Size(350, 40),
                    Location = new Point(0, 0),
                    BackColor = Color.FromArgb(46, 204, 113)
                };

                Label lblTitle = new Label
                {
                    Text = "Успех",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(10, 8),
                    Size = new Size(330, 24)
                };

                Label lblMessage = new Label
                {
                    Text = message,
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(50, 50, 50),
                    Location = new Point(20, 60),
                    Size = new Size(310, 60),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                Button btnOk = new Button
                {
                    Text = "OK",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Location = new Point(130, 130),
                    Size = new Size(90, 30),
                    BackColor = Color.FromArgb(46, 204, 113),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnOk.FlatAppearance.BorderSize = 0;
                btnOk.Click += (s, ev) => successForm.Close();

                header.Controls.Add(lblTitle);
                successForm.Controls.AddRange(new Control[] { header, lblMessage, btnOk });
                successForm.ShowDialog();
            }
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
    }
}