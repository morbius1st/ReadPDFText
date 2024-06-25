#region + Using Directives

using iText.Kernel.Font;
using iText.Kernel.Geom;

#endregion

// user name: jeffs
// created:   5/27/2024 10:13:51 AM

namespace ShItextCode.ElementCreation
{
	public static class CreateSupport
	{
		public const string CS = @"https://www.cyberstudio.pro/";
		public const string DefaultFontFile = @"c:\windows\fonts\arialn.ttf";
		public static PdfFont DefaultFont;


		static CreateSupport()
		{
			DefaultFont = PdfFontFactory.CreateFont(DefaultFontFile);
		}


		public static float PdfPageRotation { get; set; }
		public static Rectangle PageSizeWithRotation { get; set; }

		public static Rectangle rotatSheetRectangleIfNeeded(Rectangle r)
		{
			return r;
		}

		public static Rectangle rotatSheetRectangleIfNeeded3(Rectangle r)
		{
			if (PdfPageRotation == 0) return r;

			Rectangle rr;
			Rectangle ps = PageSizeWithRotation;

			// if (PageRotation == 90)
			if (PdfPageRotation != 0)
			{
				rr = new Rectangle(ps.GetHeight() - r.GetY(), r.GetX(), -1 * r.GetHeight(), r.GetWidth());
			}
			else
			{
				rr = new Rectangle(r.GetY(), ps.GetWidth() - r.GetX(), r.GetHeight(), -1 * r.GetWidth());
			}

			return rr;
		}
	}
}
