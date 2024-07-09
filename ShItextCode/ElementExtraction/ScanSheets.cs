using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using ShItextCode.ElementExtraction;
using ShSheetData.SheetData2;
using ShSheetData.ShSheetData2;
using ShTempCode.DebugCode;
using Path = System.IO.Path;
using ShSheetData.SheetData;
using System.Diagnostics;
using System.IO;


// Solution:     ReadPDFText
// Project:       ReadPDFTextTests
// File:             ScanSheets.cs
// Created:      2024-06-24 (8:57 PM)

namespace ShItextCode.ElementExtraction
{
	public class ScanSheets
	{
		int startDupCount;
		int startExtraCount;

		int dupsAdded;
		int extrasAdded;

		/// <summary>scan a list of PDF and extract their 
		/// metrics and text information
		/// </summary>
		public bool Process(List<string> sheets)
		{
			DM.DbxLineEx(0, "start", 1);

			// ss = new ScanStatus();

			int count = sheets.Count;

			scanSheets(sheets);

			Console.WriteLine(" done\n");

			showScanReport();

			showScanErrReport();

			if (!processStatus())
			{
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			DM.DbxLineEx(0, "end", 0, -1);

			Console.WriteLine($"{SheetDataManager2.SheetsCount - count} >= 0? | {ScanStatus.ErrCount} == 0? | {!ScanStatus.HasFatalErrors} | all must be true");

			return SheetDataManager2.SheetsCount - count >= 0 && ScanStatus.ErrCount == 0 && !ScanStatus.HasFatalErrors;
		}

		private bool processStatus()
		{
			DM.DbxLineEx(0, "start", 1);

			if (ScanStatus.ErrCount > 0)
			{
				Console.WriteLine("\nDue to errors, changes will not be saved");

				SheetDataManager2.Read();

				DM.DbxLineEx(0, $"end 1 (error count {ScanStatus.ErrCount})", 0, -1);

				return false;
			}

			SheetDataManager2.Write();
			Console.WriteLine("\nData Written");

			DM.DbxLineEx(0, "end", 0, -1);

			return true;
		}

		private void scanSheets(List<string> sheets)
		{
			DM.DbxLineEx(0, "start", 1);

			string fileName;

			ScanPdf scan = new ScanPdf();

			SheetDataManager2.Data.DataFileDescription = "";

			for (int i = 0; i < sheets.Count; i++)
			{
				startDupCount = ScanStatus.DupsCount;
				startExtraCount = ScanStatus.XtraCount;

				Console.Write(".");

				fileName = Path.GetFileNameWithoutExtension(sheets[i]);

				if (!checkFileExists(sheets[i], fileName)) continue;

				if (!checkDuplicateFile(fileName)) continue;

				scan.ProcessPdf(sheets[i]);

				checkForDupsAndExtras(fileName);
			}

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private bool checkFileExists(string fullFilePath, string fname)
		{
			if (!File.Exists(fullFilePath))
			{
				ScanStatus.AddError(
					fname, "Sheet type found", ScanErrorLevel.ERROR_IS_FATAL);

				ScanStatus.HasFatalErrors = true;

				return false;
			}

			return true;
		}

		private bool checkDuplicateFile(string fname)
		{
			if ((SheetDataManager2.Data?.SheetDataList) == null) return true;

			if (SheetDataManager2.Data.SheetDataList.ContainsKey(fname))
			{
				ScanStatus.AddError(
					fname, "Sheet already saved (duplicate)", ScanErrorLevel.ERROR_MAYBE_FATAL);

				return false;
			}

			return true;
		}

		private void checkForDupsAndExtras(string fileName)
		{
			dupsAdded =  ScanStatus.DupsCount - startDupCount;
			extrasAdded =  ScanStatus.XtraCount - startExtraCount;

			if (dupsAdded > 0)
			{
				ScanStatus.AddError(
					fileName, $"{dupsAdded} duplicate {getRectangleQtyDesc(dupsAdded)} found", ScanErrorLevel.ERROR_IS_FATAL);
			}

			if (extrasAdded > 0)
			{
				ScanStatus.AddError(
					fileName, $"{extrasAdded} extra {getRectangleQtyDesc(extrasAdded)} found", ScanErrorLevel.ERROR_IS_FATAL);
			}
		}

		private string getRectangleQtyDesc(int qty)
		{
			if (qty == 1)
			{
				return "rectangle";
			}

			return "rectangles";
		}

		// reports

		private void showScanReport()
		{
			bool result;

			Console.WriteLine($"\nScan forSheet Boxes Report");

			foreach (KeyValuePair<string, SheetData2> kvp in SheetDataManager2.Data.SheetDataList)
			{
				showReportShtRects(kvp);

				showReportOptRects(kvp);
			}
		}

		private void showReportShtRects(KeyValuePair<string, SheetData2> kvp)
		{
			bool result;

			Console.WriteLine($"\nfor {kvp.Value.Name}");

			if (kvp.Value.AllShtRectsFound)
			{
				Console.WriteLine($"\tAll sheet boxes found");
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

				Console.Write("\n");
			}
		}

		private void showReportOptRects(KeyValuePair<string, SheetData2> kvp)
		{
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

		private void showScanErrReport()
		{
			ScanStatus.ShowScanErrorReport();
		}
	}
}