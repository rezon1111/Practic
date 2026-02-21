using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace MiniMarketPekarnia
{
    public partial class UsersForm : Form
    {
        private DataGridView dgvUsers;
        private TextBox txtSearch;
        private TextBox txtFullName;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private ComboBox cmbRole;
        private CheckBox chkIsActive;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnSave;
        private Button btnCancel;
        private Button btnRefresh;
        private Button btnResetPassword;
        private Panel headerPanel;
        private Label lblTitle;
        private Panel editPanel;
        private bool isEditMode = false;
        private int currentUserId = 0;

        public UsersForm()
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
            LoadUsers();
            LoadRoles();
        }

        private void SetupForm()
        {
            // Header
            headerPanel = new Panel
            {
                Size = new Size(1200, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(52, 73, 94)
            };

            lblTitle = new Label
            {
                Text = "👤  Управление пользователями",
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
                BackColor = Color.FromArgb(52, 73, 94),
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
                Text = "Поиск пользователей:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 15),
                Size = new Size(150, 20)
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

            // Left panel - Users list
            Panel leftPanel = new Panel
            {
                Size = new Size(760, 520),
                Location = new Point(20, 160),
                BackColor = Color.White
            };

            // Users grid
            dgvUsers = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(720, 410),
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

            dgvUsers.EnableHeadersVisualStyles = false;
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvUsers.ColumnHeadersHeight = 40;
            dgvUsers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 73, 94);
            dgvUsers.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvUsers.RowTemplate.Height = 35;
            dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);
            dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;

            // Buttons
            btnAdd = new Button
            {
                Text = "➕ Добавить",
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 450),
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
                Location = new Point(130, 450),
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
                Location = new Point(260, 450),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            btnResetPassword = new Button
            {
                Text = "🔄 Сброс пароля",
                Font = new Font("Segoe UI", 11),
                Location = new Point(370, 450),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnResetPassword.FlatAppearance.BorderSize = 0;
            btnResetPassword.Click += BtnResetPassword_Click;

            leftPanel.Controls.AddRange(new Control[] {
                dgvUsers, btnAdd, btnEdit, btnDelete, btnResetPassword
            });

            // Right panel - Edit form
            editPanel = new Panel
            {
                Size = new Size(380, 520),
                Location = new Point(800, 160),
                BackColor = Color.White
            };

            Label lblEditTitle = new Label
            {
                Text = "📝 Данные пользователя",
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

            // Username
            Label lblUsername = new Label
            {
                Text = "Логин:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 135),
                Size = new Size(340, 20)
            };

            txtUsername = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 160),
                Size = new Size(340, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Password
            Label lblPassword = new Label
            {
                Text = "Пароль (оставьте пустым, чтобы не менять):",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 200),
                Size = new Size(340, 20)
            };

            txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 225),
                Size = new Size(340, 25),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '*',
                UseSystemPasswordChar = true
            };

            // Role
            Label lblRole = new Label
            {
                Text = "Роль:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 265),
                Size = new Size(340, 20)
            };

            cmbRole = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 290),
                Size = new Size(340, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Is Active
            chkIsActive = new CheckBox
            {
                Text = "Активен",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 330),
                Size = new Size(200, 25),
                Checked = true
            };

            // Save button
            btnSave = new Button
            {
                Text = "💾 Сохранить",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 380),
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
                Location = new Point(200, 380),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            editPanel.Controls.AddRange(new Control[] {
                lblEditTitle, lblFullName, txtFullName, lblUsername, txtUsername,
                lblPassword, txtPassword, lblRole, cmbRole, chkIsActive,
                btnSave, btnCancel
            });

            this.Controls.AddRange(new Control[] { headerPanel, searchPanel, leftPanel, editPanel });
            ClearEditForm();
        }

        private void LoadUsers()
        {
            string query = @"SELECT u.UserID, 
                        u.FullName AS 'ФИО', 
                        u.Username AS 'Логин',
                        r.RoleName AS 'Роль',
                        CASE WHEN u.IsActive = 1 THEN 'Да' ELSE 'Нет' END AS 'Активен',
                        FORMAT(u.CreatedDate, 'dd.MM.yyyy') AS 'Дата регистрации'
                 FROM Users u
                 INNER JOIN Roles r ON u.RoleID = r.RoleID
                 ORDER BY u.FullName";

            DataTable dataTable = DatabaseHelper.ExecuteQuery(query);
            dgvUsers.DataSource = dataTable;

            if (dgvUsers.Columns.Contains("UserID"))
                dgvUsers.Columns["UserID"].Visible = false;
        }

        private void LoadRoles()
        {
            string query = "SELECT RoleID, RoleName FROM Roles ORDER BY RoleName";
            DataTable roles = DatabaseHelper.ExecuteQuery(query);

            cmbRole.DisplayMember = "RoleName";
            cmbRole.ValueMember = "RoleID";
            cmbRole.DataSource = roles;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadUsers();
            }
            else
            {
                string query = @"SELECT u.UserID, 
                                        u.FullName AS 'ФИО', 
                                        u.Username AS 'Логин',
                                        r.RoleName AS 'Роль',
                                        CASE WHEN u.IsActive = 1 THEN 'Да' ELSE 'Нет' END AS 'Активен',
                                        FORMAT(u.CreatedDate, 'dd.MM.yyyy') AS 'Дата регистрации'
                                 FROM Users u
                                 INNER JOIN Roles r ON u.RoleID = r.RoleID
                                 WHERE u.FullName LIKE @search OR u.Username LIKE @search
                                 ORDER BY u.FullName";

                var parameters = new[] {
                    new SqlParameter("@search", $"%{searchText}%")
                };

                DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvUsers.DataSource = dataTable;
            }
        }

        private void DgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow != null && !isEditMode)
            {
                DataGridViewRow row = dgvUsers.CurrentRow;
                currentUserId = Convert.ToInt32(row.Cells["UserID"].Value);
                txtFullName.Text = row.Cells["ФИО"].Value.ToString();
                txtUsername.Text = row.Cells["Логин"].Value.ToString();
                txtPassword.Clear();

                string roleName = row.Cells["Роль"].Value.ToString();
                for (int i = 0; i < cmbRole.Items.Count; i++)
                {
                    DataRowView item = cmbRole.Items[i] as DataRowView;
                    if (item != null && item["RoleName"].ToString() == roleName)
                    {
                        cmbRole.SelectedIndex = i;
                        break;
                    }
                }

                chkIsActive.Checked = row.Cells["Активен"].Value.ToString() == "Да";
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            isEditMode = false;
            currentUserId = 0;
            ClearEditForm();
            txtFullName.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null)
            {
                CustomMessageBox.Show("Внимание", "Выберите пользователя для редактирования!", MessageType.Warning);
                return;
            }
            isEditMode = true;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null)
            {
                CustomMessageBox.Show("Внимание", "Выберите пользователя для удаления!", MessageType.Warning);
                return;
            }

            if (currentUserId == CurrentUser.UserID)
            {
                CustomMessageBox.Show("Ошибка", "Вы не можете удалить самого себя!", MessageType.Error);
                return;
            }

            DialogResult result = CustomMessageBox.ShowConfirm("Подтверждение",
                $"Вы уверены, что хотите удалить пользователя \"{txtFullName.Text}\"?");

            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM Users WHERE UserID = @id";
                SqlParameter[] parameters = { new SqlParameter("@id", currentUserId) };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Пользователь успешно удален!", MessageType.Success);
                    LoadUsers();
                    ClearEditForm();
                }
            }
        }

        private void BtnResetPassword_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null)
            {
                CustomMessageBox.Show("Внимание", "Выберите пользователя!", MessageType.Warning);
                return;
            }

            DialogResult result = CustomMessageBox.ShowConfirm("Подтверждение",
                $"Сбросить пароль пользователя \"{txtFullName.Text}\" на \"123456\"?");

            if (result == DialogResult.Yes)
            {
                string newPassword = HashPassword("123456");
                string query = "UPDATE Users SET PasswordHash = @password WHERE UserID = @id";
                SqlParameter[] parameters = {
                    new SqlParameter("@password", newPassword),
                    new SqlParameter("@id", currentUserId)
                };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Пароль сброшен на 123456!", MessageType.Success);
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFullName.Text))
            {
                CustomMessageBox.Show("Ошибка", "Введите имя пользователя!", MessageType.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                CustomMessageBox.Show("Ошибка", "Введите логин!", MessageType.Warning);
                return;
            }

            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            int roleId = (int)cmbRole.SelectedValue;
            int isActive = chkIsActive.Checked ? 1 : 0;

            if (isEditMode && currentUserId > 0)
            {
                // Check if username already exists for another user
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username AND UserID != @id";
                SqlParameter[] checkParams = {
                    new SqlParameter("@username", username),
                    new SqlParameter("@id", currentUserId)
                };
                int exists = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParams));

                if (exists > 0)
                {
                    CustomMessageBox.Show("Ошибка", "Пользователь с таким логином уже существует!", MessageType.Error);
                    return;
                }

                // Update
                string query;
                SqlParameter[] parameters;

                if (string.IsNullOrEmpty(password))
                {
                    query = @"UPDATE Users 
                             SET FullName = @name, Username = @username, RoleID = @roleId, IsActive = @isActive
                             WHERE UserID = @id";

                    parameters = new SqlParameter[] {
                        new SqlParameter("@name", fullName),
                        new SqlParameter("@username", username),
                        new SqlParameter("@roleId", roleId),
                        new SqlParameter("@isActive", isActive),
                        new SqlParameter("@id", currentUserId)
                    };
                }
                else
                {
                    string hashedPassword = HashPassword(password);
                    query = @"UPDATE Users 
                             SET FullName = @name, Username = @username, PasswordHash = @password, 
                                 RoleID = @roleId, IsActive = @isActive
                             WHERE UserID = @id";

                    parameters = new SqlParameter[] {
                        new SqlParameter("@name", fullName),
                        new SqlParameter("@username", username),
                        new SqlParameter("@password", hashedPassword),
                        new SqlParameter("@roleId", roleId),
                        new SqlParameter("@isActive", isActive),
                        new SqlParameter("@id", currentUserId)
                    };
                }

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Данные пользователя обновлены!", MessageType.Success);
                }
            }
            else
            {
                // Check if username already exists
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                SqlParameter[] checkParams = { new SqlParameter("@username", username) };
                int exists = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParams));

                if (exists > 0)
                {
                    CustomMessageBox.Show("Ошибка", "Пользователь с таким логином уже существует!", MessageType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    CustomMessageBox.Show("Ошибка", "Введите пароль!", MessageType.Warning);
                    return;
                }

                // Insert
                string hashedPassword = HashPassword(password);
                string query = @"INSERT INTO Users (FullName, Username, PasswordHash, RoleID, IsActive)
                                VALUES (@name, @username, @password, @roleId, @isActive)";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", fullName),
                    new SqlParameter("@username", username),
                    new SqlParameter("@password", hashedPassword),
                    new SqlParameter("@roleId", roleId),
                    new SqlParameter("@isActive", isActive)
                };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rows > 0)
                {
                    CustomMessageBox.Show("Успех", "Пользователь успешно добавлен!", MessageType.Success);
                }
            }

            LoadUsers();
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
            LoadUsers();
            txtSearch.Clear();
        }

        private void ClearEditForm()
        {
            txtFullName.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            if (cmbRole.Items.Count > 0)
                cmbRole.SelectedIndex = 0;
            chkIsActive.Checked = true;
            currentUserId = 0;
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

        private void UsersForm_Load(object sender, EventArgs e)
        {

        }

     
    }
}