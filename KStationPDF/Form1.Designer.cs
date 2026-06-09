namespace KStationPDF
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
            panelDrop = new Panel();
            lblDrop = new Label();
            panelButtons = new Panel();
            chkShowPassword = new CheckBox();
            txtPassword = new TextBox();
            chkOverwrite = new CheckBox();
            btnClear = new Button();
            btnSelect = new Button();
            panelBottom = new Panel();
            lblStatus = new Label();
            progressBar1 = new ProgressBar();
            txtLog = new RichTextBox();
            panelDrop.SuspendLayout();
            panelButtons.SuspendLayout();
            panelBottom.SuspendLayout();
            SuspendLayout();
            // 
            // panelDrop
            // 
            panelDrop.AllowDrop = true;
            panelDrop.BackColor = Color.FromArgb(240, 245, 255);
            panelDrop.BorderStyle = BorderStyle.FixedSingle;
            panelDrop.Controls.Add(lblDrop);
            panelDrop.Dock = DockStyle.Top;
            panelDrop.Location = new Point(0, 0);
            panelDrop.Name = "panelDrop";
            panelDrop.Size = new Size(761, 120);
            panelDrop.TabIndex = 0;
            // 
            // lblDrop
            // 
            lblDrop.AutoSize = true;
            lblDrop.Dock = DockStyle.Fill;
            lblDrop.ForeColor = Color.FromArgb(60, 90, 150);
            lblDrop.Location = new Point(0, 0);
            lblDrop.Name = "lblDrop";
            lblDrop.Size = new Size(196, 15);
            lblDrop.TabIndex = 1;
            lblDrop.Text = "📄 ここに PDF ファイルをドラッグ＆ドロップ";
            lblDrop.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelButtons
            // 
            panelButtons.Controls.Add(chkShowPassword);
            panelButtons.Controls.Add(txtPassword);
            panelButtons.Controls.Add(chkOverwrite);
            panelButtons.Controls.Add(btnClear);
            panelButtons.Controls.Add(btnSelect);
            panelButtons.Dock = DockStyle.Top;
            panelButtons.Location = new Point(0, 120);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(761, 45);
            panelButtons.TabIndex = 1;
            // 
            // chkShowPassword
            // 
            chkShowPassword.AutoSize = true;
            chkShowPassword.Location = new Point(655, 12);
            chkShowPassword.Name = "chkShowPassword";
            chkShowPassword.Size = new Size(50, 19);
            chkShowPassword.TabIndex = 5;
            chkShowPassword.Text = "表示";
            chkShowPassword.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(470, 9);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "パスワード（あれば入力）";
            txtPassword.Size = new Size(180, 23);
            txtPassword.TabIndex = 4;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // chkOverwrite
            // 
            chkOverwrite.AutoSize = true;
            chkOverwrite.Checked = true;
            chkOverwrite.CheckState = CheckState.Checked;
            chkOverwrite.Location = new Point(175, 12);
            chkOverwrite.Name = "chkOverwrite";
            chkOverwrite.Size = new Size(114, 19);
            chkOverwrite.TabIndex = 3;
            chkOverwrite.Text = "元ファイルを上書き";
            chkOverwrite.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(340, 7);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(120, 32);
            btnClear.TabIndex = 2;
            btnClear.Text = "🗑️ ログクリア";
            btnClear.UseVisualStyleBackColor = true;
            // 
            // btnSelect
            // 
            btnSelect.Location = new Point(10, 7);
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(150, 32);
            btnSelect.TabIndex = 0;
            btnSelect.Text = "📂 ファイルを選択";
            btnSelect.UseVisualStyleBackColor = true;
            // 
            // panelBottom
            // 
            panelBottom.Controls.Add(lblStatus);
            panelBottom.Controls.Add(progressBar1);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.ForeColor = Color.Green;
            panelBottom.Location = new Point(0, 451);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(761, 50);
            panelBottom.TabIndex = 2;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.Location = new Point(0, 18);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(8, 0, 0, 0);
            lblStatus.Size = new Size(78, 15);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "✅ 準備完了";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // progressBar1
            // 
            progressBar1.Dock = DockStyle.Top;
            progressBar1.Location = new Point(0, 0);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(761, 18);
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 0;
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.BorderStyle = BorderStyle.None;
            txtLog.Dock = DockStyle.Fill;
            txtLog.ForeColor = Color.Gainsboro;
            txtLog.Location = new Point(0, 165);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.Size = new Size(761, 286);
            txtLog.TabIndex = 2;
            txtLog.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(761, 501);
            Controls.Add(txtLog);
            Controls.Add(panelBottom);
            Controls.Add(panelButtons);
            Controls.Add(panelDrop);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Location = new Point(175, 12);
            MinimizeBox = false;
            MinimumSize = new Size(500, 400);
            Name = "Form1";
            Text = "PDF オブジェクト圧縮解除ツール ";
            panelDrop.ResumeLayout(false);
            panelDrop.PerformLayout();
            panelButtons.ResumeLayout(false);
            panelButtons.PerformLayout();
            panelBottom.ResumeLayout(false);
            panelBottom.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelDrop;
        private Label lblDrop;
        private Panel panelButtons;
        private Button btnClear;
        private Button btnSelect;
        private CheckBox chkOverwrite;
        private Panel panelBottom;
        private RichTextBox txtLog;
        private Label lblStatus;
        private ProgressBar progressBar1;
        private CheckBox chkShowPassword;
        private TextBox txtPassword;
    }
}
