#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

using iText.Forms;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Navigation;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Colorspace;
using Org.BouncyCastle.Asn1.BC;
using System.Net.Mail;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Properties;
using static iText.Kernel.XMP.XMPConst;

using CommonPdfCodeShCode;
using Org.BouncyCastle.Security;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   2/10/2024 6:48:52 PM

/*
 * settings:
 * rectangle for sheet number location
 * page that the TOC resides
 * banner location (inches) from the lower-right corner
 * banner provide y/n
 * banner message / format / data to include
 * footer provide y/n
 * footer message / format / data to include
 * process references in sentences
 * meta data:
 * project name -> title
 * document type -> subject
 * user's name -> author
 * app description -> creator
 *
 * possible app settings
 * banner font
 * banner color
 * banner text size
 * banner weight
 * above - same for footer
 * footer orientation
 * provide hyperlink rectangles & layer
 * hyperlink border color
 * hyperlink border dashed pattern
 * hyperlink border line thickness
 * hyperlink fill color
 * hyperlink fill opacity
 *
 */

namespace SharedCode.ShDataSupport
{
	/*
	public class PdfText5
	{
		private const string BANNER = "This is a banner";
		private const string FOOTER = "This is a custom footer";
		private const string FOOTER_FORMAT = "This set assembled on {0} at {1} by {2} | {3}";

		// settings

		private int tocPage;
		private Color borderColor;
		private Color fillColor;
		private PdfArray rectFillColor;
		private bool normalPgOrientationIsVert;
		private bool processStringText;


		// mist
		private string idxFmt = "{0:D7}";

		private int shtNumMaxLen = 0;
		private int shtNumMinLen = 100;

		private Dictionary<string, TextAndLineSegmentData> shtNumData = new Dictionary<string, TextAndLineSegmentData>();
		public Dictionary<int,  List<TextAndLineSegmentData>> linkText = new Dictionary<int,  List<TextAndLineSegmentData>>();

		private static TextAndLocationXcopier4 talXc;

		private float pi180 = (float) Math.PI;
		private float pi90;
		private float pi270;

		private PdfArray border = new PdfArray(new int[] { 0, 0, 1 });

		private PdfDocument destPdfDoc ;
		private Document destDoc;

		// private Dictionary<SheetBorderType, SheetData> shtData;

		// banner info
		private float bannerXoffset;
		private float bannerYoffset;

		private AffineTransform bannerTrans;
		private Color bannerColor;



		private float footerX;
		private float footerY;

		private Rectangle rectFooter;

		private Style footerStyle;

		private string fontFile = @"c:\windows\fonts\arialn.ttf";
		private PdfFont font;

		private PdfNodeTree tree;
		private List<SampleData> sampleData;
		Dictionary<SheetBorderType, SheetData> shtData;


		// stop watch

		Stopwatch sw = Stopwatch.StartNew();

		private TimeSpan[] el;
		private string[] elt;

		private int START                  ;
		private int MERGEDONE              ;
		private int SHTNUMSDONE            ;
		private int SHTREFSDONE            ;
		private int REFSHTNUMSDONE         ;
		// private int ADDBANNERANDFOOTERDONE ;
		private int ADDLINKSDONE           ;
		private int ADDACTIONSDONE         ;
		private int DESTDOCCLOSEDONE       ;
		private int DESTPDFDOCCLOSEDONE    ;
		private int END                    ;


		public PdfText5(bool normalPgOrientationIsVert)
		{
			this.pi90 = pi180 / 2;
			this.pi270 = pi90 * 3;

			this.normalPgOrientationIsVert=normalPgOrientationIsVert;

			bannerXoffset = 0.8f * 72f - 4;
			bannerYoffset = 0.2f * 72;

			footerX = 3f;
			footerY = 3f;

			tocPage = 1;

			borderColor = new DeviceRgb(0, 192, 255);
			fillColor = new DeviceRgb(75, 214, 254);

			rectFillColor = new PdfArray(new [] { 75 / 255f, 214 / 255f, 1.0f } );

			font = PdfFontFactory.CreateFont(fontFile);

			footerStyle = new Style()
			.SetRotationAngle(pi90)
			.SetFont(font)
			.SetFontColor(DeviceRgb.BLACK)
			.SetFontSize(4);

			setElText();

			processStringText = true;

			bannerTrans = new AffineTransform();
			bannerColor = new DeviceRgb(255, 0, 0);
			
			shtData = SheetConfig.SheetConfigData;

		}

		private void setElText()
		{
			elt = new string[20];
			int idx = 0;

			START                  = idx++;
			MERGEDONE              = idx++;
			SHTNUMSDONE            = idx++;
			SHTREFSDONE            = idx++;
			REFSHTNUMSDONE         = idx++;
			// ADDBANNERANDFOOTERDONE = idx++;
			ADDLINKSDONE           = idx++;
			ADDACTIONSDONE         = idx++;
			DESTDOCCLOSEDONE       = idx++;
			DESTPDFDOCCLOSEDONE    = idx++;
			END                    = idx++;


			elt[START                 ] = "Start";
			elt[MERGEDONE             ] = "after merge";
			elt[SHTNUMSDONE           ] = "after get sht nums";
			elt[SHTREFSDONE           ] = "after got sht refs";
			elt[REFSHTNUMSDONE        ] = "after xref refs";
			// elt[ADDBANNERANDFOOTERDONE] = "after add banner/footers";
			elt[ADDACTIONSDONE        ] = "after add actionss";
			elt[ADDLINKSDONE          ] = "after add links";
			elt[DESTDOCCLOSEDONE      ] = "after dest doc closed";
			elt[DESTPDFDOCCLOSEDONE   ] = "after dest pdf closed";

		}

		public void AddLinks(List<SampleData> sampleData, string dest)
		{
			this.sampleData = sampleData;

			rectFooter = new Rectangle(footerX + 15, footerY, 10, 100);

			el = new TimeSpan[20];

			sw.Start();

			PdfWriter w = (new PdfWriter(dest));

			destPdfDoc = new PdfDocument(w);
			destDoc = new Document(destPdfDoc);

			el[START] = sw.Elapsed;

			// step 1
			merge();
			el[MERGEDONE] = sw.Elapsed;




			//// step 2
			//locateShtNums();
			//showShtNumData();
			//el[SHTNUMSDONE] = sw.Elapsed;

			
			//// step 3
			//locateShtRefs();
			//showShtRefs();
			//el[SHTREFSDONE] = sw.Elapsed;


			//// step 4
			//crossRefWithShtNums();
			//// showShtXRefs();
			//el[REFSHTNUMSDONE] = sw.Elapsed;

			//// addBannerAndFooter();
			//// el[ADDBANNERANDFOOTERDONE] = sw.Elapsed;


			//// step 6
			//addLinksAndAnno();
			//el[ADDLINKSDONE] = sw.Elapsed;

			//// step 7
			//addActions();
			//el[ADDACTIONSDONE] = sw.Elapsed;




			// test();

			destDoc.Close();
			el[DESTDOCCLOSEDONE] = sw.Elapsed;

			destPdfDoc.Close();
			el[DESTPDFDOCCLOSEDONE] = sw.Elapsed;


			el[END] = sw.Elapsed;
			sw.Stop();

			showElapsed();
		}

		// step 1 - merge and find sheet numbers
		private bool merge()
		{
			bool result = true;

			foreach (SampleData sd in sampleData)
			{
				if (sd.Type== PdfTreeItemType.PT_BRANCH) continue;

				int page = 1;

				PdfDocument srcPdfDoc =  new PdfDocument(new PdfReader(sd.String1));

				int pgCount = srcPdfDoc.GetNumberOfPages();

				for (int i = 1; i <= pgCount; i++, page++)
				{
					srcPdfDoc.CopyPagesTo(i, i, destPdfDoc);
				}

				srcPdfDoc.Close();
			}

			return result;
		}

		// start invalid routines
		/*
		private bool merge(PdfNodeTree tree)
		{
			bool result = true;

			merge(tree.Root);

			return result;
		}

		private void merge(APdfTreeNode node)
		{
			if (!node.Bookmark.Equals(PdfNodeTree.ROOTNAME))
			{
				merge(node);
			}

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in node.ItemList)
			{
				if (kvp.Value.ItemType == PdfTreeItemType.PT_BRANCH)
				{
					merge((APdfTreeNode) kvp.Value);
				}
				else
				if (kvp.Value.ItemType == PdfTreeItemType.PT_LEAF)
				{
					int page = 1;

					PdfDocument srcPdfDoc =  new PdfDocument(new PdfReader(((PdfTreeLeaf) kvp.Value).FilePath));
					int pgCount = srcPdfDoc.GetNumberOfPages();

					for (int i = 1; i <= pgCount; i++, page++)
					{
						srcPdfDoc.CopyPagesTo(i, i, destPdfDoc);
					}

					srcPdfDoc.Close();
				}
				else
				{
					throw new InvalidParameterException("Invalid PdfTreeItemType found");
				}
			}
		}
		*/

		// start valid routines
		/*
		// step 2 - get a list of used sheet numbers
		private bool locateShtNums()
		{
			bool result = true;

			destPdfDoc.GetNumberOfPages();
			PdfPage page;

			int pageRotation;


			for (int i = 1; i <= destPdfDoc.GetNumberOfPages(); i++)
			{
				page = destPdfDoc.GetPage(i);

				// showPageData(page);

				pageRotation = page.GetRotation();

				if (!normalPgOrientationIsVert && pageRotation != 270) page.SetRotation(270);


				ShtNumLocationFilter filter = new ShtNumLocationFilter(shtNumLoc, pageRotation, normalPgOrientationIsVert);
				FilteredEventListener shtNumListener = new FilteredEventListener();

				LocationTextExtractionStrategy xStrat =
					shtNumListener.AttachEventListener(new LocationTextExtractionStrategy(), filter);

				new PdfCanvasProcessor(shtNumListener).ProcessPageContent(page);

				TextAndLineSegmentData tals = filter.talData;

				if (tals.IsValid)
				{
					int len = tals.Text.Length;

					if (len > shtNumMaxLen) shtNumMaxLen = len;
					if (len < shtNumMinLen) shtNumMinLen = len;

					tals.OnPageNumber = i;
					tals.ToPageNumber = tocPage;
					shtNumData.Add(tals.Text, tals);

					string idx = string.Format(idxFmt, i);
					shtNumData.Add(idx, tals);
				}
			}

			return result;
		}

		// step 3 - get all of the used sheet references
		private bool locateShtRefs()
		{
			bool result = true;

			destPdfDoc.GetNumberOfPages();
			PdfPage page;

			TextAndLocationXstrategy5 talStrat =
				new TextAndLocationXstrategy5(shtNumData, shtNumLoc, 
					shtNumMinLen, shtNumMaxLen, normalPgOrientationIsVert, processStringText);

			for (int i = 1; i <= destPdfDoc.GetNumberOfPages(); i++)
			{
				page = destPdfDoc.GetPage(i);

				linkText.Add(i, talStrat.GetList(page, i));
			}

			return result;
		}

		// step 4 - cross reference sht refs with the correct page
		private bool crossRefWithShtNums()
		{
			bool result = true;
			int toPage;

			foreach (KeyValuePair<int, List<TextAndLineSegmentData>> kvp in linkText)
			{
				if (kvp.Value.Count == 0) continue;

				for (int i = 0; i < kvp.Value.Count; i++)
				{
					toPage = shtNumData[kvp.Value[i].Text].OnPageNumber;

					if (toPage != -1)
					{
						kvp.Value[i].ToPageNumber = toPage;
					}
				}
			}

			return result;
		}
		*/

		// start invalid routines
		/*
		// step 5 - add banner and footer
		private bool addBannerAndFooter()
		{
			bool results = true;
			Rectangle pageSize;
			Paragraph footer = getFooter(FOOTER);

			for (var i = 1; i <= linkText.Count; i++)
			{
				pageSize = destPdfDoc.GetPage(i).GetPageSize();

				addBanner(BANNER, i, pageSize);

				addFooter(footer, i, pageSize);
			}

			return results;
		}
		*/

		// start valid routines
		/*
		// step 6 - place hyberlinks
		private bool addLinksAndAnno()
		{
			bool result = true;
			int pageNum;
			string rtnIdx;


			Rectangle pageSize;

			PdfLayer layer = new PdfLayer("Hyperlinks Rectangles", destPdfDoc);
			layer.SetOn(true);

			Paragraph footer = getFooter(FOOTER);
			Paragraph disclaimer = getDisclaimer("DISCLAIMER: Click to view disclaimers that apply to this document and any associated documents.");

			// PdfAnnotationBorder border = new PdfAnnotationBorder(0, 0, 3);

			foreach (KeyValuePair<int, List<TextAndLineSegmentData>> kvp in linkText)
			{
				pageNum = kvp.Key;
				rtnIdx = string.Format(idxFmt, pageNum);

				PdfPage page = destPdfDoc.GetPage(kvp.Key);

				pageSize = page.GetPageSize();

				// el[21] = sw.Elapsed;
				// addBanner(BANNER, pageNum, pageSize);
				// el[25] = sw.Elapsed;

				PdfCanvas pdfCanvas = new PdfCanvas(page);
				// Canvas canvas = new Canvas(pdfCanvas, pageSize);

				addBanner(BANNER, pdfCanvas, pageNum, pageSize);

				// el[27] = sw.Elapsed;
				addFooter(footer, pageNum, pageSize);
				// el[29] = sw.Elapsed;

				addDisclaimer(disclaimer, pageNum, pageSize);

				// debug only
				// addAnnoRects(pdfCanvas);

				pdfCanvas.SetFillColorRgb(75 / 255f, 214 / 255f, 1);
				pdfCanvas.SetStrokeColor(borderColor);
				pdfCanvas.SetLineDash(6, 3);
				pdfCanvas.SetLineWidth(1);
				pdfCanvas.SetExtGState(new PdfExtGState().SetFillOpacity(0.4f));

				addDisclaimerLink(page, pdfCanvas, layer, sd.DisclaimerRect);

				addAuthorLink(page, pdfCanvas, layer, sd.PrimaryAuthorRect);

				addReturnLink(page, pdfCanvas, layer, pageNum, shtNumData[rtnIdx]);

				// el[33] = sw.Elapsed;

				if (kvp.Value.Count != 0)
				{
					pdfCanvas.BeginLayer(layer);

					addPageLinks(page, pdfCanvas, kvp.Value, pageNum);

					pdfCanvas.EndLayer();
				}

				pdfCanvas.Release();

				// el[37] = sw.Elapsed;
				page.Flush();
				// el[39] = sw.Elapsed;
			}

			return result;
		}

		// step 7 - add actions
		private bool addActions()
		{
			bool result = true;

			destPdfDoc.GetCatalog().SetOpenAction(
				PdfExplicitDestination.CreateFit(destPdfDoc.GetFirstPage()));

			// this causes a dialog to be displayed when the document is opened
			// destPdfDoc.GetCatalog().SetOpenAction(
			// 	PdfAction.CreateJavaScript("app.alert('Thanks for using Josh')"));


			PdfDocumentInfo info = destPdfDoc.GetDocumentInfo();

			info.SetTitle("Project: SBG");
			info.SetAuthor(ReadPDFText.userName);
			info.SetSubject("Progress Set");
			info.SetProducer("Josh by CyberStudio");

			info.SetMoreInfo("Disclaimer","https://www.aoarchitects.com/legal-disclaimer/");

			//https://www.aoarchitects.com/legal-disclaimer/

			return result;
		}
		*/


		// start valid routines
		/*
		private void addPageLinks(PdfPage page, PdfCanvas pdfCanvas, List<TextAndLineSegmentData> tals, int pageNum)
		{
			Rectangle r;

			foreach (TextAndLineSegmentData ls in tals)
			{
				r = ls.GetRectangle(2);

				if (ls.ToPageNumber == pageNum) continue;

				page.AddAnnotation(makeLinkAnnotation(r, ls));

				addLinkRectangle(pdfCanvas, ls);

				pdfCanvas.FillStroke();
			}
		}

		private void addLinkRectangle(PdfCanvas pdfCanvas, TextAndLineSegmentData ls)
		{
			if (ls.IsRect)
			{
				pdfCanvas.Rectangle(ls.GetRectangle(2));
			}
			else
			{
				double[] a = ls.GetRectPath();

				pdfCanvas.MoveTo(a[0], a[1]);

				for (int i = 2; i < a.Length; i += 2)
				{
					pdfCanvas.LineTo(a[i], a[i + 1]);
				}

				pdfCanvas.ClosePath();
			}
		}

		private void addReturnLink(PdfPage page, PdfCanvas canvas, PdfLayer layer, int pageNum, TextAndLineSegmentData shtNumdata)
		{
			if (pageNum == tocPage) return;

			// PdfAnnotation pa = new PdfLinkAnnotation(shtNumLoc)
			// .SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT)
			// .SetAction(PdfAction.CreateGoTo(
			// 		PdfExplicitDestination.CreateFit(destPdfDoc.GetPage(tocPage))))
			// .SetFlag(PdfAnnotation.LOCKED);

			// PdfAnnotation rect = new PdfSquareAnnotation(shtNumLoc)
			// .SetBorderStyle(PdfAnnotation.STYLE_DASHED)
			// .SetBorder(border)
			// .SetFlag(PdfAnnotation.LOCKED);
			//
			// page.AddAnnotation(rect);
			// rect.SetLayer(layer);

			page.AddAnnotation(makeLinkAnnotation(shtNumLoc, shtNumdata));

			canvas.BeginLayer(layer);
			canvas.Rectangle(shtNumLoc);
			canvas.FillStroke();
			canvas.EndLayer();
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
		*/

		// start invalid routines
		/*
		private void addBanner2(string banner, PdfCanvas pdfCanvas, Rectangle pageSize, int pageNum)
		{
			// this method appears to be no faster (maybe a bit slower)
			// use the original method

			Text text = getBanner(pageNum, banner);

			float x;
			float y;
			float rotation;

			getBannerLocation(pageSize, out x, out y, out rotation);

			AffineTransform aft = new AffineTransform();
			aft.Translate(x,y);
			aft.Rotate(pi90);


			pdfCanvas
			.SaveState()
			.BeginText()
			.SetFillColor(bannerColor)
			.SetFontAndSize(font, 24)
			// .MoveText(72, 72)
			.SetTextMatrix(aft)
			.ShowText(text.GetText())

			.EndText()
			.RestoreState();
		}
		*/

		// start valid routines
		/*
		private void addBanner(string banner, PdfCanvas pdfCanvas, int pageNum, Rectangle pageSize)
		{
			Paragraph b = getBanner(banner, pageNum);

			float x = sd.BannerRect.GetX();
			float y = sd.BannerRect.GetY();
			float w = sd.BannerRect.GetWidth();
			float h = sd.BannerRect.GetHeight();
			float rx = x;
			float ry = y;

			float rotation = pi270;
			TextAlignment ta = TextAlignment.RIGHT;

			if (sd.BannerLocation == BannerLocations.BL_BR_VERTICAL)
			{
				rotation = 0;
				ta = TextAlignment.LEFT;
				y += 4;
				x += 4;
			} 
			else if (sd.BannerLocation == BannerLocations.BL_TR_VERTICAL)
			{
				rotation = 0;
				x += (w - 4);
				y += 4;
			}
			else if (sd.BannerLocation == BannerLocations.BL_BR_HORIZONTAL)
			{
				y += 4;
				x += 4;
			}
			else if (sd.BannerLocation == BannerLocations.BL_TR_HORIZONTAL)
			{
				y += 4;
				x += 4;
			}

			//
			// getBannerLocation(pageSize, out x, out y, out rotation);
			//
			// float w = pageSize.GetHeight() - x * 2;
			// float h = 26;
			
			Rectangle r = new Rectangle(rx, ry, w, h);
			// addRectangle(pdfCanvas, r, DeviceRgb.WHITE, DeviceRgb.WHITE);
			// addRectangle(pdfCanvas, r, DeviceRgb.BLUE, DeviceRgb.WHITE);
			//
			// Console.WriteLine($"add banner here| x| {x,10:F4} | y| {y,10:F4} | r| {rotation:F4}");
			// Console.WriteLine($"add rect here  | x| {x,10:F4} | y| {y,10:F4} | w {w,10:F4} | h| {h:F4}");


			destDoc.ShowTextAligned(b, x, y, pageNum, ta, VerticalAlignment.BOTTOM, rotation);
			// destDoc.ShowTextAligned(b, 72, 72, pageNum, TextAlignment.LEFT, VerticalAlignment.BOTTOM, pi90);
		}

		private void addFooter(Paragraph footer, int pageNum, Rectangle pageSize)
		{
			float rotation = pi270;

			/*
			float x = footerX;
			float y = footerY;

			if (!normalPgOrientationIsVert)
			{
				rotation = pi270;
				x = footerY;
				y = pageSize.GetHeight() - footerX;
			}
			#1#

			float x = sd.FooterRect.GetX()+2f;
			float y = sd.FooterRect.GetY() + sd.FooterRect.GetHeight();

			// Console.WriteLine($"add footer here| x| {x,10:F4} | y| {y,10:F4} | r| {rotation}");

			destDoc.ShowTextAligned(footer, x, y, pageNum, TextAlignment.LEFT, VerticalAlignment.TOP, rotation);
		}

		private void addDisclaimer(Paragraph disclaimer, int pageNum, Rectangle pageSize) 
		{
			float rotation = 0;

			float x = sd.DisclaimerRect.GetX()+sd.DisclaimerRect.GetWidth();
			float y = sd.DisclaimerRect.GetY()+4f;

			// Console.WriteLine($"add footer here| x| {x,10:F4} | y| {y,10:F4} | r| {rotation}");

			destDoc.ShowTextAligned(disclaimer, x, y, pageNum, TextAlignment.RIGHT, VerticalAlignment.BOTTOM, rotation);
		}

		private void addDisclaimerLink(PdfPage page, PdfCanvas canvas, PdfLayer layer, Rectangle r)
		{
			PdfAnnotation anno = makeUrlLinkAnnotation(r, "https://www.aoarchitects.com/legal-disclaimer/");

			addUrlLinkOnLayer(page, canvas, layer, r, anno);
		}

		private void addAuthorLink(PdfPage page, PdfCanvas canvas, PdfLayer layer, Rectangle r)
		{
			PdfAnnotation anno = makeUrlLinkAnnotation(r, "https://www.aoarchitects.com/");

			addUrlLinkOnLayer(page, canvas, layer, r, anno);
		}

		private void addUrlLinkOnLayer(PdfPage page, PdfCanvas canvas, PdfLayer layer, Rectangle r, PdfAnnotation anno)
		{
			page.AddAnnotation(anno);

			canvas.BeginLayer(layer);
			canvas.Rectangle(r);
			canvas.FillStroke();
			canvas.EndLayer();
		}

		private PdfAnnotation makeUrlLinkAnnotation(Rectangle r, string url)
		{
			PdfLinkAnnotation pa = new PdfLinkAnnotation(r);

			pa.SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT)
			.SetAction(PdfAction.CreateURI(url))
			.SetFlag(PdfAnnotation.LOCKED);

			return pa;
		}
		*/

		// start invalid routines
		/*
		private void test() //FileStream fs)
		{

			// PdfDictionary catalog = destPdfDoc.GetTrailer();
			// PdfDictionary map = catalog.GetAsDictionary(PdfName.Info);

			// Stream os = destPdfDoc.GetWriter().GetOutputStream();
			//
			// PdfStream s = new PdfStream(destPdfDoc.GetXmpMetadata());

			// XMPMeta a = XMPMetaFactory.Parse(fs);
			// XMPMeta a = XMPMetaFactory.Create();
			//
			// a.SetProperty(NS_XMP_RIGHTS, "owner", "AO Architects");
			// a.SetProperty(NS_XMP_RIGHTS, "WebStatement", "https://www.aoarchitects.com/legal-disclaimer/");
			//
			//
			// XMPProperty p1 = a.GetProperty(NS_XMP_RIGHTS, "owner");
			// XMPProperty p2 = a.GetProperty(NS_XMP_RIGHTS, "WebStatement");
			//
			// destPdfDoc.SetXmpMetadata(a);

			int x = 1;

		}
		*/

		// start valid routines
		/*
		private void addAnnoRects(PdfCanvas canvas)
		{
			addRectangle(canvas, sd.TitleBlockRect, DeviceRgb.WHITE, DeviceRgb.GREEN, 0.3f);
			addRectangle(canvas, sd.BannerRect, DeviceRgb.BLUE, DeviceRgb.BLUE, 0.3f);
			addRectangle(canvas, sd.SheetNumberLinkRect, DeviceCmyk.MAGENTA, DeviceCmyk.MAGENTA, 0.3f);
			addRectangle(canvas, sd.SheetNumberFindRect, DeviceRgb.GREEN, DeviceRgb.GREEN, 0.3f);
			addRectangle(canvas, sd.ContentRect, DeviceCmyk.MAGENTA, DeviceCmyk.MAGENTA, 0.1f);
			addRectangle(canvas, sd.FooterRect, DeviceRgb.RED, DeviceRgb.RED, 0.2f);
			addRectangle(canvas, sd.DisclaimerRect, DeviceRgb.RED, DeviceRgb.RED, 0.2f);
			addRectangle(canvas, sd.PrimaryAuthorRect, DeviceRgb.GREEN, DeviceRgb.GREEN, 0.3f);
		}
		*/
		

		/*	private void addFooter2(string footer, Canvas canvas)
		{
			Paragraph p = new Paragraph(footer)
			.AddStyle(footerStyle)
			.SetFixedPosition(footerX + 15, footerY, 10);
			canvas.Add(p);

		}
		*/

		/*
		private PdfAnnotation makeLinkAnnotation(Rectangle r, TextAndLineSegmentData ls)
		{
			// if (ls.Text.Equals("A1.2-0d"))
			// {
			// 	showSegData(ls);
			// }

			PdfLinkAnnotation pa;

			if (ls.IsRect)
			{
				pa = new PdfLinkAnnotation(r);
			}
			else
			{
				pa = makePolyLinkAnnotation(ls);
			}

			pa.SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT)
			.SetAction(PdfAction.CreateGoTo(
					PdfExplicitDestination.CreateFit(destPdfDoc.GetPage(ls.ToPageNumber))))
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

		// helper routines
		private Paragraph getBanner(string banner, int pageNum)
		{
			Text text = getBanner(pageNum, banner);

			Paragraph pg1 = new Paragraph(text)
			.SetMargin(0f)
			.SetMultipliedLeading(1)
			.SetFontColor(new DeviceRgb(255, 0, 0))
			.SetFontSize(18f);

			return pg1;
		}

		private Text getBanner(int pageNum, string banner)
		{
			return new Text($"{pageNum} of {linkText.Count} pages | {banner}");
		}

		private Paragraph getFooter(string footer)
		{
			string date = $"{DateTime.Now:D}";
			string time = $"{DateTime.Now:t}";
			string me = ReadPDFText.UserName;

			Text text = new Text(string.Format(FOOTER_FORMAT, date, time, me, FOOTER));

			Paragraph pg1 = new Paragraph(text)
			.SetMargin(0f)
			.SetMultipliedLeading(0)
			.SetFontColor(DeviceRgb.BLACK)
			.SetFontSize(3f);

			return pg1;
		}

		private Paragraph getDisclaimer(string disclaimer)
		{
			Text text = new Text(disclaimer);

			Paragraph pg1 = new Paragraph(text)
			.SetMargin(0f)
			.SetMultipliedLeading(0)
			.SetFontColor(DeviceRgb.BLACK)
			.SetFontSize(8f);

			return pg1;
		}



		private void getBannerLocation(Rectangle pageSize, 
			out float x, out float y, out float rotation)
		{
			y = x = 0;
			rotation = 0;

			if (normalPgOrientationIsVert)
			{
				x = pageSize.GetWidth() - bannerXoffset;
				y = bannerYoffset;
				rotation = pi90;
			}
			else
			{
				x = bannerXoffset;
				y = bannerYoffset;
				

			}
		}

		// debug routines
		private void showShtNumData()
		{
			if (shtNumData == null || shtNumData.Count == 0)
			{
				Console.WriteLine("no sheet numbers found");
				return;
			}

			foreach (KeyValuePair<string, TextAndLineSegmentData> kvp in shtNumData)
			{
				Console.WriteLine($"key | {kvp.Key,-8} | text| {kvp.Value.Text,-8}| page number| {kvp.Value.OnPageNumber}");
			}
		}

		private void showShtRefs()
		{
			Console.WriteLine($"\n*** Sheet references found ***");

			foreach (KeyValuePair<int,  List<TextAndLineSegmentData>> kvp in linkText)
			{
				Console.WriteLine($"page {kvp.Key}| has {kvp.Value.Count} items");

				foreach (TextAndLineSegmentData tals in kvp.Value)
				{
					Console.WriteLine($"{tals.Text}"); // on page {tals.OnPageNumber} and to page {tals.ToPageNumber}");
				}
			}
		}

		private void showShtXRefs()
		{
			Console.WriteLine($"\n*** Sheet references found ***");

			foreach (KeyValuePair<int,  List<TextAndLineSegmentData>> kvp in linkText)
			{
				Console.WriteLine($"page {kvp.Key}| has {kvp.Value.Count} items");

				foreach (TextAndLineSegmentData tals in kvp.Value)
				{
					Console.WriteLine($"{tals.Text} on page {tals.OnPageNumber} and to page {tals.ToPageNumber}");
				}
			}
		}

		private void showSegData(TextAndLineSegmentData tals)
		{
			tals.SetOverAll();

			Console.WriteLine($"\n{tals.Text}");
			Console.WriteLine($"   angle | {tals.GetAngle()}");
			Console.WriteLine($"top line | {tals.TSV.Get(0)}, {tals.TSV.Get(1)}  to  {tals.TEV.Get(0)}, {tals.TEV.Get(1)}");
			Console.WriteLine($"bott line| {tals.BSV.Get(0)}, {tals.BSV.Get(1)}  to  {tals.BEV.Get(0)}, {tals.BEV.Get(1)}");
			Console.WriteLine($"t corners| [2] | {tals.Corners[2].x}, {tals.Corners[2].y} | [3] | {tals.Corners[3].x}, {tals.Corners[3].y}");
			Console.WriteLine($"b corners| [0] | {tals.Corners[0].x}, {tals.Corners[0].y} | [1] | {tals.Corners[1].x}, {tals.Corners[1].y}");
			Console.WriteLine($"   min x | {tals.MinX}  | min y | {tals.MinY}");
			Console.WriteLine($"       W | {tals.Width}  |    H | {tals.Height}");
			Console.WriteLine($"    run1 | {tals.run1} | run2 | {tals.run2} | rise1 | {tals.rise1} | rise2 | {tals.rise2}");
			Console.WriteLine($"    OA W | {tals.OAwidth} | OA H | {tals.OAheight}");
		}

		private void showPageData(PdfPage page)
		{
			Console.WriteLine($"page info");
			Console.WriteLine($"     rotation| {page.GetRotation()}");
			Console.WriteLine($"  pg sz w/rot| {fmtRect(page.GetPageSizeWithRotation())}");
			Console.WriteLine($"        pg sz| {fmtRect(page.GetPageSize())}");
			Console.WriteLine($"    media box| {fmtRect(page.GetMediaBox())}");
			Console.WriteLine($"     crop box| {fmtRect(page.GetCropBox())}");
			Console.WriteLine($"      art box| {fmtRect(page.GetArtBox())}");
			Console.Write("\n");
		}

		public static string fmtRect(Rectangle r)
		{
			return $"{r.GetX():F2} x {r.GetY():F2} | w| {r.GetWidth():F4} | h| {r.GetHeight():F4}";
		}

		private void showElapsed()
		{

			Console.WriteLine($"{elt[0],-30} | {el[0]}");

			double total = 0;

			for (int i = 1; i < END; i++)
			{
				double diff = el[i].TotalMilliseconds - el[i - 1].TotalMilliseconds;

				total += diff;

				Console.Write($"{elt[i],-25} | diff| {diff,11:F4}");

				// Console.Write($" | {elt[i],-25} ({el[i].TotalMilliseconds,10:F4})  |  {elt[i-1],-25} ({el[i-1].TotalMilliseconds,10:F4})");

				Console.Write("\n");

			}

			Console.WriteLine($"total millisecs| {el[END].TotalMilliseconds}");
			Console.WriteLine($"total      secs| {el[END].TotalSeconds}");
			Console.WriteLine($"total total    | {total}");

		}

	}

	*/

	/*
	/// <summary>
	/// find text (sht numbers) with in the provided rectangle
	/// </summary>
	public class ShtNumLocationFilter : TextRegionEventFilter
	{
		private Rectangle shtNumLoc;

		public TextAndLineSegmentData talData { get; private set; }

		private int pageRotation;
		private bool normalOrientationIsVertical { get; set; }

		public ShtNumLocationFilter(Rectangle shtNumLocation, int pageRotation, 
			bool normalOrientationIsVertical) : base(shtNumLocation)
		{
			this.pageRotation= pageRotation;
			this.normalOrientationIsVertical = normalOrientationIsVertical;
			shtNumLoc = shtNumLocation;
			talData = TextAndLineSegmentData.Invalid();
		}

		public override bool Accept(IEventData data, EventType type)
		{
			if (type.Equals(EventType.RENDER_TEXT))
			{
				TextRenderInfo ri = data as TextRenderInfo;

				string text = ri.GetText();

				if (text.Length < 6 || text.Length > 8) return false;
				
				// Rectangle r;

				// LineSegment top =  ri.GetAscentLine().TransformBy(ri.GetTextMatrix());

				// Console.WriteLine($"found| {ri.GetText()}");
				// need to use ri.gettextmstrix() with page is rotated

				// Rectangle r = TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine());


				// if (ri.GetText().Substring(0,4).Equals("A2.2"))
				// {
				//
				// 	r = TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine(), normalOrientationIsVertical);
				// 	
				// 	showInfo(ri, pageRotation, r);
				// }

				

				// if (shtNumLoc.Contains(TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine(), normalOrientationIsVertical)))
				if (shtNumLoc.Contains(TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine())))
				{
					talData = new TextAndLineSegmentData(ri.GetText(), ri.GetDescentLine(), ri.GetAscentLine(), normalOrientationIsVertical);
					return true;
				}
			}

			return false;
		}

		private void showInfo(TextRenderInfo ri, int pageRotation, Rectangle r)
		{
			TextAndLineSegmentData tals = new TextAndLineSegmentData(ri.GetText(), ri.GetAscentLine(), ri.GetDescentLine(), normalOrientationIsVertical);

			Console.WriteLine($"{tals.Text}");

			Console.WriteLine($"    test rect | {PdfText5.fmtRect(shtNumLoc)}");
			Console.WriteLine($"     act rect | {PdfText5.fmtRect(r)}\n");
			
			Console.WriteLine($"       pg rot | {pageRotation}");
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

	/*
	/// <summary>
	/// provides extra page coping logic.  this runs the text
	/// extractor and places the text into a list
	/// </summary>
	public class TextAndLocationXcopier5 : IPdfPageExtraCopier
	{
		private static TextAndLocationXstrategy5 tals;
		private List<string> toFind;
		private Rectangle sheetNumberArea;

		public List<TextAndLineSegmentData> LinkText { get; private set; }


		public TextAndLocationXcopier5(List<string> toFindList, Rectangle shtNumArea)
		{
			toFind = toFindList;
			sheetNumberArea = shtNumArea;
			tals = new TextAndLocationXstrategy5(toFind, sheetNumberArea);
		}


		public void Copy(PdfPage fromPage, PdfPage toPage)
		{
			LinkText = new List<TextAndLineSegmentData>();

			LinkText = tals.GetList(toPage);
		}
	}
	*/

	/*
	/// <summary>
	/// finds all text per page.  adds the found text along with
	/// its position information to a list for each string that
	/// matches the list of strings to find
	/// </summary>
	// class TextAndLocationXstrategy5 : PdfTextAndLocationStrategy
	class TextAndLocationXstrategy5 : SimpleTextExtractionStrategy
	{
		private  Dictionary<string, TextAndLineSegmentData> toFind;
		private Rectangle sheetNumberArea;

		private int onPage;

		private int minLen;
		private int maxLen;

		public List<TextAndLineSegmentData> list;
		public List<int> ignoreList;

		private bool useIgnoreList;
		private bool processStrings;
		private bool normalOrientationIsVertical;

		public TextAndLocationXstrategy5(
			Dictionary<string, TextAndLineSegmentData> toFindList,
			Rectangle shtNumArea,
			int minLen, int maxLen,
			bool normalOrientationIsVertical,
			bool processStrings = false,
			bool useIgnoreList = false,
			List<int> ignoreList = null)
		{
			toFind = toFindList;
			sheetNumberArea = shtNumArea;

			this.useIgnoreList = useIgnoreList;
			this.ignoreList = ignoreList;

			this.minLen = minLen;
			this.maxLen = maxLen;

			this.processStrings = processStrings;

			this.normalOrientationIsVertical = normalOrientationIsVertical;
		}

		public List<TextAndLineSegmentData> GetList(PdfPage page, int currPage)
		{
			list = new List<TextAndLineSegmentData>();

			if (useIgnoreList &&  ignoreList != null && ignoreList.Contains(currPage))
			{
				return list;
			}

			onPage = currPage;

			PdfTextExtractor.GetTextFromPage(page, this);

			return this.list;
		}

		public override void EventOccurred(IEventData data, EventType type)
		{
			if (!type.Equals(EventType.RENDER_TEXT)) return;

			TextRenderInfo ri = (TextRenderInfo) data;

			Rectangle r;

			string text = ri.GetText();

			// Console.WriteLine($"\nvalidating| {text}");

			int result = validateText(text);

			// Console.Write($"validation results| {result} | ");

			if (result == 1)
			{
				// Console.Write("found simple");

				// r = TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine(), normalOrientationIsVertical);
				r = TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine());

				if (!sheetNumberArea.Contains(r))
				{
					// Console.WriteLine($"1 adding| {text}");
					list.Add(new TextAndLineSegmentData(text, ri.GetAscentLine(), ri.GetDescentLine(), normalOrientationIsVertical, onPage));
				}
			}
			else if (result == -1)
			{
				// Console.Write("found complex | ");
				// == -1 - long string

				List<string> matches;

				if (validateTextString(text, out matches))
				{
					// Console.Write($"found {matches.Count} matches | ");

					TextAndLineSegmentData reference;

					foreach (string match in matches)
					{
						// Console.Write("m");

						int len = match.Length;

						int start = text.IndexOf(match);

						while (start >= 0)
						{
							// Console.Write(".");

							reference = getRefFromString(match, start, ri);

							Console.WriteLine($"2 adding| {text}");
							list.Add(reference);

							start = text.IndexOf(match, start + len);
						}
					}
				}
				// else
				// {
				// 	Console.Write("but no matches");
				// }
			}
			// else
			// {
			// 	Console.Write($"ignore all");
			// }

			// Console.Write("\n");

			base.EventOccurred(data, type);
		}

		private int validateText(string findString)
		{
			if (toFind.ContainsKey(findString) ) return 1;

			int len = findString.Length;
			// string findString = testString.Trim();

			// 2 means - ignore
			if (len < minLen) return 2;

			// -1 means - process the string
			if (processStrings && len > minLen) return -1;

			return  0;
		}

		private bool validateTextString2(string text, out List<string> matches)
		{
			string[] x = text.Split(new char[] { ' ' });


			matches = new List<string>();

			foreach (KeyValuePair<string, TextAndLineSegmentData> kvp in toFind)
			{
				if (text.Contains(kvp.Key))
				{
					matches.Add(kvp.Key);
				}
			}

			return matches.Count > 0;
		}


		private bool validateTextString(string text, out List<string> matches)
		{
			matches = new List<string>();

			string[] words = text.Split(new char[] { ' ', '/' });
			int len;
			bool result;

			foreach (string word in words)
			{
				// Console.Write($" w| >{word}<");

				len = word.Length;
				if (len > maxLen || len < minLen) continue;

				if (toFind.ContainsKey(word))
				{
					matches.Add(word);
				}
			}

			return matches.Count > 0;
		}

		private TextAndLineSegmentData getRefFromString(string found, int startIdx,
			TextRenderInfo ri)
		{
			int endIdx = startIdx + found.Length - 1;

			IList<TextRenderInfo> ris = ri.GetCharacterRenderInfos();

			LineSegment top = new LineSegment(ris[startIdx].GetAscentLine().GetStartPoint(),
				ris[endIdx].GetAscentLine().GetEndPoint());

			LineSegment bott = new LineSegment(ris[startIdx].GetDescentLine().GetStartPoint(),
				ris[endIdx].GetDescentLine().GetEndPoint());

			return new TextAndLineSegmentData(found, top, bott, normalOrientationIsVertical);
		}
	}
	*/

	/*
	class SheetNumberLocStrat : PdfTextAndLocationStrategy
	{
		public List<TextAndLineSegmentData> list;
		private Rectangle shtNumRect;

		public SheetNumberLocStrat(Rectangle shtNumRect)
		{
			this.shtNumRect= shtNumRect;
		}

		public List<TextAndLineSegmentData> GetShtNums(PdfPage page)
		{
			list= new List<TextAndLineSegmentData>();

			PdfTextExtractor.GetTextFromPage(page, this);

			return list;
		}

		public override void EventOccurred(IEventData data, EventType type)
		{
			TextAndLineSegmentData tls;

			if (!type.Equals(EventType.RENDER_TEXT)) return;

			TextRenderInfo ri = (TextRenderInfo) data;

			tls = new TextAndLineSegmentData(null, ri.GetAscentLine(), ri.GetDescentLine());

			if (shtNumRect.Contains(tls.GetRectangle()))
			{
				tls.Text = ri.GetText();
			}
		}

	}
	*/


}