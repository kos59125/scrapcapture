using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RecycleBin.ScrapCapture
{
	public class ThumbnailPanel : PictureBox
	{
		public event EventHandler SourceSizeChanged;
		public event EventHandler SourceWindowChanged;
		public event EventHandler FixedChanged;

		private IntPtr thumbnail;
		private IntPtr windowHandle;
		private Rectangle drawnRegion;
		private float scale;
		private BackgroundWorker windowObserver;
		private bool clientAreaOnly;
		private bool isFixed;

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
				OnSourceSizeChanged(EventArgs.Empty);
			}
		}

		public bool IsFixed
		{
			get
			{
				return isFixed;
			}
			set
			{
				if (isFixed != value)
				{
					isFixed = value;
					OnFixedChanged(EventArgs.Empty);
				}
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

		internal protected IntPtr GetWindow()
		{
			return windowHandle;
		}

		public void ResetDrawnRegion()
		{
			if (thumbnail != IntPtr.Zero)
			{
				DrawnRegion = new Rectangle(Point.Empty, SourceSize);
			}
		}

		public Image CaptureCurrentImage()
		{
			Rectangle bounds = GetVisibleBounds(ClientRectangle);
			Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bitmap.Size);
			}
			return bitmap;
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
				Form parentForm = FindForm();
				visibleRegion.Intersect(parentForm.RectangleToScreen(parentForm.ClientRectangle));
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

		//public Image CaptureCurrentImage()
		//{
		//   RECT rect;
		//   GetWindowRect(windowHandle, out rect);
		//   Size size = rect.Size;

		//   if (size.IsEmpty)
		//   {
		//      throw new ApplicationException("ウィンドウサイズが取得できませんでした。");
		//   }

		//   // ウィンドウ枠も含めてウィンドウ全体をビットマップに描画する。
		//   IntPtr deviceContext = GetWindowDC(windowHandle);
		//   Bitmap bitmap = new Bitmap(size.Width, size.Height);
		//   using (Graphics g = Graphics.FromImage(bitmap))
		//   {
		//      IntPtr hDC = g.GetHdc();
		//      BitBlt(hDC, 0, 0, bitmap.Width, bitmap.Height, deviceContext, 0, 0, SRCCOPY);
		//      g.ReleaseHdc(hDC);
		//   }
		//   ReleaseDC(windowHandle, deviceContext);

		//   Padding borderMargin = new Padding();
		//   if (ClientAreaOnly)
		//   {
		//      RECT clientRect;
		//      GetClientRect(windowHandle, out clientRect);
		//      Size clientSize = clientRect.Size;
		//      int borderWidth = (size.Width - clientSize.Width) / 2;
		//      borderMargin.Left = borderWidth;
		//      borderMargin.Right = borderWidth;
		//      borderMargin.Bottom = borderWidth;
		//      borderMargin.Top = size.Height - clientSize.Height - borderWidth;
		//   }

		//   Bitmap clippedBitmap = new Bitmap(DrawnSize.Width, DrawnSize.Height);
		//   using (Graphics g = Graphics.FromImage(clippedBitmap))
		//   {
		//      Rectangle clippedRegion = new Rectangle(
		//         drawnRegion.Left + borderMargin.Left,
		//         drawnRegion.Top + borderMargin.Top,
		//         drawnRegion.Width,
		//         drawnRegion.Height
		//      );
		//      g.InterpolationMode = InterpolationMode.Bicubic;
		//      g.DrawImage(bitmap, new Rectangle(Point.Empty, clippedBitmap.Size), clippedRegion, GraphicsUnit.Pixel);
		//   }

		//   bitmap.Dispose();
		//   return clippedBitmap;
		//}

		//private const int SRCCOPY = 0x00CC0020;

		//[DllImport("user32.dll")]
		//private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
		//[DllImport("user32.dll")]
		//private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
		//[DllImport("user32.dll")]
		//private static extern IntPtr GetWindowDC(IntPtr hWnd);
		//[DllImport("user32.dll")]
		//private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
		//[DllImport("gdi32.dll")]
		//private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

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
					fVisible = Visible & !IsFixed,
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

		protected virtual void OnFixedChanged(EventArgs e)
		{
			if (isFixed)
			{
				Dictionary<ThumbnailPanel, bool> dictionary = null;
				if (Parent != null)
				{
					dictionary = Parent.Controls.OfType<ThumbnailPanel>().Where(panel => !object.ReferenceEquals(panel, this)).ToDictionary(panel => panel, panel => panel.Visible);
				}
				foreach (ThumbnailPanel panel in dictionary.Keys)
				{
					panel.Visible = false;
				}
				if (Image == null)
				{
					Image = new Bitmap(Width, Height);
				}
				using (Image capture = CaptureCurrentImage())
				using (Graphics g = Graphics.FromImage(Image))
				{
					g.Clear(Color.Black);
					g.DrawImageUnscaled(capture, RectangleToClient(GetVisibleBounds(ClientRectangle)));
				}
				if (dictionary != null)
				{
					foreach (ThumbnailPanel panel in dictionary.Keys)
					{
						panel.Visible = dictionary[panel];
					}
				}
				BackColor = Color.Black;
			}
			else
			{
				Image = null;
				BackColor = Color.White;
			}
			UpdateThubmnail();

			if (FixedChanged != null)
			{
				FixedChanged(this, e);
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
			while (ApplicationWindow.GetApplicationWindows().Any(window => window.Handle == windowHandle))
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
