using System;
using System.Windows.Forms;

namespace RecycleBin.ScrapCapture
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
		{
			try
			{
				ValidatePlatform();
				LaunchApplication();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}
		}

		private static void LaunchApplication()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new WindowCaptureForm());
		}

		private static void ValidatePlatform()
		{
			OperatingSystem os = Environment.OSVersion;
			if (os.Platform != PlatformID.Win32NT || os.Version.Major < 6 /* less than Vista */)
			{
				throw new PlatformNotSupportedException("Windows Vista 以降の OS でのみ動作します。");
			}
			if (!DesktopWindowManager.IsCompositionEnabled)
			{
				throw new ApplicationException("DWM コンポジションが有効ではありません。");
			}
		}
	}
}
