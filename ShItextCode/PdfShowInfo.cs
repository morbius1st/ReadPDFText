#region + Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using DebugCode;
using iText.Kernel.Geom;
using iText.Layout.Properties;
using ShItextCode;
using ShSheetData.SheetData;
using ShSheetData.Support;

using UtilityLibrary;
using ShowWhere = UtilityLibrary.ShowWhere;

#endregion

// user name: jeffs
// created:   5/23/2024 7:01:59 PM



namespace ShItextCode
{

	// public enum ShowWhere
	// {
	// 	DEBUG = 0,
	// 	CONSOLE = 1,
	// 	DBG_CONS = 2
	// }

	public static class PdfShowInfo
	{
		private const int TITLE_WIDTH = 36;

		private static ShowWhere showWhere;

		private const int TYPE_WIDTH = -20;
		private const int START_MSG_WIDTH = 50;

		private static readonly  string SEPARATOR = "*".Repeat(START_MSG_WIDTH);

		private const int LABLE_WIDTH = 28;
		private const string TAB_S = "    ";


		// location is variable

		public static void StartMsg(string msg1, string msg2 = null,  ShowWhere where = ShowWhere.DEBUG)
		{
			showWhere = where;

			showMsg("\n\n");
			showMsgLine(SEPARATOR);
			showMsgLine($"*** {msg1,-1 * (START_MSG_WIDTH - 8)} ***");
			if (msg2 != null) showMsgLine($"*** {msg2,-1 * (START_MSG_WIDTH - 8)} ***");
			showMsgLine(SEPARATOR);
			showMsg("\n");
		}

		public static void ShowPdfInfoBasic(List<PdfDocInfo> docs, ShowWhere where = ShowWhere.DEBUG)
		{
			StartMsg("Show Pdf Doc Info", DateTime.Now.ToString(), where);

			showWhere = where;
			string s;
			PdfPageInfo p;

			foreach (PdfDocInfo di in docs)
			{
				showMsgLine($"{di.Name}");

				for (int i = 1; i < di.PageInfo.Count + 1; i++)
				{
					p = di.PageInfo[i];

					showMsgLine($"{TAB_S}page {i}");
					showMsgLine($"{$"{TAB_S}{TAB_S}rotation",-TITLE_WIDTH} {p.PageRotation}");
					showMsgLine($"{$"{TAB_S}{TAB_S}pg size",-TITLE_WIDTH} {p.PageSize.GetWidth()}, {p.PageSize.GetHeight()}");
					showMsgLine($"{$"{TAB_S}{TAB_S}pg size w/rotate",-TITLE_WIDTH} {p.PageSizeWithRotation.GetWidth()}, {p.PageSizeWithRotation.GetHeight()}");
				}

				showMsg("\n");
			}

			showMsgLine("Done\n");
		}

		public static void ShowPdfInfoEx(List<PdfDocInfo> docs, ShowWhere where = ShowWhere.DEBUG)
		{
			StartMsg("Show Pdf Doc Info", DateTime.Now.ToString(), where);

			showWhere = where;
			string s;
			PdfPageInfo p;

			foreach (PdfDocInfo di in docs)
			{
				showMsgLine($"{di.Name}");

				for (int i = 1; i < di.PageInfo.Count + 1; i++)
				{
					p = di.PageInfo[i];

					DM.DbxLineEx(1, $"page {i}",0,1);

					DM.DbxLineEx(1, $"{"doc info| name",-20} {di.Name}");
					DM.DbxLineEx(1, $"{"doc info| author",-20} {di.Author}");
					DM.DbxLineEx(1, $"{"doc info| c date",-20} {di.CreationData}");
					DM.DbxLineEx(1, $"{"doc info| creator",-20} {di.Creator}");
					DM.DbxLineEx(1, "");
					
					DM.DbxLineEx(1, $"{"page rotation",-20} {p.PageRotation}");
					DM.DbxLineEx(1, $"{"page size",-20} {p.PageSize}");
					DM.DbxLineEx(1, $"{"page size w/ rotation",-20} {p.PageSizeWithRotation}");

					DM.DbxLineEx(1, $"",-2,0);
				}

				showMsg("\n");
			}

			showMsgLine("Done\n");
		}


		// location is to debug

		public static void ShowPageInfo(Rectangle ps, Rectangle psWrot, float rot)
		{
			Debug.WriteLine($"{"rotation",-TITLE_WIDTH} | {rot:F2}");
			Debug.WriteLine($"{"page size",-TITLE_WIDTH} | w {ps.GetWidth():F2} | h {ps.GetHeight():F2}");
			Debug.WriteLine($"{"page size w ro",-TITLE_WIDTH} | w {psWrot.GetWidth():F2} | h {psWrot.GetHeight():F2}");
		}

		// public static void ShowRectParams(SheetRectData<SheetRectId> pStr)
		// {
		// 	Debug.Write("\n");
		// 	string temp = FormatItextData.FormatRectangle(pStr.Rect);
		// 	Debug.WriteLine($"{"rectangle",-TITLE_WIDTH} | {temp}");
		//
		// 	temp = FormatItextData.FormatColor(pStr.BdrColor);
		// 	Debug.WriteLine($"{"bdr color",-TITLE_WIDTH} | [ {temp} ]");
		// 	Debug.WriteLine($"{"bdr opacity",-TITLE_WIDTH} | {pStr.BdrOpacity}");
		// 	Debug.WriteLine($"{"bdr width",-TITLE_WIDTH} | {pStr.BdrWidth}");
		//
		// 	temp = FormatItextData.FormatDashArray(pStr.BdrDashPattern);
		// 	Debug.WriteLine($"{"bdr width",-TITLE_WIDTH} | {temp}");
		//
		// 	temp = FormatItextData.FormatColor(pStr.FillColor);
		// 	Debug.WriteLine($"{"fill color",-TITLE_WIDTH} | [ {temp} ]");
		// 	Debug.WriteLine($"{"fill opacity",-TITLE_WIDTH} | {pStr.FillOpacity}");
		//
		// }



		// utility

		public static void showMsgLine(string msg)
		{

			showMsg(msg + "\n");
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


		public static void showSinAndCos()
		{
			Debug.WriteLine("");
			Debug.WriteLine("sin and cos angles");

			Debug.WriteLine("\nstd math");
			Debug.WriteLine($"for angle {0,5:F0} | sin {Math.Sin(FloatOps.ToRad(0)),6:F0} cos {Math.Cos(FloatOps.ToRad(0)),6:F0}");
			Debug.WriteLine($"for angle {90,5:F0} | sin {Math.Sin(FloatOps.ToRad(90)),6:F0} cos {Math.Cos(FloatOps.ToRad(90)),6:F0}");
			Debug.WriteLine($"for angle {180,5:F0} | sin {Math.Sin(FloatOps.ToRad(180)),6:F0} cos {Math.Cos(FloatOps.ToRad(180)),6:F0}");
			Debug.WriteLine($"for angle {270,5:F0} | sin {Math.Sin(FloatOps.ToRad(270)),6:F0} cos {Math.Cos(FloatOps.ToRad(270)),6:F0}");

			Debug.WriteLine("\nfloat (float) Math");
			Debug.WriteLine($"for angle {0,5:F0} | sin {(float) Math.Sin(FloatOps.ToRad(0)),6:F0} cos {(float) Math.Cos(FloatOps.ToRad(0)),6:F0}");
			Debug.WriteLine($"for angle {90,5:F0} | sin {(float) Math.Sin(FloatOps.ToRad(90)),6:F0} cos {(float) Math.Cos(FloatOps.ToRad(90)),6:F0}");
			Debug.WriteLine($"for angle {180,5:F0} | sin {(float) Math.Sin(FloatOps.ToRad(180)),6:F0} cos {(float) Math.Cos(FloatOps.ToRad(180)),6:F0}");
			Debug.WriteLine($"for angle {270,5:F0} | sin {(float) Math.Sin(FloatOps.ToRad(270)),6:F0} cos {(float) Math.Cos(FloatOps.ToRad(270)),6:F0}");

		}

		public static void showAlignment()
		{
			Debug.WriteLine("\nHorizontalAlignment");
			Debug.WriteLine($"\t{"left",-12} {(int) HorizontalAlignment.LEFT}");
			Debug.WriteLine($"\t{"center",-12} {(int) HorizontalAlignment.CENTER}");
			Debug.WriteLine($"\t{"right",-12} {(int) HorizontalAlignment.RIGHT}");

			Debug.WriteLine("\nVerticalAlignment");
			Debug.WriteLine($"\t{"top",-12} {(int) VerticalAlignment.TOP}");
			Debug.WriteLine($"\t{"middle",-12} {(int) VerticalAlignment.MIDDLE}");
			Debug.WriteLine($"\t{"bottom",-12} {(int) VerticalAlignment.BOTTOM}");
			
			Debug.WriteLine("\nTextAlignment");
			Debug.WriteLine($"\t{"left",-12} {(int) TextAlignment.LEFT}");
			Debug.WriteLine($"\t{"center",-12} {(int) TextAlignment.CENTER}");
			Debug.WriteLine($"\t{"right",-12} {(int) TextAlignment.RIGHT}");
		}

	}


}