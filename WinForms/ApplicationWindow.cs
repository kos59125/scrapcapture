using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace RecycleBin.ScrapCapture
{
	internal class ApplicationWindow
	{
		private readonly IntPtr windowHandle;

		public ApplicationWindow(IntPtr windowHandle)
		{
			this.windowHandle = windowHandle;
		}

		public IntPtr Handle
		{
			get
			{
				return windowHandle;
			}
		}

		public string Text
		{
			get
			{
				return GetText(Handle);
			}
		}

		public Icon Icon
		{
			get
			{
				IntPtr iconHandle = GetIconHandle(Handle);
				return iconHandle == IntPtr.Zero ? null : Icon.FromHandle(iconHandle);
			}
		}

		public bool Visible
		{
			get
			{
				return IsWindowVisible(Handle);
			}
		}

		public int ProcessId
		{
			get
			{
				int processId;
				GetWindowThreadProcessId(Handle, out processId);
				return processId;
			}
		}

		public static IEnumerable<ApplicationWindow> GetApplicationWindows()
		{
			List<ApplicationWindow> windows = new List<ApplicationWindow>();
			EnumWindowsCallback callback = new EnumWindowsCallback(
				(hWnd, lParam) =>
				{
					if (!IsWindowVisible(hWnd) || string.IsNullOrEmpty(GetText(hWnd)))
					{
						return true;
					}

					if ((long)GetParent(hWnd) == 0)
					{
						bool hasOwner = (long)GetWindow(hWnd, GW_OWNER) != 0;
						uint exStyle = (uint)GetWindowLongPtr(hWnd, GWL_EXSTYLE);

						if ((exStyle & WS_EX_TOOLWINDOW) == 0 && !hasOwner)
						{
							ApplicationWindow window = new ApplicationWindow(hWnd);
							windows.Add(window);
						}
					}

					return true;
				}
			);
			EnumWindows(callback, IntPtr.Zero);
			return windows;
		}

		private static string GetText(IntPtr handle)
		{
			int length = GetWindowTextLength(handle);
			if (length > 0)
			{
				StringBuilder text = new StringBuilder(length + 1);
				if (GetWindowText(handle, text, text.Capacity) > 0)
				{
					return text.ToString();
				}
			}
			return String.Empty;
		}

		private static IntPtr GetIconHandle(IntPtr windowHandle)
		{
			return GetClassLongPtr(windowHandle, GCL_HICONSM);
		}

		private const uint GW_OWNER = 4;
		private const uint WS_EX_TOOLWINDOW = 0x80;
		private const int GWL_EXSTYLE = -20;
		private const int GCL_HICONSM = -34;
		private delegate bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern bool EnumWindows(EnumWindowsCallback lpEnumFunc, IntPtr lParam);
		[DllImport("user32.dll")]
		private static extern bool IsWindowVisible(IntPtr hWnd);
		[DllImport("user32.dll")]
		private static extern IntPtr GetParent(IntPtr hWnd);
		[DllImport("user32.dll")]
		private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		private static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		private static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);
		[DllImport("user32.dll")]
		private static extern int GetWindowTextLength(IntPtr hWnd);
		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
	}
}
