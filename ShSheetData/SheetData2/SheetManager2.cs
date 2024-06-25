#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShCode.ShDebugInfo;
using ShSheetData.ShSheetData;
using ShTempCode.DebugCode;
using UtilityLibrary;
using ShowWhere = ShTempCode.DebugCode.ShowWhere;

#endregion

// user name: jeffs
// created:   6/21/2024 11:18:40 PM

namespace ShSheetData.SheetData2
{
	public class SheetManager2
	{
		private SheetFileManager2 sfm2;

		public SheetManager2()
		{
			sfm2 = new SheetFileManager2();
		}

		// private FilePath<FileNameSimple> dataFilePath;

		// the location that the list of sheet box information is saved
		public string DataFilePath => sfm2.DataFilePath.FullFilePath;

		public bool SdmInitialized => SheetDataManager2.Initialized;
		public bool SdmHasSheets => SheetDataManager2.HasSheets;
		public int SdmSheetCount => SheetDataManager2.SheetsCount;

		public bool? DataFilesInit => sfm2?.GotDataFile ?? false;
		public bool SheetFilesInit { get; private set; }

		/// <summary> initialize the various data files<br/> creates the list of sheet PDF files to process<br/>
		/// initialize the SheetDataManager but does not read SheetData
		/// </summary>
		public bool InitDataFile()
		{
			DM.DbxLineEx(0, "start", 1);

			if (DataFilesInit == true) return false;

			// config the sheet data folder
			if (!sfm2.GetDataFileFolder(ShSamples.DATA_FILE_FOLDER))
			{
				DM.DbxLineEx(0, "end 1 (fail)", -1);
				return false;
			}

			// config the sheet data folder file name
			if (!sfm2.GetDataFile(ShSamples.DATA_FILE_NAME))
			{
				DM.DbxLineEx(0, "file not found");
			}

			DM.DbxLineEx(0, $"end | data file status {sfm2.GotDataFile?.ToString() ?? "null"}", -1);

			return true;
		}

		/// <summary> initialize the sheet files - folder and read the sheet files
		/// </summary>
		public bool InitSheetFiles(int idx)
		{
			DM.DbxLineEx(0, "start", 1);

			if (DataFilesInit == false || SheetFilesInit)
			{
				DM.DbxLineEx(0, "end 1", -1);
				return false;
			} // not initialized or already init'd

			if (sfm2.GotSheetFiles)
			{
				DM.DbxLineEx(0, "end 2", -1);
				return false;
			} // already got - don't re-load

			SheetFilesInit = false;

			ShSamples ss = new ShSamples();

			Sample sample;

			if (!ss.SampleData.TryGetValue(idx, out sample))
			{
				DM.DbxLineEx(0, "end 3", -1);
				return false;
			}

			// config the the sheet file folder (do not read the file list)
			sfm2.GetSheetFileFolder(sample.PdfFolder.FullFilePath);

			if (!sfm2.GetSheetFiles())
			{
				DM.DbxLineEx(0, "end 4", -1);
				return false;
			}

			SheetFilesInit = true;

			DM.DbxLineEx(0, "end", -1);

			return true;
		}

		/// <summary> initialize the SheetDataManager
		/// </summary>
		public bool initDataManager()
		{
			DM.DbxLineEx(0,"start", 1);

			if (DataFilesInit ==  false)
			{
				DM.DbxLineEx(0,"end 1", -1);
				return false;
			}

			if (!SheetDataManager2.Initialized)
			{
				SheetDataManager2.Init(sfm2.DataFilePath);
			}

			DM.DbxLineEx(0, $"sheetdatamanager", 1);
			DM.DbxLineEx(0, $"init   {SdmInitialized}", 1);
			DM.DbxLineEx(0, $"exists {SheetDataManager2.SettingsFileExists}");
			DM.DbxLineEx(0, $"loaded {SdmHasSheets}", -1);
			DM.DbxLineEx(0, $"sheetdatamanager", -1);

			DM.DbxLineEx(0,"end", -1);

			return true;
		}

		public bool? ReadSheetData()
		{
			DM.DbxLineEx(0,"start", 1);

			if (DataFilesInit ==  false)
			{
				DM.DbxLineEx(0,"end 1 - not init", -1);
				return false;
			}

			if (SheetDataManager2.HasSheets)
			{
				DM.DbxLineEx(0,$"end 2 - is loaded and has {SheetDataManager2.SheetsCount} sheets", -1);
				return null;
			}
			
			SheetDataManager2.Read();

			DM.DbxLineEx(0,$"\tstatus | loaded {SheetDataManager2.HasSheets} | read {SheetDataManager2.SheetsCount} sheets", 1);

			DM.DbxLineEx(0,"end", -1);

			return true;
		}

	#region public operations

		// scan sheets
		// reset sheets - clear from memory
		// remove sheets
		// query sheets

		public bool ScanSheets()
		{
			DM.DbxLineEx(0,"start", 1);

			if (SdmHasSheets)
			{
				DM.DbxLineEx(0,"end 1", -1);
				return false;
			}


			showScanResults(0);

			DM.DbxLineEx(0,"end", -1);

			return true;
		}


		public void ResetSheetData()
		{
			sfm2.ResetDataFile();
			sfm2.ResetSheetFiles();

			// clear the sheet data
			SheetDataManager2.Reset();
		}


		public void QuerySheets()
		{
			if (!SdmHasSheets) return;
		}

		public void RemoveSheets()
		{
			if (!SdmHasSheets) return;
		}


	#endregion

	#region private operations

		private bool ReadSheets()
		{
			if (SdmHasSheets) return false;

			return true;
		}

		private void showScanResults(int option)
		{

		}

		private void showSheetInformation(int option)
		{

		}

		private void showRemoveResults()
		{

		}

	#endregion


	}
}
