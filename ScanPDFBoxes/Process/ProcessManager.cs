#region + Using Directives

using iText.Kernel.Geom;
using ScanPDFBoxes.SheetData;
using ShItextCode;
using ShSheetData.SheetData;
using UtilityLibrary;

using Path = System.IO.Path;

#endregion

// user name: jeffs
// created:   5/24/2024 8:05:30 PM

namespace ScanPDFBoxes.Process
{
	public enum ErrorLevel
	{
		WARNING,
		ERROR_MAYBE_FATAL,
		ERROR_IS_FATAL
	}

	public class ProcessManager
	{
		private static int cpm = SheetDataManager.SheetsCount;

		int startDupCount;
		int startExtraCount;

		int dupsAdded;
		int extrasAdded;

		public List<Tuple<string, string, Rectangle>> duplicateRects { get; private set; }
		public List<Tuple<string, string, Rectangle>> extraRects { get; private set; }
		public List<Tuple<string?, string?, ErrorLevel>> errors { get; private set; }

		public ProcessManager(FilePath<FileNameSimple> dataFile)
		{
			DataFilePath = dataFile;
		}

		public FilePath<FileNameSimple> DataFilePath { get; set; }

		public bool HasFatalErrors {get; set; }

		// config

		private void ResetSheetData()
		{
			SheetDataManager.Admin!.Reset();
		}

		public void ConfigSheetData()
		{
			if (!SheetDataManager.Initialized)
			{
				SheetDataManager.Init(DataFilePath);
			}

			SheetDataManager.Read();
		}

		// setup

		public bool ScanSheets(string[] sheets)
		{
			HasFatalErrors = false;

			duplicateRects = new List<Tuple<string, string, Rectangle>>();
			extraRects = new List<Tuple<string, string, Rectangle>>();
			errors = new List<Tuple<string?, string?, ErrorLevel>>();

			ConfigSheetData();

			// ShowSheetRectInfo.showStatus(ShowWhere.DEBUG, "@22");

			int count = SheetDataManager.SheetsCount;

			scanSheets(sheets);

			Console.WriteLine(" done\n");

			ShowSheetRectInfo.showScanReport(this);

			// ShowSheetRectInfo.showStatus(ShowWhere.DEBUG, "@23");

			if (errors.Count == 0)
			{
				writeData();

				Console.WriteLine("\nData Written");
			}
			else
			{
				Console.WriteLine("\nDue to errors, the changes will not be saved");

				SheetDataManager.Read();
			}

			// ShowSheetRectInfo.showStatus(ShowWhere.DEBUG, "@29");

			return SheetDataManager.SheetsCount - count > 0 && errors.Count == 0 && !HasFatalErrors;
		}

		public bool RemoveSheets(string[] sheets)
		{
			// ShowSheetRectInfo.showStatus(ShowWhere.DEBUG, "@31");

			int initCount = SheetDataManager.SheetsCount;

			HasFatalErrors = false;

			errors = new List<Tuple<string?, string?, ErrorLevel>>();

			ConfigSheetData();

			// ShowSheetRectInfo.showStatus(ShowWhere.DEBUG, "@32");

			for (var i = 0; i < sheets.Length; i++)
			{
				removeSheet(sheets[i]);
			}

			ShowSheetRectInfo.ShowRemoveReport(this, initCount);

			// ShowSheetRectInfo.showStatus(ShowWhere.DEBUG, "@33");

			if (errors.Count == 0)
			{
				writeData();

				Console.WriteLine("\nData Written");
			}
			else
			{
				Console.WriteLine("\nDue to errors, the changes will not be saved");

				SheetDataManager.Read();
			}

			// ShowSheetRectInfo.showStatus(ShowWhere.DEBUG, "@39");

			return errors.Count > 0 && !HasFatalErrors;
		}

		public void QuerySheets(string[] sheets)
		{
			Process.QuerySheets qs = new QuerySheets();

			qs.Process(sheets);

			PdfShowInfo.ShowPdfInfoBasic(qs.Docs);
		}

		// action

		private void scanSheets(string[] sheets)
		{
			string fileName;

			ScanSheet ss = new ScanSheet(this);

			for (var i = 0; i < sheets.Length; i++)
			{
				startDupCount = duplicateRects.Count;
				startExtraCount = extraRects.Count;

				Console.Write(".");

				fileName = Path.GetFileNameWithoutExtension(sheets[i]);

				if (!checkFileExists(sheets[i], fileName)) continue;

				if (!checkDuplicateFile(fileName)) continue;

				ss.ProcessSheet(sheets[i]);

				checkForDupsAndExtras(fileName);

			}
		}

		private void removeSheet(string sheet)
		{
			string fileName = Path.GetFileNameWithoutExtension(sheet);

			if (!File.Exists(sheet))
			{
				errors.Add(new Tuple<string?, string?, ErrorLevel>(
					Path.GetFileName(sheet), "Sheet type not found", ErrorLevel.ERROR_MAYBE_FATAL));

				return;
			}

			bool result = SheetDataManager.Data!.SheetRectangles.Remove(fileName);

			if (!result)
			{
				errors.Add(new Tuple<string?, string?, ErrorLevel>
					(Path.GetFileName(sheet), "Could not find / remove sheet", ErrorLevel.ERROR_IS_FATAL));

				HasFatalErrors = true;
			}
		}


		// util

		private void checkForDupsAndExtras(string fileName)
		{
			dupsAdded =  duplicateRects.Count - startDupCount;
			extrasAdded =  extraRects.Count - startExtraCount;

			if (dupsAdded > 0)
			{
				errors.Add(new Tuple<string?, string?, ErrorLevel>(
					fileName, $"{dupsAdded} duplicate {getRectangleQtyDesc(dupsAdded)} found", ErrorLevel.ERROR_IS_FATAL));
			}

			if (extrasAdded > 0)
			{
				errors.Add(new Tuple<string?, string?, ErrorLevel>(
					fileName, $"{extrasAdded} extra {getRectangleQtyDesc(extrasAdded)} found", ErrorLevel.ERROR_IS_FATAL));
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

		private bool checkFileExists(string fullFilePath, string fname)
		{
			if (!File.Exists(fullFilePath))
			{
				errors.Add(new Tuple<string?, string?, ErrorLevel>(
					fname, "Sheet type found", ErrorLevel.ERROR_IS_FATAL));

				HasFatalErrors = true;

				return false;
			}

			return true;
		}

		private bool checkDuplicateFile(string fname)
		{
			if (SheetDataManager.Data.SheetRectangles.ContainsKey(fname))
			{
				errors.Add(new Tuple<string?, string?, ErrorLevel>(
					fname, "Sheet already saved (duplicate)", ErrorLevel.ERROR_MAYBE_FATAL));

				return false;
			}

			return true;
		}

		private bool writeData()
		{
			bool result = true;

			SheetDataManager.Write();

			return result;
		}


	}
}
