using MiniMarketPekarnia;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniMarketPekarnia
{
   

    public static class CustomMessageBox
    {
        public static void Show(string title, string message, MessageType type)
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
                BackColor = type == MessageType.Success ? Color.FromArgb(46, 204, 113) :
                           type == MessageType.Error ? Color.FromArgb(231, 76, 60) :
                           type == MessageType.Warning ? Color.FromArgb(241, 196, 15) :
                           Color.FromArgb(52, 152, 219)
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

            Button btnOk = new Button
            {
                Text = "OK",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(150, 140),
                Size = new Size(100, 30),
                BackColor = header.BackColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (s, e) => msgForm.Close();

            header.Controls.Add(lblTitle);
            msgForm.Controls.AddRange(new Control[] { header, lblMessage, btnOk });
            msgForm.ShowDialog();
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