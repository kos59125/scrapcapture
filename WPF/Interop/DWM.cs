using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace RecycleBin.ScrapCapture.Interop
{
   internal static class DesktopWindowManager
   {
      private const int S_OK = 0;
      private static readonly Version WindowsVistaVersion = new Version(6, 0);

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

      public static bool IsSupportedOperatingSystem
      {
         get
         {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT && os.Version >= WindowsVistaVersion;
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

      public static bool ExtendFrame(this Window window, bool extended, Color background)
      {
         MARGINS margins = extended ? MARGINS.All : MARGINS.None;
         return window.ExtendFrame(margins, background);
      }

      public static bool ExtendFrame(this Window window, Thickness thickness, Color background)
      {
         MARGINS margins = new MARGINS(thickness);
         return window.ExtendFrame(margins, background);
      }

      private static bool ExtendFrame(this Window window, MARGINS margins, Color background)
      {
         IntPtr hWnd = new WindowInteropHelper(window).Handle;
         HwndSource.FromHwnd(hWnd).CompositionTarget.BackgroundColor = background;
         return DwmExtendFrameIntoClientArea(hWnd, margins) == S_OK;
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
      [DllImport("dwmapi.dll")]
      private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, MARGINS pMarInset);

      [Conditional("DEBUG")]
      [DllImport("dwmapi.dll", PreserveSig = false, EntryPoint = "DwmEnableComposition")]
      public static extern void EnableComposition(bool uCompositionAction);
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
   internal struct MARGINS
   {
      public static readonly MARGINS All = new MARGINS(-1);
      public static readonly MARGINS None = new MARGINS(0);

      public int cxLeftWidth;
      public int cxRightWidth;
      public int cyTopHeight;
      public int cyBottomHeight;

      public MARGINS(Thickness thickness)
      {
         cxLeftWidth = (int)thickness.Left;
         cxRightWidth = (int)thickness.Right;
         cyTopHeight = (int)thickness.Top;
         cyBottomHeight = (int)thickness.Bottom;
      }

      public MARGINS(int value)
      {
         cxLeftWidth = value;
         cxRightWidth = value;
         cyTopHeight = value;
         cyBottomHeight = value;
      }

      public MARGINS(int left, int right, int top, int bottom)
      {
         cxLeftWidth = left;
         cxRightWidth = right;
         cyTopHeight = top;
         cyBottomHeight = bottom;
      }
   }
}
