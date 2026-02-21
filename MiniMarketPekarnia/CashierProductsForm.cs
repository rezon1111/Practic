using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MiniMarketPekarnia
{
    public partial class CashierProductsForm : Form
    {
        private DataGridView dgvProducts;
        private TextBox txtSearch;
        private Button btnRefresh;
        private Button btnClose;
        private Panel headerPanel;
        private Label lblTitle;

        public CashierProductsForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 245, 245);
            SetupForm();
            LoadProducts();
        }

        private void SetupForm()
        {
            // Header panel
            headerPanel = new Panel
            {
                Size = new Size(1200, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(52, 152, 219)
            };

            lblTitle = new Label
            {
                Text = "📦  Просмотр товаров",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(400, 30)
            };

            Button btnCloseHeader = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(1150, 15),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCloseHeader.FlatAppearance.BorderSize = 0;
            btnCloseHeader.Click += (s, e) => this.Close();

            headerPanel.Controls.AddRange(new Control[] { lblTitle, btnCloseHeader });

            // Search panel
            Panel searchPanel = new Panel
            {
                Size = new Size(1160, 80),
                Location = new Point(20, 80),
                BackColor = Color.White
            };

            Label lblSearch = new Label
            {
                Text = "Поиск товаров:",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 15),
                Size = new Size(150, 25)
            };

            txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 45),
                Size = new Size(400, 30),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            btnRefresh = new Button
            {
                Text = "🔄 Обновить",
                Font = new Font("Segoe UI", 12),
                Location = new Point(450, 42),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            btnClose = new Button
            {
                Text = "✕ Закрыть",
                Font = new Font("Segoe UI", 12),
                Location = new Point(610, 42),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            searchPanel.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnRefresh, btnClose
            });

            // DataGridView
            dgvProducts = new DataGridView
            {
                Location = new Point(20, 180),
                Size = new Size(1160, 460),
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

            // Style DataGridView
            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvProducts.ColumnHeadersHeight = 40;
            dgvProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgvProducts.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvProducts.RowTemplate.Height = 35;
            dgvProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);

            this.Controls.AddRange(new Control[] { headerPanel, searchPanel, dgvProducts });
        }

        private void LoadProducts()
        {
            string query = @"SELECT p.ProductID, 
                                    p.ProductName AS 'Название товара', 
                                    c.CategoryName AS 'Категория', 
                                    FORMAT(p.Price, 'C', 'ru-RU') AS 'Цена', 
                                    p.Quantity AS 'Количество', 
                                    p.Unit AS 'Единица',
                                    p.Description AS 'Описание'
                             FROM Products p
                             INNER JOIN Categories c ON p.CategoryID = c.CategoryID
                             ORDER BY p.ProductName";

            DataTable dataTable = DatabaseHelper.ExecuteQuery(query);
            dgvProducts.DataSource = dataTable;

            if (dgvProducts.Columns.Contains("ProductID"))
                dgvProducts.Columns["ProductID"].Visible = false;

            // Format columns
            foreach (DataGridViewColumn col in dgvProducts.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadProducts();
            }
            else
            {
                string query = @"SELECT p.ProductID, 
                                        p.ProductName AS 'Название товара', 
                                        c.CategoryName AS 'Категория', 
                                        FORMAT(p.Price, 'C', 'ru-RU') AS 'Цена', 
                                        p.Quantity AS 'Количество', 
                                        p.Unit AS 'Единица',
                                        p.Description AS 'Описание'
                                 FROM Products p
                                 INNER JOIN Categories c ON p.CategoryID = c.CategoryID
                                 WHERE p.ProductName LIKE @search 
                                    OR c.CategoryName LIKE @search
                                    OR p.Description LIKE @search
                                 ORDER BY p.ProductName";

                var parameters = new[] {
                    new System.Data.SqlClient.SqlParameter("@search", $"%{searchText}%")
                };

                DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvProducts.DataSource = dataTable;
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadProducts();
            txtSearch.Clear();
        }
    }
}