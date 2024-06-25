#region + Using Directives

#endregion

// user name: jeffs
// created:   2/27/2024 7:20:52 PM

namespace SharedCode.ShDataSupport
{
	/*

	public class PdfRotate2
	{
		private PdfDocument destPdfDoc;
		private Document destDoc;
		private PdfDocument srcPdfDoc;
		private PdfPage page;

		private int pgCount;
		private int pageNum;

		private bool doRotate = false;

		private bool useAltRotation = false;

		private int sheetDataTypeIdx = 0;

		
		private SheetData[] sheetDataTypes = new SheetData[]
		{
			SheetConfig.SheetConfigData[SheetBorderType.ST_AO_36X48],
			SheetConfig.SheetConfigData[SheetBorderType.ST_CS_11X17],
			SheetConfig.SheetConfigData[SheetBorderType.ST_CS_11X17],
		};


		public bool Process(string source, string dest)
		{

			// SheetConfig.ShowSheetData();
			//
			// return true;

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

		
		 //* rotation tests
		 //* 1a 1b change page size to 
		 //*
		 //*
		 

		private bool process()
		{
			bool result = true;
			int useAltShtDataIdx = 4;

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

				if (i == useAltShtDataIdx)
				{
					useAltRotation = true;
					sheetDataTypeIdx = 0;
				}

				runTest(i);

				sheetDataTypeIdx++;

				page.Flush();
			}

			return result;
		}

		private Rectangle pageSize = null;

		private void runTest(int pageNumber)
		{
			int test = 3;
			doRotate = checkRotate();
			
			switch (pageNumber)
			{

			default:
				{
					showRotation();
					showBoxes();
					if (test==1) addRectTest1();
					if (test==2) addAnnoRect();
					if (test==3) addAnnoRects();
					break;
				}
			}

			
		}



		private bool checkRotate()
		{
			bool result = true;

			Rectangle ps = page.GetPageSizeWithRotation();
			float rotation = page.GetRotation();

			if (ps.GetWidth() > ps.GetHeight() && rotation == 270)
			{
				result = false;
			}

			Debug.WriteLine($"check rotate| ps width| {ps.GetWidth()} | ps height| {ps.GetHeight()}| rotation| {rotation}");

			return result;
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

			addAnnoRects(pdfCanvas, sheetDataTypes[sheetDataTypeIdx]);
		}

		private void addAnnoRects(PdfCanvas canvas, SheetData sd)
		{
			 Debug.WriteLine($"do rotate| {doRotate,-6}| sht type idx| {sheetDataTypeIdx}");

			Rectangle[] r = new [] { new Rectangle(36, 72, 144, 36), rotateRectangle(new Rectangle(36, 72, 144, 36)) };

			addRectangle(canvas, maybeRotateRect(r), DeviceRgb.RED, DeviceRgb.RED, 0.3f);


			addRectangle(canvas, maybeRotateRect(sd.TitleBlockRect), DeviceRgb.WHITE, DeviceRgb.GREEN, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.SheetNumberLinkRect), DeviceCmyk.MAGENTA, DeviceCmyk.MAGENTA, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.SheetNumberFindRect), DeviceRgb.GREEN, DeviceRgb.GREEN, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.ContentRect), DeviceCmyk.MAGENTA, DeviceCmyk.MAGENTA, 0.1f);
			addRectangle(canvas, maybeRotateRect(sd.FooterRect), DeviceRgb.RED, DeviceRgb.RED, 0.2f);
			addRectangle(canvas, maybeRotateRect(sd.DisclaimerRect), DeviceRgb.RED, DeviceRgb.RED, 0.2f);
			addRectangle(canvas, maybeRotateRect(sd.PrimaryAuthorRect), DeviceRgb.GREEN, DeviceRgb.GREEN, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.BannerRects[0, 0]), DeviceRgb.BLUE, DeviceRgb.BLUE, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.BannerRects[0, 1]), DeviceRgb.BLUE, DeviceRgb.BLUE, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.BannerRects[1, 0]), DeviceRgb.BLUE, DeviceRgb.BLUE, 0.3f);
			addRectangle(canvas, maybeRotateRect(sd.BannerRects[1, 1]), DeviceRgb.BLUE, DeviceRgb.BLUE, 0.3f);
		}

		

		private Rectangle maybeRotateRect(Rectangle[] r)
		{

			if (!doRotate)
			{
				// showInfo("before rectangle", $"{ReadPDFText.fmtRect(r[0])}  (no rotation)");
				return r[0];
			}

			int shtDataIdx = 0;

			if (useAltRotation) { shtDataIdx = 1; }

			// showInfo("before rectangle", $"{ReadPDFText.fmtRect(r[shtDataIdx])}");

			Debug.WriteLine($"\talt rotate| {useAltRotation,-6}| sht data idx| {shtDataIdx}");

			return rotateRectangle(r[shtDataIdx]);
		}

		private Rectangle rotateRectangle(Rectangle r)
		{
			if (useAltRotation) return r;

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

			Rectangle[] r = new [] { new Rectangle(20.0f, 20.0f, 75.0f, 150.0f), rotateRectangle( new Rectangle(20.0f, 20.0f, 75.0f, 150.0f)) };
			Rectangle rx;

			addRectangle(canvas, r[0], DeviceRgb.GREEN, DeviceRgb.GREEN, 0.3f);
			addRectangle(canvas, r[1], new DeviceRgb(255, 0, 255), new DeviceRgb(255, 0, 255), 0.3f);

			rx = maybeRotateRect(r);
			addRectangle(canvas, rx, DeviceRgb.RED, DeviceRgb.RED, 0.3f);

			rx = new Rectangle(640.0f, 20.0f, 75.0f, 150.0f);
			addRectangle(canvas, rx, DeviceRgb.GREEN, DeviceRgb.GREEN, 0.3f);
			
			rx = maybeRotateRect(r);
			addRectangle(canvas, rx, DeviceRgb.RED, DeviceRgb.RED, 0.3f);


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
