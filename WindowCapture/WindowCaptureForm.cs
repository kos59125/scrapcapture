using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;

namespace RecycleBin.WindowCapture
{
	public partial class WindowCaptureForm : Form
	{
		private readonly ToolStripMenuItem NoSelectionItem;

		public WindowCaptureForm()
		{
			InitializeComponent();

			notifyIcon.Icon = this.Icon;
			notifyIcon.Visible = true;
			NoSelectionItem = new ToolStripMenuItem("ウィンドウが見つかりません");
			NoSelectionItem.Enabled = false;
			addWindowsToolStripMenuItem.DropDownItems.Add(NoSelectionItem);
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);

#if !DEBUG
			if (e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing)
			{
				e.Cancel = false;
			}
			else if (MessageBox.Show("本当に終了しますか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
			{
				e.Cancel = true;
			}
#endif
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			notifyIcon.Dispose();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			foreach (ThumbnailPanel panel in Controls)
			{
				if (panel.Bounds.Contains(e.Location))
				{
					if (e.Delta > 0)
					{
						panel.DrawnScale += 0.05F;
					}
					else if (e.Delta < 0 && panel.DrawnScale > 0.05F)
					{
						panel.DrawnScale -= 0.05F;
					}
					break;
				}
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void topMostToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			TopMost = topMostToolStripMenuItem.Checked;
		}

		private void selectWindowsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			ToolStripItemCollection items = addWindowsToolStripMenuItem.DropDownItems;
			items.Clear();
			foreach (ApplicationWindow window in ApplicationWindow.GetApplicationWindows())
			{
				if (window.Handle == this.Handle)
				{
					continue;
				}

				Process process = Process.GetProcessById(window.ProcessId);
				IntPtr handle = window.Handle;
				string title = window.Text;
				Icon icon = window.Icon;

				ToolStripMenuItem windowItem = new ToolStripMenuItem(title);
				windowItem.Click += (eventSender, eventArgs) => CaptureWindow(handle);
				if (icon != null)
				{
					windowItem.Image = icon.ToBitmap();
				}
				items.Add(windowItem);
			}
			if (items.Count == 0)
			{
				items.Add(NoSelectionItem);
			}
		}

		private void CaptureWindow(IntPtr windowHandle)
		{
			ThumbnailPanel panel = new ThumbnailPanel();
			panel.ContextMenuStrip = panelContextMenuStrip;
			Controls.Add(panel);
			panel.SetWindow(windowHandle);
			panel.BringToFront();
		}

		private void adjustWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Region region = new Region();
			region.MakeEmpty();
			foreach (Control panel in Controls)
			{
				region.Union(panel.Bounds);
			}

			using (Graphics g = CreateGraphics())
			{
				RectangleF bounds = region.GetBounds(g);
				ClientSize = new Size((int)Math.Ceiling(bounds.Width), (int)Math.Ceiling(bounds.Height));
				foreach (Control panel in Controls)
				{
					panel.Left -= (int)bounds.X;
					panel.Top -= (int)bounds.Y;
				}
			}
		}

		private void hideWindowBorderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ThumbnailPanel panel = (ThumbnailPanel)panelContextMenuStrip.SourceControl;
			panel.ClientAreaOnly = hideWindowBorderToolStripMenuItem.Checked;
		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Control panel = panelContextMenuStrip.SourceControl;
			Controls.Remove(panel);
			if (!panel.IsDisposed)
			{
				panel.Dispose();
			}
		}

		private void panelContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			ThumbnailPanel panel = (ThumbnailPanel)panelContextMenuStrip.SourceControl;
			hideWindowBorderToolStripMenuItem.Checked = panel.ClientAreaOnly;
		}
	}
}
