#region + Using Directives
using Settings;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using ShCommonCode.ShSheetData;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   5/24/2024 8:05:30 PM

namespace ScanPDFBoxes.Process
{
	public class ProcessManager
	{
		public List<Tuple<string, string, Rectangle>> duplicates { get; private set; }
		public List<Tuple<string, string, Rectangle>> extras { get; private set; }
		public List<Tuple<string?, string?, Rectangle?>> fails { get; private set; }

		public ProcessManager(FilePath<FileNameSimple> dataFile)
		{
			DataFilePath = dataFile;
		}

		public FilePath<FileNameSimple> DataFilePath { get; set; }


		public void ResetSheetData()
		{
			SheetDataManager.Init(DataFilePath);

			SheetDataManager.Write();
		}

		private void configSheetData()
		{
			if (!SheetDataManager.Initialized)
			{
				ResetSheetData();
			}

			SheetDataManager.Read();
		}

		public bool RemoveSheets(string[] sheets)
		{
			int initCount = SheetDataManager.SheetsCount;

			fails = new List<Tuple<string?, string?, Rectangle?>>();

			configSheetData();

			for (var i = 0; i < sheets.Length; i++)
			{
				removeSheet(sheets[i]);
			}

			if (true)
			{
				ProcessResults pr = new ProcessResults(this);

				pr.ProcessRemove(initCount);
			}

			return fails.Count > 0;
		}

		public bool ScanSheets(string[] sheets)
		{
			// Debug.WriteLine($"@1 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");

			duplicates = new List<Tuple<string, string, Rectangle>>();
			extras = new List<Tuple<string, string, Rectangle>>();

			configSheetData();

			int count = SheetDataManager.SheetsCount;

			scanSheets(sheets);

			// Debug.WriteLine($"@9 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");

			Console.WriteLine(" done\n");

			writeData();

			if (true)
			{
				ProcessResults pr = new ProcessResults(this);

				pr.ShowBasicRects = true;
				pr.ShowRectValues = false;
				pr.ShowReport=true;

				pr.ProcessAdd();
			}

			return SheetDataManager.SheetsCount - count > 0;
		}

		private void scanSheets(string[] sheets)
		{
			ScanSheet ss = new ScanSheet(this);

			for (var i = 0; i < sheets.Length; i++)
			{
				Console.Write(".");

				// Debug.WriteLine($"@11 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");

				ss.ProcessSheet(sheets[i]);

				// Debug.WriteLine($"@19 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");
			}
		}

		private bool writeData()
		{
			bool result = true;

			SheetDataManager.Write();

			return result;
		}

		private void removeSheet(string sheet)
		{
			bool result = SheetDataManager.Data!.SheetRectangles.Remove(sheet);

			if (result) return;

			fails.Add(new Tuple<string?, string?, Rectangle?>(sheet, "Could not find / remove sheet", null));

		}
	}
}
