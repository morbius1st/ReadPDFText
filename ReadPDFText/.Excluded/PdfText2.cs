#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Navigation;
using iText.Layout.Element;
using iText.Layout.Renderer;

#endregion

// user name: jeffs
// created:   2/4/2024 6:23:09 PM

namespace SharedCode.ShDataSupport
{
	/*
	// example from itext - does not work and not sure what it would do
	public class PdfText2
	{
		private string[] shtNums = new [] { "A1.1-0", "A1.2-0a", "A1.2-0b" };


		public void Test2AddLinks(PdfDocument pdfDoc)
		{
			pdfDoc.GetCatalog().SetPageMode(PdfName.UseOutlines);

			PdfOutline ol = null;

			for (int i = 1; i < pdfDoc.GetNumberOfPages(); i++)
			{
				ol = createOutline(ol, pdfDoc, shtNums[i-1], i);
			}
		}


		public PdfOutline createOutline(PdfOutline ol,
			PdfDocument pdfDoc, string title, int page)
		{
			if (ol == null)
			{
				ol = pdfDoc.GetOutlines(false);
				ol = ol.AddOutline(title);
				return ol;
			}

			Text t = new Text(title);

			OutlineRenderer renderer = new OutlineRenderer(t, pdfDoc.GetPage(page));

			return ol;
		}
	}

	public class OutlineRenderer : TextRenderer
	{
		private PdfOutline parent;

		private string title = "A1.1-0";

		private PdfPage page;

		public OutlineRenderer(Text textElement, PdfPage page) : base(textElement)
		{
			title = textElement.GetText();
			this.page = page;
		}

		public override void Draw(DrawContext drawContext)
		{
			base.Draw(drawContext);

			PdfDestination dest =
				PdfExplicitDestination.CreateFit(page);

			PdfOutline ol = parent.AddOutline(title);
			ol.AddDestination(dest);
		}


		// public OutlineRenderer(Text textElement, string text) : base(textElement, text)
		// {
		// 	Debug.Write($"te2| {textElement.GetText()} & tx| {text}");
		// }
		//
		// protected internal OutlineRenderer(TextRenderer other) : base(other)
		// {
		// 	Debug.Write($"te1| {other.GetText()}");
		// }
	}

	*/
}