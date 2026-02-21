using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MiniMarketPekarnia
{
    public partial class AdminProductsForm : Form
    {
        private DataGridView dgvProducts;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnClose;
        private Panel headerPanel;
        private Label lblTitle;
        private ComboBox cmbCategory;
        private TextBox txtName;
        private TextBox txtPrice;
        private TextBox txtQuantity;
        private TextBox txtUnit;
        private TextBox txtDescription;
        private Panel editPanel;
        private bool isEditMode = false;
        private int currentProductId = 0;

        public AdminProductsForm()
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
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 245, 245);
            SetupForm();
            LoadProducts();
            LoadCategories();
        }

        private void SetupForm()
        {
            // Header panel
            headerPanel = new Panel
            {
                Size = new Size(1400, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(41, 128, 185)
            };

            lblTitle = new Label
            {
                Text = "📦  Управление товарами (Администратор)",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(600, 30)
            };

            Button btnCloseHeader = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(1350, 15),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCloseHeader.FlatAppearance.BorderSize = 0;
            btnCloseHeader.Click += (s, e) => this.Close();

            headerPanel.Controls.AddRange(new Control[] { lblTitle, btnCloseHeader });

            // Left panel - Products list
            Panel leftPanel = new Panel
            {
                Size = new Size(900, 720),
                Location = new Point(20, 80),
                BackColor = Color.White
            };

            // Search
            Label lblSearch = new Label
            {
                Text = "Поиск:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 20),
                Size = new Size(60, 20)
            };

            txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(90, 17),
                Size = new Size(350, 25),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            btnRefresh = new Button
            {
                Text = "🔄",
                Font = new Font("Segoe UI", 12),
                Location = new Point(450, 15),
                Size = new Size(40, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            // DataGridView
            dgvProducts = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(860, 500),
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
            dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;

            // Buttons
            btnAdd = new Button
            {
                Text = "➕ Добавить",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 580),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "✏️ Редактировать",
                Font = new Font("Segoe UI", 12),
                Location = new Point(170, 580),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button
            {
                Text = "🗑️ Удалить",
                Font = new Font("Segoe UI", 12),
                Location = new Point(320, 580),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            leftPanel.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnRefresh, dgvProducts,
                btnAdd, btnEdit, btnDelete
            });

            // Right panel - Edit form
            editPanel = new Panel
            {
                Size = new Size(440, 720),
                Location = new Point(940, 80),
                BackColor = Color.White
            };

            Label lblEditTitle = new Label
            {
                Text = "📝 Редактирование товара",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 20),
                Size = new Size(400, 30)
            };

            // Category
            Label lblCategory = new Label
            {
                Text = "Категория:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 70),
                Size = new Size(400, 20)
            };

            cmbCategory = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 95),
                Size = new Size(400, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Name
            Label lblName = new Label
            {
                Text = "Название товара:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 135),
                Size = new Size(400, 20)
            };

            txtName = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 160),
                Size = new Size(400, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Price
            Label lblPrice = new Label
            {
                Text = "Цена:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 200),
                Size = new Size(400, 20)
            };

            txtPrice = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 225),
                Size = new Size(400, 25),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtPrice.KeyPress += TxtPrice_KeyPress;

            // Quantity
            Label lblQuantity = new Label
            {
                Text = "Количество:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 265),
                Size = new Size(400, 20)
            };

            txtQuantity = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 290),
                Size = new Size(400, 25),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtQuantity.KeyPress += TxtQuantity_KeyPress;

            // Unit
            Label lblUnit = new Label
            {
                Text = "Единица измерения:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 330),
                Size = new Size(400, 20)
            };

            txtUnit = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 355),
                Size = new Size(400, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Description
            Label lblDescription = new Label
            {
                Text = "Описание:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 395),
                Size = new Size(400, 20)
            };

            txtDescription = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 420),
                Size = new Size(400, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Save button
            Button btnSave = new Button
            {
                Text = "💾 Сохранить",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 480),
                Size = new Size(190, 45),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            // Cancel button
            Button btnCancel = new Button
            {
                Text = "❌ Отмена",
                Font = new Font("Segoe UI", 12),
                Location = new Point(230, 480),
                Size = new Size(190, 45),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            editPanel.Controls.AddRange(new Control[] {
                lblEditTitle, lblCategory, cmbCategory, lblName, txtName,
                lblPrice, txtPrice, lblQuantity, txtQuantity, lblUnit, txtUnit,
                lblDescription, txtDescription, btnSave, btnCancel
            });

            this.Controls.AddRange(new Control[] { headerPanel, leftPanel, editPanel });
            ClearEditForm();
        }

        private void LoadProducts()
        {
            string query = @"SELECT p.ProductID, 
                                    p.ProductName AS 'Название товара', 
                                    c.CategoryName AS 'Категория', 
                                    p.Price AS 'Цена', 
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
        }

        private void LoadCategories()
        {
            string query = "SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName";
            DataTable categories = DatabaseHelper.ExecuteQuery(query);

            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryID";
            cmbCategory.DataSource = categories;
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
                                        p.Price AS 'Цена', 
                                        p.Quantity AS 'Количество', 
                                        p.Unit AS 'Единица',
                                        p.Description AS 'Описание'
                                 FROM Products p
                                 INNER JOIN Categories c ON p.CategoryID = c.CategoryID
                                 WHERE p.ProductName LIKE @search 
                                    OR c.CategoryName LIKE @search
                                 ORDER BY p.ProductName";

                var parameters = new[] {
                    new SqlParameter("@search", $"%{searchText}%")
                };

                DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvProducts.DataSource = dataTable;
            }
        }

        private void DgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow != null)
            {
                DataGridViewRow row = dgvProducts.CurrentRow;
                currentProductId = Convert.ToInt32(row.Cells["ProductID"].Value);
                txtName.Text = row.Cells["Название товара"].Value.ToString();
                txtPrice.Text = row.Cells["Цена"].Value.ToString();
                txtQuantity.Text = row.Cells["Количество"].Value.ToString();
                txtUnit.Text = row.Cells["Единица"].Value.ToString();
                txtDescription.Text = row.Cells["Описание"]?.Value?.ToString() ?? "";

                // Select category
                string categoryName = row.Cells["Категория"].Value.ToString();
                for (int i = 0; i < cmbCategory.Items.Count; i++)
                {
                    DataRowView item = cmbCategory.Items[i] as DataRowView;
                    if (item != null && item["CategoryName"].ToString() == categoryName)
                    {
                        cmbCategory.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            isEditMode = false;
            currentProductId = 0;
            ClearEditForm();
            txtName.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null)
            {
                CustomMessageBox.Show("Внимание", "Выберите товар для редактирования!", MessageType.Warning);
                return;
            }
            isEditMode = true;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null)
            {
                CustomMessageBox.Show("Внимание", "Выберите товар для удаления!", MessageType.Warning);
                return;
            }

            DialogResult result = CustomMessageBox.ShowConfirm("Подтверждение",
                $"Вы уверены, что хотите удалить товар \"{txtName.Text}\"?");

            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM Products WHERE ProductID = @id";
                SqlParameter[] parameters = { new SqlParameter("@id", currentProductId) };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Товар успешно удален!", MessageType.Success);
                    LoadProducts();
                    ClearEditForm();
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            string name = txtName.Text.Trim();
            int categoryId = (int)cmbCategory.SelectedValue;
            decimal price = decimal.Parse(txtPrice.Text);
            int quantity = int.Parse(txtQuantity.Text);
            string unit = txtUnit.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (isEditMode && currentProductId > 0)
            {
                // Update
                string query = @"UPDATE Products 
                                SET ProductName = @name, CategoryID = @catId, Price = @price, 
                                    Quantity = @quantity, Unit = @unit, Description = @desc
                                WHERE ProductID = @id";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", name),
                    new SqlParameter("@catId", categoryId),
                    new SqlParameter("@price", price),
                    new SqlParameter("@quantity", quantity),
                    new SqlParameter("@unit", unit),
                    new SqlParameter("@desc", description),
                    new SqlParameter("@id", currentProductId)
                };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Товар успешно обновлен!", MessageType.Success);
                }
            }
            else
            {
                // Insert
                string query = @"INSERT INTO Products (ProductName, CategoryID, Price, Quantity, Unit, Description)
                                VALUES (@name, @catId, @price, @quantity, @unit, @desc)";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", name),
                    new SqlParameter("@catId", categoryId),
                    new SqlParameter("@price", price),
                    new SqlParameter("@quantity", quantity),
                    new SqlParameter("@unit", unit),
                    new SqlParameter("@desc", description)
                };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Товар успешно добавлен!", MessageType.Success);
                }
            }

            LoadProducts();
            ClearEditForm();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ClearEditForm();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadProducts();
            txtSearch.Clear();
        }

        private void ClearEditForm()
        {
            txtName.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();
            txtUnit.Clear();
            txtDescription.Clear();
            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
            currentProductId = 0;
            isEditMode = false;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                CustomMessageBox.Show("Ошибка", "Введите название товара!", MessageType.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(txtPrice.Text))
            {
                CustomMessageBox.Show("Ошибка", "Введите цену!", MessageType.Warning);
                return false;
            }
            if (!decimal.TryParse(txtPrice.Text, out _))
            {
                CustomMessageBox.Show("Ошибка", "Цена должна быть числом!", MessageType.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                CustomMessageBox.Show("Ошибка", "Введите количество!", MessageType.Warning);
                return false;
            }
            if (!int.TryParse(txtQuantity.Text, out _))
            {
                CustomMessageBox.Show("Ошибка", "Количество должно быть целым числом!", MessageType.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(txtUnit.Text))
            {
                CustomMessageBox.Show("Ошибка", "Введите единицу измерения!", MessageType.Warning);
                return false;
            }
            return true;
        }

        private void TxtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            if (e.KeyChar == ',' && (sender as TextBox).Text.Contains(","))
            {
                e.Handled = true;
            }
        }

        private void TxtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void AdminProductsForm_Load(object sender, EventArgs e)
        {

        }

       
    }


}