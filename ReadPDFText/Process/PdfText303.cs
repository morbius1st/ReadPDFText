#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using static System.Drawing.Color;
using CommonPdfCodeShCode;
using iText.Kernel.Colors;
using iText.Kernel.Crypto.Securityhandler;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Navigation;
using iText.Layout;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Layer;
using iText.Layout.Element;
using iText.Layout.Properties;
using Rectangle = iText.Kernel.Geom.Rectangle;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using SharedCode.ShDataSupport;
using ReadPDFText.PdfSupport;
using UtilityLibrary;
using static SharedCode.ShDataSupport.ReadPDFText;
using static iText.Svg.SvgConstants;
using System.Windows.Media.Imaging;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;

#endregion

// user name: jeffs
// created:   4/27/2024 4:24:38 PM


/*
 * notes:
 *  ... Sheet ... based on created information provided to the system
 *					e.g. sheet information measured from a title block
 *
 *  ... page ... based on information read from a PdfPage
 *					e.g. text extraction coordinates
 */

namespace ReadPDFText.Process
{
	public class PdfText303
	{
		public const string PAGES_PG_NUM_FORMAT = "*{0:D5}";

		private const float LINK_BOX_W = 250;
		private const float LINK_BOX_H = 50;

		private MergePdfConfig cfg;

		private Dictionary<SheetBorderType, SheetData> shtData;
		private SheetData sd;

		private PdfDocument destPdfDoc ;

		private Document destDoc;

		private PdfCanvasProcessor parser;

		private PdfPage pdfPage;
		private PdfLayer layer;
		private  PdfCanvas canvas;

		private Dictionary<string, PdfTreeLeaf> pages;

		private int pageNum;

		private string disclaimer = "please press this link in order to view the disclaimer";
		private string footer = " this is a footer";

		private string bannerTh = "banner - top, horizontal";
		private string bannerTv = "banner - top, vertical";
		private string bannerBh = "banner - bottom, horizontal";
		private string bannerBv = "banner - bottom, vertical";

		private string watermark = "PROGRESS ONLY";
		private float watermarkHeight = 3f * 72f; // 4 inches high

		private	List<float> unRotateList;

		private Rectangle pageSize;
		private Rectangle pageSizeWithRotation;
		private float pageRotation;

		private Rectangle[] rectBoxes;

		private annoInfo[,] annoInfo;
		private annoInfo[,] linkInfo;
		private annoInfo[,] testRectsDoNotRotate;
		private annoInfo[,] testRectsToRotate;

		private annoInfo[,] banners;
		private annoInfo[,] pageAnno;

		private annoInfo origin;
		private annoInfo footerAnnoInfo;
		private annoInfo shtName;

		private annoInfo currAnnoInfo;

		private PdfTreeLeaf currLeaf;

		private DeviceRgb[,] rgbInfo2;


		private bool doRotate = true;

		private int shtNumMaxLen = 0;
		private int shtNumMinLen = 100;

		private float txtDx = 0;
		private float txtDy = 0;

		//                    page 1 gets   v  v  v  v
		//                    page 2 gets      v  v  v  v
		private int[] pageRefs = new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,};
		private int pageRefIdx = 0;


		public PdfText303()
		{
			config();
		}

		private void config()
		{
			// shtData = ShtData;

			cfg = new MergePdfConfig();

			makeBoxes();
		}

		private void closeDocs()
		{
			destDoc.Close();

			destPdfDoc.Close();
		}

		public void Process(PdfNodeTree tree, string dest)
		{
			// unRotateList = new List<float>();

			WriterProperties wp = new WriterProperties();
			wp.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
			wp.UseSmartMode();
			wp.SetFullCompressionMode(true);

			PdfWriter w = new PdfWriter(dest, wp);

			destPdfDoc = new PdfDocument(w);
			destDoc = new Document(destPdfDoc);

			Debug.WriteLine("merge start| ");

			merge(tree);

			getSheetNumbers();

			locateSheetReferences();

			addAnnoAndLinks();

			// reRotatePages();

			closeDocs();
		}

	#region merge

		private void merge(PdfNodeTree tree)
		{
			pageNum = 1;

			pages = new Dictionary<string, PdfTreeLeaf>();

			Console.Write("merging ");

			merge(tree.Root);

			Console.WriteLine(" complete");
		}

		private void merge(PdfTreeBranch branchNode)
		{
			bool result = true;
			int numPages;
			PdfDocument srcPdfDoc = null;

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in branchNode.ItemList)
			{
				// if (SHOW_DEBUG_INFO) Debug.WriteLine($"merging| {kvp.Value.Bookmark}");

				Console.Write(".");

				if (kvp.Value.ItemType == PdfTreeItemType.PT_BRANCH)
				{
					merge((PdfTreeBranch) kvp.Value);
				}
				else if (kvp.Value.ItemType == PdfTreeItemType.PT_LEAF)
				{
					PdfTreeLeaf leaf = (PdfTreeLeaf) kvp.Value;

					srcPdfDoc =  new PdfDocument(new PdfReader(leaf.FilePath));

					numPages = srcPdfDoc.GetNumberOfPages();

					srcPdfDoc.CopyPagesTo(1, numPages, destPdfDoc);

					pages.Add(leaf.SheetNumber, leaf);

					pages.Add(string.Format(PAGES_PG_NUM_FORMAT, pageNum++), leaf);

					srcPdfDoc.Close();
				}
				else if (kvp.Value.ItemType == PdfTreeItemType.PT_NODE_FILE)
				{
					mergeNodeFile((PdfTreeNodeFile) kvp.Value);
				}
				else
				{
					throw new InvalidOperationException("Invalid PdfTreeItemType found");
				}
			}
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

				pages.Add(node.SheetNumber, node);
				pages.Add(string.Format(PAGES_PG_NUM_FORMAT, pageNum++), node);
			}

			srcPdfDoc.Close();
		}

	#endregion

	#region validate

		private void getSheetNumbers()
		{
			// this is a "b u g" fix - stops Itext from throwing an exception
			ClipperBridge.floatMultiplier = Math.Pow(10, 12);

			bool result;

			Stopwatch sw = Stopwatch.StartNew();

			sw.Start();

			Console.Write("validate ");

			foreach (KeyValuePair<string, PdfTreeLeaf> kvp in pages)
			{
				if (kvp.Key[0] == '*') continue;

				// Debug.WriteLine($"\nvalidating| {kvp.Key} - {kvp.Value.SheetName}");

				currLeaf = kvp.Value;

				result = getSheetInfo();

				if (result)
				{
					Console.Write(".");
				}
				else
				{
					Console.Write("v");
				}
			}

			Console.WriteLine(" complete");

			sw.Stop();

			double t = sw.ElapsedMilliseconds;

			Console.WriteLine($"time (one) | milliseconds {t:F1}");

			showShtNumsAndNames();
		}

		private bool getSheetInfo()
		{
			bool result;

			pageNum = currLeaf.PageNumber;

			pdfPage = destPdfDoc.GetPage(pageNum);

			adjustPageRotation();

			// below needed for rotate rectangle
			pageRotation = pdfPage.GetRotation();
			pageSizeWithRotation = pdfPage.GetPageSizeWithRotation();

			// todo replace with sheet data rectangle

			// Rectangle r = rectBoxes[0];
			//
			// currLeaf.SheetNumberTals = getSheetNumberTals(r);
			//
			// if (!validateSheetNumberTextData())
			// {
			// 	Debug.WriteLine($"Get sheet number failed| {currLeaf.SheetNumber}");
			// 	return false;
			// }
			//
			// return true;

			result = getSheetNumber();

			if (cfg.ExtractAndValidateSheetNames)
			{
				result = result && getSheetName();
			}
			

			return result;
		}

		private bool getSheetNumber()
		{
			// do not rotate - get sheet info does the rotation
			// Rectangle r = rectBoxes[0];
			Rectangle r = rectBoxes[21];

			List<TextAndLineSegmentData> result=getSheetInfoTals(r);

			currLeaf.SheetNumberTals = result == null ? null : result[0];

			if (!validateSheetNumberTextData())
			{
				Debug.WriteLine($"Get sheet number failed| {currLeaf.SheetNumber}");
				Debug.WriteLine($"rotation  | {pageRotation}");
				Debug.WriteLine($"pg size w/| {fmtRect(pageSizeWithRotation)}");
				return false;
			}

			return true;
		}

		private bool getSheetName()
		{
			// do not rotate - get sheet info does the rotation
			// for sample 5
			// Rectangle r = rectBoxes[22];
			// for coliseum
			Rectangle r = rectBoxes[23];

			List<TextAndLineSegmentData> result=getSheetInfoTals(r);

			if (result == null || result.Count == 0) return false;

			StringBuilder sb = new StringBuilder();
			int i;

			for (i = 0; i < result.Count-1; i++)
			{
				sb.Append(result[i].Text).Append(" ");
			}

			sb.Append(result[i].Text);

			currLeaf.SheetNameExtracted = sb.ToString();

			return true;
		}

		private List<TextAndLineSegmentData> getSheetInfoTals(Rectangle r)
		{
			Rectangle rr;

			rr = rotateFindRectangle(r);

			List<TextAndLineSegmentData> result;

			

			TextRegionEventFilter filter = new TextRegionEventFilter(rr);
			FilteredEventListener listener = new FilteredEventListener();

			LocationTextExtraction3 strat3 =
				listener.AttachEventListener(new LocationTextExtraction3(null), filter);

			strat3.ShowTextStream = false;
			strat3.ShowSegmentText = false;

			if (parser != null) parser.Reset();
			
			parser = new PdfCanvasProcessor(listener);
			
			parser.ProcessPageContent(pdfPage);

			// string answer=
			// PdfTextExtractor.GetTextFromPage(pdfPage, strat3);

			strat3.Finalize();

			result = strat3.list.Count > 0 ? strat3.list : null;

			return result;
		}

		private bool validateSheetNumberTextData()
		{
			if (!(currLeaf.SheetNumberTals?.IsValid ?? false)) return false;

			bool result = true;

			int len = currLeaf.SheetNumberTals.Text.Length;

			if (len > shtNumMaxLen) shtNumMaxLen = len;
			if (len < shtNumMinLen) shtNumMinLen = len;


			currLeaf.SheetNumberTals.OnPageNumber = pageNum;

			// int p = pageNumMax;

			currLeaf.SheetNumberTals.ToPageNumber = 1;

			if (currLeaf.SheetNumberIsTemp)
			{
				currLeaf.SheetNumber = currLeaf.SheetNumberTals.Text;
			}
			else
			{
				if (!currLeaf.SheetNumber.Equals(currLeaf.SheetNumberTals.Text.Trim()))
				{
					currLeaf.SheetNumbersMisMatch = true;
					result = false;
				}
			}

			return result;
		}


		private void showShtNumsAndNames()
		{
			foreach (KeyValuePair<string, PdfTreeLeaf> kvp in pages)
			{
				if (kvp.Key[0]=='*') continue;

				Debug.WriteLine($"for {kvp.Key,-8}| num extracted >{kvp.Value.SheetNumberTals.Text}< | name extracted >{kvp.Value.SheetNameExtracted}<");

			}

		}
	#endregion

	#region get sheet xrefs

		private void locateSheetReferences()
		{
			// todo remove
			shtNumMinLen = 4;
			shtNumMaxLen = 4;

			ListTextExtractionFilter3 filter3 = new ListTextExtractionFilter3(pages, shtNumMinLen, shtNumMaxLen);
			LocationTextExtraction3 strat3 =
				new LocationTextExtraction3(filter3);

			Console.Write("get sheet xrefs ");

			for (int i = 1; i <= destPdfDoc.GetNumberOfPages(); i++)
			{
				Console.Write(".");

				currLeaf = pages[string.Format(PAGES_PG_NUM_FORMAT, i)];

				pdfPage = destPdfDoc.GetPage(i);

				strat3.OnPage = i;
				strat3.SheetNumberTals = currLeaf.SheetNumberTals;
				strat3.ShowTextStream = false;
				strat3.ShowSegmentText = false;

				// Debug.WriteLine($"\nextracting from| {currLeaf.SheetNumber}");

				string a = PdfTextExtractor.GetTextFromPage(pdfPage, strat3);

				strat3.Finalize();

				currLeaf.PageLinkData = strat3.list;

				strat3.Reset();

				// Debug.Write("\n");

				crossRefShtNums();
			}

			Console.WriteLine(" complete");
		}

		private void crossRefShtNums()
		{
			if (currLeaf.PageLinkData == null || currLeaf.PageLinkData.Count == 0) return;

			foreach (TextAndLineSegmentData tals in currLeaf.PageLinkData)
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

	#region anno and links

		/*anno and links .
		 */

		/*
		* definitions
		* sheet coordinates - based on taking a zero rotation, landscape page and
		* measuring coordinates from the lower-left corner
		*
		* page coordinates - based onextracting coordinates from a page which
		* may be rotated
		*
		*/

		private void addAnnoAndLinks()
		{
			layer = new PdfLayer("Hyperlinks Rectangles", destPdfDoc);
			layer.SetOn(true);

			Console.Write("add annotations and links ");

			foreach (KeyValuePair<string, PdfTreeLeaf> kvp in pages)
			{
				if (kvp.Key[0] == '*') continue;

				currLeaf = kvp.Value;

				pageNum = currLeaf.PageNumber;

				Console.Write(".");

				addAnno(currLeaf);
			}

			Console.WriteLine(" complete");
		}

		private void addAnno(PdfTreeLeaf leaf)
		{
			pdfPage = destPdfDoc.GetPage(pageNum);
			pageRotation = pdfPage.GetRotation();
			pageSize = pdfPage.GetPageSize();
			pageSizeWithRotation = pdfPage.GetPageSizeWithRotation();

			canvas = new PdfCanvas(pdfPage);

			sd = leaf.SheetData;

			// place watermark - first and below everything else
			addSheetWatermark(2);

			// add links from page to page
			addPageRefLinks();

			// add sheet links (disclaimer, etc).
			addSheetLinks();

			// add banners
			addBanners();

			// add footer
			addFooter();
		}

		/* basic place element routines
		 */

		private void addPageRefLinks()
		{
			canvas.SaveState();

			configCanvasForPageRefLinks();

			placePageRefLink();

			canvas.RestoreState();
		}

		private void addSheetLinks()
		{
			for (var i = 0; i < pageAnno.GetLength(1); i++)
			{
				currAnnoInfo = pageAnno[0, i];

				if (pageRefIdx == 20) pageRefIdx = 0;

				placeSheetLink(currAnnoInfo.rect, pageRefs[pageRefIdx+1],
					currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill, 0.30f);

				if (currAnnoInfo.text != null)
				{
					Paragraph p = new Paragraph(currAnnoInfo.text)
					.SetFontSize(8);

					placeSheetText(p, 
						currAnnoInfo.rect.GetX(), currAnnoInfo.rect.GetY(), 0, 0, 
						currAnnoInfo.textRotation, TextAlignment.LEFT, VerticalAlignment.TOP);
				}
			}
		}

		private void addBanners()
		{
			for (var i = 0; i < banners.GetLength(1); i++)
			{
				currAnnoInfo = banners[0, i];

				placeSheetRectangle(currAnnoInfo.rect, 
					currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill, 0.30f);

				if (currAnnoInfo.text != null)
				{
					Paragraph p = new Paragraph(currAnnoInfo.text)
					.SetFontSize(24);
				
					placeSheetText(p, 
						currAnnoInfo.rect.GetX(), currAnnoInfo.rect.GetY(), 
						currAnnoInfo.textDx, currAnnoInfo.textDy, 
						currAnnoInfo.textRotation, currAnnoInfo.ta, VerticalAlignment.MIDDLE);
				}
			}
		}

		private void addFooter()
		{
			Paragraph p = new Paragraph(footer)
			.SetFontColor(rgbInfo2[11,0], 0.70f)
			.SetFontSize(6);

			placeSheetText(p, footerAnnoInfo.rect.GetX(), 
				footerAnnoInfo.rect.GetY(), 0, 0, 
				footerAnnoInfo.textRotation,
				footerAnnoInfo.ta, VerticalAlignment.BOTTOM);
		}

		/*page links .
			*/

		private void placePageRefLink()
		{
			Rectangle r;
			PdfAnnotation pa;

			canvas.BeginLayer(layer);

			foreach (TextAndLineSegmentData tals in currLeaf.PageLinkData)
			{
				if (tals.ToPageNumber == pageNum) continue;

				r = tals.GetRectangle();

				pa = makePageRefLinkAnnotation(r, tals);
				if (pa == null) continue;

				pdfPage.AddAnnotation(pa);

				placePageRefLinkRectangle(tals);
			}

			canvas.EndLayer();
		}

		private PdfAnnotation makePageRefLinkAnnotation(Rectangle r, TextAndLineSegmentData tals)
		{
			if (tals == null || tals.ToPageNumber == -1 || tals.Text.IsVoid()) return null;

			PdfLinkAnnotation pa;

			if (tals.IsRect)
			{
				pa = new PdfLinkAnnotation(r);
			}
			else
			{
				pa = makePolyPageRefLinkAnnotation(tals);
			}

			pa.SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT)
			.SetAction(PdfAction.CreateGoTo(
					PdfExplicitDestination.CreateFit(destPdfDoc.GetPage(tals.ToPageNumber))));

			pa.SetFlag(PdfAnnotation.LOCKED) ;

			return pa;
		}

		private PdfLinkAnnotation makePolyPageRefLinkAnnotation(TextAndLineSegmentData tals)
		{
			Rectangle r = tals.GetOArectangle();

			PdfLinkAnnotation pa = new PdfLinkAnnotation(r);
			pa.Put(PdfName.QuadPoints, tals.GetRectPolyPath() );

			return pa;
		}

		private void placePageRefLinkRectangle(TextAndLineSegmentData tals)
		{
			if (tals.IsRect)
			{
				canvas.Rectangle(tals.GetRectangle());
			}
			else
			{
				float[] path = tals.GetRectPath();

				canvas.MoveTo(path[0], path[1]);

				for (int i = 2; i < path.Length; i += 2)
				{
					canvas.LineTo(path[i], path[i + 1]);
				}

				canvas.ClosePath();
			}

			canvas.FillStroke();
		}

		private void configCanvasForPageRefLinks()
		{
			canvas.SetFillColor(new DeviceRgb(SkyBlue));
			canvas.SetStrokeColor(new DeviceRgb(MidnightBlue));
			canvas.SetLineDash(3, 1);
			canvas.SetLineWidth(2);
			canvas.SetExtGState(new PdfExtGState().SetFillOpacity(0.30f));
		}

		/*sheet links .
			*/

		// add sheet links such as disclaimer - these get rotated
		private void placeSheetLink(Rectangle r, int toPage, Color strokeColor, Color fillColor, float fillOpacity = 1)
		{
			Rectangle rr = rotateSheetRectangle(r);

			placePageLink(rr, toPage);

			placePageRectangle(rr, strokeColor, fillColor, fillOpacity);
		}

		/*page links .
		 */

		// place PdfAnnotation (e.g. link) without adjustments
		private void placePageLink(Rectangle r, int toPage)
		{
			PdfAnnotation pa = makePageRefLinkAnnotation(r, toPage);

			pdfPage.AddAnnotation(pa);
		}

		private PdfAnnotation makePageRefLinkAnnotation(Rectangle r, int toPage)
		{
			PdfLinkAnnotation pa;

			pa = new PdfLinkAnnotation(r)
			.SetAction(PdfAction.CreateGoTo(
					PdfExplicitDestination.CreateFit(
						destPdfDoc.GetPage(toPage))))
			.SetHighlightMode(PdfAnnotation.HIGHLIGHT_OUTLINE);

			pa.SetFlag(PdfAnnotation.LOCKED);

			return pa;
		}

		/*rects .
		 */

		// place a rectangle adjusted for sheet rotation
		private void placeSheetRectangle(Rectangle r, Color strokeColor, Color fillColor, float fillOpacity = 1)
		{
			Rectangle rr = rotateSheetRectangle(r);

			placePageRectangle(rr, strokeColor, fillColor, fillOpacity);
		}

		// place rectangle without adjustments
		private void placePageRectangle(Rectangle r, Color strokeColor, Color fillColor, float fillOpacity = 1)
		{
			canvas.SaveState();

			PdfExtGState gs = new PdfExtGState().SetFillOpacity(fillOpacity);

			canvas.Rectangle(r)
			.SetExtGState(gs)
			.SetLineWidth(0)
			.SetStrokeColor(strokeColor)
			.SetFillColor(fillColor)
			.FillStroke();

			canvas.RestoreState();
		}

		/*Text .
		 */

		// general place text at any sheet coordinate
		private void placeSheetText(Paragraph text, float x0, float y0,
			float dx, float dy,
			float txtRotation, 
			TextAlignment ta, VerticalAlignment va)
		{
			float rotation;
			float x1;
			float y1;

			rotation = adjustTextRotation(txtRotation);

			rotateCoord(x0, y0, out x1, out y1);

			adjustTextLocation(x1, y1, dx, dy, out x1, out y1);

			destDoc.ShowTextAligned(text, x1, y1, pageNum, ta, va, rotation);
		}

		/*watermark .
		 */

		private void addSheetWatermark(int which)
		{
			Style watermarkStyle = new Style()
			.SetFont(cfg.DefaultFont)
			.SetFontColor(rgbInfo2[11, 0], 0.40f)
			.SetFontSize(watermarkHeight);

			Paragraph p = new Paragraph(watermark).AddStyle(watermarkStyle);

			if (which == 0 || which == 2)
			{
				placeSheetWatermarkCentered(p);
			}

			if (which == 1 || which == 2)
			{
				placeSheetWatermarkTitleBlock(p);
			}
		}

		private void placeSheetWatermarkCentered(Paragraph p)
		{
			float rotation = FloatOps.ToRad(36);

			float x = pageSizeWithRotation.GetWidth() / 2;
			float y = pageSizeWithRotation.GetHeight() / 2;

			placeSheetText(p, x, y, 0,  0, rotation, TextAlignment.CENTER, VerticalAlignment.MIDDLE);
		}

		private void placeSheetWatermarkTitleBlock(Paragraph p)
		{
			float rotation = FloatOps.ToRad(90);

			float y = pageSizeWithRotation.GetHeight() / 2;
			float x =  pageSizeWithRotation.GetWidth() - 2f * 72f;

			placeSheetText(p, x, y, 0, 0, rotation, TextAlignment.CENTER, VerticalAlignment.MIDDLE);
		}


		/* datum circle .
		 */

		private void placeDatumCircle( double x, double y, double r)
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

		/*general routines .
		 */

		/*adjust location / rotations .
		 */

		/*rects .
		 */

		private void adjustPageRotation()
		{
			Rectangle ps = pdfPage.GetPageSizeWithRotation();

			if (!(ps.GetWidth() > ps.GetHeight()) && pdfPage.GetRotation() == 0)
			{
				pdfPage.SetRotation(-90);
			}
		}

		private Rectangle rotateFindRectangle(Rectangle r)
		{
			if (pageRotation == 0) return r;

			Rectangle rr;

			if (pageRotation == 90)
			{
				rr = new Rectangle(pageSizeWithRotation.GetHeight() - r.GetY() - r.GetHeight(), r.GetX(), r.GetHeight(), r.GetWidth());
			}
			else
			{
				rr = new Rectangle(r.GetY(), pageSizeWithRotation.GetWidth() - r.GetX() - r.GetWidth(), r.GetHeight(), r.GetWidth());
			}

			return rr;
		}

		private Rectangle rotateSheetRectangle(Rectangle r)
		{
			if (pageRotation == 0) return r;

			Rectangle rr;
			Rectangle ps = pageSizeWithRotation;

			if (pageRotation == 90)
			{
				rr = new Rectangle(ps.GetHeight() - r.GetY(), r.GetX(), -1 * r.GetHeight(), r.GetWidth());
			}
			else
			{
				rr = new Rectangle(r.GetY(), ps.GetWidth() - r.GetX(), r.GetHeight(), -1 * r.GetWidth());
			}

			return rr;
		}


		/*coords .
		 */

		// for when rotating rectangles with widths and heights

		private void rotateCoord(float x0, float y0, out float x1, out float y1)
		{
			x1 = x0;
			y1 = y0;

			if (pageRotation == 0) return ;

			if (pageRotation == 90)
			{
				y1 = x0;
				x1 = pageSizeWithRotation.GetHeight() - y0;
			}
			else
			{
				x1 = y0;
				y1 = pageSizeWithRotation.GetWidth() - x0;
			}
		}


		/* text .
		 */

		private float adjustTextRotation(float txtRotation)
		{
			if (pageRotation == 0) return txtRotation;

			float rotation = FloatOps.ToRad(90);

			if (pageRotation == 90)
			{
				return txtRotation + rotation;
			}

			return txtRotation - rotation;
		}

		private void adjustTextLocation(float x, float y,
			float dx0, float dy0, out float x1, out float y1)
		{
			x1 = dx0;
			y1 = dy0;

			if (pageRotation != 0)
			{
				(y1, x1) = (dx0, dy0);

				if (pageRotation == 90)
				{
					x1 = -1 * x1;
				}
				else
				{
					y1 = -1 * y1;
				}
			}

			x1 += x;
			y1 += y;
		}

	#endregion

		private void makeBoxes()
		{
			// values based on landscape orientation
			// 0,0 at lower-left corner

			float ps0x = 48f * 72f;
			float ps0y = 36f * 72f;

			float ctr0x = ps0x / 2;
			float ctr0y = ps0y / 2;

			float ps1x = 42f * 72f;
			float ps1y = 30f * 72f;

			float ctr1x = ps1x / 2;
			float ctr1y = ps1y / 2;


			float snBxW = 190f;
			float snBxH = 150f;

			float auBxW = 190f;
			float auBxH = 150f;


			float bnH = 30f;
			float bnW = 720f;

			float dsBxW = 10f;
			float dsBxH = 250f;

			float tstBxW = 150f;
			float tstBxH = 50f;


			float W = LINK_BOX_W;
			float H = LINK_BOX_H;

			float marginX = 70;
			float marginY = 50;

			float bannerMarginX = 10f;
			float bannerMarginY = 10f;

			rgbInfo2 = new DeviceRgb[,]
			{
				{ new DeviceRgb(255, 180, 180), new DeviceRgb(255, 90, 90) }, // 0   redish
				{ new DeviceRgb(180, 255, 180), new DeviceRgb(90, 255, 90) }, // 1  greenish
				{ new DeviceRgb(180, 180, 255), new DeviceRgb(90, 90, 255) }, // 2  blueish
				{ new DeviceRgb(140, 40, 225), new DeviceRgb(140, 20, 180) }, // 3  purple
				{ new DeviceRgb(150, 200, 50), new DeviceRgb(80, 100, 25) },  // 4  olive
				{ new DeviceRgb(255, 125, 90), new DeviceRgb(170, 80, 50) },  // 5  orangish

				{ new DeviceRgb(Red), DeviceRgb.MakeDarker(new DeviceRgb(Red)), },                 // 6
				{ new DeviceRgb(DeepSkyBlue), DeviceRgb.MakeDarker(new DeviceRgb(DeepSkyBlue)), }, // 7
				{ new DeviceRgb(LawnGreen), DeviceRgb.MakeDarker(new DeviceRgb(LawnGreen)), },     // 8
				{ new DeviceRgb(Tomato), DeviceRgb.MakeDarker(new DeviceRgb(Tomato)), },           // 9

				{ new DeviceRgb(Fuchsia), DeviceRgb.MakeDarker(new DeviceRgb(Fuchsia)), },           // 10
				{ new DeviceRgb(LightSkyBlue), DeviceRgb.MakeDarker(new DeviceRgb(LightSkyBlue)), }, // 11
			};


			// boxes[] = 0 for 36x48 | 1 for 30x42
			// boxes[][0] = sheet number box
			// boxes[][1] = author box
			// boxes[][3] = disclaimer box
			rectBoxes = new []
			{
				new Rectangle(ps0x - snBxW - marginX, marginY, snBxW, snBxH),                // 0  sheet number
				new Rectangle(ps0x - auBxW - marginX, ps0y - marginY - auBxH, auBxW, auBxH), // 1  author
				new Rectangle(marginX, ps0y - marginY - dsBxH, dsBxW, dsBxH),                // 2  disclaimer

				new Rectangle(0, 0, tstBxW, tstBxH),                         // 3  at origin
				new Rectangle(ps0x - tstBxW, 0, tstBxW, tstBxH),             // 4  bottom edge right side
				new Rectangle(ps0x - tstBxH, ps0y - tstBxW, tstBxH, tstBxW), // 5  origin or diag corner

				// to page A left-down from center - horizontal orientation
				new Rectangle(ctr0x - 75 , ctr0y - 175, W, H), // 6 ctr - lower-left
				new Rectangle(ctr0x + 125, ctr0y - 75 , H, W), // 7 ctr - lower-right
				new Rectangle(ctr0x - 175, ctr0y + 125, W, H), // 8 ctr - upper-right
				new Rectangle(ctr0x - 175, ctr0y - 175, H, W), // 9 ctr - upper-left
				new Rectangle(0, 0, W, H),                     // 10 origin
				new Rectangle(800, 300, W, H),                 // 11 test

				new Rectangle(3244.25f, 122.04f, 79.11f, 47.27f),   // 12 - 0° rotation
				new Rectangle(2469.96f, 3244.25f, -47.27f, 79.08f), // 13 - 90° rotation
				new Rectangle(121.99f, 212.06f, 47.27f, -79.08f),   // 14 - 270° rotation

				new Rectangle(1400f, 1000f, 250f, 75f), // 15 - 

				new Rectangle(ps0x - bannerMarginX-bnW, bannerMarginY, bnW, bnH),				// 16 - banner, BH
				new Rectangle(ps0x - bannerMarginX-bnH, bannerMarginY, bnH, bnW),			// 17 - banner, BV
					
				new Rectangle(ps0x - bannerMarginX-bnW, ps0y-bannerMarginY-bnH, bnW, bnH),	// 18 - banner, TH
				new Rectangle(ps0x - bannerMarginX-bnH, ps0y-bannerMarginY-bnW, bnH, bnW),	// 19 - banner, TV

				new Rectangle(10,5, 150,10),												// 20 - footer
				new Rectangle(3245, 58, 170, 158),										    // 21 - actual sheet number location for a 36x48 sheet
				new Rectangle(3276, 864, 108,720),                                          // 22 - test sheet name location (for sample 5)
				new Rectangle(3235, 645, 180, 360),                                         // 23 - test sheet name location (for coliseum sheet)

					
			};

			origin = new annoInfo(rectBoxes[10], rgbInfo2[6, 0], rgbInfo2[6, 1], "36x48 - link - E (Origin)",
				0, LINK_BOX_W / 2, LINK_BOX_H / 2, TextAlignment.CENTER);

			footerAnnoInfo = new annoInfo(rectBoxes[22], rgbInfo2[10, 0], rgbInfo2[10, 1], footer, 0, 0, 10/2f, TextAlignment.LEFT);

			banners = new annoInfo[,]
			{
				{
					new annoInfo(rectBoxes[16], rgbInfo2[0, 0], rgbInfo2[0, 1], bannerBh, 0, bnW, bnH/2, TextAlignment.RIGHT),   // banner, BH                                               // 0
					new annoInfo(rectBoxes[17], rgbInfo2[1, 0], rgbInfo2[1, 1], bannerBv, 90, bnH/2, 0, TextAlignment.LEFT),   // banner, BH                                               // 0
					new annoInfo(rectBoxes[18], rgbInfo2[2, 0], rgbInfo2[2, 1], bannerTh, 0, bnW, bnH/2, TextAlignment.RIGHT),   // banner, BH                                               // 0
					new annoInfo(rectBoxes[19], rgbInfo2[3, 0], rgbInfo2[3, 1], bannerTv, 90, bnH/2, 0, TextAlignment.LEFT),   // banner, BH                                               // 0
				}
			};

			pageAnno = new annoInfo[,]
			{
				{
					new annoInfo(rectBoxes[21], rgbInfo2[0, 0], rgbInfo2[0, 1], null, 0, 10, 20, TextAlignment.RIGHT),          // 0 - sheet number
					new annoInfo(rectBoxes[1], rgbInfo2[1, 0], rgbInfo2[1, 1], null, 0, 10, 20, TextAlignment.LEFT),           // 1 - author
					new annoInfo(rectBoxes[2], rgbInfo2[2, 0], rgbInfo2[2, 1], "please press this link in order to view the disclaimer", 90, 5, 5, TextAlignment.LEFT), // 2
				}
			};


			annoInfo = new annoInfo[,]
			{
				{
					new annoInfo(rectBoxes[1], rgbInfo2[1, 0], rgbInfo2[1, 1], "36x48 - anno - author", 0, 15, 15, TextAlignment.LEFT),        // 0
					new annoInfo(rectBoxes[2], rgbInfo2[2, 0], rgbInfo2[2, 1], "36x48 - anno - disclaimer", 90, 5, 5, TextAlignment.LEFT),     // 1
					new annoInfo(rectBoxes[4], rgbInfo2[4, 0], rgbInfo2[4, 1], "36x48 - anno - lower-right", 0, 15, 15, TextAlignment.LEFT),   // 2
					new annoInfo(rectBoxes[5], rgbInfo2[5, 0], rgbInfo2[5, 1], "36x48 - anno - upper-right", 90, 15, 15, TextAlignment.RIGHT), // 3
					// new annoInfo(rectBoxes[0], rgbInfo2[0,0], rgbInfo2[0,1], "36x48 - anno - sheet number"),
				},
				{
					new annoInfo(rectBoxes[1], rgbInfo2[1, 0], rgbInfo2[1, 1], "30x42 - anno - author") ,
					new annoInfo(rectBoxes[2], rgbInfo2[2, 0], rgbInfo2[2, 1], "30x42 - anno - disclaimer"),
					new annoInfo(rectBoxes[4], rgbInfo2[4, 0], rgbInfo2[4, 1], "30x42 - anno - lower-right"),
					new annoInfo(rectBoxes[5], rgbInfo2[5, 0], rgbInfo2[5, 1], "30x42 - anno - upper-right")
					// new annoInfo(rectBoxes[0], rgbInfo2[0,0], rgbInfo2[0,1], "30x42 - anno - sheet number"),
				}
			};

			testRectsDoNotRotate = new annoInfo[,]
			{
				{
					new annoInfo(rectBoxes[3], rgbInfo2[10, 0], rgbInfo2[10, 1], "A A0.0 - origin", 0, 15, 15, TextAlignment.LEFT),
					new annoInfo(rectBoxes[4], rgbInfo2[11, 0], rgbInfo2[11, 1], "B A0.0 - sht num corner", 0, 15, 15, TextAlignment.LEFT),
				},
				{
					new annoInfo(rectBoxes[3], rgbInfo2[10, 0], rgbInfo2[10, 1], "C A1.0 - origin", 0, 15, 15, TextAlignment.LEFT),
					new annoInfo(rectBoxes[4], rgbInfo2[11, 0], rgbInfo2[11, 1], "D A1.0 - sht num corner", 90, 15, 15, TextAlignment.LEFT),
				},
				{
					new annoInfo(rectBoxes[3], rgbInfo2[10, 0], rgbInfo2[10, 1], "E A2.0 - origin", 0, 15, 15, TextAlignment.LEFT),
					new annoInfo(rectBoxes[4], rgbInfo2[11, 0], rgbInfo2[11, 1], "F A2.0 - sht num corner", 0, 15, 15, TextAlignment.LEFT),
				},
				{
					new annoInfo(rectBoxes[3], rgbInfo2[10, 0], rgbInfo2[10, 1], "G A3.0 - origin", 0, 15, 15, TextAlignment.LEFT),
					new annoInfo(rectBoxes[4], rgbInfo2[11, 0], rgbInfo2[11, 1], "H A3.0 - sht num corner", 90, 15, 15, TextAlignment.LEFT),
				},
				{
					new annoInfo(rectBoxes[3], rgbInfo2[10, 0], rgbInfo2[10, 1], "I A4.0 - origin", 0, 15, 15, TextAlignment.LEFT),
					new annoInfo(rectBoxes[4], rgbInfo2[11, 0], rgbInfo2[11, 1], "J A4.0 - sht num corner", 0, 15, 15, TextAlignment.LEFT),
				},
				{
					new annoInfo(rectBoxes[3], rgbInfo2[10, 0], rgbInfo2[10, 1], "K 5.0 - origin", 0, 15, 15, TextAlignment.LEFT),
					new annoInfo(rectBoxes[4], rgbInfo2[11, 0], rgbInfo2[11, 1], "L A5.0 - sht num corner", 90, 15, 15, TextAlignment.LEFT),
				}
			};

			testRectsToRotate = new annoInfo[,]
			{
				{
					new annoInfo(rectBoxes[12], rgbInfo2[10, 0], rgbInfo2[10, 1], "M A0.0 - 0°", 0, 15, 15, TextAlignment.LEFT),
				},
				{
					new annoInfo(rectBoxes[14], rgbInfo2[10, 0], rgbInfo2[10, 1], "N A1.0 - 270°", 270, 15, 15, TextAlignment.LEFT),
				},
				{
					new annoInfo(rectBoxes[12], rgbInfo2[10, 0], rgbInfo2[10, 1], "P A2.0 - 0°", 0, 15, 15, TextAlignment.LEFT),
				},
				{
					new annoInfo(rectBoxes[14], rgbInfo2[10, 0], rgbInfo2[10, 1], "Q A3.0 - 270°", 270, 15, 15, TextAlignment.LEFT),
				},
				{
					new annoInfo(rectBoxes[12], rgbInfo2[10, 0], rgbInfo2[10, 1], "R A4.0 - 0°", 0, 15, 15, TextAlignment.LEFT),
				},
				{
					new annoInfo(rectBoxes[13], rgbInfo2[10, 0], rgbInfo2[10, 1], "S A5.0 - 90°", 90, 15, 15, TextAlignment.LEFT),
				}
			};


			linkInfo = new [,]
			{
				{
					new annoInfo(rectBoxes[6], rgbInfo2[6, 0], rgbInfo2[6, 1],
						"36x48 - link - A (LL)",
						0, LINK_BOX_W / 2, LINK_BOX_H / 2, TextAlignment.CENTER),

					new annoInfo(rectBoxes[7], rgbInfo2[7, 0], rgbInfo2[7, 1],
						"36x48 - link - B (LR)",
						90, LINK_BOX_H / 2, LINK_BOX_W / 2, TextAlignment.CENTER) ,

					new annoInfo(rectBoxes[8], rgbInfo2[8, 0], rgbInfo2[8, 1],
						"36x48 - link - C (UR)",
						0, LINK_BOX_W / 2, LINK_BOX_H / 2, TextAlignment.CENTER),

					new annoInfo(rectBoxes[9], rgbInfo2[9, 0], rgbInfo2[9, 1],
						"36x48 - link - D (UL)",
						90, LINK_BOX_H / 2, LINK_BOX_W / 2, TextAlignment.CENTER),

					new annoInfo(rectBoxes[0], rgbInfo2[10, 0], rgbInfo2[10, 1],
						"36x48 - link - F (Sht Num)",
						0, LINK_BOX_W / 2, LINK_BOX_H / 2, TextAlignment.CENTER),

					new annoInfo(rectBoxes[11], rgbInfo2[11, 0], rgbInfo2[11, 1],
						"36x48 - link - G (Test)",
						0, LINK_BOX_W / 2, LINK_BOX_H / 2, TextAlignment.CENTER),
				},
			};


		}

		public override string ToString()
		{
			return $"this is {nameof(PdfText302)}";
		}

		// 		private void addAnnoAndLinks2()
		// 		{
		// 			Debug.WriteLine("add annotations and links");
		//
		// 			layer = new PdfLayer("Hyperlinks Rectangles", destPdfDoc);
		// 			layer.SetOn(true);
		//
		// 			foreach (KeyValuePair<string, PdfTreeLeaf> kvp in pages)
		// 			{
		// 				if (kvp.Key[0] == '*') continue;
		//
		// 				currLeaf = kvp.Value;
		//
		// 				Debug.WriteLine($"\ntesting| {kvp.Key} - {kvp.Value.SheetName}");
		//
		// 				pageNum = currLeaf.PageNumber;
		//
		// 				addAnno(currLeaf);
		// 			}
		// 		}
		//
		// 		private void addAnno2(PdfTreeLeaf leaf)
		// 		{
		// 			pdfPage = destPdfDoc.GetPage(pageNum);
		//
		// 			pageRotation = pdfPage.GetRotation();
		// 			pageSize = pdfPage.GetPageSize();
		// 			pageSizeWithRotation = pdfPage.GetPageSizeWithRotation();
		//
		// 			canvas = new PdfCanvas(pdfPage);
		//
		// 			sd = leaf.SheetData;
		//
		// 			canvas.SaveState();
		//
		// 			// configCanvasForPageRefLinks(canvas);
		//
		// 			// addPageRefLinks(canvas);
		//
		// 			canvas.RestoreState();
		//
		//
		// 			// placePageRects(pageNum, canvas);
		//
		// 			// Debug.WriteLine("not rotated rectangles");
		// 			//
		// 			// placeTestRects(pageNum, canvas);
		//
		//
		// 			// Debug.WriteLine("rotated rectangles");
		// 			// placeTestRectsRotated(pageNum, canvas);
		//
		// 			// Debug.WriteLine("rotated rectangles");
		// 			// placeShtNumberRect(pageNum, canvas);
		//
		// 			// Debug.WriteLine("found sht number rotated rectangles");
		// 			// placeFoundShtNumberRects(pageNum, canvas, leaf);
		//
		// 			// placeTestRectsAtFind();
		//
		// 			placeWatermark();
		//
		// 			// addPageLinks();
		//
		// 			// rotatePage();
		// 		}
		//
		// 		/*		private void placeShtNumberRect(int pageNum, PdfCanvas canvas)
		// 		{
		// 			currAnnoInfo = testRectsToRotate[pageNum - 1, 0];
		//
		// 			string coord = $"{currAnnoInfo.rect.GetX():F2}, {currAnnoInfo.rect.GetY():F2} ";
		//
		// 			placePageRectangle($"text found| {currAnnoInfo.text} {coord}", pageNum, canvas, currAnnoInfo.rect, currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill, 0.30f);
		// 		}*/
		//
		// 		private void configCanvasForPageRefLinks2(PdfCanvas canvas)
		// 		{
		// 			canvas.SetFillColor(new DeviceRgb(SkyBlue));
		// 			canvas.SetStrokeColor(new DeviceRgb(MidnightBlue));
		// 			canvas.SetLineDash(3, 1);
		// 			canvas.SetLineWidth(2);
		// 			canvas.SetExtGState(new PdfExtGState().SetFillOpacity(0.30f));
		// 		}
		//
		// 		/*		private void placeTestRectsAtFind()
		// 		{
		// 			Debug.WriteLine($"sheet number rectangle for page | {pageNum}");
		//
		// 			currAnnoInfo = pageAnno[0, 0];
		//
		// 			Rectangle r = rotateSheetRectangle(currAnnoInfo.rect);
		//
		// 			placePageRectangle(currAnnoInfo.text, pageNum, canvas, r, currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill, 0.3f);
		// 			Debug.WriteLine($"find rect| {currAnnoInfo.text,-20}| {fmtRect(r)}");
		// 		}*/
		//
		//
		// 		private void addPageRefLinks2(PdfCanvas canvas)
		// 		{
		// 			// canvas.BeginLayer(layer);
		// 			placePageRefLink2(canvas);
		// 			// canvas.EndLayer();
		// 		}
		//
		// 		private void placePageRefLink2(PdfCanvas canvas)
		// 		{
		// 			Rectangle r;
		// 			PdfAnnotation pa;
		//
		// 			canvas.BeginLayer(layer);
		//
		// 			foreach (TextAndLineSegmentData tals in currLeaf.PageLinkData)
		// 			{
		// 				// if (tals.ToPageNumber == pageNum) continue;
		//
		// 				// Debug.WriteLine($"\nfor| {tals.Text}");
		//
		// 				r = tals.GetRectangle();
		//
		// 				// Paragraph p = new Paragraph($"sht xref link| {r.GetX():F2}, {r.GetY():F2}");
		// 				//
		// 				// placeText(p, pageNum, r);
		//
		// 				// showRects(r);
		//
		// 				pa = makeLinkAnnotation2(r, tals);
		// 				if (pa == null) continue;
		//
		// 				pdfPage.AddAnnotation(pa);
		//
		// 				placePageRefLinkRectangle2(canvas, tals);
		//
		// 				canvas.FillStroke();
		//
		// 			}
		//
		// 			canvas.EndLayer();
		// 		}
		//
		// 		private PdfAnnotation makeLinkAnnotation2(Rectangle r, TextAndLineSegmentData tals)
		// 		{
		// 			if (tals == null || tals.ToPageNumber == -1 || tals.Text.IsVoid()) return null;
		//
		// 			PdfLinkAnnotation pa = null;
		//
		// 			if (tals.IsRect)
		// 			{
		// 				pa = new PdfLinkAnnotation(r);
		// 			}
		// 			else
		// 			{
		// 				pa = makePolyLinkAnnotation2(tals);
		// 			}
		//
		// 			pa.SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT)
		// 			.SetAction(PdfAction.CreateGoTo(
		// 					PdfExplicitDestination.CreateFit(destPdfDoc.GetPage(tals.ToPageNumber))));
		// 			// .SetFlag(PdfAnnotation.LOCKED) ;
		//
		// 			return pa;
		// 		}
		//
		// 		private void placePageRefLinkRectangle2(PdfCanvas pdfCanvas, TextAndLineSegmentData tals)
		// 		{
		// 			Rectangle r;
		// 			Rectangle oar;
		//
		// 			if (tals.IsRect)
		// 			{
		// 				r = tals.GetRectangle();
		//
		// 				// r = rotateLinkAnnoRectangle(r);
		// 				// oar = rotateLinkAnnoRectangle(tals.GetOArectangle());
		//
		// 				// Debug.WriteLine($"   anno rect| {fmtRect(r, 1)}");
		//
		// 				pdfCanvas.Rectangle(r);
		// 			}
		// 			else
		// 			{
		// 				float[] a = tals.GetRectPath();
		// 				// float[] a = rotateLinkPath(tals.GetRectPath());
		//
		// 				pdfCanvas.MoveTo(a[0], a[1]);
		//
		// 				for (int i = 2; i < a.Length; i += 2)
		// 				{
		// 					pdfCanvas.LineTo(a[i], a[i + 1]);
		// 				}
		//
		// 				pdfCanvas.ClosePath();
		// 			}
		// 		}
		//
		// 		private void placeLinkRect(Rectangle r) { }
		//
		// 		private PdfLinkAnnotation makePolyLinkAnnotation2(TextAndLineSegmentData tals)
		// 		{
		// 			Rectangle r = tals.GetOArectangle();
		//
		// 			PdfLinkAnnotation pa = new PdfLinkAnnotation(r);
		// 			pa.Put(PdfName.QuadPoints, tals.GetRectPolyPath() );
		//
		// 			return pa;
		// 		}
		//
		//
		// 		/*
		// 		private void showRects(Rectangle r)
		// 		{
		// 			Debug.WriteLine($"unrotated|  {fmtRect(r,1)}");
		//
		// 			Rectangle rr = rotateRectangle(r);
		//
		// 			Debug.WriteLine($"rotated 1|  {fmtRect(rr,1)}");
		//
		// 			rr = rotateRectangle2(r);
		//
		// 			Debug.WriteLine($"rotated 2|  {fmtRect(rr,1)}");
		//
		// 			rr = rotateLinkAnnoRectangle(r);
		//
		// 			Debug.WriteLine($"rotated 3|  {fmtRect(rr,1)}\n");
		// 		}
		//
		// 		private PdfAnnotation makeLinkAnnotation(Rectangle r, TextAndLineSegmentData tals)
		// 		{
		// 			if (tals == null || tals.ToPageNumber == -1 || tals.Text.IsVoid()) return null;
		//
		// 			PdfLinkAnnotation pa;
		//
		// 			if (tals.IsRect)
		// 			{
		// 				pa = new PdfLinkAnnotation(r);
		// 			}
		// 			else
		// 			{
		// 				pa = makePolyLinkAnnotation(tals);
		// 			}
		//
		// 			pa.SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT)
		// 			.SetAction(PdfAction.CreateGoTo(
		// 					PdfExplicitDestination.CreateFit(destPdfDoc.GetPage(tals.ToPageNumber))));
		// 			// .SetFlag(PdfAnnotation.LOCKED) ;
		//
		// 			return pa;
		// 		}
		//
		// 		private void placeLinkRectangle(PdfCanvas pdfCanvas, TextAndLineSegmentData tals)
		// 		{
		// 			Rectangle r;
		// 			Rectangle oar;
		//
		// 			if (tals.IsRect)
		// 			{
		// 				r = tals.GetRectangle();
		//
		// 				r = rotateLinkAnnoRectangle(r);
		// 				oar = rotateLinkAnnoRectangle(tals.GetOArectangle());
		//
		// 				// Debug.WriteLine($"   anno rect| {fmtRect(r, 1)}");
		//
		// 				pdfCanvas.Rectangle(r);
		// 			}
		// 			else
		// 			{
		// 				// double[] a = tals.GetRectPath();
		// 				float[] a = rotateLinkPath(tals.GetRectPath());
		//
		// 				pdfCanvas.MoveTo(a[0], a[1]);
		//
		// 				for (int i = 2; i < a.Length; i += 2)
		// 				{
		// 					pdfCanvas.LineTo(a[i], a[i + 1]);
		// 				}
		//
		// 				pdfCanvas.ClosePath();
		// 			}
		// 		}
		//
		//
		//
		//  
		//
		//
		//
		// 		private void placeShtRefLink(PdfCanvas canvas)
		// 		{
		// 			Rectangle r;
		//
		// 			Debug.WriteLine($"\ncurr page| {currLeaf.SheetNumber}");
		//
		// 			foreach (TextAndLineSegmentData tals in currLeaf.PageLinkData)
		// 			{
		// 				// if (tals.ToPageNumber == pageNum) continue;
		//
		// 				Debug.WriteLine($"\nfor| {tals.Text}");
		//
		// 				r = tals.GetRectangle();
		// 				
		// 				showRects(r);
		//
		// 				pdfPage.AddAnnotation(makeLinkAnnotation(r, tals));
		//
		// 				placeLinkRectangle(canvas, tals);
		//
		// 				canvas.FillStroke();
		// 			}
		// 		}
		// 		
		// 		
		// 		
		// 		
		// 		private void configCanvasForSheetRefLinks(PdfCanvas canvas)
		// 		{
		// 			canvas.SetFillColor(new DeviceRgb(SkyBlue));
		// 			canvas.SetStrokeColor(new DeviceRgb(MidnightBlue));
		// 			canvas.SetLineDash(3,1);
		// 			canvas.SetLineWidth(2);
		// 			canvas.SetExtGState(new PdfExtGState().SetFillOpacity(0.30f));
		// 		}
		//
		// 		private void configCanvasForPageLinks(PdfCanvas canvas)
		// 		{
		// 			canvas.SetStrokeColor(new DeviceRgb(Black));
		// 			canvas.SetExtGState(new PdfExtGState().SetFillOpacity(1.0f));
		// 		}
		//
		// 		private void addSheetRefLinks(PdfCanvas canvas)
		// 		{
		// 			canvas.BeginLayer(layer);
		// 			placeShtRefLink(canvas);
		// 			canvas.EndLayer();
		//
		// 		}
		// */
		//
		// 		// page anno
		//
		// 		private void addPageLinks2()
		// 		{
		// 			annoInfo ai;
		//
		// 			for (var i = 0; i < pageAnno.GetLength(1); i++)
		// 			{
		// 				ai = pageAnno[0, i];
		//
		// 				currAnnoInfo = ai;
		//
		// 				Rectangle r = rotateSheetRectangle(currAnnoInfo.rect);
		//
		// 				placePageLink(r, pageRefs[pageNum + i]);
		//
		// 				placeSheetRectangle(r, currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill, 0.30f);
		//
		//
		// 				// string text = $"on page {pageNum}| name {currAnnoInfo.text} to page {pageRefs[pageNum + i]}";
		// 				string text = $"{currAnnoInfo.text} to page {pageRefs[pageNum + i]} | x {r.GetX():F2}, y {r.GetY():F2}";
		//
		// 				Paragraph p = new Paragraph(text);
		//
		// 				// placeText(p, pageNum, r, -10f, -10f);
		// 			}
		//
		// 			// currAnnoInfo = origin;
		//
		// 			// placePageLink(currAnnoInfo.rect, pageRefs[0]);
		// 			//
		// 			// placeRectangle(currAnnoInfo.text, pageNum, canvas, currAnnoInfo.rect, 
		// 			// 	currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill, 0.30f);
		// 		}
		//
		// 		private void placePageLink2(Rectangle r, int toPage)
		// 		{
		// 			PdfAnnotation pa = makePageRefLinkAnnotation(r, toPage);
		//
		// 			pdfPage.AddAnnotation(pa);
		// 		}
		//
		// 		private PdfAnnotation makePageLinkAnnotation2(Rectangle r, int toPage)
		// 		{
		// 			PdfLinkAnnotation pa;
		//
		// 			string text = $"{currAnnoInfo.text} to page {toPage}";
		//
		// 			pa = new PdfLinkAnnotation(r)
		// 			.SetAction(PdfAction.CreateGoTo(
		// 					PdfExplicitDestination.CreateFit(
		// 						destPdfDoc.GetPage(toPage))))
		// 			.SetHighlightMode(PdfAnnotation.HIGHLIGHT_OUTLINE);
		//
		//
		// 			pa.SetTitle(new PdfString(text))
		// 			.SetContents(text)
		// 			.SetContents(text)
		// 			.SetFlag(PdfAnnotation.LOCKED)
		// 			.SetColor(rgbInfo2[7, 0]);
		//
		// 			return pa;
		// 		}
		//
		// 		private void placeWatermark()
		// 		{
		// 			Style watermarkStyle = new Style()
		// 			.SetFont(cfg.DefaultFont)
		// 			.SetFontColor(rgbInfo2[11, 0], 0.40f)
		// 			.SetFontSize(watermarkHeight);
		//
		// 			Paragraph p = new Paragraph(watermark).AddStyle(watermarkStyle);
		//
		//
		// 			float rotation = FloatOps.ToRad(36);
		//
		// 			float x = pageSizeWithRotation.GetWidth() / 2;
		// 			float y = pageSizeWithRotation.GetHeight() / 2;
		//
		// 			placeSheetText(p, x, y, rotation, TextAlignment.CENTER, VerticalAlignment.MIDDLE);
		//
		//
		// 			rotation = FloatOps.ToRad(90);
		//
		// 			x =  pageSizeWithRotation.GetWidth() - 2f * 72f;
		//
		// 			placeSheetText(p, x, y, rotation, TextAlignment.CENTER, VerticalAlignment.MIDDLE);
		// 		}
		//
		// 		private void placeRectangle2(string name, int pageNum, PdfCanvas canvas,
		// 			Rectangle r, Color strokeColor, Color fillColor,
		// 			float fillOpacity = 1)
		// 		{
		// 			canvas.SaveState();
		//
		// 			PdfExtGState gs = new PdfExtGState().SetFillOpacity(fillOpacity);
		//
		// 			canvas.Rectangle(r)
		// 			.SetExtGState(gs)
		// 			.SetLineWidth(0)
		// 			.SetStrokeColor(strokeColor)
		// 			.SetFillColor(fillColor)
		// 			.FillStroke();
		//
		// 			canvas.RestoreState();
		//
		// 			placeDatumCircle(r.GetX(), r.GetY(), 3);
		//
		// 			Paragraph p = new Paragraph(name);
		//
		// 			// placeText(p, pageNum, r);
		// 		}


		// private void placeCircle(PdfCanvas canvas, double x, double y, double r)
		// {
		// 	PdfExtGState gs = new PdfExtGState().SetFillOpacity(0.3f);
		//
		// 	canvas.SaveState();
		//
		// 	canvas.SetLineWidth(0.25f);
		// 	canvas.SetStrokeColor(DeviceRgb.BLACK);
		// 	canvas.SetFillColor(DeviceRgb.GREEN);
		// 	canvas.SetExtGState(gs);
		//
		// 	canvas.MoveTo(x - 5, y);
		// 	canvas.LineTo(x + 5, y);
		//
		// 	canvas.MoveTo(x, y - 5);
		// 	canvas.LineTo(x, y + 5);
		//
		// 	canvas.Circle(x, y, r);
		//
		// 	canvas.FillStroke();
		//
		// 	canvas.RestoreState();
		// }


		// removed as this is based on text coordinates + an offset
		// private void placeText(Paragraph text, int pageNum, Rectangle r, float dx = 0, float dy = 0)
		// {
		// 	float rotation;
		// 	float x1;
		// 	float y1;
		//
		// 	rotation = adjustTextRotation(currAnnoInfo.textRotation);
		//
		// 	// not correct
		// 	rotateCoord(currAnnoInfo.textX + dx + txtDx, currAnnoInfo.textY + dy + txtDy, out x1, out y1);
		//
		// 	rotation = adjustTextLocAndRot(currAnnoInfo.textRotation,
		// 		currAnnoInfo.textX + dx + txtDx,
		// 		currAnnoInfo.textY + dy + txtDy,
		// 		out x1, out y1);
		//
		//
		// 	text.Add($" | dx {x1:F2}, dy {y1}");
		//
		// 	x1 += r.GetX();
		// 	y1 += r.GetY();
		//
		//
		// 	destDoc.ShowTextAligned(text, x1, y1, pageNum,
		// 		currAnnoInfo.ta, VerticalAlignment.MIDDLE, rotation);
		//
		// 	txtDx = 0;
		// 	txtDy = 0;
		// }


		// private void adjustTextLocation(float x0, float y0, out float x1, out float y1)
		// {
		// 	x1 = x0;
		// 	y1 = y0;
		//
		// 	if (pageRotation == 0) return;
		//
		// 	(y1, x1) = (x0, y0);
		//
		// 	if (pageRotation == 90)
		// 	{
		// 		x1 = -1 * x1;
		// 	}
		// 	else
		// 	{
		// 		y1 = -1 * y1;
		// 	}
		// }

		// private float adjustTextLocAndRot(float txtRotation,
		// 	float x0, float y0,
		// 	out float x1, out float y1)
		// {
		// 	x1 = x0;
		// 	y1 = y0;
		//
		// 	if (pageRotation == 0) return txtRotation;
		//
		// 	float rotation = FloatOps.ToRad(90);
		//
		// 	(y1, x1) = (x0, y0);
		//
		// 	if (pageRotation == 90)
		// 	{
		// 		x1 = -1 * x1;
		//
		// 		rotation = txtRotation + rotation;
		// 	}
		// 	else
		// 	{
		// 		y1 = -1 * y1;
		//
		// 		rotation = txtRotation - rotation;
		// 	}
		//
		// 	return rotation;
		// }	


		// private void placeFoundShtNumberRects(int pageNum, PdfCanvas canvas, PdfTreeLeaf leaf)
		// {
		// 	TextAndLineSegmentData tals = leaf.PageLinkData[0];
		//
		// 	string coord = $"{tals.GetRectangle().GetX():F2}, {tals.GetRectangle().GetY():F2} ";
		//
		// 	placeRectangle($"text found| {tals.Text} {coord}", pageNum, canvas,tals.GetRectangle(), rgbInfo2[11,0], rgbInfo2[11,1], 0.30f);
		//
		// }
		//
		// private void placeRectRotationTest(int pageNum, PdfCanvas canvas, 
		// 	string text,
		// 	Rectangle r,
		// 	DeviceRgb border,
		// 	DeviceRgb fill
		// 	)
		// {
		// 	Rectangle rr = r;
		//
		// 	string coord = $"{rr.GetX():F2}, {rr.GetY():F2} ";
		//
		// 	txtDx = txtDy = -10;
		// 	Debug.WriteLine($" unrotated rect| {currAnnoInfo.text,-20}| {fmtRect(r)}");
		// 	placeRectangle(coord+text+" no rot", pageNum, canvas, rr, border, fill, 0.3f);
		//
		// 	txtDx = txtDy = -3;
		// 	rr = rotateRectangle(r);
		// 	Debug.WriteLine($" rotate v1 rect| {currAnnoInfo.text,-20}| {fmtRect(rr)}");
		// 	placeRectangle(coord+text+" v1 rot", pageNum, canvas, rr, border, fill, 0.3f);
		//
		// 	txtDx = txtDy = 3;
		// 	rr = rotateRectangle2(r);
		// 	Debug.WriteLine($" rotate v2 rect| {currAnnoInfo.text,-20}| {fmtRect(rr)}");
		// 	placeRectangle(coord+text+" v2 rot", pageNum, canvas, rr, border, fill, 0.3f);
		// 	
		// 	txtDx = txtDy = 10;
		// 	rr = rotateLinkAnnoRectangle(r);
		// 	Debug.WriteLine($" rotate v3 rect| {currAnnoInfo.text,-20}| {fmtRect(rr)}");
		// 	placeRectangle(coord+text+" v3 rot", pageNum, canvas, rr, border, fill, 0.3f);
		// }


		// sheet links

		// private void placeTestRects(int pageNum, PdfCanvas canvas)
		// {
		// 	Rectangle ps;
		//
		// 	// ps = pageSize;
		// 	ps = pageSizeWithRotation;
		//
		// 	Debug.WriteLine($"for page | {pageNum}");
		// 	Debug.WriteLine($"page info| rotation {pageRotation}");
		// 	Debug.WriteLine($"page info| pg sz-no rot| w {pageSize.GetWidth()} x h{pageSize.GetHeight()}");
		// 	Debug.WriteLine($"page info| pg sz-w  rot| w {pageSizeWithRotation.GetWidth()} x h{pageSizeWithRotation.GetHeight()}");
		//
		// 	for (var i = 0; i < testRectsDoNotRotate.GetLength(1); i++)
		// 	{
		// 		currAnnoInfo = testRectsDoNotRotate[pageNum-1, i];
		// 		placeRectangle(currAnnoInfo.text, pageNum, canvas, currAnnoInfo.rect, currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill, 0.3f);
		//
		// 		Debug.WriteLine($"rect| {currAnnoInfo.text,-20}| {fmtRect(currAnnoInfo.rect)}");
		// 	}
		// }


		// private void placeTestRectsRotated(int pageNum, PdfCanvas canvas)
		// {
		// 	Rectangle rr;
		// 	
		// 	Rectangle ps;
		//
		// 	// ps = pageSize;
		// 	ps = pageSizeWithRotation;
		//
		// 	Debug.WriteLine($"for page | {pageNum}");
		// 	Debug.WriteLine($"page info| rotation {pageRotation}");
		// 	Debug.WriteLine($"page info| pg sz-no rot| w {pageSize.GetWidth()} x h{pageSize.GetHeight()}");
		// 	Debug.WriteLine($"page info| pg sz-w  rot| w {pageSizeWithRotation.GetWidth()} x h{pageSizeWithRotation.GetHeight()}");
		//
		// 	for (var i = 0; i < testRectsToRotate.GetLength(1); i++)
		// 	{
		// 		currAnnoInfo = testRectsToRotate[pageNum-1, i];
		//
		// 		placeRectRotationTest(pageNum, canvas, currAnnoInfo.text, currAnnoInfo.rect, currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill);
		// 	}
		//
		//
		// 	placeRectRotationTest(pageNum, canvas, $"test rect",  rectBoxes[15], rgbInfo2[10,0], rgbInfo2[10,1]);
		// }


		// private void rotateCoord(float x0, float y0, out float x1, out float y1)
		// {
		// 	x1 = x0;
		// 	y1 = y0;
		//
		// 	if (pageRotation == 0) return ;
		//
		// 	Rectangle r = pageSizeWithRotation;
		//
		// 	if (pageRotation == 90)
		// 	{
		// 		y1 = x0;
		// 		x1 = r.GetHeight() - y0;
		// 	}
		// 	else
		// 	{
		// 		x1 = y0;
		// 		y1 = r.GetWidth() - x0;
		// 	}
		// }


		// // rotate v1
		// private Rectangle rotateRectangleForShtNumFind(Rectangle r)
		// {
		//
		// 	// if (checkRotatePage() == 0 || !doRotate) return r;
		//
		// 	if (pageRotation == 0) return r;
		//
		// 	Rectangle rr = null;
		// 	Rectangle ps = pageSizeWithRotation;
		//
		// 	if (pageRotation == 90)
		// 	{
		// 		rr = new Rectangle(ps.GetHeight() - r.GetY() - r.GetHeight(), r.GetX(), r.GetHeight(), r.GetWidth());
		// 	}
		// 	else
		// 	{
		// 		rr = new Rectangle(r.GetY(), ps.GetWidth() - r.GetX() - r.GetWidth(), r.GetHeight(), r.GetWidth());
		// 	}
		//
		// 	return rr;
		// }


		// private void placeWatermark2()
		// {
		// 	Rectangle r;
		//
		// 	float rotation = FloatOps.ToRad(36);
		//
		// 	float x = pageSizeWithRotation.GetWidth()/ 2;
		// 	float y = pageSizeWithRotation.GetHeight() / 2;
		//
		// 	r = rotateRectangle(new Rectangle(x, y, 0f, 0f));
		//
		// 	Style watermarkStyle = new Style()
		// 	.SetFont(cfg.DefaultFont)
		// 	.SetFontColor(rgbInfo2[11, 0], 0.40f)
		// 	.SetFontSize(watermarkHeight);
		//
		// 	Paragraph p = new Paragraph(watermark).AddStyle(watermarkStyle);
		//
		// 	placeText(p, r.GetX(), r.GetY(), rotation, TextAlignment.CENTER, VerticalAlignment.MIDDLE);
		//
		// 	x =  pageSizeWithRotation.GetWidth() - 2f * 72f;
		//
		// 	r = rotateRectangle(new Rectangle(x, y, 0f, 0f));
		//
		// 	rotation = FloatOps.ToRad(90);
		//
		// 	placeText(p, r.GetX(), r.GetY(), rotation, TextAlignment.CENTER, VerticalAlignment.MIDDLE);
		//
		// }


		/*	private void placePageRects(int pageNum, PdfCanvas canvas)
			{
				// SheetData sd = leaf.SheetData;

				Rectangle ps;

				// ps = pageSize;
				ps = pageSizeWithRotation;

				Debug.WriteLine($"page info| rotation {pageRotation}");
				Debug.WriteLine($"page info| pg sz-no rot| w {pageSize.GetWidth()} x h{pageSize.GetHeight()}");
				Debug.WriteLine($"page info| pg sz-w  rot| w {pageSizeWithRotation.GetWidth()} x h{pageSizeWithRotation.GetHeight()}");

				annoInfo ai;

				for (var i = 0; i < annoInfo.GetLength(1); i++)
				{
					currAnnoInfo = annoInfo[0, i];
					// placeTestRect(currAnnoInfo.text, pageNum, canvas, currAnnoInfo.rect, currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill);

					placeRectangle(currAnnoInfo.text, pageNum, canvas, currAnnoInfo.rect, currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill, 0.3f);
				}
			}*/

		// page links

		/*


		/*removed
		 */

		/*



*/

		// general


		// private void placeText(Paragraph text, float x0, float y0, float txtRotation, TextAlignment ta, VerticalAlignment va)
		// {
		// 	float rotation;
		// 	float x1;
		// 	float y1;
		//
		// 	rotation = adjustTextLocAndRot(txtRotation, x0,y0, out x1, out y1, false);
		//
		// 	destDoc.ShowTextAligned(text, x0, y0, pageNum, ta, va, rotation);
		// }

		/*
				private void placeRectangle(PdfCanvas canvas, Rectangle r)
				{
					placeRectangle(currAnnoInfo.text, pageNum, canvas, r, 
						currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill, 0.30f);
				}
		*/

		/*		private void placeTestRect(string name, int pageNum, PdfCanvas canvas, Rectangle r, 
					DeviceRgb borderRgb, DeviceRgb fillRgb)
				{
					if (r == null) return;

					placeRectangle(name, pageNum, canvas, r, borderRgb, fillRgb, 0.3f);
				}
		*/


		/*
		private void rotateLinkAnnoCoord(float x0, float y0, out float x1, out float y1, out int sw, out int sh)
		{
			x1 = x0;
			y1 = y0;
			sw = -1;
			sh = 1;
		
			Rectangle r = pageSizeWithRotation;

		
			if (pageRotation == 0) return ;


			return;

		
			if (pageRotation == 90)
			{
				y1 = x0;
				x1 = r.GetHeight() - y0;
			}
			else
			{
				x1 = y0;
				y1=r.GetWidth() - x0;
		
				sw *= -1;
				sh *= -1;
			}
		
		}
		*/

		// // rotate v2
		// private Rectangle rotateRectangle2(Rectangle r)
		// {
		//
		// 	// if (checkRotatePage() == 0 || !doRotate) return r;
		//
		// 	if (pageRotation == 0) return r;
		//
		// 	Rectangle rr = null;
		// 	Rectangle ps = pageSizeWithRotation;
		//
		// 	float x1;
		// 	float y1;
		// 	int sw;
		// 	int sh;
		//
		// 	rotateLinkAnnoCoord(r.GetX(),r.GetY(), out x1, out y1, out sw, out sh);
		//
		// 	rr = new Rectangle(x1, y1, r.GetHeight()*sw, r.GetWidth()*sh);
		//
		// 	return rr;
		// }
		//
		// // rotate v3
		// private Rectangle rotateLinkAnnoRectangle(Rectangle r)
		// {
		//
		// 	// return rotateRectangle2(r);
		//
		// 	if (pageRotation == 0) return r;
		//
		// 	Rectangle rr = null;
		// 	Rectangle ps = pageSizeWithRotation;
		//
		// 	if (pageRotation == 270)
		// 	{
		// 		// for 270 = page 1
		// 		rr = new Rectangle(ps.GetWidth()-r.GetY()-r.GetHeight(), r.GetX(), r.GetHeight(), r.GetWidth());
		// 	}
		// 	else
		// 	{
		// 		// for 90 = page 5
		// 		rr = new Rectangle(r.GetY()+r.GetHeight(), ps.GetHeight()-r.GetX(), -1*r.GetHeight(), -1*r.GetWidth());
		// 	}
		//
		// 	return rr;
		// }

		/*
		private  float[] rotateLinkPath(float[] pt0)
		{


			int sw, sh;
			if (pageRotation == 0) return pt0;

			float[] pt1=new float[pt0.Length];

			for (var i = 0; i < pt0.Length; i+=2)
			{
				rotateLinkAnnoCoord(pt0[i], pt0[i+1], out pt1[i], out pt1[i+1], out sw, out sh);
			}

			return pt1;
		}
*/

		/*
		private float checkRotatePage()
		{
			if (pageSize.GetWidth() > pageSize.GetHeight()) return 0;

			return pageRotation;

		}
*/
	}
}