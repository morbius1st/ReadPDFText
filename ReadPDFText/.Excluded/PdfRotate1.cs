#region + Using Directives
using iText.Kernel.Pdf;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;

#endregion

// user name: jeffs
// created:   2/27/2024 7:20:52 PM

namespace SharedCode.ShDataSupport
{

	/*
	public class PdfRotate1
	{
		private PdfDocument destPdfDoc;
		private Document destDoc;
		private PdfDocument srcPdfDoc;
		private PdfPage page;

		private int pgCount;
		private int pageNum;

		public bool Process(string source, string dest)
		{
			Debug.WriteLine($"reading| {source} and writing to| {dest}");

			bool result = true;

			pageNum = 1;

			destPdfDoc = new PdfDocument(new PdfWriter(dest));
			destDoc = new Document(destPdfDoc);
			srcPdfDoc =  new PdfDocument(new PdfReader(source));

			pgCount = srcPdfDoc.GetNumberOfPages();

			Debug.Write($"page| ");

			for (int i = 1; i <= pgCount; i++, pageNum++)
			{
				srcPdfDoc.CopyPagesTo(i, i, destPdfDoc);

				Debug.Write($"{pageNum} copied| ");
			}

			Debug.WriteLine("\n");

			process();

			Debug.WriteLine($"closing source");

			srcPdfDoc.Close();

			Debug.WriteLine($"closing destination");
			destPdfDoc.Close();

			return result;
		}

		
		 // * rotation tests
		 // * 1a 1b change page size to 
		 // *
		 // *
		 

		private bool process()
		{
			bool result = true;

			pgCount = destPdfDoc.GetNumberOfPages();

			destPdfDoc.GetNumberOfPages();

			for (int i = 1; i <= pgCount; i++)
			{
				page = destPdfDoc.GetPage(i);
				pageSize = page.GetPageSizeWithRotation();

				// if (i == 1)
				// {
				// 	Debug.WriteLine($"\nbefore tests");
				// 	showPageInfo(i);
				// }

				Debug.WriteLine($"\nprocessing page| {i}");

				runTest(i);

				page.Flush();
			}

			return result;
		}

		private Rectangle pageSize = null;

		private void runTest(int testNumber)
		{
			int test = 2;
			doRotate = true;
			

			//switch (testNumber)
			//{

			
			//case 1:
			//	{
			//		doRotate = false;
			//		showRotation();
			//		showBoxes();
			//		if (test==1) addRectTest1();
			//		if (test==2) addAnnoRect();
			//		if (test==3) addAnnoRects();
			//		break;
			//	}

			//case 2:
			//	{
			//		Debug.WriteLine("just rotate page to 270");
			//		page.SetRotation(270);
			//		showRotation();
			//		showBoxes();
			//		if (test==1) addRectTest1();
			//		if (test==2) addAnnoRect();
			//		if (test==3) addAnnoRects();
			//		break;
			//	}

			//case 3:
			//	{
			//		showRotation();
			//		showBoxes();
			//		if (test==1) addRectTest1();
			//		if (test==2) addAnnoRect();
			//		if (test==3) addAnnoRects();
			//		break;
			//	}

			//case 8:
			//	{
			//		showRotation();
			//		showBoxes();
			//		if (test==1) addRectTest1();
			//		if (test==2) addAnnoRect();
			//		if (test==3) addAnnoRects();
			//		break;
			//	}

			//case 9:
			//	{
			//		Debug.WriteLine("just rotate page to 0");
			//		page.SetRotation(0);
			//		showRotation();
			//		showBoxes();
			//		if (test==1) addRectTest1();
			//		if (test==2) addAnnoRect();
			//		if (test==3) addAnnoRects();
			//		break;
			//	}

				

			default:
				{
					showRotation();
					showBoxes();
					if (test==1) addRectTest1();
					if (test==2) addAnnoRect();
					if (test==3) addAnnoRects();
					break;
				}

			// case 2:
			// 	{
			// 		rotateTest1();
			// 		showRotation();
			// 		showBoxes();
			// 		break;
			// 	}
			//
			// case 2:
			// 	{
			// 		rotateTest2();
			// 		showRotation();
			// 		showBoxes();
			// 		break;
			// 	}
			//
			// case 3:
			// 	{
			// 		rotateTest3();
			// 		showRotation();
			// 		showBoxes();
			// 		break;
			// 	}
			//
			// case 4:
			// 	{
			// 		rotateTest4();
			// 		showRotation();
			// 		showBoxes();
			// 		break;
			// 	}
			//
			// case 5:
			// 	{
			// 		rotateTest5();
			// 		showRotation();
			// 		showBoxes();
			// 		break;
			// 	}

			}

			
		}

		private void rotateTest1()
		{
			Debug.WriteLine("just rotate page to 270");
			page.SetRotation(270);
		}

		private void rotateTest2()
		{
			Debug.WriteLine("rotate page to 270");
			Debug.WriteLine("set media box to 17x22");
			page.SetRotation(270);
			page.SetMediaBox(makeRect(0, 0, 17, 22));
		}

		private void rotateTest3()
		{
			Debug.WriteLine("set media box to 17x22");
			page.SetMediaBox(makeRect(0, 0, 17, 22));
		}

		private void rotateTest4()
		{
			Debug.WriteLine("rotate page to 270");
			Debug.WriteLine("set media box to 17x22");
			Debug.WriteLine("set crop box to 17x22");
			page.SetRotation(270);
			page.SetMediaBox(makeRect(0, 0, 17, 22));
			page.SetCropBox(makeRect(0, 0, 17, 22));
		}

		private void rotateTest5()
		{
			Debug.WriteLine("set media box to 17x22");
			Debug.WriteLine("set crop box to 17x22");
			page.SetMediaBox(makeRect(0, 0, 17, 22));
			page.SetCropBox(makeRect(0, 0, 17, 22));
		}

		private void rotateTest6()
		{
			Debug.WriteLine("change orientation");

			PageSize ps = new PageSize(page.GetPageSize());

			ps.Rotate();
		}

		
		private void addRectTest1()
		{
			// Rectangle r = new Rectangle(15.32f, 10.05f, 1.55f, 0.83f);
			// Rectangle r = makeRect(15.32f, 10.05f, 1.55f, 0.83f);
			Rectangle r = new Rectangle(1113.66f, 9.31f, 110.0f, 58.4f);

			addLinkRectangle(r);

		}


		private Rectangle makeRect(float x, float y, float width, float height)
		{
			return new Rectangle(x*72, y*72, width*72, height*72);
		}

		private void addLinkRectangle(Rectangle rect)
		{
			pageSize = page.GetPageSize();

			PdfCanvas pdfCanvas = new PdfCanvas(page);
			CanvasGraphicsState gs = pdfCanvas.GetGraphicsState();

			Matrix a = gs.GetCtm();

			showMatrix(a);

			Debug.WriteLine("adding rectangle");

			pdfCanvas.SetFillColorRgb(75 / 255f, 214 / 255f, 1);
			pdfCanvas.SetStrokeColor(DeviceRgb.BLUE);
			pdfCanvas.SetLineDash(6, 3);
			pdfCanvas.SetLineWidth(1);
			pdfCanvas.SetExtGState(new PdfExtGState().SetFillOpacity(0.4f));

			pdfCanvas.Rectangle(rect).FillStroke();

			// pdfCanvas.Release();

		}




		private void addAnnoRects()
		{
			Debug.WriteLine("adding rectangle");
			pageSize = page.GetPageSizeWithRotation();

			PdfCanvas pdfCanvas = new PdfCanvas(page);

			addAnnoRects(pdfCanvas);
		}

		private void addAnnoRects(PdfCanvas canvas)
		{
			SheetData sd = SheetConfig.SheetConfigData[SheetBorderType.ST_CS_11X17];

			addRectangle(canvas, maybeRotateRect(new Rectangle(36,72,144,36)), DeviceRgb.RED, DeviceRgb.RED, 0.3f);


			addRectangle(canvas, maybeRotateRect(sd.TitleBlockRect[0]), DeviceRgb.WHITE, DeviceRgb.GREEN, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.SheetNumberLinkRect[0]), DeviceCmyk.MAGENTA, DeviceCmyk.MAGENTA, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.SheetNumberFindRect[0]), DeviceRgb.GREEN, DeviceRgb.GREEN, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.ContentRect[0]), DeviceCmyk.MAGENTA, DeviceCmyk.MAGENTA, 0.1f);
			addRectangle(canvas, maybeRotateRect(sd.FooterRect[0]), DeviceRgb.RED, DeviceRgb.RED, 0.2f);
			addRectangle(canvas, maybeRotateRect(sd.DisclaimerRect[0]), DeviceRgb.RED, DeviceRgb.RED, 0.2f);
			addRectangle(canvas, maybeRotateRect(sd.PrimaryAuthorRect[0]), DeviceRgb.GREEN, DeviceRgb.GREEN, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.BannerRects[0, 0][0]), DeviceRgb.BLUE, DeviceRgb.BLUE, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.BannerRects[0, 1][0]), DeviceRgb.BLUE, DeviceRgb.BLUE, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.BannerRects[1, 0][0]), DeviceRgb.BLUE, DeviceRgb.BLUE, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.BannerRects[1, 1][0]), DeviceRgb.BLUE, DeviceRgb.BLUE, 0.3f);
		}

		private bool doRotate = false;

		private Rectangle maybeRotateRect(Rectangle r)
		{

			if (!doRotate)
			{
				showInfo("before rectangle", $"{ReadPDFText.fmtRect(r)}  (no rotation)");
				return r;
			}

			showInfo("before rectangle", $"{ReadPDFText.fmtRect(r)}");


			float temp;
			float x = r.GetX();
			float y = r.GetY();
			float w = r.GetWidth();
			float h = r.GetHeight();

			(x,  y) = (-1*y, x);
			(w, h) = (h, w);

			x += pageSize.GetWidth() - w;

			Rectangle rx = new Rectangle(x, y, w, h);

			return rx;
		}


		private void addAnnoRect()
		{
			Debug.WriteLine("adding rectangle");
			PdfCanvas canvas = new PdfCanvas(page);

			Rectangle r = new Rectangle(20.0f, 20.0f, 75.0f, 150.0f);
			addRectangle(canvas, r, DeviceRgb.GREEN, DeviceRgb.GREEN, 0.3f);

			r = maybeRotateRect(r);
			addRectangle(canvas, r, DeviceRgb.RED, DeviceRgb.RED, 0.3f);

			r = new Rectangle(640.0f, 20.0f, 75.0f, 150.0f);
			addRectangle(canvas, r, DeviceRgb.GREEN, DeviceRgb.GREEN, 0.3f);
			
			r = maybeRotateRect(r);
			addRectangle(canvas, r, DeviceRgb.RED, DeviceRgb.RED, 0.3f);


		}

		private void addRectangle(PdfCanvas canvas, Rectangle r, Color strokeColor, Color fillColor, float fillOpacity = 1)
		{
			showInfo("     after rectangle", $"{ReadPDFText.fmtRect(r)}");
			
			PdfExtGState gs = new PdfExtGState().SetFillOpacity(fillOpacity);

			canvas.Rectangle(r)
			.SetExtGState(gs)
			.SetLineWidth(0)
			.SetStrokeColor(strokeColor)
			.SetFillColor(fillColor)
			.FillStroke();
		}

		private void showMatrix(Matrix m)
		{
			Debug.WriteLine($"*** matrix | {m.Get(0)} {m.Get(1)} {m.Get(2)} {m.Get(3)} {m.Get(4)} {m.Get(5)} {m.Get(6)}");
		}

		private void showPageInfo(int pageNum)
		{
			showInfo("page", $"{pageNum}");

			showRotation();
			showBoxes();
			showMargins();

			showInfo("", $"");
			showInfo("", $"");
		}

		private void showRotation()
		{
			showInfo("rotation", $"{page.GetRotation()}");
		}

		private void showBoxes()
		{
			showInfo("page size (rect)", $"{ReadPDFText.fmtRect(page.GetPageSize())}");
			showInfo("page size (rot) (rect)", $"{ReadPDFText.fmtRect(page.GetPageSizeWithRotation())}");

			// showInfo("media box (rect", $"{ReadPDFText.fmtRect(page.GetMediaBox())}");
			// showInfo("crop box (rect)", $"{ReadPDFText.fmtRect(page.GetCropBox())}");

			
			//showInfo("art box (rect)", $"{ReadPDFText.fmtRect(page.GetArtBox())}");
			//showInfo("trim box (rect)", $"{ReadPDFText.fmtRect(page.GetTrimBox())}");
			//showInfo("bleed box (rect)", $"{ReadPDFText.fmtRect(page.GetBleedBox())}");
			

		}

		private void showMargins()
		{
			string mLeft = $"{destDoc.GetLeftMargin() / 72}";
			string mTop = $"{destDoc.GetTopMargin() / 72}";
			string mRight = $"{destDoc.GetRightMargin() / 72}";
			string mBott = $"{destDoc.GetBottomMargin() / 72}";

			showInfo("margin", $"l| {mLeft,-7}| t| {mTop,-7}| r| {mRight,-7}| b| {mBott,-7}");
		}

		private void showInfo(string title, string info)
		{
			Debug.WriteLine($"{title,-30}| {info}");
		}


		public override string ToString()
		{
			return $"this is {nameof(PdfRotate1)}";
		}

	}
		*/
}
