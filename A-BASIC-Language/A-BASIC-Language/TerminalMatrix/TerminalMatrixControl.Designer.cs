using System.Drawing;

namespace TerminalMatrixNetFramework
{
    partial class TerminalMatrixControl
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
            if (disposing)
            {
                QuitFlag = true;
                _timer.Dispose();
                _fastBitmap?.Dispose();
                _fastBitmap = null;
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // TerminalMatrixControl
            // 
            Name = "TerminalMatrixControl";
            Size = new Size(800, 450);
            Load += UserControl1_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}
