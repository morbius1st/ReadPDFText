#region + Using Directives

#endregion

// user name: jeffs
// created:   3/2/2024 6:55:53 AM

namespace SharedCode.ShDataSupport
{

	/*
	public class PdfTest1
	{
		private PdfDocument destPdfDoc;
		private Document destDoc;
		private PdfDocument srcPdfDoc;
		private PdfPage page;

		private int pgCount;
		private int pageNum;

		public bool Process(string source, string dest)
		{
			Debug.WriteLine($"\n\n****  test1  ****");
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

			pgCount = destPdfDoc.GetNumberOfPages();

			destPdfDoc.GetNumberOfPages();

			for (int i = 1; i <= pgCount; i++)
			{
				pageNum = i;
				page = destPdfDoc.GetPage(i);

				Debug.WriteLine($"\nprocessing page| {i}");

				// runTest(i);

				test1();

				page.Flush();
			}

			return result;
		}

		
		// private void runTest(int testNumber)
		// {
		// 	switch (testNumber)
		// 	{
		//
		// 	case 1:
		// 		{
		// 			showRotation();
		// 			showBoxes();
		// 			test1();
		// 			// addAnnoRects();
		// 			break;
		// 		}
		// 	}
		// 	
		// }

		private void test1()
		{
			showRotation();
			showBoxes();

			Rectangle find1 = new Rectangle(0.77f*72f, 0.57f*72f, 2.18f*72f, 2.36f*72f);
			Rectangle find2 = new Rectangle(7.63f, 8.63f, 59f, 108f);

			Rectangle find = find1;

			Rectangle ps = page.GetPageSize();
			Rectangle psr = page.GetPageSizeWithRotation();

			if (pageNum > 2) find = find2;

			find = adjustShtNumLoc(page, find);

			List<TextAndLineSegmentData> talx;
			TextAndLineSegmentData tals = getSheetNumberTextData(page, find, out talx);

			showTestData(tals, talx, find);

		}



		private TextAndLineSegmentData getSheetNumberTextData(PdfPage page, Rectangle findRect, out List<TextAndLineSegmentData> tals)
		{
			SheetNumLocationFilter201 filter =
				new SheetNumLocationFilter201(findRect 
	//, page.GetRotation()// 
	, true);

			FilteredEventListener shtNumListener = new FilteredEventListener();

			LocationTextExtractionStrategy xStrat =
				shtNumListener.AttachEventListener(new LocationTextExtractionStrategy(), filter);

			new PdfCanvasProcessor(shtNumListener).ProcessPageContent(page);

			tals = filter.tals;

			return filter.talData;
		}

		private Rectangle adjustShtNumLoc(PdfPage page, Rectangle findRect)
		{
			Rectangle ps = page.GetPageSize();
			float pageWidth = ps.GetWidth();
			float pageHeight = ps.GetHeight();

			if (pageWidth < pageHeight) return findRect;

			float x = pageWidth - findRect.GetY() - findRect.GetHeight();
			float y = findRect.GetX();
			float w = findRect.GetHeight();
			float h = findRect.GetWidth();

			return new Rectangle(x, y, w, h);
		}



		private void addAnnoRects(Rectangle r)
		{
			Debug.WriteLine("adding rectangle");
			PdfCanvas canvas = new PdfCanvas(page);
			addRectangle(canvas, r, DeviceRgb.GREEN, DeviceRgb.GREEN, 0.3f);
		}

		private void addRectangle(PdfCanvas canvas, Rectangle r, Color strokeColor, Color fillColor, float fillOpacity = 1)
		{
			PdfExtGState gs = new PdfExtGState().SetFillOpacity(fillOpacity);

			canvas.Rectangle(r)
			.SetExtGState(gs)
			.SetLineWidth(0)
			.SetStrokeColor(strokeColor)
			.SetFillColor(fillColor)
			.FillStroke();
		}



		private void showTestData(TextAndLineSegmentData tals, List<TextAndLineSegmentData> talx, Rectangle find)
		{
			Debug.WriteLine($"\n\n{"info for page",-20}| {pageNum}");
			Debug.WriteLine($"{"find rect",-20}| {ReadPDFText.fmtRect(find)}");


			Debug.WriteLine("\nreturned tals|");

			showTals(tals);

			return;

			Debug.WriteLine("\nall tals|");
			foreach (TextAndLineSegmentData t in talx)
			{
				showTals(t);
			}
		}


		private void showTals(TextAndLineSegmentData tals)
		{
			Debug.WriteLine($"{tals.Text,-20}| {ReadPDFText.fmtRect(tals.GetOArectangle())}");
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

		private void showInfo(string title, string info)
		{
			Debug.WriteLine($"{title,-25}| {info}");
		}


		public override string ToString()
		{
			return $"this is {nameof(PdfTest1)}";
		}
	}

	
	public class SheetNumLocationFilter201 : TextRegionEventFilter
	{
		private Rectangle shtNumLoc;

		public TextAndLineSegmentData talData { get; private set; }
		public List<TextAndLineSegmentData> tals { get; private set; }

		// private int pageRotation;
		private bool normalOrientationIsVertical { get; set; }

		public SheetNumLocationFilter201(Rectangle shtNumLocation, 
	//int pageRotation,

			bool normalOrientationIsVertical) : base(shtNumLocation)
		{
			// this.pageRotation = pageRotation;
			this.normalOrientationIsVertical = normalOrientationIsVertical;
			shtNumLoc = shtNumLocation;
			talData = TextAndLineSegmentData.Invalid();

			tals = new List<TextAndLineSegmentData>();
		}

		public override bool Accept(IEventData data, EventType type)
		{
			if (type.Equals(EventType.RENDER_TEXT))
			{
				TextRenderInfo ri = data as TextRenderInfo;

				string text = ri.GetText();

				// if (text.Length < 4 || text.Length > 12) return false;
				if (text.Length > 12) return false;

				// Rectangle s = shtNumLoc;
				// Rectangle r = TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine());
				// Rectangle rx = new Rectangle(r.GetX(), r.GetY(), -1 * r.GetWidth(), r.GetHeight());
				//
				// bool b1 = shtNumLoc.Contains(r);
				// bool b2 = shtNumLoc.Contains(rx);

				tals.Add(new TextAndLineSegmentData(ri.GetText(), ri.GetDescentLine(), ri.GetAscentLine(), normalOrientationIsVertical));

				if (shtNumLoc.Contains(TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine())))
				{
					Debug.Write($"\n");
					Debug.WriteLine($"sht num loc| {ReadPDFText.fmtRect(shtNumLoc)}");
					Debug.WriteLine($"found loc  | {ReadPDFText.fmtRect(TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine()))}");
					Debug.Write($"\n");

					talData = new TextAndLineSegmentData(ri.GetText(), ri.GetDescentLine(), ri.GetAscentLine(), normalOrientationIsVertical);
					return true;
				}
			}

			return false;
		}

		private void showInfo(TextRenderInfo ri 
	//, int pageRotation 
	, Rectangle r)
		{
			TextAndLineSegmentData tals = new TextAndLineSegmentData(ri.GetText(), ri.GetAscentLine(), ri.GetDescentLine(), normalOrientationIsVertical);

			Console.WriteLine($"{tals.Text}");

			// Console.WriteLine($"    test rect | {PdfText5.fmtRect(shtNumLoc)}");
			// Console.WriteLine($"     act rect | {PdfText5.fmtRect(r)}\n");

			// Console.WriteLine($"       pg rot | {pageRotation}");
			Console.WriteLine($"norm or'n vert| {normalOrientationIsVertical}");
			Console.WriteLine($"        angle | {tals.GetAngle():F4}");
			Console.WriteLine($"     top line | {tals.TSV.Get(0):F4}, {tals.TSV.Get(1):F4}  to  {tals.TEV.Get(0):F4}, {tals.TEV.Get(1):F4}");
			Console.WriteLine($"     bott line| {tals.BSV.Get(0):F4}, {tals.BSV.Get(1):F4}  to  {tals.BEV.Get(0):F4}, {tals.BEV.Get(1):F4}");
			Console.WriteLine($"     t corners| [2] | {tals.Corners[2].x:F4}, {tals.Corners[2].y:F4} | [3] | {tals.Corners[3].x:F4}, {tals.Corners[3].y:F4}");
			Console.WriteLine($"     b corners| [0] | {tals.Corners[0].x:F4}, {tals.Corners[0].y:F4} | [1] | {tals.Corners[1].x:F4}, {tals.Corners[1].y:F4}");
			Console.WriteLine($"        min x | {tals.MinX:F4}  | min y | {tals.MinY:F4}");
			Console.WriteLine($"            W | {tals.Width:F4}  |    H | {tals.Height:F4}");
			Console.WriteLine($"         run1 | {tals.run1:F4} | run2 | {tals.run2:F4} | rise1 | {tals.rise1:F4} | rise2 | {tals.rise2:F4}");
			Console.WriteLine($"         OA W | {tals.OAwidth:F4} | OA H | {tals.OAheight:F4}");
			Console.Write($"\n");
		}
	}
	*/
}
