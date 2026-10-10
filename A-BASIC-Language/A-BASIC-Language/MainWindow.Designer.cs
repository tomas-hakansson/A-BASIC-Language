using System.Drawing;
using System.Windows.Forms;

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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openFrom101BASICComputerGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullscreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highQualityRenderingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.programToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.versionHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnDebug = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnOptions = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblCursPos = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUserAction = new System.Windows.Forms.ToolStripStatusLabel();
            this.terminalMatrixControl1 = new TerminalMatrixNetFramework.TerminalMatrixControl();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.programToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(786, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openFrom101BASICComputerGamesToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(293, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(290, 6);
            // 
            // openFrom101BASICComputerGamesToolStripMenuItem
            // 
            this.openFrom101BASICComputerGamesToolStripMenuItem.Name = "openFrom101BASICComputerGamesToolStripMenuItem";
            this.openFrom101BASICComputerGamesToolStripMenuItem.Size = new System.Drawing.Size(293, 22);
            this.openFrom101BASICComputerGamesToolStripMenuItem.Text = "Open from 101 BASIC Computer Games...";
            this.openFrom101BASICComputerGamesToolStripMenuItem.Click += new System.EventHandler(this.openFrom101BASICComputerGamesToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullscreenToolStripMenuItem,
            this.debugOutputToolStripMenuItem,
            this.resolutionToolStripMenuItem,
            this.highQualityRenderingToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // fullscreenToolStripMenuItem
            // 
            this.fullscreenToolStripMenuItem.Name = "fullscreenToolStripMenuItem";
            this.fullscreenToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.fullscreenToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.fullscreenToolStripMenuItem.Text = "Fullscreen";
            this.fullscreenToolStripMenuItem.Click += new System.EventHandler(this.fullscreenToolStripMenuItem_Click);
            // 
            // debugOutputToolStripMenuItem
            // 
            this.debugOutputToolStripMenuItem.Image = global::A_BASIC_Language.Properties.Resources.debug;
            this.debugOutputToolStripMenuItem.Name = "debugOutputToolStripMenuItem";
            this.debugOutputToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.debugOutputToolStripMenuItem.Text = "Debug output";
            this.debugOutputToolStripMenuItem.Click += new System.EventHandler(this.debugOutputToolStripMenuItem_Click);
            // 
            // resolutionToolStripMenuItem
            // 
            this.resolutionToolStripMenuItem.Name = "resolutionToolStripMenuItem";
            this.resolutionToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.resolutionToolStripMenuItem.Text = "Resolution";
            // 
            // highQualityRenderingToolStripMenuItem
            // 
            this.highQualityRenderingToolStripMenuItem.Checked = true;
            this.highQualityRenderingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.highQualityRenderingToolStripMenuItem.Name = "highQualityRenderingToolStripMenuItem";
            this.highQualityRenderingToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.highQualityRenderingToolStripMenuItem.Text = "High quality rendering";
            this.highQualityRenderingToolStripMenuItem.Click += new System.EventHandler(this.highQualityRenderingToolStripMenuItem_Click);
            // 
            // programToolStripMenuItem
            // 
            this.programToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem});
            this.programToolStripMenuItem.Name = "programToolStripMenuItem";
            this.programToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.programToolStripMenuItem.Text = "Program";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Image = global::A_BASIC_Language.Properties.Resources.PlayHS;
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.runToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Image = global::A_BASIC_Language.Properties.Resources.settings;
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.optionsToolStripMenuItem.Text = "Options...";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.onlineHelpToolStripMenuItem,
            this.versionHistoryToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // onlineHelpToolStripMenuItem
            // 
            this.onlineHelpToolStripMenuItem.Name = "onlineHelpToolStripMenuItem";
            this.onlineHelpToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.onlineHelpToolStripMenuItem.Text = "Online help...";
            this.onlineHelpToolStripMenuItem.Click += new System.EventHandler(this.onlineHelpToolStripMenuItem_Click);
            // 
            // versionHistoryToolStripMenuItem
            // 
            this.versionHistoryToolStripMenuItem.Name = "versionHistoryToolStripMenuItem";
            this.versionHistoryToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.versionHistoryToolStripMenuItem.Text = "Version history...";
            this.versionHistoryToolStripMenuItem.Click += new System.EventHandler(this.versionHistoryToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDebug,
            this.toolStripSeparator2,
            this.btnPlay,
            this.toolStripSeparator1,
            this.btnOptions});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(786, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnDebug
            // 
            this.btnDebug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDebug.Image = global::A_BASIC_Language.Properties.Resources.debug;
            this.btnDebug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDebug.Name = "btnDebug";
            this.btnDebug.Size = new System.Drawing.Size(23, 22);
            this.btnDebug.Text = "Debug output";
            this.btnDebug.Click += new System.EventHandler(this.btnDebug_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnPlay
            // 
            this.btnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPlay.Image = global::A_BASIC_Language.Properties.Resources.PlayHS;
            this.btnPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(23, 22);
            this.btnPlay.Text = "Run";
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnOptions
            // 
            this.btnOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOptions.Image = global::A_BASIC_Language.Properties.Resources.settings;
            this.btnOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(23, 22);
            this.btnOptions.Text = "Options...";
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblCursPos,
            this.toolStripStatusLabel1,
            this.lblUserAction});
            this.statusStrip1.Location = new System.Drawing.Point(0, 517);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
            this.statusStrip1.Size = new System.Drawing.Size(786, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblCursPos
            // 
            this.lblCursPos.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.lblCursPos.Name = "lblCursPos";
            this.lblCursPos.Size = new System.Drawing.Size(25, 17);
            this.lblCursPos.Text = "0, 0";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(69, 17);
            this.toolStripStatusLabel1.Text = "User action:";
            // 
            // lblUserAction
            // 
            this.lblUserAction.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.lblUserAction.Name = "lblUserAction";
            this.lblUserAction.Size = new System.Drawing.Size(16, 17);
            this.lblUserAction.Text = "   ";
            // 
            // terminalMatrixControl1
            // 
            this.terminalMatrixControl1.AutoProgramManagement = true;
            this.terminalMatrixControl1.BorderHeight = 10;
            this.terminalMatrixControl1.BorderWidth = 10;
            this.terminalMatrixControl1.ControlOverlayPainter = null;
            this.terminalMatrixControl1.CurrentCursorColor = ((byte)(1));
            this.terminalMatrixControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.terminalMatrixControl1.Location = new System.Drawing.Point(0, 49);
            this.terminalMatrixControl1.Name = "terminalMatrixControl1";
            this.terminalMatrixControl1.RenderingMode = TerminalMatrixNetFramework.RenderingMode.HighQuality;
            this.terminalMatrixControl1.Size = new System.Drawing.Size(786, 468);
            this.terminalMatrixControl1.TabIndex = 0;
            this.terminalMatrixControl1.UnlimitedInput = false;
            this.terminalMatrixControl1.Use32BitForeground = false;
            this.terminalMatrixControl1.UseBackground24Bit = false;
            this.terminalMatrixControl1.TypedLine += new TerminalMatrixNetFramework.Events.TypedLineDelegate(this.terminalMatrixControl1_TypedLine);
            this.terminalMatrixControl1.InputCompleted += new TerminalMatrixNetFramework.Events.InputCompletedDelegate(this.terminalMatrixControl1_InputCompleted);
            this.terminalMatrixControl1.UserBreak += new TerminalMatrixNetFramework.Events.UserBreakDelegate(this.terminalMatrixControl1_UserBreak);
            this.terminalMatrixControl1.RequestToggleFullscreen += new TerminalMatrixNetFramework.Events.RequestToggleFullscreenDelegate(this.terminalMatrixControl1_RequestToggleFullscreen);
            this.terminalMatrixControl1.Tick += new TerminalMatrixNetFramework.Events.TickDelegate(this.terminalMatrixControl1_Tick);
            this.terminalMatrixControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.terminalMatrixControl1_Paint);
            this.terminalMatrixControl1.Leave += new System.EventHandler(this.terminalMatrixControl1_Leave);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 539);
            this.Controls.Add(this.terminalMatrixControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "A-BASIC-Language";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStrip toolStrip1;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private TerminalMatrixNetFramework.TerminalMatrixControl terminalMatrixControl1;
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
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem openFrom101BASICComputerGamesToolStripMenuItem;
        private ToolStripMenuItem programToolStripMenuItem;
        private ToolStripMenuItem runToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton btnPlay;
    }
}
