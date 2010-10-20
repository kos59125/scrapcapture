using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RecycleBin.ScrapCapture
{
	public partial class Cover : Form
	{
		private Form internalCover;
		private Control coveredControl;
		private Point mouseLocation;

		public Cover()
		{
			InitializeComponent();
			InitializeInternalCover();
			FrameColor = Color.Lime;
		}

		private void InitializeInternalCover()
		{
			internalCover = new Form();
			internalCover.SuspendLayout();
			internalCover.FormBorderStyle = FormBorderStyle.None;
			internalCover.TransparencyKey = internalCover.BackColor;
			internalCover.ResumeLayout(false);
		}

		public Control CoveredControl
		{
			get
			{
				return coveredControl;
			}
			set
			{
				coveredControl = value;
				if (coveredControl != null)
				{
					ResetCover();
					this.ContextMenuStrip = coveredControl.ContextMenuStrip;
				}
			}
		}

		public Color FrameColor
		{
			get;
			set;
		}

		public void ResetCover()
		{
			if (CoveredControl != null)
			{
				this.Bounds = CoveredControl.RectangleToScreen(CoveredControl.Bounds);
			}
			Invalidate();
		}

		private void DrawFrame()
		{
			if (CoveredControl != null)
			{
				internalCover.Refresh();
				Rectangle frame = new Rectangle(0, 0, internalCover.Width - 1, internalCover.Height - 1);
				using (Graphics g = internalCover.CreateGraphics())
				using (Pen pen = new Pen(FrameColor))
				{
					g.DrawRectangle(pen, frame);
				}
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			internalCover.Show(this);
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			internalCover.Close();
			base.OnFormClosed(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			DrawFrame();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				mouseLocation = e.Location;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				Location = new Point(this.Location.X + e.X - mouseLocation.X, this.Location.Y + e.Y - mouseLocation.Y);
				AdjustBounds();
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				if (Bounds.IsEmpty || (CoveredControl != null && !CoveredControl.RectangleToScreen(CoveredControl.Bounds).Contains(this.Bounds)))
				{
					ResetCover();
				}
			}
		}

		private void AdjustBounds()
		{
			if (CoveredControl != null)
			{
				Rectangle coveredBounds = CoveredControl.RectangleToScreen(CoveredControl.Bounds);
				this.Left = Math.Max(this.Left, coveredBounds.Left);
				this.Top = Math.Max(this.Top, coveredBounds.Top);
				this.Width = Math.Min(this.Right - this.Left, coveredBounds.Right - this.Left);
				this.Height = Math.Min(this.Bottom - this.Top, coveredBounds.Bottom - this.Top);
				mouseLocation = PointToClient(Cursor.Position);
				Invalidate();
			}
		}

		protected override void OnLocationChanged(EventArgs e)
		{
			base.OnLocationChanged(e);

			if (internalCover != null)
			{
				internalCover.Bounds = this.Bounds;
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			if (internalCover != null)
			{
				internalCover.Bounds = this.Bounds;
			}
		}
	}
}
