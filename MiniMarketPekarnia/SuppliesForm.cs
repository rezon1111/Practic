using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MiniMarketPekarnia
{
    public partial class SuppliesForm : Form
    {
        private DataGridView dgvSupplies;
        private DataGridView dgvProducts;
        private ComboBox cmbProduct;
        private TextBox txtQuantity;
        private TextBox txtSupplier;
        private TextBox txtCost;
        private Button btnAddSupply;
        private Button btnRefresh;
        private Button btnClose;
        private Panel headerPanel;
        private Label lblTitle;
        private DateTimePicker dtpSupplyDate;

        public SuppliesForm()
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
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 245, 245);
            SetupForm();
            LoadSupplies();
            LoadProducts();
        }

        private void SetupForm()
        {
            // Header
            headerPanel = new Panel
            {
                Size = new Size(1200, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(230, 126, 34)
            };

            lblTitle = new Label
            {
                Text = "📥  Управление поставками",
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
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCloseHeader.FlatAppearance.BorderSize = 0;
            btnCloseHeader.Click += (s, e) => this.Close();

            headerPanel.Controls.AddRange(new Control[] { lblTitle, btnCloseHeader });

            // Left panel - Products
            Panel leftPanel = new Panel
            {
                Size = new Size(580, 600),
                Location = new Point(20, 80),
                BackColor = Color.White
            };

            Label lblProductsTitle = new Label
            {
                Text = "📋 Текущие товары",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 20),
                Size = new Size(300, 30)
            };

            // Products grid
            dgvProducts = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(540, 300),
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

            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvProducts.ColumnHeadersHeight = 40;
            dgvProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 126, 34);
            dgvProducts.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvProducts.RowTemplate.Height = 35;
            dgvProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);

            // Add supply panel
            Panel addPanel = new Panel
            {
                Size = new Size(540, 200),
                Location = new Point(20, 380),
                BackColor = Color.FromArgb(249, 249, 249)
            };

            Label lblAddTitle = new Label
            {
                Text = "➕ Добавить поставку",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 15),
                Size = new Size(500, 25)
            };

            Label lblProduct = new Label
            {
                Text = "Товар:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 50),
                Size = new Size(100, 20)
            };

            cmbProduct = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(120, 47),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblQuantity = new Label
            {
                Text = "Кол-во:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(330, 50),
                Size = new Size(60, 20)
            };

            txtQuantity = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(400, 47),
                Size = new Size(120, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblSupplier = new Label
            {
                Text = "Поставщик:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 90),
                Size = new Size(100, 20)
            };

            txtSupplier = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(120, 87),
                Size = new Size(200, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblCost = new Label
            {
                Text = "Цена закупки:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(330, 90),
                Size = new Size(100, 20)
            };

            txtCost = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(440, 87),
                Size = new Size(80, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblDate = new Label
            {
                Text = "Дата:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 130),
                Size = new Size(100, 20)
            };

            dtpSupplyDate = new DateTimePicker
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(120, 127),
                Size = new Size(200, 25),
                Format = DateTimePickerFormat.Short
            };

            btnAddSupply = new Button
            {
                Text = "✅ Добавить поставку",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(330, 125),
                Size = new Size(190, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddSupply.FlatAppearance.BorderSize = 0;
            btnAddSupply.Click += BtnAddSupply_Click;

            addPanel.Controls.AddRange(new Control[] {
                lblAddTitle, lblProduct, cmbProduct, lblQuantity, txtQuantity,
                lblSupplier, txtSupplier, lblCost, txtCost, lblDate, dtpSupplyDate, btnAddSupply
            });

            leftPanel.Controls.AddRange(new Control[] {
                lblProductsTitle, dgvProducts, addPanel
            });

            // Right panel - Supplies history
            Panel rightPanel = new Panel
            {
                Size = new Size(560, 600),
                Location = new Point(620, 80),
                BackColor = Color.White
            };

            Label lblHistoryTitle = new Label
            {
                Text = "📜 История поставок",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 20),
                Size = new Size(300, 30)
            };

            // Supplies grid
            dgvSupplies = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(520, 480),
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

            dgvSupplies.EnableHeadersVisualStyles = false;
            dgvSupplies.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvSupplies.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSupplies.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvSupplies.ColumnHeadersHeight = 40;
            dgvSupplies.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 126, 34);
            dgvSupplies.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvSupplies.RowTemplate.Height = 35;
            dgvSupplies.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);

            btnRefresh = new Button
            {
                Text = "🔄 Обновить",
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 550),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            rightPanel.Controls.AddRange(new Control[] {
                lblHistoryTitle, dgvSupplies, btnRefresh
            });

            this.Controls.AddRange(new Control[] { headerPanel, leftPanel, rightPanel });
        }

        private void LoadProducts()
        {
            string query = "SELECT ProductID, ProductName, Quantity, Unit FROM Products ORDER BY ProductName";
            DataTable products = DatabaseHelper.ExecuteQuery(query);

            cmbProduct.DisplayMember = "ProductName";
            cmbProduct.ValueMember = "ProductID";
            cmbProduct.DataSource = products;

            // Load products grid
            string gridQuery = @"SELECT ProductName AS 'Товар', 
                                        Quantity AS 'Количество',
                                        Unit AS 'Ед.'
                                 FROM Products 
                                 ORDER BY ProductName";
            dgvProducts.DataSource = DatabaseHelper.ExecuteQuery(gridQuery);
        }

        private void LoadSupplies()
        {
            string query = @"SELECT s.SupplyID,
                                    FORMAT(s.SupplyDate, 'dd.MM.yyyy') AS 'Дата',
                                    p.ProductName AS 'Товар',
                                    s.Quantity AS 'Количество',
                                    s.Supplier AS 'Поставщик',
                                    FORMAT(s.Cost, 'C', 'ru-RU') AS 'Цена закупки',
                                    u.FullName AS 'Принял'
                             FROM Supplies s
                             INNER JOIN Products p ON s.ProductID = p.ProductID
                             INNER JOIN Users u ON s.UserID = u.UserID
                             ORDER BY s.SupplyDate DESC";

            DataTable dataTable = DatabaseHelper.ExecuteQuery(query);
            dgvSupplies.DataSource = dataTable;

            if (dgvSupplies.Columns.Contains("SupplyID"))
                dgvSupplies.Columns["SupplyID"].Visible = false;
        }

        private void BtnAddSupply_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedValue == null)
            {
                CustomMessageBox.Show("Ошибка", "Выберите товар!", MessageType.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                CustomMessageBox.Show("Ошибка", "Введите количество!", MessageType.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                CustomMessageBox.Show("Ошибка", "Введите корректное количество!", MessageType.Warning);
                return;
            }

            decimal cost = 0;
            if (!string.IsNullOrEmpty(txtCost.Text))
            {
                if (!decimal.TryParse(txtCost.Text, out cost))
                {
                    CustomMessageBox.Show("Ошибка", "Введите корректную цену!", MessageType.Warning);
                    return;
                }
            }

            int productId = (int)cmbProduct.SelectedValue;
            string supplier = txtSupplier.Text.Trim();
            DateTime supplyDate = dtpSupplyDate.Value;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Add supply
                    string supplyQuery = @"INSERT INTO Supplies (SupplyDate, ProductID, Quantity, Supplier, Cost, UserID)
                                         VALUES (@date, @productId, @quantity, @supplier, @cost, @userId)";

                    SqlCommand supplyCmd = new SqlCommand(supplyQuery, conn, transaction);
                    supplyCmd.Parameters.AddWithValue("@date", supplyDate);
                    supplyCmd.Parameters.AddWithValue("@productId", productId);
                    supplyCmd.Parameters.AddWithValue("@quantity", quantity);
                    supplyCmd.Parameters.AddWithValue("@supplier", string.IsNullOrEmpty(supplier) ? DBNull.Value : (object)supplier);
                    supplyCmd.Parameters.AddWithValue("@cost", cost == 0 ? DBNull.Value : (object)cost);
                    supplyCmd.Parameters.AddWithValue("@userId", CurrentUser.UserID);
                    supplyCmd.ExecuteNonQuery();

                    // Update product quantity
                    string updateQuery = "UPDATE Products SET Quantity = Quantity + @quantity WHERE ProductID = @productId";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn, transaction);
                    updateCmd.Parameters.AddWithValue("@quantity", quantity);
                    updateCmd.Parameters.AddWithValue("@productId", productId);
                    updateCmd.ExecuteNonQuery();

                    transaction.Commit();

                    CustomMessageBox.Show("Успех", "Поставка успешно добавлена!", MessageType.Success);

                    // Clear form
                    txtQuantity.Clear();
                    txtSupplier.Clear();
                    txtCost.Clear();
                    dtpSupplyDate.Value = DateTime.Now;

                    // Refresh data
                    LoadProducts();
                    LoadSupplies();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    CustomMessageBox.Show("Ошибка", "Ошибка при добавлении поставки: " + ex.Message, MessageType.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadSupplies();
        }


       

    }


}