#region + Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using iText.Commons.Utils;
using iText.Forms.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css.Resolve.Shorthand.Impl;
using Microsoft.VisualBasic;
using ShSheetData.SheetData;
using ShSheetData.ShSheetData2;
using ShSheetData.Support;
using UtilityLibrary;
using static System.Net.Mime.MediaTypeNames;
using Path = iText.Kernel.Geom.Path;
using Text = iText.Layout.Element.Text;
using Vector = System.Numerics.Vector;

#endregion

// user name: jeffs
// created:   6/8/2024 7:13:19 AM

namespace ShItextCode.ElementCreation
{

	public class CreateElement
	{
		public static void PlaceDatum( float x, float y,
			PdfCanvas canvas, int r = 3,
			Color fill = null)
		{
			if (fill == null)
			{
				fill = DeviceRgb.GREEN;
			}

			float ext = r * 1.5f;

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

		public static void PlaceLine(
			float startx, float starty,
			float endx, float endy,
			PdfCanvas canvas)
		{
			canvas.SaveState();
			canvas.SetLineWidth(0.15f);
			canvas.SetStrokeColor(DeviceRgb.RED);
			canvas.MoveTo(startx, starty);
			canvas.LineTo(endx, endy);
			canvas.FillStroke();
			canvas.RestoreState();
		}

		// do not use box settings - box settings refer to a text box
		public static void PlaceRectangleRotated(Rectangle r, PdfCanvas canvas,
			int rotationOrigin = 3, float rotation = 0,
			float bdrWidth = 0,
			Color fillColor = null, float fillOp = 0,
			Color bdrColor = null, float bdrOp = 0
			)
		{
			PdfExtGState gs = new PdfExtGState();

			gs.SetFillOpacity(fillOp == 0 ? 1 : fillOp);
			gs.SetStrokeOpacity(bdrOp == 0 ? 1 : bdrOp);

			canvas.SaveState();

			float x;
			float y;

			if (rotation != 0)
			{
				CreateSupport.GetRotationPoint(r, rotationOrigin, out x, out y);
				
				float rot = FloatOps.ToRad(rotation);

				AffineTransform af = AffineTransform.GetRotateInstance(rot, x, y);
				canvas.ConcatMatrix(af);
			}

			canvas.Rectangle(r);
			canvas.SetExtGState(gs);

			canvas.SetLineWidth(bdrWidth == 0 ? bdrWidth : 0.1f);
			canvas.SetStrokeColor(bdrColor == null ? DeviceRgb.BLACK : bdrColor);
			canvas.SetFillColor(fillColor == null ? DeviceRgb.BLACK : fillColor);

			canvas.FillStroke();

			canvas.RestoreState();
		}

		public static void PlaceText(string text, float x, float y, float rotation,
			Canvas canvas, TextSettings txs)
		{
			PdfFont pf = CreateSupport.GetPdfFont(txs.FontFamily);

			canvas.SetFont(pf);
			if (txs.TextOpacity != 0) canvas.SetOpacity(txs.TextOpacity);
			if (txs.TextColor != null) canvas.SetFontColor(txs.TextColor);
			if (txs.TextSize > 0) canvas.SetFontSize(txs.TextSize);

			if (txs.FontStyle == 1 || txs.FontStyle == 3) canvas.SetBold();
			if (txs.FontStyle == 2 || txs.FontStyle == 3) canvas.SetItalic();
			if (TextDecorations.HasLinethrough(txs.TextDecoration)) canvas.SetLineThrough();
			if (TextDecorations.HasUnderline(txs.TextDecoration)) canvas.SetUnderline();

			TextAlignment ta = (TextAlignment) (int)txs.TextHorizAlignment;
			VerticalAlignment va = txs.TextVertAlignment;

			float rotate = FloatOps.ToRad(rotation);

			canvas.ShowTextAligned(text, x, y, ta, va, rotate);
		}

		public static void PlaceParagraph(Paragraph pg, float x, float y, int pgNum, float rotation,
			TextAlignment? ta, VerticalAlignment? va, Canvas canvas)
		{
			float rotate = FloatOps.ToRad(rotation);

			canvas.ShowTextAligned(pg, x, y, pgNum, ta, va, rotate);
		}

				// make polygon part 3 - 
		public static void PlacePolygonAnno(PdfDocument doc, PdfPage page,
			PdfPolyGeomAnnotation poly, float rotation)
		{
			double rotate = FloatOps.ToRad(rotation);

			// this rectangle is a true bounding box (includes the adjustment for 
			// boundary line width)
			// PdfArray p1 = poly1.GetRectangle();

			Rectangle rBB = CreateSupport.convertArrayToRect(poly.GetRectangle());

			float[] verts = poly.GetVertices().ToFloatArray();

			PdfFormXObject xo = new PdfFormXObject(rBB);

			if (rotation != 0)
			{
				Point center = CreateSupport.getRectCenter(rBB);

				float cenX = (float) center.x;
				float cenY = (float) center.y;

				float[] mx = new float[6];
				Point[] pxIn = rBB.ToPointsArray();
				Point[] pxOut = new Point[pxIn.Length];
				AffineTransform af = AffineTransform.GetRotateInstance(-rotate, cenX, cenY);

				af.GetMatrix(mx);
				af.Transform(pxIn, 0, pxOut, 0, pxIn.Length);

				Rectangle rPoly = CreateSupport.getBoundingRect(pxOut);
				
				poly.SetRectangle(new PdfArray(rPoly));

				PdfArray pdfArray = new PdfArray(mx);

				xo.Put(PdfName.Matrix, pdfArray);

				poly.Put(new PdfName("Rotation"), new PdfNumber(rotation));
			}

			xo.Put(PdfName.FormType, new PdfNumber(1));

			PdfExtGState gs = new PdfExtGState();
			gs.Put(PdfName.Type, PdfName.ExtGState);
			gs.SetFillOpacity(poly.GetNonStrokingOpacity());
			gs.SetStrokeOpacity(poly.GetStrokingOpacity());

			PdfResources res = new PdfResources();
			res.AddExtGState(gs);

			res.SetProcSet(new PdfArray(new List<PdfObject>() { new PdfName("PDF") } ));

			xo.Put(PdfName.Resources, res.GetPdfObject());

			PdfCanvas canvas = new PdfCanvas(xo, doc);
			
			canvas.SaveState();

			CreateSupport.setDashPattern(canvas, poly.GetBorderStyle());
			// canvas.SetLineDash(dashPat[0], dashPat[1], 0);
			
			canvas.SetExtGState(gs);
			canvas.SetFillColor(poly.GetInteriorColor());
			canvas.SetStrokeColor(DeviceRgb.CreateColorWithColorSpace(poly.GetColorObject().ToFloatArray()));
			canvas.SetLineWidth(5f);
			CreateSupport.drawPath(verts, canvas);
			canvas.ClosePathFillStroke();
			canvas.RestoreState();

			poly.SetNormalAppearance(xo.GetPdfObject());
			poly.Put(PdfName.Type, PdfName.Annot);
			poly.SetFlags(PdfAnnotation.PRINT);
			poly.SetDate (new PdfString(new PdfDate(DateAndTime.Now).GetPdfObject().ToString()));

			page.AddAnnotation(poly);
		}


	#region polygon annotation support

		// make polygon part 1 - make the poly annotation
		public static PdfPolyGeomAnnotation makePolygon(float[] verts, 
			float offset, string name = null, int flag = 4)
		{
			Rectangle rBB;

			rBB = CreateSupport.getOffsetBoundingRect(verts, offset);

			PdfPolyGeomAnnotation poly =
				PdfPolyGeomAnnotation.CreatePolygon(rBB, verts);

			if (name != null) poly.SetName(new PdfString(name));
			poly.SetFlag(flag == 4 ? 4 : flag);

			return poly;
		}

		// make polygon part 2 - add the decoration information to the polygon
		public static PdfPolyGeomAnnotation decoratePolygon(PdfPolyGeomAnnotation poly,
			// PdfAnnotationBorder annoBdr, 
			PdfDictionary annoBdr, 
			Color fColor = null, float fillOp = 1.0f,
			Color bColor = null, float bdrOp = 1.0f )
		{
			poly.SetBorderStyle(annoBdr);
			poly.SetColor(bColor == null ? DeviceRgb.BLACK : bColor);
			poly.SetInteriorColor((fColor == null ? DeviceRgb.GREEN : fColor).GetColorValue());

			poly.SetOpacity(new PdfNumber(fillOp));
			poly.SetNonStrokingOpacity(fillOp);
			poly.SetStrokingOpacity(bdrOp);
			poly.Put(new PdfName("FillOpacity"), new PdfNumber(fillOp));

			// PdfDictionary b1 = poly.GetBorderStyle();
			// PdfDictionary b2 = poly.GetBorderEffect();
			// PdfArray b3 = poly.GetBorder();
			// PdfDictionary b4 = poly.GetAppearanceDictionary();
			// PdfDictionary b5 = poly.GetAppearanceObject(PdfName.BorderColor);
			// PdfArray b6 = poly.GetColorObject();
			//
			// PdfDictionary b7 = poly.GetNormalAppearanceObject();
			//
			// PdfDictionary o1 = poly.GetPdfObject();
			// PdfObject o2 = o1.Get(PdfName.Border);
			// PdfArray o3= o1.GetAsArray(PdfName.Border);
			// PdfDictionary o4= o1.GetAsDictionary(PdfName.Border);
			//
			// float[] f1 = o3.ToFloatArray();
			// float[] f2 = o3.GetAsArray(3);
			//
			// PdfDashPattern dp1 = new PdfDashPattern()

			return poly;
		}


	#endregion

		// keep temoprarly
		public static void PlacePolygon10(PdfDocument doc, PdfPage page, 
			float x1, float y1)
		{
			Color fColor = DeviceRgb.RED;
			Color bColor = DeviceRgb.BLUE;

			float bdrOp = 0.6f;
			float filOp = 0.2f;

			float bdrWidth = 5.0f;

			float rotation = 45.0f;

			double rotate = FloatOps.ToRad(rotation);

			float x = x1;
			float y = y1;
			float w = 300f;
			float h = 100f;

			float cenX = x + w / 2;
			float cenY = y + h / 2;

			float[] verts = new []
			{
				x          , y,
				x + w - 50f, y,
				x + w      , y + h,
				x + 50f    , y + h
			};

			float[] pathout;

			CreateSupport.getOffsetPath(verts, bdrWidth/2, EndType.CLOSED_POLYGON,out pathout, 10.0f);
			Rectangle rBB = CreateSupport.getBoundingRect(pathout);

			float[] mx = new float[6];
			Point[] pxIn = rBB.ToPointsArray();
			Point[] pxOut = new Point[pxIn.Length];
			AffineTransform af = AffineTransform.GetRotateInstance(-rotate, cenX, cenY);

			af.GetMatrix(mx);
			af.Transform(pxIn, 0, pxOut, 0, 4);

			Rectangle rPoly = CreateSupport.getBoundingRect(pxOut);

			PdfPolyGeomAnnotation poly =
				PdfPolyGeomAnnotation.CreatePolygon(rBB, verts);

			poly.SetRectangle(new PdfArray(rPoly));

			PdfArray pdfArray = new PdfArray(mx);

			PdfFormXObject xo = new PdfFormXObject(rBB);
			xo.Put(PdfName.Matrix, pdfArray);
			xo.Put(PdfName.FormType, new PdfNumber(1));

			PdfDictionary pds = new PdfDictionary();
			pds.Put(PdfName.Type, PdfName.Border);
			pds.Put(PdfName.W, new PdfNumber(5));
			pds.Put(PdfName.S, PdfAnnotation.STYLE_DASHED);
			pds.Put(PdfName.D, new PdfArray (new float[] { 5, 3, 10, 3 }));

			PdfExtGState gs = new PdfExtGState();
			gs.Put(PdfName.Type, PdfName.ExtGState);
			gs.SetFillOpacity(filOp);
			gs.SetStrokeOpacity(bdrOp);
			gs.SetLineWidth(bdrWidth);
			
			PdfResources res = new PdfResources();
			res.AddExtGState(gs);

			res.SetProcSet(new PdfArray(new List<PdfObject>() { new PdfName("PDF") } ));

			xo.Put(PdfName.Resources, res.GetPdfObject());

			PdfCanvas canvas = new PdfCanvas(xo, doc);
			
			canvas.SaveState();
			canvas.SetExtGState(gs);
			canvas.SetLineDash(new float[]{5, 3, 10, 3}, 0);
			canvas.SetFillColor(fColor);
			canvas.SetStrokeColor(bColor);
			CreateSupport.drawPath(verts, canvas);
			canvas.ClosePathFillStroke();
			canvas.RestoreState();

			poly.SetNormalAppearance(xo.GetPdfObject());
			poly.SetBorderStyle(pds);
			poly.SetColor(bColor);
			poly.SetInteriorColor(fColor.GetColorValue());
			poly.SetStrokingOpacity(bdrOp);
			poly.Put(new PdfName("FillOpacity"), new PdfNumber(filOp));
			poly.Put(new PdfName("Rotation"), new PdfNumber(rotation));
			poly.Put(PdfName.Type, PdfName.Annot);
			poly.SetFlags(PdfAnnotation.PRINT);
			poly.SetDate (new PdfString(new PdfDate(DateAndTime.Now).GetW3CDate()));
			poly.SetName(new PdfString("Poly 10"));

			page.AddAnnotation(poly);
		}
	}
}


		
		/* 		public static void PlaceRectangleOriginRotation(Rectangle r, PdfCanvas canvas,
			float bdrWidth = 0,
			float rotation = 0,
			Color fillColor = null,
			float fillOp = 0,
			Color bdrColor = null,
			float bdrOp = 0
			)
		{
			PdfExtGState gs = new PdfExtGState();

			gs.SetFillOpacity(fillOp == 0 ? 1 : fillOp);
			gs.SetStrokeOpacity(bdrOp == 0 ? 1 : bdrOp);

			canvas.SaveState();

			if (rotation != 0)
			{
				float rot = FloatOps.ToRad(rotation);
				AffineTransform af = AffineTransform.GetRotateInstance(rot, r.GetX(), r.GetY());
				canvas.ConcatMatrix(af);
			}

			PlaceDatum(r.GetX(), r.GetY(), canvas, 10, DeviceRgb.RED);

			canvas.Rectangle(r);
			canvas.SetExtGState(gs);

			canvas.SetLineWidth(bdrWidth == 0 ? bdrWidth : 0.1f);
			canvas.SetStrokeColor(bdrColor == null ? DeviceRgb.BLACK : bdrColor);
			canvas.SetFillColor(fillColor == null ? DeviceRgb.BLACK : fillColor);

			canvas.FillStroke();

			canvas.RestoreState();
		}
		*/

		//
		//
		//
		// // polygon is not rotated
		// public static void PlacePolygon(float[] verts, BoxSettings bxs, PdfPage pdfPage)
		// {
		// 	PdfPolyGeomAnnotation poly =
		// 		CreateSupport.MakePolygonAnno(verts);
		//
		// 	if (bxs.BdrColor != null) poly.SetColor(bxs.BdrColor);
		// 	if (bxs.FillColor != null) poly.SetInteriorColor(bxs.FillColor.GetColorValue());
		// 	if (bxs.BdrDashPattern != null) poly.SetDashPattern(new PdfArray(bxs.BdrDashPattern));
		// 	if (bxs.FillOpacity != 0) poly.SetOpacity(new PdfNumber(bxs.FillOpacity));
		// 	if (bxs.BdrOpacity != 0) poly.SetStrokingOpacity(bxs.BdrOpacity);
		// 	if (bxs.BdrWidth > 0) poly.SetBorder(new PdfArray(new float[] { 0, 0, bxs.BdrWidth }));
		//
		// 	pdfPage.AddAnnotation(poly);
		// }
		//
		// public static void PlacePolygon0(float[] verts,
		// 	BoxSettings bxs, PdfPage pdfPage, PdfDocument pdfDocument, float rotX, float rotY)
		// {
		// 	PdfPolyGeomAnnotation poly =
		// 		CreateSupport.MakePolygonAnno(verts);
		//
		// 	PdfFormXObject xo = null;
		//
		// 	if (bxs.BdrColor != null) poly.SetColor(bxs.BdrColor);
		// 	if (bxs.FillColor != null) poly.SetInteriorColor(bxs.FillColor.GetColorValue());
		// 	if (bxs.BdrDashPattern != null) poly.SetDashPattern(new PdfArray(bxs.BdrDashPattern));
		// 	if (bxs.FillOpacity != 0) poly.SetOpacity(new PdfNumber(bxs.FillOpacity));
		// 	if (bxs.BdrOpacity != 0) poly.SetStrokingOpacity(bxs.BdrOpacity);
		//
		// 	if (bxs.TextBoxRotation != 0)
		// 	{
		// 		float rot = FloatOps.ToRad(bxs.TextBoxRotation);
		// 		AffineTransform af =
		// 			AffineTransform.GetRotateInstance(rot, rotX, rotY);
		//
		// 		float[] mx = new float[6];
		//
		// 		af.GetMatrix(mx);
		// 		mx[4] = -671f;
		// 		mx[5] = -1286f;
		//
		// 		PdfArray pdfArray = new PdfArray(mx);
		//
		//
		// 		PdfArray pdfArray2 = new PdfArray(new float[] { bxs.Rect.GetX() - 100f, bxs.Rect.GetY() - 100f, 500f, 500f });
		//
		// 		Rectangle bb = new Rectangle( bxs.Rect.GetX() - 100f, bxs.Rect.GetY() - 100f, 500f, 500f);
		//
		// 		Collection<KeyValuePair<PdfName, PdfObject> > col =
		// 			new Collection<KeyValuePair<PdfName, PdfObject>>();
		//
		// 		col.Add(new KeyValuePair<PdfName, PdfObject>(PdfName.BBox, pdfArray2));
		// 		col.Add(new KeyValuePair<PdfName, PdfObject>(PdfName.Matrix, pdfArray));
		// 		// col.Add(new KeyValuePair<PdfName, PdfObject>(PdfName.Rotate, new PdfNumber(rot)));
		//
		// 		// PdfDictionary d2 = poly.get
		// 		// d2.Put(PdfName.BBox, pdfArray2);
		// 		// d2.Put(PdfName.Matrix, pdfArray);
		// 		//
		// 		// poly.SetNormalAppearance(d2);
		// 		//
		// 		// poly.Put(PdfName.Type, PdfName.Annot);
		//
		// 		poly.Put(new PdfName("Rotation"), new PdfNumber(bxs.TextBoxRotation));
		//
		// 		// poly.Put(PdfName.Matrix, pdfArray);
		//
		// 		xo = new PdfFormXObject(bb);
		// 		xo.Put(PdfName.Matrix, new PdfArray(pdfArray));
		// 	}
		//
		// 	PdfExtGState gs3 = new PdfExtGState();
		// 	gs3.SetFillOpacity(.6f);
		// 	gs3.SetStrokeOpacity(0.7f);
		// 	gs3.Put(PdfName.Type, PdfName.ExtGState);
		//
		//
		// 	PdfExtGState gs2 = new PdfExtGState(gs3.GetPdfObject());
		//
		//
		// 	// PdfDictionary pd2 = new PdfDictionary(gs3.GetPdfObject());
		// 	// PdfDictionary pd1 = new PdfDictionary();
		// 	// pd1.Put(PdfName.ExtGState, pd2);
		//
		// 	// level 2 /Exstate dictionary
		// 	PdfDictionary pd1 = new PdfDictionary();
		// 	pd1.Put(new PdfName("BBGS"), gs2.GetPdfObject());
		//
		//
		// 	// extstate dictionary
		// 	PdfExtGState gs = new PdfExtGState();
		// 	gs.SetLineWidth(2f);
		// 	gs.Put(PdfName.ExtGState, pd1);
		// 	gs.Put(PdfName.Type, PdfName.ExtGState);
		// 	gs.Put(PdfName.BorderColor, new PdfArray(new float[] { 1, 0, 0 }));
		// 	gs.Put(PdfName.Color, new PdfArray(new float[] { 0, 0, 1 }));
		//
		// 	// level 1 - dictionary of type /extstate
		// 	PdfResources res = new PdfResources(gs.GetPdfObject());
		// 	res.SetProcSet(new PdfArray(new List<PdfObject>() { new PdfName("PDF") } ));
		//
		//
		// 	xo.Put(PdfName.Resources, res.GetPdfObject());
		//
		// 	poly.SetNormalAppearance(xo.GetPdfObject());
		//
		// 	poly.SetFlag(0);
		// 	poly.SetName(new PdfString("poly1"));
		//
		// 	poly.Put(PdfName.Type, PdfName.Annot);
		//
		// 	pdfPage.AddAnnotation(poly);
		// }
		//
		// // rotation of polygon version 1
		// public static void PlacePolygon2(PdfPage pdfPage)
		// {
		// 	float rotation = 15.0f;
		// 	float x = 700f;
		// 	float y = 1000f;
		// 	float w = 300f;
		// 	float h = 100f;
		//
		// 	float[] verts = new []
		// 	{
		// 		x          , y,
		// 		x + w - 50f, y,
		// 		x + w      , y + h,
		// 		x + 50f    , y + h,
		// 		x          , y
		// 	};
		//
		// 	Rectangle r = new Rectangle(verts[0], verts[1], w, h);
		//
		// 	PdfPolyGeomAnnotation poly =
		// 		PdfPolyGeomAnnotation.CreatePolygon(r, verts);
		//
		// 	PdfFormXObject xo = null;
		//
		// 	poly.SetColor(DeviceRgb.RED);
		// 	poly.SetInteriorColor(DeviceRgb.GREEN.GetColorValue());
		// 	poly.SetOpacity(new PdfNumber(0.4));
		// 	poly.SetStrokingOpacity(0.7f);
		//
		//
		// 	// this section is intended to mimic the
		// 	// information for a manually placed
		// 	// rotated polygon
		// 	// start here
		//
		// 	float rot = FloatOps.ToRad(rotation);
		// 	AffineTransform af =
		// 		AffineTransform.GetRotateInstance(rot, verts[0], verts[1]);
		//
		// 	float[] mx = new float[6];
		//
		// 	af.GetMatrix(mx);
		// 	// mx[4] = -671f;
		// 	// mx[5] = -1286f;
		//
		// 	PdfArray pdfArray = new PdfArray(mx);
		//
		// 	PdfArray pdfArray2 = new PdfArray(new float[] { r.GetX() - 100f, r.GetY() - 100f, 500f, 500f });
		//
		// 	Rectangle bb = new Rectangle( r.GetX() - 100f, r.GetY() - 100f, 500f, 500f);
		//
		// 	Collection<KeyValuePair<PdfName, PdfObject> > col =
		// 		new Collection<KeyValuePair<PdfName, PdfObject>>();
		//
		// 	col.Add(new KeyValuePair<PdfName, PdfObject>(PdfName.BBox, pdfArray2));
		// 	col.Add(new KeyValuePair<PdfName, PdfObject>(PdfName.Matrix, pdfArray));
		//
		// 	poly.Put(new PdfName("Rotation"), new PdfNumber(rotation));
		//
		// 	xo = new PdfFormXObject(bb);
		// 	xo.Put(PdfName.Matrix, new PdfArray(pdfArray));
		//
		// 	PdfExtGState gs3 = new PdfExtGState();
		// 	gs3.SetFillOpacity(.6f);
		// 	gs3.SetStrokeOpacity(0.7f);
		// 	gs3.Put(PdfName.Type, PdfName.ExtGState);
		//
		// 	PdfExtGState gs2 = new PdfExtGState(gs3.GetPdfObject());
		//
		// 	PdfDictionary pd1 = new PdfDictionary();
		// 	pd1.Put(new PdfName("BBGS"), gs2.GetPdfObject());
		//
		// 	PdfExtGState gs = new PdfExtGState();
		// 	gs.SetLineWidth(2f);
		// 	gs.Put(PdfName.ExtGState, pd1);
		// 	gs.Put(PdfName.Type, PdfName.ExtGState);
		// 	gs.Put(PdfName.BorderColor, new PdfArray(new float[] { 1, 0, 0 }));
		// 	gs.Put(PdfName.Color, new PdfArray(new float[] { 0, 0, 1 }));
		//
		// 	PdfResources res = new PdfResources(gs.GetPdfObject());
		// 	res.SetProcSet(new PdfArray(new List<PdfObject>() { new PdfName("PDF") } ));
		//
		// 	xo.Put(PdfName.Resources, res.GetPdfObject());
		//
		// 	poly.SetNormalAppearance(xo.GetPdfObject());
		//
		// 	// end here
		//
		// 	poly.SetFlag(0);
		// 	poly.SetName(new PdfString("poly1"));
		//
		// 	poly.Put(PdfName.Type, PdfName.Annot);
		//
		//
		// 	pdfPage.AddAnnotation(poly);
		// }
		//
		// // ordinary, non-rotated, polygon
		// public static void PlacePolygon1(PdfPage pdfPage)
		// {
		// 	float rotation = 15.0f;
		// 	float x = 200f;
		// 	float y = 1000f;
		// 	float w = 300f;
		// 	float h = 100f;
		//
		// 	float[] verts = new []
		// 	{
		// 		x          , y,
		// 		x + w - 50f, y,
		// 		x + w      , y + h,
		// 		x + 50f    , y + h,
		// 		x          , y
		// 	};
		//
		// 	Rectangle r = new Rectangle(verts[0], verts[1], w, h);
		//
		// 	PdfPolyGeomAnnotation poly =
		// 		PdfPolyGeomAnnotation.CreatePolygon(r, verts);
		//
		//
		// 	poly.SetColor(DeviceRgb.RED);
		// 	poly.SetInteriorColor(DeviceRgb.GREEN.GetColorValue());
		// 	poly.SetOpacity(new PdfNumber(0.4));
		// 	poly.SetStrokingOpacity(0.7f);
		//
		// 	pdfPage.AddAnnotation(poly);
		// }
		//
		// // rotation of polygon version 2
		// public static void PlacePolygon3(PdfPage pdfPage)
		// {
		// 	float rotation = 15.0f;
		// 	float x = 1200f;
		// 	float y = 1000f;
		// 	float w = 300f;
		// 	float h = 100f;
		//
		// 	float[] verts = new []
		// 	{
		// 		x          , y,
		// 		x + w - 50f, y,
		// 		x + w      , y + h,
		// 		x + 50f    , y + h,
		// 		x          , y
		// 	};
		//
		// 	Rectangle r = new Rectangle(verts[0], verts[1], w, h);
		//
		// 	PdfPolyGeomAnnotation poly =
		// 		PdfPolyGeomAnnotation.CreatePolygon(r, verts);
		//
		// 	poly.SetColor(DeviceRgb.RED);
		// 	poly.SetInteriorColor(DeviceRgb.GREEN.GetColorValue());
		// 	poly.SetOpacity(new PdfNumber(0.4));
		// 	poly.SetStrokingOpacity(0.7f);
		//
		// 	poly.Put(PdfName.Rotate, new PdfNumber(15.0));
		//
		// 	pdfPage.AddAnnotation(poly);
		// }
		//
		// // rotation of polygon version 3
		// public static void PlacePolygon4(PdfPage pdfPage)
		// {
		// 	float rotation = 15.0f;
		// 	float x = 1700f;
		// 	float y = 1000f;
		// 	float w = 300f;
		// 	float h = 100f;
		//
		// 	float[] verts = new []
		// 	{
		// 		x          , y,
		// 		x + w - 50f, y,
		// 		x + w      , y + h,
		// 		x + 50f    , y + h,
		// 		x          , y
		// 	};
		//
		// 	Rectangle r = new Rectangle(verts[0], verts[1], w, h);
		//
		// 	PdfPolyGeomAnnotation poly =
		// 		PdfPolyGeomAnnotation.CreatePolygon(r, verts);
		//
		// 	poly.SetColor(DeviceRgb.RED);
		// 	poly.SetInteriorColor(DeviceRgb.GREEN.GetColorValue());
		// 	poly.SetOpacity(new PdfNumber(0.4));
		// 	poly.SetStrokingOpacity(0.7f);
		//
		// 	poly.Put(new PdfName("Rotation"), new PdfNumber(15.0));
		//
		// 	pdfPage.AddAnnotation(poly);
		// }
		//
		// // rotation of polygon version 3
		// public static void PlacePolygon5(PdfPage pdfPage)
		// {
		// 	float rotation = 45.0f;
		// 	float x = 1700f;
		// 	float y = 1000f;
		// 	float w = 300f;
		// 	float h = 100f;
		//
		// 	float[] verts = new []
		// 	{
		// 		415.2474f, 1462.807f,
		// 		768.4167f, 1462.807f,
		// 		855.2338f, 1581.654f,
		// 		506.2289f, 1581.654f,
		// 	};
		//
		// 	Rectangle r = new Rectangle(436.59376f, 1323.6034f, 833.8678f, 1720.8775f);
		//
		// 	PdfPolyGeomAnnotation poly =
		// 		PdfPolyGeomAnnotation.CreatePolygon(r, verts);
		//
		// 	poly.SetColor(DeviceRgb.RED);
		// 	poly.SetInteriorColor(DeviceRgb.GREEN.GetColorValue());
		// 	poly.SetOpacity(new PdfNumber(0.4));
		// 	poly.SetStrokingOpacity(0.7f);
		//
		// 	poly.Put(new PdfName("Rotation"), new PdfNumber(rotation));
		// 	poly.Put(PdfName.Matrix, new PdfArray(new float[] { .707107f, -.707107f, .707107f, .707107f, -414.23497f, -1462.307f}));
		//
		// 	pdfPage.AddAnnotation(poly);
		// }
		//
		// public static void PlacePolygon6(PdfPage pdfPage)
		// {
		// 	float rotation = 45.0f;
		//
		//
		// 	float[] verts = new []
		// 	{
		// 		415.2474f, 1462.807f,
		// 		768.4167f, 1462.807f,
		// 		855.2338f, 1581.654f,
		// 		506.2289f, 1581.654f,
		// 	};
		//
		// 	Rectangle bbr = new Rectangle(436.59376f, 1323.6034f, 833.8678f, 1720.8775f);
		//
		// 	PdfPolyGeomAnnotation poly =
		// 		PdfPolyGeomAnnotation.CreatePolygon(bbr, verts);
		//
		// 	PdfFormXObject xo = null;
		//
		// 	poly.SetColor(DeviceRgb.RED);
		// 	poly.SetInteriorColor(DeviceRgb.GREEN.GetColorValue());
		// 	poly.SetOpacity(new PdfNumber(0.4));
		// 	poly.SetStrokingOpacity(0.7f);
		//
		// 	PdfArray pdfArray = new PdfArray(new float[] { .707107f, -.707107f, .707107f, .707107f, -414.23497f, -1462.307f});
		//
		// 	// PdfArray bb = new PdfArray(new float[] {436.59376f, 1323.6034f, 833.8678f, 1720.8775f});
		//
		// 	// Collection<KeyValuePair<PdfName, PdfObject> > col =
		// 	// 	new Collection<KeyValuePair<PdfName, PdfObject>>();
		// 	//
		// 	// col.Add(new KeyValuePair<PdfName, PdfObject>(PdfName.BBox, bb));
		// 	// col.Add(new KeyValuePair<PdfName, PdfObject>(PdfName.Matrix, pdfArray));
		//
		// 	poly.Put(new PdfName("Rotation"), new PdfNumber(rotation));
		//
		// 	xo = new PdfFormXObject(bbr);
		// 	xo.Put(PdfName.Matrix, new PdfArray(pdfArray));
		//
		// 	// PdfExtGState gs3 = new PdfExtGState();
		// 	// gs3.SetFillOpacity(.6f);
		// 	// gs3.SetStrokeOpacity(0.7f);
		// 	// gs3.Put(PdfName.Type, PdfName.ExtGState);
		// 	//
		// 	// PdfExtGState gs2 = new PdfExtGState(gs3.GetPdfObject());
		// 	//
		// 	// PdfDictionary pd1 = new PdfDictionary();
		// 	// pd1.Put(new PdfName("BBGS"), gs2.GetPdfObject());
		//
		// 	PdfExtGState gs = new PdfExtGState();
		//
		// 	gs.Put(PdfName.Type, PdfName.ExtGState);
		// 	gs.Put(PdfName.BorderColor, new PdfArray(new float[] { 1, 0, 0 }));
		// 	gs.Put(PdfName.Color, new PdfArray(new float[] { 0, 0, 1 }));
		//
		//
		// 	PdfDictionary dxo = new PdfDictionary();
		//
		//
		//
		// 	PdfDictionary d = new PdfDictionary();
		// 	d.Put(PdfName.XObject, (PdfObject) xo.GetPdfObject());
		// 	d.Put(PdfName.ExtGState, gs.GetPdfObject());
		//
		// 	PdfResources res = new PdfResources(d);
		//
		// 	// res.AddExtGState(gs.GetPdfObject());
		// 	
		// 	
		//
		// 	xo.Put(PdfName.Resources, res.GetPdfObject());
		//
		// 	poly.SetNormalAppearance(xo.GetPdfObject());
		//
		// 	// end here
		//
		// 	poly.SetFlag(0);
		// 	poly.SetName(new PdfString("poly1"));
		//
		// 	poly.Put(PdfName.Type, PdfName.Annot);
		//
		//
		// 	pdfPage.AddAnnotation(poly);
		// }
		//
		// public static void PlacePolygon7(PdfPage pdfPage)
		// {
		//
		// 	float rotation = 45.0f;
		// 	float x = 400f;
		// 	float y = 1000f;
		// 	float w = 300f;
		// 	float h = 100f;
		//
		// 	float[] verts = new []
		// 	{
		// 		x          , y,
		// 		x + w - 50f, y,
		// 		x + w      , y + h,
		// 		x + 50f    , y + h
		// 	};
		//
		// 	Rectangle r = new Rectangle(verts[0], verts[1], w, h);
		//
		// 	PdfPolyGeomAnnotation poly =
		// 		PdfPolyGeomAnnotation.CreatePolygon(r, verts);
		//
		// 	PdfFormXObject xo = null;
		// 	PdfFormXObject xo2 = null;
		//
		// 	PdfStream ps;
		//
		// 	poly.SetColor(DeviceRgb.RED);
		// 	poly.SetInteriorColor(DeviceRgb.GREEN.GetColorValue());
		// 	poly.SetOpacity(new PdfNumber(0.4));
		// 	poly.SetStrokingOpacity(0.7f);
		// 	
		// 	PdfArray pdfArray = new PdfArray(new float[] { .707107f, -.707107f, .707107f, .707107f, -1000f, 10f});
		// 	PdfArray pdfArray2 = new PdfArray(new float[] { .707107f, -.707107f, .707107f, .707107f, -700f, -1000f});
		//
		//
		// 	poly.Put(new PdfName("Rotation"), new PdfNumber(rotation));
		//
		// 	xo = new PdfFormXObject(r);
		// 	
		//
		// 	bool b = xo.GetPdfObject().IsEmpty();
		// 	ps = xo.GetPdfObject();
		// 	int l = ps.GetLength();
		// 	PdfOutputStream os = ps.GetOutputStream();
		// 	b = xo.GetPdfObject().IsStream();
		//
		// 	xo.SetModified();
		//
		//
		// 	b = xo.GetPdfObject().IsEmpty();
		// 	ps = xo.GetPdfObject();
		// 	l = ps.GetLength();
		// 	os = ps.GetOutputStream();
		// 	b = xo.GetPdfObject().IsStream();
		//
		// 	xo.Flush();
		//
		// 	b = xo.GetPdfObject().IsEmpty();
		// 	ps = xo.GetPdfObject();
		// 	l = ps.GetLength();
		// 	os = ps.GetOutputStream();
		// 	b = xo.GetPdfObject().IsStream();
		//
		// 	// xo.Put(PdfName.Matrix, new PdfArray(pdfArray));
		//
		// 	// xo.SetBBox(new PdfArray(new float[] {verts[0], verts[1], verts[4], verts[5]}));
		//
		// 	xo2 = new PdfFormXObject(r);
		// 	xo2.Put(PdfName.Matrix, new PdfArray(pdfArray));
		// 	xo2.Put(PdfName.Length, new PdfNumber(98));
		//
		//
		// 	PdfExtGState gs = new PdfExtGState();
		// 	gs.SetLineWidth(2f);
		// 	gs.Put(PdfName.Type, PdfName.ExtGState);
		// 	gs.Put(PdfName.BorderColor, new PdfArray(new float[] { 1, 0, 0 }));
		// 	gs.Put(PdfName.Color, new PdfArray(new float[] { 0, 0, 1 }));
		// 	gs.SetFillOpacity(0.7f);
		// 	gs.SetStrokeOpacity(0.5f);
		//
		//
		// 	PdfResources res = new PdfResources();
		// 	res.AddExtGState(gs.GetPdfObject());
		// 	
		// 	res.AddForm(xo2);
		//
		// 	xo.Put(PdfName.Resources, res.GetPdfObject());
		//
		// 	poly.SetNormalAppearance(xo.GetPdfObject());
		//
		// 	xo.Put(PdfName.Matrix, new PdfArray(pdfArray));
		//
		// 	// end here
		// 	poly.SetDate(new PdfString(DateTime.UtcNow.ToString()));
		// 	poly.SetFlag(0);
		// 	poly.SetName(new PdfString("poly1"));
		//
		// 	poly.Put(PdfName.Type, PdfName.Annot);
		//
		//
		// 	pdfPage.AddAnnotation(poly);
		// }
		//
		// // do not modify - this works
		// public static void PlacePolygon8(PdfDocument doc, PdfPage page)
		// {
		//
		// 	float rotation = 45.0f;
		// 	
		//
		// 	float x = 400f;
		// 	float y = 1000f;
		// 	float w = 300f;
		// 	float h = 100f;
		//
		// 	float[] verts = new []
		// 	{
		// 		x          , y,
		// 		x + w - 50f, y,
		// 		x + w      , y + h,
		// 		x + 50f    , y + h
		// 	};
		//
		//
		// 	Rectangle rPoly = new Rectangle(400.8004f, 900.8004f, 298.3995f, 298.3995f);
		// 	Rectangle rBB = new Rectangle(394.5f, 995.5f, 311f, 110f);
		//
		// 	PdfPolyGeomAnnotation poly =
		// 		PdfPolyGeomAnnotation.CreatePolygon(rPoly, verts);
		//
		// 	// added
		//
		// 	PdfArray pdfArrayPath = new PdfArray(new float[]
		// 	{
		// 		verts[0], verts[1],
		// 		verts[2], verts[3],
		// 		verts[4], verts[5],
		// 		verts[6], verts[7],
		//
		// 	});
		//
		// 	poly.SetPath(pdfArrayPath);
		// 	poly.SetColor(DeviceRgb.RED);
		// 	poly.SetInteriorColor(DeviceRgb.GREEN.GetColorValue());
		// 	poly.SetOpacity(new PdfNumber(0.4));
		// 	poly.SetStrokingOpacity(0.7f);
		// 	// added
		//
		// 	PdfFormXObject xo = new PdfFormXObject(rBB);
		// 	
		//
		// 	PdfArray pdfArray = new PdfArray(new float[] { .707107f, -.707107f, .707107f, .707107f, -982.1713f, -204.3536f});
		//
		// 	xo.Put(PdfName.Matrix, pdfArray);
		//
		//
		// 	// added
		//
		// 	PdfFormXObject xo2 = null;
		// 	xo2 = new PdfFormXObject(rBB);
		//
		// 	xo2.Put(PdfName.Matrix, new PdfArray(pdfArray));
		// 	
		// 	PdfExtGState gs = new PdfExtGState();
		// 	gs.SetLineWidth(2f);
		// 	gs.Put(PdfName.Type, PdfName.ExtGState);
		// 	// gs.Put(PdfName.BorderColor, new PdfArray(new float[] { 1, 0, 0 }));
		// 	// gs.Put(PdfName.Color, new PdfArray(new float[] { 0, 0, 1 }));
		// 	gs.SetFillOpacity(0.7f);
		// 	gs.SetStrokeOpacity(0.5f);
		// 	
		//
		// 	PdfResources res = new PdfResources();
		// 	res.AddExtGState(gs.GetPdfObject());
		// 	
		// 	res.AddForm(xo2);
		//
		// 	poly.GetPath();
		//
		// 	xo.Put(PdfName.Resources, res.GetPdfObject());
		// 	
		//
		// 	PdfCanvas canvas = new PdfCanvas(xo, doc);
		// 	canvas.SetExtGState(gs);
		// 	canvas.SetFillColor(DeviceRgb.GREEN);
		// 	canvas.SetStrokeColor(DeviceRgb.RED);
		// 	canvas.SetLineWidth(1);
		// 	canvas.MoveTo(verts[0], verts[1]);
		// 	canvas.LineTo(verts[2], verts[3]);
		// 	canvas.LineTo(verts[4], verts[5]);
		// 	canvas.LineTo(verts[6], verts[7]);
		// 	canvas.ClosePathFillStroke();
		//
		// 	// canvas.AddXObjectAt(xo2, x, y);
		// 	// canvas.ConcatMatrix( new PdfArray(pdfArray));
		//
		// 	// added
		// 	poly.Put(new PdfName("Rotation"), new PdfNumber(rotation));
		//
		// 	poly.SetNormalAppearance(xo.GetPdfObject());
		// 	
		//
		// 	poly.Put(PdfName.Type, PdfName.Annot);
		//
		// 	poly.SetFlags(PdfAnnotation.PRINT);
		// 	poly.SetDate(new PdfString(DateTime.UtcNow.ToString()));
		// 	poly.SetName(new PdfString("poly1"));
		//
		// 	page.AddAnnotation(poly);
		//
		// 	
		// 	
		// }
		//
		// public static void PlacePolygon9(PdfDocument doc, PdfPage page)
		// {
		// 	float bbMarg = 6;
		//
		// 	float rotation = 45.0f;
		// 	double rotate = FloatOps.ToRad(rotation);
		//
		// 	float x = 400f;
		// 	float y = 1000f;
		// 	float w = 300f;
		// 	float h = 100f;
		//
		// 	float[] verts = new []
		// 	{
		// 		x          , y,
		// 		x + w - 50f, y,
		// 		x + w      , y + h,
		// 		x + 50f    , y + h
		// 	};
		//
		// 	float[] vertBB = new []
		// 	{
		// 		verts[0]-bbMarg, verts[1]-bbMarg,
		// 		verts[2]-bbMarg, verts[3]+bbMarg,
		// 	};
		//
		// 	Rectangle rPoly = new Rectangle(400.8004f, 900.8004f, 298.3995f, 298.3995f);
		// 	// Rectangle rBB = new Rectangle(394.5f, 995.5f, 311f, 110f);
		// 	Rectangle rBB = new Rectangle(400f, 1000f, 300f, 100f);
		// 	Rectangle rBB_wMarg = rBB.ApplyMargins(bbMarg, bbMarg, bbMarg, bbMarg, true);
		//
		// 	float cenX = x + w / 2;
		// 	float cenY = y + h / 2;
		//
		// 	// note that the affine trans needs to be reversed direction
		// 	AffineTransform af = AffineTransform.GetRotateInstance(-rotate, cenX, cenY);
		// 	float[] mx = new float[6];
		// 	Point[] pxIn = rBB_wMarg.ToPointsArray();
		// 	Point[] pxOut = new Point[pxIn.Length];
		// 	;
		// 	af.GetMatrix(mx);
		// 	af.Transform(pxIn, 0, pxOut, 0, 4);
		// 	
		// 	PdfPolyGeomAnnotation poly =
		// 		PdfPolyGeomAnnotation.CreatePolygon(rPoly, verts);
		//
		// 	PdfArray pdfArrayPath;
		// 	// pdfArrayPath = new PdfArray(new float[]
		// 	// {
		// 	// 	verts[0], verts[1],
		// 	// 	verts[2], verts[3],
		// 	// 	verts[4], verts[5],
		// 	// 	verts[6], verts[7],
		// 	//
		// 	// });
		//
		// 	pdfArrayPath = new PdfArray(verts);
		//
		//
		// 	// failed
		// 	// List<Point> pts = new List<Point>();
		// 	// pts.Add(new Point(verts[0], verts[1]));
		// 	// pts.Add(new Point(verts[2], verts[3]));
		// 	// pts.Add(new Point(verts[4], verts[5]));
		// 	// pts.Add(new Point(verts[6], verts[7]));
		// 	// Rectangle rBx = Rectangle.CalculateBBox(pts);
		//
		// 	
		//
		//
		// 	poly.SetPath(pdfArrayPath);
		// 	poly.SetColor(DeviceRgb.RED);
		// 	poly.SetInteriorColor(DeviceRgb.BLUE.GetColorValue());
		// 	poly.SetOpacity(new PdfNumber(0.4));
		// 	poly.SetStrokingOpacity(0.7f);
		// 	// added
		//
		// 	PdfFormXObject xo = new PdfFormXObject(rBB);
		// 	
		// 	// PdfArray pdfArray = new PdfArray(new float[] { .707107f, -.707107f, .707107f, .707107f, -982.1713f, -204.3536f});
		//
		//
		// 	PdfArray pdfArray = new PdfArray(mx);
		// 	xo.Put(PdfName.Matrix, pdfArray);
		// 	
		// 	// added
		//
		// 	PdfFormXObject xo2 = null;
		// 	xo2 = new PdfFormXObject(rBB);
		//
		// 	xo2.Put(PdfName.Matrix, new PdfArray(pdfArray));
		// 	
		// 	PdfExtGState gs = new PdfExtGState();
		// 	gs.SetLineWidth(2f);
		// 	gs.Put(PdfName.Type, PdfName.ExtGState);
		// 	// gs.Put(PdfName.BorderColor, new PdfArray(new float[] { 1, 0, 0 }));
		// 	// gs.Put(PdfName.Color, new PdfArray(new float[] { 0, 0, 1 }));
		// 	gs.SetFillOpacity(0.7f);
		// 	gs.SetStrokeOpacity(0.5f);
		// 	
		//
		// 	PdfResources res = new PdfResources();
		// 	res.AddExtGState(gs.GetPdfObject());
		// 	
		// 	res.AddForm(xo2);
		//
		// 	poly.GetPath();
		//
		// 	xo.Put(PdfName.Resources, res.GetPdfObject());
		// 	
		//
		// 	PdfCanvas canvas = new PdfCanvas(xo, doc);
		// 	canvas.SetExtGState(gs);
		// 	canvas.SetFillColor(DeviceRgb.BLUE);
		// 	canvas.SetStrokeColor(DeviceRgb.RED);
		// 	canvas.SetLineWidth(1);
		// 	canvas.MoveTo(verts[0], verts[1]);
		// 	canvas.LineTo(verts[2], verts[3]);
		// 	canvas.LineTo(verts[4], verts[5]);
		// 	canvas.LineTo(verts[6], verts[7]);
		// 	canvas.ClosePathFillStroke();
		//
		// 	// canvas.AddXObjectAt(xo2, x, y);
		// 	// canvas.ConcatMatrix( new PdfArray(pdfArray));
		//
		// 	// added
		// 	poly.Put(new PdfName("Rotation"), new PdfNumber(rotation));
		//
		// 	poly.SetNormalAppearance(xo.GetPdfObject());
		// 	
		//
		// 	poly.Put(PdfName.Type, PdfName.Annot);
		//
		// 	poly.SetFlags(PdfAnnotation.PRINT);
		// 	poly.SetDate(new PdfString(DateTime.UtcNow.ToString()));
		//
		// 	// per pdf specs, no need for name
		// 	// poly.SetName(new PdfString("poly1"));
		//
		// 	page.AddAnnotation(poly);
		//
		// 	
		// 	
		// }
