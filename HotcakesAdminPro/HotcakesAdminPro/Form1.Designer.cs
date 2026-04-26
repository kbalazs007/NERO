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
            btnLoadColors = new Button();
            tabPage2 = new TabPage();
            gridVip = new DataGridView();
            btnLoadVip = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridVip).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(776, 426);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnLoadColors);
            tabPage1.Location = new Point(4, 34);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(768, 388);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Színeladások";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnLoadColors
            // 
            btnLoadColors.Location = new Point(6, 6);
            btnLoadColors.Name = "btnLoadColors";
            btnLoadColors.Size = new Size(138, 60);
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
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
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
    }
}
