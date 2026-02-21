namespace MiniMarketPekarnia
{
    partial class SuppliesForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // SuppliesForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 750);
            Margin = new Padding(3, 4, 3, 4);
            Name = "SuppliesForm";
            Text = "Управление поставками";
            //Load += SuppliesForm_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}