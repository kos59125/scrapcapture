﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RecycleBin.ScrapCapture
{
	public partial class RegionSelector : Form
	{
		private Rectangle drawnRegion;
		private Cover cover;
		private Point? mouseLocation;

		public RegionSelector()
		{
			InitializeComponent();
			mouseLocation = null;
		}

		public void SetWindow(IntPtr thumbnail, bool clientAreaOnly)
		{
			thumbnailPanel.SetWindow(thumbnail);
			thumbnailPanel.ClientAreaOnly = clientAreaOnly;
		}

		public Rectangle DrawnRegion
		{
			get
			{
				return drawnRegion;
			}
			set
			{
				drawnRegion = value;
				UpdateCover();
			}
		}

		public void ResetDrawnRegion()
		{
			DrawnRegion = new Rectangle(Point.Empty, thumbnailPanel.DrawnSize);
			UpdateCover();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			cover = new Cover();
			cover.CoveredControl = thumbnailPanel;
			cover.SizeChanged += (sender, args) => OnCoverBoundsChanged(args);
			cover.LocationChanged += (sender, args) => OnCoverBoundsChanged(args);
			cover.MouseMove += (sender, args) => regionToolStripStatusLabel.Text = string.Format("描画領域: {0}", drawnRegion);
			cover.Show(this);
			UpdateCover();
		}

		private void OnCoverBoundsChanged(EventArgs e)
		{
			if (cover.Visible)
			{
				drawnRegion = RectangleToClient(cover.Bounds);
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			thumbnailPanel.UnsetWindow();
			cover.Close();
			base.OnFormClosed(e);
		}

		protected override void OnLocationChanged(EventArgs e)
		{
			base.OnLocationChanged(e);

			if (cover != null)
			{
				UpdateCover();
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			if (cover != null)
			{
				UpdateCover();
			}
		}

		private void UpdateCover()
		{
			if (cover != null)
			{
				cover.Bounds = RectangleToScreen(drawnRegion);
				cover.Refresh();
			}
		}

		private void UpdateSize()
		{
			Size thumbnailSize = thumbnailPanel.DrawnSize;
			ClientSize = new Size(thumbnailSize.Width, thumbnailSize.Height + statusStrip.Height);
		}

		private void thumbnailPanel_SourceWindowChanged(object sender, EventArgs e)
		{
			UpdateSize();
		}

		private void thumbnailPanel_SourceSizeChanged(object sender, EventArgs e)
		{
			UpdateSize();
		}

		private void thumbnailPanel_MouseDown(object sender, MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				ResetDrawnRegion();
				mouseLocation = e.Location;
			}
		}

		private void thumbnailPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left && mouseLocation != null)
			{
				cover.Refresh();
				Rectangle rectangle = GetRectangleFromPoints(mouseLocation.Value, e.Location);
				if (!rectangle.IsEmpty)
				{
					DrawnRegion = rectangle;
				}
				regionToolStripStatusLabel.Text = string.Format("描画領域: {0}; 開始点: {1}; 現在の座標: {2}", GetRectangleFromPoints(mouseLocation.Value, e.Location), mouseLocation, e.Location);
			}
			else
			{
				regionToolStripStatusLabel.Text = string.Format("描画領域を開始: {0}", e.Location);
			}
		}

		private void thumbnailPanel_MouseUp(object sender, MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left && mouseLocation != null)
			{
				Rectangle rectangle = GetRectangleFromPoints(mouseLocation.Value, e.Location);
				if (rectangle.IsEmpty)
				{
					ResetDrawnRegion();
				}
				else
				{
					DrawnRegion = rectangle;
				}
				mouseLocation = null;
			}
		}

		private Rectangle GetRectangleFromPoints(Point p1, Point p2)
		{
			int x0 = Math.Max(0, Math.Min(p1.X, p2.X));
			int y0 = Math.Max(0, Math.Min(p1.Y, p2.Y));
			int x1 = Math.Min(Math.Max(p1.X, p2.X), thumbnailPanel.Right);
			int y1 = Math.Min(Math.Max(p1.Y, p2.Y), thumbnailPanel.Bottom);
			return new Rectangle(x0, y0, x1 - x0, y1 - y0);
		}

		private void resetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ResetDrawnRegion();
		}

		private void okToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
