namespace A_BASIC_Language.Gui.Dialogs
{
    partial class OptionsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            terminalResolutionComboBox1 = new UserControls.TerminalResolutionComboBox();
            btnOk = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 8);
            label1.Name = "label1";
            label1.Size = new Size(111, 15);
            label1.TabIndex = 0;
            label1.Text = "Terminal resolution:";
            // 
            // terminalResolutionComboBox1
            // 
            terminalResolutionComboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            terminalResolutionComboBox1.FormattingEnabled = true;
            terminalResolutionComboBox1.Location = new Point(8, 24);
            terminalResolutionComboBox1.Name = "terminalResolutionComboBox1";
            terminalResolutionComboBox1.Resolution = TerminalMatrix.Resolution.Pixels480x200Characters60x25;
            terminalResolutionComboBox1.Size = new Size(328, 23);
            terminalResolutionComboBox1.TabIndex = 1;
            // 
            // btnOk
            // 
            btnOk.Location = new Point(180, 76);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 2;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(260, 76);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // OptionsDialog
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(343, 106);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(terminalResolutionComboBox1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OptionsDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Options";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private UserControls.TerminalResolutionComboBox terminalResolutionComboBox1;
        private Button btnOk;
        private Button btnCancel;
    }
}