#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScanPDFBoxes.Process2;
using ShItextCode.ElementExtraction;
using ShSheetData.ShSheetData2;
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
		public string DataFilePath => SheetFileManager2.DataFilePath.FullFilePath;

		public bool SdmInitialized => SheetDataManager2.Initialized;
		public bool SdmHasSheets => SheetDataManager2.HasSheets;
		public int SdmSheetCount => SheetDataManager2.SheetsCount;

		public bool? DataFileInit => sfm2?.GotDataFile ?? false;
		public bool DataFilePathInit => sfm2?.DataFilePathInit ?? false;
		public bool SheetFilesInit { get; private set; }

		/// <summary> initialize the various data files<br/> creates the list of sheet PDF files to process<br/>
		/// initialize the SheetDataManager but does not read SheetData
		/// </summary>
		public bool InitDataFile()
		{
			DM.DbxLineEx(0, "start", 1);

			if (DataFilePathInit) return false;

			// config the sheet data folder
			if (!sfm2.GetDataFileFolder(ShSamples.DATA_FILE_FOLDER))
			{
				DM.DbxLineEx(0, "end 1 (fail)", -1);
				return false;
			}

			// config the sheet data folder file name
			if (!sfm2.GetDataFile(SheetDataManager2.DataFileName))
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

			if (DataFilePathInit == false || SheetFilesInit)
			{
				DM.DbxLineEx(0, $"end 1 - DataFileInit {DataFileInit} or SheetFilesInit {SheetFilesInit} issue", -1);
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

			if (DataFileInit ==  false)
			{
				DM.DbxLineEx(0,"end 1", -1);
				return false;
			}

			createDataManager();

			DM.DbxLineEx(0, $"sheetdatamanager", 1);
			DM.DbxLineEx(0, $"init   {SdmInitialized}", 1);
			DM.DbxLineEx(0, $"exists {SheetDataManager2.SettingsFileExists}");
			DM.DbxLineEx(0, $"loaded {SdmHasSheets}", -1);
			DM.DbxLineEx(0, $"sheetdatamanager", -1);

			DM.DbxLineEx(0,"end", -1);

			return true;
		}

		public void createDataManager()
		{
			DM.DbxLineEx(0,"start", 1);

			if (!SheetDataManager2.Initialized)
			{
				SheetDataManager2.Open(SheetFileManager2.DataFilePath);
			}

			DM.DbxLineEx(0,"end", -1);
		}

		public bool? InitSheetData()
		{
			DM.DbxLineEx(0,"start", 1);

			if (DataFileInit ==  false)
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

			DM.DbxLineEx(0,$"\tstatus | loaded {SheetDataManager2.HasSheets} | read {SheetDataManager2.SheetsCount} sheets");

			if (SheetDataManager2.SheetsCount == -1)
			{
				SheetDataManager2.Reset();
				SheetDataManager2.Data.SheetDataList = new Dictionary<string, SheetData2>();
			}

			DM.DbxLineEx(0,"end", -1);

			return true;
		}

	#region public operations

		// scan sheets
		// reset sheets - clear from memory
		// remove sheets
		// query sheets

		/// <summary> scan the list of sheets and create the data file<br/>
		/// true = worked <br/>
		/// false = did not work or other error<br/>
		/// null = cannot proceed / config issue
		/// </summary>
		public bool? ScanShts()
		{
			bool result;

			DM.DbxLineEx(0,"start", 1);


			if (!sfm2.GotSheetFiles)
			{
				DM.DbxLineEx(0,"end 1 - exit - sheet file paths not configured", -1);
				return null;
			}

			if (SdmHasSheets)
			{
				DM.DbxLineEx(0,"end 2 - exit - sheet files exist", -1);
				return false;
			}

			SheetDataManager2.updateHeader();

			ScanSheets ss = new ScanSheets();

			result = ss.Process(sfm2.SheetFileList);

			showScanResults(0);

			DM.DbxLineEx(0,"end", -1);

			return result;
		}

		public void ResetShtData()
		{

			// clear the sheet data
			SheetDataManager2.Reset();
		}

		public void QueryShts()
		{
			if (!SdmHasSheets) return;
		}

		public void RemoveShts()
		{
			if (!SdmHasSheets) return;
		}

		public bool Close()
		{
			DM.DbxLineEx(0, "start");

			sfm2.ResetSheetFiles();
			sfm2.ResetDataFile();

			SheetDataManager2.Close();

			DM.DbxLineEx(0, "end", -1);

			return true;
		}

		public bool? Open()
		{


			return true;
		}

		public bool configDataManager()
		{
			DM.DbxLineEx(0, "start", 1);

			if (!InitDataFile())
			{
				DM.DbxLineEx(0, "end 2", -1);
				return false;
			}

			if (!initDataManager())
			{
				DM.DbxLineEx(0, "end 3", -1);
				return false;
			}
			
			DM.DbxLineEx(0, "end", -1);

			return true;
		}

		public bool ConfigSheetFileInfo(int idx)
		{
			DM.DbxLineEx(0, "start", 1);

			if (!InitSheetFiles(idx))
			{
				DM.DbxLineEx(0, "end 1 - (", -1);
				return false;
			}

			DM.DbxLineEx(0, "end", -1);

			return true;
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
