using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace RecycleBin.WindowCapture
{
	public class ThumbnailPanel : Control
	{
		private IntPtr thumbnail;
		private IntPtr windowHandle;
		private Thread observer;
		private Point mousePoint;
		private Rectangle drawnRegion;
		private float scale;
		private bool clientAreaOnly;
		private ContextMenuStrip contextMenuStrip;
		private IContainer components;
		private ToolStripMenuItem removeToolStripMenuItem;
		private ToolStripMenuItem hideWindowBorderToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;

		public ThumbnailPanel()
		{
			InitializeComponent();
			scale = 1;
		}

		public Size SourceSize
		{
			get
			{
				return DesktopWindowManager.QueryThumbnailSourceSize(thumbnail);
			}
		}

		public Rectangle DrawnRegion
		{
			get
			{
				return drawnRegion;
			}
			set
			{
				if (!new Rectangle(Point.Empty, SourceSize).Contains(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}

				drawnRegion = value;
				Size = DrawnSize;
				UpdateThubmnail();
			}
		}

		public float DrawnScale
		{
			get
			{
				return scale;
			}
			set
			{
				if (scale <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				scale = value;
				Size = DrawnSize;
				UpdateThubmnail();
			}
		}

		public bool ClientAreaOnly
		{
			get
			{
				return clientAreaOnly;
			}
			set
			{
				clientAreaOnly = value;
				Size = DrawnSize;
				UpdateThubmnail();
			}
		}

		public Size DrawnSize
		{
			get
			{
				return new Size((int)Math.Ceiling(DrawnRegion.Width * DrawnScale), (int)Math.Ceiling(DrawnRegion.Height * DrawnScale));
			}
		}

		public void SetWindow(IntPtr windowHandle)
		{
			this.windowHandle = windowHandle;

			if (observer != null)
			{
				observer.Abort();
			}
			if (thumbnail != IntPtr.Zero)
			{
				DesktopWindowManager.Unregister(thumbnail);
				thumbnail = IntPtr.Zero;
			}

			thumbnail = DesktopWindowManager.Register(FindForm(), windowHandle);
			observer = new Thread(new ThreadStart(
				() =>
				{
					while (Process.GetProcesses().Any(process => process.MainWindowHandle == windowHandle))
					{
						Thread.Sleep(100);
					}
					Dispose(true);
				}
			));
			observer.Start();

			ResetDrawnRegion();
			UpdateThubmnail();
		}

		public void ResetDrawnRegion()
		{
			if (thumbnail != IntPtr.Zero)
			{
				DrawnRegion = new Rectangle(Point.Empty, SourceSize);
			}
		}

		public new void BringToFront()
		{
			base.BringToFront();
			if (thumbnail != IntPtr.Zero)
			{
				DesktopWindowManager.Unregister(thumbnail);
				thumbnail = IntPtr.Zero;
			}
			thumbnail = DesktopWindowManager.Register(FindForm(), windowHandle);
			UpdateThubmnail();
		}

		private void UpdateThubmnail()
		{
			if (thumbnail != IntPtr.Zero)
			{
				DWM_THUMBNAIL_PROPERTIES properties = new DWM_THUMBNAIL_PROPERTIES()
				{
					dwFlags = DWM_TNP.DWM_TNP_OPACITY | DWM_TNP.DWM_TNP_RECTDESTINATION | DWM_TNP.DWM_TNP_RECTSOURCE | DWM_TNP.DWM_TNP_SOURCECLIENTAREAONLY | DWM_TNP.DWM_TNP_VISIBLE,
					fSourceClientAreaOnly = ClientAreaOnly,
					fVisible = Visible,
					opacity = Byte.MaxValue,
					rcDestination = new RECT(Location, Size),
					rcSource = new RECT(DrawnRegion)
				};
				if (!DesktopWindowManager.Update(thumbnail, ref properties))
				{
					MessageBox.Show("サムネイルの更新に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private delegate void DisposeCallback(bool disposing);
		protected override void Dispose(bool disposing)
		{
			if (InvokeRequired)
			{
				DisposeCallback callback = new DisposeCallback(Dispose);
				Invoke(callback, disposing);
			}
			else
			{
				if (observer != null && observer.IsAlive)
				{
					observer.Abort();
				}
				if (thumbnail != IntPtr.Zero)
				{
					DesktopWindowManager.Unregister(thumbnail);
					thumbnail = IntPtr.Zero;
				}
				base.Dispose(disposing);
			}
		}

		protected override void OnLocationChanged(EventArgs e)
		{
			base.OnLocationChanged(e);

			UpdateThubmnail();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			UpdateThubmnail();
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			UpdateThubmnail();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				mousePoint = e.Location;
				this.BringToFront();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				Location = new Point(this.Location.X + e.X - mousePoint.X, this.Location.Y + e.Y - mousePoint.Y);
			}
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.hideWindowBorderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideWindowBorderToolStripMenuItem,
            this.toolStripSeparator1,
            this.removeToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(244, 54);
			// 
			// hideWindowBorderToolStripMenuItem
			// 
			this.hideWindowBorderToolStripMenuItem.CheckOnClick = true;
			this.hideWindowBorderToolStripMenuItem.Name = "hideWindowBorderToolStripMenuItem";
			this.hideWindowBorderToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			this.hideWindowBorderToolStripMenuItem.Text = "ウィンドウ枠を表示しない (&H)";
			this.hideWindowBorderToolStripMenuItem.CheckedChanged += new System.EventHandler(this.hideWindowBorderToolStripMenuItem_CheckedChanged);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(240, 6);
			// 
			// removeToolStripMenuItem
			// 
			this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
			this.removeToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			this.removeToolStripMenuItem.Text = "削除 (&X)";
			this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			// 
			// ThumbnailPanel
			// 
			this.ContextMenuStrip = this.contextMenuStrip;
			this.contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Dispose(true);
		}

		private void hideWindowBorderToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			ClientAreaOnly = hideWindowBorderToolStripMenuItem.Checked;
		}
	}
}
