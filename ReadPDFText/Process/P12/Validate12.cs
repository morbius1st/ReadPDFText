#region + Using Directives
using CommonPdfCodeShCode;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using SharedCode.ShDataSupport.PdfSupport;
using SharedCode.ShDataSupport;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using iText.Kernel.Geom;
using ReadPDFText.PdfSupport;

#endregion

// user name: jeffs
// created:   4/16/2024 7:40:28 PM

namespace ReadPDFText.Process.P12
{
	public class Validate12 
	{
		private int pageNum;
		private PdfDocument destPdfDoc ;

		private PdfText12 p12;

		// private int rectangleIdx = 0;
		// private bool? doPageRotate = false;

		private bool normalPgOrientationIsVert;


		public static List<PdfTreeLeaf> ValidationFailList;
		public static List<PdfTreeLeaf> RotationFailList;

		private PdfCanvasProcessor parser;


		public Validate12(PdfDocument destPdfDoc, 
			int shtNumMinLen, int shtNumMaxLen, 
			bool normalPgOrientationIsVert, PdfText12 pdf12)
		{
			p12 = pdf12;

			this.destPdfDoc= destPdfDoc;
			this.shtNumMinLen = shtNumMinLen;
			this.shtNumMaxLen = shtNumMaxLen;

			this.normalPgOrientationIsVert = normalPgOrientationIsVert;

			cfg = PdfText12.cfg;

			ClipperBridge.floatMultiplier = Math.Pow(10, 12);

			ValidationFailList = new List<PdfTreeLeaf>();
			RotationFailList = new List<PdfTreeLeaf>();
		}

		public  MergePdfConfig cfg { get; set; }

		public int shtNumMaxLen { get; set; } = 0;
		public int shtNumMinLen { get; set; } = 100;

	#region step 2 - validate (& get sht nums)

		// run once per leaf / page
		public bool validate(PdfTreeLeaf leaf)
		{
			// Debug.WriteLine("\n\n********************\n");
			// Debug.Write($"validating| {leaf.SheetNumber} - {leaf.SheetName}\n");

			// this is a "b u g" fix - stops Itext from throwing an exception
			

			Console.Write(".");

			bool result = true;
			PdfPage page;
			Rectangle shtNumFindRect;

			pageNum = leaf.PageNumber;
			
			page = destPdfDoc.GetPage(pageNum);
			page.SetIgnorePageRotationForContent(true);

			if (PdfText12.SHOW_DEBUG_INFO) showPage(page, leaf);

			// part 1
/*
			result = !cfg.CorrectSheetRotation || adjustRotation(page);

			if (!result)
			{
				RotationFailList.Add(leaf);
				// return false;
				// Debug.WriteLine($"\t\tSheet rotation failed");
				Console.Write("r");
			}
*/

			// part 2

			// p12.checkIfRotate(page);

			Rectangle r = null;


			Debug.WriteLine($"at get sht number| rotation {p12.pageRotation}");
			
			
			if (p12.pageRotation == 0)
			{
				Rectangle ps = page.GetPageSize();
			
				r = leaf.SheetData.SheetNumberFindRect[0];
			
				float w = r.GetHeight();
				float h = r.GetWidth();
			
				float x = ps.GetWidth()-r.GetY() - w;
				// float y = ps.GetHeight() - r.GetX() - h;
			
				float y = r.GetX();
			
				r = new Rectangle(x, y, w, h);
			}
			else
			if (p12.pageRotation == 90)
			{
				Rectangle ps = page.GetPageSize();
			
				r = leaf.SheetData.SheetNumberFindRect[0];
			
				Debug.WriteLine($"before find sht num rect| {SharedCode.ShDataSupport.ReadPDFText.fmtRect(r,1)}");
				Debug.WriteLine($"before find pg size| w {ps.GetWidth()} h {ps.GetHeight()}");
			
				float h = r.GetHeight();
				float w = r.GetWidth();
			
				float x = ps.GetWidth()-r.GetX() - w;
			
				float y = ps.GetHeight() - r.GetY() - h;
			
				r = new Rectangle(x, y, w, h);
			}
			else
			{
				r = leaf.SheetData.SheetNumberFindRect[p12.rectangleIdx];
			}


			
			Debug.WriteLine($"after find sht num rect| {SharedCode.ShDataSupport.ReadPDFText.fmtRect(r,1)}");
			


			// this is the current method
			leaf.SheetNumberTals = getSheetNumberTextData(page, r, false);
			// leaf.SheetNumberTals = getSheetNumberTextData(page, leaf.SheetData.SheetNumberFindRect[rectangleIdx], false);

			// if (leaf.SheetNumberTals == null)
			// {
				// Debug.WriteLine("got null");

				// Debug.WriteLine($"sheet data name| {leaf.SheetData.Name}");
				// Debug.WriteLine($"sheet size| W {leaf.SheetData.Width:F3} x H {leaf.SheetData.Height:F3}");
				// Debug.WriteLine($"rectangle is| x {r.GetX()} | y {r.GetY()} | w {r.GetWidth()} | h {r.GetHeight()}");
				//
				//
				// Debug.WriteLine("r1 - primary rectangle");
				//
				// Rectangle rx = leaf.SheetData.SheetNumberFindRect[0];
				// TextAndLineSegmentData a = getSheetNumberTextData(page, rx);
				//
				// Debug.WriteLine("r2 - alt x & y - bigger rect");
				// rx = new Rectangle(r.GetX(), r.GetY(), 300f, 300f);
				// a = getSheetNumberTextData(page, rx);
				//
				// Debug.WriteLine("r3 - primary x & y - bigger rect");
				// rx = new Rectangle(rx.GetX(), rx.GetY(), 300f, 300f);
				// a = getSheetNumberTextData(page, rx);
				//
				// Debug.WriteLine("r4 - alt rectangle");
				// rx = leaf.SheetData.SheetNumberFindRect[1];
				// a = getSheetNumberTextData(page, rx);
				//
				// Debug.WriteLine("r5 - primary x & y - bigger rect");
				// rx = new Rectangle(rx.GetX(), rx.GetY(), 300f, 300f);
				// a = getSheetNumberTextData(page, rx);
				//
				//
				// Debug.WriteLine("r6 - primary x & y - x bigger rect");
				// rx = new Rectangle(rx.GetX(), rx.GetY(), 500f, 500f);
				// a = getSheetNumberTextData(page, rx);

			// }

			// List<TextAndLineSegmentData> tals;
			// leaf.SheetNumberTals = getSheetNumberTextData(page, leaf.SheetData.SheetNumberFindRect[rectangleIdx], out tals);


			// this may be better
			// getSheetNumberInfo(page, leaf.SheetData.SheetNumberFindRect[rectangleIdx]);


			// nope
			// getPageText(page);
			// getPageText2(page);
			// GetTextExtractor(page);
			// GetTextExtrator2(page);


			// Debug.WriteLine($"  | found| {leaf.SheetNumberTals.Text}");
			

			if (leaf.SheetNumberTals != null)
			{

				// if (PdfText12.SHOW_DEBUG_INFO) showTalsList(leaf.SheetNumberTals, tals);

				result = validateSheetNumberTextData(leaf);

				if (!result)
				{
					ValidationFailList.Add(leaf);
					Console.Write("v");
					// Debug.WriteLine($"Sheet number validation failed for| {leaf.SheetNumber} ({leaf.SheetNumberTals?.Text ?? "null"})");
					leaf.PageLinkData = null;
				}
				// else
				// {
				// 	Debug.WriteLine($"Got sheet number {leaf.SheetNumber} should match| ({leaf.SheetNumberTals?.Text ?? "null"})");
				// }
			}
			else
			{
				Debug.WriteLine($"Get sheet number failed| {leaf.SheetNumber}");
			}

			// return true;
			return result;
		}

		private bool validateSheetNumberTextData(PdfTreeLeaf leaf)
		{
			bool result = leaf.SheetNumberTals.IsValid;

			if (result)
			{
				int len = leaf.SheetNumberTals.Text.Length;

				if (len > shtNumMaxLen) shtNumMaxLen = len;
				if (len < shtNumMinLen) shtNumMinLen = len;


				leaf.SheetNumberTals.OnPageNumber = pageNum;

				// int p = pageNumMax;

				leaf.SheetNumberTals.ToPageNumber = cfg.TocPage;

				if (leaf.SheetNumberIsTemp)
				{
					leaf.SheetNumber = leaf.SheetNumberTals.Text;
				}
				else
				{
					
					if (!leaf.SheetNumber.Equals(leaf.SheetNumberTals.Text.Trim()))
					{
						leaf.SheetNumbersMisMatch = true;
						result = false;
					}
				}
			}

			return result;
		}



		private TextAndLineSegmentData getSheetNumberTextData2(PdfPage page, Rectangle findRect, bool showText = false)
		{
			SimpleTextAndLocationXstrategy talStrat = new SimpleTextAndLocationXstrategy(findRect, normalPgOrientationIsVert, showText);

			TextAndLineSegmentData talsd = talStrat.GetShtNumData(page);

			// Debug.WriteLine(talStrat.Sb.ToString());

			return talsd;

		}

		private TextAndLineSegmentData getSheetNumberTextData1(PdfPage page, Rectangle findRect, bool showText = false)
		{
			TextAndLineSegmentData result = null;

			TextRegionEventFilter filter = new TextRegionEventFilter(findRect);

			FilteredEventListener listner = new FilteredEventListener();

			LocationTextExtractor5 strat =
				listner.AttachEventListener(new LocationTextExtractor5(null, false), filter);

			// strat.Flip();

			try
			{

				if (parser != null) parser.Reset();

				parser = new PdfCanvasProcessor(listner);

				parser.ProcessPageContent(page);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);

				return null;
			}

			

			// make sure that last item in the queue is processed;
			strat.FinalizeProcessing();

			result = strat.list.Count>0 ? strat.list[0] : null;

			// Debug.WriteLine(SharedCode.ShDataSupport.ReadPDFText.fmtRect(findRect, 1));
			// Debug.WriteLine($"{result?.Text ?? "got null"}");

			// Debug.WriteLine($"\n\n*** space character counts| preface {strat.PrefaceSpaceCounts} | suffix {strat.SuffixSpaceCounts}\n\n");

			// result = null;

			return result;
		}

		private int t = 0;

		private TextAndLineSegmentData getSheetNumberTextData(PdfPage page, Rectangle findRect, bool showText = false)
		{
			TextAndLineSegmentData result = null;

			// if (t > 1)
			// {
			// 	if (t == 2)
			// 	{
			// 		findRect = new Rectangle(1974f, 2792f, 140f, 190f);
			// 	} 
			// 	else if (t == 3)
			// 	{
			// 		findRect = new Rectangle(1974f, 2792f, 140f, 190f);
			// 	}
			// 	else
			// 	{
			// 		findRect = new Rectangle(1974f, 2792f, 140f, 190f);
			// 	}
			// }
			//
			// Debug.WriteLine($"page rotation info| rot {page.GetRotation()}");
			// Debug.WriteLine($"page size - no rotation {page.GetPageSize().GetWidth()} x {page.GetPageSize().GetHeight()}");
			// Debug.WriteLine($"page size - w/ rotation {page.GetPageSizeWithRotation().GetWidth()} x {page.GetPageSizeWithRotation().GetHeight()}");
			// Debug.WriteLine($"find rect| T is {t++} | rectidx {rectangleIdx}  |  ");
			// Debug.WriteLine(SharedCode.ShDataSupport.ReadPDFText.fmtRect(findRect,1));

			TextRegionEventFilter filter = new TextRegionEventFilter(findRect);
			FilteredEventListener listner = new FilteredEventListener();

			LocationTextExtraction3 strat =
				listner.AttachEventListener(new LocationTextExtraction3(null), filter);

			if (parser != null) parser.Reset();

			parser = new PdfCanvasProcessor(listner);

			parser.ProcessPageContent(page);

			strat.Finalize();

			result = strat.list.Count>0 ? strat.list[0] : null;

			Debug.WriteLine($"\n{result?.Text ?? "got null 3"}");

			return result;
		}


		// private TextAndLineSegmentData getSheetNumberTextData(PdfPage page, Rectangle findRect, out List<TextAndLineSegmentData> tals)
		// {
		// 	// findRect = adjustShtNumLoc(page, findRect);
		// 	SheetNumLocationFilter filter =
		// 		new SheetNumLocationFilter(findRect /*, page.GetRotation()*/ , normalPgOrientationIsVert);
		//
		// 	tals = null;
		//
		// 	try
		// 	{
		// 		FilteredEventListener shtNumListener = new FilteredEventListener();
		//
		// 		LocationTextExtractionStrategy xStrat =
		// 			shtNumListener.AttachEventListener(new LocationTextExtractionStrategy(), filter);
		//
		// 		new PdfCanvasProcessor(shtNumListener).ProcessPageContent(page);
		//
		// 		tals = filter.tals;
		//
		// 	}
		// 	catch (Exception e)
		// 	{
		// 		Debug.Write("e");
		// 		// Debug.WriteLine(e);
		// 		return null;
		// 	}
		//
		// 	return filter.talData;
		// }

		/*
		private void GetTextExtrator2(PdfPage page)
		{
			TextAndLocationXstrategy strat = new TextAndLocationXstrategy(null, 1, 10, true);

			List<TextAndLineSegmentData> talsd = strat.GetList(page, 1);

			string t = strat.GetResultantText();
		}

		private void GetTextExtractor(PdfPage page)
		{
			TextLocationStrat strat = new TextLocationStrat();
			
			string text = PdfTextExtractor.GetTextFromPage(page, strat);


			Debug.WriteLine("\n*** text got ***");
			Debug.WriteLine(text);
		}

		private PdfCanvasProcessor parser;
		
		private void getSheetNumberInfo(PdfPage page, Rectangle findRect)
		{
			CustomRectangleFilter cf = new CustomRectangleFilter(findRect);

			FilteredEventListener listner = new FilteredEventListener();

			LocationTextExtractionStrategy strat = listner.AttachEventListener(new LocationTextExtractionStrategy(), cf);

			if (parser != null) parser.Reset();

			parser = new PdfCanvasProcessor(listner);

			parser.ProcessPageContent(page);

			string text = strat.GetResultantText();

			Debug.WriteLine($"\n*** found *** | >>{text}<<");

		}

		private void getPageText(PdfPage page)
		{
			string text = PdfTextExtractor.GetTextFromPage(page);

			Debug.WriteLine(text);
		}

		private void getSheetNumberInfo2(PdfPage page, Rectangle findRect)
		{
			CustomFilter cf = new CustomFilter(findRect);

			FilteredEventListener listner = new FilteredEventListener();

			LocationTextExtractionStrategy strat = listner.AttachEventListener(new LocationTextExtractionStrategy(), cf);

			if (parser != null) parser.Reset();

			parser = new PdfCanvasProcessor(listner);

			parser.ProcessPageContent(page);
			

			string text = strat.GetResultantText();

			Debug.WriteLine($"\n*** found *** | >>{text}<<");

		}
		*/

		// private Rectangle adjustShtNumLoc(PdfPage page, Rectangle findRect)
		// {
		// 	Rectangle ps = page.GetPageSize();
		// 	float pageWidth = ps.GetWidth();
		// 	float pageHeight = ps.GetHeight();
		//
		// 	if (pageWidth < pageHeight) return findRect;
		//
		// 	float x = pageWidth - findRect.GetY() - findRect.GetHeight();
		// 	float y = findRect.GetX();
		// 	float w = findRect.GetHeight();
		// 	float h = findRect.GetWidth();
		//
		// 	return new Rectangle(x, y, w, h);
		// }

		// private int pnx = 1;

/*
		private bool adjustRotation(PdfPage page)
		{
			bool result = true;
			// int pageRotation;
			// pageRotation = page.GetRotation();

			Rectangle r = page.GetPageSizeWithRotation();
			// Rectangle r = page.GetPageSize();


			// Debug.Write($"\t\trotate?| {a.SheetNumber,-8}| {r.GetWidth():F2} x {r.GetHeight():F2} | {pageRotation}° | ");
			//
			// // if (!fixRotation && r.GetWidth() > r.GetHeight() )
			// if (!fixRotation)
			// {
			// 	Debug.WriteLine("NO");
			// 	return result;
			// }
			// Debug.Write("MAYBE | ");


			if (cfg.CorrectSheetRotation && r.GetWidth() < r.GetHeight()) // &&  pageRotation != 270)
			{
				// Debug.WriteLine("YES");
				page.SetRotation(270);
			}
			// else
			// {
			// 	Debug.WriteLine("NO");
			// }

			// Debug.WriteLine("at adj rotate");
			// showPageOrientation(page.GetRotation(),
			// 	page.GetPageSizeWithRotation(), page.GetPageSize());

			return result;
		}
*/

		// private List<TextAndLineSegmentData> validateSheetRefs(List<TextAndLineSegmentData> tals)
		// {
		// 	if (tals==null || tals.Count == 0) return null;
		//
		// 	List<TextAndLineSegmentData> talx = new List<TextAndLineSegmentData>();
		//
		// 	foreach (TextAndLineSegmentData t in tals)
		// 	{
		// 		if (!pages.ContainsKey(t.Text)) continue;
		//
		// 		bool gotCorners = t.Corners != null && t.Corners.Count > 0;
		//
		// 		Debug.WriteLine($"key found| {t.Text}| got corners?| {gotCorners}");
		// 	}
		//
		// 	return talx;
		// }

	#endregion


		// private void checkIfRotate(PdfPage page)
		// {
		//
		// 	bool? result = needToRotatePage(page);
		//
		// 	if (result == null)
		// 	{
		// 		doPageRotate = false;
		// 		rectangleIdx = 2;
		// 	}
		// 	else
		// 	if (result == true)
		// 	{
		// 		doPageRotate = true;
		// 		rectangleIdx = 1;
		// 	}
		// 	else
		// 	{				
		// 		doPageRotate = false;
		// 		rectangleIdx = 0;
		// 	}
		//
		// }

		// private bool? needToRotatePage(PdfPage page)
		// {
		// 	Rectangle ps;
		// 	// Rectangle psNoRot;
		// 	float rotation;
		//
		// 	rotation = page.GetRotation();
		// 	// psNoRot = page.GetPageSize();
		// 	ps = page.GetPageSizeWithRotation();
		//
		// 	// Debug.WriteLine("at need to rotate");
		// 	// showPageOrientation(rotation, ps, psNoRot);
		//
		// 	if (rotation == 90) return null;
		//
		// 	if ( (rotation == 0 && ps.GetHeight() > ps.GetWidth()) ||
		// 		(rotation == 270 && ps.GetWidth() > ps.GetHeight())
		// 		) return false;
		//
		//
		// 	// if (rotation == 270 && ps.GetWidth() > ps.GetHeight()) return false;
		//
		// 	return true;
		// }

		// private void showPageOrientation(float rotation, Rectangle yesRot, Rectangle noRot )
		// {
		// 	Debug.WriteLine($"rotation| {rotation}");
		// 	Debug.WriteLine($"page size, yes rotation| W {yesRot.GetWidth()} | H {yesRot.GetHeight()}");
		// 	Debug.WriteLine($"page size, not rotation| W {noRot.GetWidth()} | H {noRot.GetHeight()}");
		// }




		private void showPage(PdfPage page, PdfTreeLeaf leaf)
		{
			Rectangle r = page.GetPageSize();
			Rectangle rr = page.GetPageSizeWithRotation();

			string p = $"({leaf.PageNumber})";
			string ignoreRot = page.IsIgnorePageRotationForContent().ToString();
			string rotationInv = page.IsPageRotationInverseMatrixWritten().ToString();

			Debug.WriteLine(
				$"\n\tpage| {p,-5}| {leaf.SheetNumber,10}| rotation| {page.GetRotation()} | ignore rotation| {ignoreRot,-6} | inv matrix| {rotationInv,-6} | page size (wxh)| {r.GetWidth()} x {r.GetHeight()}| ({rr.GetWidth()} x {rr.GetHeight()})");
		}




		public override string ToString()
		{
			return $"this is {nameof(Validate12)}";
		}
	}
}
