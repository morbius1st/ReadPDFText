#region + Using Directives

using iText.Kernel.Pdf.Canvas.Parser.Data;

#endregion

// user name: jeffs
// created:   4/21/2024 6:00:50 PM

namespace ReadPDFText.PdfSupport
{
	public static class PdfExtensions
	{
		public static float GetStartX(this TextRenderInfo ri)
		{
			return ri.GetDescentLine().GetStartPoint().Get(0);
		}

		public static float GetStartY(this TextRenderInfo ri)
		{
			return ri.GetDescentLine().GetStartPoint().Get(1);
		}

		public static float GetEndX(this TextRenderInfo ri)
		{
			return ri.GetDescentLine().GetEndPoint().Get(0);
		}

		public static float GetEndY(this TextRenderInfo ri)
		{
			return ri.GetDescentLine().GetEndPoint().Get(1);
		}


	}
}
