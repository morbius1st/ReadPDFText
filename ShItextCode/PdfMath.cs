#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;

#endregion

// user name: jeffs
// created:   6/8/2024 2:30:47 PM

namespace ShItextCode
{
	public static class PdfMath
	{

		public static Rectangle BBoxForRotatedRect(Rectangle r, double angleRad)
		{
			double w = r.GetHeight() * Math.Sin(angleRad) + r.GetWidth() * Math.Cos(angleRad);
			double h = r.GetHeight() * Math.Cos(angleRad) + r.GetWidth() * Math.Sin(angleRad);

			return new Rectangle(r.GetX(), r.GetY(), (float) w, (float) h);
		}


	}
}
