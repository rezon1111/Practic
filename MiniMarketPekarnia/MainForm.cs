using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MiniMarketPekarnia
{
    public partial class MainForm : Form
    {
        private Panel headerPanel;
        private Panel sidebarPanel;
        private Panel contentPanel;
        private Label lblWelcome;
        private Label lblRole;
        private Button btnDashboard;
        private Button btnProducts;
        private Button btnSales;
        private Button btnCustomers;
        private Button btnSupplies;
        private Button btnReports;
        private Button btnUsers;
        private Button btnCategories;
        private Button btnSettings;
        private Button btnLogout;
        private Button btnMinimize;
        private Button btnClose;

        public MainForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            SetupForm();
            ConfigureAccessByRole();
        }

        private void SetupForm()
        {
            // Header panel
            headerPanel = new Panel
            {
                Size = new Size(1400, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(44, 62, 80)
            };

            Label lblAppName = new Label
            {
                Text = "Минимаркет Пекарня",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(300, 30)
            };

            lblWelcome = new Label
            {
                Text = $"Добро пожаловать, {CurrentUser.FullName}",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(350, 15),
                Size = new Size(400, 25)
            };

            lblRole = new Label
            {
                Text = $"Роль: {CurrentUser.Role}",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(350, 40),
                Size = new Size(200, 20)
            };

            btnMinimize = new Button
            {
                Text = "—",
                Font = new Font("Segoe UI", 14),
                Location = new Point(1300, 15),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

            btnClose = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14),
                Location = new Point(1350, 15),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Application.Exit();

            headerPanel.Controls.AddRange(new Control[] {
                lblAppName, lblWelcome, lblRole, btnMinimize, btnClose
            });

            // Sidebar panel
            sidebarPanel = new Panel
            {
                Size = new Size(250, 740),
                Location = new Point(0, 60),
                BackColor = Color.FromArgb(52, 73, 94)
            };
            sidebarPanel.Paint += SidebarPanel_Paint;

            // Content panel
            contentPanel = new Panel
            {
                Size = new Size(1150, 740),
                Location = new Point(250, 60),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            // Create sidebar buttons
            btnDashboard = CreateSidebarButton("🏠", "Главная", 10);
            btnProducts = CreateSidebarButton("📦", "Товары", 70);
            btnSales = CreateSidebarButton("💰", "Продажи", 130);
            btnCustomers = CreateSidebarButton("👥", "Клиенты", 190);

            // Административные функции (будут скрыты для кассира)
            btnSupplies = CreateSidebarButton("📥", "Поставки", 250);
            btnReports = CreateSidebarButton("📊", "Отчеты", 310);
            btnUsers = CreateSidebarButton("👤", "Пользователи", 370);
            btnCategories = CreateSidebarButton("🏷️", "Категории", 430);

            btnSettings = CreateSidebarButton("⚙️", "Настройки", 550);
            btnLogout = CreateSidebarButton("🚪", "Выход", 610);

            // Подписка на события
            btnDashboard.Click += BtnDashboard_Click;
            btnProducts.Click += BtnProducts_Click;
            btnSales.Click += BtnSales_Click;
            btnCustomers.Click += BtnCustomers_Click;
            btnSupplies.Click += BtnSupplies_Click;
            btnReports.Click += BtnReports_Click;
            btnUsers.Click += BtnUsers_Click;
            btnCategories.Click += BtnCategories_Click;
            btnLogout.Click += BtnLogout_Click;

            // Добавляем кнопки на боковую панель
            sidebarPanel.Controls.Add(btnDashboard);
            sidebarPanel.Controls.Add(btnProducts);
            sidebarPanel.Controls.Add(btnSales);
            sidebarPanel.Controls.Add(btnCustomers);

            // Административные кнопки добавим позже, в зависимости от роли

            sidebarPanel.Controls.Add(btnSettings);
            sidebarPanel.Controls.Add(btnLogout);

            this.Controls.AddRange(new Control[] { headerPanel, sidebarPanel, contentPanel });

            // Показываем дашборд по умолчанию
            ShowDashboard();
        }

        private void ConfigureAccessByRole()
        {
            // Для кассира скрываем административные функции
            if (CurrentUser.Role == "Кассир")
            {
                // Скрываем кнопки администратора
                btnSupplies.Visible = false;
                btnReports.Visible = false;
                btnUsers.Visible = false;
                btnCategories.Visible = false;

                // Перемещаем кнопку настроек выше
                btnSettings.Location = new Point(10, 370);
                btnLogout.Location = new Point(10, 430);
            }
            else
            {
                // Для администратора показываем все кнопки
                sidebarPanel.Controls.Add(btnSupplies);
                sidebarPanel.Controls.Add(btnReports);
                sidebarPanel.Controls.Add(btnUsers);
                sidebarPanel.Controls.Add(btnCategories);

                // Расставляем в правильном порядке
                btnDashboard.Location = new Point(10, 10);
                btnProducts.Location = new Point(10, 70);
                btnSales.Location = new Point(10, 130);
                btnCustomers.Location = new Point(10, 190);
                btnSupplies.Location = new Point(10, 250);
                btnReports.Location = new Point(10, 310);
                btnUsers.Location = new Point(10, 370);
                btnCategories.Location = new Point(10, 430);
                btnSettings.Location = new Point(10, 550);
                btnLogout.Location = new Point(10, 610);
            }
        }

        private Button CreateSidebarButton(string icon, string text, int y)
        {
            Button btn = new Button
            {
                Text = $"{icon}  {text}",
                Font = new Font("Segoe UI", 12),
                Location = new Point(10, y),
                Size = new Size(230, 50),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(41, 128, 185);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(52, 73, 94);
            return btn;
        }

        private void SidebarPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                new Rectangle(0, 0, 250, 740),
                Color.FromArgb(52, 73, 94),
                Color.FromArgb(44, 62, 80),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, new Rectangle(0, 0, 250, 740));
            }
        }

        private void ShowDashboard()
        {
            contentPanel.Controls.Clear();

            Panel dashboardPanel = new Panel
            {
                Size = contentPanel.Size,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            Label lblTitle = new Label
            {
                Text = "Главная панель",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(30, 20),
                Size = new Size(500, 50)
            };

            // Статистика
            Panel statsPanel = new Panel
            {
                Size = new Size(1100, 150),
                Location = new Point(25, 80),
                BackColor = Color.White
            };

            statsPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, statsPanel.ClientRectangle,
                    Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
            };

            // Для кассира показываем другую статистику
            if (CurrentUser.Role == "Кассир")
            {
                string[,] stats = {
                    { "📦", "Товаров в наличии", GetProductCount() },
                    { "💰", "Продаж сегодня", GetTodaySales() },
                    { "👥", "Клиентов", GetCustomerCount() }
                };

                for (int i = 0; i < 3; i++)
                {
                    Panel card = CreateStatCard(stats[i, 0], stats[i, 1], stats[i, 2], i);
                    statsPanel.Controls.Add(card);
                }
            }
            else
            {
                string[,] stats = {
                    { "📦", "Товаров", GetProductCount() },
                    { "💰", "Продаж сегодня", GetTodaySales() },
                    { "👥", "Клиентов", GetCustomerCount() },
                    { "📊", "Выручка", GetRevenue() }
                };

                for (int i = 0; i < 4; i++)
                {
                    Panel card = CreateStatCard(stats[i, 0], stats[i, 1], stats[i, 2], i);
                    statsPanel.Controls.Add(card);
                }
            }

            // Быстрые действия
            Label lblQuickActions = new Label
            {
                Text = "Быстрые действия",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(30, 250),
                Size = new Size(300, 30)
            };

            Panel actionsPanel = new Panel
            {
                Size = new Size(1100, 200),
                Location = new Point(25, 290),
                BackColor = Color.White
            };

            actionsPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, actionsPanel.ClientRectangle,
                    Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
            };

            // Для кассира - другие быстрые действия
            if (CurrentUser.Role == "Кассир")
            {
                string[,] actions = {
                    { "💰", "Новая продажа", "Оформить продажу" },
                    { "👤", "Новый клиент", "Зарегистрировать" },
                    { "📦", "Поиск товара", "Найти товар" }
                };

                for (int i = 0; i < 3; i++)
                {
                    Button actionBtn = CreateActionButton(actions[i, 0], actions[i, 1], actions[i, 2], i);

                    // Добавляем обработчики
                    int index = i;
                    actionBtn.Click += (s, e) =>
                    {
                        if (index == 0) BtnSales_Click(s, e);
                        else if (index == 1) BtnCustomers_Click(s, e);
                        else if (index == 2) BtnProducts_Click(s, e);
                    };

                    actionsPanel.Controls.Add(actionBtn);
                }
            }
            else
            {
                string[,] actions = {
                    { "➕", "Новая продажа", "Оформить продажу" },
                    { "📦", "Добавить товар", "Пополнить склад" },
                    { "👤", "Новый клиент", "Зарегистрировать" },
                    { "📊", "Отчет", "Посмотреть отчет" }
                };

                for (int i = 0; i < 4; i++)
                {
                    Button actionBtn = CreateActionButton(actions[i, 0], actions[i, 1], actions[i, 2], i);

                    // Добавляем обработчики
                    int index = i;
                    actionBtn.Click += (s, e) =>
                    {
                        if (index == 0) BtnSales_Click(s, e);
                        else if (index == 1) BtnProducts_Click(s, e);
                        else if (index == 2) BtnCustomers_Click(s, e);
                        else if (index == 3 && CurrentUser.Role != "Кассир") BtnReports_Click(s, e);
                    };

                    actionsPanel.Controls.Add(actionBtn);
                }
            }

            dashboardPanel.Controls.AddRange(new Control[] {
                lblTitle, statsPanel, lblQuickActions, actionsPanel
            });

            contentPanel.Controls.Add(dashboardPanel);
        }

        private Panel CreateStatCard(string icon, string label, string value, int index)
        {
            Panel card = new Panel
            {
                Size = new Size(250, 130),
                Location = new Point(20 + (index * 265), 10),
                BackColor = Color.White
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 30),
                Location = new Point(15, 15),
                Size = new Size(50, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(80, 15),
                Size = new Size(150, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblLabel = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(80, 55),
                Size = new Size(150, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.AddRange(new Control[] { lblIcon, lblValue, lblLabel });
            return card;
        }

        private Button CreateActionButton(string icon, string title, string desc, int index)
        {
            int buttonWidth = CurrentUser.Role == "Кассир" ? 350 : 260;

            Button btn = new Button
            {
                Text = $"{icon}  {title}\n{desc}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20 + (index * (buttonWidth + 20)), 20),
                Size = new Size(buttonWidth, 160),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(249, 249, 249),
                ForeColor = Color.FromArgb(44, 62, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(240, 240, 240);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(249, 249, 249);
            return btn;
        }

        private string GetProductCount()
        {
            var result = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Products");
            return result?.ToString() ?? "0";
        }

        private string GetTodaySales()
        {
            var result = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Sales WHERE CAST(SaleDate AS DATE) = CAST(GETDATE() AS DATE)");
            return result?.ToString() ?? "0";
        }

        private string GetCustomerCount()
        {
            var result = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Customers");
            return result?.ToString() ?? "0";
        }

        private string GetRevenue()
        {
            var result = DatabaseHelper.ExecuteScalar(
                "SELECT ISNULL(SUM(TotalAmount), 0) FROM Sales WHERE CAST(SaleDate AS DATE) = CAST(GETDATE() AS DATE)");
            decimal revenue = Convert.ToDecimal(result);
            return revenue.ToString("C");
        }

        private void BtnDashboard_Click(object sender, EventArgs e) => ShowDashboard();

        private void BtnProducts_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Role == "Администратор")
            {
                AdminProductsForm productsForm = new AdminProductsForm();
                productsForm.ShowDialog();
            }
            else
            {
                CashierProductsForm productsForm = new CashierProductsForm();
                productsForm.ShowDialog();
            }
            ShowDashboard(); // Обновляем дашборд после закрытия формы
        }

        private void BtnSales_Click(object sender, EventArgs e)
        {
            SalesForm salesForm = new SalesForm();
            salesForm.ShowDialog();
            ShowDashboard(); // Обновляем дашборд после продажи
        }

        private void BtnCustomers_Click(object sender, EventArgs e)
        {
            CustomersForm customersForm = new CustomersForm();
            customersForm.ShowDialog();
            ShowDashboard(); // Обновляем дашборд
        }

        private void BtnSupplies_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Role == "Администратор")
            {
                SuppliesForm suppliesForm = new SuppliesForm();
                suppliesForm.ShowDialog();
                ShowDashboard(); // Обновляем дашборд после поставки
            }
            else
            {
                CustomMessageBox.Show("Доступ запрещен",
                    "У вас нет прав для доступа к этому разделу!",
                    MessageType.Warning);
            }
        }

        private void BtnReports_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Role == "Администратор")
            {
                ReportsForm reportsForm = new ReportsForm();
                reportsForm.ShowDialog();
            }
            else
            {
                CustomMessageBox.Show("Доступ запрещен",
                    "У вас нет прав для доступа к этому разделу!",
                    MessageType.Warning);
            }
        }

        private void BtnUsers_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Role == "Администратор")
            {
                UsersForm usersForm = new UsersForm();
                usersForm.ShowDialog();
            }
            else
            {
                CustomMessageBox.Show("Доступ запрещен",
                    "У вас нет прав для доступа к этому разделу!",
                    MessageType.Warning);
            }
        }

        private void BtnCategories_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Role == "Администратор")
            {
                CategoriesForm categoriesForm = new CategoriesForm();
                categoriesForm.ShowDialog();
                ShowDashboard(); // Обновляем дашборд
            }
            else
            {
                CustomMessageBox.Show("Доступ запрещен",
                    "У вас нет прав для доступа к этому разделу!",
                    MessageType.Warning);
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = CustomMessageBox.ShowConfirm("Выход",
                "Вы действительно хотите выйти из системы?");

            if (result == DialogResult.Yes)
            {
                CurrentUser.UserID = 0;
                CurrentUser.Username = null;
                CurrentUser.FullName = null;
                CurrentUser.Role = null;

                this.Hide();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
            }
        }
    }
}