﻿using System;
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
			thumbnail.MouseWheel += Thumbnail_MouseWheel;
			thumbnail.MouseDown += Thumbnail_MouseDown;
			thumbnail.MouseUp += Thumbnail_MouseUp;
			thumbnail.MouseMove += Thumbnail_MouseMove;
			canvas.Children.Add(thumbnail);
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

		private void Thumbnail_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Thumbnail thumbnail = sender as Thumbnail;
				mouseLocation = e.GetPosition(this);
				if (thumbnail != null)
				{
					BringToFront(thumbnail);
					thumbnail.CaptureMouse();
					e.Handled = true;
				}
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

		private void Thumbnail_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Thumbnail thumbnail = sender as Thumbnail;
			if (thumbnail != null)
			{
				thumbnail.ReleaseMouseCapture();
				e.Handled = true;
			}
		}
	}
}
