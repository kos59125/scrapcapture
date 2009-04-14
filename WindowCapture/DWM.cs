using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace RecycleBin.WindowCapture
{
	internal static class DesktopWindowManager
	{
		private const int S_OK = 0;

		public static bool IsCompositionEnabled
		{
			get
			{
				bool enabled;
				if (DwmIsCompositionEnabled(out enabled) == S_OK)
				{
					return enabled;
				}
				return false;
			}
		}

		public static IntPtr Register(Form form, IntPtr source)
		{
			IntPtr thumbnail;
			if (DwmRegisterThumbnail(form.Handle, source, out thumbnail) == S_OK)
			{
				return thumbnail;
			}

			return IntPtr.Zero;
		}

		public static bool Unregister(IntPtr thumbnail)
		{
			return DwmUnregisterThumbnail(thumbnail) == S_OK;
		}

		public static Size QueryThumbnailSourceSize(IntPtr thumbnail)
		{
			PSIZE size;
			DwmQueryThumbnailSourceSize(thumbnail, out size);
			return size.ToSize();
		}

		public static bool Update(IntPtr thumbnail, ref DWM_THUMBNAIL_PROPERTIES properties)
		{
			return DwmUpdateThumbnailProperties(thumbnail, ref properties) == S_OK;
		}

		[DllImport("dwmapi.dll")]
		private static extern int DwmRegisterThumbnail(IntPtr hwndDestination, IntPtr hwndSource, out IntPtr phThumbnailId);
		[DllImport("dwmapi.dll")]
		private static extern int DwmUnregisterThumbnail(IntPtr hThumbnailId);
		[DllImport("dwmapi.dll")]
		private static extern int DwmQueryThumbnailSourceSize(IntPtr hThumbnail, out PSIZE pSize);
		[DllImport("dwmapi.dll")]
		private static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);
		[DllImport("dwmapi.dll")]
		private static extern int DwmIsCompositionEnabled(out bool pfEnabled);
	}

	[Flags]
	internal enum DWM_TNP
	{
		DWM_TNP_RECTDESTINATION = 0x00000001,
		DWM_TNP_RECTSOURCE = 0x00000002,
		DWM_TNP_OPACITY = 0x00000004,
		DWM_TNP_VISIBLE = 0x00000008,
		DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010,
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct PSIZE
	{
		public int cx;
		public int cy;

		public Size ToSize()
		{
			return new Size(cx, cy);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct DWM_THUMBNAIL_PROPERTIES
	{
		public DWM_TNP dwFlags;
		public RECT rcDestination;
		public RECT rcSource;
		public byte opacity;
		public bool fVisible;
		public bool fSourceClientAreaOnly;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public RECT(Point location, Size size)
		{
			Left = location.X;
			Top = location.Y;
			Right = location.X + size.Width;
			Bottom = location.Y + size.Height;
		}

		public RECT(Rectangle rectangle)
		{
			Left = rectangle.Left;
			Top = rectangle.Top;
			Right = rectangle.Right;
			Bottom = rectangle.Bottom;
		}

		public Size Size
		{
			get
			{
				return new Size(Right - Left, Bottom - Top);
			}
		}
	}
}
