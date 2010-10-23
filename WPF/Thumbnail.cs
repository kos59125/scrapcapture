using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RecycleBin.ScrapCapture
{
	public class Thumbnail : FrameworkElement
	{
		public event RoutedEventHandler ThumbnailUpdated
		{
			add
			{
				AddHandler(ThumbnailUpdatedEvent, value);
			}
			remove
			{
				RemoveHandler(ThumbnailUpdatedEvent, value);
			}
		}
		public event RoutedEventHandler ThumbnailUpdateFailed
		{
			add
			{
				AddHandler(ThumbnailUpdateFailedEvent, value);
			}
			remove
			{
				RemoveHandler(ThumbnailUpdateFailedEvent, value);
			}
		}

		public static readonly RoutedEvent ThumbnailUpdatedEvent = EventManager.RegisterRoutedEvent("ThumbnailUpdated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Thumbnail));
		public static readonly RoutedEvent ThumbnailUpdateFailedEvent = EventManager.RegisterRoutedEvent("ThumbnailUpdateFailed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Thumbnail));

		public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(Thumbnail), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, TopChanged));
		public static readonly DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof(double), typeof(Thumbnail), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, LeftChanged));
		public static readonly DependencyProperty DrawnRegionProperty = DependencyProperty.Register("DrawnRegion", typeof(Rect), typeof(Thumbnail), new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.None, DrawnRegionChanged));
		public static readonly DependencyProperty ClientAreaOnlyProperty = DependencyProperty.Register("ClientAreaOnly", typeof(bool), typeof(Thumbnail), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, ClientAreaOnlyChanged));
		public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(Thumbnail), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.None, ScaleChanged), scale => (double)scale > 0);

		private IntPtr thumbnail;
		public bool IsRegistered
		{
			get
			{
				return thumbnail != IntPtr.Zero;
			}
		}

		private ApplicationWindow target;
		public ApplicationWindow Target
		{
			get
			{
				return target;
			}
			set
			{
				if (IsRegistered)
				{
					DesktopWindowManager.Unregister(thumbnail);
					thumbnail = IntPtr.Zero;
				}
				target = null;

				if (value != null && DesktopWindowManager.IsCompositionEnabled)
				{
					target = value;
					thumbnail = DesktopWindowManager.Register(Owner, target);
					ResetDrawnRegion();
					UpdateThumbnail();
				}
			}
		}

		public Window Owner
		{
			get;
			set;
		}

		public double Top
		{
			get
			{
				return (double)GetValue(TopProperty);
			}
			set
			{
				SetValue(TopProperty, value);
			}
		}

		public double Left
		{
			get
			{
				return (double)GetValue(LeftProperty);
			}
			set
			{
				SetValue(LeftProperty, value);
			}
		}

		public Rect DrawnRegion
		{
			get
			{
				return (Rect)GetValue(DrawnRegionProperty);
			}
			set
			{
				SetValue(DrawnRegionProperty, value);
			}
		}

		public bool ClientAreaOnly
		{
			get
			{
				return (bool)GetValue(ClientAreaOnlyProperty);
			}
			set
			{
				SetValue(ClientAreaOnlyProperty, value);
			}
		}

		public double Scale
		{
			get
			{
				return (double)GetValue(ScaleProperty);
			}
			set
			{
				SetValue(ScaleProperty, value);
			}
		}

		public Size SourceSize
		{
			get
			{
				return DesktopWindowManager.QueryThumbnailSourceSize(thumbnail);
			}
		}

		public void ResetDrawnRegion()
		{
			if (thumbnail != IntPtr.Zero)
			{
				Size size = SourceSize;
				DrawnRegion = new Rect(0, 0, size.Width, size.Height);
			}
		}

		public void UpdateThumbnail()
		{
			if (thumbnail != IntPtr.Zero)
			{
				Point location = new Point(Left, Top);
				Size size = ComputeThumbnailSize();
				UpdateThumbnail(location, size);
			}
		}

		private void UpdateThumbnail(Point location, Size size)
		{
			DWM_THUMBNAIL_PROPERTIES properties = new DWM_THUMBNAIL_PROPERTIES()
			{
				dwFlags = DWM_TNP.DWM_TNP_OPACITY | DWM_TNP.DWM_TNP_RECTDESTINATION | DWM_TNP.DWM_TNP_RECTSOURCE | DWM_TNP.DWM_TNP_SOURCECLIENTAREAONLY | DWM_TNP.DWM_TNP_VISIBLE,
				fSourceClientAreaOnly = ClientAreaOnly,
				fVisible = true,
				opacity = (byte)(Byte.MaxValue * Opacity),
				rcDestination = new RECT(location.X, location.Y, size.Width, size.Height),
				rcSource = new RECT(DrawnRegion),
			};
			if (DesktopWindowManager.Update(thumbnail, ref properties))
			{
				OnThumbnailUpdated(this, EventArgs.Empty);
			}
			else
			{
				OnThumbnailUpdateFailed(this, EventArgs.Empty);
			}
		}

		public void BringToFront()
		{
			if (IsRegistered)
			{
				DesktopWindowManager.Unregister(thumbnail);
				thumbnail = DesktopWindowManager.Register(Owner, target);
				UpdateThumbnail();
			}
		}

		protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
#if DEBUG
			drawingContext.DrawRectangle(System.Windows.Media.Brushes.Black, null, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
#endif
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			Size newSize = ComputeThumbnailSize();
			Point location = new Point(Left + (finalSize.Width - newSize.Width) / 2, Top + (finalSize.Height - newSize.Height) / 2);
			UpdateThumbnail(location, newSize);
			return newSize;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			Size size = ComputeThumbnailSize();
			Width = size.Width;
			Height = size.Height;
			return size;
		}

		private Size ComputeThumbnailSize()
		{
			Size source = SourceSize;
			double width = DrawnRegion.Width;
			if (width <= 0 || width > source.Width)
			{
				width = source.Width;
			}
			double height = DrawnRegion.Height;
			if (height <= 0 || height > source.Height)
			{
				height = source.Height;
			}
			return new Size(Math.Ceiling(width * Scale), Math.Ceiling(height * Scale));
		}

		protected virtual void OnThumbnailUpdated(object sender, EventArgs e)
		{
			RaiseEvent(new RoutedEventArgs(ThumbnailUpdatedEvent, sender));
		}

		protected virtual void OnThumbnailUpdateFailed(object sender, EventArgs args)
		{
			RaiseEvent(new RoutedEventArgs(ThumbnailUpdateFailedEvent, sender));
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == UIElement.OpacityProperty)
			{
				UpdateThumbnail();
			}
		}

		private static void DrawnRegionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Thumbnail thumbnail = d as Thumbnail;
			if (thumbnail != null)
			{
				thumbnail.UpdateThumbnail();
			}
		}

		private static void ClientAreaOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Thumbnail thumbnail = d as Thumbnail;
			if (thumbnail != null)
			{
				thumbnail.UpdateThumbnail();
			}
		}

		private static void TopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Thumbnail thumbnail = d as Thumbnail;
			if (thumbnail != null)
			{
				Canvas.SetTop(thumbnail, (double)e.NewValue);
				thumbnail.UpdateThumbnail();
			}
		}

		private static void LeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Thumbnail thumbnail = d as Thumbnail;
			if (thumbnail != null)
			{
				Canvas.SetLeft(thumbnail, (double)e.NewValue);
				thumbnail.UpdateThumbnail();
			}
		}

		private static void ScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Thumbnail thumbnail = d as Thumbnail;
			if (thumbnail != null)
			{
				thumbnail.RenderSize = thumbnail.ComputeThumbnailSize();
				Size newSize = thumbnail.RenderSize;
				thumbnail.Left = thumbnail.Left + (thumbnail.Width - newSize.Width) / 2;
				thumbnail.Top = thumbnail.Top + (thumbnail.Height - newSize.Height) / 2;
				thumbnail.Width = newSize.Width;
				thumbnail.Height = newSize.Height;
				Point location = new Point(thumbnail.Left, thumbnail.Top);
				thumbnail.UpdateThumbnail(location, newSize);
				thumbnail.InvalidateVisual();
			}
		}
	}
}
