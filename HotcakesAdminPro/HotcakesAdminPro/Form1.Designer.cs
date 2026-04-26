namespace HotcakesAdminPro
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            panel2 = new Panel();
            gridTopProducts = new DataGridView();
            panel1 = new Panel();
            lblAvgOrder = new Label();
            lblTotalRevenue = new Label();
            lblTotalSold = new Label();
            btnLoadColors = new Button();
            tabPage2 = new TabPage();
            gridVip = new DataGridView();
            btnLoadVip = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridTopProducts).BeginInit();
            panel1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridVip).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(950, 821);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(panel2);
            tabPage1.Controls.Add(panel1);
            tabPage1.Location = new Point(4, 34);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(942, 783);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Színeladások";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Controls.Add(gridTopProducts);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(3, 580);
            panel2.Name = "panel2";
            panel2.Size = new Size(936, 200);
            panel2.TabIndex = 2;
            // 
            // gridTopProducts
            // 
            gridTopProducts.AllowUserToAddRows = false;
            gridTopProducts.AllowUserToDeleteRows = false;
            gridTopProducts.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridTopProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridTopProducts.Location = new Point(0, 0);
            gridTopProducts.Name = "gridTopProducts";
            gridTopProducts.ReadOnly = true;
            gridTopProducts.RowHeadersWidth = 62;
            gridTopProducts.Size = new Size(936, 200);
            gridTopProducts.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(lblAvgOrder);
            panel1.Controls.Add(lblTotalRevenue);
            panel1.Controls.Add(lblTotalSold);
            panel1.Controls.Add(btnLoadColors);
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(936, 120);
            panel1.TabIndex = 1;
            // 
            // lblAvgOrder
            // 
            lblAvgOrder.AutoSize = true;
            lblAvgOrder.Location = new Point(3, 40);
            lblAvgOrder.Name = "lblAvgOrder";
            lblAvgOrder.Size = new Size(196, 25);
            lblAvgOrder.TabIndex = 3;
            lblAvgOrder.Text = "Átlagos kosárérték: - Ft";
            // 
            // lblTotalRevenue
            // 
            lblTotalRevenue.AutoSize = true;
            lblTotalRevenue.Location = new Point(566, 8);
            lblTotalRevenue.Name = "lblTotalRevenue";
            lblTotalRevenue.Size = new Size(104, 25);
            lblTotalRevenue.TabIndex = 2;
            lblTotalRevenue.Text = "Bevétel: - Ft";
            // 
            // lblTotalSold
            // 
            lblTotalSold.AutoSize = true;
            lblTotalSold.Location = new Point(277, 8);
            lblTotalSold.Name = "lblTotalSold";
            lblTotalSold.Size = new Size(135, 25);
            lblTotalSold.TabIndex = 1;
            lblTotalSold.Text = "Eladott darab: -";
            // 
            // btnLoadColors
            // 
            btnLoadColors.Location = new Point(3, 3);
            btnLoadColors.Name = "btnLoadColors";
            btnLoadColors.Size = new Size(268, 34);
            btnLoadColors.TabIndex = 0;
            btnLoadColors.Text = "Szín Statisztika Betöltése";
            btnLoadColors.UseVisualStyleBackColor = true;
            btnLoadColors.Click += btnLoadColors_Click_1;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(gridVip);
            tabPage2.Controls.Add(btnLoadVip);
            tabPage2.Location = new Point(4, 34);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(768, 388);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "VIP Kereső";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // gridVip
            // 
            gridVip.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridVip.Location = new Point(6, 46);
            gridVip.Name = "gridVip";
            gridVip.RowHeadersWidth = 62;
            gridVip.Size = new Size(756, 336);
            gridVip.TabIndex = 1;
            // 
            // btnLoadVip
            // 
            btnLoadVip.Location = new Point(6, 6);
            btnLoadVip.Name = "btnLoadVip";
            btnLoadVip.Size = new Size(245, 34);
            btnLoadVip.TabIndex = 0;
            btnLoadVip.Text = "VIP Vásárlók Keresése";
            btnLoadVip.UseVisualStyleBackColor = true;
            btnLoadVip.Click += btnLoadVip_Click_1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(967, 845);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridTopProducts).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridVip).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private DataGridView gridVip;
        private Button btnLoadVip;
        private Button btnLoadColors;
        private Panel panel1;
        private Panel panel2;
        private Label lblTotalRevenue;
        private Label lblTotalSold;
        private DataGridView gridTopProducts;
        private Label lblAvgOrder;
    }
}
