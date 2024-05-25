#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using SettingsManager;
using SharedCode.ShDataSupport;

using System.Text.Json;
using System.Text.Json.Serialization;
using Settings;
using ShCommonCode.ShSheetData;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   5/19/2024 9:05:41 AM

namespace ReadPDFTextTests.SheetData
{
	public class ProcessSheetBoxes
	{
		public const string SHEET_METRIC_FOLDER = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\";

		private const int TITLE_WIDTH = 24;
		private const int TYPE_WIDTH = -20;


		private ExtractSheetBoxes eb;
		private string DataFilePathJson = SHEET_METRIC_FOLDER + "SheetMetrics.json";
		private string DataFilePath { get; }= SHEET_METRIC_FOLDER + SheetDataSet.DataFileName;

		public void Process()
		{
			bool result = true;
			bool show = true;

			SheetDataManager.Init( new FilePath<FileNameSimple>(SheetDataSetConsts.DataFilePath));

			// ReadBoxes();

			result = scanBoxes();
			
			if (result) WriteBoxes();

			// if (result) WriteBoxesJson();

			showReport();

			if (SheetDataManager.SheetMetricsCount > 0
				&& show && result)
			{
				ShowSheetRectInfo.showShtRects();
				ShowSheetRectInfo.ShowValues();
			}

		}

		private bool scanBoxes()
		{
			eb=new ExtractSheetBoxes();

			Console.WriteLine("scanning sheets for boxes");

			startMsg("begin scan boxes");

			eb.PreProcess();
			
			// SheetMetricsSupport.ShowValues();

			Console.WriteLine(" done\n");

			return validate();
		}

		public void ReadBoxes()
		{
			Debug.WriteLine("sheet data");
			Debug.WriteLine($"exists (before) | {SheetDataManager.SettingsFileExists}");

			// SheetMetricsSupport.ReadSheetMetrics();
			SheetDataManager.Read();

			Debug.WriteLine($"exists (after)  | {SheetDataManager.SettingsFileExists}");

			Debug.WriteLine($"   sheet data found | {SheetDataManager.SheetMetricsCount}");
			// Debug.WriteLine($" sheet data A found | {SheetDataManager.SheetMetricsACount}");


			startMsg("read boxes");
		}

		// public void WriteBoxesJson()
		// {
		// 	ShtData.Data.SheetMetricsA = SheetMetricsSupport.Convert(ShtData.Data.SheetMetrics);
		//
		// 	JsonSerializerOptions options = new JsonSerializerOptions();
		// 	options.WriteIndented=true;
		//
		// 	// string json = JsonSerializer.Serialize(ShtData.Data.SheetMetricsA, options);
		// 	string json = JsonSerializer.Serialize(ShtData.Data.SheetMetrics, options);
		// 	File.WriteAllText(DataFilePathJson, json);
		// }

		public void WriteBoxes()
		{
			Console.WriteLine("Sheet Metric Data Saved\n");

			SheetDataManager.Write();
		}

		private bool validate()
		{
			bool result = true;

			result = eb.duplicates.Count == 0;

			result &= eb.extras.Count == 0;

			foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.Data.SheetRectangles)
			{
				result &= kvp.Value.AllShtRectsFound;
			}

			return result;
		}

		private void showReport()
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

			if (eb == null) return;

			if (eb.duplicates.Count > 0)
			{
				showDuplicates();
				result = false;
			}

			if (eb.extras.Count > 0)
			{
				showExtras();
				result = false;
			}

			if (!result)
			{
				Console.WriteLine("\ndue to issues, the sheet metrics data will not be saved\n");
			}

			Console.Write("\n");
		}

		private void showDuplicates()
		{
			Console.WriteLine($"\nduplicate boxes found | {eb.duplicates.Count}\n");

			foreach (Tuple<string, string, Rectangle> dups in eb.duplicates)
			{
				Console.WriteLine($"\tfile {dups.Item1,-20} | name {dups.Item2,-20} | location {dups.Item3.GetX():F2}, {dups.Item3.GetY():F2}");
			}

			// Console.WriteLine("\nplease eliminate the duplicate boxes and try again\n");
		}

		private void showExtras()
		{
			Console.WriteLine($"\nextra boxes found | {eb.extras.Count}\n");

			foreach (Tuple<string, string, Rectangle> xtra in eb.extras)
			{
				Console.WriteLine($"\tfile {xtra.Item1,-20} | name {xtra.Item2,-20} | location {xtra.Item3.GetX():F2}, {xtra.Item3.GetY():F2}");
			}

			// Console.WriteLine("\nplease eliminate the extra boxes and try again\n");
		}

		/* 		private void showShtMetrics()
		{
			if (SheetDataManager.Data.SheetMetrics == null || SheetDataManager.Data.SheetMetrics.Count == 0)
			{
				Debug.WriteLine("no metric data");
				return;
			}


			foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.Data.SheetMetrics)
			{
				int missing;

				Debug.WriteLine($"for {kvp.Key}");

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
				
				foreach (KeyValuePair<SheetMetricId, SheetRectData<SheetMetricId>> kvp2 in kvp.Value.ShtRects)
				{
					string name = SheetRectSupport.GetShtRectName(kvp2.Key)!;
					string type = kvp2.Value.Type.ToString();

					Debug.WriteLine($"{name,TITLE_WIDTH}| {type,TYPE_WIDTH}| {CsItextHelpers.FormatRect(kvp2.Value.Rect)}");
				}

				Debug.Write("\n");
			}
		}
*/

		public override string ToString()
		{
			return $"this is {nameof(ProcessSheetBoxes)}";
		}

		private void startMsg(string msg)
		{
			Debug.WriteLine($"\n\n{"*".Repeat(30)}");
			Debug.WriteLine($"*** {msg,-22} ***");
			Debug.WriteLine($"{"*".Repeat(30)}\n");
			Debug.WriteLine($"{DateTime.Now}\n");
		}

		public static Rectangle convertCoordToPage(SheetRects sm, Rectangle r)
		{
			Rectangle result;
			Rectangle page = sm.PageSizeWithRotation;

			float x = r.GetX() / 72f;
			float y = (page.GetHeight() - r.GetY() - r.GetHeight()) / 72f;
			float w = r.GetWidth() / 72f;
			float h = r.GetHeight() / 72f;

			return new Rectangle(x, y, w, h);

		}
	}
}
