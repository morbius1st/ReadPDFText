#region + Using Directives
using System.Diagnostics;
using iText.Kernel.Geom;

using UtilityLibrary;

using ShCommonCode.ShSheetData;

#endregion

// user name: jeffs
// created:   5/23/2024 7:01:59 PM


namespace ShCode.ShDebugShow
{
	public static class DebugShowInfo
	{
		private const int TITLE_WIDTH = 40;
		private static readonly  string SEPARATOR = "*".Repeat(TITLE_WIDTH);

		public static void StartMsg(string msg1, string msg2=null)
		{
			Debug.Write("\n\n");
			Debug.WriteLine(SEPARATOR);
			Debug.WriteLine($"*** {msg1,-1*(TITLE_WIDTH-8)} ***");
			if (msg2!=null) Debug.WriteLine($"*** {msg2,-1*(TITLE_WIDTH-8)} ***");
			Debug.WriteLine(SEPARATOR);
			Debug.Write("\n");
		}

		public static void ShowPageInfo(Rectangle ps, Rectangle psWrot, float rot)
		{
			Debug.WriteLine($"{"rotation",-TITLE_WIDTH} |  {rot:F2}");
			Debug.WriteLine($"{"page size",-TITLE_WIDTH} | w {ps.GetWidth():F2} | Half {ps.GetHeight():F2}");
			Debug.WriteLine($"{"page size w ro",-TITLE_WIDTH} | w {ps.GetWidth():F2} | Half {ps.GetHeight():F2}");
		}

		public static void ShowRectParams(SheetRectData<SheetRectId> pStr)
		{
			Debug.Write("\n");
			string temp = FormatItextData.FormatRectangle(pStr.Rect);
			Debug.WriteLine($"{"rectangle",-TITLE_WIDTH} | {temp}");

			temp = FormatItextData.FormatColor(pStr.BdrColor);
			Debug.WriteLine($"{"bdr color",-TITLE_WIDTH} | [ {temp} ]");
			Debug.WriteLine($"{"bdr opacity",-TITLE_WIDTH} | {pStr.BdrOpacity}");
			Debug.WriteLine($"{"bdr width",-TITLE_WIDTH} | {pStr.BdrWidth}");

			temp = FormatItextData.FormatDashArray(pStr.BdrDashPattern);
			Debug.WriteLine($"{"bdr width",-TITLE_WIDTH} | {temp}");

			temp = FormatItextData.FormatColor(pStr.FillColor);
			Debug.WriteLine($"{"fill color",-TITLE_WIDTH} | [ {temp} ]");
			Debug.WriteLine($"{"fill opacity",-TITLE_WIDTH} | {pStr.FillOpacity}");

		}

	}
}
