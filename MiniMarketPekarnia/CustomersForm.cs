using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MiniMarketPekarnia
{
    public partial class CustomersForm : Form
    {
        private DataGridView dgvCustomers;
        private TextBox txtSearch;
        private TextBox txtFullName;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private CheckBox chkDiscountCard;
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
        private int currentCustomerId = 0;

        public CustomersForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 245, 245);
            SetupForm();
            LoadCustomers();
        }

        private void SetupForm()
        {
            // Header
            headerPanel = new Panel
            {
                Size = new Size(1200, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(155, 89, 182)
            };

            lblTitle = new Label
            {
                Text = "👥  Управление клиентами",
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
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCloseHeader.FlatAppearance.BorderSize = 0;
            btnCloseHeader.Click += (s, e) => this.Close();

            headerPanel.Controls.AddRange(new Control[] { lblTitle, btnCloseHeader });

            // Search panel
            Panel searchPanel = new Panel
            {
                Size = new Size(1160, 70),
                Location = new Point(20, 80),
                BackColor = Color.White
            };

            Label lblSearch = new Label
            {
                Text = "Поиск клиентов:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 15),
                Size = new Size(120, 20)
            };

            txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 40),
                Size = new Size(400, 25),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            btnRefresh = new Button
            {
                Text = "🔄 Обновить",
                Font = new Font("Segoe UI", 11),
                Location = new Point(440, 37),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            searchPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnRefresh });

            // Left panel - Customers list
            Panel leftPanel = new Panel
            {
                Size = new Size(760, 540),
                Location = new Point(20, 160),
                BackColor = Color.White
            };

            // Customers grid
            dgvCustomers = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(720, 430),
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

            dgvCustomers.EnableHeadersVisualStyles = false;
            dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCustomers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvCustomers.ColumnHeadersHeight = 40;
            dgvCustomers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(155, 89, 182);
            dgvCustomers.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvCustomers.RowTemplate.Height = 35;
            dgvCustomers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);
            dgvCustomers.SelectionChanged += DgvCustomers_SelectionChanged;

            // Buttons
            btnAdd = new Button
            {
                Text = "➕ Добавить",
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 470),
                Size = new Size(120, 40),
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
                Location = new Point(150, 470),
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
                Location = new Point(280, 470),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            leftPanel.Controls.AddRange(new Control[] {
                dgvCustomers, btnAdd, btnEdit, btnDelete
            });

            // Right panel - Edit form
            editPanel = new Panel
            {
                Size = new Size(380, 540),
                Location = new Point(800, 160),
                BackColor = Color.White
            };

            Label lblEditTitle = new Label
            {
                Text = "📝 Данные клиента",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 20),
                Size = new Size(340, 30)
            };

            // Full Name
            Label lblFullName = new Label
            {
                Text = "Полное имя:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 70),
                Size = new Size(340, 20)
            };

            txtFullName = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 95),
                Size = new Size(340, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Phone
            Label lblPhone = new Label
            {
                Text = "Телефон:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 135),
                Size = new Size(340, 20)
            };

            txtPhone = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 160),
                Size = new Size(340, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Email
            Label lblEmail = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 200),
                Size = new Size(340, 20)
            };

            txtEmail = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 225),
                Size = new Size(340, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Discount card
            chkDiscountCard = new CheckBox
            {
                Text = "Дисконтная карта",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 270),
                Size = new Size(200, 25)
            };

            // Save button
            btnSave = new Button
            {
                Text = "💾 Сохранить",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 320),
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
                Location = new Point(200, 320),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            editPanel.Controls.AddRange(new Control[] {
                lblEditTitle, lblFullName, txtFullName, lblPhone, txtPhone,
                lblEmail, txtEmail, chkDiscountCard, btnSave, btnCancel
            });

            this.Controls.AddRange(new Control[] { headerPanel, searchPanel, leftPanel, editPanel });
            ClearEditForm();
        }

        private void LoadCustomers()
        {
            string query = @"SELECT CustomerID, 
                                    FullName AS 'ФИО', 
                                    Phone AS 'Телефон', 
                                    Email AS 'Email',
                                    CASE WHEN DiscountCard = 1 THEN 'Да' ELSE 'Нет' END AS 'Дисконтная карта'
                             FROM Customers
                             ORDER BY FullName";

            DataTable dataTable = DatabaseHelper.ExecuteQuery(query);
            dgvCustomers.DataSource = dataTable;

            if (dgvCustomers.Columns.Contains("CustomerID"))
                dgvCustomers.Columns["CustomerID"].Visible = false;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadCustomers();
            }
            else
            {
                string query = @"SELECT CustomerID, 
                                        FullName AS 'ФИО', 
                                        Phone AS 'Телефон', 
                                        Email AS 'Email',
                                        CASE WHEN DiscountCard = 1 THEN 'Да' ELSE 'Нет' END AS 'Дисконтная карта'
                                 FROM Customers
                                 WHERE FullName LIKE @search OR Phone LIKE @search OR Email LIKE @search
                                 ORDER BY FullName";

                var parameters = new[] {
                    new SqlParameter("@search", $"%{searchText}%")
                };

                DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvCustomers.DataSource = dataTable;
            }
        }

        private void DgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow != null && !isEditMode)
            {
                DataGridViewRow row = dgvCustomers.CurrentRow;
                currentCustomerId = Convert.ToInt32(row.Cells["CustomerID"].Value);
                txtFullName.Text = row.Cells["ФИО"].Value.ToString();
                txtPhone.Text = row.Cells["Телефон"]?.Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"]?.Value?.ToString() ?? "";
                chkDiscountCard.Checked = row.Cells["Дисконтная карта"].Value.ToString() == "Да";
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            isEditMode = false;
            currentCustomerId = 0;
            ClearEditForm();
            txtFullName.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow == null)
            {
                CustomMessageBox.Show("Внимание", "Выберите клиента для редактирования!", MessageType.Warning);
                return;
            }
            isEditMode = true;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow == null)
            {
                CustomMessageBox.Show("Внимание", "Выберите клиента для удаления!", MessageType.Warning);
                return;
            }

            DialogResult result = CustomMessageBox.ShowConfirm("Подтверждение",
                $"Вы уверены, что хотите удалить клиента \"{txtFullName.Text}\"?");

            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM Customers WHERE CustomerID = @id";
                SqlParameter[] parameters = { new SqlParameter("@id", currentCustomerId) };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Клиент успешно удален!", MessageType.Success);
                    LoadCustomers();
                    ClearEditForm();
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFullName.Text))
            {
                CustomMessageBox.Show("Ошибка", "Введите имя клиента!", MessageType.Warning);
                return;
            }

            string fullName = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();
            int discountCard = chkDiscountCard.Checked ? 1 : 0;

            if (isEditMode && currentCustomerId > 0)
            {
                // Update
                string query = @"UPDATE Customers 
                                SET FullName = @name, Phone = @phone, Email = @email, DiscountCard = @discount
                                WHERE CustomerID = @id";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", fullName),
                    new SqlParameter("@phone", string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone),
                    new SqlParameter("@email", string.IsNullOrEmpty(email) ? DBNull.Value : (object)email),
                    new SqlParameter("@discount", discountCard),
                    new SqlParameter("@id", currentCustomerId)
                };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Данные клиента обновлены!", MessageType.Success);
                }
            }
            else
            {
                // Insert
                string query = @"INSERT INTO Customers (FullName, Phone, Email, DiscountCard)
                                VALUES (@name, @phone, @email, @discount)";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", fullName),
                    new SqlParameter("@phone", string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone),
                    new SqlParameter("@email", string.IsNullOrEmpty(email) ? DBNull.Value : (object)email),
                    new SqlParameter("@discount", discountCard)
                };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Клиент успешно добавлен!", MessageType.Success);
                }
            }

            LoadCustomers();
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
            LoadCustomers();
            txtSearch.Clear();
        }

        private void ClearEditForm()
        {
            txtFullName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            chkDiscountCard.Checked = false;
            currentCustomerId = 0;
        }


        public static DialogResult ShowConfirm(string title, string message)
        {
            Form msgForm = new Form
            {
                Text = title,
                Size = new Size(400, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.White
            };

            Panel header = new Panel
            {
                Size = new Size(400, 50),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(241, 196, 15)
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 10),
                Size = new Size(380, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblMessage = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(50, 50, 50),
                Location = new Point(20, 70),
                Size = new Size(360, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnYes = new Button
            {
                Text = "Да",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(120, 140),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Yes
            };
            btnYes.FlatAppearance.BorderSize = 0;
            btnYes.Click += (s, e) => msgForm.Close();

            Button btnNo = new Button
            {
                Text = "Нет",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(210, 140),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.No
            };
            btnNo.FlatAppearance.BorderSize = 0;
            btnNo.Click += (s, e) => msgForm.Close();

            header.Controls.Add(lblTitle);
            msgForm.Controls.AddRange(new Control[] { header, lblMessage, btnYes, btnNo });
            return msgForm.ShowDialog();
        }
    }
}