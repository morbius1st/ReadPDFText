#region + Using Directives

#endregion

// user name: jeffs
// created:   5/12/2024 8:18:57 AM

namespace ReadPDFTextTests
{
	/*
	public class ProcessBoxes
	{
		private const int TITLE_WIDTH = 20;

		private ScanBoxes sb;

		public const string SHEET_METRIC_FOLDER = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\";

		public string DataFilePath { get; }= ProcessBoxes.SHEET_METRIC_FOLDER + SheetDataSet.DataFileName;

		public void Process()
		{
			bool result;

			SheetRectSupport.FilePath = new FilePath<FileNameSimple>(SheetDataSetConsts.DataFilePath);

			SheetDataManager.Init(SheetRectSupport.FilePath);

			// ReadBoxes();

			result=CreateBoxes();
			
			// showReport();
			
			// if(result) SaveBoxes();
		}

		private void SaveBoxes()
		{
			Console.WriteLine("Sheet Metric Data Saved\n");

			SheetDataManager.Write();
		}

		public bool CreateBoxes()
		{
			sb = new ScanBoxes();

			Console.Write("scanning for boxes ");

			sb.process();

			Console.WriteLine(" done\n");

			return validate();
		}

		public void ReadBoxes()
		{

			Debug.WriteLine("sheet data");
			Debug.WriteLine($"exists (before) | {SheetDataManager.SettingsFileExists}");

			SheetDataManager.Read();

			Debug.WriteLine($"exists (after)  | {SheetDataManager.SettingsFileExists}");

			Debug.WriteLine($"   sheet data found | {SheetDataManager.SheetMetricsCount}");
			// Debug.WriteLine($" sheet data A found | {SheetDataManager.SheetMetricsACount}");

			if (SheetDataManager.SheetMetricsCount > 0) showShtMetrics();

		}

		private bool validate()
		{
			bool result = true;

			result = sb.duplicates.Count == 0;

			result &= sb.extras.Count == 0;

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

			Console.WriteLine($"\ncreate sheet metrics report");

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

			if (sb.duplicates.Count > 0)
			{
				showDuplicates();
				result = false;
			}

			if (sb.extras.Count > 0)
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
			Console.WriteLine($"\nduplicate boxes found | {sb.duplicates.Count}\n");

			foreach (Tuple<string, string, Rectangle> dups in sb.duplicates)
			{
				Console.WriteLine($"\tfile {dups.Item1,-20} | name {dups.Item2,-20} | location {dups.Item3.GetX():F2}, {dups.Item3.GetY():F2}");
			}

			// Console.WriteLine("\nplease eliminate the duplicate boxes and try again\n");
		}

		private void showExtras()
		{
			Console.WriteLine($"\nextra boxes found | {sb.extras.Count}\n");

			foreach (Tuple<string, string, Rectangle> xtra in sb.extras)
			{
				Console.WriteLine($"\tfile {xtra.Item1,-20} | name {xtra.Item2,-20} | location {xtra.Item3.GetX():F2}, {xtra.Item3.GetY():F2}");
			}

			// Console.WriteLine("\nplease eliminate the extra boxes and try again\n");
		}

		private void showShtMetrics()
		{
			if (SheetDataManager.Data.SheetRectangles == null || SheetDataManager.Data.SheetRectangles.Count == 0)
			{
				Debug.WriteLine("no metric data");
				return;
			}


			foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.Data.SheetRectangles)
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
				
				foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp2 in kvp.Value.ShtRects)
				{
					string name = SheetRectSupport.GetShtRectName(kvp2.Key)!;
					string type = kvp2.Value.Type.ToString();

					Debug.WriteLine($"{name,TITLE_WIDTH}| {type,-18}| {FormatItextData.FormatRectangle(kvp2.Value.Rect)}");
				}

				Debug.Write("\n");
			}
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

		public override string ToString()
		{
			return $"this is {nameof(ProcessBoxes)}";
		}
	}

	*/
}
