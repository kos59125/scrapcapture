using System.Runtime.InteropServices;
using System.Windows;

namespace RecycleBin.ScrapCapture
{
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
	internal struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;

		public RECT(double x, double y, double width, double height)
		{
			left = (int)x;
			top = (int)y;
			right = (int)(x + width);
			bottom = (int)(y + height);
		}

		public RECT(Point location, Size size)
		{
			left = (int)location.X;
			top = (int)location.Y;
			right = (int)(location.X + size.Width);
			bottom = (int)(location.Y + size.Height);
		}

		public RECT(Rect rectangle)
		{
			left = (int)rectangle.Left;
			top = (int)rectangle.Top;
			right = (int)rectangle.Right;
			bottom = (int)rectangle.Bottom;
		}

		public Size Size
		{
			get
			{
				return new Size(right - left, bottom - top);
			}
		}
	}
}
