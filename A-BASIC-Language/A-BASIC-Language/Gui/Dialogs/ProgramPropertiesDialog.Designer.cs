namespace A_BASIC_Language.Gui.Dialogs
{
    partial class ProgramPropertiesDialog
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
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.txtSourceFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDataFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOpenSourceFolder = new System.Windows.Forms.Button();
            this.btnOpenDataFolder = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lvVariables = new A_BASIC_Language.Gui.UserControls.TwoColumnListView();
            this.lvDimensions = new A_BASIC_Language.Gui.UserControls.TwoColumnListView();
            this.label5 = new System.Windows.Forms.Label();
            this.lvFunctions = new A_BASIC_Language.Gui.UserControls.TwoColumnListView();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current filename:";
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(8, 24);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(520, 20);
            this.txtFilename.TabIndex = 1;
            // 
            // txtSourceFolder
            // 
            this.txtSourceFolder.Location = new System.Drawing.Point(8, 64);
            this.txtSourceFolder.Name = "txtSourceFolder";
            this.txtSourceFolder.ReadOnly = true;
            this.txtSourceFolder.Size = new System.Drawing.Size(460, 20);
            this.txtSourceFolder.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Source folder:";
            // 
            // txtDataFolder
            // 
            this.txtDataFolder.Location = new System.Drawing.Point(8, 104);
            this.txtDataFolder.Name = "txtDataFolder";
            this.txtDataFolder.ReadOnly = true;
            this.txtDataFolder.Size = new System.Drawing.Size(460, 20);
            this.txtDataFolder.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Data folder:";
            // 
            // btnOpenSourceFolder
            // 
            this.btnOpenSourceFolder.Location = new System.Drawing.Point(472, 62);
            this.btnOpenSourceFolder.Name = "btnOpenSourceFolder";
            this.btnOpenSourceFolder.Size = new System.Drawing.Size(56, 23);
            this.btnOpenSourceFolder.TabIndex = 4;
            this.btnOpenSourceFolder.Text = "Open...";
            this.btnOpenSourceFolder.UseVisualStyleBackColor = true;
            // 
            // btnOpenDataFolder
            // 
            this.btnOpenDataFolder.Location = new System.Drawing.Point(472, 102);
            this.btnOpenDataFolder.Name = "btnOpenDataFolder";
            this.btnOpenDataFolder.Size = new System.Drawing.Size(56, 23);
            this.btnOpenDataFolder.TabIndex = 7;
            this.btnOpenDataFolder.Text = "Open...";
            this.btnOpenDataFolder.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(452, 480);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Variables:";
            // 
            // lvVariables
            // 
            this.lvVariables.FullRowSelect = true;
            this.lvVariables.HideSelection = false;
            this.lvVariables.Location = new System.Drawing.Point(8, 144);
            this.lvVariables.Name = "lvVariables";
            this.lvVariables.Size = new System.Drawing.Size(520, 97);
            this.lvVariables.TabIndex = 9;
            this.lvVariables.UseCompatibleStateImageBehavior = false;
            this.lvVariables.View = System.Windows.Forms.View.Details;
            // 
            // lvDimensions
            // 
            this.lvDimensions.FullRowSelect = true;
            this.lvDimensions.HideSelection = false;
            this.lvDimensions.Location = new System.Drawing.Point(8, 260);
            this.lvDimensions.Name = "lvDimensions";
            this.lvDimensions.Size = new System.Drawing.Size(520, 97);
            this.lvDimensions.TabIndex = 11;
            this.lvDimensions.UseCompatibleStateImageBehavior = false;
            this.lvDimensions.View = System.Windows.Forms.View.Details;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 244);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Arrays:";
            // 
            // lvFunctions
            // 
            this.lvFunctions.FullRowSelect = true;
            this.lvFunctions.HideSelection = false;
            this.lvFunctions.Location = new System.Drawing.Point(8, 376);
            this.lvFunctions.Name = "lvFunctions";
            this.lvFunctions.Size = new System.Drawing.Size(520, 97);
            this.lvFunctions.TabIndex = 13;
            this.lvFunctions.UseCompatibleStateImageBehavior = false;
            this.lvFunctions.View = System.Windows.Forms.View.Details;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 360);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Functions:";
            // 
            // ProgramPropertiesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 509);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lvDimensions);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lvVariables);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOpenDataFolder);
            this.Controls.Add(this.btnOpenSourceFolder);
            this.Controls.Add(this.txtDataFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSourceFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvFunctions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgramPropertiesDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Program properties";
            this.Load += new System.EventHandler(this.ProgramPropertiesDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.TextBox txtSourceFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDataFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOpenSourceFolder;
        private System.Windows.Forms.Button btnOpenDataFolder;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label4;
        private UserControls.TwoColumnListView lvVariables;
        private UserControls.TwoColumnListView lvDimensions;
        private System.Windows.Forms.Label label5;
        private UserControls.TwoColumnListView lvFunctions;
        private System.Windows.Forms.Label label6;
    }
}