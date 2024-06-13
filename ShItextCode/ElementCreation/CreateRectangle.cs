#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using SharedCode;
using ShCode.ShDebugInfo;
using ShCommonCode.ShSheetData;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   5/27/2024 10:12:41 AM

namespace ShItextCode.ElementCreation
{
	public class CreateRectangle
	{

		public void PlaceSheetRectangle(PdfCanvas pdfCanvas,
			SheetRectData<SheetRectId> srd, bool rotate = false)
		{
			Rectangle r;

			r= CreateSupport.rotatSheetRectangleIfNeeded(srd.Rect);

			PlaceSheetRectangleRaw(pdfCanvas, srd, r);
		}

		public void PlaceSheetRectangleRaw(PdfCanvas pdfCanvas,
			SheetRectData<SheetRectId> srd, Rectangle r, PdfPage page = null)
		{
			// PdfShowInfo.ShowRectParams(srd);

			pdfCanvas.SaveState();

			PdfExtGState gs = new PdfExtGState();
			if (srd.FillOpacity>0) gs.SetFillOpacity(srd.FillOpacity);
			if (srd.BdrOpacity>0) gs.SetStrokeOpacity(srd.BdrOpacity);

			if (srd.TextBoxRotation % 90 != 0)
			{
				r = r.MoveRight((float) (r.GetHeight() * Math.Sin(FloatOps.ToRad(srd.TextBoxRotation))));

				CreateElement.PlaceDatum(r.GetX(), r.GetY(), pdfCanvas, 10, DeviceRgb.RED);
				
				AffineTransform af = AffineTransform.GetRotateInstance(FloatOps.ToRad(srd.TextBoxRotation), r.GetX(), r.GetY());

				pdfCanvas.ConcatMatrix(af);

			}

			pdfCanvas.Rectangle(r);
			pdfCanvas.SetExtGState(gs);
			if (srd.BdrWidth>0) pdfCanvas.SetLineWidth(srd.BdrWidth);
			if (srd.BdrColor!= null) pdfCanvas.SetStrokeColor(srd.BdrColor);
			if (srd.FillColor != null) pdfCanvas.SetFillColor(srd.FillColor);

			pdfCanvas.FillStroke();

			pdfCanvas.RestoreState();
		}

		private Vector getRectCenter(Rectangle r)
		{
			float w = r.GetWidth() / 2;
			float h = r.GetHeight() / 2;

			double x = r.GetX() + w;
			double y = r.GetY() + h;

			return new Vector((float) x, (float) y, 0);
		}


		// private Vector getRotatedRectCenter(Rectangle r, float rotation)
		// {
		// 	float w = r.GetWidth() / 2;
		// 	float h = r.GetHeight() / 2;
		//
		// 	double cosAngle = Math.Cos(FloatOps.ToRad(rotation));
		// 	double sinAngle = Math.Sin(FloatOps.ToRad(rotation));
		//
		//  double x = r.GetX() + w * cosAngle - h * sinAngle;
		// 	double y = r.GetY() + w * sinAngle + h * cosAngle;
		//
		// 	return new Vector((float) x, (float) y, 0);
		// }


		public override string ToString()
		{
			return $"this is {nameof(CreateRectangle)}";
		}
	}
}
