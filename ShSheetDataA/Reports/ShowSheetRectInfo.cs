#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using ScanPDFBoxes.Process;

using Settings;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   5/25/2024 7:05:05 AM

namespace ShCommonCode.ShSheetData
{
	public class ShowSheetRectInfo
	{
		private const int TITLE_WIDTH = 24;
		private const int TYPE_WIDTH = -20;

		public static void showShtRects()
		{
			if (SheetDataManager.Data.SheetRectangles == null || SheetDataManager.Data.SheetRectangles.Count == 0)
			{
				return;
			}

			foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.Data.SheetRectangles)
			{
				int missing;

				Debug.WriteLine($"\n\nfor {kvp.Key}");

				Debug.Write($"{"sheet rectangles",TITLE_WIDTH}| found {kvp.Value.ShtRects.Count}");

				missing = SheetRectSupport.ShtRectsQty - kvp.Value.ShtRects.Count;

				if (missing > 0)
				{
					Debug.WriteLine($" | missing {missing}");
				}
				else
				{
					Debug.Write("\n");
				}

				Debug.WriteLine($"{"optional rectangles",TITLE_WIDTH}| found {kvp.Value.OptRects.Count}");

				foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp2 in kvp.Value.ShtRects)
				{
					string name = SheetRectSupport.GetShtRectName(kvp2.Key)!;
					string type = kvp2.Value.Type.ToString();

					Debug.WriteLine($"{name,TITLE_WIDTH}| {type,TYPE_WIDTH}| {FormatItextData.FormatRectangle(kvp2.Value.Rect)}");
				}

				Debug.Write("\n");
			}
		}

		public static void ShowValues()
		{
			foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.Data.SheetRectangles)
			{
				Debug.WriteLine($"sheet name| {kvp.Key}");

				foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp2 in kvp.Value.ShtRects)
				{
					Debug.WriteLine($"\n{kvp2.Key}");

					showBoxValues(kvp2.Value);
				}

				foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp3 in kvp.Value.OptRects)
				{
					Debug.WriteLine($"\n {kvp3.Key}");

					showBoxValues(kvp3.Value);
				}
			}
		}

		private static void showBoxValues(SheetRectData<SheetRectId> box)
		{
			Debug.WriteLine($"\tbox data                | ");
			Debug.WriteLine($"\t\tbox id              | {box.Id}");
			Debug.WriteLine($"\t\tbox type            | {box.Type}");
			Debug.WriteLine($"\t\trectangle           | {FormatItextData.FormatRectangle(box.Rect)}");
			Debug.WriteLine($"\t\tRotation            | {box.Rotation }");

			if (box.Id == SheetRectId.SM_XREF)
			{
				showBoundingBoxValues(box);
				return;
			}

			if (box.Type == SheetRectType.SRT_NA ||
				box.Type == SheetRectType.SRT_LOCATION
				) return;


			Debug.  Write($"\tbounding box info       | ");
			if (box.HasType(SheetRectType.SRT_BOX))
			{
				Debug.Write("\n");
				showBoundingBoxValues(box);
			}
			else
			{
				Debug.WriteLine("n/a");
			}

			Debug.Write($"\tlink info               | ");
			if (box.HasType(SheetRectType.SRT_LINK))
			{
				Debug.Write("\n");
				Debug.WriteLine($"\t\tUrlLink             | {box.UrlLink }");
			}
			else
			{
				Debug.WriteLine("n/a");
			}


			Debug.Write($"\ttext info               | ");
			if (box.HasType(SheetRectType.SRT_TEXT))
			{
				Debug.Write("\n");
				showTextValues(box);
			}
			else
			{
				Debug.WriteLine("n/a");
			}
		}

		private static void showBoundingBoxValues(SheetRectData<SheetRectId> box)
		{
			Debug.WriteLine($"\t\tFillColor           | {FormatItextData.FormatColor(box.FillColor)}");
			Debug.WriteLine($"\t\tFillOpacity         | {box.FillOpacity}");
			Debug.WriteLine($"\t\tBdrWidth            | {box.BdrWidth }");
			Debug.WriteLine($"\t\tBdrColor            | {FormatItextData.FormatColor(box.BdrColor)}");
			Debug.WriteLine($"\t\tBdrOpacity          | {box.BdrOpacity }");
			Debug.WriteLine($"\t\tBdrDashPattern      | {FormatItextData.FormatDashArray(box.BdrDashPattern)}");
		}

		private static void showTextValues(SheetRectData<SheetRectId> box)
		{
			Debug.WriteLine($"\t\tInfoText            | {box.InfoText }");
			Debug.WriteLine($"\t\tFontFamily          | {box.FontFamily }");
			Debug.WriteLine($"\t\tFontStyle           | {FormatItextData.FormatFontStyle(box.FontStyle)}");
			Debug.WriteLine($"\t\tTextSize            | {box.TextSize }");
			Debug.WriteLine($"\t\tTextHorizAlignment  | {box.TextHorizAlignment }");
			Debug.WriteLine($"\t\tTextVertAlignment   | {box.TextVertAlignment }");
			Debug.WriteLine($"\t\tTextWeight          | {box.TextWeight}");
			Debug.WriteLine($"\t\tTextDecoration      | {TextDecorations.FormatTextDeco(box.TextDecoration)}");
			Debug.WriteLine($"\t\tTextColor           | {FormatItextData.FormatColor(box.TextColor)}");
			Debug.WriteLine($"\t\tTextOpacity         | {box.TextOpacity }");
		}

		public static void showScanRectReport(ProcessManager pm)
		{
			bool result = true;
			string temp;

			Console.WriteLine($"\nscan sheet boxes report");

			foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.Data.SheetRectangles)
			{
				Console.WriteLine($"\nfor | {kvp.Value.Name}");

				if (kvp.Value.AllShtRectsFound)
				{
					Console.WriteLine($"\tall boxes found");
				}
				else
				{
					Console.WriteLine($"\tthese boxes are missing");

					result = false;

					foreach (KeyValuePair<string, SheetRectInfo<SheetRectId>> kvp2 in SheetRectSupport.ShtRectIdXref)
					{
						if (kvp2.Value.Id == SheetRectId.SM_NA) continue;

						if (!kvp.Value.ShtRects.ContainsKey(kvp2.Value.Id))
						{
							string name = SheetRectSupport.GetShtRectName(kvp2.Value.Id) ?? "no name";

							Console.WriteLine($"\t{name}");
						}
					}

					Debug.Write("\n");
				}

				if (kvp.Value.OptRects.Count == 0)
				{
					Console.WriteLine($"\tno optional boxes found");
				}
				else
				{
					if (kvp.Value.OptRects.Count == 1)
					{
						Console.WriteLine($"\t{kvp.Value.OptRects.Count} optional box found");
					}
					else
					{
						Console.WriteLine($"\t{kvp.Value.OptRects.Count} optional boxes found");
					}
					
				}
			}

			showDuplicates(pm);

			showExtras(pm);

		}

		private static void showDuplicates(ProcessManager pm)
		{
			if (pm.duplicates.Count == 0)
			{
				Console.WriteLine($"\nno duplicate boxes found | {pm.duplicates.Count}\n");
				return;
			}

			Console.WriteLine($"\nduplicate boxes found | {pm.duplicates.Count}\n");

			foreach (Tuple<string, string, Rectangle> dups in pm.duplicates)
			{
				Console.WriteLine($"\tfile {dups.Item1,-20} | name {dups.Item2,-20} | location {dups.Item3.GetX():F2}, {dups.Item3.GetY():F2}");
			}

			// Console.WriteLine("\nplease eliminate the duplicate boxes and try again\n");
		}

		private static void showExtras(ProcessManager pm)
		{
			if (pm.extras.Count == 0)
			{
				Console.WriteLine($"\nno extra boxes found | {pm.extras.Count}\n");
				return;
			}

			Console.WriteLine($"\nextra boxes found | {pm.extras.Count}\n");

			foreach (Tuple<string, string, Rectangle> xtra in pm.extras)
			{
				Console.WriteLine($"\tfile {xtra.Item1,-20} | name {xtra.Item2,-20} | location {xtra.Item3.GetX():F2}, {xtra.Item3.GetY():F2}");
			}

			// Console.WriteLine("\nplease eliminate the extra boxes and try again\n");
		}

		public static void ShowRemoveReport(ProcessManager pm, int beginCount)
		{
			int finalCount = SheetDataManager.SheetsCount;
			Console.WriteLine($"Initial count {beginCount} | final count {finalCount} | removed {beginCount - finalCount}");

			if (pm.fails.Count == 0)
			{
				Console.WriteLine("All good - no error encountered");
				return;
			}

			if ( pm.fails.Count == 1)
			{
				Console.WriteLine("An error was encountered|");
			}
			else
			{
				Console.WriteLine("Some errors were encountered|");
			}

			foreach (Tuple<string, string, Rectangle> fail in pm.fails)
			{
				Console.WriteLine($"file {fail.Item1} | issue {fail.Item2}");
			}

		}
	}
}