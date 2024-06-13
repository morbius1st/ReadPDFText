#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using ShCode.ShDebugInfo;
using ShCommonCode.ShSheetData;

#endregion

// user name: jeffs
// created:   5/22/2024 10:45:02 PM

namespace ShCommonCode.ShPdfSupport
{
	public struct PdfRectParams2
	{
		public Rectangle R { get; set; }
		public Color? StrokeColor { get; set; }
		public Color? FillColor { get; set; }
		public float FillOpacity { get; set; }
		public float StrokeThickness { get; set; }
		public float[] StrokeDashArray { get; set; }
		public float StrokeOpacity { get; set; }
	}

	public class ShPdfSupport
	{
		public float PageRotation { get; set; }

		public Rectangle PageSizeWithRotation { get; set; }

		// public void PlaceSheetRectangle2(PdfCanvas pdfCanvas,
		// 	PdfRectParams pStr)
		// {
		// 	pStr.R = rotatSheetRectangleIfNeeded(pStr.R);
		//
		// 	DebugShowInfo.ShowRectParams(pStr);
		//
		// 	pdfCanvas.SaveState();
		//
		// 	PdfExtGState gs = new PdfExtGState();
		// 	gs.SetFillOpacity(pStr.FillOpacity);
		// 	gs.SetStrokeOpacity(pStr.StrokeOpacity);
		//
		// 	pdfCanvas.Rectangle(pStr.R)
		// 	.SetExtGState(gs)
		// 	.SetLineWidth(pStr.StrokeThickness)
		// 	.SetStrokeColor(pStr.StrokeColor)
		// 	.SetFillColor(pStr.FillColor)
		// 	.FillStroke();
		//
		// 	pdfCanvas.RestoreState();
		// }

		
		public void PlaceSheetRectangle(PdfCanvas pdfCanvas,
			SheetRectData<SheetRectId> srd)
		{
			Rectangle r= rotatSheetRectangleIfNeeded(srd.Rect);

			PdfShowInfo.ShowRectParams(srd);

			pdfCanvas.SaveState();

			PdfExtGState gs = new PdfExtGState();
			if (srd.FillOpacity>0) gs.SetFillOpacity(srd.FillOpacity);
			if (srd.BdrOpacity>0) gs.SetStrokeOpacity(srd.BdrOpacity);

			pdfCanvas.Rectangle(r);
			pdfCanvas.SetExtGState(gs);
			if (srd.BdrWidth>0) pdfCanvas.SetLineWidth(srd.BdrWidth);
			if (srd.BdrColor!= null) pdfCanvas.SetStrokeColor(srd.BdrColor);
			if (srd.FillColor != null) pdfCanvas.SetFillColor(srd.FillColor);

			pdfCanvas.FillStroke();

			pdfCanvas.RestoreState();
		}


		public Rectangle rotatSheetRectangleIfNeeded(Rectangle r)
		{
			if (PageRotation == 0) return r;

			Rectangle rr;
			Rectangle ps = PageSizeWithRotation;

			if (PageRotation == 90)
			{
				rr = new Rectangle(ps.GetHeight() - r.GetY(), r.GetX(), -1 * r.GetHeight(), r.GetWidth());
			}
			else
			{
				rr = new Rectangle(r.GetY(), ps.GetWidth() - r.GetX(), r.GetHeight(), -1 * r.GetWidth());
			}

			return rr;
		}

		public override string ToString()
		{
			return $"this is {nameof(ShPdfSupport)}";
		}
	}


}