namespace RecycleBin.WindowCapture
{
	partial class RegionSelector
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
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.regionToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.okToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.thumbnailPanel = new RecycleBin.WindowCapture.ThumbnailPanel();
			this.statusStrip.SuspendLayout();
			this.contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.regionToolStripStatusLabel});
			this.statusStrip.Location = new System.Drawing.Point(0, 241);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(284, 22);
			this.statusStrip.TabIndex = 0;
			this.statusStrip.Text = "statusStrip1";
			// 
			// regionToolStripStatusLabel
			// 
			this.regionToolStripStatusLabel.Name = "regionToolStripStatusLabel";
			this.regionToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.okToolStripMenuItem,
            this.cancelToolStripMenuItem,
            this.resetToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(159, 70);
			// 
			// resetToolStripMenuItem
			// 
			this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
			this.resetToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.resetToolStripMenuItem.Text = "リセット (&R)";
			this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
			// 
			// okToolStripMenuItem
			// 
			this.okToolStripMenuItem.Name = "okToolStripMenuItem";
			this.okToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.okToolStripMenuItem.Text = "&OK";
			this.okToolStripMenuItem.Click += new System.EventHandler(this.okToolStripMenuItem_Click);
			// 
			// cancelToolStripMenuItem
			// 
			this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
			this.cancelToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.cancelToolStripMenuItem.Text = "キャンセル (&C)";
			this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
			// 
			// thumbnailPanel
			// 
			this.thumbnailPanel.BackColor = System.Drawing.Color.White;
			this.thumbnailPanel.ClientAreaOnly = false;
			this.thumbnailPanel.ContextMenuStrip = this.contextMenuStrip;
			this.thumbnailPanel.DrawnRegion = new System.Drawing.Rectangle(0, 0, 0, 0);
			this.thumbnailPanel.DrawnScale = 1F;
			this.thumbnailPanel.Location = new System.Drawing.Point(0, 0);
			this.thumbnailPanel.Name = "thumbnailPanel";
			this.thumbnailPanel.Size = new System.Drawing.Size(284, 241);
			this.thumbnailPanel.TabIndex = 1;
			this.thumbnailPanel.Text = "thumbnailPanel";
			this.thumbnailPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.thumbnailPanel_MouseMove);
			this.thumbnailPanel.SourceWindowChanged += new System.EventHandler(this.thumbnailPanel_SourceWindowChanged);
			this.thumbnailPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.thumbnailPanel_MouseDown);
			this.thumbnailPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.thumbnailPanel_MouseUp);
			this.thumbnailPanel.SourceSizeChanged += new System.EventHandler(this.thumbnailPanel_SourceSizeChanged);
			// 
			// RegionSelector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 263);
			this.Controls.Add(this.thumbnailPanel);
			this.Controls.Add(this.statusStrip);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "RegionSelector";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "領域を選択";
			this.TopMost = true;
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel regionToolStripStatusLabel;
		private ThumbnailPanel thumbnailPanel;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem okToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cancelToolStripMenuItem;
	}
}