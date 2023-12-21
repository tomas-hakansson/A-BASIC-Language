namespace A_BASIC_Language.Gui
{
    partial class TerminalEmulator
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TerminalEmulator));
            imageList1 = new ImageList(components);
            terminalMatrixControl1 = new TerminalMatrix.TerminalMatrixControl();
            SuspendLayout();
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "status_icon_empty.png");
            imageList1.Images.SetKeyName(1, "status_icon_running.png");
            imageList1.Images.SetKeyName(2, "status_icon_done.png");
            // 
            // terminalMatrixControl1
            // 
            terminalMatrixControl1.BorderHeight = 0;
            terminalMatrixControl1.BorderWidth = 0;
            terminalMatrixControl1.CurrentCursorColor = 1;
            terminalMatrixControl1.Dock = DockStyle.Fill;
            terminalMatrixControl1.Location = new Point(0, 0);
            terminalMatrixControl1.Name = "terminalMatrixControl1";
            terminalMatrixControl1.Size = new Size(759, 498);
            terminalMatrixControl1.TabIndex = 1;
            terminalMatrixControl1.TypedLine += terminalMatrixControl1_TypedLine;
            // 
            // TerminalEmulator
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(759, 498);
            Controls.Add(terminalMatrixControl1);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimizeBox = false;
            Name = "TerminalEmulator";
            Activated += TerminalEmulator_Activated;
            Deactivate += TerminalEmulator_Deactivate;
            FormClosed += TerminalEmulator_FormClosed;
            Load += TerminalEmulator_Load;
            Shown += TerminalEmulator_Shown;
            ResumeLayout(false);
        }

        #endregion
        private ImageList imageList1;
        private TerminalMatrix.TerminalMatrixControl terminalMatrixControl1;
    }
}
