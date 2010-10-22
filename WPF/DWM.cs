using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace RecycleBin.ScrapCapture
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

		public static IntPtr Register(Window destination, ApplicationWindow source)
		{
			IntPtr thumbnail;
			IntPtr destinationHandle = new WindowInteropHelper(destination).Handle;
			if (DwmRegisterThumbnail(destinationHandle, source, out thumbnail) == S_OK)
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
			SIZE size;
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
		private static extern int DwmQueryThumbnailSourceSize(IntPtr hThumbnail, out SIZE pSize);
		[DllImport("dwmapi.dll")]
		private static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);
		[DllImport("dwmapi.dll")]
		private static extern int DwmIsCompositionEnabled(out bool pfEnabled);

#if DEBUG
		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern void DwmEnableComposition(bool uCompositionAction);
#endif
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
	internal struct SIZE
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

		public RECT(double x, double y, double width, double height)
		{
			Left = (int)x;
			Top = (int)y;
			Right = (int)(x + width);
			Bottom = (int)(y + height);
		}

		public RECT(Point location, Size size)
		{
			Left = (int)location.X;
			Top = (int)location.Y;
			Right = (int)(location.X + size.Width);
			Bottom = (int)(location.Y + size.Height);
		}

		public RECT(Rect rectangle)
		{
			Left = (int)rectangle.Left;
			Top = (int)rectangle.Top;
			Right = (int)rectangle.Right;
			Bottom = (int)rectangle.Bottom;
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
