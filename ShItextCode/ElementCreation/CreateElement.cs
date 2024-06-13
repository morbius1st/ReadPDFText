#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;

#endregion

// user name: jeffs
// created:   6/8/2024 7:13:19 AM

namespace ShItextCode.ElementCreation
{
	public class CreateElement
	{

		public static void PlaceDatum( double x, double y,
			PdfCanvas pdfCanvas, int r = 3,
			Color fill = null)
		{
			if (fill == null)
			{
				fill = DeviceRgb.GREEN;
			}

			float ext = r * 1.5f;

			PdfCanvas canvas = pdfCanvas;

			PdfExtGState gs = new PdfExtGState().SetFillOpacity(0.3f);

			canvas.SaveState();

			canvas.SetLineWidth(0.15f);
			canvas.SetStrokeColor(DeviceRgb.BLACK);
			canvas.SetFillColor(fill);
			canvas.SetExtGState(gs);

			canvas.MoveTo(x - ext, y);
			canvas.LineTo(x + ext, y);

			canvas.MoveTo(x, y - ext);
			canvas.LineTo(x, y + ext);

			canvas.Circle(x, y, r);

			canvas.FillStroke();

			canvas.RestoreState();
		}


		public override string ToString()
		{
			return $"this is {nameof(CreateElement)}";
		}
	}
}
