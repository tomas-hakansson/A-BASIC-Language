using System.Drawing;
using System.Windows.Forms;

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
            this.label1 = new System.Windows.Forms.Label();
            this.terminalResolutionComboBox1 = new A_BASIC_Language.Gui.UserControls.TerminalResolutionComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkHighQualityRendering = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Terminal resolution:";
            // 
            // terminalResolutionComboBox1
            // 
            this.terminalResolutionComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.terminalResolutionComboBox1.FormattingEnabled = true;
            this.terminalResolutionComboBox1.Location = new System.Drawing.Point(7, 21);
            this.terminalResolutionComboBox1.Name = "terminalResolutionComboBox1";
            this.terminalResolutionComboBox1.Resolution = TerminalMatrixNetFramework.Resolution.Pixels480x200Characters60x25;
            this.terminalResolutionComboBox1.Size = new System.Drawing.Size(282, 21);
            this.terminalResolutionComboBox1.TabIndex = 1;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(154, 88);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(64, 20);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(223, 88);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(64, 20);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkHighQualityRendering
            // 
            this.chkHighQualityRendering.AutoSize = true;
            this.chkHighQualityRendering.Location = new System.Drawing.Point(12, 52);
            this.chkHighQualityRendering.Name = "chkHighQualityRendering";
            this.chkHighQualityRendering.Size = new System.Drawing.Size(128, 17);
            this.chkHighQualityRendering.TabIndex = 2;
            this.chkHighQualityRendering.Text = "High quality rendering";
            this.chkHighQualityRendering.UseVisualStyleBackColor = true;
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(294, 113);
            this.Controls.Add(this.chkHighQualityRendering);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.terminalResolutionComboBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.OptionsDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private UserControls.TerminalResolutionComboBox terminalResolutionComboBox1;
        private Button btnOk;
        private Button btnCancel;
        private CheckBox chkHighQualityRendering;
    }
}