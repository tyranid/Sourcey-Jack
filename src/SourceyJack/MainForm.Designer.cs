namespace SourceyJack
{
    partial class MainForm
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
            System.Windows.Forms.Label labelExe;
            System.Windows.Forms.Label labelCmdLine;
            System.Windows.Forms.ColumnHeader columnHeaderPid;
            System.Windows.Forms.ColumnHeader columnHeaderName;
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageConfig = new System.Windows.Forms.TabPage();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.lablePort = new System.Windows.Forms.Label();
            this.labelServer = new System.Windows.Forms.Label();
            this.tabPageCreate = new System.Windows.Forms.TabPage();
            this.btnStart = new System.Windows.Forms.Button();
            this.textBoxCmdLine = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.textBoxExe = new System.Windows.Forms.TextBox();
            this.tabPageInject = new System.Windows.Forms.TabPage();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnInject = new System.Windows.Forms.Button();
            this.listViewProcesses = new System.Windows.Forms.ListView();
            labelExe = new System.Windows.Forms.Label();
            labelCmdLine = new System.Windows.Forms.Label();
            columnHeaderPid = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.tabPageCreate.SuspendLayout();
            this.tabPageInject.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelExe
            // 
            labelExe.AutoSize = true;
            labelExe.Location = new System.Drawing.Point(8, 6);
            labelExe.Name = "labelExe";
            labelExe.Size = new System.Drawing.Size(63, 13);
            labelExe.TabIndex = 0;
            labelExe.Text = "Executable:";
            // 
            // labelCmdLine
            // 
            labelCmdLine.AutoSize = true;
            labelCmdLine.Location = new System.Drawing.Point(8, 32);
            labelCmdLine.Name = "labelCmdLine";
            labelCmdLine.Size = new System.Drawing.Size(80, 13);
            labelCmdLine.TabIndex = 4;
            labelCmdLine.Text = "Command Line:";
            // 
            // columnHeaderPid
            // 
            columnHeaderPid.Text = "PID";
            // 
            // columnHeaderName
            // 
            columnHeaderName.Text = "Name";
            columnHeaderName.Width = 466;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(579, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageConfig);
            this.tabControl.Controls.Add(this.tabPageCreate);
            this.tabControl.Controls.Add(this.tabPageInject);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(579, 372);
            this.tabControl.TabIndex = 1;
            // 
            // tabPageConfig
            // 
            this.tabPageConfig.Controls.Add(this.numPort);
            this.tabPageConfig.Controls.Add(this.textBoxServer);
            this.tabPageConfig.Controls.Add(this.lablePort);
            this.tabPageConfig.Controls.Add(this.labelServer);
            this.tabPageConfig.Location = new System.Drawing.Point(4, 22);
            this.tabPageConfig.Name = "tabPageConfig";
            this.tabPageConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConfig.Size = new System.Drawing.Size(571, 346);
            this.tabPageConfig.TabIndex = 2;
            this.tabPageConfig.Text = "Config";
            this.tabPageConfig.UseVisualStyleBackColor = true;
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(55, 39);
            this.numPort.Maximum = new decimal(new int[] {
            64435,
            0,
            0,
            0});
            this.numPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(120, 20);
            this.numPort.TabIndex = 3;
            this.numPort.Value = new decimal(new int[] {
            1080,
            0,
            0,
            0});
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(55, 13);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(231, 20);
            this.textBoxServer.TabIndex = 2;
            this.textBoxServer.Text = "127.0.0.1";
            // 
            // lablePort
            // 
            this.lablePort.AutoSize = true;
            this.lablePort.Location = new System.Drawing.Point(8, 44);
            this.lablePort.Name = "lablePort";
            this.lablePort.Size = new System.Drawing.Size(26, 13);
            this.lablePort.TabIndex = 1;
            this.lablePort.Text = "Port";
            // 
            // labelServer
            // 
            this.labelServer.AutoSize = true;
            this.labelServer.Location = new System.Drawing.Point(8, 16);
            this.labelServer.Name = "labelServer";
            this.labelServer.Size = new System.Drawing.Size(41, 13);
            this.labelServer.TabIndex = 0;
            this.labelServer.Text = "Server:";
            // 
            // tabPageCreate
            // 
            this.tabPageCreate.Controls.Add(this.btnStart);
            this.tabPageCreate.Controls.Add(labelCmdLine);
            this.tabPageCreate.Controls.Add(this.textBoxCmdLine);
            this.tabPageCreate.Controls.Add(this.btnBrowse);
            this.tabPageCreate.Controls.Add(this.textBoxExe);
            this.tabPageCreate.Controls.Add(labelExe);
            this.tabPageCreate.Location = new System.Drawing.Point(4, 22);
            this.tabPageCreate.Name = "tabPageCreate";
            this.tabPageCreate.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCreate.Size = new System.Drawing.Size(571, 346);
            this.tabPageCreate.TabIndex = 0;
            this.tabPageCreate.Text = "Create";
            this.tabPageCreate.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(13, 63);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // textBoxCmdLine
            // 
            this.textBoxCmdLine.Location = new System.Drawing.Point(94, 29);
            this.textBoxCmdLine.Name = "textBoxCmdLine";
            this.textBoxCmdLine.Size = new System.Drawing.Size(461, 20);
            this.textBoxCmdLine.TabIndex = 3;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(480, 1);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // textBoxExe
            // 
            this.textBoxExe.Location = new System.Drawing.Point(94, 3);
            this.textBoxExe.Name = "textBoxExe";
            this.textBoxExe.Size = new System.Drawing.Size(380, 20);
            this.textBoxExe.TabIndex = 1;
            // 
            // tabPageInject
            // 
            this.tabPageInject.Controls.Add(this.btnRefresh);
            this.tabPageInject.Controls.Add(this.btnInject);
            this.tabPageInject.Controls.Add(this.listViewProcesses);
            this.tabPageInject.Location = new System.Drawing.Point(4, 22);
            this.tabPageInject.Name = "tabPageInject";
            this.tabPageInject.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInject.Size = new System.Drawing.Size(571, 346);
            this.tabPageInject.TabIndex = 1;
            this.tabPageInject.Text = "Inject";
            this.tabPageInject.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(488, 308);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnInject
            // 
            this.btnInject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInject.Location = new System.Drawing.Point(8, 308);
            this.btnInject.Name = "btnInject";
            this.btnInject.Size = new System.Drawing.Size(75, 23);
            this.btnInject.TabIndex = 1;
            this.btnInject.Text = "Inject";
            this.btnInject.UseVisualStyleBackColor = true;
            this.btnInject.Click += new System.EventHandler(this.btnInject_Click);
            // 
            // listViewProcesses
            // 
            this.listViewProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewProcesses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeaderPid,
            columnHeaderName});
            this.listViewProcesses.FullRowSelect = true;
            this.listViewProcesses.Location = new System.Drawing.Point(6, 6);
            this.listViewProcesses.MultiSelect = false;
            this.listViewProcesses.Name = "listViewProcesses";
            this.listViewProcesses.Size = new System.Drawing.Size(557, 296);
            this.listViewProcesses.TabIndex = 0;
            this.listViewProcesses.UseCompatibleStateImageBehavior = false;
            this.listViewProcesses.View = System.Windows.Forms.View.Details;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 396);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Sourcey Jack";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageConfig.ResumeLayout(false);
            this.tabPageConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.tabPageCreate.ResumeLayout(false);
            this.tabPageCreate.PerformLayout();
            this.tabPageInject.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageConfig;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.Label lablePort;
        private System.Windows.Forms.Label labelServer;
        private System.Windows.Forms.TabPage tabPageCreate;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox textBoxCmdLine;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox textBoxExe;
        private System.Windows.Forms.TabPage tabPageInject;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnInject;
        private System.Windows.Forms.ListView listViewProcesses;
    }
}

