#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugCode;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using Settings;
using ShItextCode.ElementCreation;
using ShSheetData.ShSheetData2;
using ShTempCode.DebugCode;
using UtilityLibrary;
using Path = System.IO.Path;

#endregion

// user name: jeffs
// created:   7/10/2024 9:06:45 PM

namespace CreatePDFElements.Support
{
	public class AddToExistPdfs
	{

		private string pdfFilePath;

		private PdfDocument srcPdfDoc;
		private PdfDocument destPdfDoc ;
		private Document destDoc;

		private PdfPage pdfPage;
		private PdfCanvas pdfCanvas;
		private Canvas canvas;

		private TextSettings txs;
		private Paragraph pg;
		private PdfFont pf = PdfFontFactory.CreateFont();

		private int pageNum;

		public bool Process()
		{
			init();

			initWriter();

			merge();

			add();

			endPdf();

			return true;
		}

		private void endPdf()
		{
			pdfPage.Flush();

			destDoc.Close();
		}

		public string[] GetSheets(int idx)
		{
			return Directory.GetFiles(Program.Folders[idx], "*.pdf");
		}

		private void initWriter()
		{
			WriterProperties wp = new WriterProperties();
			wp.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
			wp.UseSmartMode();
			wp.SetFullCompressionMode(true);

			PdfWriter w = new PdfWriter(pdfFilePath, wp);

			destPdfDoc = new PdfDocument(w);
			destDoc = new Document(destPdfDoc);
		}

		private void merge()
		{
			foreach (string file in GetSheets(1))
			{
				srcPdfDoc =  new PdfDocument(new PdfReader(file));

				Debug.WriteLine($"source {Path.GetFileNameWithoutExtension(file)} | rotation {srcPdfDoc.GetPage(1).GetRotation()}");

				int numPages = srcPdfDoc.GetNumberOfPages();

				srcPdfDoc.CopyPagesTo(1, numPages, destPdfDoc);

				break;
			}
		}

		private void add()
		{

			for (int i = 1; i < destPdfDoc.GetNumberOfPages()+1; i++)
			{
				pageNum = i;

				pdfPage = destPdfDoc.GetPage(i);
				pdfPage.SetIgnorePageRotationForContent(true);

				Debug.WriteLine($"page {i} | rotation {pdfPage.GetRotation()}");

				pdfCanvas = new PdfCanvas(pdfPage);

				canvas = new Canvas(pdfCanvas, pdfPage.GetPageSize());

				addElements();
			}

			
		}

		private void addElements()
		{
			float rotation;

			DM.DbxLineEx(0, "start", 1);
			
			placeLine1(300f, 500f);
			placeRect1();
			placeRect2();
			
			placeText1();
			placeText2();
			
			placeParagraph1();
			placeParagraph2();
			placeParagraph3();

			placePolygonAnno1(100f, 1000f);

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private TextSettings getFauxTextSetting(Color color, 
			HorizontalAlignment ha, VerticalAlignment va)
		{
			TextSettings txs = new TextSettings();

			txs.FontFamily = "Arial";
			txs.TextHorizAlignment = ha;
			txs.TextVertAlignment = va;
			txs.TextColor = color;
			txs.TextSize = 18.0f;

			return txs;
		}

		private BoxSettings getFauxBoxSettings(float rotation, 
			float originX, float originY, float w, float h)
		{
			Rectangle r = new Rectangle(originX, originY, w, h);

			BoxSettings bxs = new BoxSettings(
				r, rotation, DeviceRgb.GREEN, DeviceRgb.BLUE, 
				2f, 0.7f, 0.5f, new []{3f, 1.5f});

			return bxs;
		}

		private Paragraph getSampleParagraphText(TextSettings txs)
		{
			// Style s;
			Text t;
			Text[] tx = new Text[3];

			// s = CreateSupport.MakeTextStyle(txs);

			t = new Text("this is Text line 1\n");
			t.SetFontColor(DeviceRgb.GREEN);
			t.SetFontSize(18f);
			t.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
			tx[0] = t;

			t = new Text("this is Text line 2\n");
			t.SetFontColor(DeviceRgb.BLACK);
			t.SetFontSize(24.0f);
			t.SetHorizontalAlignment(HorizontalAlignment.LEFT);
			tx[1] = t;

			
			t = new Text("this is Text line 3");
			t.SetFontColor(DeviceRgb.RED);
			t.SetFontSize(14.0f);
			t.SetBold();
			t.SetHorizontalAlignment(HorizontalAlignment.CENTER);
			tx[2] = t;

			Paragraph pg = CreateSupport.MakeParagraph(tx);

			return pg;
		}

		private Paragraph getSampleParagraphString(TextSettings txs)
		{
			string t;
			Style s = CreateSupport.MakeTextStyle(txs);
			String[] tx = new String[3];

			s = CreateSupport.MakeTextStyle(txs);

			t = "this is Text line 1\n";
			tx[0] = t;

			t = "this is Text line 2\n";
			tx[1] = t;

			
			t ="this is Text line 3";
			tx[2] = t;

			Paragraph pg = CreateSupport.MakeParagraph(tx, s);

			return pg;
		}

		private void init()
		{
			pdfFilePath = SheetDataSetConsts.SHEET_DATA_FOLDER+ProcessManager.PDF_FILE_NAME;
		}



		private void placeLine1(float x, float y)
		{
			CreateElement.PlaceDatum(x, y, pdfCanvas, 8, DeviceRgb.RED);

			CreateElement.PlaceLine(0f, 0f, x, y, pdfCanvas);
		}

		private void placeRect1()
		{
			Rectangle r = new Rectangle(500f, 200f, 300f, 100f);

			placeLine1(500f, 200f);

			CreateElement.PlaceRectangleRotated(r, pdfCanvas, 5, 1f, 20.0f, DeviceRgb.GREEN, 0.30f);
		}

		private void placeRect2()
		{
			placeLine1(400f, 1000f);

			Rectangle r = new Rectangle(400f, 1000f, 300f, 100f);

			CreateElement.PlaceRectangleRotated(r, pdfCanvas, 3, 1f, -20.0f, DeviceRgb.GREEN, 0.30f);
		}

		private void placeText1()
		{
			placeLine1(800f, 1000f);

			txs = getFauxTextSetting(DeviceRgb.GREEN,
				HorizontalAlignment.CENTER, VerticalAlignment.TOP);
			CreateElement.PlaceText("this is sample text 1", 
				800f, 1000f, 0f, canvas, txs);

		}

		private void placeText2()
		{
			placeLine1(1000f, 800f);

			txs = getFauxTextSetting(DeviceRgb.BLUE,
				HorizontalAlignment.RIGHT, VerticalAlignment.BOTTOM);

			CreateElement.PlaceText("this is sample text 2", 
				1000f, 800f, 45f, canvas, txs);
		}

		private void placeParagraph1()
		{
			pg = getSampleParagraphText(txs);

			
			placeLine1(1400f, 400f);

			CreateElement.PlaceParagraph(pg, 1400f, 400f, pageNum, 30f, 
				TextAlignment.CENTER, VerticalAlignment.MIDDLE, canvas);
		}

		private void placeParagraph2()
		{
			pg = getSampleParagraphText(txs);

			
			placeLine1(1400f, 600f);

			CreateElement.PlaceParagraph(pg, 1400f, 600f, pageNum, 30f, 
				// null, null, canvas);
				TextAlignment.RIGHT, VerticalAlignment.BOTTOM, canvas);
		}

		private void placeParagraph3()
		{
			pg = getSampleParagraphString(txs);

			placeLine1(1600f, 300f);

			CreateElement.PlaceParagraph(pg, 1600f, 300f, pageNum, -30f, 
				TextAlignment.CENTER, VerticalAlignment.MIDDLE, canvas);
		}

		private void placePolygonAnno1(float x, float y)
		{
			placeLine1(x, y);

			PdfPolyGeomAnnotation poly = makePolygon(x, y);

			CreateElement.PlacePolygonAnno(destPdfDoc, pdfPage, poly, 45.0f);

			// other polygon examples
			placePolygon11star4pt(1300f, 1000f);
			placePolygon11star5pt(1000f, 1000f);
			placePolygon11star6ptEx(1000f, 600f);
		}



	#region polygon test routines

		private void placePolygon11star5pt(float x, float y)
		{
			placeLine1(x, y);

			float[] verts = makeStar(x, y, 32.0f, 84.0f, new [] { 72f, 72f, 72f, 72f, 72f });

			PdfPolyGeomAnnotation poly = makePolygon(verts);

			CreateElement.PlacePolygonAnno(destPdfDoc, pdfPage, poly, 30.0f);
		}

		private void placePolygon11star4pt(float x, float y)
		{
			placeLine1(x, y);

			float[] verts = makeStar(x, y, 32.0f, 84.0f, 4);

			PdfPolyGeomAnnotation poly = makePolygon(verts);

			CreateElement.PlacePolygonAnno(destPdfDoc, pdfPage, poly, -30.0f);
		}

		private void placePolygon11star6ptEx(float x, float y)
		{
			placeLine1(x, y);

			float[] a = new [] { 70f, 70f, 55f, 55f, 55f, 55f };
			float[] ra = new [] { 30f, 30f, 30f, 30f, 30f, 30f };
			float[] rb = new [] { 80f, 120f, 80f, 80f, 80f, 80f };

			float[] verts = makeStar(x, y, ra, rb, a);

			PdfPolyGeomAnnotation poly = makePolygon(verts);

			CreateElement.PlacePolygonAnno(destPdfDoc, pdfPage, poly, -30.0f);
		}

		private PdfPolyGeomAnnotation makePolygon(float x1, float y1)
		{
			Color fColor = DeviceRgb.BLUE;
			Color bColor = DeviceRgb.GREEN;
			
			float bdrOp = 0.6f;
			float filOp = 0.2f;
			
			float strokeWidth = 5.0f;

			float[] dashPat = new float[] { 5, 3, 10, 3 };

			float[] verts = makeTrap1(x1, y1, 300f, 100f, 50f);

			PdfDictionary pds = getBorderStyle(strokeWidth, dashPat);

			PdfPolyGeomAnnotation poly = CreateElement.makePolygon(verts, strokeWidth/2);

			// PdfAnnotationBorder annoBdr = 
			// 	new PdfAnnotationBorder(hCorRad, vCorRad, strokeWidth, dashPat);

			return  CreateElement.decoratePolygon(
				poly, pds, fColor, filOp, bColor, bdrOp);
		}

		private PdfPolyGeomAnnotation makePolygon(float[] verts)
		{
			Color fColor = DeviceRgb.BLUE;
			Color bColor = DeviceRgb.GREEN;
			
			float bdrOp = 0.6f;
			float filOp = 0.2f;
			
			float strokeWidth = 5.0f;

			float[] dashPat = new float[] { 5, 3, 10, 3 };

			PdfDictionary pds = getBorderStyle(strokeWidth, dashPat);

			PdfPolyGeomAnnotation poly = CreateElement.makePolygon(verts, strokeWidth/2);

			return  CreateElement.decoratePolygon(
				poly, pds, fColor, filOp, bColor, bdrOp);
		}

		private PdfDictionary getBorderStyle(float bdrWidth, float[] dashPat)
		{
			PdfDictionary pds = new PdfDictionary();
			pds.Put(PdfName.Type, PdfName.Border);
			pds.Put(PdfName.W, new PdfNumber(bdrWidth));
			pds.Put(PdfName.S, PdfAnnotation.STYLE_DASHED);
			pds.Put(PdfName.D, new PdfArray (dashPat));
			
			return pds;
		}

		private float[] makeTrap1(float x, float y, float w, float h, float offset)
		{
			return new []
			{
				x          , y,
				x + w - offset, y,
				x + w      , y + h,
				x + offset    , y + h
			};
		}

		// defined number angles between
		private float[] makeStar(float x, float y, float radA, float radB, float[] a)
		{
			float[] ra = new float[a.Length];
			float[] rb = new float[a.Length];

			for (int i = 0; i < a.Length; i++)
			{
				ra[i] = radA;
				rb[i] = radB;
			}
			return makeStar(x, y, ra, rb, a);
		}

		// defined number of points with equan angle between
		private float[] makeStar(float x, float y, float radA, float radB, int pts)
		{
			float[] a = new float[pts];
			float[] ra = new float[pts];
			float[] rb = new float[pts];

			for (int i = 0; i < pts; i++)
			{
				a[i] = 360.0f / pts;

				ra[i] = radA;
				rb[i] = radB;
			}

			return makeStar(x, y, ra, rb,  a);
		}

		private float[] makeStar(float x, float y, float[] radA, float[] radB, float[] a)
		{
			float[] f = new float[a.Length * 4];

			float p1x;
			float p1y;
			float p2x;
			float p2y;

			float a1;
			float a2 = 0;

			int idx;

			for (var i = 0; i < a.Length; i++)
			{
				idx = i * 4;
				a1 = a2 + a[i] / 2;
				a2 += a[i];

				p1x = x+ (float) (Math.Cos((double) FloatOps.ToRad(a1)) * radA[i]);
				p1y = y+ (float) (Math.Sin((double) FloatOps.ToRad(a1)) * radA[i]);

				p2x = x+ (float) (Math.Cos((double) FloatOps.ToRad(a2)) * radB[i]);
				p2y = y+ (float) (Math.Sin((double) FloatOps.ToRad(a2)) * radB[i]);

				f[idx + 0] = p1x;
				f[idx + 1] = p1y;
				f[idx + 2] = p2x;
				f[idx + 3] = p2y;
			}

			return f;
		}
		

	#endregion

	}
}



// placePolygon(new [] { 300f, 1500f, 300f, 100f });

// test routines for SO question
// placePolygon0(new [] { 300f, 1500f, 300f, 100f });
// placePolygon1();
// placePolygon2();
// placePolygon3();
// placePolygon4();
// placePolygon5();
// placePolygon6();
// placePolygon7();

// CreateElement.PlacePolygon8(destPdfDoc, pdfPage);
// CreateElement.PlacePolygon9(destPdfDoc, pdfPage);

// CreateElement.PlacePolygon10(destPdfDoc, pdfPage, 500f, 1000f);

/*
private float[] getPolygonVerts1(float x, float y, float w, float h)
{
	float adj = 20f;

	float[] f  =new []
	{
		x+adj, y,
		x+w, y+h-adj,
		x+w-adj, y+h,
		x, y+adj,
		x+adj, y

	};


	return f;
}
private float[] getPolygonVerts2(float x, float y, float w, float h)
{
	float adj = 40f;

	float[] f  =new []
	{
		x, y,
		x+w-adj, y,
		x+w, y+h,
		x+adj, y+h,
		x, y

	};


	return f;
}
private void placePolygon(float[] v)
{
	float[] verts = getPolygonVerts2(v[0], v[1], v[2], v[3]);

	BoxSettings bxs = getFauxBoxSettings(15f, v[0], v[1], v[2], v[3]);

	float x = v[0];
	float y = v[1];

	placeLine1(x, y);

	CreateElement.PlacePolygon(verts, bxs, pdfPage);

}
// test routines for SO question
private void placePolygon0(float[] v)
{
	float[] verts = getPolygonVerts2(v[0], v[1], v[2], v[3]);

	BoxSettings bxs = getFauxBoxSettings(15f, v[0], v[1], v[2], v[3]);

	float x = v[0];
	float y = v[1];

	placeLine1(x, y);

	CreateElement.PlacePolygon0(verts, bxs, pdfPage, destPdfDoc, x, y);

}
private void placePolygon1()
{
	CreateElement.PlacePolygon1(pdfPage);

}
private void placePolygon2()
{
	CreateElement.PlacePolygon2(pdfPage);

}
private void placePolygon3()
{
	CreateElement.PlacePolygon3(pdfPage);

}
private void placePolygon4()
{
	CreateElement.PlacePolygon4(pdfPage);

}
private void placePolygon5()
{
	CreateElement.PlacePolygon5(pdfPage);

}
private void placePolygon6()
{
	CreateElement.PlacePolygon6(pdfPage);

}
private void placePolygon7()
{
	CreateElement.PlacePolygon7(pdfPage);
}
*/

