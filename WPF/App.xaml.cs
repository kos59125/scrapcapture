using System;
using System.Windows;
using RecycleBin.ScrapCapture.Interop;

namespace RecycleBin.ScrapCapture
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      protected override void OnStartup(StartupEventArgs e)
      {
         base.OnStartup(e);

         if (!DesktopWindowManager.IsSupportedOperatingSystem)
         {
            MessageBox.Show("Windows Vista 以降の Windows Aero に対応した OS で動作します。", "未対応 OS", MessageBoxButton.OK, MessageBoxImage.Error);
            Current.Shutdown();
         }
         if (!DesktopWindowManager.IsCompositionEnabled)
         {
            MessageBox.Show("デスクトップ コンポジションが無効です。", "DWM エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            Current.Shutdown();
         }
      }
   }
}
