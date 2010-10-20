using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;

namespace RecycleBin.ScrapCapture
{
	public partial class WindowCaptureForm : Form
	{
		private readonly ToolStripMenuItem NoSelectionItem;
		private Point mouseLocation;

		public WindowCaptureForm()
		{
			InitializeComponent();

			notifyIcon.Icon = this.Icon;
			notifyIcon.Visible = true;
			NoSelectionItem = new ToolStripMenuItem("ウィンドウが見つかりません");
			NoSelectionItem.Enabled = false;
			addWindowsToolStripMenuItem.DropDownItems.Add(NoSelectionItem);

#if DEBUG
			ToolStripMenuItem dwmCompositionEnabledItem = new ToolStripMenuItem("DMW コンポジションを無効にする");
			dwmCompositionEnabledItem.CheckOnClick = true;
			dwmCompositionEnabledItem.CheckedChanged += (sender, e) => DesktopWindowManager.DwmEnableComposition(!dwmCompositionEnabledItem.Checked);
			ContextMenuStrip.Items.Add(new ToolStripSeparator());
			ContextMenuStrip.Items.Add(dwmCompositionEnabledItem);
#endif
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			ClientSize = new Size(800, 600);
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
			RegisterMouseEventHandler(panel);
			Controls.Add(panel);
			panel.SetWindow(windowHandle);
			panel.BringToFront();
		}

		private void RegisterMouseEventHandler(ThumbnailPanel panel)
		{
			panel.MouseDown += (sender, e) =>
			{
				if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
				{
					mouseLocation = e.Location;
					panel.BringToFront();
				}
			};
			panel.MouseMove += (sender, e) =>
			{
				if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
				{
					panel.Location = new Point(panel.Location.X + e.X - mouseLocation.X, panel.Location.Y + e.Y - mouseLocation.Y);
				}
			};
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

		private void clipboardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ThumbnailPanel panel = (ThumbnailPanel)panelContextMenuStrip.SourceControl;
			using (Image image = panel.CaptureCurrentImage())
			{
				Clipboard.SetImage(image);
			}
		}

		private void fixToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			ThumbnailPanel panel = (ThumbnailPanel)panelContextMenuStrip.SourceControl;
			panel.IsFixed = fixToolStripMenuItem.Checked;
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
			fixToolStripMenuItem.Checked = panel.IsFixed;
		}

		private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (saveImageFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				string path = saveImageFileDialog.FileName;

				// DrawToBitmap は DMW が描画したウィンドウをキャプチャしてくれないので
				// 対象とする領域のみスクリーンキャプチャを行う。
				// あくまでスクリーンキャプチャなのでウィンドウより前面に別のウィンドウがあればそれが前面に描画されてしまう。
				// また，スクリーンからはみ出た領域についてはキャプチャできない。
				Rectangle visibleBounds = GetVisibleBounds(ClientRectangle);
				if (visibleBounds.IsEmpty)
				{
					MessageBox.Show("スクリーンに表示された領域が存在しないためキャプチャできません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					BringToFront();
					using (Bitmap bitmap = new Bitmap(visibleBounds.Width, visibleBounds.Height))
					using (Graphics g = Graphics.FromImage(bitmap))
					{
						g.CopyFromScreen(visibleBounds.X, visibleBounds.Y, 0, 0, bitmap.Size);
						bitmap.Save(path, GetImageFormatFromExtension(path));
					}
				}
			}
		}

		private static ImageFormat GetImageFormatFromExtension(string path)
		{
			switch (Path.GetExtension(path).ToLower())
			{
				case ".png":
					return ImageFormat.Png;
				case ".jpg":
					return ImageFormat.Jpeg;
				case ".bmp":
					return ImageFormat.Bmp;
				default:
					return ImageFormat.Png;
			}
		}

		private Rectangle GetVisibleBounds(Rectangle rectangle)
		{
			using (Graphics g = CreateGraphics())
			{
				Region visibleRegion = new Region();
				visibleRegion.MakeEmpty();
				foreach (Screen screen in Screen.AllScreens)
				{
					visibleRegion.Union(screen.Bounds);
				}
				visibleRegion.Intersect(RectangleToScreen(rectangle));

				RectangleF savedRegion = visibleRegion.GetBounds(g);
				return new Rectangle(
					(int)savedRegion.X,
					(int)savedRegion.Y,
					(int)Math.Ceiling(savedRegion.Width),
					(int)Math.Ceiling(savedRegion.Height)
				);
			}
		}

		private void panelRegionSelectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ThumbnailPanel panel = (ThumbnailPanel)panelContextMenuStrip.SourceControl;
			RegionSelector selector = new RegionSelector();
			selector.Load += (eventSender, eventArgs) =>
			{
				selector.SetWindow(panel.GetWindow(), panel.ClientAreaOnly);
				selector.DrawnRegion = panel.DrawnRegion;
			};
			if (selector.ShowDialog(this) == DialogResult.OK)
			{
				panel.DrawnRegion = selector.DrawnRegion;
			}
		}
	}
}
