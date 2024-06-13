#region + Using Directives
using System.Diagnostics;
using iText.Kernel.Geom;

using UtilityLibrary;

using ShCommonCode.ShSheetData;
using ShItextCode;

#endregion

// user name: jeffs
// created:   5/23/2024 7:01:59 PM


namespace ShCode.ShDebugInfo
{




	public enum ShowWhere
	{
		DEBUG=0,
		CONSOLE=1,
		DBG_CONS=2
	}

	public static class PdfShowInfo
	{
		private const int TITLE_WIDTH = 30;
		private static readonly  string SEPARATOR = "*".Repeat(TITLE_WIDTH);

		private static ShowWhere showWhere;

		private const int TYPE_WIDTH = -20;
		private const int START_MSG_WIDTH = 30;

		private const int LABLE_WIDTH = 28;
		private const string TAB_S = "    ";


		// location is variable

		public static void StartMsg(string msg1, string msg2=null,  ShowWhere where = ShowWhere.DEBUG)
		{
			showWhere = where;

			showMsg("\n\n");
			showMsgLine(SEPARATOR);
			showMsgLine($"*** {msg1,-1*(START_MSG_WIDTH-8)} ***");
			if (msg2!=null) showMsgLine($"*** {msg2,-1*(START_MSG_WIDTH-8)} ***");
			showMsgLine(SEPARATOR);
			showMsg("\n");
		}

		public static void ShowPdfInfo(List<PdfDocInfo> Docs)
		{

		}


		// location is to debug

		public static void ShowPageInfo(Rectangle ps, Rectangle psWrot, float rot)
		{
			Debug.WriteLine($"{"rotation",-TITLE_WIDTH} | {rot:F2}");
			Debug.WriteLine($"{"page size",-TITLE_WIDTH} | w {ps.GetWidth():F2} | h {ps.GetHeight():F2}");
			Debug.WriteLine($"{"page size w ro",-TITLE_WIDTH} | w {psWrot.GetWidth():F2} | h {psWrot.GetHeight():F2}");
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



		// utility

		public static void showMsgLine(string msg)
		{

			showMsg(msg+"\n");
		}

		public static void showMsg(string msg)
		{
			if (showWhere == ShowWhere.DEBUG || showWhere == ShowWhere.DBG_CONS)
			{
				Debug.Write(msg);
			}
			
			if (showWhere == ShowWhere.CONSOLE || showWhere == ShowWhere.DBG_CONS)
			{
				Console.Write(msg);
			}
		}



		// voided
		// public static void StartMsg(string msg1, string msg2=null)
		// {
		// 	Debug.Write("\n\n");
		// 	Debug.WriteLine(SEPARATOR);
		// 	Debug.WriteLine($"*** {msg1,-1*(TITLE_WIDTH-8)} ***");
		// 	if (msg2!=null) Debug.WriteLine($"*** {msg2,-1*(TITLE_WIDTH-8)} ***");
		// 	Debug.WriteLine(SEPARATOR);
		// 	Debug.Write("\n");
		// }


	}
}
