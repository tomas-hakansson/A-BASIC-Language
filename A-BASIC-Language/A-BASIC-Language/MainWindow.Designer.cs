namespace A_BASIC_Language
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            fullscreenToolStripMenuItem = new ToolStripMenuItem();
            debugOutputToolStripMenuItem = new ToolStripMenuItem();
            resolutionToolStripMenuItem = new ToolStripMenuItem();
            highQualityRenderingToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            onlineHelpToolStripMenuItem = new ToolStripMenuItem();
            versionHistoryToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            btnDebug = new ToolStripButton();
            statusStrip1 = new StatusStrip();
            lblCursPos = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            lblUserAction = new ToolStripStatusLabel();
            terminalMatrixControl1 = new TerminalMatrix.TerminalMatrixControl();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            btnOptions = new ToolStripButton();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(917, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(180, 22);
            exitToolStripMenuItem.Text = "&Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { fullscreenToolStripMenuItem, debugOutputToolStripMenuItem, resolutionToolStripMenuItem, highQualityRenderingToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "&View";
            // 
            // fullscreenToolStripMenuItem
            // 
            fullscreenToolStripMenuItem.Name = "fullscreenToolStripMenuItem";
            fullscreenToolStripMenuItem.Size = new Size(193, 22);
            fullscreenToolStripMenuItem.Text = "Fullscreen";
            fullscreenToolStripMenuItem.Click += fullscreenToolStripMenuItem_Click;
            // 
            // debugOutputToolStripMenuItem
            // 
            debugOutputToolStripMenuItem.Image = Properties.Resources.debug;
            debugOutputToolStripMenuItem.Name = "debugOutputToolStripMenuItem";
            debugOutputToolStripMenuItem.Size = new Size(193, 22);
            debugOutputToolStripMenuItem.Text = "Debug output";
            debugOutputToolStripMenuItem.Click += debugOutputToolStripMenuItem_Click;
            // 
            // resolutionToolStripMenuItem
            // 
            resolutionToolStripMenuItem.Name = "resolutionToolStripMenuItem";
            resolutionToolStripMenuItem.Size = new Size(193, 22);
            resolutionToolStripMenuItem.Text = "Resolution";
            // 
            // highQualityRenderingToolStripMenuItem
            // 
            highQualityRenderingToolStripMenuItem.Checked = true;
            highQualityRenderingToolStripMenuItem.CheckState = CheckState.Checked;
            highQualityRenderingToolStripMenuItem.Name = "highQualityRenderingToolStripMenuItem";
            highQualityRenderingToolStripMenuItem.Size = new Size(193, 22);
            highQualityRenderingToolStripMenuItem.Text = "High quality rendering";
            highQualityRenderingToolStripMenuItem.Click += highQualityRenderingToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { onlineHelpToolStripMenuItem, versionHistoryToolStripMenuItem, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // onlineHelpToolStripMenuItem
            // 
            onlineHelpToolStripMenuItem.Name = "onlineHelpToolStripMenuItem";
            onlineHelpToolStripMenuItem.Size = new Size(160, 22);
            onlineHelpToolStripMenuItem.Text = "Online help...";
            onlineHelpToolStripMenuItem.Click += onlineHelpToolStripMenuItem_Click;
            // 
            // versionHistoryToolStripMenuItem
            // 
            versionHistoryToolStripMenuItem.Name = "versionHistoryToolStripMenuItem";
            versionHistoryToolStripMenuItem.Size = new Size(160, 22);
            versionHistoryToolStripMenuItem.Text = "Version history...";
            versionHistoryToolStripMenuItem.Click += versionHistoryToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(160, 22);
            aboutToolStripMenuItem.Text = "About...";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { btnDebug, toolStripSeparator1, btnOptions });
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(917, 25);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // btnDebug
            // 
            btnDebug.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnDebug.Image = Properties.Resources.debug;
            btnDebug.ImageTransparentColor = Color.Magenta;
            btnDebug.Name = "btnDebug";
            btnDebug.Size = new Size(23, 22);
            btnDebug.Text = "Debug output";
            btnDebug.Click += btnDebug_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblCursPos, toolStripStatusLabel1, lblUserAction });
            statusStrip1.Location = new Point(0, 600);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(917, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblCursPos
            // 
            lblCursPos.BorderStyle = Border3DStyle.Sunken;
            lblCursPos.Name = "lblCursPos";
            lblCursPos.Size = new Size(25, 17);
            lblCursPos.Text = "0, 0";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.BorderStyle = Border3DStyle.Sunken;
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(69, 17);
            toolStripStatusLabel1.Text = "User action:";
            // 
            // lblUserAction
            // 
            lblUserAction.BorderStyle = Border3DStyle.Sunken;
            lblUserAction.Name = "lblUserAction";
            lblUserAction.Size = new Size(16, 17);
            lblUserAction.Text = "   ";
            // 
            // terminalMatrixControl1
            // 
            terminalMatrixControl1.AutoProgramManagement = true;
            terminalMatrixControl1.BorderHeight = 10;
            terminalMatrixControl1.BorderWidth = 10;
            terminalMatrixControl1.ControlOverlayPainter = null;
            terminalMatrixControl1.CurrentCursorColor = 1;
            terminalMatrixControl1.Dock = DockStyle.Fill;
            terminalMatrixControl1.Location = new Point(0, 49);
            terminalMatrixControl1.Name = "terminalMatrixControl1";
            terminalMatrixControl1.RenderingMode = TerminalMatrix.RenderingMode.HighQuality;
            terminalMatrixControl1.Size = new Size(917, 551);
            terminalMatrixControl1.TabIndex = 0;
            terminalMatrixControl1.UnlimitedInput = false;
            terminalMatrixControl1.Use32BitForeground = false;
            terminalMatrixControl1.UseBackground24Bit = false;
            terminalMatrixControl1.TypedLine += terminalMatrixControl1_TypedLine;
            terminalMatrixControl1.InputCompleted += terminalMatrixControl1_InputCompleted;
            terminalMatrixControl1.UserBreak += terminalMatrixControl1_UserBreak;
            terminalMatrixControl1.RequestToggleFullscreen += terminalMatrixControl1_RequestToggleFullscreen;
            terminalMatrixControl1.Tick += terminalMatrixControl1_Tick;
            terminalMatrixControl1.Paint += terminalMatrixControl1_Paint;
            terminalMatrixControl1.Leave += terminalMatrixControl1_Leave;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { optionsToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(46, 20);
            toolsToolStripMenuItem.Text = "&Tools";
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Image = Properties.Resources.settings;
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(180, 22);
            optionsToolStripMenuItem.Text = "Options...";
            optionsToolStripMenuItem.Click += optionsToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // btnOptions
            // 
            btnOptions.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnOptions.Image = Properties.Resources.settings;
            btnOptions.ImageTransparentColor = Color.Magenta;
            btnOptions.Name = "btnOptions";
            btnOptions.Size = new Size(23, 22);
            btnOptions.Text = "Options...";
            btnOptions.Click += btnOptions_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(917, 622);
            Controls.Add(terminalMatrixControl1);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "MainWindow";
            Text = "A-BASIC-Language";
            FormClosing += MainWindow_FormClosing;
            Load += MainWindow_Load;
            Shown += MainWindow_Shown;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStrip toolStrip1;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private TerminalMatrix.TerminalMatrixControl terminalMatrixControl1;
        private ToolStripMenuItem viewToolStripMenuItem;
        public ToolStripMenuItem debugOutputToolStripMenuItem;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel lblUserAction;
        private ToolStripStatusLabel lblCursPos;
        private ToolStripMenuItem resolutionToolStripMenuItem;
        private ToolStripMenuItem highQualityRenderingToolStripMenuItem;
        public ToolStripMenuItem fullscreenToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem onlineHelpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem versionHistoryToolStripMenuItem;
        private ToolStripButton btnDebug;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btnOptions;
    }
}
