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
            btnTestApi = new Button();
            txtConsole = new TextBox();
            SuspendLayout();
            // 
            // btnTestApi
            // 
            btnTestApi.Location = new Point(12, 12);
            btnTestApi.Name = "btnTestApi";
            btnTestApi.Size = new Size(112, 34);
            btnTestApi.TabIndex = 0;
            btnTestApi.Text = "API Teszt";
            btnTestApi.UseVisualStyleBackColor = true;
            btnTestApi.Click += btnTestApi_Click;
            // 
            // txtConsole
            // 
            txtConsole.Location = new Point(12, 52);
            txtConsole.Multiline = true;
            txtConsole.Name = "txtConsole";
            txtConsole.ScrollBars = ScrollBars.Vertical;
            txtConsole.Size = new Size(776, 386);
            txtConsole.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtConsole);
            Controls.Add(btnTestApi);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnTestApi;
        private TextBox txtConsole;
    }
}
