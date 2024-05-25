#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommonPdfCodeShCode;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Navigation;
using iText.Layout;
using UtilityLibrary;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Layer;
using iText.Layout.Element;
using iText.Layout.Properties;
using Rectangle = iText.Kernel.Geom.Rectangle;
using SettingsManager;
using SharedCode.ShDataSupport.ExcelSupport;
using SharedCode.ShDataSupport.PdfSupport;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using ReadPDFText.Process;
using iText.StyledXmlParser.Jsoup.Parser;

#endregion

// user name: jeffs
// created:   2/24/2024 2:46:42 PM

namespace SharedCode.ShDataSupport
{
	public class PdfText11
	{
		private const bool SHOW_DEBUG_INFO = false;
		private const bool SHOW_DEBUG_SHEET_DATA = false;
		private const bool SHOW_ORIGIN_MARKER = false;


		private const string PAGES_PG_NUM_FORMAT = "*{0:D5}";

		// misc fields
		private float pi180 = (float) Math.PI;
		private float pi90;
		private float pi270;

		private int rectangleIdx = 0;
		private bool? doPageRotate = false;

		private int pageNum;
		private int pageNumMax;
		private Rectangle pageSize;
		private int pageRotation;

		private int shtNumMaxLen = 0;
		private int shtNumMinLen = 100;

		bool tocPageFound = false;

		private int totalRefs;

		private PdfCanvasProcessor parser;

		public static List<PdfTreeLeaf> ValidationFailList;
		public static List<PdfTreeLeaf> RotationFailList;
		public static int FailCode = 0;

		// pdf elements
		private PdfDocument destPdfDoc ;

		private Document destDoc;
		// private PdfPage page;


		// objects
		
		private Dictionary<SheetBorderType, SheetData> shtData;


		// collections

		// string is the sheet number or the page number as :D5
		// using two entries to allow the leaf to be found by 
		// sheet number of page number
		private Dictionary<string, PdfTreeLeaf> pages;



		// settings
		private bool normalPgOrientationIsVert;
		private string fontFile;

		// styles
		// from 'cfg()'

		// debug / stopwatch
		Stopwatch sw = Stopwatch.StartNew();

		private TimeSpan[] el;
		public static string[] elt;

		public static int START                  ;
		public static int GOTUSERNAME            ;
		public static int MERGEDONE              ;
		public static int VALIDATEDONE           ;
		public static int BOOKMARKSDONE          ;
		public static int FINDXREFSDONE          ;
		public static int CROSSREFDONE           ;
		public static int ADDLINKSDONE           ;
		public static int ADDDOCUMENTELEM        ;
		public static int DESTDOCCLOSEDONE       ;
		public static int DESTPDFDOCCLOSEDONE    ;
		public static int END                    ;


		public PdfText11()
		{
			config();
		}


		public static MergePdfConfig cfg { get; set; }
		


		private void config()
		{
			// field init
			this.pi90 = pi180 / 2;
			this.pi270 = pi90 * 3;

			setElText();


			// objects

			// shtData = SheetConfig.SheetConfigData;
			shtData = ReadPDFText.ShtData;

			// topOutlineColor = UserSettings.Data.outlineColor.GetDeviceRgb;
		}

		public void UpdateConfig(PdfAssemblerSettingSupport pass)
		{
			UpdateConfig();

			pass.updateConfig(cfg);
		}

		public void UpdateConfig()
		{
			cfg = new MergePdfConfig();
			normalPgOrientationIsVert = cfg.NormalPgOrientationIsVert;
		}


		public bool AddLinks(PdfNodeTree tree, string dest)
		{
			// if (SHOW_DEBUG_SHEET_DATA)
			// {
			// 	SheetConfig.ShowSheetData();
			// 	// return true;
			// }

			Debug.WriteLine("**** begin ****\n");

			int i = 0;

			el = new TimeSpan[20];
			sw = new Stopwatch();
			sw.Start();

			WriterProperties wp = new WriterProperties();
			wp.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
			wp.UseSmartMode();
			wp.SetFullCompressionMode(true);

			PdfWriter w = new PdfWriter(dest, wp);

			destPdfDoc = new PdfDocument(w);
			destDoc = new Document(destPdfDoc);

			// START = i++;
			el[START] = sw.Elapsed;


			string username = cfg.UserName;
			el[GOTUSERNAME] = sw.Elapsed;

			// step 1 - merge the files into a single PDF

			Console.Write("merge start| ");

			if (!merge(tree))
			{
				el[MERGEDONE] = sw.Elapsed;
				FailCode = MERGEDONE;
				closeDocs(i);
				return false;
			}
			el[MERGEDONE] = sw.Elapsed;

			Console.WriteLine(" merge done");

			// step 2

			Console.Write("validate start| ");

			if (!validate())
			{
				el[VALIDATEDONE] = sw.Elapsed;
				FailCode = VALIDATEDONE;
				closeDocs(i);
				return false;
			}
			el[VALIDATEDONE] = sw.Elapsed;

			Console.WriteLine(" validate done");

			// step 3
			
			Console.Write("bookmarks start| ");

			if (!bookmarks(tree))
			{
				el[BOOKMARKSDONE] = sw.Elapsed;
				FailCode = BOOKMARKSDONE;
				closeDocs(i);
				return false;
			}
			el[BOOKMARKSDONE] = sw.Elapsed;

			Console.WriteLine(" bookmarks done");

			// step 4
			
			Console.Write("sheet xrefs start| ");

			if (!sheetrefs())
			{
				el[FINDXREFSDONE] = sw.Elapsed;
				FailCode = FINDXREFSDONE;
				closeDocs(i);
				return false;
			}
			el[FINDXREFSDONE] = sw.Elapsed;
			
			Console.WriteLine(" sheet xrefs done");

			// step 5
			
			Console.Write("links and anno start| ");

			if (!linksAndAnno())
			{
				el[ADDLINKSDONE] = sw.Elapsed;
				FailCode = ADDLINKSDONE;
				closeDocs(i);
				return false;
			}
			el[ADDLINKSDONE] = sw.Elapsed;

			Console.WriteLine(" links and anno done");

			// step 6
			// ADDDOCUMENTELEM = i++;
			if (!metaData())
			{
				el[ADDDOCUMENTELEM] = sw.Elapsed;
				FailCode = ADDDOCUMENTELEM;
				closeDocs(i);
				return false;
			}
			el[ADDDOCUMENTELEM] = sw.Elapsed;

			closeDocs(i);

			return true;
		}

		private void closeDocs(int i)
		{
			// close the documents
			// DESTDOCCLOSEDONE = i++;
			destDoc.Close();
			el[DESTDOCCLOSEDONE] = sw.Elapsed;

			// DESTPDFDOCCLOSEDONE = i++;
			destPdfDoc.Close();
			el[DESTPDFDOCCLOSEDONE] = sw.Elapsed;

			// all done
			// END = i++;
			el[END] = sw.Elapsed;
			sw.Stop();

			showElapsed();
			showElapsedTotal();
		}

		// step 1
		// merge the individual files
		// create the "sheet list"
		private bool merge(PdfNodeTree tree)
		{
			bool result = false;

			pageNum = 1;

			pages = new Dictionary<string, PdfTreeLeaf>();

			result = merge(tree.Root);

			if (cfg.TocInclude && !tocPageFound)
			{
				string a = $"*** warning| the configured TOC page| {cfg.TocSheet} was not found.";
				string b = pages[string.Format(PAGES_PG_NUM_FORMAT, cfg.TocPage)].SheetNumber;
				string c = $"Using the default of {b} instead";

				Console.WriteLine(
					$"{a}\n{c}");
			}

			// return false;

			return result;
		}

		// step 2
		// validate the sheet list versus the 
		// data read from the PDF and fill-in the
		// missing info for pre-combined pdf's
		// replaces "locate sht numbers"
		private bool validate()
		{
			bool result = true;

			if (SHOW_DEBUG_INFO) Debug.WriteLine($"\n*** validate ***");

			RotationFailList = new List<PdfTreeLeaf>();
			ValidationFailList = new List<PdfTreeLeaf>();

			Debug.WriteLine($"fix rotations| {cfg.CorrectSheetRotation}");

			foreach (KeyValuePair<string, PdfTreeLeaf> kvp in pages)
			{
				if (kvp.Key[0] == '*') continue;

				if (SHOW_DEBUG_INFO) Debug.WriteLine($"validating| {kvp.Value.SheetNumber}");
				result = validate(kvp.Value) && result;
			}

			if (SHOW_DEBUG_INFO) showPages();

			result = (cfg.IgnoreRotationFails || RotationFailList.Count == 0)
				&& (cfg.IgnoreSheetNumberValidationFails || ValidationFailList.Count == 0);

			return result;
		}

		// step 3
		// add bookmarks
		private bool bookmarks(PdfNodeTree tree)
		{
			bool result = true;

			addBookmarks(tree.Root);

			return result;
		}

		// step 4
		// scan pages to locate cross references
		// create the list of hyperlinks that need
		// to be added - cross-ref with sheet numbers
		private bool sheetrefs()
		{
			bool result = true;

			result = locateSheetRefs();

			if (SHOW_DEBUG_INFO) showSheetReferences();

			return result;
		}

		// step 5
		// add the hyperlinks, annotation,
		// and other elements to the pdf
		private bool linksAndAnno()
		{
			bool result = true;

			Rectangle pageSize;
			PdfPage page;

			PdfLayer layer = new PdfLayer("Hyperlinks Rectangles", destPdfDoc);
			layer.SetOn(true);

			makeVertBanner();
			makeHorizBanner();
			makeFooter();
			makeDisclaimer();

			// string returnIdx = $"*{cfg.TocPage:D5}";
			string returnIdx;

			foreach (KeyValuePair<string, PdfTreeLeaf> kvp in pages)
			{
				Console.Write(".");

				if (kvp.Key[0] == '*') continue;

				PdfTreeLeaf leaf = kvp.Value;

				pageNum = leaf.PageNumber;

				addAnnoAndLinks(leaf, layer);
			}


			return result;
		}

		// step 6
		// add document meta-data
		// such as "author" "title" "subject" etc.
		private bool metaData()
		{
			bool result = true;

			result = addDocInfo();

			return result;
		}


		/* worker routines
		 *****************
		 *
		 */


	#region step 1 - merge

		private bool merge(PdfTreeBranch branchNode)
		{
			if (SHOW_DEBUG_INFO) Debug.WriteLine($"merging| {branchNode.Bookmark}");

			bool result = true;

			int numPages;
			

			PdfDocument srcPdfDoc = null;

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in branchNode.ItemList)
			{
				if (SHOW_DEBUG_INFO) Debug.WriteLine($"merging| {kvp.Value.Bookmark}");

				Console.Write(".");

				if (kvp.Value.ItemType == PdfTreeItemType.PT_BRANCH)
				{
					result = merge((PdfTreeBranch) kvp.Value);
				}
				else if (kvp.Value.ItemType == PdfTreeItemType.PT_LEAF)
				{
					PdfTreeLeaf leaf = (PdfTreeLeaf) kvp.Value;

					srcPdfDoc =  new PdfDocument(new PdfReader(leaf.FilePath));

					numPages = srcPdfDoc.GetNumberOfPages();

					srcPdfDoc.CopyPagesTo(1, numPages, destPdfDoc);

					if (cfg.TocInclude && !cfg.TocSheet.IsVoid() && leaf.SheetNumber.Equals(cfg.TocSheet))
					{
						cfg.TocPage = pageNum;
						tocPageFound = true;
					}

					pages.Add(leaf.SheetNumber, leaf);
					// pages.Add($"*{pageNum++:D5}", leaf);
					pages.Add(string.Format(PAGES_PG_NUM_FORMAT, pageNum++), leaf);

					srcPdfDoc.Close();

					// showPage(destPdfDoc.GetLastPage(), leaf);

					
				}
				else if (kvp.Value.ItemType == PdfTreeItemType.PT_NODE_FILE)
				{
					// Debug.WriteLine($"node file");

					mergeNodeFile((PdfTreeNodeFile) kvp.Value);
				}
				else
				{
					throw new InvalidOperationException("Invalid PdfTreeItemType found");
				}
			}

			pageNumMax = pageNum - 1;

			return result;
		}

		private void mergeNodeFile(PdfTreeNodeFile nodeFile)
		{
			int i = 1;
			PdfTreeNode node;
			PdfDocument srcPdfDoc =  new PdfDocument(new PdfReader(nodeFile.FilePath));
			string shtNum;

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in nodeFile.ItemList)
			{
				srcPdfDoc.CopyPagesTo(i, i++, destPdfDoc);

				node = (PdfTreeNode) kvp.Value;

				if (!cfg.TocSheet.IsVoid() && kvp.Value.SheetNumber.Equals(cfg.TocSheet))
				{
					cfg.TocPage = pageNum;
					tocPageFound = true;
				}

				pages.Add(node.SheetNumber, node);
				pages.Add(string.Format(PAGES_PG_NUM_FORMAT, pageNum++), node);

			}

			srcPdfDoc.Close();
		}

	#endregion

	#region step 2 - validate (& get sht nums)

		// run once per leaf / page
		private bool validate(PdfTreeLeaf leaf)
		{
			// Debug.Write($"validating| {leaf.SheetNumber}");

			// this is a "b u g" fix - stops Itext from throwing an exception
			ClipperBridge.floatMultiplier = Math.Pow(10, 12);

			Console.Write(".");

			bool result = true;
			PdfPage page;
			Rectangle shtNumFindRect;

			pageNum = leaf.PageNumber;
			
			page = destPdfDoc.GetPage(pageNum);

			if (SHOW_DEBUG_INFO) showPage(page, leaf);

			// part 1

			result = !cfg.CorrectSheetRotation || adjustRotation(page);

			if (!result)
			{
				RotationFailList.Add(leaf);
				// return false;
				// Debug.WriteLine($"\t\tSheet rotation failed");
				Console.Write("r");
			}


			// part 2

			checkIfRotate(page);


			// this is the current method
			leaf.SheetNumberTals = getSheetNumberTextData(page, leaf.SheetData.SheetNumberFindRect[rectangleIdx], false);



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

				// if (SHOW_DEBUG_INFO) showTalsList(leaf.SheetNumberTals, tals);

				result = validateSheetNumberTextData(leaf);

				if (!result)
				{
					ValidationFailList.Add(leaf);
					Console.Write("v");
					Debug.WriteLine($"Sheet number validation failed for| {leaf.SheetNumber} ({leaf.SheetNumberTals?.Text ?? "null"})");
					leaf.PageLinkData = null;
				}
				// else
				// {
				// 	Debug.WriteLine($"Got sheet number {leaf.SheetNumber} should match| ({leaf.SheetNumberTals?.Text ?? "null"})");
				// }
			}
			else
			{
				Debug.WriteLine($"\nfailed| {leaf.SheetNumber}\n");
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

			Debug.Write($">{talStrat.Sb.ToString()}<  |  ");
			Debug.WriteLine($">{talStrat.SheetNumber}<");

			return talsd;

		}

		private TextAndLineSegmentData getSheetNumberTextData(PdfPage page, Rectangle findRect, bool showText = false)
		{
			TextAndLineSegmentData result = null;

			TextRegionEventFilter filter = new TextRegionEventFilter(findRect);

			FilteredEventListener listner = new FilteredEventListener();

			LocationTextExtractor5 strat =
				listner.AttachEventListener(new LocationTextExtractor5(null, false), filter);


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

			result = strat.list[0];

			Debug.WriteLine(ReadPDFText.fmtRect(findRect, 1));

			Debug.WriteLine($"{result.Text}");

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

			return result;
		}

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

	#region step 3 - bookmarks

		// add bookmarks
		private bool addBookmarks(PdfTreeBranch rootNode)
		{
			if (SHOW_DEBUG_INFO) Debug.WriteLine("\n*** adding bookmarks");

			bool result = true;

			int numPages = destPdfDoc.GetNumberOfPages();

			PdfOutline rootOutline = destPdfDoc.GetOutlines(false);

			try
			{
				createOutlines(rootNode, rootOutline, 1);
			}
			catch (Exception e)
			{
				Console.WriteLine($"*** exception *** | {e.Message}");

				if (e.InnerException != null )
				{
					Console.WriteLine($"\t*** inner exception *** | {e.InnerException.Message}");
				}

				return false;
			}

			return result;
		}

		private void createOutlines(PdfTreeBranch branch, PdfOutline outline, int level)
		{
			PdfOutline ol = null;
			PdfPage page = null;

			if (SHOW_DEBUG_INFO) Debug.WriteLine("\n*** creating outlines 1");

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in branch.ItemList)
			{
				Console.Write(".");

				if (kvp.Value.ItemType == PdfTreeItemType.PT_NODE_FILE)
				{
					ol = outline.AddOutline(kvp.Value.Bookmark);

					page = destPdfDoc.GetPage(kvp.Value.PageNumber);

					ol.AddDestination(PdfExplicitDestination.CreateFit(page));

					createOutlines((PdfTreeNodeFile) kvp.Value, ol, level);

					continue;
				}

				ol = outline.AddOutline(kvp.Value.Bookmark);

				if (level == 1) ol.SetColor(cfg.TopLevelBookmarkColor);
				ol.SetStyle(cfg.TopLevelBookmarkTextStyle);

				ol.SetOpen(false);

				page = destPdfDoc.GetPage(kvp.Value.PageNumber);

				ol.AddDestination(PdfExplicitDestination.CreateFit(page));

				if (kvp.Value.ItemType == PdfTreeItemType.PT_BRANCH)
				{
					createOutlines((PdfTreeBranch) kvp.Value, ol, level + 1);
				}
				else
				{
					page.SetPageLabel(null, $"{getPageLabel((PdfTreeLeaf)kvp.Value)}");
				}
			}
		}

		private void createOutlines(PdfTreeNode node, PdfOutline outline, int level)
		{
			if (SHOW_DEBUG_INFO) Debug.WriteLine("\n*** creating outlines 2");

			PdfPage page;
			PdfOutline ol = null;

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in node.ItemList)
			{
				ol = outline.AddOutline(kvp.Value.Bookmark);

				page = destPdfDoc.GetPage(kvp.Value.PageNumber);

				page.SetPageLabel(null, $"{getPageLabel((PdfTreeLeaf)kvp.Value)}");

				ol.AddDestination(PdfExplicitDestination.CreateFit(page));

				if (((PdfTreeNode) kvp.Value).Level > level)
				{
					createOutlines((PdfTreeNode) kvp.Value, ol, level + 1);
				}
			}
		}

		private static string getPageLabel(APdfTreeNodeEx item)
		{
			return fmtPageLabel(item.SheetNumber, item.SheetName, item.PageNumber);
		}

		private static string fmtPageLabel(string shtNum, string shtName, int number)
		{
			return $"{number} | {shtNum} - {shtName}";
		}

	#endregion

	#region step 4 - find sheet references

		private bool locateSheetRefs()
		{
			if (!cfg.LinksInclude) return true;

			bool result = true;
			PdfPage page;


			TextExtractorFilter2 filter = new TextExtractorFilter2(pages, shtNumMinLen, shtNumMaxLen);

			LocationTextExtractor5 strat5 = 
				new LocationTextExtractor5(filter, normalPgOrientationIsVert);


			// TextAndLocationXstrategy talStrat =
			// 	new TextAndLocationXstrategy(pages, shtNumMinLen, shtNumMaxLen, normalPgOrientationIsVert);

			// process through each page number
			for (int i = 1; i <= destPdfDoc.GetNumberOfPages(); i++)
			{
				Console.Write(".");
				// ignore this page?
				// PdfTreeLeaf leaf = pages[$"*{i:D5}"];
				PdfTreeLeaf leaf = pages[string.Format(PAGES_PG_NUM_FORMAT, i)];

				// Debug.WriteLine($"getting xrefs from| {leaf.SheetNumber} / {leaf.SheetName}");

				if (!leaf.AddXrLinks || !leaf.AnnotateSheet)
				{
					// Debug.WriteLine("\tthis page skipped");
					continue;
				}

				page = destPdfDoc.GetPage(i);

				checkIfRotate(page);

				// talStrat.sheetNumberArea = leaf.SheetData.SheetNumberFindRect;
				// talStrat.sheetNumberArea = leaf.SheetData.SheetNumberFindRect[rectangleIdx];

				strat5.OnPage = i;
				strat5.SheetNumberTals = leaf.SheetNumberTals;

				// if (i == 3)
				// {
				// 	strat5.Flip();
				// }

				string a = PdfTextExtractor.GetTextFromPage(page, strat5);

				// save the list of sheet references found
				leaf.PageLinkData = strat5.list;

				strat5.Reset();
				// leaf.PageLinkData = removeDuplicateRefs(strat5.list);
				// leaf.PageLinkData = removeDuplicateRefs(talStrat.GetList(page, i));

				totalRefs += leaf.PageLinkData.Count;

				crossRefShtNums(leaf);
			}

			return result;
		}


		private List<TextAndLineSegmentData> removeDuplicateRefs(List<TextAndLineSegmentData> tals)
		{
			if (tals == null || tals.Count == 0) return tals;

			Dictionary<string, TextAndLineSegmentData> found = new Dictionary<string, TextAndLineSegmentData>();
			List <TextAndLineSegmentData> talx = new List<TextAndLineSegmentData>();

			foreach (TextAndLineSegmentData t in tals)
			{
				string text = t.Text;
				double x = t.CornerPoints[0].x;
				string blX = $"{(t.CornerPoints[0].x):F3}";
				string blY = $"{(t.CornerPoints[0].y):F3}";
				// string trX = $"{t.Corners[2].x:F:3}";
				// string trY = $"{t.Corners[2].y:F:3}";

				text = $"{t.Text}|{blX}x{blY}";

				if (found.ContainsKey(text)) continue;

				found.Add(text, t);

				talx.Add(t);
			}

			return talx;
		}

		private void crossRefShtNums(PdfTreeLeaf leaf)
		{
			if (leaf.PageLinkData == null || leaf.PageLinkData.Count == 0) return;

			foreach (TextAndLineSegmentData tals in leaf.PageLinkData)
			{
				int pg = 1;
				// if (pages.ContainsKey(tals.Text))
				// {
				pg = pages[tals.Text].PageNumber;
				// }

				// int p = pageNumMax;

				tals.ToPageNumber = pg;
			}
		}

	#endregion

	#region step 5 - add annot and links

		// run once per leaf
		private bool addAnnoAndLinks(PdfTreeLeaf leaf, PdfLayer layer)
		{
			
			bool results = true;

			string returnIdx = string.Format(PAGES_PG_NUM_FORMAT, pageNum);

			PdfPage page = destPdfDoc.GetPage(pageNum);

			pageSize = page.GetPageSize();
			pageRotation = page.GetRotation();

			// page.SetIgnorePageRotationForContent(true);

			PdfCanvas pdfCanvas = new PdfCanvas(page);

			checkIfRotate(page);

			// debug only
			// if (cfg.TestMakeRects) placeAnnoRects(leaf, pdfCanvas);

			// sheet banner / footer, etc.
			addPageAnnotations(leaf, pdfCanvas, pageSize);
			
			// links for disclaimer and author
			addPageLinks(leaf, page, pdfCanvas, layer);
			
			// add sheet xrefs &  return link
			addSheetRefLinks(leaf, page, pdfCanvas, layer);


			pdfCanvas.Release();

			page.Flush();

			return results;
		}


		// adds annotations but no linke / link rectangles
		private void addPageAnnotations(PdfTreeLeaf leaf, PdfCanvas canvas, Rectangle pageSize)
		{
			if (!leaf.AnnotateSheet) return;

			if (leaf.AddBanner)
			{
				placeVertBanner(leaf, canvas);
				placeHorizBanner(leaf, canvas);
			}

			if (leaf.AddDisclaimer) placeDisclaimer(leaf);

			placeFooter(leaf);
		}
		
		// adds actions / links but no link rectangles
		private void addPageLinks(PdfTreeLeaf leaf, PdfPage page, PdfCanvas canvas, PdfLayer layer)
		{
			if (!leaf.AnnotateSheet) return;

			// configure canvas
			canvas.SetFillColor(cfg.LinkFillColor);
			canvas.SetStrokeColor(cfg.LinkStrokeColor);
			canvas.SetLineDash(cfg.LinkDashPattern.GetDash(), cfg.LinkDashPattern.GetGap());
			canvas.SetLineWidth(cfg.LinkLineWidth);
			canvas.SetExtGState(new PdfExtGState().SetFillOpacity(cfg.LinkFillOpacity));

			if (leaf.AddDisclaimer) addDisclaimerLink(leaf, page, canvas, layer);
			if (leaf.AddAuthorLink )addAuthorLink(leaf, page, canvas, layer);

			placeReturnLink(leaf, page, canvas, layer);
		}

		// adds all sheet xref links
		// this does not need to "rotate if needed" because the actual page coordinates are
		// read from the page when the sheet xref is found
		private void addSheetRefLinks(PdfTreeLeaf leaf, PdfPage page, PdfCanvas canvas, PdfLayer layer)
		{
			if (!leaf.AnnotateSheet) return;

			if (leaf.PageLinkData == null || leaf.PageLinkData.Count == 0) return;

			canvas.BeginLayer(layer);

			placeShtRefLink(leaf, page, canvas);

			canvas.EndLayer();

		}

		private void makeVertBanner()
		{
			if (!cfg.BannerVerticalInclude) return;

			cfg.BannerVerticalPg = new Paragraph(cfg.BannerVertical).AddStyle(cfg.BannerVerticalPgStyle);
		}

		private void placeVertBanner(PdfTreeLeaf leaf, PdfCanvas canvas /*, Rectangle pageSize */)
		{
			if (!cfg.BannerVerticalInclude) return;

			TextAlignment ta = TextAlignment.LEFT;

			Rectangle r = 
				leaf.SheetData.BannerRects[(int) BannerOrientation.BO_VERTICAL, (int) cfg.BannerVerticalPosition][0];

			float x = r.GetX();
			float y = r.GetY();

			float w = r.GetWidth();
			float h = r.GetHeight();

			float rotation = 0;

			if (cfg.BannerVerticalPosition == BannerPosition.BP_TOP)
			{
				x += w;
				ta = TextAlignment.RIGHT;
			}

			if (SHOW_ORIGIN_MARKER) placeCircle(canvas, x, y, 3);
			
			y += h / 2;

			if (doPageRotate == true)
			{
				rotation = pi90;
				rotateCoordinate(x, y, 0, 0, out x, out y);
			}

			// place mask over old text
			placeRectangle(canvas, r, DeviceRgb.WHITE, DeviceRgb.WHITE, 1.0f);

			// place the new text
			destDoc.ShowTextAligned(
				cfg.BannerVerticalPg,
				x,
				y,
				pageNum,
				ta,
				VerticalAlignment.MIDDLE,
				rotation);
		}

		private void makeHorizBanner()
		{
			if (!cfg.BannerHorizontalInclude) return;

			cfg.BannerHorizontalPg = new Paragraph(cfg.BannerHorizontal).AddStyle(cfg.BannerHorizontalPgStyle);
		}

		private void placeHorizBanner(PdfTreeLeaf leaf, PdfCanvas canvas)
		{
			if (!cfg.BannerHorizontalInclude) return;

			TextAlignment ta = TextAlignment.RIGHT;

			Rectangle r = 
				leaf.SheetData.BannerRects[(int) BannerOrientation.BO_HORIZONTAL, (int) cfg.BannerHorizontalPosition][0];

			float x = r.GetX();// + 4;
			float y = r.GetY();// + 4;

			float w = r.GetWidth();
			float h = r.GetHeight();

			float rotation = pi270;

			if (SHOW_ORIGIN_MARKER) placeCircle(canvas, x, y, 3);

			x += w / 2;

			if (doPageRotate == true)
			{
				rotation = 0;
				rotateCoordinate(x, y, 0, 0, out x, out y);
			}

			// place maxk over old text
			placeRectangle(canvas, r, DeviceRgb.WHITE, DeviceRgb.WHITE, 1.0f);

			// place te3xt
			destDoc.ShowTextAligned(
				cfg.BannerHorizontalPg,
				x,
				y,
				pageNum,
				ta,
				VerticalAlignment.MIDDLE,
				rotation);
		}

		private void makeFooter()
		{
			string date = $"{DateTime.Now:D}";
			string time = $"{DateTime.Now:t}";

			Text text = new Text(string.Format(cfg.FooterFormat, date, time, cfg.UserName, cfg.FooterUser));

			cfg.FooterPg = new Paragraph(text).AddStyle(cfg.FooterPgStyle);
			// .SetMargin(0f)
			// .SetMultipliedLeading(0)
			// .SetFontColor(DeviceRgb.BLACK)
			// .SetFontSize(3f);
		}

		private void placeFooter(PdfTreeLeaf leaf)
		{
			float rotation;

			float x = leaf.SheetData.FooterRect[0].GetX() + 2f;
			float y = leaf.SheetData.FooterRect[0].GetY();


			if (doPageRotate == true)
			{
				rotation = 0;
				rotateCoordinate(x, y, leaf.SheetData.FooterRect[0].GetHeight(), 0, out x, out y);

			}
			else
			{
				rotation  = pi270;
				y+= leaf.SheetData.FooterRect[0].GetHeight();
			}

			// Debug.WriteLine($"footer rotate?| sheet| {leaf.SheetNumber,-10} | do rotation?| {doPageRotate,-6} | page rotation| {pageRotation,-5} | X| {x,8:F2} | Y| {y,8:F2} | orig X| {origX,8:F2} | orig Y| {origY,8:F2} | orig w| {origW,8:F2} | orig h| {origH,8:F2}  | pg w| {pageSize.GetWidth(),8:F2} | pg h| {pageSize.GetHeight(),8:F2}");

			destDoc.ShowTextAligned(
				cfg.FooterPg,
				x, y, pageNum,
				TextAlignment.LEFT,
				VerticalAlignment.MIDDLE,
				rotation);
		}


		private void makeDisclaimer()
		{
			if (!cfg.DisclaimerInclude) return;

			cfg.DisclaimerPg = new Paragraph(cfg.Disclaimer).AddStyle(cfg.DisclaimerPgStyle);
		}


		private void placeDisclaimer(PdfTreeLeaf leaf)
		{
			if (!cfg.DisclaimerInclude) return;

			float rotation = 0;

			float x = leaf.SheetData.DisclaimerRect[0].GetX() ;
			float y = leaf.SheetData.DisclaimerRect[0].GetY() + 4f;


			if (doPageRotate == true)
			{
				rotation = pi90;
				rotateCoordinate(x, y, 0, leaf.SheetData.DisclaimerRect[0].GetWidth(), out x, out y);
			}
			else
			{
				x +=  leaf.SheetData.DisclaimerRect[0].GetWidth();
			}

			// Debug.WriteLine($"disclaimer rotate?| sheet| {leaf.SheetNumber,-10} | do rotation?| {doPageRotate,-6} | page rotation| {pageRotation,-5} | X| {x,8:F2} | Y| {y,8:F2} | orig X| {origX,8:F2} | orig Y| {origY,8:F2} | orig w| {origW,8:F2} | orig h| {origH,8:F2}  | pg w| {pageSize.GetWidth(),8:F2} | pg h| {pageSize.GetHeight(),8:F2}");

			destDoc.ShowTextAligned(
				cfg.DisclaimerPg,
				x, y, pageNum,
				TextAlignment.RIGHT,
				VerticalAlignment.MIDDLE,
				rotation);
		}

		private void addDisclaimerLink(PdfTreeLeaf leaf, PdfPage page, PdfCanvas canvas, PdfLayer layer)
		{
// ????
			PdfAnnotation anno = makeUrlLinkAnnotation(
				leaf.SheetData.DisclaimerRect[rectangleIdx],
				cfg.DisclaimerUrl);

			placeUrlLinkOnLayer(page, canvas, layer, leaf.SheetData.DisclaimerRect[0], anno);
		}


		private void addAuthorLink(PdfTreeLeaf leaf, PdfPage page, PdfCanvas canvas, PdfLayer layer)
		{
			if (!cfg.AuthorLinkInclude) return;


			PdfAnnotation anno = makeUrlLinkAnnotation(
				leaf.SheetData.PrimaryAuthorRect[rectangleIdx],
				cfg.AuthorLinkUrl);

			placeUrlLinkOnLayer(page, canvas, layer, leaf.SheetData.PrimaryAuthorRect[0], anno);
		}

		private void placeReturnLink(PdfTreeLeaf leaf, PdfPage page, PdfCanvas canvas, PdfLayer layer)
		{
			if (!cfg.LinksInclude) return;

			if (leaf.PageNumber == cfg.TocPage) return;

			PdfAnnotation pa = makeLinkAnnotation(
				leaf.SheetData.SheetNumberLinkRect[rectangleIdx],
				leaf.SheetNumberTals);

			if (pa == null) return;

			page.AddAnnotation(pa);

			canvas.BeginLayer(layer);
// ***
			canvas.Rectangle(
				leaf.SheetData.SheetNumberLinkRect[rectangleIdx]);
			canvas.FillStroke();
			canvas.EndLayer();
		}

		private void placeShtRefLink(PdfTreeLeaf leaf, PdfPage page, PdfCanvas canvas)
		{
			if (!cfg.LinksInclude) return;

			Rectangle r;
			Rectangle ro;

			foreach (TextAndLineSegmentData tals in leaf.PageLinkData)
			{
				if (tals.ToPageNumber == pageNum && !cfg.TestMakeRects) continue;

				// don't rotate this rectangle - it is done in the place routine
				r = tals.GetRectangle();
				// r = tals.GetRectangle(cfg);

				page.AddAnnotation(makeLinkAnnotation(r, tals));

				placeLinkRectangle(canvas, tals);

				canvas.FillStroke();
			}
		}


		private PdfAnnotation makeUrlLinkAnnotation(Rectangle r, string url)
		{
			PdfLinkAnnotation pa = new PdfLinkAnnotation(r);

			pa.SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT)
			.SetAction(PdfAction.CreateURI(url))
			.SetFlag(PdfAnnotation.LOCKED);

			return pa;
		}

		private PdfAnnotation makeLinkAnnotation(Rectangle r, TextAndLineSegmentData tals)
		{
			if (tals == null || tals.ToPageNumber == -1 || tals.Text.IsVoid()) return null;

			PdfLinkAnnotation pa;

			if (tals.IsRect)
			{
				pa = new PdfLinkAnnotation(r);
			}
			else
			{
				pa = makePolyLinkAnnotation(tals);
			}

			pa.SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT)
			.SetAction(PdfAction.CreateGoTo(
					PdfExplicitDestination.CreateFit(destPdfDoc.GetPage(tals.ToPageNumber))))
			.SetFlag(PdfAnnotation.LOCKED) ;

			return pa;
		}

		private PdfLinkAnnotation makePolyLinkAnnotation(TextAndLineSegmentData ls)
		{
			Rectangle r = ls.GetOArectangle();

			PdfLinkAnnotation pa = new PdfLinkAnnotation(r);
			pa.Put(PdfName.QuadPoints, ls.GetRectPolyPath() );

			return pa;
		}

		// place the link annotation and add the link rectangle
		private void placeUrlLinkOnLayer(PdfPage page, PdfCanvas canvas, 
			PdfLayer layer, Rectangle r, PdfAnnotation anno)
		{
			page.AddAnnotation(anno);

			canvas.BeginLayer(layer);
// ***
			canvas.Rectangle(
				rotateIfNeeded(r));
			canvas.FillStroke();
			canvas.EndLayer();
		}


		// the below does not need to "rotate if needed" because the actual page coordinates are
		// read from the page when the sheet xref is found
		private void placeLinkRectangle(PdfCanvas pdfCanvas, TextAndLineSegmentData ls)
		{
			if (ls.IsRect)
			{
				// pdfCanvas.Rectangle(ls.GetRectangle(cfg));
				pdfCanvas.Rectangle(ls.GetRectangle());
			}
			else
			{
				float[] a = ls.GetRectPath();

				pdfCanvas.MoveTo(a[0], a[1]);

				for (int i = 2; i < a.Length; i += 2)
				{
					pdfCanvas.LineTo(a[i], a[i + 1]);
				}

				pdfCanvas.ClosePath();
			}
		}

		private void placeRectangle(PdfCanvas canvas, Rectangle r, Color strokeColor, Color fillColor, float fillOpacity = 1)
		{
			// showInfo("rectangle|", $"{ReadPDFText.fmtRect(r)}");

			Rectangle rr = rotateIfNeeded(r);

			canvas.SaveState();

			PdfExtGState gs = new PdfExtGState().SetFillOpacity(fillOpacity);

			canvas.Rectangle(rr)
			.SetExtGState(gs)
			.SetLineWidth(0)
			.SetStrokeColor(strokeColor)
			.SetFillColor(fillColor)
			.FillStroke();

			canvas.RestoreState();

			if (SHOW_ORIGIN_MARKER) placeCircle(canvas, rr.GetX(), rr.GetY(), 3);
		}

		private void placeCircle(PdfCanvas canvas, double x, double y, double r)
		{
			PdfExtGState gs = new PdfExtGState().SetFillOpacity(0.3f);

			canvas.SaveState();

			canvas.SetLineWidth(0.25f);
			canvas.SetStrokeColor(DeviceRgb.BLACK);
			canvas.SetFillColor(DeviceRgb.GREEN);
			canvas.SetExtGState(gs);

			canvas.MoveTo(x - 5, y);
			canvas.LineTo(x + 5, y);

			canvas.MoveTo(x, y - 5);
			canvas.LineTo(x, y + 5);

			canvas.Circle(x, y, r);

			canvas.FillStroke();

			canvas.RestoreState();


		}



		private Rectangle rotateIfNeeded(Rectangle r)
		{
			if (doPageRotate == false)
			{
				// Debug.WriteLine("no rotate");
				return r;
			}

			// Debug.WriteLine("rotate");

			float x = (-1 * r.GetY()) + pageSize.GetWidth() - r.GetHeight();

			Rectangle rx = new Rectangle(x, r.GetX(), r.GetHeight(), r.GetWidth());

			return rx;
		}

		private Rectangle rotateIfNeeded(float x, float y, float w, float h)
		{
			if (doPageRotate == false) return new Rectangle(x, y, w, h);

			(x,  y) = (-1*y, x);

			x += pageSize.GetWidth() - w;

			Rectangle rx = new Rectangle(x, y, h, w);

			return rx;
		}

		private void rotateCoordinmentIfNeeded(float x, float y, float w, float h, out float x1, out float y1)
		{
			if (doPageRotate == false)
			{
				x1 = x;
				y1 = y;
				return;
			}
		
			rotateCoordinate(x, y, w, h, out x1, out y1);

		}

		private void rotateCoordinate(float x, float y, float w, float h, out float x1, out float y1)
		{
			(x1,  y1) = (y, x);

			x1 = pageSize.GetWidth() -x1 - w;
			y1 += h;
		}

		// debug only
		private void placeAnnoRects(  PdfTreeLeaf leaf, PdfCanvas canvas, PdfPage page = null)
		{

			/*  detailed debug info  
			Debug.WriteLine($"\nrectangles for| {leaf.Bookmark}");
			Debug.WriteLine($"rotation| {page.GetRotation()}");
			Debug.WriteLine($"sheet size (not rotate) | {ReadPDFText.fmtRect(page.GetPageSize())}");
			Debug.WriteLine($"sheet size (with rotate)| {ReadPDFText.fmtRect(page.GetPageSizeWithRotation())}");
			Debug.WriteLine($"media box size          | {ReadPDFText.fmtRect(page.GetMediaBox())}");
			Debug.WriteLine($"crop box size           | {ReadPDFText.fmtRect(page.GetCropBox())}");

			// if (leaf.SheetNumber.Equals("Cx01"))
			// {
			PdfDictionary pd = page.GetPdfObject();
			PdfResources res = new PdfResources(pd);
			
			ICollection<PdfName> k = pd.KeySet();
			PdfObject c = pd.Get(PdfName.Contents);

			PdfDictionary pd2 = res.GetPdfObject();
			ICollection<PdfName> k2 = pd2.KeySet();

			ICollection<PdfName> a = res.GetResourceNames();

			PdfDictionary pr = pd.GetAsDictionary(PdfName.Resources);

			PdfDictionary px= pr.GetAsDictionary(PdfName.XObject);

			ICollection<PdfName> ks1 = px.KeySet();

			List<PdfStream> pdx = new List<PdfStream>();
			PdfStream pox;
			PdfArray m;
			PdfIndirectReference ir1 = pd.GetIndirectReference();
			PdfIndirectReference ir2 = px.GetIndirectReference();
			PdfIndirectReference ir3;
			PdfIndirectReference ir4;

			PdfName pnx = null;
			PdfArray mx = null;
			PdfIndirectReference irx = null;


			int max3 = 0;

			string prefix;

			foreach (PdfName pn in ks1)
			{
				prefix = pn.ToString().Substring(0, 3);

				if (!pn.ToString().Substring(0,4).Equals("/BBA")) continue;

				pox = px.GetAsStream(pn);
				pdx.Add(pox);

				m = pox.GetAsArray(PdfName.Matrix);

				ir3 = pox.GetIndirectReference();

				// max3 = ir3.GetObjNumber()>max3? ir3.GetObjNumber():max3;

				if (ir3.GetObjNumber() > max3)
				{
					max3 = ir3.GetObjNumber();

					pnx = pn;
					mx = m;
					irx = ir3;
				}

				//
				// max4 = ir4.GetGenNumber()>max4? ir4.GetGenNumber():max4;
				
				// showMatrix(pn, m, ir3);
			}

			

			showMatrix(pnx, mx, irx);

			Debug.WriteLine($"\t *** ir maxs| (pox) {max3}");
			// }
			*/

			SheetData sd = leaf.SheetData;

			// removed
			// placeTestRect(canvas, sd.TitleBlockRect[0]     , cfg. );
			// placeTestRect(canvas, sd.ContentRect[0]        , cfg. );

			placeTestRect(canvas, sd.FooterRect[0]         , cfg.TestRecFillFooter );
			placeTestRect(canvas, sd.DisclaimerRect[0]     , cfg.TestRecFillDisclaimer );
			placeTestRect(canvas, sd.PrimaryAuthorRect[0]  , cfg.TestRecFillAuthor );
			placeTestRect(canvas, sd.BannerRects[0 , 0][0] , cfg.TestRecFillBanner );
			placeTestRect(canvas, sd.BannerRects[0 , 1][0] , cfg.TestRecFillBanner );
			placeTestRect(canvas, sd.BannerRects[1 , 0][0] , cfg.TestRecFillBanner );
			placeTestRect(canvas, sd.BannerRects[1 , 1][0] , cfg.TestRecFillBanner );

			placeTestRect(canvas, sd.SheetNumberLinkRect[0], cfg.TestRecFillReturn );
			placeTestRect(canvas, sd.SheetNumberFindRect[0], cfg.TestRecFillFind );
			
			// Rectangle testRect1 = new Rectangle(100, 20, 100, 20);
			// Rectangle testRect2 = new Rectangle(100, 20, 20, 100);
			//
			// placeTestRect(canvas, testRect1, new DeviceRgb(System.Drawing.Color.Red) );
			// placeTestRect(canvas, testRect2, new DeviceRgb(System.Drawing.Color.Blue) );

		}

		private void placeTestRect(PdfCanvas canvas,  Rectangle rect, DeviceRgb rgb)
		{
			if (rect == null) return;

			placeRectangle(canvas, rect, cfg.TestRectBorder, rgb, cfg.TestRectFillOpacity);
		}


	#endregion

	#region step 6 - add document info - meta data

		private bool addDocInfo()
		{
			bool result = true;

			destPdfDoc.GetCatalog().SetOpenAction(
				PdfExplicitDestination.CreateFit(destPdfDoc.GetFirstPage()));

			// this causes a dialog to be displayed when the document is opened
			// destPdfDoc.GetCatalog().SetOpenAction(
			// 	PdfAction.CreateJavaScript("app.alert('Thanks for using Josh')"));

			PdfDocumentInfo info = destPdfDoc.GetDocumentInfo();

			info.SetTitle($"Project: {cfg.ProjectNumber} - {cfg.ProjectName}");
			info.SetAuthor($"{cfg.SetAuthor} ({cfg.UserName})");
			info.SetCreator(cfg.SetProducer);
			info.SetSubject(cfg.SetDescription);
			info.SetProducer(cfg.SetProducer);
			info.AddCreationDate();

			return result;
		}
		

	#endregion


		public override string ToString()
		{
			return $"this is {nameof(PdfText10)}";
		}

		// common routines

		private void checkIfRotate(PdfPage page)
		{

			if (needToRotatePage(page))
			{
				doPageRotate = true;
				rectangleIdx = 1;
			}
			else
			{				
				doPageRotate = false;
				rectangleIdx = 0;
			}

		}

		private bool needToRotatePage(PdfPage page)
		{
			Rectangle ps = page.GetPageSizeWithRotation();
			float rotation = page.GetRotation();

			if (ps.GetWidth() > ps.GetHeight() && rotation == 270) return false;

			return true;
		}

		// debug

		private void showElapsed()
		{
			Console.WriteLine($"{elt[0],-25} | {el[0].TotalMilliseconds}");

			// double total = 0;

			for (int i = 1; i < END; i++)
			{
				double diff = el[i].TotalMilliseconds - el[i - 1].TotalMilliseconds;

				// total += diff;

				Console.Write($"{elt[i],-25} | diff| {diff,11:F4}");

				// Console.Write($" | {elt[i],-25} ({el[i].TotalMilliseconds,10:F4})  |  {elt[i-1],-25} ({el[i-1].TotalMilliseconds,10:F4})");

				Console.Write("\n");
			}
		}

		private void showElapsedTotal()
		{
			Console.WriteLine($"total millisecs| {el[END].TotalMilliseconds}");
			Console.WriteLine($"total      secs| {el[END].TotalSeconds}");
		}

		private void setElText()
		{
			elt = new string[20];
			int idx = 0;

			// primary steps
			START                  = idx++; // 0
			GOTUSERNAME            = idx++; // 1
			MERGEDONE              = idx++; // 2
			VALIDATEDONE           = idx++; // 3
			BOOKMARKSDONE          = idx++; // 4
			FINDXREFSDONE          = idx++; // 5
			ADDLINKSDONE           = idx++;
			ADDDOCUMENTELEM        = idx++;

			// completion steps
			DESTDOCCLOSEDONE       = idx++;
			DESTPDFDOCCLOSEDONE    = idx++;
			END                    = idx++;

			CROSSREFDONE           = idx++; // 6

			// primary steps
			elt[START                 ] = "Start";
			elt[GOTUSERNAME           ] = "got user name";
			elt[MERGEDONE             ] = "after merge";
			elt[VALIDATEDONE          ] = "after validate";
			elt[BOOKMARKSDONE         ] = "after bookmarks";
			elt[FINDXREFSDONE         ] = "after find xrefs";
			elt[CROSSREFDONE          ] = "after cross-ref sht nums";
			elt[ADDLINKSDONE          ]	= "after add anno and links";
			elt[ADDDOCUMENTELEM       ] = "after add doc elem's.";

			// completion steps
			elt[DESTDOCCLOSEDONE      ] = "after dest doc closed";
			elt[DESTPDFDOCCLOSEDONE   ] = "after dest pdf closed";
		}

		private void showPages()
		{
			Debug.WriteLine($"\n*** pages ***\n");

			foreach (KeyValuePair<string, PdfTreeLeaf> kvp in pages)
			{
				Debug.WriteLine($"{kvp.Value.PageNumber,3}| {kvp.Key,-8}| {kvp.Value.Bookmark}");
			}
		}

		private void showSheetReferences()
		{
			Debug.WriteLine($"\n*** Show Sheet references ***");

			string refs;


			foreach (KeyValuePair<string, PdfTreeLeaf> kvp in pages)
			{
				if (kvp.Key[0] == '*') continue;

				PdfTreeLeaf leaf = kvp.Value;

				if (leaf.AddXrLinks)
				{
					refs = $"number of references| {kvp.Value.PageLinkData?.Count.ToString() ?? "is null"}";
				}
				else
				{
					refs = "page ignored";
				}


				Debug.WriteLine($"{kvp.Value.PageNumber,3} | for page| ${kvp.Key,-10}| {refs}");

				if (kvp.Value.PageLinkData == null) continue;

				foreach (TextAndLineSegmentData tals in kvp.Value.PageLinkData)
				{
					Debug.WriteLine($"\t{tals.Text} on page {tals.OnPageNumber} and to page {tals.ToPageNumber}");
				}
			}
		}

		private void showTalsList(TextAndLineSegmentData tals, List<TextAndLineSegmentData> talx)
		{
			Debug.WriteLine("\n\t*** tals data ***");
			showTals(tals);

			// foreach (TextAndLineSegmentData t in talx)
			// {
			// 	showTals(t);
			// }

			Debug.Write("\n");
		}

		private void showTals(TextAndLineSegmentData tals)
		{
			Debug.WriteLine($"\t{tals.Text,-20}| {ReadPDFText.fmtRect(tals.GetOArectangle())}");
		}

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

		private void showInfo(string title, string info)
		{
			Debug.WriteLine($"{title,-30}| {info}");
		}

		private void showMatrix(Matrix m)
		{
			Debug.WriteLine($"*** matrix | {m.Get(0)} {m.Get(1)} {m.Get(2)} {m.Get(3)} {m.Get(4)} {m.Get(5)} {m.Get(6)}");
		}

		private void showMatrix(PdfName pn, PdfArray m, PdfIndirectReference ir)
		{

			if (m == null)
			{
				Debug.WriteLine($"\t*** matrix | is null");
				return;
			}

			float[] f = m.ToFloatArray();

			Debug.WriteLine($"\t*** matrix ({pn.ToString(),-8}) | ({ir.ToString()})| {f[0]} {f[1]} {f[2]} {f[3]} {f[4]} {f[5]}");
		}

		// replaced / saved

		/* merge using task for processing
		private bool merge()
		{
			bool result = false;

			Task[] tasks = new Task[1];
			tasks[0] = Task.Factory.StartNew(() => result = mrgTest());
			Task.Factory.ContinueWhenAll(tasks, complete =>
			{
				Console.WriteLine("all tasks complete");
			});

			mrgTest();

			return result;
		}

		private bool mrgTest()
		{
			bool result = true;
			for (int i = 0; i < 100; i++)
			{
				Console.Write(".");
			}

			Console.Write("\n");

			return result;
		}
		*/

		/* validation replaced
		private bool validate(PdfTreeLeaf leaf, int a)
		{
			bool result = true;

			PdfPage page;
			// PdfTreeLeaf leaf;

			Rectangle shtNumFindRect;

			// Debug.Write("\n\n");

			foreach (KeyValuePair<string, PdfTreeLeaf> kvp in Pages)
			{
				leaf = (PdfTreeLeaf) kvp.Value;
				pageNum = leaf.PageNumber;
				shtNumFindRect = leaf.SheetData.SheetNumberFindRect;
				page = destPdfDoc.GetPage(pageNum);

				if (!validateRotation(page))
				{
					FailList.Add(leaf);
					continue;
				}

				/*
				SheetNumLocationFilter filter = 
					new SheetNumLocationFilter(shtNumFindRect /* , pageRotation #2#, normalPgOrientationIsVert);

				FilteredEventListener shtNumListener = new FilteredEventListener();

				LocationTextExtractionStrategy xStrat =
					shtNumListener.AttachEventListener(new LocationTextExtractionStrategy(), filter);

				new PdfCanvasProcessor(shtNumListener).ProcessPageContent(page);

				TextAndLineSegmentData tals = filter.talData;
				#1#


				TextAndLineSegmentData tals = getSheetNumberTextData(page, shtNumFindRect);

				result = validateSheetNumberTextData(leaf, tals);

				/*
				if (tals.IsValid)
				{
					int len = tals.Text.Length;

					if (len > shtNumMaxLen) shtNumMaxLen = len;
					if (len < shtNumMinLen) shtNumMinLen = len;

					tals.OnPageNumber = pageNum;
					tals.ToPageNumber = cfg.TocPage;

					leaf.SheetNumberTals = tals;

					if (leaf.SheetNumberIsTemp)
					{
						leaf.SheetNumber = tals.Text;
					}
					else
					{
						Debug.WriteLine($"comparing| pg num|{pageNum}| >{leaf.SheetNumber}< to >{tals.Text.Trim()}<");
						if (!leaf.SheetNumber.Equals(tals.Text.Trim()))
						{
							leaf.SheetNumbersMisMatch = true;
							FailList.Add(leaf);
						}
					}
				}
				#1#


			}

			return result;
		}
		*/

		/* validation replaced
		private bool adjustOrientation(PdfPage page)
		{
			Rectangle s = page.GetPageSize();

			if (s.GetWidth() < s.GetHeight()) return true;

			Console.WriteLine($"adjusting orientation");

			// page.SetMediaBox(new Rectangle(s.GetX(), s.GetY(), s.GetHeight(), s.GetWidth()));


			PdfDictionary pageDict = page.GetPdfObject();
			PdfArray mediaBox = pageDict.GetAsArray(PdfName.MediaBox);
			float llx = mediaBox.GetAsNumber(0).FloatValue();
			float lly = mediaBox.GetAsNumber(1).FloatValue();
			float urx = mediaBox.GetAsNumber(2).FloatValue();
			float ury = mediaBox.GetAsNumber(3).FloatValue();

			mediaBox.Set(2, new PdfNumber(ury));
			mediaBox.Set(3, new PdfNumber(urx));

			return true;
		}
		*/

	}
}