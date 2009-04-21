using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RecycleBin.WindowCapture
{
	public partial class Cover : Form
	{
		public Cover()
		{
			InitializeComponent();
		}

		public Color FrameColor
		{
			get;
			set;
		}

		public override void Refresh()
		{
			base.Refresh();
			DrawFrame();
		}

		private void DrawFrame()
		{
			Rectangle frame = new Rectangle(0, 0, Width - 1, Height - 1);
			using (Graphics g = CreateGraphics())
			using (Pen pen = new Pen(FrameColor))
			{
				g.DrawRectangle(pen, frame);
			}
		}
	}
}
