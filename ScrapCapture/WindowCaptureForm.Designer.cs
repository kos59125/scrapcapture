namespace RecycleBin.ScrapCapture
{
	partial class WindowCaptureForm
	{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナで生成されたコード

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.adjustWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.topMostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.panelContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.hideWindowBorderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panelRegionSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveImageFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.fixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip.SuspendLayout();
			this.panelContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addWindowsToolStripMenuItem,
            this.adjustWindowToolStripMenuItem,
            this.saveImageToolStripMenuItem,
            this.toolStripSeparator2,
            this.topMostToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(386, 126);
			// 
			// addWindowsToolStripMenuItem
			// 
			this.addWindowsToolStripMenuItem.Name = "addWindowsToolStripMenuItem";
			this.addWindowsToolStripMenuItem.Size = new System.Drawing.Size(385, 22);
			this.addWindowsToolStripMenuItem.Text = "取り込むウィンドウを追加";
			this.addWindowsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.selectWindowsToolStripMenuItem_DropDownOpening);
			// 
			// adjustWindowToolStripMenuItem
			// 
			this.adjustWindowToolStripMenuItem.Name = "adjustWindowToolStripMenuItem";
			this.adjustWindowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.adjustWindowToolStripMenuItem.Size = new System.Drawing.Size(385, 22);
			this.adjustWindowToolStripMenuItem.Text = "ウィンドウサイズをクライアントに合わせる (&A)";
			this.adjustWindowToolStripMenuItem.Click += new System.EventHandler(this.adjustWindowToolStripMenuItem_Click);
			// 
			// saveImageToolStripMenuItem
			// 
			this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
			this.saveImageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(385, 22);
			this.saveImageToolStripMenuItem.Text = "画像に保存する (&S)";
			this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(382, 6);
			// 
			// topMostToolStripMenuItem
			// 
			this.topMostToolStripMenuItem.CheckOnClick = true;
			this.topMostToolStripMenuItem.Name = "topMostToolStripMenuItem";
			this.topMostToolStripMenuItem.Size = new System.Drawing.Size(385, 22);
			this.topMostToolStripMenuItem.Text = "ウィンドウを前面に表示する";
			this.topMostToolStripMenuItem.CheckedChanged += new System.EventHandler(this.topMostToolStripMenuItem_CheckedChanged);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(382, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(385, 22);
			this.exitToolStripMenuItem.Text = "終了 (&X)";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
			this.notifyIcon.Text = "notifyIcon";
			this.notifyIcon.Visible = true;
			// 
			// panelContextMenuStrip
			// 
			this.panelContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideWindowBorderToolStripMenuItem,
            this.panelRegionSelectionToolStripMenuItem,
            this.clipboardToolStripMenuItem,
            this.fixToolStripMenuItem,
            this.toolStripSeparator3,
            this.removeToolStripMenuItem});
			this.panelContextMenuStrip.Name = "contextMenuStrip";
			this.panelContextMenuStrip.Size = new System.Drawing.Size(255, 120);
			this.panelContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.panelContextMenuStrip_Opening);
			// 
			// hideWindowBorderToolStripMenuItem
			// 
			this.hideWindowBorderToolStripMenuItem.CheckOnClick = true;
			this.hideWindowBorderToolStripMenuItem.Name = "hideWindowBorderToolStripMenuItem";
			this.hideWindowBorderToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.hideWindowBorderToolStripMenuItem.Text = "ウィンドウ枠を表示しない (&H)";
			this.hideWindowBorderToolStripMenuItem.Click += new System.EventHandler(this.hideWindowBorderToolStripMenuItem_Click);
			// 
			// panelRegionSelectionToolStripMenuItem
			// 
			this.panelRegionSelectionToolStripMenuItem.Name = "panelRegionSelectionToolStripMenuItem";
			this.panelRegionSelectionToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.panelRegionSelectionToolStripMenuItem.Text = "描画領域を選択 (&R)";
			this.panelRegionSelectionToolStripMenuItem.Click += new System.EventHandler(this.panelRegionSelectionToolStripMenuItem_Click);
			// 
			// clipboardToolStripMenuItem
			// 
			this.clipboardToolStripMenuItem.Name = "clipboardToolStripMenuItem";
			this.clipboardToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.clipboardToolStripMenuItem.Text = "クリップボードにコピーする (&C)";
			this.clipboardToolStripMenuItem.Click += new System.EventHandler(this.clipboardToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(251, 6);
			// 
			// removeToolStripMenuItem
			// 
			this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
			this.removeToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.removeToolStripMenuItem.Text = "削除 (&X)";
			this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			// 
			// saveImageFileDialog
			// 
			this.saveImageFileDialog.DefaultExt = "png";
			this.saveImageFileDialog.Filter = "PNG ファイル (*.png)|*.png|JPEG ファイル (*.jpg)|*.jpg|Bitmap ファイル (*.bmp)|*.bmp";
			// 
			// fixToolStripMenuItem
			// 
			this.fixToolStripMenuItem.CheckOnClick = true;
			this.fixToolStripMenuItem.Name = "fixToolStripMenuItem";
			this.fixToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.fixToolStripMenuItem.Text = "toolStripMenuItem1";
			this.fixToolStripMenuItem.CheckedChanged += new System.EventHandler(this.fixToolStripMenuItem_CheckedChanged);
			// 
			// WindowCaptureForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(464, 323);
			this.ContextMenuStrip = this.contextMenuStrip;
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Name = "WindowCaptureForm";
			this.Text = "Scrap Capture";
			this.contextMenuStrip.ResumeLayout(false);
			this.panelContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem topMostToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem addWindowsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.ToolStripMenuItem adjustWindowToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip panelContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem hideWindowBorderToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveImageToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog saveImageFileDialog;
		private System.Windows.Forms.ToolStripMenuItem panelRegionSelectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clipboardToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fixToolStripMenuItem;
	}
}

