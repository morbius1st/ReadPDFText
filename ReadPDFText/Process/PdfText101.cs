#region + Using Directives
using iText.Kernel.Pdf;
using System.Diagnostics;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;

#endregion

// user name: jeffs
// created:   4/12/2024 8:38:27 PM

namespace ReadPDFText.Process
{
	public class PdfText101
	{ 
		
		private PdfDocument destPdfDoc ;
		private Document destDoc;

		private PdfCanvasProcessor parser;

		private string[] sources = new []
		{
			@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test3\PDF files\T1.0-0 - TITLE SHEET.pdf", 
			@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test3\PDF files\A0.1.0 - COVER SHEET.pdf"
		};

		private string dest = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test3\combined.pdf";

		public void ProcessPdf()
		{
			Rectangle r;
			PdfPage page;
			string result;

			PdfWriter w = new PdfWriter(dest);

			destPdfDoc = new PdfDocument(w);
			destDoc = new Document(destPdfDoc);

			PdfDocument srcPdf = new PdfDocument(new PdfReader(sources[1]));

			srcPdf.CopyPagesTo(1, 1, destPdfDoc);

			srcPdf.Close();

			page = destPdfDoc.GetPage(1);

			result = Extract(page);

			Debug.WriteLine(result);

		}

		private string Extract(PdfPage page)
		{
			string result;

			TextLengthFilter cf = new TextLengthFilter();

			FilteredEventListener listener = new FilteredEventListener();

			LocationTextExtractionStrategy strat = listener.AttachEventListener(new LocationTextExtractionStrategy(), cf);

			if (parser != null) parser.Reset();

			parser = new PdfCanvasProcessor(listener);

			parser.ProcessPageContent(page);

			result = strat.GetResultantText();

			return result;
		}

	}

	public class TextLengthFilter : IEventFilter
	{
		private string text; 

		public bool Accept(IEventData data, EventType type)
		{
			if (type != EventType.RENDER_TEXT) return false;

			TextRenderInfo ri = (TextRenderInfo) data;

			text = ri.GetText();

			if (text == null || text.Length < 3) return false;

			// Debug.WriteLine(text);

			return true;
		}
	}
}
