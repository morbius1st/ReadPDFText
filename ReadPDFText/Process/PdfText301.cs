#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Drawing.Color;
using CommonPdfCodeShCode;
using iText.Kernel.Colors;
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

#endregion

// user name: jeffs
// created:   4/24/2024 8:56:34 PM

namespace ReadPDFText.Process
{
	public class PdfText301
	{
		public const string PAGES_PG_NUM_FORMAT = "*{0:D5}";

		private const float LINK_BOX_W = 250;
		private const float LINK_BOX_H = 50;



		private Dictionary<SheetBorderType, SheetData> shtData;

		private PdfDocument destPdfDoc ;

		private Document destDoc;

		private PdfCanvasProcessor parser;

		private Dictionary<string, PdfTreeLeaf> pages;


		private int pageNum;

		private string disclaimer = "this is a disclaimer";
		private string footer = " this is a footer";
		private string bannerTh = "banner - top, horizontal";
		private string bannerTv = "banner - top, vertical";
		private string bannerBh = "banner - bottom, horizontal";
		private string bannerBv = "banner - bottom, vertical";


		private Rectangle pageSize;
		private Rectangle pageSizeWithRotation;
		private float pageRotation;

		private Rectangle[] rectBoxes;
		private annoInfo[,] annoInfo;
		private annoInfo[,] linkInfo;

		private annoInfo[,] testInfo;

		private annoInfo origin;

		private DeviceRgb[,] rgbInfo2;

		private PdfPage pdfPage;

		private annoInfo currAnnoInfo;

		//                    page 1 gets   v  v  v  v
		//                    page 2 gets      v  v  v  v
		private int[] pageRefs = new [] {1, 2, 3, 4, 5, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 };


		public PdfText301()
		{
			config();
		}

		private void config()
		{
			// shtData = ShtData;

			makeBoxes();
		}

		private void closeDocs()
		{
			destDoc.Close();

			destPdfDoc.Close();

		}

		public void Process(PdfNodeTree tree, string dest)
		{
			WriterProperties wp = new WriterProperties();
			wp.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
			wp.UseSmartMode();
			wp.SetFullCompressionMode(true);

			PdfWriter w = new PdfWriter(dest, wp);

			destPdfDoc = new PdfDocument(w);
			destDoc = new Document(destPdfDoc);

			Debug.WriteLine("merge start| ");

			merge(tree);

			validate();

			addAnnoAndLinks();

			closeDocs();
		}

	#region merge

		private void merge(PdfNodeTree tree)
		{			
			
			pageNum = 1;

			pages = new Dictionary<string, PdfTreeLeaf>();

			merge(tree.Root);
		}

		private void merge(PdfTreeBranch branchNode)
		{
			bool result = true;
			int numPages;
			PdfDocument srcPdfDoc = null;

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in branchNode.ItemList)
			{
				// if (SHOW_DEBUG_INFO) Debug.WriteLine($"merging| {kvp.Value.Bookmark}");

				Debug.Write(".");

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
					// pages.Add($"*{pageNum++:D5}", leaf);
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

		private void validate()
		{
			string result;

			Debug.WriteLine("validate| getting sheet numbers");
			
			for (int i = 1; i <= destPdfDoc.GetNumberOfPages(); i++)
			{
				pdfPage = destPdfDoc.GetPage(i);

				rotatePage();

				pageSize = pdfPage.GetPageSize();
				pageSizeWithRotation = pdfPage.GetPageSizeWithRotation();
				pageRotation = pdfPage.GetRotation();

				pdfPage.SetIgnorePageRotationForContent(true);

				Rectangle r = rectBoxes[0];

				result = getSheetNumberData(r);
				
				Debug.WriteLine($"sht number| >{result}<");

			}
		}

		private string getSheetNumberData(Rectangle r)
		{
			// TextAndLineSegmentData result = null;
			Rectangle rr = rotateRectangle(r);

			TextRegionEventFilter filter = new TextRegionEventFilter(rr);
			FilteredEventListener listener = new FilteredEventListener();

			LocationTextExtractionStrategy strat =
				listener.AttachEventListener(new LocationTextExtractionStrategy(), filter);

			if (parser != null) parser.Reset();

			parser = new PdfCanvasProcessor(listener);

			parser.ProcessPageContent(pdfPage);

			string result = strat.GetResultantText();

			return result;
		}

		

	#endregion

	#region anno and links

		private void addAnnoAndLinks()
		{
			Debug.WriteLine("add annotations and links");

			foreach (KeyValuePair<string, PdfTreeLeaf> kvp in pages)
			{
				if (kvp.Key[0]=='*') continue;
				
				PdfTreeLeaf leaf = kvp.Value;

				Debug.WriteLine($"\ntesting| {kvp.Key} - {kvp.Value.SheetName}");

				pageNum = leaf.PageNumber;

				addAnno(leaf);
			}
		}

		private void addAnno(PdfTreeLeaf leaf)
		{
			pdfPage = destPdfDoc.GetPage(pageNum);

			// pageRotation = pdfPage.GetRotation();
			// pageSizeWithRotation = pdfPage.GetPageSizeWithRotation();
			//
			// if (rotatePage())
			// {
			// 	Debug.WriteLine("rotating page");
			// 	pdfPage.SetRotation(-90);
			// }

			pageRotation = pdfPage.GetRotation();
			pageSize = pdfPage.GetPageSize();
			pageSizeWithRotation = pdfPage.GetPageSizeWithRotation();
			//
			// pdfPage.SetIgnorePageRotationForContent(true);

			PdfCanvas canvas = new PdfCanvas(pdfPage);

			placePageRects(leaf, pageNum, canvas);

			addPageLinks(canvas, null);

		}

		private bool doRotate = true;

		private void addPageLinks(PdfCanvas canvas, PdfLayer layer)
		{
			annoInfo ai;


			for (var i = 0; i < linkInfo.GetLength(1); i++)
			{
				ai = linkInfo[0, i];
				currAnnoInfo = ai;

				placePageLink(canvas, layer, rotateRectangle(currAnnoInfo.rect), pageRefs[pageNum + i]);

				placeRectangle(canvas, currAnnoInfo.rect);

				placeText($"on page {pageNum}| name {currAnnoInfo.text} to page {pageRefs[pageNum + i]}",
					pageNum, currAnnoInfo.rect,-10f,-10f);
			}

			doRotate = false;

			currAnnoInfo = origin;

			placePageLink(canvas, layer, rotateRectangle(currAnnoInfo.rect), pageRefs[5]);
			
			doRotate = true;

			placeRectangle(canvas, currAnnoInfo.rect);

		}

		private void placePageLink(PdfCanvas canvas, PdfLayer layer, Rectangle r, int toPage)
		{
			PdfAnnotation pa = makePageLinkAnnotation(r, toPage);

			pdfPage.AddAnnotation(pa);
		}

		private void placeRectangle(PdfCanvas canvas, Rectangle r)
		{
			placeRectangle(currAnnoInfo.text, pageNum, canvas, r, 
				currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill, 0.30f);
		}

		private PdfAnnotation makePageLinkAnnotation(Rectangle r, int toPage)
		{
			PdfLinkAnnotation pa;

			string text = $"{currAnnoInfo.text} to page {toPage}";

			pa = new PdfLinkAnnotation(r)
			.SetAction(PdfAction.CreateGoTo(
					PdfExplicitDestination.CreateFit(
						destPdfDoc.GetPage(toPage))))
			.SetHighlightMode(PdfAnnotation.HIGHLIGHT_OUTLINE);
			;

			pa.SetTitle(new PdfString(text))
			.SetContents(text)
			.SetContents(text)
			.SetFlag(PdfAnnotation.LOCKED)
			.SetColor(rgbInfo2[7, 0]);

			return pa;

		}
		
		// place elements on the page
		private void placePageRects(PdfTreeLeaf leaf, int pageNum, PdfCanvas canvas)
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
				placeTestRect(currAnnoInfo.text, pageNum, canvas, currAnnoInfo.rect, currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill);
			}

			for (var i = 0; i < testInfo.GetLength(1); i++)
			{
				currAnnoInfo = testInfo[0, i];
				placeTestRect(currAnnoInfo.text, pageNum, canvas, currAnnoInfo.rect, currAnnoInfo.rgbBorder, currAnnoInfo.rgbFill);
			}

			// for (var i = 0; i < annoInfo.GetLength(1); i++)
			// {
			// 	ai = annoInfo[0, i];
			// 	currAnnoInfo = ai;
			// 	placeTestRect(ai.text, pageNum, canvas, ai.rect, ai.rgbBorder, ai.rgbFill);
			// }
		}

		private void placeTestRect(string name, int pageNum, PdfCanvas canvas, Rectangle r, 
			DeviceRgb borderRgb, DeviceRgb fillRgb)
		{
			if (r == null) return;

			placeRectangle(name, pageNum, canvas, r, borderRgb, fillRgb, 0.3f);
		}

		private void placeRectangle(string name, int pageNum, PdfCanvas canvas, 
			Rectangle r, Color strokeColor, Color fillColor,
			float fillOpacity = 1)
		{

			// Debug.WriteLine($"place rect| {name,-35}| {fmtRect(r,1)}");

			canvas.SaveState();


			PdfExtGState gs = new PdfExtGState().SetFillOpacity(fillOpacity);

			canvas.Rectangle(r)
			.SetExtGState(gs)
			.SetLineWidth(0)
			.SetStrokeColor(strokeColor)
			.SetFillColor(fillColor)
			.FillStroke();

			canvas.RestoreState();

			placeCircle(canvas, r.GetX(), r.GetY(), 3);

			placeText(name, pageNum, r);
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


		private void placeText(string text, int pageNum, Rectangle r, float dx = 0, float dy = 0)
		{
			Paragraph p = new Paragraph(text);

			destDoc.ShowTextAligned(p, 
				r.GetX()+currAnnoInfo.textDx+dx, 
				r.GetY()+currAnnoInfo.textDy+dy, 
				pageNum, 
				currAnnoInfo.ta, 
				VerticalAlignment.MIDDLE, 
				currAnnoInfo.textRotation);
		}

		

	#endregion


		
		private void rotatePage()
		{
			Rectangle ps = pdfPage.GetPageSizeWithRotation();

			if (!(ps.GetWidth() > ps.GetHeight()) && pdfPage.GetRotation() == 0)
			{
				pdfPage.SetRotation(-90);
			}
		}

		private Rectangle rotateRectangle(Rectangle r)
		{
			if (checkRotatePage() == 0 || !doRotate) return r;

			Rectangle rr = null;
			Rectangle ps = pageSizeWithRotation;


			if (pageRotation == 90)
			{
				// for 90 = page 5
				rr = new Rectangle(ps.GetHeight()-r.GetY()-r.GetHeight(), r.GetX(), r.GetHeight(), r.GetWidth());
			}
			else
			{
				// for 270 = page 1
				rr = new Rectangle(r.GetY(), ps.GetWidth() - r.GetX() - r.GetWidth(), r.GetHeight(), r.GetWidth());
			}

			return rr;
		}

		private float checkRotatePage()
		{
			if (pageSize.GetWidth() > pageSize.GetHeight()) return 0;

			return pageRotation;

		}




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
			float auBxH = 150f;;

			float dsBxW = 10f;
			float dsBxH = 250f;

			float tstBxW = 150f;
			float tstBxH = 50f;

			
			float W = LINK_BOX_W;
			float H = LINK_BOX_H;

			float marginX = 70;
			float marginY = 50;

			rgbInfo2 = new DeviceRgb[,]
			{
				{new DeviceRgb(255, 180, 180),new DeviceRgb(255, 90, 90)},							// 0   redish
				{new DeviceRgb(180, 255, 180),new DeviceRgb(90, 255, 90)},							// 1  greenish
				{new DeviceRgb(180, 180, 255),new DeviceRgb(90, 90, 255)},							// 2  blueish
				{new DeviceRgb(140, 40, 225), new DeviceRgb(140, 20,180)},							// 3  purple
				{new DeviceRgb(150, 200, 50), new DeviceRgb(80, 100, 25)},							// 4  olive
				{new DeviceRgb(255, 125, 90), new DeviceRgb(170, 80, 50)},							// 5  orangish

				{new DeviceRgb(Red), DeviceRgb.MakeDarker(new DeviceRgb(Red)), },					// 6
				{new DeviceRgb(DeepSkyBlue), DeviceRgb.MakeDarker(new DeviceRgb(DeepSkyBlue)), },	// 7
				{new DeviceRgb(LawnGreen), DeviceRgb.MakeDarker(new DeviceRgb(LawnGreen)), },		// 8
				{new DeviceRgb(Tomato), DeviceRgb.MakeDarker(new DeviceRgb(Tomato)), },	            // 9

				{new DeviceRgb(Fuchsia), DeviceRgb.MakeDarker(new DeviceRgb(Fuchsia)), },	        // 10
				{new DeviceRgb(LightSkyBlue), DeviceRgb.MakeDarker(new DeviceRgb(LightSkyBlue)), }, // 11

				{new DeviceRgb(IndianRed), DeviceRgb.MakeDarker(new DeviceRgb(IndianRed)), }, // 12
				{new DeviceRgb(Bisque), DeviceRgb.MakeDarker(new DeviceRgb(Bisque)), }, // 13
				{new DeviceRgb(Tan), DeviceRgb.MakeDarker(new DeviceRgb(Tan)), }, // 14
			};


			// boxes[] = 0 for 36x48 | 1 for 30x42
			// boxes[][0] = sheet number box
			// boxes[][1] = author box
			// boxes[][3] = disclaimer box
			rectBoxes = new []
			{
					new Rectangle(ps0x-snBxW-marginX,marginY,snBxW,snBxH),              // 0  sheet number
					new Rectangle(ps0x-auBxW-marginX,ps0y-marginY-auBxH,auBxW,auBxH),   // 1  author
					new Rectangle(marginX,ps0y-marginY-dsBxH,dsBxW,dsBxH),              // 2  disclaimer
																						     
					new Rectangle(0,0, tstBxW, tstBxH),		                            // 3  at origin
					new Rectangle(ps0x-tstBxW,0, tstBxW, tstBxH),                       // 4  bottom edge right side
					new Rectangle(ps0x-tstBxH, ps0y-tstBxW, tstBxH, tstBxW),            // 5  origin or diag corner
					
					// to page A left-down from center - horizontal orientation
					new Rectangle(ctr0x-75 , ctr0y-175,W,H),                            // 6 ctr - lower-left
					new Rectangle(ctr0x+125, ctr0y-75 ,H,W),                            // 7 ctr - lower-right
					new Rectangle(ctr0x-175, ctr0y+125,W,H),                            // 8 ctr - upper-right
					new Rectangle(ctr0x-175, ctr0y-175,H,W),                            // 9 ctr - upper-left
					new Rectangle(0,0,W,H),                                             // 10 origin
					new Rectangle(800,300,W,H),                                         // 11 test

					new Rectangle(300,1200,200,50),                                     // 12 test
					new Rectangle(300,1200,-200,50),                                    // 13 test
					new Rectangle(300,1200,-200,-50),                                   // 14 test
			};


			annoInfo = new annoInfo[,]
			{
				{
					// new annoInfo(rectBoxes[0], rgbInfo2[0,0], rgbInfo2[0,1], "36x48 - anno - sheet number"),
					new annoInfo(rectBoxes[1], rgbInfo2[1,0], rgbInfo2[1,1], "36x48 - anno - author", 0, 15,15, TextAlignment.LEFT),
					new annoInfo(rectBoxes[2], rgbInfo2[2,0], rgbInfo2[2,1], "36x48 - anno - disclaimer", 90, 5,5, TextAlignment.LEFT),
					new annoInfo(rectBoxes[3], rgbInfo2[3,0], rgbInfo2[3,1], "36x48 - anno - origin", 0, 15,15, TextAlignment.LEFT),
					new annoInfo(rectBoxes[4], rgbInfo2[4,0], rgbInfo2[4,1], "36x48 - anno - lower-right", 0, 15,15, TextAlignment.LEFT),
					new annoInfo(rectBoxes[5], rgbInfo2[5,0], rgbInfo2[5,1], "36x48 - anno - upper-right", 90, 15,15, TextAlignment.RIGHT),
				},
				{
					// new annoInfo(rectBoxes[0], rgbInfo2[0,0], rgbInfo2[0,1], "30x42 - anno - sheet number"),
					new annoInfo(rectBoxes[1], rgbInfo2[1,0], rgbInfo2[1,1], "30x42 - anno - author") ,
					new annoInfo(rectBoxes[2], rgbInfo2[2,0], rgbInfo2[2,1], "30x42 - anno - disclaimer"),
					new annoInfo(rectBoxes[3], rgbInfo2[3,0], rgbInfo2[3,1], "30x42 - anno - origin"),
					new annoInfo(rectBoxes[4], rgbInfo2[4,0], rgbInfo2[4,1], "30x42 - anno - lower-right"),
					new annoInfo(rectBoxes[5], rgbInfo2[5,0], rgbInfo2[5,1], "30x42 - anno - upper-right")
				}
			};

			testInfo = new annoInfo[,]
			{
				{
					new annoInfo(rectBoxes[12], rgbInfo2[12,0], rgbInfo2[12,1], "36x48 - test A (+200 x +50", 0, 5,20, TextAlignment.LEFT),
					new annoInfo(rectBoxes[13], rgbInfo2[13,0], rgbInfo2[13,1], "36x48 - test B (-200 x +50", 0, -195, 20, TextAlignment.LEFT),
					new annoInfo(rectBoxes[14], rgbInfo2[14,0], rgbInfo2[14,1], "36x48 - test C (-200 x -50", 0, -195, -20, TextAlignment.LEFT),
				}
			};


			linkInfo = new [,]
			{
				{
					new annoInfo(rectBoxes[6], rgbInfo2[6,0], rgbInfo2[6,1], 
						"36x48 - link - A (LL)", 
						0, LINK_BOX_W/2, LINK_BOX_H/2, TextAlignment.CENTER),

					new annoInfo(rectBoxes[7], rgbInfo2[7,0], rgbInfo2[7,1], 
						"36x48 - link - B (LR)", 
						90, LINK_BOX_H/2, LINK_BOX_W/2, TextAlignment.CENTER) ,

					new annoInfo(rectBoxes[8], rgbInfo2[8,0], rgbInfo2[8,1], 
						"36x48 - link - C (UR)", 
						0, LINK_BOX_W/2, LINK_BOX_H/2, TextAlignment.CENTER),

					new annoInfo(rectBoxes[9], rgbInfo2[9,0], rgbInfo2[9,1], 
						"36x48 - link - D (UL)", 
						90, LINK_BOX_H/2, LINK_BOX_W/2, TextAlignment.CENTER),

					new annoInfo(rectBoxes[0], rgbInfo2[10,0], rgbInfo2[10,1], 
						"36x48 - link - F (Sht Num)", 
						0, LINK_BOX_W/2, LINK_BOX_H/2, TextAlignment.CENTER),

					new annoInfo(rectBoxes[11], rgbInfo2[11,0], rgbInfo2[11,1], 
						"36x48 - link - G (Test)", 
						0, LINK_BOX_W/2, LINK_BOX_H/2, TextAlignment.CENTER),


				},

			};

			origin = new annoInfo(rectBoxes[10], rgbInfo2[6, 0], rgbInfo2[6, 1],
				"36x48 - link - E (Origin)",
				0, LINK_BOX_W / 2, LINK_BOX_H / 2, TextAlignment.CENTER);




		}

		public override string ToString()
		{
			return $"this is {nameof(PdfText301)}";
		}
	}

	
	// private void placePageLinkRotated(PdfCanvas canvas, PdfLayer layer, Rectangle r, int toPage)
	// {
	// 	Rectangle rr = rotateRectangle2(r);
	//
	// 	placePageLink(canvas, layer, rr, toPage);
	// }

	// private Rectangle rotateRectangle(Rectangle r)
	// {
	// 	int ti = 0;
	// 	int bi = 1;
	//
	// 	double rotation = FloatOps.ToRad(pageRotation);
	//
	// 	float rotCtr = pageRotation == 90? pageSize.GetWidth()/2 : pageSize.GetHeight()/2;
	//
	// 	Point[] ic = new []
	// 	{
	// 		new Point(r.GetX(), r.GetY()),
	// 		new Point(r.GetX()+r.GetWidth(), r.GetY()+r.GetHeight()),
	// 	};
	//
	// 	Point[] oc = new Point[2];
	//
	// 	AffineTransform at = AffineTransform.GetRotateInstance(rotation, rotCtr, rotCtr);
	//
	// 	at.Transform(ic, 1, oc, 1, 1);
	// 	Point a;
	// 	Point b;
	//
	// 	if (pageRotation == 90)
	// 	{
	// 		a = at.Transform(ic[0], oc[0]);
	// 		b = at.Transform(ic[1], oc[1]);
	// 	}
	// 	else
	// 	{
	// 		b = at.Transform(ic[0], oc[0]);
	// 		a = at.Transform(ic[1], oc[1]);
	// 	}
	//
	// 	Rectangle rr= new Rectangle((float) b.y, (float) b.x,
	// 		(float) (b.x - a.x), (float) (b.y - a.y));
	//
	// 	return rr;
	// }

}
