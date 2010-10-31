using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RecycleBin.ScrapCapture.Globalization
{
	public static class CultureManager
	{
		public static UIMessages GetUIMessages()
		{
			return new UIMessages();
		}

		private static CultureInfo uiCulture;
		public static CultureInfo UICulture
		{
			get
			{
				return uiCulture ?? CultureInfo.CurrentUICulture;
			}
			set
			{
				uiCulture = value;
				UpdateCulture();
			}
		}

		private static void UpdateCulture()
		{
			UIMessages.Culture = UICulture;
			CultureResourceProvider.Refresh();
		}

		private static ObjectDataProvider provider;
		public static ObjectDataProvider CultureResourceProvider
		{
			get
			{
				if (provider == null && Application.Current != null)
				{
					provider = (ObjectDataProvider)Application.Current.FindResource("CultureManager");
				}
				return provider;
			}
		}
	}
}
