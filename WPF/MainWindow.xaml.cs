using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using RecycleBin.ScrapCapture.Interop;

namespace RecycleBin.ScrapCapture
{
   public partial class MainWindow : Window
   {
      public static readonly DependencyProperty AeroGlassProperty = DependencyProperty.Register("AeroGlass", typeof(bool), typeof(MainWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, AeroGlassChanged));

      private static readonly double ScaleDelta = 0.1;

      private Point mouseLocation;
      private Brush background;

      public bool AeroGlass
      {
         get
         {
            return (bool)GetValue(AeroGlassProperty);
         }
         set
         {
            SetValue(AeroGlassProperty, value);
         }
      }

      public MainWindow()
      {
         InitializeComponent();
         DataContext = this;

         AddDwmMenu(ContextMenu);
      }

      [Conditional("DEBUG")]
      private void AddDwmMenu(ItemsControl items)
      {
         ContextMenu.Items.Add(new Separator());
         MenuItem compositionItem = new MenuItem()
         {
            Header = "デスクトップ コンポジションを無効にする",
            IsCheckable = true,
            IsChecked = !DesktopWindowManager.IsCompositionEnabled,
         };
         compositionItem.Click += (sender, e) => DesktopWindowManager.EnableComposition(!compositionItem.IsChecked);
         ContextMenu.Items.Add(compositionItem);
      }

      private void selectWindowMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
      {
         ItemCollection items = selectWindowMenuItem.Items;
         items.Clear();
         ApplicationWindow[] windows = ApplicationWindow.GetApplicationWindows().Where(window => !window.IsIdentical(this)).ToArray();
         for (int index = 0; index < windows.Length; index++)
         {
            ApplicationWindow window = windows[index];
            Process process = Process.GetProcessById(window.ProcessId);
            MenuItem windowItem = new MenuItem()
            {
               Header = window.Text,
               Icon = new Image()
               {
                  Source = window.GetIcon(IconSize.Small),
                  Width = 16,
                  Height = 16,
               },
            };
            windowItem.Click += (eventSender, eventArgs) => CaptureWindow(window);
            items.Add(windowItem);
         }
         if (items.Count == 0)
         {
            object[] extension = (object[])this.FindResource("noSelectionItem");
            foreach (var item in extension)
            {
               items.Add(item);
            }
         }
      }

      public void CaptureWindow(ApplicationWindow window)
      {
         Thumbnail thumbnail = new Thumbnail();
         SetContextMenu(thumbnail);
         RegisterMouseEvent(thumbnail);
         canvas.Children.Add(thumbnail);
         thumbnail.SetWindow(window);
      }

      private void SetContextMenu(Thumbnail thumbnail)
      {
         ContextMenu contextMenu = new ContextMenu();

         MenuItem toggleItem = new MenuItem()
         {
            Header = Globalization.UIMessages.ToggleClientOnlyMenuItem,
            IsCheckable = true,
         };
         Binding checkedBinding = new Binding("IsChecked")
         {
            Source = toggleItem,
         };
         thumbnail.SetBinding(Thumbnail.ClientAreaOnlyProperty, checkedBinding);
         contextMenu.Items.Add(toggleItem);

         MenuItem selectorItem = new MenuItem()
         {
            Header = Globalization.UIMessages.SelectRegionMenuItem,
         };
         Binding enabledBinding = new Binding("IsEnabled")
         {
            Source = selectorItem,
            Converter = Converters.BooleanToggleConverter.Instance,
            Mode = BindingMode.OneWayToSource,
         };
         toggleItem.SetBinding(MenuItem.IsCheckedProperty, enabledBinding);
         selectorItem.Click += (sender, e) =>
         {
            Rect region = thumbnail.DrawnRegion;
            RegionSelector selector = new RegionSelector();
            selector.thumbnail.Scale = Math.Min(RegionSelector.ViewerWidth / region.Width, RegionSelector.ViewerHeight / region.Height);
            selector.thumbnail.ClientAreaOnly = thumbnail.ClientAreaOnly;
            selector.Loaded += (a, b) =>
            {
               selector.Background = Brushes.Transparent;
               selector.ExtendFrame(RegionSelector.AeroRegion, Colors.Transparent);
               selector.thumbnail.SetWindow(thumbnail.Target);
               selector.X = region.X;
               selector.Y = region.Y;
               selector.ThumbnailWidth = region.Width;
               selector.ThumbnailHeight = region.Height;
            };
            selector.ContentRendered += (a, b) =>
            {
               selector.thumbnail.DrawnRegion = region;
            };
            if (selector.ShowDialog().GetValueOrDefault(false) == true)
            {
               thumbnail.DrawnRegion = selector.thumbnail.DrawnRegion;
            }
         };
         contextMenu.Items.Add(selectorItem);

         contextMenu.Items.Add(new Separator());

         MenuItem removeItem = new MenuItem()
         {
            Header = Globalization.UIMessages.RemoveMenuItem,
         };
         removeItem.Click += (sender, e) =>
         {
            thumbnail.Dispose();
            canvas.Children.Remove(thumbnail);
         };
         contextMenu.Items.Add(removeItem);

         thumbnail.ContextMenu = contextMenu;
      }

      private void RegisterMouseEvent(Thumbnail thumbnail)
      {
         thumbnail.MouseWheel += Thumbnail_MouseWheel;
         thumbnail.MouseLeftButtonDown += Thumbnail_MouseLeftButtonDown;
         thumbnail.MouseLeftButtonUp += Thumbnail_MouseLeftButtonUp;
         thumbnail.MouseMove += Thumbnail_MouseMove;
      }

      private void BringToFront(Thumbnail thumbnail)
      {
         thumbnail.BringToFront();
         canvas.Children.Remove(thumbnail);
         canvas.Children.Add(thumbnail);
      }

      private static void AeroGlassChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         MainWindow window = d as MainWindow;
         if (window != null && DesktopWindowManager.IsCompositionEnabled)
         {
            bool extended = (bool)e.NewValue;
            if (extended)
            {
               window.background = window.Background;
               window.Background = Brushes.Transparent;
            }
            else
            {
               window.Background = window.background;
            }
            window.ExtendFrame(extended, Colors.Transparent);
         }
      }

      private void Thumbnail_MouseWheel(object sender, MouseWheelEventArgs e)
      {
         if (e.Delta == 0)
         {
            return;
         }

         Thumbnail thumbnail = sender as Thumbnail;
         if (thumbnail == null)
         {
            return;
         }

         if (e.Delta > 0)
         {
            thumbnail.Scale += ScaleDelta;
         }
         else
         {
            thumbnail.Scale = Math.Max(ScaleDelta, thumbnail.Scale - ScaleDelta);
         }
      }

      private void Thumbnail_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         Thumbnail thumbnail = sender as Thumbnail;
         if (thumbnail != null && e.LeftButton == MouseButtonState.Pressed)
         {
            mouseLocation = e.GetPosition(this);
            BringToFront(thumbnail);
            thumbnail.CaptureMouse();
         }
      }

      private void Thumbnail_MouseMove(object sender, MouseEventArgs e)
      {
         Thumbnail thumbnail = sender as Thumbnail;
         if (thumbnail != null && thumbnail.IsMouseCaptured)
         {
            Point position = e.GetPosition(this);
            thumbnail.Left += position.X - mouseLocation.X;
            thumbnail.Top += position.Y - mouseLocation.Y;
            mouseLocation = position;
         }
      }

      private void Thumbnail_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
         Thumbnail thumbnail = sender as Thumbnail;
         if (thumbnail != null)
         {
            thumbnail.ReleaseMouseCapture();
         }
      }
   }
}
