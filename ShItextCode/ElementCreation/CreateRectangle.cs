#region + Using Directives

using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using ShItextCode.PdfCalculations;
using ShSheetData.SheetData;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   5/27/2024 10:12:41 AM

namespace ShItextCode.ElementCreation
{
	public class CreateRectangle
	{
		private string rectname;


		/*
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
			rectname = srd.Id.ToString();

			pdfCanvas.SaveState();

			PdfExtGState gs = new PdfExtGState();
			if (srd.FillOpacity>0) gs.SetFillOpacity(srd.FillOpacity);
			if (srd.BdrOpacity>0) gs.SetStrokeOpacity(srd.BdrOpacity);

			if (!rectname.Equals("SM_OPT3") || !rectname.Equals("SM_OPT4") || !rectname.Equals("SM_OPT6"))
			{
				if (srd.TextBoxRotation % 90 != 0)
				{
					r = r.MoveRight((float) (r.GetHeight() * Math.Sin(FloatOps.ToRad(srd.TextBoxRotation))));

					CreateElement.PlaceDatum(r.GetX(), r.GetY(), pdfCanvas, 10, DeviceRgb.RED);
					
					AffineTransform af = AffineTransform.GetRotateInstance(FloatOps.ToRad(srd.TextBoxRotation), r.GetX(), r.GetY());

					pdfCanvas.ConcatMatrix(af);

				}
			}
			else
			{
				CreateElement.PlaceDatum(r.GetX(), r.GetY(), pdfCanvas, 10, DeviceRgb.RED);
			}

			pdfCanvas.Rectangle(r);
			pdfCanvas.SetExtGState(gs);
			if (srd.BdrWidth>0) pdfCanvas.SetLineWidth(srd.BdrWidth);
			if (srd.BdrColor!= null) pdfCanvas.SetStrokeColor(srd.BdrColor);
			if (srd.FillColor != null) pdfCanvas.SetFillColor(srd.FillColor);

			pdfCanvas.FillStroke();

			pdfCanvas.RestoreState();
		}
		*/

		private Vector getRectCenter(Rectangle r)
		{
			float w = r.GetWidth() / 2;
			float h = r.GetHeight() / 2;

			double x = r.GetX() + w;
			double y = r.GetY() + h;

			return new Vector((float) x, (float) y, 0);
		}



		// current
		public void PlaceSheetRectangleRaw(PdfCanvas pdfCanvas,
			SheetRectData<SheetRectId> srd, Rectangle r, PdfPage page)
		{
			float rotation = 0;
			rotation = srd.TextBoxRotation;
			rotation = FloatOps.ToRad(rotation);
			rotation = srd.GetAdjTextRotation(srd.SheetRotation);

			rectname = srd.Id.ToString();

			pdfCanvas.SaveState();

			PdfExtGState gs = new PdfExtGState();
			if (srd.FillOpacity>0) gs.SetFillOpacity(srd.FillOpacity);
			if (srd.BdrOpacity>0) gs.SetStrokeOpacity(srd.BdrOpacity);
	
			float x = srd.Rect.GetX();
			float y = srd.Rect.GetY();
			float w = srd.Rect.GetWidth();
			float h = srd.Rect.GetHeight();

			// if (srd.TextBoxRotation % 90 != 0)
			// {
			PdfCalcTbOrigin.show = true;
			PdfCalcTbOrigin.GetTextBoxOrigin(srd, srd.SheetRotation, out x, out y);

			AffineTransform af = AffineTransform.GetRotateInstance(rotation, x, y);
			pdfCanvas.ConcatMatrix(af);

			if (srd.SheetRotation == 0 )
			{
				if ((srd.TextBoxRotation == 90 || srd.TextBoxRotation == 270)) (w, h) = (h, w);
			}
			else
			if (srd.SheetRotation == 90)
			{
				if ((srd.TextBoxRotation != 90 && srd.TextBoxRotation != 270)) (w, h) = (h, w);
			}
			else
			if (srd.SheetRotation == 270)
			{
				if ((srd.TextBoxRotation != 90 && srd.TextBoxRotation != 270))(w, h) = (h, w);
			}


			r = new Rectangle(x, y, w, h);

			CreateElement.PlaceDatum(x, y, pdfCanvas, 10, DeviceRgb.RED);

			pdfCanvas.Rectangle(r);
			pdfCanvas.SetExtGState(gs);
			if (srd.BdrWidth>0) pdfCanvas.SetLineWidth(srd.BdrWidth);
			if (srd.BdrColor!= null) pdfCanvas.SetStrokeColor(srd.BdrColor);
			if (srd.FillColor != null) pdfCanvas.SetFillColor(srd.FillColor);

			pdfCanvas.FillStroke();

			pdfCanvas.RestoreState();
		}

		public void PlaceRectangleDirect(SheetRectData<SheetRectId> srd, PdfCanvas pdfCanvas, 
			float bWidth, Color bColor, float bOpacity, Color fColor, float fOpacity)
		{
			pdfCanvas.SaveState();

			PdfExtGState gs = new PdfExtGState();
			if (fOpacity>0) gs.SetFillOpacity(fOpacity);
			if (bOpacity>0) gs.SetStrokeOpacity(bOpacity);

			float x = srd.Rect.GetX();
			float y = srd.Rect.GetY();
			float w = srd.Rect.GetWidth();
			float h = srd.Rect.GetHeight();


			PdfCalcTbOrigin.show = true;
			PdfCalcTbOrigin.GetTextBoxOrigin(srd, srd.SheetRotation, out x, out y);


			CreateElement.PlaceDatum(x, y, pdfCanvas, 10, DeviceRgb.RED);

			Rectangle r = new Rectangle(x, y, w, h);

			pdfCanvas.Rectangle(r);
			pdfCanvas.SetExtGState(gs);
			if (bWidth>0) pdfCanvas.SetLineWidth(bWidth);
			if (bColor!= null) pdfCanvas.SetStrokeColor(bColor);
			if (fColor != null) pdfCanvas.SetFillColor(fColor);

			pdfCanvas.FillStroke();

			pdfCanvas.RestoreState();
		}

		public override string ToString()
		{
			return $"this is {nameof(CreateRectangle)}";
		}
	}
}
