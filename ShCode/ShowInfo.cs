#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using iText.Kernel.Geom;
using ShSheetData.SheetData;
using UtilityLibrary;
using ShowWhere = ShTempCode.DebugCode.ShowWhere;

#endregion

// user name: jeffs
// created:   6/13/2024 12:31:48 PM

namespace ShCode
{
	// public enum ShowWhere
	// {
	// 	DEBUG=0,
	// 	CONSOLE=1,
	// 	DBG_CONS=2
	// }

	public class ShowInfo
	{
		private const int TITLE_WIDTH = 36;
		private static readonly  string SEPARATOR = "*".Repeat(TITLE_WIDTH);

		private static ShowWhere showWhere;

		private const int TYPE_WIDTH = -20;
		private const int START_MSG_WIDTH = 36;

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

		public static void showShtRects(ShowWhere where)
		{
			showWhere = where;

			if (SheetDataManager.Data.SheetRectangles == null || SheetDataManager.Data.SheetRectangles.Count == 0)
			{
				return;
			}

			foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.Data.SheetRectangles)
			{
				int missing;

				showMsgLine($"\n\nfor {kvp.Key}");

				showMsg($"{"sheet rectangles",TITLE_WIDTH}| found {kvp.Value.ShtRects.Count}");

				missing = SheetRectSupport.ShtRectsQty - kvp.Value.ShtRects.Count;

				if (missing > 0)
				{
					showMsgLine($" | missing {missing}");
				}
				else
				{
					showMsg("\n");
				}

				showMsgLine($"{"optional rectangles",TITLE_WIDTH}| found {kvp.Value.OptRects.Count}");

				foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp2 in kvp.Value.ShtRects)
				{
					showMsgLine(formatSingleRect(kvp2));
				}

				showMsg("\n");
				
				foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp2 in kvp.Value.OptRects)
				{
					showMsgLine(formatSingleRect(kvp2));
				}

				showMsg("\n");
			}
		}

		public static void ShowValues(ShowWhere where)
		{
			showWhere = where;

			foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.Data.SheetRectangles)
			{
				showMsgLine($"sheet name| {kvp.Key}");

				foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp2 in kvp.Value.ShtRects)
				{
					showMsgLine($"\n{kvp2.Key}");

					showBoxValues(kvp2.Value);
				}

				foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp3 in kvp.Value.OptRects)
				{
					showMsgLine($"\n {kvp3.Key}");

					showBoxValues(kvp3.Value);
				}
			}
		}

		private static void showBoxValues(SheetRectData<SheetRectId> box)
		{
			showMsgLine($"{$"{TAB_S}box data",-LABLE_WIDTH}| ");
			showMsgLine($"{$"{TAB_S}{TAB_S}box id",-LABLE_WIDTH}| {box.Id}");
			showMsgLine($"{$"{TAB_S}{TAB_S}box type",-LABLE_WIDTH}| {box.Type}");
			showMsgLine($"{$"{TAB_S}{TAB_S}rectangle",-LABLE_WIDTH}| {FormatItextData.FormatRectangle(box.Rect, false)}");
			showMsgLine($"{$"{TAB_S}{TAB_S}rotation",-LABLE_WIDTH}| {box.TextBoxRotation:F2}");

			if (box.Id == SheetRectId.SM_XREF)
			{
				showBoundingBoxValues(box);
				return;
			}

			if (box.Type == SheetRectType.SRT_NA ||
				box.Type == SheetRectType.SRT_LOCATION
				) return;

			showMsg($"{$"{TAB_S}bounding box info",-LABLE_WIDTH}| ");

			if (box.HasType(SheetRectType.SRT_BOX))
			{
				showMsg("\n");
				showBoundingBoxValues(box);
			}
			else
			{
				showMsgLine("n/a");
			}

			showMsg($"{$"{TAB_S}link info",-LABLE_WIDTH}| ");

			if (box.HasType(SheetRectType.SRT_LINK))
			{
				showMsg("\n");
				showMsgLine($"{$"{TAB_S}{TAB_S}UrlLink",-LABLE_WIDTH}| {box.UrlLink}");

			}
			else
			{
				showMsgLine("n/a");
			}

			showMsg($"{$"{TAB_S}text info",-LABLE_WIDTH}| ");

			if (box.HasType(SheetRectType.SRT_TEXT))
			{
				showMsg("\n");
				showTextValues(box);
			}
			else
			{
				showMsgLine("n/a");
			}
		}

		private static void showBoundingBoxValues(SheetRectData<SheetRectId> box)
		{
			
			showMsgLine($"{$"{TAB_S}{TAB_S}FillColor",-LABLE_WIDTH}| {FormatItextData.FormatColor(box.FillColor, false)}");
			showMsgLine($"{$"{TAB_S}{TAB_S}FillOpacity",-LABLE_WIDTH}| {box.FillOpacity}");
			showMsgLine($"{$"{TAB_S}{TAB_S}BdrWidth",-LABLE_WIDTH}| {box.BdrWidth}");
			showMsgLine($"{$"{TAB_S}{TAB_S}BdrColor",-LABLE_WIDTH}| {FormatItextData.FormatColor(box.BdrColor, false)}");
			showMsgLine($"{$"{TAB_S}{TAB_S}BdrOpacity",-LABLE_WIDTH}| {box.BdrOpacity}");
			showMsgLine($"{$"{TAB_S}{TAB_S}BdrDashPattern",-LABLE_WIDTH}| {FormatItextData.FormatDashArray(box.BdrDashPattern)}");

		}

		private static void showTextValues(SheetRectData<SheetRectId> box)
		{
			showMsgLine($"{$"{TAB_S}{TAB_S}InfoText",-LABLE_WIDTH}| {box.InfoText }");
			showMsgLine($"{$"{TAB_S}{TAB_S}FontFamily",-LABLE_WIDTH}| {box.FontFamily }");
			showMsgLine($"{$"{TAB_S}{TAB_S}FontStyle",-LABLE_WIDTH}| {FormatItextData.FormatFontStyle(box.FontStyle)}");
			showMsgLine($"{$"{TAB_S}{TAB_S}TextSize",-LABLE_WIDTH}| {box.TextSize }");
			showMsgLine($"{$"{TAB_S}{TAB_S}TextHorizAlignment",-LABLE_WIDTH}| {box.TextHorizAlignment }");
			showMsgLine($"{$"{TAB_S}{TAB_S}TextVertAlignment",-LABLE_WIDTH}| {box.TextVertAlignment }");
			showMsgLine($"{$"{TAB_S}{TAB_S}TextWeight",-LABLE_WIDTH}| {box.TextWeight}");
			showMsgLine($"{$"{TAB_S}{TAB_S}TextDecoration",-LABLE_WIDTH}| {TextDecorations.FormatTextDeco(box.TextDecoration)}");
			showMsgLine($"{$"{TAB_S}{TAB_S}TextColor",-LABLE_WIDTH}| {FormatItextData.FormatColor(box.TextColor, false)}");
			showMsgLine($"{$"{TAB_S}{TAB_S}TextOpacity",-LABLE_WIDTH}| {box.TextOpacity }");

		}


		// utility

		private static string formatSingleRect(KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp2, Rectangle pageSize = null)
		{
			string name = SheetRectSupport.GetShtRectName(kvp2.Key);

			if (name.IsVoid())
			{
				name = SheetRectSupport.GetOptRectName(kvp2.Key);
			}

			string rotation = $"{kvp2.Value.TextBoxRotation,8:F2}";

			string type = kvp2.Value.Type.ToString();

			Rectangle r = kvp2.Value.Rect;

			float oaWidth = r.GetX()+r.GetWidth();
			float oaHeight = r.GetY()+r.GetHeight();

			string oa = null;

			if (pageSize != null)
			{
				float wDiff = pageSize.GetWidth() - oaWidth;
				float hDiff = pageSize.GetHeight() - oaHeight;

				oa = $" | oaW| {oaWidth,8:F2}  wDif {$"({wDiff:F2})",10} | oaH {oaHeight,8:F2}  hDif {$"({hDiff:F2})",10}";
			}

			return $"{name,TITLE_WIDTH}| {type,TYPE_WIDTH}| {FormatItextData.FormatRectangle(kvp2.Value.Rect)} ({rotation}°)  {oa}";
		}

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

	}
}
