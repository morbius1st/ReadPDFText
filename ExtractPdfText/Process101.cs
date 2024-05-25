#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonPdfCodeShCode;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Layer;
using iText.Layout;
using static iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy;

using Rectangle = iText.Kernel.Geom.Rectangle;

#endregion

// user name: jeffs
// created:   5/5/2024 11:16:00 PM

namespace ExtractPdfText
{

	/* final answer - at the moment - other extraction methods do not work or are no faster
	 */


	public class Process101
	{
		public const string PAGES_PG_NUM_FORMAT = "*{0:D5}";


		private PdfDocument destPdfDoc ;
		private Document destDoc;
		private PdfCanvasProcessor parser;
		private PdfPage pdfPage;
		private PdfLayer layer;
		private  PdfCanvas canvas;

		private Rectangle r;

		private Dictionary<string, PdfTreeLeaf> pages;
		private PdfTreeLeaf currLeaf;

		private int numPages;
		private int pageNum;
		private Rectangle pageSizeWithRotation;
		private float pageRotation;

		// private Rectangle shtNumFindRect = new Rectangle(3196, 50, 190, 150);  // ?? 36x48
		private Rectangle shtNumFindRect = new Rectangle(2810, 41, 179, 144);  // 30x42 - simon brea

		private string shtNumber2ndChar = "0123456789";

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

			getText();

			// getSheetNumbers();
			//
			// locateSheetReferences();
			//
			// addAnnoAndLinks();

			// reRotatePages();

			closeDocs();
		}

		private void closeDocs()
		{
			destDoc.Close();

			destPdfDoc.Close();
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

			PdfDocument srcPdfDoc = null;

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in branchNode.ItemList)
			{
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

			numPages = destPdfDoc.GetNumberOfPages();
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


		private void getText()
		{
			bool result;

			Stopwatch sw = Stopwatch.StartNew();

			sw.Start();

			Console.Write("validate ");

			foreach (KeyValuePair<string, PdfTreeLeaf> kvp in pages)
			{
				if (kvp.Key[0] == '*') continue;

				// Debug.WriteLine($"\nvalidating| {kvp.Key} - {kvp.Value.SheetName}");

				currLeaf = kvp.Value;

				result = getTextInfo();
				// getTextv4();
				getTextv5();

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
		}

		private bool getTextInfo()
		{
			pageNum = currLeaf.PageNumber;

			pdfPage = destPdfDoc.GetPage(pageNum);

			// below needed for rotate rectangle
			pageRotation = pdfPage.GetRotation();
			pageSizeWithRotation = pdfPage.GetPageSizeWithRotation();

			getTextv3();

			return true;
		}


		private bool getTextv1()
		{
			Rectangle rr = rotateFindRectangle(shtNumFindRect);

			TextRegionEventFilter filter = new TextRegionEventFilter(rr);
			TextMarginFinder listener = new TextMarginFinder();

			LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();

			return true;
		}


		private void getTextv2()
		{
			PdfDocumentContentParser p = new PdfDocumentContentParser(destPdfDoc);

			TextMarginFinder s = p.ProcessContent(1, new TextMarginFinder());

			Rectangle r = s.GetTextRectangle();

			Debug.WriteLine($"info| x {r.GetX():F2}| y {r.GetY():F2} w {r.GetWidth():F2}| h {r.GetHeight():F2}");
		}

		private void getTextv3()
		{
			Rectangle rr = rotateFindRectangle(shtNumFindRect);

			IList<TextChunk> chunks;

			string[] texts = ReaderExtensions.ExtractText(pdfPage, new [] { rr });

			chunks = ReaderExtensions.locationalResult;

			// Debug.WriteLine($"{currLeaf.SheetNumber}");

			// foreach (string text in texts)
			// {
			// 	Debug.WriteLine($"find rect| >{text}<\n");
			// }

		}

		// works well but still getting one character at a time - does not help.
		private void getTextv4()
		{
			Rectangle rr = rotateFindRectangle(new Rectangle(0,0,48f*72.0f, 36f*72.0f));

			IList<TextChunk> chunks;

			string[] texts = ReaderExtensions.ExtractText(pdfPage, new [] { rr });

			chunks = ReaderExtensions.locationalResult;

			// Debug.WriteLine($"{currLeaf.SheetNumber}| count {(chunks?.Count ?? -1)}");

			ITextChunkLocation a;
			ITextChunkLocation first;
			ITextChunkLocation last;

			ITextChunkLocation p =
				chunks[0].GetLocation();

			first = p;

			int idx = 0;
			string text = null;

			foreach (TextChunk tc in chunks)
			{
				a = tc.GetLocation();

				if (a.IsAtWordBoundary(p))
				{
					if (text[0]=='A'  && text.Length > 3 && shtNumber2ndChar.IndexOf(text[1]) != -1 && text.Length < 7)
					{
						// Debug.Write($"text >{text}<  ");

						first = a;
						last = p;

						// Debug.WriteLine($"first x {a.GetStartLocation().Get(0):F2}| y {a.GetStartLocation().Get(1):F2}");
						// Debug.WriteLine($" last x {p.GetEndLocation().Get(0):F2}| y {p.GetEndLocation().Get(1):F2}");
					}

					text = null;
				}

				text += tc.GetText();

				p = a;

				// Debug.Write($"text >{tc.GetText()}< ");
				//
				// Debug.WriteLine($"start x {a.GetStartLocation().Get(0):F2}| y {a.GetStartLocation().Get(1):F2}");
				// Debug.WriteLine($"  end x {a.GetEndLocation().Get(0):F2}| y {a.GetEndLocation().Get(1):F2}");
				// Debug.WriteLine($"o mag   {a.OrientationMagnitude()}");

			}

		}

		private void getTextv5()
		{
			// Rectangle rr = rotateFindRectangle(new Rectangle(0,0,48f*72.0f, 36f*72.0f));

			IList<TextChunk> chunks;

			 ExtractionExtension.ExtractText(pdfPage);

			chunks = ExtractionExtension.locationalResult;

			// Debug.WriteLine($"{currLeaf.SheetNumber}| count {(chunks?.Count ?? -1)}");

			ITextChunkLocation a;
			ITextChunkLocation first;
			ITextChunkLocation last;

			ITextChunkLocation p =
				chunks[0].GetLocation();

			first = p;

			int idx = 0;
			string text = null;

			foreach (TextChunk tc in chunks)
			{
				a = tc.GetLocation();

				if (a.IsAtWordBoundary(p))
				{
					if (text[0]=='A'  && text.Length > 3 && shtNumber2ndChar.IndexOf(text[1]) != -1 && text.Length < 7)
					{
						// Debug.Write($"text >{text}<  ");

						first = a;
						last = p;

						// Debug.WriteLine($"first x {a.GetStartLocation().Get(0):F2}| y {a.GetStartLocation().Get(1):F2}");
						// Debug.WriteLine($" last x {p.GetEndLocation().Get(0):F2}| y {p.GetEndLocation().Get(1):F2}");
					}

					text = null;
				}

				text += tc.GetText();

				p = a;

				// Debug.Write($"text >{tc.GetText()}< ");
				//
				// Debug.WriteLine($"start x {a.GetStartLocation().Get(0):F2}| y {a.GetStartLocation().Get(1):F2}");
				// Debug.WriteLine($"  end x {a.GetEndLocation().Get(0):F2}| y {a.GetEndLocation().Get(1):F2}");
				// Debug.WriteLine($"o mag   {a.OrientationMagnitude()}");

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


		public override string ToString()
		{
			return $"this is {nameof(Process101)}";
		}
	}


	public static class ReaderExtensions
	{
		public static IList<TextChunk> locationalResult { get; set; }


		public static string[] ExtractText(this PdfPage page, params Rectangle[] rects)
		{
			LocationTextExtractionStrategy textEventListener = new LocationTextExtractionStrategy();
			PdfTextExtractor.GetTextFromPage(page, textEventListener);
			string[] result = new string[rects.Length];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = textEventListener.GetResultantText(rects[i]);
			}
			return result;
		}

		public static String GetResultantText(this LocationTextExtractionStrategy strategy, Rectangle rect)
		{
			locationalResult = (IList<TextChunk>)locationalResultField.GetValue(strategy);
			List<TextChunk> nonMatching = new List<TextChunk>();
			foreach (TextChunk chunk in locationalResult)
			{
				ITextChunkLocation location = chunk.GetLocation();
				Vector start = location.GetStartLocation();
				Vector end = location.GetEndLocation();
				if (!rect.IntersectsLine(start.Get(Vector.I1), start.Get(Vector.I2), end.Get(Vector.I1), end.Get(Vector.I2)))
				{
					nonMatching.Add(chunk);
				}
			}
			nonMatching.ForEach(c => locationalResult.Remove(c));
			try
			{
				return strategy.GetResultantText();
			}
			finally
			{
				nonMatching.ForEach(c => locationalResult.Add(c));
			}
		}

		static FieldInfo locationalResultField = typeof(LocationTextExtractionStrategy).GetField("locationalResult", BindingFlags.NonPublic | BindingFlags.Instance);
	}

	
	public static class ExtractionExtension
	{
		public static IList<TextChunk> Results { get; set; }
		public static IList<TextChunk> locationalResult;


		public static int ExtractText(this PdfPage page)
		{
			var textEventListener = new LocationTextExtractionStrategy();

			PdfTextExtractor.GetTextFromPage(page, textEventListener);

			locationalResult = (IList<TextChunk>) locationalResultField.GetValue(textEventListener);

			return Results.Count;
		}

		private static void process()
		{

		}




		// public static void GetTextInfo(this LocationTextExtractionStrategy strategy, Rectangle rect)
		// {
		// 	locationalResult = (IList<TextChunk>) locationalResultField.GetValue(strategy);

			// List<TextChunk> nonMatching = new List<TextChunk>();
			//
			// foreach (TextChunk chunk in locationalResult)
			// {
			// 	ITextChunkLocation location = chunk.GetLocation();
			// 	Vector start = location.GetStartLocation();
			// 	Vector end = location.GetEndLocation();
			// 	if (!rect.IntersectsLine(start.Get(Vector.I1), start.Get(Vector.I2), end.Get(Vector.I1), end.Get(Vector.I2)))
			// 	{
			// 		nonMatching.Add(chunk);
			// 	}
			// }
			// nonMatching.ForEach(c => locationalResult.Remove(c));
			// try
			// {
			// 	return strategy.GetResultantText();
			// }
			// finally
			// {
			// 	nonMatching.ForEach(c => locationalResult.Add(c));
			// }
		// }

		static FieldInfo locationalResultField = typeof(LocationTextExtractionStrategy).GetField("locationalResult", BindingFlags.NonPublic | BindingFlags.Instance);
	}
}
