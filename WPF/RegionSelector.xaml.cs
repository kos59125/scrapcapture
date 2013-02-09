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
      internal static readonly Thickness AeroRegion = new Thickness(ViewerWidth + 10, 0, 0, 0);

      public static readonly DependencyProperty SourceXProperty = DependencyProperty.Register("SourceX", typeof(double), typeof(RegionSelector), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, SourceXChanged));
      public static readonly DependencyProperty SourceYProperty = DependencyProperty.Register("SourceY", typeof(double), typeof(RegionSelector), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, SourceYChanged));
      public static readonly DependencyProperty SourceWidthProperty = DependencyProperty.Register("SourceWidth", typeof(double), typeof(RegionSelector), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, SourceWidthChanged));
      public static readonly DependencyProperty SourceHeightProperty = DependencyProperty.Register("SourceHeight", typeof(double), typeof(RegionSelector), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, SourceHeightChanged));

      public double SourceX
      {
         get
         {
            return (double)GetValue(SourceXProperty);
         }
         set
         {
            SetValue(SourceXProperty, value);
         }
      }
      public double SourceY
      {
         get
         {
            return (double)GetValue(SourceYProperty);
         }
         set
         {
            SetValue(SourceYProperty, value);
         }
      }
      public double SourceWidth
      {
         get
         {
            return (double)GetValue(SourceWidthProperty);
         }
         set
         {
            SetValue(SourceWidthProperty, value);
         }
      }
      public double SourceHeight
      {
         get
         {
            return (double)GetValue(SourceHeightProperty);
         }
         set
         {
            SetValue(SourceHeightProperty, value);
         }
      }

      public RegionSelector()
      {
         InitializeComponent();
         DataContext = this;
      }

      private static void SourceXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         RegionSelector selector = d as RegionSelector;
         if (selector != null)
         {
            double x = (double)e.NewValue;
            double sourceWidth = selector.thumbnail.SourceSize.Width;
            if (x < 0 || x >= sourceWidth)
            {
               selector.SourceX = (double)e.OldValue;
            }
            if (x + selector.SourceWidth > sourceWidth)
            {
               selector.SourceWidth = sourceWidth - x;
            }
            Scale(selector);
         }
      }

      private static void SourceYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         RegionSelector selector = d as RegionSelector;
         if (selector != null)
         {
            double y = (double)e.NewValue;
            double sourceHeight = selector.thumbnail.SourceSize.Height;
            if (y < 0 || y >= sourceHeight)
            {
               selector.SourceY = (double)e.OldValue;
            }
            if (y + selector.SourceHeight > sourceHeight)
            {
               selector.SourceHeight = sourceHeight - y;
            }
            Scale(selector);
         }
      }

      private static void SourceWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         RegionSelector selector = d as RegionSelector;
         if (selector != null)
         {
            double width = (double)e.NewValue;
            double sourceWidth = selector.thumbnail.SourceSize.Width;
            if (width < 0 || selector.SourceX + width > sourceWidth)
            {
               selector.SourceWidth = sourceWidth - selector.SourceX;
            }
            Scale(selector);
         }
      }

      private static void SourceHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         RegionSelector selector = d as RegionSelector;
         if (selector != null)
         {
            double height = (double)e.NewValue;
            double sourceHeight = selector.thumbnail.SourceSize.Height;
            if (height < 0 || selector.SourceY + height > sourceHeight)
            {
               selector.SourceHeight = sourceHeight - selector.SourceX;
            }
            Scale(selector);
         }
      }

      private static void Scale(RegionSelector selector)
      {
         Rect region = new Rect(selector.SourceX, selector.SourceY, selector.SourceWidth, selector.SourceHeight);
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
