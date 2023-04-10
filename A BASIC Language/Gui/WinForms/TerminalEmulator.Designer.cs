namespace A_BASIC_Language.Gui.WinForms
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TerminalEmulator));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "status_icon_empty.png");
            this.imageList1.Images.SetKeyName(1, "status_icon_running.png");
            this.imageList1.Images.SetKeyName(2, "status_icon_done.png");
            // 
            // TerminalEmulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 498);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "TerminalEmulator";
            this.Activated += new System.EventHandler(this.TerminalEmulator_Activated);
            this.Deactivate += new System.EventHandler(this.TerminalEmulator_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TerminalEmulator_FormClosed);
            this.Load += new System.EventHandler(this.TerminalEmulator_Load);
            this.Shown += new System.EventHandler(this.TerminalEmulator_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TerminalEmulator_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TerminalEmulator_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TerminalEmulator_KeyPress);
            this.Resize += new System.EventHandler(this.TerminalEmulator_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private ImageList imageList1;
    }
}
