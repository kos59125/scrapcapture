using System;
using System.Windows;
using System.Windows.Controls;

namespace RecycleBin.ScrapCapture
{
	/// <summary>
	/// Interaction logic for RegionSelector.xaml
	/// </summary>
	public partial class RegionSelector : Window
	{
		public static readonly double ViewerWidth = 480;
		public static readonly double ViewerHeight = 480;

		public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(RegionSelector), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, XChanged));
		public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(RegionSelector), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, YChanged));
		public static readonly DependencyProperty ThumbnailWidthProperty = DependencyProperty.Register("ThumbnailWidth", typeof(double), typeof(RegionSelector), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, ThumbnailWidthChanged));
		public static readonly DependencyProperty ThumbnailHeightProperty = DependencyProperty.Register("ThumbnailHeight", typeof(double), typeof(RegionSelector), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, ThumbnailHeightChanged));

		public double X
		{
			get
			{
				return (double)GetValue(XProperty);
			}
			set
			{
				SetValue(XProperty, value);
			}
		}
		public double Y
		{
			get
			{
				return (double)GetValue(YProperty);
			}
			set
			{
				SetValue(YProperty, value);
			}
		}
		public double ThumbnailWidth
		{
			get
			{
				return (double)GetValue(ThumbnailWidthProperty);
			}
			set
			{
				SetValue(ThumbnailWidthProperty, value);
			}
		}
		public double ThumbnailHeight
		{
			get
			{
				return (double)GetValue(ThumbnailHeightProperty);
			}
			set
			{
				SetValue(ThumbnailHeightProperty, value);
			}
		}

		public RegionSelector()
		{
			InitializeComponent();
			DataContext = this;
		}

		private static void XChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RegionSelector selector = d as RegionSelector;
			if (selector != null)
			{
				double x = (double)e.NewValue;
				double sourceWidth = selector.thumbnail.SourceSize.Width;
				if (x < 0 || x >= sourceWidth)
				{
					selector.X = (double)e.OldValue;
				}
				if (x + selector.ThumbnailWidth > sourceWidth)
				{
					selector.ThumbnailWidth = sourceWidth - x;
				}
				Scale(selector);
			}
		}

		private static void YChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RegionSelector selector = d as RegionSelector;
			if (selector != null)
			{
				double y = (double)e.NewValue;
				double sourceHeight = selector.thumbnail.SourceSize.Height;
				if (y < 0 || y >= sourceHeight)
				{
					selector.Y = (double)e.OldValue;
				}
				if (y + selector.ThumbnailHeight > sourceHeight)
				{
					selector.ThumbnailHeight = sourceHeight - y;
				}
				Scale(selector);
			}
		}

		private static void ThumbnailWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RegionSelector selector = d as RegionSelector;
			if (selector != null)
			{
				double width = (double)e.NewValue;
				double sourceWidth = selector.thumbnail.SourceSize.Width;
				if (width < 0 || selector.X + width > sourceWidth)
				{
					selector.ThumbnailWidth = sourceWidth - selector.X;
				}
				Scale(selector);
			}
		}

		private static void ThumbnailHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RegionSelector selector = d as RegionSelector;
			if (selector != null)
			{
				double height = (double)e.NewValue;
				double sourceHeight = selector.thumbnail.SourceSize.Height;
				if (height < 0 || selector.Y + height > sourceHeight)
				{
					selector.ThumbnailHeight = sourceHeight - selector.X;
				}
				Scale(selector);
			}
		}

		private static void Scale(RegionSelector selector)
		{
			Rect region = new Rect(selector.X, selector.Y, selector.ThumbnailWidth, selector.ThumbnailHeight);
			selector.thumbnail.DrawnRegion = region;
			selector.thumbnail.Scale = Math.Min(ViewerWidth / region.Width, ViewerHeight / region.Height);
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private static double ParseOrDefault(TextBox textBox, double defaultValue)
		{
			double value = defaultValue;
			double.TryParse(textBox.Text, out value);
			return value;
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				textBox.SelectAll();
			}
		}
	}
}
