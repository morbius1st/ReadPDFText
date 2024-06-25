#region + Using Directives
using System;
using System.Globalization;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;

#endregion

// user name: jeffs
// created:   5/18/2024 5:26:02 PM

namespace SharedCode.ShPdfSupport
{
	public class PdfSupport
	{
		public override string ToString()
		{
			return $"this is {nameof(PdfSupport)}";
		}

		// public static string FormatRectangle(AltRectangle r)
		// {
		// 	return $"x| {r.X:F2} | y| {r.Y:F2} | w| {r.Width:F2} | h| {r.Height:F2}";
		// }



		// for the below, null represents no color or "transparent"
		public Color makeColor(string h)
		{
			if (h == null) return null;

			float[] c = new float[3];
			c[0] = Int32.Parse(h.Substring(1, 2), NumberStyles.AllowHexSpecifier);
			c[1] = Int32.Parse(h.Substring(3, 2), NumberStyles.AllowHexSpecifier);
			c[2] = Int32.Parse(h.Substring(5, 2), NumberStyles.AllowHexSpecifier);

			return Color.CreateColorWithColorSpace(c);
		}

		public Color makeColor(PdfArray pa)
		{
			if (pa == null) return null;

			float[] c = pa.ToFloatArray();

			return Color.CreateColorWithColorSpace(c);
		}

		public Color makeColor(PdfString ps)
		{
			if (ps == null || ps.GetValue() == null) return null;

			string[] s = ps.ToString().Split(' ');

			float[] c = new float[3];

			c[0] = float.Parse(s[0]);
			c[1] = float.Parse(s[1]);
			c[2] = float.Parse(s[2]);

			return Color.CreateColorWithColorSpace(c);
		}

		/*

		public static string FormatRectangle(Rectangle r)
		{
			return $"x| {r.GetX():F2} | y| {r.GetY():F2} | w| {r.GetWidth():F2} | h| {r.GetHeight():F2}";
		}


		public static string FormatColor(Color c)
		{
			if (c == null) return "transparent";

			int qty = c.GetColorSpace().GetNumberOfComponents();

			StringBuilder result = new StringBuilder();

			float[] v = c.GetColorValue();

			int rgbValue;

			foreach (float f in v)
			{
				if (qty == 3)
				{
					rgbValue = (int) (f * 256);
					result.Append($"{rgbValue} ");
				}
				else
				{
					result.Append($"{f:F2} ");
				}
				
			}

			return result.ToString();
		}

		public static string FormatArray(float[] floats)
		{
			if (floats == null) return "none";

			StringBuilder result = new StringBuilder();

			int i;

			for (i = 0; i < floats.Length-1; i++)
			{
				result.Append($"{floats[i]:F2}, ");
			}

			result.Append($"{floats[i]:F2}");

			return result.ToString();
		}

		public static string FormatFontWeight(int weight)
		{
			string[] weights = new []
			{
				"THIN",
				"EXTRA_LIGHT",
				"LIGHT",
				"NORMAL",
				"MEDIUM",
				"SEMI_BOLD",
				"BOLD",
				"EXTRA_BOLD",
				"BLACK",
			};

			return weights[(weight / 100)-1];
		}

		public static string FormatFontStyle(int style)
		{
			if (style == -1) return "UNDEFINED";
			if (style == 0) return "NORMAL";

			string result = null;

			if ((style & 1) == 1) result = "BOLD";
			if ((style & 2) == 2) result += "ITALIC";

			return result;
		}
		*/
	}
}

