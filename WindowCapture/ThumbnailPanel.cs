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
		public event EventHandler SourceSizeChanged;
		public event EventHandler SourceWindowChanged;

		private IntPtr thumbnail;
		private IntPtr windowHandle;
		private Point mousePoint;
		private Rectangle drawnRegion;
		private float scale;
		private BackgroundWorker windowObserver;
		private bool clientAreaOnly;

		public ThumbnailPanel()
		{
			InitializeComponent();
			scale = 1;
#if DEBUG
			BackColor = Color.Black;
#endif
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
				OnSourceSizeChanged(EventArgs.Empty);
			}
		}

		public Size DrawnSize
		{
			get
			{
				return new Size((int)Math.Ceiling(DrawnRegion.Width * DrawnScale), (int)Math.Ceiling(DrawnRegion.Height * DrawnScale));
			}
		}

		public bool IsWindowSet
		{
			get
			{
				return windowHandle != IntPtr.Zero;
			}
		}

		public void SetWindow(IntPtr windowHandle)
		{
			UnsetWindow();

			if (windowHandle != IntPtr.Zero && DesktopWindowManager.IsCompositionEnabled)
			{
				this.windowHandle = windowHandle;

				thumbnail = DesktopWindowManager.Register(FindForm(), windowHandle);
				windowObserver.RunWorkerAsync();

				OnSourceWindowChanged(EventArgs.Empty);
			}
		}

		public void UnsetWindow()
		{
			windowHandle = IntPtr.Zero;
			if (windowObserver.IsBusy)
			{
				windowObserver.CancelAsync();
			}
			if (thumbnail != IntPtr.Zero)
			{
				DesktopWindowManager.Unregister(thumbnail);
				thumbnail = IntPtr.Zero;
			}

			OnSourceWindowChanged(EventArgs.Empty);
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
					if (MessageBox.Show("サムネイルの更新に失敗しました。", "エラー", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) != DialogResult.Retry)
					{
						UnsetWindow();
					}
				}
			}
		}

		protected virtual void OnSourceSizeChanged(EventArgs e)
		{
			if (InvokeRequired)
			{
				Action<EventArgs> callback = new Action<EventArgs>(OnSourceSizeChanged);
				Invoke(callback, e);
			}
			else
			{
				Size = SourceSize;
				ResetDrawnRegion();
				UpdateThubmnail();

				if (SourceSizeChanged != null)
				{
					SourceSizeChanged(this, e);
				}
			}
		}

		protected virtual void OnSourceWindowChanged(EventArgs e)
		{
			ResetDrawnRegion();
			UpdateThubmnail();

			if (SourceWindowChanged != null)
			{
				SourceWindowChanged(this, e);
			}
		}

		protected override void Dispose(bool disposing)
		{
			UnsetWindow();
			base.Dispose(disposing);
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
			this.windowObserver = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// windowObserver
			// 
			this.windowObserver.WorkerSupportsCancellation = true;
			this.windowObserver.DoWork += new System.ComponentModel.DoWorkEventHandler(this.windowObserver_DoWork);
			this.windowObserver.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.windowObserver_RunWorkerCompleted);
			// 
			// ThumbnailPanel
			// 
			this.BackColor = System.Drawing.Color.White;
			this.ResumeLayout(false);

		}

		private void windowObserver_DoWork(object sender, DoWorkEventArgs e)
		{
			Size sourceSize = DesktopWindowManager.QueryThumbnailSourceSize(thumbnail);
			while (Process.GetProcesses().Any(process => process.MainWindowHandle == windowHandle))
			{
				Size size = DesktopWindowManager.QueryThumbnailSourceSize(thumbnail);
				if (!sourceSize.Equals(size))
				{
					sourceSize = size;
					OnSourceSizeChanged(EventArgs.Empty);
				}
			}
		}

		private void windowObserver_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!e.Cancelled)
			{
				Dispose(true);
			}
		}
	}
}
