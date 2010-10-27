using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RecycleBin.ScrapCapture
{
	public partial class MainWindow : Window
	{
		private static readonly double ScaleDelta = 0.1;

		private Point mouseLocation;

		public MainWindow()
		{
			InitializeComponent();
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
				Header = "クライアント領域のみを表示",
				IsCheckable = true,
			};
			Binding checkedBinding = new Binding("IsChecked")
			{
				Source = toggleItem,
			};
			thumbnail.SetBinding(Thumbnail.ClientAreaOnlyProperty, checkedBinding);
			contextMenu.Items.Add(toggleItem);

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
