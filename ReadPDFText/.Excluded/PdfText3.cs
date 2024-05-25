#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

#endregion

// user name: jeffs
// created:   2/7/2024 10:22:01 PM

namespace SharedCode.ShDataSupport
{
	/*

	public class PdfText3
	{
		private int pgCount;

		private List<string> shtNums;
		private Rectangle shtId;

		public void GetPdfContents(List<string> srcList, string dest)
		{
			shtNums = new List<string>()
			{
				"A1.1-0", "A1.2-0a", "A1.2-0b", "A1.2-0c"
			};

			shtId = new Rectangle(426f, 27f, 161f, 123f);


			foreach (string s in shtNums)
			{
				Console.WriteLine($"sht num| {s}");
			}



			PdfPageFormCopier formCopier = new PdfPageFormCopier();

			TextAndLocationExtraCopier TalXc = new TextAndLocationExtraCopier(shtNums, shtId);

			PdfDocument destPdfDoc = new PdfDocument(new PdfWriter(dest));

			Document destDoc = new Document(destPdfDoc);

			destPdfDoc.InitializeOutlines();

			Dictionary<string, PdfDocument> fileList = initFilesToMerge(srcList);

			int page = 1;

			foreach (KeyValuePair<string, PdfDocument> kvp in fileList)
			{
				PdfDocument srcDoc = kvp.Value;

				int pageCount = srcDoc.GetNumberOfPages();

				for (int i = 1; i <= pageCount; i++, page++)
				{
					Text text = new Text($"Page {page}");

					srcDoc.CopyPagesTo(i, i, destPdfDoc, TalXc);

					if (i == 1)
					{
						text.SetDestination("p" + page);
					}

					destDoc.Add(new Paragraph(text).SetFixedPosition(page, 540, 756, 140)
						.SetMargin(0f)
						.SetMultipliedLeading(1)
						.SetFontColor(new DeviceRgb(255,0,0))
						.SetFontSize(18f)
						);
				}
			}

			foreach (PdfDocument srcDoc2 in fileList.Values)
			{
				srcDoc2.Close();
			}

			destDoc.Close();

			destPdfDoc.Close();
		}


		private Dictionary<string, PdfDocument> initFilesToMerge(List<string> srcList)
		{
			Dictionary<string, PdfDocument> fileList = new Dictionary<string, PdfDocument>();

			foreach (string s in srcList)
			{
				fileList.Add(s, new PdfDocument(new PdfReader(s)));
			}

			return fileList;
		}




		public override string ToString()
		{
			return $"this is {nameof(PdfText3)}";
		}
	}

	*/

	// public class TextAndLocationExtraCopier : IPdfPageExtraCopier
	// {
	// 	private List<string> toFind;
	// 	private Rectangle sheetNumberArea;
	//
	// 	public TextAndLocationExtraCopier(List<string> toFindList, Rectangle shtNumArea)
	// 	{
	// 		toFind= toFindList;
	// 		sheetNumberArea = shtNumArea;
	// 	}
	//
	//
	// 	public void Copy(PdfPage fromPage, PdfPage toPage)
	// 	{
	// 		TextAndLocationExtractorStrategy tals = new TextAndLocationExtractorStrategy(toFind, sheetNumberArea);
	//
	// 		List<TextAndLineSegmentData> list = tals.GetList(toPage);
	//
	// 		foreach (TextAndLineSegmentData ls in list)
	// 		{
	// 			Debug.WriteLine($"text| {ls.Text}");
	// 		}
	// 	}
	// }


	//
	// class TextAndLocationExtractorStrategy : PdfTextAndLocationStrategy
	// {
	// 	private List<string> toFind;
	// 	private Rectangle sheetNumberArea;
	//
	// 	public List<TextAndLineSegmentData> list;
	//
	// 	public TextAndLocationExtractorStrategy(List<string> toFindList, Rectangle shtNumArea)
	// 	{
	// 		toFind= toFindList;
	// 		sheetNumberArea = shtNumArea;
	//
	// 		list = new List<TextAndLineSegmentData>();
	// 	}
	//
	// 	public List<TextAndLineSegmentData> GetList(PdfPage page)
	// 	{
	// 		PdfTextExtractor.GetTextFromPage(page, this);
	//
	// 		return this.list;
	// 	}
	//
	//
	// 	public override void EventOccurred(IEventData data, EventType type)
	// 	{
	// 		if (!type.Equals(EventType.RENDER_TEXT)) return;
	//
	// 		TextRenderInfo ri = (TextRenderInfo) data;
	//
	// 		string text = ri.GetText();
	//
	// 		if (validateText(text))
	// 		{
	// 			LineSegment lsTop = ri.GetAscentLine();
	// 			LineSegment lsBott = ri.GetDescentLine();
	//
	// 			list.Add(new TextAndLineSegmentData(text, lsTop, lsBott, true));
	// 		}
	//
	// 		base.EventOccurred(data, type);
	// 	}
	//
	// 	private bool validateText(string  text)
	// 	{
	// 		Console.WriteLine($"testing| {text}");
	//
	// 		return toFind.Contains(text);
	// 	}
	//
	//
	// 	protected override bool IsChunkAtWordBoundary(TextChunk chunk, TextChunk previousChunk)
	// 	{
	// 		Debug.WriteLine($"got chunk| {chunk}");
	//
	// 		return base.IsChunkAtWordBoundary(chunk, previousChunk);
	// 	}
	// }

}