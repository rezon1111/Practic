using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using ClosedXML.Excel;
using System.Diagnostics;

namespace MiniMarketPekarnia
{
    public partial class ReportsForm : Form
    {
        private TabControl tabControl;
        private Panel headerPanel;
        private Label lblTitle;
        private DataGridView dgvSalesReport;
        private DataGridView dgvProductsReport;
        private DataGridView dgvUsersReport;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnGenerateReport;
        private Button btnExportExcel;
        private Button btnExportProductsExcel;
        private Button btnExportUsersExcel;
        private Button btnPrint;
        private Label lblTotalSales;
        private Label lblTotalRevenue;
        private Label lblAvgCheck;
        private Label lblTopProduct;
        private Panel statsPanel;

        public ReportsForm()
        {
            // Проверка прав доступа
            if (CurrentUser.Role != "Администратор")
            {
                CustomMessageBox.Show("Доступ запрещен",
                    "У вас нет прав для доступа к этому разделу!",
                    MessageType.Error);
                this.Close();
                return;
            }

            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1300, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 245, 245);
            SetupForm();
            LoadDashboard();
        }

        private void SetupForm()
        {
            // Header
            headerPanel = new Panel
            {
                Size = new Size(1300, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(155, 89, 182)
            };

            lblTitle = new Label
            {
                Text = "📊  Отчеты и аналитика",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(400, 30)
            };

            Button btnCloseHeader = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(1250, 15),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCloseHeader.FlatAppearance.BorderSize = 0;
            btnCloseHeader.Click += (s, e) => this.Close();

            headerPanel.Controls.AddRange(new Control[] { lblTitle, btnCloseHeader });

            // Tab Control
            tabControl = new TabControl
            {
                Location = new Point(20, 80),
                Size = new Size(1260, 650),
                Font = new Font("Segoe UI", 11),
                Alignment = TabAlignment.Top
            };

            // Dashboard Tab
            TabPage dashboardTab = new TabPage("Главная панель");
            dashboardTab.BackColor = Color.FromArgb(245, 245, 245);
            SetupDashboardTab(dashboardTab);

            // Sales Report Tab
            TabPage salesTab = new TabPage("Отчет по продажам");
            salesTab.BackColor = Color.FromArgb(245, 245, 245);
            SetupSalesTab(salesTab);

            // Products Report Tab
            TabPage productsTab = new TabPage("Отчет по товарам");
            productsTab.BackColor = Color.FromArgb(245, 245, 245);
            SetupProductsTab(productsTab);

            // Users Report Tab
            TabPage usersTab = new TabPage("Отчет по пользователям");
            usersTab.BackColor = Color.FromArgb(245, 245, 245);
            SetupUsersTab(usersTab);

            tabControl.TabPages.Add(dashboardTab);
            tabControl.TabPages.Add(salesTab);
            tabControl.TabPages.Add(productsTab);
            tabControl.TabPages.Add(usersTab);

            this.Controls.AddRange(new Control[] { headerPanel, tabControl });
        }

        private void SetupDashboardTab(TabPage tab)
        {
            // Stats Panel
            statsPanel = new Panel
            {
                Size = new Size(1220, 150),
                Location = new Point(20, 20),
                BackColor = Color.White
            };

            // Today's stats
            AddStatCard(statsPanel, "💰 Выручка сегодня", GetTodayRevenue(), 0, Color.FromArgb(46, 204, 113));
            AddStatCard(statsPanel, "📦 Продаж сегодня", GetTodaySalesCount(), 1, Color.FromArgb(52, 152, 219));
            AddStatCard(statsPanel, "👥 Новых клиентов", GetNewCustomersCount(), 2, Color.FromArgb(155, 89, 182));
            AddStatCard(statsPanel, "📊 Средний чек", GetAvgCheck(), 3, Color.FromArgb(230, 126, 34));

            // Charts Panel
            Panel chartsPanel = new Panel
            {
                Size = new Size(1220, 200),
                Location = new Point(20, 190),
                BackColor = Color.White
            };

            Label lblChartTitle = new Label
            {
                Text = "Продажи по дням (последние 7 дней)",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 15),
                Size = new Size(400, 25)
            };

            // Simple chart using Panel with Paint event
            Panel chartPanel = new Panel
            {
                Size = new Size(1180, 140),
                Location = new Point(20, 50),
                BackColor = Color.FromArgb(249, 249, 249)
            };
            chartPanel.Paint += ChartPanel_Paint;

            chartsPanel.Controls.AddRange(new Control[] { lblChartTitle, chartPanel });

            // Top Products
            Panel topProductsPanel = new Panel
            {
                Size = new Size(1220, 150),
                Location = new Point(20, 410),
                BackColor = Color.White
            };

            Label lblTopProducts = new Label
            {
                Text = "Топ-5 товаров",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 15),
                Size = new Size(400, 25)
            };

            ListBox lbTopProducts = new ListBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 50),
                Size = new Size(1180, 80),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(249, 249, 249)
            };

            // Load top products
            DataTable topProducts = GetTopProducts();
            foreach (DataRow row in topProducts.Rows)
            {
                lbTopProducts.Items.Add($"{row["ProductName"]} - продано: {row["TotalSold"]} шт. на сумму {Convert.ToDecimal(row["TotalRevenue"]):C}");
            }

            topProductsPanel.Controls.AddRange(new Control[] { lblTopProducts, lbTopProducts });

            tab.Controls.AddRange(new Control[] { statsPanel, chartsPanel, topProductsPanel });
        }

        private void SetupSalesTab(TabPage tab)
        {
            // Date range panel
            Panel datePanel = new Panel
            {
                Size = new Size(1220, 60),
                Location = new Point(20, 20),
                BackColor = Color.White
            };

            Label lblStartDate = new Label
            {
                Text = "С:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 20),
                Size = new Size(30, 25)
            };

            dtpStartDate = new DateTimePicker
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(50, 17),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddMonths(-1)
            };

            Label lblEndDate = new Label
            {
                Text = "По:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(220, 20),
                Size = new Size(30, 25)
            };

            dtpEndDate = new DateTimePicker
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(250, 17),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            btnGenerateReport = new Button
            {
                Text = "📊 Сформировать отчет",
                Font = new Font("Segoe UI", 11),
                Location = new Point(420, 15),
                Size = new Size(180, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGenerateReport.FlatAppearance.BorderSize = 0;
            btnGenerateReport.Click += BtnGenerateSalesReport_Click;

            btnExportExcel = new Button
            {
                Text = "📥 Excel",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(610, 15),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportExcel.FlatAppearance.BorderSize = 0;
            btnExportExcel.Click += BtnExportSalesToExcel_Click;

            btnPrint = new Button
            {
                Text = "🖨️ Печать",
                Font = new Font("Segoe UI", 11),
                Location = new Point(720, 15),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Click += BtnPrint_Click;

            datePanel.Controls.AddRange(new Control[] {
                lblStartDate, dtpStartDate, lblEndDate, dtpEndDate,
                btnGenerateReport, btnExportExcel, btnPrint
            });

            // Summary panel
            Panel summaryPanel = new Panel
            {
                Size = new Size(1220, 80),
                Location = new Point(20, 90),
                BackColor = Color.White
            };

            lblTotalSales = new Label
            {
                Text = "Всего продаж: 0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 15),
                Size = new Size(200, 25)
            };

            lblTotalRevenue = new Label
            {
                Text = "Общая выручка: 0 ₽",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new Point(240, 15),
                Size = new Size(250, 25)
            };

            lblAvgCheck = new Label
            {
                Text = "Средний чек: 0 ₽",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(155, 89, 182),
                Location = new Point(510, 15),
                Size = new Size(200, 25)
            };

            lblTopProduct = new Label
            {
                Text = "Топ товар: -",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34),
                Location = new Point(730, 15),
                Size = new Size(400, 25)
            };

            summaryPanel.Controls.AddRange(new Control[] {
                lblTotalSales, lblTotalRevenue, lblAvgCheck, lblTopProduct
            });

            // Sales grid
            dgvSalesReport = new DataGridView
            {
                Location = new Point(20, 180),
                Size = new Size(1220, 410),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 10)
            };

            dgvSalesReport.EnableHeadersVisualStyles = false;
            dgvSalesReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvSalesReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSalesReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvSalesReport.ColumnHeadersHeight = 40;
            dgvSalesReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgvSalesReport.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvSalesReport.RowTemplate.Height = 35;
            dgvSalesReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);

            tab.Controls.AddRange(new Control[] { datePanel, summaryPanel, dgvSalesReport });
        }

        private void SetupProductsTab(TabPage tab)
        {
            Panel buttonPanel = new Panel
            {
                Size = new Size(1220, 50),
                Location = new Point(20, 20),
                BackColor = Color.White
            };

            btnExportProductsExcel = new Button
            {
                Text = "📥 Экспорт в Excel",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 10),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportProductsExcel.FlatAppearance.BorderSize = 0;
            btnExportProductsExcel.Click += BtnExportProductsToExcel_Click;

            buttonPanel.Controls.Add(btnExportProductsExcel);

            dgvProductsReport = new DataGridView
            {
                Location = new Point(20, 80),
                Size = new Size(1220, 510),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 10)
            };

            dgvProductsReport.EnableHeadersVisualStyles = false;
            dgvProductsReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvProductsReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProductsReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvProductsReport.ColumnHeadersHeight = 40;
            dgvProductsReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
            dgvProductsReport.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvProductsReport.RowTemplate.Height = 35;
            dgvProductsReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);

            LoadProductsReport();

            tab.Controls.AddRange(new Control[] { buttonPanel, dgvProductsReport });
        }

        private void SetupUsersTab(TabPage tab)
        {
            Panel buttonPanel = new Panel
            {
                Size = new Size(1220, 50),
                Location = new Point(20, 20),
                BackColor = Color.White
            };

            btnExportUsersExcel = new Button
            {
                Text = "📥 Экспорт в Excel",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 10),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportUsersExcel.FlatAppearance.BorderSize = 0;
            btnExportUsersExcel.Click += BtnExportUsersToExcel_Click;

            buttonPanel.Controls.Add(btnExportUsersExcel);

            dgvUsersReport = new DataGridView
            {
                Location = new Point(20, 80),
                Size = new Size(1220, 510),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 10)
            };

            dgvUsersReport.EnableHeadersVisualStyles = false;
            dgvUsersReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvUsersReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsersReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvUsersReport.ColumnHeadersHeight = 40;
            dgvUsersReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(155, 89, 182);
            dgvUsersReport.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvUsersReport.RowTemplate.Height = 35;
            dgvUsersReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);

            LoadUsersReport();

            tab.Controls.AddRange(new Control[] { buttonPanel, dgvUsersReport });
        }

        private void AddStatCard(Panel parent, string title, string value, int index, Color color)
        {
            Panel card = new Panel
            {
                Size = new Size(290, 120),
                Location = new Point(10 + (index * 300), 15),
                BackColor = color
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.White,
                Location = new Point(15, 20),
                Size = new Size(260, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 50),
                Size = new Size(260, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.AddRange(new Control[] { lblTitle, lblValue });
            parent.Controls.Add(card);
        }

        private void ChartPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DataTable sales = GetDailySales();
            if (sales.Rows.Count == 0) return;

            int width = 1180;
            int height = 140;
            int padding = 40;
            int barWidth = (width - 2 * padding) / sales.Rows.Count - 10;

            // Find max value
            decimal maxValue = 0;
            foreach (DataRow row in sales.Rows)
            {
                maxValue = Math.Max(maxValue, Convert.ToDecimal(row["Total"]));
            }

            if (maxValue == 0) maxValue = 1;

            // Draw bars
            for (int i = 0; i < sales.Rows.Count; i++)
            {
                DataRow row = sales.Rows[i];
                decimal value = Convert.ToDecimal(row["Total"]);
                int barHeight = (int)((value / maxValue) * (height - 60));

                int x = padding + i * (barWidth + 15);
                int y = height - 30 - barHeight;

                using (LinearGradientBrush brush = new LinearGradientBrush(
                    new Rectangle(x, y, barWidth, barHeight),
                    Color.FromArgb(52, 152, 219),
                    Color.FromArgb(41, 128, 185),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, x, y, barWidth, barHeight);
                }

                using (Pen pen = new Pen(Color.FromArgb(41, 128, 185), 1))
                {
                    g.DrawRectangle(pen, x, y, barWidth, barHeight);
                }

                // Draw day label
                string day = row["Day"].ToString();
                using (Font font = new Font("Segoe UI", 8))
                {
                    SizeF textSize = g.MeasureString(day, font);
                    g.DrawString(day, font, Brushes.Black,
                        x + (barWidth - textSize.Width) / 2, height - 20);
                }
            }
        }

        private void LoadDashboard()
        {
            // Stats are updated in AddStatCard calls
        }

        private string GetTodayRevenue()
        {
            var result = DatabaseHelper.ExecuteScalar(
                "SELECT ISNULL(SUM(TotalAmount), 0) FROM Sales WHERE CAST(SaleDate AS DATE) = CAST(GETDATE() AS DATE)");
            decimal revenue = Convert.ToDecimal(result);
            return revenue.ToString("C");
        }

        private string GetTodaySalesCount()
        {
            var result = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Sales WHERE CAST(SaleDate AS DATE) = CAST(GETDATE() AS DATE)");
            return result?.ToString() ?? "0";
        }

        private string GetNewCustomersCount()
        {
            // Проверяем, существует ли колонка CreatedDate
            string checkQuery = @"
                SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'Customers' AND COLUMN_NAME = 'CreatedDate'";

            int hasColumn = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery));

            if (hasColumn > 0)
            {
                var result = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Customers WHERE CAST(CreatedDate AS DATE) = CAST(GETDATE() AS DATE)");
                return result?.ToString() ?? "0";
            }
            else
            {
                // Если колонки нет, возвращаем общее количество
                var result = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Customers");
                return $"всего: {result}";
            }
        }

        private string GetAvgCheck()
        {
            var result = DatabaseHelper.ExecuteScalar(
                "SELECT ISNULL(AVG(TotalAmount), 0) FROM Sales WHERE CAST(SaleDate AS DATE) = CAST(GETDATE() AS DATE)");
            decimal avg = Convert.ToDecimal(result);
            return avg.ToString("C");
        }

        private DataTable GetDailySales()
        {
            string query = @"
                SELECT TOP 7 
                    FORMAT(SaleDate, 'dd.MM') AS Day,
                    ISNULL(SUM(TotalAmount), 0) AS Total
                FROM Sales
                WHERE SaleDate >= DATEADD(day, -7, GETDATE())
                GROUP BY FORMAT(SaleDate, 'dd.MM'), CAST(SaleDate AS DATE)
                ORDER BY CAST(SaleDate AS DATE)";

            return DatabaseHelper.ExecuteQuery(query);
        }

        private DataTable GetTopProducts()
        {
            string query = @"
                SELECT TOP 5 
                    p.ProductName,
                    ISNULL(SUM(sd.Quantity), 0) AS TotalSold,
                    ISNULL(SUM(sd.Quantity * sd.Price), 0) AS TotalRevenue
                FROM Products p
                LEFT JOIN SaleDetails sd ON p.ProductID = sd.ProductID
                LEFT JOIN Sales s ON sd.SaleID = s.SaleID AND s.SaleDate >= DATEADD(month, -1, GETDATE())
                GROUP BY p.ProductID, p.ProductName
                ORDER BY TotalSold DESC";

            return DatabaseHelper.ExecuteQuery(query);
        }

        private void BtnGenerateSalesReport_Click(object sender, EventArgs e)
        {
            string query = @"
                SELECT 
                    FORMAT(s.SaleDate, 'dd.MM.yyyy HH:mm') AS 'Дата и время',
                    u.FullName AS 'Кассир',
                    ISNULL(c.FullName, '—') AS 'Клиент',
                    COUNT(sd.SaleDetailID) AS 'Кол-во товаров',
                    s.TotalAmount AS 'Сумма',
                    s.PaymentMethod AS 'Оплата'
                FROM Sales s
                INNER JOIN Users u ON s.UserID = u.UserID
                LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                INNER JOIN SaleDetails sd ON s.SaleID = sd.SaleID
                WHERE CAST(s.SaleDate AS DATE) BETWEEN @start AND @end
                GROUP BY s.SaleDate, u.FullName, c.FullName, s.TotalAmount, s.PaymentMethod, s.SaleID
                ORDER BY s.SaleDate DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@start", dtpStartDate.Value.Date),
                new SqlParameter("@end", dtpEndDate.Value.Date)
            };

            DataTable report = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvSalesReport.DataSource = report;

            // Update summary
            string summaryQuery = @"
                SELECT 
                    COUNT(DISTINCT s.SaleID) AS TotalSales,
                    ISNULL(SUM(s.TotalAmount), 0) AS TotalRevenue,
                    ISNULL(AVG(s.TotalAmount), 0) AS AvgCheck
                FROM Sales s
                WHERE CAST(s.SaleDate AS DATE) BETWEEN @start AND @end";

            DataTable summary = DatabaseHelper.ExecuteQuery(summaryQuery, parameters);
            if (summary.Rows.Count > 0)
            {
                DataRow row = summary.Rows[0];
                lblTotalSales.Text = $"Всего продаж: {row["TotalSales"]}";
                lblTotalRevenue.Text = $"Общая выручка: {Convert.ToDecimal(row["TotalRevenue"]):C}";
                lblAvgCheck.Text = $"Средний чек: {Convert.ToDecimal(row["AvgCheck"]):C}";
            }

            // Get top product
            string topQuery = @"
                SELECT TOP 1 
                    p.ProductName,
                    SUM(sd.Quantity) AS TotalSold
                FROM SaleDetails sd
                INNER JOIN Sales s ON sd.SaleID = s.SaleID
                INNER JOIN Products p ON sd.ProductID = p.ProductID
                WHERE CAST(s.SaleDate AS DATE) BETWEEN @start AND @end
                GROUP BY p.ProductName
                ORDER BY TotalSold DESC";

            DataTable topProduct = DatabaseHelper.ExecuteQuery(topQuery, parameters);
            if (topProduct.Rows.Count > 0)
            {
                lblTopProduct.Text = $"Топ товар: {topProduct.Rows[0]["ProductName"]} ({topProduct.Rows[0]["TotalSold"]} шт.)";
            }
        }

        private void LoadProductsReport()
        {
            string query = @"
                SELECT 
                    p.ProductName AS 'Товар',
                    c.CategoryName AS 'Категория',
                    FORMAT(p.Price, 'C', 'ru-RU') AS 'Цена',
                    p.Quantity AS 'Остаток',
                    p.Unit AS 'Ед.изм.',
                    ISNULL(SUM(sd.Quantity), 0) AS 'Продано за месяц',
                    FORMAT(ISNULL(SUM(sd.Quantity * sd.Price), 0), 'C', 'ru-RU') AS 'Выручка за месяц'
                FROM Products p
                INNER JOIN Categories c ON p.CategoryID = c.CategoryID
                LEFT JOIN SaleDetails sd ON p.ProductID = sd.ProductID
                LEFT JOIN Sales s ON sd.SaleID = s.SaleID AND s.SaleDate >= DATEADD(month, -1, GETDATE())
                GROUP BY p.ProductID, p.ProductName, c.CategoryName, p.Price, p.Quantity, p.Unit
                ORDER BY p.ProductName";

            DataTable report = DatabaseHelper.ExecuteQuery(query);
            dgvProductsReport.DataSource = report;
        }

        private void LoadUsersReport()
        {
            string query = @"
                SELECT 
                    u.FullName AS 'Сотрудник',
                    r.RoleName AS 'Должность',
                    u.Username AS 'Логин',
                    FORMAT(u.CreatedDate, 'dd.MM.yyyy') AS 'Дата регистрации',
                    CASE WHEN u.IsActive = 1 THEN 'Активен' ELSE 'Неактивен' END AS 'Статус',
                    ISNULL(COUNT(s.SaleID), 0) AS 'Продаж',
                    FORMAT(ISNULL(SUM(s.TotalAmount), 0), 'C', 'ru-RU') AS 'Сумма продаж'
                FROM Users u
                INNER JOIN Roles r ON u.RoleID = r.RoleID
                LEFT JOIN Sales s ON u.UserID = s.UserID
                GROUP BY u.UserID, u.FullName, r.RoleName, u.Username, u.CreatedDate, u.IsActive
                ORDER BY SUM(s.TotalAmount) DESC";

            DataTable report = DatabaseHelper.ExecuteQuery(query);
            dgvUsersReport.DataSource = report;
        }

        // Экспорт отчета по продажам в Excel
        private void BtnExportSalesToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSalesReport.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Экспорт", "Нет данных для экспорта!", MessageType.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    FileName = $"Отчет_по_продажам_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Title = "Сохранить отчет в Excel"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Продажи");

                        // Заголовки
                        for (int i = 0; i < dgvSalesReport.Columns.Count; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = dgvSalesReport.Columns[i].HeaderText;
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 73, 94);
                            worksheet.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                        }

                        // Данные
                        for (int i = 0; i < dgvSalesReport.Rows.Count; i++)
                        {
                            for (int j = 0; j < dgvSalesReport.Columns.Count; j++)
                            {
                                worksheet.Cell(i + 2, j + 1).Value = dgvSalesReport.Rows[i].Cells[j].Value?.ToString() ?? "";
                            }
                        }

                        // Добавляем итоги
                        int lastRow = dgvSalesReport.Rows.Count + 3;
                        worksheet.Cell(lastRow, 4).Value = "ИТОГО:";
                        worksheet.Cell(lastRow, 4).Style.Font.Bold = true;
                        worksheet.Cell(lastRow, 5).Value = lblTotalRevenue.Text.Replace("Общая выручка: ", "");
                        worksheet.Cell(lastRow, 5).Style.Font.Bold = true;

                        // Форматирование
                        worksheet.Columns().AdjustToContents();

                        workbook.SaveAs(saveDialog.FileName);
                    }

                    CustomMessageBox.Show("Экспорт", "Отчет успешно сохранен!", MessageType.Success);

                    // Предложить открыть файл
                    if (CustomMessageBox.ShowConfirm("Открыть файл", "Открыть сохраненный файл?") == DialogResult.Yes)
                    {
                        Process.Start(saveDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Ошибка", "Ошибка при экспорте: " + ex.Message, MessageType.Error);
            }
        }

        // Экспорт отчета по товарам в Excel
        private void BtnExportProductsToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProductsReport.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Экспорт", "Нет данных для экспорта!", MessageType.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    FileName = $"Отчет_по_товарам_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Title = "Сохранить отчет в Excel"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Товары");

                        // Заголовки
                        for (int i = 0; i < dgvProductsReport.Columns.Count; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = dgvProductsReport.Columns[i].HeaderText;
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 73, 94);
                            worksheet.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                        }

                        // Данные
                        for (int i = 0; i < dgvProductsReport.Rows.Count; i++)
                        {
                            for (int j = 0; j < dgvProductsReport.Columns.Count; j++)
                            {
                                worksheet.Cell(i + 2, j + 1).Value = dgvProductsReport.Rows[i].Cells[j].Value?.ToString() ?? "";
                            }
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(saveDialog.FileName);
                    }

                    CustomMessageBox.Show("Экспорт", "Отчет успешно сохранен!", MessageType.Success);

                    if (CustomMessageBox.ShowConfirm("Открыть файл", "Открыть сохраненный файл?") == DialogResult.Yes)
                    {
                        Process.Start(saveDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Ошибка", "Ошибка при экспорте: " + ex.Message, MessageType.Error);
            }
        }

        // Экспорт отчета по пользователям в Excel
        private void BtnExportUsersToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsersReport.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Экспорт", "Нет данных для экспорта!", MessageType.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    FileName = $"Отчет_по_пользователям_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Title = "Сохранить отчет в Excel"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Пользователи");

                        // Заголовки
                        for (int i = 0; i < dgvUsersReport.Columns.Count; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = dgvUsersReport.Columns[i].HeaderText;
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 73, 94);
                            worksheet.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                        }

                        // Данные
                        for (int i = 0; i < dgvUsersReport.Rows.Count; i++)
                        {
                            for (int j = 0; j < dgvUsersReport.Columns.Count; j++)
                            {
                                worksheet.Cell(i + 2, j + 1).Value = dgvUsersReport.Rows[i].Cells[j].Value?.ToString() ?? "";
                            }
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(saveDialog.FileName);
                    }

                    CustomMessageBox.Show("Экспорт", "Отчет успешно сохранен!", MessageType.Success);

                    if (CustomMessageBox.ShowConfirm("Открыть файл", "Открыть сохраненный файл?") == DialogResult.Yes)
                    {
                        Process.Start(saveDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Ошибка", "Ошибка при экспорте: " + ex.Message, MessageType.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            CustomMessageBox.Show("Информация", "Функция печати будет доступна в следующей версии.", MessageType.Info);
        }
    }
}