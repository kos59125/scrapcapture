using System;
using System.Windows.Data;

namespace RecycleBin.ScrapCapture.Converters
{
	public class BooleanToggleConverter : IValueConverter
	{
		internal static readonly BooleanToggleConverter Instance = new BooleanToggleConverter();

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool flag = (bool)value;
			return !flag;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool flag = (bool)value;
			return !flag;
		}
	}
}
