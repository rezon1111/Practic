using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MiniMarketPekarnia
{
    public partial class SalesForm : Form
    {
        private DataGridView dgvCart;
        private DataGridView dgvProducts;
        private ComboBox cmbProduct;
        private ComboBox cmbCustomer;
        private ComboBox cmbPayment;
        private NumericUpDown nudQuantity;
        private Label lblTotal;
        private Label lblAvailable;
        private Label lblPrice;
        private Button btnAddToCart;
        private Button btnRemoveFromCart;
        private Button btnClearCart;
        private Button btnCompleteSale;
        private Button btnClose;
        private Panel headerPanel;
        private Label lblTitle;
        private DataTable cartTable;
        private decimal totalAmount = 0;

        public SalesForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 245, 245);
            InitializeCart();
            SetupForm();
            LoadProducts();
            LoadCustomers();
        }

        private void InitializeCart()
        {
            cartTable = new DataTable();
            cartTable.Columns.Add("ProductID", typeof(int));
            cartTable.Columns.Add("ProductName", typeof(string));
            cartTable.Columns.Add("Price", typeof(decimal));
            cartTable.Columns.Add("Quantity", typeof(int));
            cartTable.Columns.Add("Total", typeof(decimal));
        }

        private void SetupForm()
        {
            // Header
            headerPanel = new Panel
            {
                Size = new Size(1400, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(46, 204, 113)
            };

            lblTitle = new Label
            {
                Text = "💰  Оформление продажи",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(400, 30)
            };

            Button btnCloseHeader = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(1350, 15),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCloseHeader.FlatAppearance.BorderSize = 0;
            btnCloseHeader.Click += (s, e) => this.Close();

            headerPanel.Controls.AddRange(new Control[] { lblTitle, btnCloseHeader });

            // Left panel - Products
            Panel leftPanel = new Panel
            {
                Size = new Size(680, 720),
                Location = new Point(20, 80),
                BackColor = Color.White
            };

            Label lblProductsTitle = new Label
            {
                Text = "📋 Доступные товары",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 20),
                Size = new Size(300, 30)
            };

            // Products grid
            dgvProducts = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(640, 400),
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

            // Style
            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvProducts.ColumnHeadersHeight = 40;
            dgvProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
            dgvProducts.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvProducts.RowTemplate.Height = 35;
            dgvProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);
            dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;

            // Product selection panel
            Panel productSelectPanel = new Panel
            {
                Size = new Size(640, 150),
                Location = new Point(20, 480),
                BackColor = Color.FromArgb(249, 249, 249)
            };

            Label lblSelectProduct = new Label
            {
                Text = "Выберите товар:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 15),
                Size = new Size(200, 20)
            };

            cmbProduct = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 40),
                Size = new Size(400, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;

            lblPrice = new Label
            {
                Text = "Цена: 0 ₽",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new Point(20, 75),
                Size = new Size(200, 20)
            };

            lblAvailable = new Label
            {
                Text = "В наличии: 0",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 100),
                Size = new Size(200, 20)
            };

            Label lblQuantity = new Label
            {
                Text = "Количество:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(250, 75),
                Size = new Size(80, 20)
            };

            nudQuantity = new NumericUpDown
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(340, 72),
                Size = new Size(80, 25),
                Minimum = 1,
                Maximum = 999,
                Value = 1
            };

            btnAddToCart = new Button
            {
                Text = "➕ Добавить в корзину",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(250, 100),
                Size = new Size(170, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddToCart.FlatAppearance.BorderSize = 0;
            btnAddToCart.Click += BtnAddToCart_Click;

            productSelectPanel.Controls.AddRange(new Control[] {
                lblSelectProduct, cmbProduct, lblPrice, lblAvailable,
                lblQuantity, nudQuantity, btnAddToCart
            });

            leftPanel.Controls.AddRange(new Control[] {
                lblProductsTitle, dgvProducts, productSelectPanel
            });

            // Right panel - Cart
            Panel rightPanel = new Panel
            {
                Size = new Size(660, 720),
                Location = new Point(720, 80),
                BackColor = Color.White
            };

            Label lblCartTitle = new Label
            {
                Text = "🛒 Корзина",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 20),
                Size = new Size(300, 30)
            };

            // Cart grid
            dgvCart = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(620, 400),
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

            dgvCart.EnableHeadersVisualStyles = false;
            dgvCart.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvCart.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCart.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvCart.ColumnHeadersHeight = 40;
            dgvCart.DefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 76, 60);
            dgvCart.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvCart.RowTemplate.Height = 35;
            dgvCart.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);

            // Cart buttons
            btnRemoveFromCart = new Button
            {
                Text = "🗑️ Удалить из корзины",
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 470),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRemoveFromCart.FlatAppearance.BorderSize = 0;
            btnRemoveFromCart.Click += BtnRemoveFromCart_Click;

            btnClearCart = new Button
            {
                Text = "🧹 Очистить корзину",
                Font = new Font("Segoe UI", 11),
                Location = new Point(230, 470),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClearCart.FlatAppearance.BorderSize = 0;
            btnClearCart.Click += BtnClearCart_Click;

            // Total
            lblTotal = new Label
            {
                Text = "ИТОГО: 0 ₽",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new Point(20, 530),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Customer and payment
            Panel customerPanel = new Panel
            {
                Size = new Size(620, 100),
                Location = new Point(20, 580),
                BackColor = Color.FromArgb(249, 249, 249)
            };

            Label lblCustomer = new Label
            {
                Text = "Клиент:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 15),
                Size = new Size(80, 20)
            };

            cmbCustomer = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(100, 12),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblPayment = new Label
            {
                Text = "Способ оплаты:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(320, 15),
                Size = new Size(120, 20)
            };

            cmbPayment = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(440, 12),
                Size = new Size(150, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPayment.Items.AddRange(new object[] { "Наличные", "Карта", "Смешанный" });
            cmbPayment.SelectedIndex = 0;

            btnCompleteSale = new Button
            {
                Text = "✅ Завершить продажу",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 55),
                Size = new Size(280, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCompleteSale.FlatAppearance.BorderSize = 0;
            btnCompleteSale.Click += BtnCompleteSale_Click;

            btnClose = new Button
            {
                Text = "✕ Закрыть",
                Font = new Font("Segoe UI", 12),
                Location = new Point(320, 55),
                Size = new Size(280, 35),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            customerPanel.Controls.AddRange(new Control[] {
                lblCustomer, cmbCustomer, lblPayment, cmbPayment,
                btnCompleteSale, btnClose
            });

            rightPanel.Controls.AddRange(new Control[] {
                lblCartTitle, dgvCart, btnRemoveFromCart, btnClearCart,
                lblTotal, customerPanel
            });

            this.Controls.AddRange(new Control[] { headerPanel, leftPanel, rightPanel });
        }

        private void LoadProducts()
        {
            string query = "SELECT ProductID, ProductName, Price, Quantity FROM Products WHERE Quantity > 0 ORDER BY ProductName";
            DataTable products = DatabaseHelper.ExecuteQuery(query);

            cmbProduct.DisplayMember = "ProductName";
            cmbProduct.ValueMember = "ProductID";
            cmbProduct.DataSource = products;

            // Load products grid
            string gridQuery = @"SELECT ProductName AS 'Товар', 
                                        Price AS 'Цена', 
                                        Quantity AS 'В наличии',
                                        Unit AS 'Ед.'
                                 FROM Products 
                                 WHERE Quantity > 0
                                 ORDER BY ProductName";
            dgvProducts.DataSource = DatabaseHelper.ExecuteQuery(gridQuery);
        }

        private void LoadCustomers()
        {
            string query = "SELECT CustomerID, FullName FROM Customers ORDER BY FullName";
            DataTable customers = DatabaseHelper.ExecuteQuery(query);

            DataRow emptyRow = customers.NewRow();
            emptyRow["CustomerID"] = 0;
            emptyRow["FullName"] = "--- Без клиента ---";
            customers.Rows.InsertAt(emptyRow, 0);

            cmbCustomer.DisplayMember = "FullName";
            cmbCustomer.ValueMember = "CustomerID";
            cmbCustomer.DataSource = customers;
        }

        private void DgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow != null)
            {
                string productName = dgvProducts.CurrentRow.Cells["Товар"].Value.ToString();
                for (int i = 0; i < cmbProduct.Items.Count; i++)
                {
                    DataRowView item = cmbProduct.Items[i] as DataRowView;
                    if (item != null && item["ProductName"].ToString() == productName)
                    {
                        cmbProduct.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void CmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedValue != null && cmbProduct.SelectedValue is int)
            {
                DataRowView row = cmbProduct.SelectedItem as DataRowView;
                if (row != null)
                {
                    decimal price = Convert.ToDecimal(row["Price"]);
                    int available = Convert.ToInt32(row["Quantity"]);

                    lblPrice.Text = $"Цена: {price:C}";
                    lblAvailable.Text = $"В наличии: {available}";
                    nudQuantity.Maximum = available;
                }
            }
        }

        private void BtnAddToCart_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedValue == null || !(cmbProduct.SelectedValue is int))
            {
                CustomMessageBox.Show("Ошибка", "Выберите товар!", MessageType.Warning);
                return;
            }

            int productId = (int)cmbProduct.SelectedValue;
            string productName = cmbProduct.Text;
            decimal price = 0;
            int quantity = (int)nudQuantity.Value;

            if (quantity <= 0)
            {
                CustomMessageBox.Show("Ошибка", "Введите количество!", MessageType.Warning);
                return;
            }

            DataRowView row = cmbProduct.SelectedItem as DataRowView;
            if (row != null)
            {
                price = Convert.ToDecimal(row["Price"]);
            }

            // Check if product already in cart
            DataRow[] existingRows = cartTable.Select($"ProductID = {productId}");
            if (existingRows.Length > 0)
            {
                DataRow existingRow = existingRows[0];
                int newQuantity = (int)existingRow["Quantity"] + quantity;

                // Check available quantity
                int available = Convert.ToInt32(row["Quantity"]);
                if (newQuantity > available)
                {
                    CustomMessageBox.Show("Ошибка", $"Недостаточно товара! Доступно: {available}", MessageType.Warning);
                    return;
                }

                existingRow["Quantity"] = newQuantity;
                existingRow["Total"] = newQuantity * price;
            }
            else
            {
                DataRow newRow = cartTable.NewRow();
                newRow["ProductID"] = productId;
                newRow["ProductName"] = productName;
                newRow["Price"] = price;
                newRow["Quantity"] = quantity;
                newRow["Total"] = quantity * price;
                cartTable.Rows.Add(newRow);
            }

            UpdateCartDisplay();
            UpdateTotal();
            nudQuantity.Value = 1;
        }

        private void BtnRemoveFromCart_Click(object sender, EventArgs e)
        {
            if (dgvCart.CurrentRow != null)
            {
                int rowIndex = dgvCart.CurrentRow.Index;
                cartTable.Rows[rowIndex].Delete();
                UpdateCartDisplay();
                UpdateTotal();
            }
        }

        private void BtnClearCart_Click(object sender, EventArgs e)
        {
            cartTable.Clear();
            UpdateCartDisplay();
            UpdateTotal();
        }

        private void UpdateCartDisplay()
        {
            dgvCart.DataSource = null;
            dgvCart.DataSource = cartTable;

            if (dgvCart.Columns.Contains("ProductID"))
                dgvCart.Columns["ProductID"].Visible = false;
        }

        private void UpdateTotal()
        {
            totalAmount = 0;
            foreach (DataRow row in cartTable.Rows)
            {
                totalAmount += Convert.ToDecimal(row["Total"]);
            }
            lblTotal.Text = $"ИТОГО: {totalAmount:C}";
        }

        private void BtnCompleteSale_Click(object sender, EventArgs e)
        {
            if (cartTable.Rows.Count == 0)
            {
                CustomMessageBox.Show("Ошибка", "Корзина пуста!", MessageType.Warning);
                return;
            }

            DialogResult result = CustomMessageBox.ShowConfirm("Подтверждение",
                $"Завершить продажу на сумму {totalAmount:C}?");

            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Create sale
                        string saleQuery = @"INSERT INTO Sales (UserID, CustomerID, TotalAmount, PaymentMethod) 
                                           VALUES (@userID, @customerID, @total, @payment);
                                           SELECT SCOPE_IDENTITY();";

                        int customerId = 0;
                        if (cmbCustomer.SelectedValue != null && cmbCustomer.SelectedValue is int)
                            customerId = (int)cmbCustomer.SelectedValue;

                        SqlCommand saleCmd = new SqlCommand(saleQuery, conn, transaction);
                        saleCmd.Parameters.AddWithValue("@userID", CurrentUser.UserID);
                        saleCmd.Parameters.AddWithValue("@customerID", customerId == 0 ? DBNull.Value : (object)customerId);
                        saleCmd.Parameters.AddWithValue("@total", totalAmount);
                        saleCmd.Parameters.AddWithValue("@payment", cmbPayment.Text);

                        int saleId = Convert.ToInt32(saleCmd.ExecuteScalar());

                        // Add sale details and update products
                        foreach (DataRow row in cartTable.Rows)
                        {
                            int productId = Convert.ToInt32(row["ProductID"]);
                            int quantity = Convert.ToInt32(row["Quantity"]);
                            decimal price = Convert.ToDecimal(row["Price"]);

                            // Sale details
                            string detailQuery = @"INSERT INTO SaleDetails (SaleID, ProductID, Quantity, Price) 
                                                 VALUES (@saleID, @productID, @quantity, @price)";
                            SqlCommand detailCmd = new SqlCommand(detailQuery, conn, transaction);
                            detailCmd.Parameters.AddWithValue("@saleID", saleId);
                            detailCmd.Parameters.AddWithValue("@productID", productId);
                            detailCmd.Parameters.AddWithValue("@quantity", quantity);
                            detailCmd.Parameters.AddWithValue("@price", price);
                            detailCmd.ExecuteNonQuery();

                            // Update product quantity
                            string updateProductQuery = "UPDATE Products SET Quantity = Quantity - @quantity WHERE ProductID = @productID";
                            SqlCommand updateCmd = new SqlCommand(updateProductQuery, conn, transaction);
                            updateCmd.Parameters.AddWithValue("@quantity", quantity);
                            updateCmd.Parameters.AddWithValue("@productID", productId);
                            updateCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        CustomMessageBox.Show("Успех", "Продажа успешно завершена!", MessageType.Success);

                        // Clear cart and refresh
                        cartTable.Clear();
                        UpdateCartDisplay();
                        UpdateTotal();
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        CustomMessageBox.Show("Ошибка", "Ошибка при оформлении продажи: " + ex.Message, MessageType.Error);
                    }
                }
            }
        }
    }
}