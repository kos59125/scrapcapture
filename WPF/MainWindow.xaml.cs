using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RecycleBin.ScrapCapture
{
	public partial class MainWindow : Window
	{
		private static readonly double ScaleDelta = 0.1;

		private Point mouseLocation;
		private Thumbnail thumbnail;

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
						Source = window.Icon
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
			Thumbnail thumbnail = new Thumbnail()
			{
				Owner = this,
				Target = window,
				Top = 0.0,
				Left = 0.0,
			};
			canvas.Children.Add(thumbnail);
		}

		private Thumbnail FindThumbnail(Point position)
		{
			var thumbnails = canvas.Children.OfType<Thumbnail>().Reverse();
			foreach (Thumbnail thumbnail in thumbnails)
			{
				if (thumbnail.Left <= position.X && position.X <= thumbnail.Left + thumbnail.RenderSize.Width &&
					thumbnail.Top <= position.Y && position.Y <= thumbnail.Top + thumbnail.RenderSize.Height)
				{
					return thumbnail;
				}
			}
			return null;
		}

		private void BringToFront(Thumbnail thumbnail)
		{
			thumbnail.BringToFront();
			canvas.Children.Remove(thumbnail);
			canvas.Children.Add(thumbnail);
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);

			if (e.Delta == 0)
			{
				return;
			}

			Thumbnail thumbnail = FindThumbnail(e.GetPosition(this));
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

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.LeftButton == MouseButtonState.Pressed)
			{
				mouseLocation = e.GetPosition(this);
				thumbnail = FindThumbnail(mouseLocation);
				if (thumbnail != null)
				{
					BringToFront(thumbnail);
					CaptureMouse();
					e.Handled = true;
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (IsMouseCaptured)
			{
				Point position = e.GetPosition(this);
				thumbnail.Left += position.X - mouseLocation.X;
				thumbnail.Top += position.Y - mouseLocation.Y;
				mouseLocation = position;
			}
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (IsMouseCaptured && e.LeftButton == MouseButtonState.Released)
			{
				thumbnail = null;
				ReleaseMouseCapture();
				e.Handled = true;
			}
		}
	}
}
