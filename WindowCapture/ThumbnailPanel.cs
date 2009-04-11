using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

		public ThumbnailPanel()
		{
			scale = 1;
#if DEBUG
			BackColor = Color.Black;
#endif
		}

		public Size SourceSize
		{
			get
			{
				RECT rect;
				if (ClientAreaOnly)
				{
					GetClientRect(windowHandle, out rect);
				}
				else
				{
					GetWindowRect(windowHandle, out rect);
				}
				return rect.Size;
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
				Size = SourceSize;
				ResetDrawnRegion();
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
						Application.DoEvents();
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

		[DllImport("user32.dll")]
		private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
		[DllImport("user32.dll")]
		private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
	}
}
