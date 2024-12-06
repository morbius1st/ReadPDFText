#region + Using Directives

using System;
using System.Diagnostics;
// using System.Drawing;
// using System.Drawing.Text;
// using CreatePDFBoxes.PdfSupport;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using UtilityLibrary;
using Rectangle = iText.Kernel.Geom.Rectangle;
// using System.Windows.Media.Converters;
using Style = iText.Layout.Style;
using TextAlignment = iText.Layout.Properties.TextAlignment;
// using HorizontalAlignment = System.Windows.HorizontalAlignment;
using TextDecorations = ShSheetData.Support.TextDecorations;
using VerticalAlignment = iText.Layout.Properties.VerticalAlignment;
using ShItextCode.PdfCalculations;
using ShSheetData.SheetData;
using ShSheetData.ShSheetData2;
using ShSheetData.Support;

#endregion

// user name: jeffs
// created:   5/27/2024 7:16:50 PM

namespace ShItextCode.ElementCreation
{
	// revised - use this:

	// ha
	// left   = 0 => 0.0
	// center = 1 => 0.5
	// right  = 2 => 1.0

	// va
	// top    = 0 => 1.0
	// middle = 1 => 0.5
	// bottom = 2 => 0.0

	
	public class CreateText2
	{
		private string rectname;

		public PdfCanvas pdfCanvas { get; set; }

		public CreateText2(PdfCanvas pdfCanvas)
		{
			this.pdfCanvas = pdfCanvas;
		}

		public void CreateSrdText(Document doc, int pageNum,
			SheetRectData2<SheetRectId> srd)
		{
			rectname = srd.Id.ToString();
		
			float rotation;
		
			rotation = FloatOps.ToRad(srd.BoxSettings.TextBoxRotation);
			rotation = srd.GetAdjTextRotation(srd.SheetRotation);
		
			float x ;
			float y ;
		
			// x & y = box's LB corner (which for NEWS boxes, may not be its "origin")
			PdfCalcTbOrigin.show = false;
			PdfCalcTbOrigin.GetTextBoxOrigin(srd.BoxSettings.Rect, 
				srd.BoxSettings.TextBoxRotation, srd.SheetRotation, out x, out y);
		
			srd.TbOriginX = x;
			srd.TbOriginY = y;
		
		
			// if (rectname.Equals("SM_OPT7") || rectname.Equals("SM_OPT1") || rectname.Equals("SM_OPT2"))
			// if (rectname.Equals("SM_OPT2"))
			if (rectname.Equals("SM_OPT10"))
			{
				// correct answer is: 500.50 x 499.50
				PdfCalcTxOrigin2.GetTextOrigin(srd.BoxSettings.Rect, srd.TbOriginX, srd.TbOriginY,
					srd.SheetRotation, srd.BoxSettings.TextBoxRotation, srd.TextSettings, out x, out y);
			}
			else
			{
				PdfCalcTxOrigin2.GetTextOrigin(srd.BoxSettings.Rect, srd.TbOriginX, srd.TbOriginY,
					srd.SheetRotation, srd.BoxSettings.TextBoxRotation, srd.TextSettings, out x, out y);
			}
		
			string text = $"{srd.Id.ToString()} @ {x:F2} {y:F2} | {srd.BoxSettings.TextBoxRotation:F2} | {srd.TextSettings.TextHorizAlignment.ToString()[0]}-{srd.TextSettings.TextVertAlignment.ToString()[0]}";
		
			CreateTextRaw(text, doc, pageNum, srd, x, y, rotation);
		}


		public void CreateTextRaw(string text, 
			Document doc, int pageNum, SheetRectData2<SheetRectId> srd, 
			float x, float y, float rotation)
		{
			Style s = setTextStyle(srd);
		
			TextAlignment ta = (TextAlignment) (int) srd.TextSettings.TextHorizAlignment;
			VerticalAlignment va = srd.TextSettings.TextVertAlignment;
		
			CreateElement.PlaceDatum(x, y, pdfCanvas, 3);
		
			Paragraph p1 = new Paragraph(text);
		
			p1.AddStyle(s);
		
			doc.ShowTextAligned(p1, x, y, pageNum,
				ta, va, rotation);
		}


		private Style setTextStyle(SheetRectData2<SheetRectId> srd)
		{
			PdfFont pf = getPdfFont(srd);
		
			Style s = new Style();
		
			s.SetFont(pf);
			s.SetOpacity(srd.TextSettings.TextOpacity);
			s.SetFontColor(srd.TextSettings.TextColor);
			s.SetWidth(srd.TextSettings.TextWeight * 72);
			s.SetFontSize(srd.TextSettings.TextSize /2 );
		
			if (srd.TextSettings.FontStyle == 1 || srd.TextSettings.FontStyle == 3)
			{
				s.SetBold();
			}
			else if (srd.TextSettings.FontStyle == 2 || srd.TextSettings.FontStyle == 3)
			{
				s.SetItalic();
			}
		
			if (TextDecorations.HasLinethrough(srd.TextSettings.TextDecoration) )
			{
				s.SetLineThrough();
			}
		
			if (TextDecorations.HasUnderline(srd.TextSettings.TextDecoration))
			{
				s.SetUnderline();
			}
		
			return s;
		}


		private PdfFont getPdfFont(SheetRectData2<SheetRectId> srd)
		{
			string fontPath;
			PdfFont pf ;
		
			if (srd.TextSettings.FontFamily.IsVoid() ||
				srd.TextSettings.FontFamily.Equals("default"))
			{
				pf = CreateSupport.DefaultFont;
			}
			else
			{
				fontPath = CsWindowHelpers.GetFontFilePath(srd.TextSettings.FontFamily);
				pf = PdfFontFactory.CreateFont(fontPath);
			}
		
			return pf;
		}

	}
	


	public class CreateText
	{
		private static int count = 0;

		private CreateRectangle cr;
		public PdfCanvas pdfCanvas { get; set; }

		public CreateText(PdfCanvas pdfCanvas)
		{
			cr = new CreateRectangle();
			
			this.pdfCanvas = pdfCanvas;
		}

		// public void PlaceSheetText(Document doc, int pageNum,
		// 	SheetRectData<SheetRectId> srd, float sheetRotation)
		// {
		// 	Style s = setTextStyle(srd);
		//
		// 	float rotation = srd.GetAdjTextRotation(sheetRotation);
		//
		// 	TextAlignment ta = (TextAlignment) (int) srd.TextHorizAlignment;
		// 	VerticalAlignment va = srd.TextVertAlignment;
		//
		// 	rectname=$"{srd.Id}";
		//
		// 	string s1 = $"{srd.Id.ToString()}";
		// 	string s2;
		//
		// 	Debug.Write($"{s1}");
		//
		// 	Debug.WriteLine($" | {ta.ToString()[0]}-{va.ToString()[0]} | sheet rotation | initial {sheetRotation,4:F0} | adjusted {(rotation/Math.PI)*180,4:F0} | tb rotation {srd.TextBoxRotation,8:F2} ");
		// 	Debug.WriteLine($"before rect | x {srd.Rect.GetX(),8:F2} y {srd.Rect.GetY(),8:F2} w {srd.Rect.GetWidth(),8:F2} h {srd.Rect.GetHeight(),8:F2})");
		//
		//
		// 	Rectangle r = srd.Rect;
		//
		// 	// if (!rectname.Equals("SM_OPT3") || !rectname.Equals("SM_OPT4") || !rectname.Equals("SM_OPT6"))
		// 	// {
		// 	// 	r = getTextOriginPerAlignment4(srd, sheetRotation);
		// 	// }
		//
		// 	r = getTextOrigin(srd, sheetRotation);
		//
		// 	Debug.WriteLine($" after rect | x {r.GetX(),8:F2} y {r.GetY(),8:F2} w {r.GetWidth(),8:F2} h {r.GetHeight(),8:F2})\n");
		//
		// 	CreateElement.PlaceDatum(r.GetX(), r.GetY(), pdfCanvas, 3);
		//
		//
		// 	Paragraph p1 = new Paragraph(s1);
		//
		// 	p1.AddStyle(s);
		//
		// 	doc.ShowTextAligned(p1, r.GetX(), r.GetY(), pageNum,
		// 		ta, va, rotation);
		// }
		//
		//
		// private Style setTextStyle(SheetRectData<SheetRectId> srd)
		// {
		// 	PdfFont pf = getPdfFont(srd);
		//
		// 	Style s = new Style();
		//
		// 	s.SetFont(pf);
		// 	s.SetOpacity(srd.TextOpacity);
		// 	s.SetFontColor(srd.TextColor);
		// 	s.SetWidth(srd.TextWeight * 72);
		//
		// 	// s.SetFontSize(srd.TextSize);
		// 	s.SetFontSize(srd.TextSize /2 );
		//
		// 	// s.SetVerticalAlignment(srd.TextVertAlignment);
		// 	// s.SetHorizontalAlignment(srd.TextHorizAlignment);
		// 	// s.SetRotationAngle(srd.GetRotationRad(CreateSupport.PageRotation));
		//
		// 	if (srd.FontStyle == 1 || srd.FontStyle == 3)
		// 	{
		// 		s.SetBold();
		// 	}
		// 	else if (srd.FontStyle == 2 || srd.FontStyle == 3)
		// 	{
		// 		s.SetItalic();
		// 	}
		//
		// 	if (TextDecorations.HasLinethrough(srd.TextDecoration) )
		// 	{
		// 		s.SetLineThrough();
		// 	}
		//
		// 	if (TextDecorations.HasUnderline(srd.TextDecoration))
		// 	{
		// 		s.SetUnderline();
		// 	}
		//
		// 	return s;
		// }
		//
		// private PdfFont getPdfFont(SheetRectData<SheetRectId> srd)
		// {
		// 	string fontPath;
		// 	PdfFont pf ;
		//
		// 	if (srd.FontFamily.IsVoid() ||
		// 		srd.FontFamily.Equals("default"))
		// 	{
		// 		pf = CreateSupport.DefaultFont;
		// 	}
		// 	else
		// 	{
		// 		// Debug.WriteLine($"looking for| {srd.FontFamily}");
		//
		// 		fontPath = CsWindowHelpers.GetFontFilePath(srd.FontFamily);
		// 		pf = PdfFontFactory.CreateFont(fontPath);
		//
		// 		// pf = null;
		//
		// 		// CsWindowHelpers.TestApxMatch();
		//
		// 		// CsWindowHelpers.TestGetFontFile();
		// 	}
		//
		// 	return pf;
		// }


		private string rectname;

		private string wStrType;

		private string hStrType;

		private string xStart;
		private string wStrValue;
		private string xWadj;
		private string xWFinalAdj;
		private string xWIntAmt;

		private string hStrValue;
		private string xHadj;
		private string xHFinalAdj;

		private string xFinalAdj;

		private string xFinal;

		// private string xAdjFinalH;
		// private string xAdjInitH;
		// private string yWFinalAdj;
		// private string yAdjFinalH;

		private string yStart;
		private string yWadj;
		private string yWFinalAdj;
		private string yWIntAmt;

		private string yHadj;
		private string yHFinalAdj;

		private string yFinalAdj;

		private string yFinal;

		
		// private Rectangle getTextOrigin(SheetRectData<SheetRectId> srd,
		// 	float sheetRotation)
		// {
		// 	float x = srd.Rect.GetX();
		// 	float y = srd.Rect.GetY();
		// 	float w = srd.Rect.GetWidth();
		// 	float h = srd.Rect.GetHeight();
		//
		// 	float tbRotationRad = FloatOps.ToRad(sheetRotation + srd.TextBoxRotation);
		//
		// 	float sinTbRot = (float) Math.Sin(tbRotationRad);
		// 	float cosTbRot = (float) Math.Cos(tbRotationRad);
		//
		// 	PdfCalcTbOrigin.show = false;
		// 	// PdfCalcTbOrigin.GetTextBoxOrigin(srd, sheetRotation, out x, out y);
		// 	PdfCalcTbOrigin.GetTextBoxOrigin(srd.Rect, srd.TextBoxRotation, sheetRotation, out x, out y);
		//
		// 	return new Rectangle(x, y, w, h);
		// }
		//
		// private Rectangle getTextOriginPerAlignment4(SheetRectData<SheetRectId> srd,
		// 	float sheetRotation)
		// {
		// 	float[][] adj = new float[][]
		// 	{
		// 		new float[] { 0f, 0.5f, 1f },
		// 		new float[] { 1f, 0.5f, 0f }
		// 	};
		//
		//
		// 	float wAlignAdj = adj[0][(int) srd.TextHorizAlignment];
		// 	float hAlignAdj = adj[0][(int) srd.TextVertAlignment];
		//
		// 	// Debug.WriteLine($"alignments | H {srd.TextHorizAlignment.ToString()[0]} ({wAlignAdj}) | V {srd.TextVertAlignment.ToString()[0]} ({hAlignAdj})");
		//
		// 	wStrType = srd.TextHorizAlignment.ToString()[0].ToString();
		// 	wStrValue = wAlignAdj.ToString();
		// 	hStrType = srd.TextVertAlignment.ToString()[0].ToString();
		// 	hStrValue = hAlignAdj.ToString();
		//
		// 	float rotation = sheetRotation + srd.TextBoxRotation;
		// 	// rotationStr = rotation.ToString("F2");
		//
		// 	float tbRotationRad = FloatOps.ToRad(sheetRotation + srd.TextBoxRotation);
		//
		// 	float sinTbRot = (float) Math.Sin(tbRotationRad);
		// 	float cosTbRot = (float) Math.Cos(tbRotationRad);
		//
		// 	float x;
		// 	float y;
		//
		// 	PdfCalcTbOrigin.show = false;
		// 	PdfCalcTbOrigin.GetTextBoxOrigin(srd.Rect, srd.TextBoxRotation, sheetRotation, out x, out y);
		// 	// PdfCalcTbOrigin.GetTextBoxOrigin(srd, sheetRotation, out x, out y);
		//
		//
		// 	// adjusted origin worked out
		// 	// w & h need to be re-adjusted
		//
		// 	float w= srd.Rect.GetWidth();
		// 	float h= srd.Rect.GetHeight();
		//
		// 	// first "correct" the height / width
		// 	if (sheetRotation == 0)
		// 	{
		// 		if (srd.TextBoxRotation == 90) (w,h) = (h,w);
		// 	}
		// 	else
		// 	{
		// 		if (srd.TextBoxRotation != 90) (w,h) = (h,w);
		// 	}
		//
		// 	xStart = x.ToString("F2");
		//
		// 	float wAdjX = w * cosTbRot;
		//
		// 	xWadj = wAdjX.ToString("F2");
		//
		// 	float xwAdj = wAlignAdj * w * cosTbRot;
		//
		// 	xWFinalAdj = xwAdj.ToString("F2");
		//
		// 	float xWIntAmount = x + xwAdj;
		//
		// 	xWIntAmt = xWIntAmount.ToString("F2");
		//
		//
		//
		// 	float hAdjX = h * sinTbRot;
		//
		// 	xHadj = hAdjX.ToString("F2");
		//
		//
		// 	float xhAdj = hAlignAdj * h * sinTbRot;
		//
		// 	xHFinalAdj = xhAdj.ToString("F2");
		//
		//
		// 	float xAdjFinal = xwAdj + xhAdj;
		//
		// 	xFinalAdj = xAdjFinal.ToString("F2");
		//
		// 	x = x + xwAdj + xhAdj;
		// 	xFinal = x.ToString("F2");
		//
		// 	yStart = y.ToString("F2");
		//
		// 	float wAdjY = w * sinTbRot;
		//
		// 	yWadj = wAdjY.ToString("F2");
		//
		// 	float ywAdj = wAlignAdj * w * sinTbRot;
		//
		// 	yWFinalAdj = ywAdj.ToString("F2");
		//
		//
		// 	float yIntAmount = y+ ywAdj;
		//
		// 	yWIntAmt = yIntAmount.ToString("F2");
		//
		//
		//
		// 	float hAdjY = h * cosTbRot;
		//
		// 	yHadj = hAdjY.ToString("F2");
		//
		//
		// 	float yhAdj = hAlignAdj * h * cosTbRot;
		//
		// 	yHFinalAdj = yhAdj.ToString("F2");
		//
		//
		// 	float yAdjFinal = ywAdj - yhAdj;
		//
		// 	yFinalAdj = yAdjFinal.ToString("F2");
		//
		// 	y = y + ywAdj - yhAdj;
		//
		// 	yFinal = y.ToString("F2");
		//
		//
		// 	Debug.WriteLine(
		// 		$"X = {wStrType,-3}{hStrType,-3} {xStart,8} + (({wStrValue,4} {xWadj,8}) = {xWFinalAdj,8}) ({xWIntAmt,8}) + (({hStrValue,4} {xHadj,8}) = {xHFinalAdj,8}) = {xFinalAdj} = {xFinal}");
		//
		// 	Debug.WriteLine(
		// 		$"X = {wStrType,-3}{hStrType,-3} {yStart,8} + (({wStrValue,4} {yWadj,8}) = {yWFinalAdj,8}) ({yWIntAmt,8}) + (({hStrValue,4} {yHadj,8}) = {yHFinalAdj,8}) = {yFinalAdj} = {yFinal}");
		//
		// 	Debug.Write("\n");
		//
		// 	return new Rectangle(x, y, w, h);
		// }


		private string xInitBasis;
		private string yInitBasis;

		private string xFinalBasis;
		private string yFinalBasis1;
		private string yFinalBasis2;

		private string xAdjA1;
		private string xAdjB1;
		private string xADj1;
		private string xOrigFinal;

		private string yAdjA1;
		private string yAdjB1;
		private string yADj1;

		private string yAdjA2;
		private string yAdjB2;
		private string yADj2;
		private string yOrigFinal;

		// private void adjTextBoxOrigin(SheetRectData<SheetRectId> srd,
		// 	float sheetRotation, out float x, out float y)
		// {
		// 	x = srd.Rect.GetX();
		// 	y = srd.Rect.GetY();
		//
		// 	xStart = x.ToString("F2");
		// 	yStart = y.ToString("F2");
		//
		//
		// 	// adjust the starting point for the origin adjustment
		// 	// these apply 100% to page rotation of z
		// 	float adjX = 0.0f;
		// 	float adjYa = 1.0f;
		// 	float adjYb = 0.0f;
		//
		// 	float w = srd.Rect.GetWidth();
		// 	float h = srd.Rect.GetHeight();
		//
		// 	float tbRotationRad = FloatOps.ToRad(srd.TextBoxRotation);
		//
		// 	float trigX = (float) Math.Sin(tbRotationRad);
		// 	float trigYcos = (float) Math.Cos(tbRotationRad);
		// 	float trigYsin = (float) Math.Sin(tbRotationRad);
		//
		//
		// 	xInitBasis = w.ToString("F2");
		// 	yInitBasis = h.ToString("F2");
		//
		// 	Debug.WriteLine($"adjust origin");
		// 	Debug.WriteLine($"initial  | x {x,8:F2} y {y,8:F2} w {xInitBasis,8:F2} h {yInitBasis,8:F2} ");
		// 	
		// 	string n = rectname;
		//
		// 	if (sheetRotation == 0)
		// 	{
		// 		w = h;
		// 	}
		// 	else
		// 	if (sheetRotation == 90)
		// 	{
		// 		adjX = 1.0f;
		// 		adjYa = 0.0f;
		//
		// 		if (srd.TextBoxRotation != 90)
		// 		{
		// 			w = h;
		// 		}
		// 		else
		// 		{
		// 			h = w;
		// 		}
		// 	} 
		// 	else if (sheetRotation == 270)
		// 	{
		// 		adjX = 1.0f;
		// 		adjYa = 1.0f;
		// 		adjYb = 1.0f;
		//
		// 		trigX = trigYcos;
		//
		// 		if (srd.TextBoxRotation != 90)
		// 		{
		// 			(w, h) = (h, w);
		// 		}
		//
		// 	}
		//
		// 	xFinalBasis = h.ToString("F2");
		// 	yFinalBasis1 = w.ToString("F2");
		// 	yFinalBasis2 = h.ToString("F2");
		//
		//
		// 	Debug.WriteLine($"adjusted | w (x) {w,8:F2} h (x) {xFinalBasis} | w (y) {yFinalBasis1} h (y) {yFinalBasis2}");
		//
		//
		// 	// using the above, adjust the origin to the TL corner of the rectangle
		//
		//
		//
		// 	string sinAngle = trigYcos.ToString("F2");
		// 	string cosAngle = trigYsin.ToString("F2");
		//
		//
		//
		// 	float xAdj1 = h * trigX * adjX;
		//
		// 	x = x + xAdj1;
		// 	
		// 	float yAdj1 = w * trigYcos * adjYa;
		// 	float yAdj2 = h * trigYsin * adjYb;
		//
		// 	y = y + yAdj1 + yAdj2;
		//
		// 	Debug.WriteLine($" tb angle {srd.TextBoxRotation,8:F2} | sin {sinAngle,8:F2} cos {cosAngle,8:F2}");
		//
		//
		// 	xAdjA1 = trigX.ToString("F2");
		// 	xAdjB1 = adjX.ToString("F2");
		// 	xADj1 = xAdj1.ToString("F2");
		// 	xOrigFinal = x.ToString("F2");
		// 	yAdjA1 = trigYcos.ToString("F2");
		// 	yAdjB1 = adjYa.ToString("F2");
		// 	yADj1 = yAdj1.ToString("F2");
		// 	yAdjA2 = trigYsin.ToString("F2");
		// 	yAdjB2 = adjYb.ToString("F2");
		// 	yADj2 = yAdj2.ToString("F2");
		// 	yOrigFinal = y.ToString("F2");
		//
		// 	Debug.WriteLine(
		// 		$"x origin adjust {xStart,8} {xFinalBasis,8} {xAdjA1,8} {xAdjB1,8} {xADj1,8}, {xOrigFinal,8}" );
		// 	Debug.WriteLine(
		// 		$"y origin adjust {yStart,8} {yFinalBasis1,8} {yAdjA1,8} {yAdjB1,8} {yADj1,8}, {yFinalBasis2,8} {yAdjA2,8} {yAdjB2,8} {yADj2,8}  {yOrigFinal,8}" );
		// }
		//
		// private Rectangle getTextOriginPerAlignment3(SheetRectData<SheetRectId> srd, 
		// 	float sheetRotation)
		// {
		// 	Rectangle r = srd.Rect;
		// 	float textBoxRotation = srd.TextBoxRotation;
		//
		// 	if (textBoxRotation % 90 != 0)
		// 	{
		// 		r = r.MoveRight((float) (r.GetHeight() * Math.Sin(FloatOps.ToRad(srd.TextBoxRotation))));
		// 	}
		// 	
		// 	float xa = r.GetX();
		// 	float ya = r.GetY();
		//
		// 	float w = r.GetWidth();
		// 	float h = r.GetHeight();
		//
		// 	// ha
		// 	// left   = 0 => 0
		// 	// center = 1 => 0.5
		// 	// right  = 2  => 1
		//
		// 	// va
		// 	// bottom = 2 => 0;
		// 	// middle = 1 => 0.5
		// 	// top    = 0 => 1;
		//
		// 	float[][] adj = new float[][]
		// 	{
		// 		new float[] { 0f, 0.5f, 1f },
		// 		new float[] { 1f, 0.5f, 0f }
		// 	};
		//
		//
		// 	float ha = adj[0][(int) srd.TextHorizAlignment];
		// 	float va = adj[0][(int) srd.TextVertAlignment];
		//
		// 	float xAdj;
		// 	float yAdj;
		//
		// 	if (sheetRotation == 0)
		// 	{
		// 		if (textBoxRotation == 0)
		// 		{
		// 			va = adj[1][(int) srd.TextVertAlignment];
		// 		}
		// 	} 
		// 	else if (sheetRotation == 90)
		// 	{
		// 		if (textBoxRotation != 0)
		// 		{
		// 			ha = adj[1][(int) srd.TextHorizAlignment];
		// 		}
		// 	}
		// 	else // == 270
		// 	{
		// 		if (textBoxRotation == 0)
		// 		{
		// 			ha = adj[1][(int) srd.TextHorizAlignment];
		// 			va = adj[1][(int) srd.TextVertAlignment];
		// 		}
		// 		else
		// 		{
		// 			va = adj[1][(int) srd.TextVertAlignment];
		// 		}
		// 	}
		//
		// 	string m;
		//
		// 	if ((sheetRotation == 0 && textBoxRotation == 0) || (sheetRotation !=0 && textBoxRotation == 90))
		// 	{
		// 		m = "0-0";
		// 		xAdj = w * ha; // a
		// 		yAdj = h * va; // b
		// 	}
		// 	else
		// 	{
		// 		m = "?-?";
		// 		xAdj = w * va; // c
		// 		yAdj = h * ha; // d
		// 	}
		//
		//
		// 	float xaf = xa + xAdj;
		// 	float yaf = ya + yAdj;
		//
		// 	Debug.WriteLine($"| {m} | {xaf,8:F2} (= {xa,8:F2} + {xAdj,8:F2}) | {yaf,8:F2} (= {ya,8:F2} + {yAdj,8:F2})");
		//
		// 	return new Rectangle(xaf, yaf, w, h);
		//
		// }
		//
		// private Rectangle getTextOriginPerAlignment(SheetRectData<SheetRectId> srd, 
		// 	float sheetRotation)
		// {
		// 	Rectangle r = srd.Rect;
		// 	float textBoxRotation = srd.TextBoxRotation;
		//
		// 	if (textBoxRotation % 90 != 0)
		// 	{
		// 		r = r.MoveRight((float) (r.GetHeight() * Math.Sin(FloatOps.ToRad(srd.TextBoxRotation))));
		// 	}
		// 	
		// 	float xa = r.GetX();
		// 	float ya = r.GetY();
		//
		// 	float w = r.GetWidth();
		// 	float h = r.GetHeight();
		//
		// 	// ha
		// 	// left   = 0 => 0
		// 	// center = 1 => 0.5
		// 	// right  = 2  => 1
		//
		// 	// va
		// 	// bottom = 2 => 0;
		// 	// middle = 1 => 0.5
		// 	// top    = 0 => 1;
		//
		// 	float[][] adj = new float[][]
		// 	{
		// 		new float[] { 0f, 0.5f, 1f },
		// 		new float[] { 1f, 0.5f, 0f }
		// 	};
		//
		//
		// 	float ha = adj[0][(int) srd.TextHorizAlignment];
		// 	float va = adj[0][(int) srd.TextVertAlignment];
		//
		// 	float xAdj;
		// 	float yAdj;
		//
		// 	float xAdj2;
		// 	float yAdj2;
		//
		// 	string m = "   -  ";
		//
		// 	if (sheetRotation == 0)
		// 	{
		// 		if (textBoxRotation == 0  || textBoxRotation % 90 !=0)
		// 		{
		// 			m = "  0- 0";
		// 			va = adj[1][(int) srd.TextVertAlignment];
		// 		}
		// 	} 
		// 	else if (sheetRotation == 90)
		// 	{
		// 		if (textBoxRotation == 90)
		// 		{
		// 			m = " 90-90";
		// 			ha = adj[1][(int) srd.TextHorizAlignment];
		// 		}
		// 	}
		// 	else // == 270
		// 	{
		// 		if (textBoxRotation == 0  || textBoxRotation % 90 !=0)
		// 		{
		// 			m = "270- 0";
		// 			ha = adj[1][(int) srd.TextHorizAlignment];
		// 			va = adj[1][(int) srd.TextVertAlignment];
		// 		}
		// 		else
		// 		{
		// 			m = "270-90";
		// 			va = adj[1][(int) srd.TextVertAlignment];
		// 		}
		// 	}
		//
		// 	
		//
		// 	if ((sheetRotation == 0 && textBoxRotation == 0) 
		// 		|| (sheetRotation !=0 && textBoxRotation == 90) ||
		// 		(sheetRotation == 0 && textBoxRotation % 90 != 0))
		// 	{
		// 		m += " + 0-0";
		// 		xAdj = w * ha; // a
		// 		yAdj = h * va; // b
		// 	}
		// 	else
		// 	{
		// 		m += " + ?-?";
		// 		xAdj = w * va; // c
		// 		yAdj = h * ha; // d
		// 	}
		//
		// 	xAdj2 = xAdj;
		// 	yAdj2 = yAdj;
		//
		// 	if (textBoxRotation % 90 != 0)
		// 	{
		// 		float angRad = FloatOps.ToRad(textBoxRotation);
		//
		// 		xAdj2 = xAdj * (float) Math.Cos(angRad) - yAdj * (float) Math.Sin(angRad);
		// 		yAdj2 = xAdj * (float) Math.Sin(angRad) + yAdj * (float) Math.Cos(angRad);
		// 	}
		//
		//
		// 	float xaf = xa + xAdj2;
		// 	float yaf = ya + yAdj2;
		//
		// 	Debug.WriteLine($"| {m} | {xaf,8:F2} (= {xa,8:F2} + {xAdj2,8:F2}) | {yaf,8:F2} (= {ya,8:F2} + {yAdj2,8:F2}) | xAdj {xAdj,8:F2} | yAdj {yAdj,8:F2}");
		//
		// 	return new Rectangle(xaf, yaf, w, h);
		//
		// }
		//
		// private Rectangle getTextOriginPerAlignment2(SheetRectData<SheetRectId> srd, float sheetRotation)
		// {
		// 	Rectangle r = srd.Rect;
		// 	float textBoxRotation = srd.TextBoxRotation;
		//
		// 	if (textBoxRotation % 90 != 0)
		// 	{
		// 		r = r.MoveRight((float) (r.GetHeight() * Math.Sin(FloatOps.ToRad(srd.TextBoxRotation))));
		// 	}
		//
		// 	float angRad = FloatOps.ToRad(srd.TextBoxRotation);
		//
		// 	float xa = r.GetX();
		// 	float ya = r.GetY();
		//
		// 	float w = r.GetWidth();
		// 	float h = r.GetHeight();
		//
		// 	float angAdjA;
		// 	float angAdjB;
		//
		// 	// ha
		// 	// left   = 0 => 0
		// 	// center = 1 => 0.5
		// 	// right  = 2  => 1
		//
		// 	// va
		// 	// bottom = 2 => 0;
		// 	// middle = 1 => 0.5
		// 	// top    = 0 => 1;
		//
		// 	float[][] adj = new float[][]
		// 	{
		// 		new float[] { 0f, 0.5f, 1f },
		// 		new float[] { 1f, 0.5f, 0f }
		// 	};
		//
		//
		// 	float ha = adj[0][(int) srd.TextHorizAlignment];
		// 	float va = adj[0][(int) srd.TextVertAlignment];
		//
		// 	float xAdj;
		// 	float yAdj;
		//
		// 	if (sheetRotation == 0)
		// 	{
		// 		if (textBoxRotation == 0)
		// 		{
		// 			va = adj[1][(int) srd.TextVertAlignment];
		// 		}
		// 	} 
		// 	else if (sheetRotation == 90)
		// 	{
		// 		if (textBoxRotation != 0)
		// 		{
		// 			ha = adj[1][(int) srd.TextHorizAlignment];
		// 		}
		// 	}
		// 	else // == 270
		// 	{
		// 		if (textBoxRotation == 0)
		// 		{
		// 			ha = adj[1][(int) srd.TextHorizAlignment];
		// 			va = adj[1][(int) srd.TextVertAlignment];
		// 		}
		// 		else
		// 		{
		// 			va = adj[1][(int) srd.TextVertAlignment];
		// 		}
		// 	}
		//
		//
		// 	// if ((sheetRotation == 0 && textBoxRotation == 0) || sheetRotation !=0 && textBoxRotation == 90)
		// 	// {
		// 	// 	xAdj = w * ha; // a
		// 	// 	yAdj = h * va; // b
		// 	//
		// 	// 	angAdjA = MathF.Cos(angRad);
		// 	// 	angAdjB = MathF.Sin(angRad);
		// 	// }
		// 	// else
		// 	// {
		// 	// 	xAdj = w * va; // c
		// 	// 	yAdj = h * ha; // d
		// 	//
		// 	// 	angAdjB = MathF.Cos(angRad);
		// 	// 	angAdjA = MathF.Sin(angRad);
		// 	// }
		//
		// 	string m;
		//
		//
		// 	// if ((sheetRotation == 0 && textBoxRotation == 0) || sheetRotation !=0 && textBoxRotation == 90)
		// 	// {
		// 	// 	xAdj = w * ha; // a
		// 	// 	yAdj = h * va; // b
		// 	//
		// 	// 	angAdjA = MathF.Cos(angRad);
		// 	// 	angAdjB = MathF.Sin(angRad);
		// 	// }
		// 	// else
		// 	// {
		// 	// 	xAdj = w * va; // c
		// 	// 	yAdj = h * ha; // d
		// 	//
		// 	// 	angAdjB = MathF.Cos(angRad);
		// 	// 	angAdjA = MathF.Sin(angRad);
		// 	// }
		//
		//
		//
		// 	if (sheetRotation == 0)
		// 	{
		// 		if (textBoxRotation == 0)
		// 		{
		// 			m = " 0-0";
		// 			xAdj = w * ha; // a
		// 			yAdj = h * va; // b
		//
		// 			angAdjA = (float) Math.Cos(angRad);
		// 			angAdjB = (float) Math.Sin(angRad);
		// 		}
		// 		else
		// 		{
		// 			m = " 0-x";
		// 			xAdj = w * va; // c
		// 			yAdj = h * ha; // d
		// 	
		// 			angAdjB = (float) Math.Cos(angRad);
		// 			angAdjA = (float) Math.Sin(angRad);
		// 		}
		// 	}
		// 	else if (sheetRotation == 90)
		// 	{
		// 		if (textBoxRotation == 0)
		// 		{
		// 			m = "90-0";
		// 			xAdj = w * va; // c
		// 			yAdj = h * ha; // d
		// 	
		// 			angAdjB = (float) Math.Cos(angRad);
		// 			angAdjA = (float) Math.Sin(angRad);
		// 		}
		// 		else
		// 		{
		// 			m = "90-x";
		// 			xAdj = w * ha; // a
		// 			yAdj = h * va; // b
		//
		// 			angAdjA = (float) Math.Cos(angRad);
		// 			angAdjB = (float) Math.Sin(angRad);
		// 		}
		// 	}
		// 	else // 270
		// 	{
		// 		if (textBoxRotation == 0)
		// 		{
		// 			m = " ?-0";
		// 			xAdj = w * va; // c
		// 			yAdj = h * ha; // d
		// 	
		// 			angAdjB = (float) Math.Cos(angRad);
		// 			angAdjA = (float) Math.Sin(angRad);
		// 		}
		// 		else
		// 		{
		// 			m = " ?-?";
		// 			xAdj = w * ha; // a
		// 			yAdj = h * va; // b
		//
		// 			angAdjA = (float) Math.Cos(angRad);
		// 			angAdjB = (float) Math.Sin(angRad);
		// 		}
		// 	}
		//
		// 	float xax = xAdj * angAdjA;
		// 	float xay = yAdj * angAdjB;
		//
		// 	float yax = xAdj * angAdjB;
		// 	float yay = yAdj * angAdjA;
		//
		// 	float xaf = xa + xax - xay;
		// 	float yaf = ya + yax + yay;
		//
		// 	Debug.WriteLine($"| {m} |  {xaf,8:F2} (= {xa,8:F2} + {xax,8:F2} - {xay,8:F2})  | {yaf,8:F2} (= {ya,8:F2} + {yax,8:F2} + {yay,8:F2}) | xAdj {xAdj,8:F2} | yAdj {yAdj,8:F2}");
		//
		// 	return new Rectangle(xaf, yaf, w, h);
		//
		// }
		//
		//
		// private Rectangle getTextOrigin3(SheetRectData<SheetRectId> srd, Rectangle r, float textBoxRotation)
		// {
		//
		// 	// float p = CreateSupport.PdfPageRotation;
		//
		// 	// textBoxRotation += p;
		//
		//
		// 	double radNeg = FloatOps.ToRad(360 - (90 - textBoxRotation));
		// 	double radPos = FloatOps.ToRad(90 - textBoxRotation);
		//
		// 	float hAdj = 0;
		// 	float wAdj = 0;
		//
		// 	float w = r.GetWidth();
		// 	float h = r.GetHeight();
		//
		// 	float x = r.GetX();
		// 	float y = r.GetY();
		//
		// 	float xa;
		// 	float ya;
		//
		// 	SheetRectId id = srd.Id;
		//
		// 	// right = 2  => 1
		// 	// center = 1 => 0.5
		// 	// left = 0 => 0
		//
		// 	// top = 0 => 1;
		// 	// middle = 1 => 0.5
		// 	// bottom = 2 => 0;
		//
		// 	// correct for rotated text
		// 	if (textBoxRotation == 00)
		// 	{
		// 		wAdj  = (new float[] { 0f, 0.5f, 1f }) [(int) srd.TextHorizAlignment];
		// 		hAdj = (new float[] { 1f, 0.5f, 0f  }) [(int) srd.TextVertAlignment];
		// 		// wAdj = (new float[] { 1f, 0.5f, 0 }) [(int) srd.TextVertAlignment];
		// 	}
		// 	else // if (rotation == 90)
		// 	{
		// 		hAdj  = (new float[] { 0, 0.5f, 1f }) [(int) srd.TextHorizAlignment];
		// 		wAdj = (new float[] { 0, 0.5f, 1f }) [(int) srd.TextVertAlignment];
		// 	}
		//
		// 	x = x + w * wAdj;
		// 	y = y + h * hAdj;
		//
		// 	if (textBoxRotation != 0 && textBoxRotation != 90)
		// 	{
		// 		xa = (float) ((x - r.GetX()) * Math.Cos(radNeg) + (y - r.GetY()) * Math.Sin(radPos)) + r.GetX();
		// 		ya = (float) ((x - r.GetX()) * Math.Sin(radNeg) + (y - r.GetY()) * Math.Cos(radPos)) + r.GetY();
		// 	}
		// 	else
		// 	{
		// 		xa = x;
		// 		ya = y;
		// 	}
		//
		//
		// 	return new Rectangle(xa, ya, r.GetWidth(), r.GetHeight());
		//
		// 	// return new Rectangle(r.GetX() + r.GetWidth() * wAdj, r.GetY() + r.GetHeight() * hAdj, r.GetWidth(), r.GetHeight());
		//
		// 	// return new Vector(r.GetX() + r.GetWidth() * w,
		// 	// 	r.GetY() + r.GetHeight() * h, 0f);
		// }


		public override string ToString()
		{
			return $"this is {nameof(CreateText)}";
		}

		/*
		public void placeTest3(Document doc, int pageNum)
		{
			SheetRectData<SheetRectId> tsrd = makeTestText3();
			PlaceSheetText(doc, pageNum, tsrd);
		}


		public void  placeTests1(Document doc, int pageNum, Rectangle r, float rotation, int idx, bool rotate = false)
		{
			SheetRectData<SheetRectId> tsrd;
			Rectangle rx;

			tsrd = makeTestText("11 LB ", HorizontalAlignment.LEFT, VerticalAlignment.BOTTOM, rotation, r);

			rx = CreateSupport.rotatSheetRectangleIfNeeded(tsrd.Rect);
			cr.PlaceSheetRectangleRaw(pdfCanvas, tsrd, rx, rotate);
			placeCircle(rx.GetX(), rx.GetY(), 5.0);

			// r =tsrd.Rect;

			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText($"{idx++} RB ", HorizontalAlignment.RIGHT, VerticalAlignment.BOTTOM, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText($"{idx++} RT ", HorizontalAlignment.RIGHT, VerticalAlignment.TOP, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText($"{idx++} LT ", HorizontalAlignment.LEFT, VerticalAlignment.TOP, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText($"{idx++} LM ", HorizontalAlignment.LEFT, VerticalAlignment.MIDDLE, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText($"{idx++} RM ", HorizontalAlignment.RIGHT, VerticalAlignment.MIDDLE, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText($"{idx++} CT ", HorizontalAlignment.CENTER, VerticalAlignment.TOP, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText($"{idx++} CB ", HorizontalAlignment.CENTER, VerticalAlignment.BOTTOM, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText($"{idx++} CM ", HorizontalAlignment.CENTER, VerticalAlignment.MIDDLE, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);
		}

		public void  placeTests2(Document doc, int pageNum, Rectangle r, float rotation, int idx, int idx2, bool rotate = false)
		{
			SheetRectData<SheetRectId> tsrd;
			Rectangle rx;

			tsrd = makeTestText2($"{idx++} LB ", HorizontalAlignment.LEFT, VerticalAlignment.BOTTOM, rotation, r);

			if (idx2 == 0)
			{
				rx = CreateSupport.rotatSheetRectangleIfNeeded(tsrd.Rect);
				cr.PlaceSheetRectangleRaw(pdfCanvas, tsrd, rx, rotate);
				placeCircle(rx.GetX(), rx.GetY(), 5.0);
				return;
			}

			// r =tsrd.Rect;

			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText2($"{idx++} RB ", HorizontalAlignment.RIGHT, VerticalAlignment.BOTTOM, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText2($"{idx++} RT ", HorizontalAlignment.RIGHT, VerticalAlignment.TOP, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText2($"{idx++} LT ", HorizontalAlignment.LEFT, VerticalAlignment.TOP, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText2($"{idx++} LM ", HorizontalAlignment.LEFT, VerticalAlignment.MIDDLE, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText2($"{idx++} RM ", HorizontalAlignment.RIGHT, VerticalAlignment.MIDDLE, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText2($"{idx++} CT ", HorizontalAlignment.CENTER, VerticalAlignment.TOP, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText2($"{idx++} CB ", HorizontalAlignment.CENTER, VerticalAlignment.BOTTOM, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);

			tsrd = makeTestText2($"{idx++} CM ", HorizontalAlignment.CENTER, VerticalAlignment.MIDDLE, rotation, r);
			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
			// PlaceSheetText1(doc, pageNum, tsrd, 90);
			PlaceSheetText(doc, pageNum, tsrd);
		}


		private SheetRectData<SheetRectId> makeTestText(   string text, HorizontalAlignment ha, VerticalAlignment va, float rotation, Rectangle r)
		{
			SheetRectData<SheetRectId> tSrd = new SheetRectData<SheetRectId>(SheetRectType.SRT_TEXT, SheetRectId.SM_PAGE_TITLE, r);

			tSrd.TextBoxRotation = rotation;

			tSrd.FontFamily = "default";
			tSrd.TextColor = DeviceRgb.RED;
			tSrd.TextHorizAlignment = ha;
			tSrd.TextVertAlignment = va;
			tSrd.TextOpacity = 1;
			tSrd.TextSize = 8f;
			tSrd.InfoText =  $"{text}";

			tSrd.FillColor = DeviceRgb.GREEN;
			tSrd.FillOpacity = 0.3f;
			tSrd.BdrWidth = 2f;
			tSrd.BdrColor = DeviceRgb.BLUE;
			tSrd.BdrOpacity = 1;

			return tSrd;
		}

		private SheetRectData<SheetRectId> makeTestText2(string text, HorizontalAlignment ha, VerticalAlignment va, float rotation, Rectangle r)
		{
			SheetRectData<SheetRectId> tSrd = new SheetRectData<SheetRectId>(SheetRectType.SRT_TEXT, SheetRectId.SM_PAGE_TITLE, r);

			tSrd.TextBoxRotation = rotation;

			tSrd.FontFamily = "default";
			tSrd.TextColor = DeviceRgb.RED;
			tSrd.TextHorizAlignment = ha;
			tSrd.TextVertAlignment = va;
			tSrd.TextOpacity = 1;
			tSrd.TextSize = 30f;
			tSrd.InfoText =  $"{text}";

			tSrd.FillColor = DeviceRgb.GREEN;
			tSrd.FillOpacity = 0.3f;
			tSrd.BdrWidth = 2f;
			tSrd.BdrColor = DeviceRgb.BLUE;
			tSrd.BdrOpacity = 1;

			return tSrd;
		}


		private SheetRectData<SheetRectId> makeTestText3()
		{
			Rectangle r = new Rectangle(194f, 163f, 2966f, 2429f);

			SheetRectData<SheetRectId> tSrd = new SheetRectData<SheetRectId>(SheetRectType.SRT_TEXT, SheetRectId.SM_PAGE_TITLE, r);

			tSrd.TextBoxRotation = 38f;

			tSrd.FontFamily = "default";
			tSrd.TextColor = DeviceRgb.RED;
			tSrd.TextHorizAlignment = HorizontalAlignment.CENTER;
			tSrd.TextVertAlignment = VerticalAlignment.MIDDLE;
			tSrd.TextOpacity = 0.3f;
			tSrd.TextSize = 200f;
			tSrd.InfoText = "Watermark";

			tSrd.FillColor = DeviceRgb.GREEN;
			tSrd.FillOpacity = 0.3f;
			tSrd.BdrWidth = 2f;
			tSrd.BdrColor = DeviceRgb.BLUE;
			tSrd.BdrOpacity = 1;

			return tSrd;
		}
		*/

		// public void PlaceSheetText2(Document doc, int pageNum,
		// 	SheetRectData<SheetRectId> srd, bool adjAlignment = true)
		// {
		// 	Rectangle r = CreateSupport.rotatSheetRectangleIfNeeded(srd.Rect);
		//
		// 	Style s = setTextStyle(srd);
		//
		// 	Vector v = getTextOrigin(srd, r);
		//
		// 	float rotation = srd.GetRotationRad(0);
		//
		// 	if (adjAlignment) rotation = srd.GetRotationRad(CreateSupport.PageRotation);
		//
		// 	// text alignment		horizontal alignment
		// 	// right = 2			right = 2
		// 	// center = 1			center = 1
		// 	// left = 0				left = 0
		// 	TextAlignment ta = (TextAlignment) (int) srd.TextHorizAlignment;
		//
		// 	// vertical alignment
		// 	// bottom = 2
		// 	// middle = 1
		// 	// top = 0
		// 	VerticalAlignment va = srd.TextVertAlignment;
		//
		// 	if (adjAlignment) adjustTextAlignmentForRotation(srd, out ta, out va);
		//
		// 	string s1 = $"{srd.InfoText ?? "missing"}";
		//
		// 	s1 += $"{ta.ToString()[0]}-{va.ToString()[0]} -> ({srd.TextHorizAlignment.ToString()[0]}-{srd.TextVertAlignment.ToString()[0]})";
		//
		// 	Paragraph p1 = new Paragraph(s1);
		//
		//
		// 	p1.AddStyle(s);
		//
		// 	doc.ShowTextAligned(p1, v.Get(0), v.Get(1), pageNum,
		// 		ta, va, rotation);
		// }
		//
		//
		// public void PlaceSheetText1(Document doc, int pageNum,
		// 	SheetRectData<SheetRectId> srd, float adjRotation)
		// {
		// 	Style s = setTextStyle(srd);
		// 	Rectangle r = srd.Rect;
		//
		// 	TextAlignment ta = (TextAlignment) (int) srd.TextHorizAlignment;
		// 	VerticalAlignment va = srd.TextVertAlignment;
		//
		// 	float rotation = srd.GetRotationRad(adjRotation);
		//
		// 	string s1 = $"{srd.InfoText ?? "missing"}";
		// 	// s1 += $"{ta.ToString()[0]}-{va.ToString()[0]} -> ({srd.TextHorizAlignment.ToString()[0]}-{srd.TextVertAlignment.ToString()[0]}) | ({rotation:F2})";
		// 	// s1 += $"{r.GetX():F0}, {r.GetY():F0}, {r.GetWidth():F0}, {r.GetHeight():F0}";
		//
		// 	Paragraph p1 = new Paragraph(s1);
		//
		// 	p1.AddStyle(s);
		//
		// 	doc.ShowTextAligned(p1, r.GetX(), r.GetY(), pageNum,
		// 		ta, va, rotation);
		// }
		//

		// private void adjustTextAlignmentForRotation(SheetRectData<SheetRectId> srd, out TextAlignment ha, out VerticalAlignment va)
		// {
		// 	string n = srd.Id.ToString();
		// 	HorizontalAlignment h = srd.TextHorizAlignment;
		// 	VerticalAlignment v = srd.TextVertAlignment;
		// 	float r = srd.Rotation;
		//
		//
		// 	ha = (TextAlignment) (int) srd.TextHorizAlignment;
		// 	va = srd.TextVertAlignment;
		//
		//
		// 	if (srd.Rotation == 0)
		// 	{
		// 		va = (VerticalAlignment) (new int[] { 2, 1, 0 })[(int) srd.TextVertAlignment];
		// 	}
		// }


		// private Vector getTextOrigin(SheetRectData<SheetRectId> srd, Rectangle r)
		// {
		// 	float w = (new float[] { 0, 0.5f, 1f }) [(int) srd.TextHorizAlignment];
		// 	float h = (new float[] { 1f, 0.5f, 0 }) [(int) srd.TextVertAlignment];
		//
		// 	// right = 2  => 1
		// 	// center = 1 => 0.5
		// 	// left = 0 => 0
		//
		// 	// top = 0 => 1;
		// 	// middle = 1 => 0.5
		// 	// bottom = 2 => 0;
		//
		// 	return new Vector(r.GetX() + r.GetWidth() * w,
		// 		r.GetY() + r.GetHeight() * h, 0f);
		// }


		// public void  placeTests(Document doc, int pageNum)
// 		{
// 			SheetRectData<SheetRectId> tsrd;
// 			Rectangle rx;
//
// 			/*tsrd = makeTestText("01 LB ", HorizontalAlignment.LEFT, VerticalAlignment.BOTTOM);
// 			r = tsrd.Rect;
//
// 			CreatePdfSample.cr.PlaceSheetRectangleRaw(CreatePdfSample.pdfCanvas, tsrd, r);
// 			placeCircle(r.GetX(), r.GetY(), 10.0);
//
// 			PlaceSheetText1(doc, pageNum, tsrd, 0);
//
// 			tsrd = makeTestText("02 RB ", HorizontalAlignment.RIGHT, VerticalAlignment.BOTTOM);
// 			PlaceSheetText1(doc, pageNum, tsrd, 0);
//
// 			tsrd = makeTestText("03 RT ", HorizontalAlignment.RIGHT, VerticalAlignment.TOP);
// 			PlaceSheetText1(doc, pageNum, tsrd, 0);
//
// 			tsrd = makeTestText("04 LT ", HorizontalAlignment.LEFT, VerticalAlignment.TOP);
// 			PlaceSheetText1(doc, pageNum, tsrd, 0);
// */
// 			Rectangle r = new Rectangle(400f, 250f, 500f, 100f);
//
// 			tsrd = makeTestText("11 LB ", HorizontalAlignment.LEFT, VerticalAlignment.BOTTOM, 0, r);
//
// 			rx = CreateSupport.rotatSheetRectangleIfNeeded(tsrd.Rect);
// 			CreatePdfSample.cr.PlaceSheetRectangleRaw(CreatePdfSample.pdfCanvas, tsrd, rx);
// 			placeCircle(r.GetX(), r.GetY(), 10.0);
//
// 			// r =tsrd.Rect;
//
// 			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
// 			// PlaceSheetText1(doc, pageNum, tsrd, 90);
// 			PlaceSheetText(doc, pageNum, tsrd);
//
// 			tsrd = makeTestText("12 RB ", HorizontalAlignment.RIGHT, VerticalAlignment.BOTTOM, 0, r);
// 			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
// 			// PlaceSheetText1(doc, pageNum, tsrd, 90);
// 			PlaceSheetText(doc, pageNum, tsrd);
//
// 			tsrd = makeTestText("13 RT ", HorizontalAlignment.RIGHT, VerticalAlignment.TOP, 0, r);
// 			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
// 			// PlaceSheetText1(doc, pageNum, tsrd, 90);
// 			PlaceSheetText(doc, pageNum, tsrd);
//
// 			tsrd = makeTestText("14 LT ", HorizontalAlignment.LEFT, VerticalAlignment.TOP, 0, r);
// 			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
// 			// PlaceSheetText1(doc, pageNum, tsrd, 90);
// 			PlaceSheetText(doc, pageNum, tsrd);
//
// 			tsrd = makeTestText("15 LM ", HorizontalAlignment.LEFT, VerticalAlignment.MIDDLE, 0, r);
// 			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
// 			// PlaceSheetText1(doc, pageNum, tsrd, 90);
// 			PlaceSheetText(doc, pageNum, tsrd);
//
// 			tsrd = makeTestText("16 RM ", HorizontalAlignment.RIGHT, VerticalAlignment.MIDDLE, 0, r);
// 			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
// 			// PlaceSheetText1(doc, pageNum, tsrd, 90);
// 			PlaceSheetText(doc, pageNum, tsrd);
//
// 			tsrd = makeTestText("17 CT ", HorizontalAlignment.CENTER, VerticalAlignment.TOP, 0, r);
// 			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
// 			// PlaceSheetText1(doc, pageNum, tsrd, 90);
// 			PlaceSheetText(doc, pageNum, tsrd);
//
// 			tsrd = makeTestText("18 CB ", HorizontalAlignment.CENTER, VerticalAlignment.BOTTOM, 0, r);
// 			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
// 			// PlaceSheetText1(doc, pageNum, tsrd, 90);
// 			PlaceSheetText(doc, pageNum, tsrd);
//
// 			tsrd = makeTestText("19 CM ", HorizontalAlignment.CENTER, VerticalAlignment.MIDDLE, 0, r);
// 			// tsrd.Rect = getTextOrigin2(tsrd, r, 90);
// 			// PlaceSheetText1(doc, pageNum, tsrd, 90);
// 			PlaceSheetText(doc, pageNum, tsrd);
// 		}
//
//
// 		public void PlaceSheetText2(Document doc, int pageNum,
// 			SheetRectData<SheetRectId> srd)
// 		{
// 			Rectangle r = CreateSupport.rotatSheetRectangleIfNeeded(srd.Rect);
//
// 			Style s = setTextStyle(srd);
//
//
// 			Vector v = getTextOrigin(srd, r);
//
// 			float rotation = srd.GetRotationRad(CreateSupport.PageRotation);
//
// 			// Debug.WriteLine($"{count} | {rotation:F4}");
//
// 			// text alignment		horizontal alignment
// 			// right = 2			right = 2
// 			// center = 1			center = 1
// 			// left = 0				left = 0
// 			TextAlignment ta; // = (TextAlignment) (int) srd.TextHorizAlignment;
//
// 			// vertical alignment
// 			// bottom = 2
// 			// middle = 1
// 			// top = 0
// 			VerticalAlignment va; // = srd.TextVertAlignment;
//
//
// 			adjustTextAlignmentForRotation(srd, out ta, out va);
//
//
// 			string r1 = $" ({srd.Rotation:F1})";
//
// 			string s1 = $"{srd.InfoText ?? "missing"} ({++count + 100}) ({srd.Rotation:F1})";
// 			// s1 += $" ({++count + 100}))";
// 			// s1 += r1;
//
// 			s1 += $"{ta.ToString()[0]} ({srd.TextHorizAlignment.ToString()[0]}) | {va.ToString()[0]} ({srd.TextVertAlignment.ToString()[0]})";
//
//
// 			string s2 = $"{srd.InfoText ?? "missing"} ({count + 200}) ({srd.Rotation:F1})";
// 			// s2 += $" ({count + 200})";
// 			// s2 += r1;
//
//
// 			string s3 = $"{srd.InfoText ?? "missing"} ({count + 300}) ({srd.Rotation:F1})";
// 			// s3 += $" ({count + 300})";
// 			// s3 += r1;
//
// 			Paragraph p1 = new Paragraph(s1);
// 			Paragraph p2 = new Paragraph(s2);
// 			Paragraph p3 = new Paragraph();
//
// 			p1.AddStyle(s);
//
// 			doc.ShowTextAligned(p1, v.Get(0), v.Get(1), pageNum,
// 				ta, va, rotation);
//
// 			// doc.ShowTextAligned(p2, 1000f, 1000f, 1, ta, VerticalAlignment.MIDDLE, rotation);
// 			//
// 			// doc.ShowTextAligned(p3, r.GetX(), r.GetY(), 1, ta, srd.TextVertAlignment, rotation);
// 		}
	}

}