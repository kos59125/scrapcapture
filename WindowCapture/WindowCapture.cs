using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RecycleBin.WindowCapture
{
	public partial class WindowCapture : Form
	{
		private readonly ToolStripMenuItem NoSelectionItem;

		public WindowCapture()
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
				if (panel.ClientRectangle.Contains(e.Location))
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
			foreach (Process process in Process.GetProcesses())
			{
				IntPtr windowHandle = process.MainWindowHandle;
				string windowTitle = process.MainWindowTitle;
				if (windowHandle != IntPtr.Zero && windowHandle != this.Handle && windowTitle != string.Empty)
				{
					ToolStripMenuItem windowItem = new ToolStripMenuItem(windowTitle);
					windowItem.Click += (eventSender, eventArgs) => CaptureWindow(windowHandle);
					// アイコンの取得
					SHFILEINFO info = new SHFILEINFO();
					IntPtr hSuccess = SHGetFileInfo(process.MainModule.FileName, 0, ref info, (uint)Marshal.SizeOf(info), 0x101 /* 小サイズのアイコン */);
					if (hSuccess != IntPtr.Zero)
					{
						windowItem.Image = Icon.FromHandle(info.hIcon).ToBitmap();
					}
					items.Add(windowItem);
				}
			}
			if (items.Count == 0)
			{
				items.Add(NoSelectionItem);
			}
		}

		private void CaptureWindow(IntPtr windowHandle)
		{
			ThumbnailPanel panel = new ThumbnailPanel();
			Controls.Add(panel);
			panel.SetWindow(windowHandle);
		}

		[DllImport("shell32.dll")]
		private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

		private struct SHFILEINFO
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

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
	}
}
