using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MiniMarketPekarnia
{
    public partial class CategoriesForm : Form
    {
        private DataGridView dgvCategories;
        private TextBox txtSearch;
        private TextBox txtCategoryName;
        private TextBox txtDescription;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnSave;
        private Button btnCancel;
        private Button btnRefresh;
        private Panel headerPanel;
        private Label lblTitle;
        private Panel editPanel;
        private bool isEditMode = false;
        private int currentCategoryId = 0;

        public CategoriesForm()
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
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 245, 245);
            SetupForm();
            LoadCategories();
        }

        private void SetupForm()
        {
            // Header
            headerPanel = new Panel
            {
                Size = new Size(1000, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(241, 196, 15)
            };

            lblTitle = new Label
            {
                Text = "🏷️  Управление категориями",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(400, 30)
            };

            Button btnCloseHeader = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(950, 15),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCloseHeader.FlatAppearance.BorderSize = 0;
            btnCloseHeader.Click += (s, e) => this.Close();

            headerPanel.Controls.AddRange(new Control[] { lblTitle, btnCloseHeader });

            // Search panel
            Panel searchPanel = new Panel
            {
                Size = new Size(960, 70),
                Location = new Point(20, 80),
                BackColor = Color.White
            };

            Label lblSearch = new Label
            {
                Text = "Поиск категорий:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 15),
                Size = new Size(120, 20)
            };

            txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 40),
                Size = new Size(300, 25),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            btnRefresh = new Button
            {
                Text = "🔄 Обновить",
                Font = new Font("Segoe UI", 11),
                Location = new Point(340, 37),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            searchPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnRefresh });

            // Left panel - Categories list
            Panel leftPanel = new Panel
            {
                Size = new Size(560, 430),
                Location = new Point(20, 160),
                BackColor = Color.White
            };

            // Categories grid
            dgvCategories = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(520, 320),
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

            dgvCategories.EnableHeadersVisualStyles = false;
            dgvCategories.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvCategories.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCategories.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvCategories.ColumnHeadersHeight = 40;
            dgvCategories.DefaultCellStyle.SelectionBackColor = Color.FromArgb(241, 196, 15);
            dgvCategories.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvCategories.RowTemplate.Height = 35;
            dgvCategories.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);
            dgvCategories.SelectionChanged += DgvCategories_SelectionChanged;

            // Buttons
            btnAdd = new Button
            {
                Text = "➕ Добавить",
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 360),
                Size = new Size(100, 40),
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
                Font = new Font("Segoe UI", 11),
                Location = new Point(130, 360),
                Size = new Size(120, 40),
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
                Font = new Font("Segoe UI", 11),
                Location = new Point(260, 360),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            leftPanel.Controls.AddRange(new Control[] {
                dgvCategories, btnAdd, btnEdit, btnDelete
            });

            // Right panel - Edit form
            editPanel = new Panel
            {
                Size = new Size(380, 430),
                Location = new Point(600, 160),
                BackColor = Color.White
            };

            Label lblEditTitle = new Label
            {
                Text = "📝 Редактирование категории",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 20),
                Size = new Size(340, 30)
            };

            // Category Name
            Label lblCategoryName = new Label
            {
                Text = "Название категории:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 70),
                Size = new Size(340, 20)
            };

            txtCategoryName = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 95),
                Size = new Size(340, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Description
            Label lblDescription = new Label
            {
                Text = "Описание:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 140),
                Size = new Size(340, 20)
            };

            txtDescription = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 165),
                Size = new Size(340, 60),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true
            };

            // Save button
            btnSave = new Button
            {
                Text = "💾 Сохранить",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 250),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            // Cancel button
            btnCancel = new Button
            {
                Text = "❌ Отмена",
                Font = new Font("Segoe UI", 11),
                Location = new Point(200, 250),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            editPanel.Controls.AddRange(new Control[] {
                lblEditTitle, lblCategoryName, txtCategoryName,
                lblDescription, txtDescription, btnSave, btnCancel
            });

            this.Controls.AddRange(new Control[] { headerPanel, searchPanel, leftPanel, editPanel });
            ClearEditForm();
        }

        private void LoadCategories()
        {
            string query = @"SELECT CategoryID, 
                                    CategoryName AS 'Название категории', 
                                    Description AS 'Описание'
                             FROM Categories
                             ORDER BY CategoryName";

            DataTable dataTable = DatabaseHelper.ExecuteQuery(query);
            dgvCategories.DataSource = dataTable;

            if (dgvCategories.Columns.Contains("CategoryID"))
                dgvCategories.Columns["CategoryID"].Visible = false;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadCategories();
            }
            else
            {
                string query = @"SELECT CategoryID, 
                                        CategoryName AS 'Название категории', 
                                        Description AS 'Описание'
                                 FROM Categories
                                 WHERE CategoryName LIKE @search OR Description LIKE @search
                                 ORDER BY CategoryName";

                var parameters = new[] {
                    new SqlParameter("@search", $"%{searchText}%")
                };

                DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvCategories.DataSource = dataTable;
            }
        }

        private void DgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow != null && !isEditMode)
            {
                DataGridViewRow row = dgvCategories.CurrentRow;
                currentCategoryId = Convert.ToInt32(row.Cells["CategoryID"].Value);
                txtCategoryName.Text = row.Cells["Название категории"].Value.ToString();
                txtDescription.Text = row.Cells["Описание"]?.Value?.ToString() ?? "";
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            isEditMode = false;
            currentCategoryId = 0;
            ClearEditForm();
            txtCategoryName.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow == null)
            {
                CustomMessageBox.Show("Внимание", "Выберите категорию для редактирования!", MessageType.Warning);
                return;
            }
            isEditMode = true;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow == null)
            {
                CustomMessageBox.Show("Внимание", "Выберите категорию для удаления!", MessageType.Warning);
                return;
            }

            // Check if category has products
            string checkQuery = "SELECT COUNT(*) FROM Products WHERE CategoryID = @id";
            SqlParameter[] checkParams = { new SqlParameter("@id", currentCategoryId) };
            int productCount = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParams));

            if (productCount > 0)
            {
                CustomMessageBox.Show("Ошибка",
                    $"Нельзя удалить категорию, так как она содержит {productCount} товаров!",
                    MessageType.Error);
                return;
            }

            DialogResult result = CustomMessageBox.ShowConfirm("Подтверждение",
                $"Вы уверены, что хотите удалить категорию \"{txtCategoryName.Text}\"?");

            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM Categories WHERE CategoryID = @id";
                SqlParameter[] parameters = { new SqlParameter("@id", currentCategoryId) };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Категория успешно удалена!", MessageType.Success);
                    LoadCategories();
                    ClearEditForm();
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCategoryName.Text))
            {
                CustomMessageBox.Show("Ошибка", "Введите название категории!", MessageType.Warning);
                return;
            }

            string categoryName = txtCategoryName.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (isEditMode && currentCategoryId > 0)
            {
                // Update
                string query = @"UPDATE Categories 
                                SET CategoryName = @name, Description = @desc
                                WHERE CategoryID = @id";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", categoryName),
                    new SqlParameter("@desc", string.IsNullOrEmpty(description) ? DBNull.Value : (object)description),
                    new SqlParameter("@id", currentCategoryId)
                };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Категория обновлена!", MessageType.Success);
                }
            }
            else
            {
                // Check if category already exists
                string checkQuery = "SELECT COUNT(*) FROM Categories WHERE CategoryName = @name";
                SqlParameter[] checkParams = { new SqlParameter("@name", categoryName) };
                int exists = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParams));

                if (exists > 0)
                {
                    CustomMessageBox.Show("Ошибка", "Категория с таким названием уже существует!", MessageType.Error);
                    return;
                }

                // Insert
                string query = @"INSERT INTO Categories (CategoryName, Description)
                                VALUES (@name, @desc)";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", categoryName),
                    new SqlParameter("@desc", string.IsNullOrEmpty(description) ? DBNull.Value : (object)description)
                };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Категория успешно добавлена!", MessageType.Success);
                }
            }

            LoadCategories();
            ClearEditForm();
            isEditMode = false;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ClearEditForm();
            isEditMode = false;
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadCategories();
            txtSearch.Clear();
        }

        private void ClearEditForm()
        {
            txtCategoryName.Clear();
            txtDescription.Clear();
            currentCategoryId = 0;
        }

        private void CategoriesForm_Load(object sender, EventArgs e)
        {

        }


      
    }
}