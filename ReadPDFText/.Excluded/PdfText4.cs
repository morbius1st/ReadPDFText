#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Navigation;
using iText.Layout.Properties;

#endregion

// user name: jeffs
// created:   2/9/2024 5:06:23 PM

namespace SharedCode.ShDataSupport
{

	/*

	public class PdfText4
	{
		private int pgCount;

		private PdfName tocName;

		private List<string> shtNums;
		public Rectangle shtId;

		public List<List<TextAndLineSegmentData>> linkText = new List<List<TextAndLineSegmentData>>();

		private static TextAndLocationXcopier4 talXc;

		private float pi180 = (float) Math.PI;
		private float pi90;

		private PdfArray border = new PdfArray(new int[] { 0, 0, 1 });
		// private Rectangle rootR = new Rectangle(0, 0, (float) 8.5 * 72, (float) 11 * 72);

		public PdfText4()
		{
			pi90 = pi180 / 2;
		}

		// A1.2-0c / A1.2-0d / A1.2-0e
		public void AddLinks(List<string> srcList, string srcLoc, string dest)
		{
			tocName = new PdfName("toc");


			shtNums = new List<string>()
			{
				"A1.1-0", "A1.2-0a", "A1.2-0b", "A1.2-0c", "A1.2-0d", "A1.1-x1", "A1.1-x2", "A1.1-x3"
			};

			shtId = new Rectangle(426f, 27f, 161f, 123f);

			// foreach (string s in shtNums)
			// {
			// 	Console.WriteLine($"sht num| {s}");
			// }

			talXc = new TextAndLocationXcopier4(shtNums, shtId);

			PdfDocument destPdfDoc = new PdfDocument(new PdfWriter(dest));

			Document destDoc = new Document(destPdfDoc);

			destPdfDoc.InitializeOutlines();

			// Dictionary<string, PdfDocument> fileList = initFilesToMerge(srcList);

			int page = 1;

			foreach (string s in srcList)
			{
				Console.WriteLine($"src file| {s}");
			}


			foreach (string s in srcList)
			{
				string src = $"{srcLoc}\\{s}";
				PdfDocument srcDoc =  new PdfDocument(new PdfReader(src));

				Console.WriteLine($"src doc| {srcDoc.GetDocumentInfo().GetTitle()}");

				int pageCount = srcDoc.GetNumberOfPages();

				for (int i = 1; i <= pageCount; i++, page++)
				{
					// Text text = new Text($"Page {page}");

					srcDoc.CopyPagesTo(i, i, destPdfDoc, talXc);

					linkText.Add(talXc.LinkText);
				}

				// all pages scanned and link text found
				srcDoc.Close();
			}

			page = 1;

			PdfLayer layer = new PdfLayer("links", destPdfDoc);
			layer.SetOn(true);

			Rectangle pgSize = destPdfDoc.GetPage(1).GetPageSize();

			ICollection<string> fonts = PdfFontFactory.GetRegisteredFonts();

			string fontFile = @"c:\windows\fonts\arialn.ttf";

			PdfFont font = PdfFontFactory.CreateFont(fontFile);
			Style banenrStyle = new Style();
			banenrStyle.SetFont(font)
			.SetFontColor(DeviceRgb.BLACK)
			.SetFontSize(24);


			Text banner = new Text("this is the page banner");
			Paragraph pg2 = new Paragraph(banner)
				// .SetFixedPosition(72,72, pgSize.GetHeight() - 150)
				.SetMargin(0)
				.SetMultipliedLeading(0)
				.AddStyle(banenrStyle)
				.SetRotationAngle(pi90)
				// .SetVerticalAlignment(VerticalAlignment.BOTTOM)
				// .SetHorizontalAlignment(HorizontalAlignment.LEFT)
				;
			// Paragraph pg3 = pg2.SetFixedPosition(72, 72, pgSize.GetHeight() - 150);

			//Console.WriteLine($"\tparagraph info| {pg2.GetMarginLeft()}| {pg2.GetMarginBottom()}| {pg2.GetPaddingLeft()}| {pg2.GetPaddingBottom()}");

			

			for (int i = 1; i <= destPdfDoc.GetNumberOfPages(); i++, page++)
			{
				int d = i == 3 ? 1 : i + 1;


				destDoc.ShowTextAligned(pg2, 144,72, i, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);

				PdfPage p = destPdfDoc.GetPage(i);

				PdfPage dPage = destPdfDoc.GetPage(i);

				Text text = new Text($"Page {page}");
				text.SetDestination("p" + page);


				// *** begin adds contents to a layer

				PdfCanvas pdfCanvas = new PdfCanvas(dPage);
				pdfCanvas = pdfCanvas.BeginLayer(layer);
				Canvas canvas = new Canvas(pdfCanvas, dPage.GetPageSize());

				pdfCanvas.SaveState();

				// also added to the layer / canvas
				// at the position noted
				canvas.ShowTextAligned(pg2, 72, 72, TextAlignment.LEFT);

				// also added to the layer / canvas but not at the location noted
				// does not work
				// canvas.SetFixedPosition(24, 72, 250).Add(pg2);

				// this worked
				canvas.Add(pg2.SetFixedPosition(24, 72, 250));

				// alternate add the paragraph to the document and not the layer

				pdfCanvas.BeginLayer(layer);

				Paragraph pg1 = new Paragraph(text)
				.SetFixedPosition(page, 540, 756, 140)
				.SetMargin(0f)
				.SetMultipliedLeading(1)
				.SetFontColor(new DeviceRgb(255, 0, 0))
				.SetFontSize(18f);

				canvas.Add(pg1);

				pdfCanvas.EndLayer();



				pdfCanvas.RestoreState();

				// *** end adds contents to a layer

				foreach (TextAndLineSegmentData ls in linkText[i - 1])
				{
					Rectangle r = ls.GetRectangle();

					PdfAnnotation anno = new PdfLinkAnnotation(r)
					.SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT)
					.SetAction(PdfAction.CreateGoTo(PdfExplicitDestination.CreateFit(dPage)))
					.SetBorder(border);

					p.AddAnnotation(anno);

					anno.SetLayer(layer);
				}

				// p.Flush();
			}

			showLinkText();

			addNamedDestLink(destPdfDoc);
			addNamedDest(destPdfDoc);
			

			destDoc.Close();

			destPdfDoc.Close();
		}


		private void addNamedDestLink(PdfDocument destPdfDoc)
		{
			PdfPage p = destPdfDoc.GetPage(2);

			PdfAnnotation pa = new PdfLinkAnnotation(shtId)
			.SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT)
			.SetAction(PdfAction.CreateGoTo(new PdfNamedDestination(tocName.ToString())))
			.SetBorder(border);

			p.AddAnnotation(pa);

		}

		private void addNamedDest(PdfDocument destPdfDoc)
		{
			PdfPage p = destPdfDoc.GetPage(1);
			Rectangle r = p.GetPageSize();

			PdfExplicitDestination destObj = PdfExplicitDestination.CreateFit(p);
			// PdfExplicitDestination destObj = PdfExplicitDestination.CreateXYZ(p, r.GetLeft(), r.GetTop(), 1);
			destPdfDoc.AddNamedDestination(tocName.ToString(), destObj.GetPdfObject());

		}



		private bool findSheetNumber()
		{
			bool results = true;

			// all found text is here: linkText
			// the zone for the sheet number is: shtId

			List<TextAndLineSegmentData> shtNumberData = new List<TextAndLineSegmentData>();
			List<string> shtNumbers = new List<string>();

			int pageNum = 0;
			TextAndLineSegmentData lsx;

			foreach (List<TextAndLineSegmentData> textAndLineSegmentDatas in linkText)
			{
				pageNum++;
				bool temp = false;

				foreach (TextAndLineSegmentData ls in textAndLineSegmentDatas)
				{
					if (shtId.Contains(ls.GetRectangle()))
					{
						lsx = ls;
						lsx.OnPageNumber = pageNum;
						shtNumberData.Add(lsx);
						shtNumbers.Add(ls.Text);
						temp = true;

						textAndLineSegmentDatas.Remove(ls);

						break;
					}
				}

				results = temp && results;
			}
			return true;
		}


		// just to show the properties obtained from the found text
		private void showLinkText()
		{
			int page = 0;
			foreach (List<TextAndLineSegmentData> textAndLineSegmentDatas in linkText)
			{
				Console.WriteLine($"\n*** page {++page} ***");

				foreach (TextAndLineSegmentData ls in textAndLineSegmentDatas)
				{
					if (ls.IsRect)
					{
						Console.WriteLine($"text| {ls.Text} | angle| {ls.AngleRad} | BL| {ls.Corners[2].x}, {ls.Corners[2].y}");
					}
					else
					{
						double angle = ls.AngleRad * (180 / pi180);

						Console.WriteLine($"text| {ls.Text} | angle| {angle} | BL| {ls.Corners[2].x}, {ls.Corners[2].y} | H| {ls.Height} | W| {ls.Width}");
					}
				}
			}
		}

		// just takes the list of file strings and places them into another list
		// here only due to adjustment
		// not used
		private Dictionary<string, PdfDocument> initFilesToMerge(List<string> srcList)
		{
			Dictionary<string, PdfDocument> fileList = new Dictionary<string, PdfDocument>();

			foreach (string s in srcList)
			{
				// fileList.Add(s, new PdfDocument(new PdfReader(s)));
				fileList.Add(s, null);
			}

			return fileList;
		}
	}

	/// <summary>
	/// provides extra page coping logic.  this runs the text
	/// extractor and places the text into a list
	/// </summary>
	public class TextAndLocationXcopier4 : IPdfPageExtraCopier
	{
		private static TextAndLocationXstrategy4 tals;
		private List<string> toFind;
		private Rectangle sheetNumberArea;

		public List<TextAndLineSegmentData> LinkText { get; private set; }


		public TextAndLocationXcopier4(List<string> toFindList, Rectangle shtNumArea)
		{
			toFind = toFindList;
			sheetNumberArea = shtNumArea;
			tals = new TextAndLocationXstrategy4(toFind, sheetNumberArea);
		}


		public void Copy(PdfPage fromPage, PdfPage toPage)
		{
			LinkText = new List<TextAndLineSegmentData>();

			LinkText = tals.GetList(toPage);
		}
	}

	/// <summary>
	/// finds all text per page.  adds the found text along with
	/// its position information to a list for each string that
	/// matches the list of strings to find
	/// </summary>
	class TextAndLocationXstrategy4 : PdfTextAndLocationStrategy
	{
		private List<string> toFind;
		private Rectangle sheetNumberArea;

		public List<TextAndLineSegmentData> list;

		public TextAndLocationXstrategy4(List<string> toFindList, Rectangle shtNumArea)
		{
			toFind = toFindList;
			sheetNumberArea = shtNumArea;
		}

		public List<TextAndLineSegmentData> GetList(PdfPage page)
		{
			list = new List<TextAndLineSegmentData>();

			PdfTextExtractor.GetTextFromPage(page, this);

			return this.list;
		}


		public override void EventOccurred(IEventData data, EventType type)
		{
			if (!type.Equals(EventType.RENDER_TEXT)) return;

			TextRenderInfo ri = (TextRenderInfo) data;

			string text = ri.GetText();

			if (validateText(text))
			{
				list.Add(new TextAndLineSegmentData(text, ri.GetAscentLine(), ri.GetDescentLine(), true));
			}

			base.EventOccurred(data, type);
		}

		private bool validateText(string  text)
		{
			Console.WriteLine($"testing| {text}");

			return toFind.Contains(text);
		}
	}

	*/
}