#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;
using SettingsManager;
using ShCode;
using ShItextCode.ElementExtraction;
using ShSheetData.ShSheetData2;
using ShTempCode.DebugCode;
using UtilityLibrary;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using ShowWhere = ShTempCode.DebugCode.ShowWhere;

#endregion

// user name: jeffs
// created:   6/21/2024 11:18:40 PM

namespace ShSheetData.SheetData2
{
	// public enum SM2_CODES
	// {
	//
	// 	SMC_INIT_DATA_MANAGER_FAILED = -4,
	// 	SMC_SCAN_PDF_FOLDER_MISSING = -3,
	// 	SMC_DATA_FILE_PATH_ALREADY_SET = -2,
	// 	SMC_DATA_FILE_HAS_SHEETS = -1,
	// 	SMC_NORMAL = 0,		// 0 neutral
	// 	SMC_WORKED = 1,		// >0 good
	// }
	//

	public class SheetManager2
	{
		private SheetFileManager2 sfm2;

		public SheetManager2(SheetFileManager2 sfm2)
		{
			this.sfm2 = sfm2;

			StatusCode = StatCodes.SC_G_NONE;
		}

		public StatCodes StatusCode { get; private set; }

	#region public properties

		public FilePath<FileNameSimple> DataFilePath => sfm2.DataFilePath;

		public bool GotDataFilePath => sfm2.GotDataFilePath;
		public bool? GotDataFile => sfm2.GotDataFile;

		public List<string> SheetFileList => sfm2.SheetFileList;
		public string SheetFileFolder => sfm2.SheetFileFolder;
		public bool GotSheetFileList => sfm2.GotSheetFileList;
		public bool GotSheetFolder => sfm2.GotSheetFolder;
		public bool SheetFolderExists => sfm2.SheetFolderExists;

	#endregion


	#region public operations

		// config operations

		/// <summary> initialize the various data files<br/> creates the list of sheet PDF files to process<br/>
		/// initialize the SheetDataManager but does not read SheetData
		/// </summary>
		public bool InitDataFilePath()
		{
			// usersettings configured with the path values

			DM.DbxLineEx(0, "start", 1);

			if (sfm2.GotDataFilePath == true)
			{
				StatusCode = StatCodes.SC_DM_DATA_FILE_PATH_ALREADY_SET;
				DM.DbxLineEx(0, "end 1 (already config'd)", -1);
				return false;
			}

			sfm2.DataFilePath = UserSettings.Data.DataFilePath;

			DM.DbxLineEx(0, $"end | data file status {sfm2.GotDataFile?.ToString() ?? "null"}", 0, -1);

			return true;
		}

		public bool InitSheetFileFolder()
		{
			DM.DbxLineEx(0, "start", 1);

			bool b1 = sfm2.GotSheetFolder;
			bool b2 = sfm2.SheetFolderExists;

			if (sfm2.SheetFolderExists)
			{
				StatusCode = StatCodes.SC_SFM_SHEET_DATA_FOLDER_MISSING;
				DM.DbxLineEx(0, "end 1 (scan pdf folder missing)", -1);
				return false;
			}

			DM.DbxLineEx(0, "end", 0, -1);

			return sfm2.SetSheetFileFolder(UserSettings.Data.ScanPDfFolder.FolderPath);
		}

		/// <summary> start or re-use the SheetDataManager<br/>
		/// if not initialized, init, write the initial data file<br/>
		/// if initialized, ok if sheet data files count == 0
		/// </summary>
		public bool StartDataManager()
		{
			DM.DbxLineEx(0,"start", 1);

			if (SheetDataManager2.Configured)
			{
				if (SheetDataManager2.GotDataSheets)
				{
					StatusCode = StatCodes.SC_DM_DATA_FILE_HAS_SHEETS;
					DM.DbxLineEx(0,"end 1", 0, -1);
					return false;
				}
			}
			else
			{
				// not configured
				if (!SheetDataManager2.Init(sfm2.DataFilePath))
				{
					StatusCode = StatCodes.SC_DM_INIT_DATA_MANAGER_FAILED;
					DM.DbxLineEx(0,"end 2", 0, -1);
					return false;
				}
			}

			SheetDataManager2.Open(); 

			// is initialized and may or may not have sheets as Open reads the existing file
			// and if the file has sheets, they are read

			DM.DbxLineEx(0, $"status", 1, 1);
			DM.DbxLineEx(0, $"exists {SheetDataManager2.SettingsFileExists}");
			DM.DbxLineEx(0, $"loaded {SheetDataManager2.GotDataSheets}");
			DM.DbxLineEx(0, $"done", -1, -1);

			DM.DbxLineEx(0, "end", 0, -1);

			StatusCode = StatCodes.SC_G_WORKED;

			return true;
		}

		// scan sheets
		// reset sheets - clear from memory
		// remove sheets
		// query sheets


		// scan operations

		/// <summary> scan the list of sheets and create the data file<br/>
		/// true = worked <br/>
		/// false = did not work or other error
		/// </summary>
		// /// null = cannot proceed / config issue
		public bool? ScanShts()
		{
			bool result;

			DM.DbxLineEx(0,"start", 1);

			// reset (clear and prep for new) the sheet data list
			SheetDataManager2.ResetSheetDataList();

			ScanSheets ss = new ScanSheets();

			result = ss.Process(sfm2.SheetFileList);

			showScanResults(0);

			DM.DbxLineEx(0,"end", 0, -1);

			return result;
		}

		public bool? ScanSht(string sheet)
		{
			bool result;

			DM.DbxLineEx(0,"start", 1);

			ScanSheets ss = new ScanSheets();

			result = ss.Process(new () {sheet});

			showScanResults(0);

			DM.DbxLineEx(0,"end", 0, -1);

			return result;
		}

		// general operations

		public void ResetFull()
		{
			// tasks
			// reset the data manager sheet list and save / update the information and date
			// reset the sheet file manager's data paths to null;
			// reset the sheet file manager's sheet file paths to null
			// reset the sheet file manager's sheet list to null

			CloseDataManager();
			ResetDataManager();

			sfm2.ResetSheetFiles();
			sfm2.ResetDataFile();
		}

		// sheet file manager operations

		public bool LoadSheetFiles()
		{
			return sfm2.GetSheetFiles();
		}

		public string SwitchboardSelectSheetFile()
		{
			DM.DbxLineEx(0,"start", 1);

			Dictionary<string, Tuple<string, string>> files;

			int result = getAvailableSheetDataFiles(out files);

			if (result == 0)
			{
				DM.DbxLineEx(0,"end 1 (nothing to select)", 0, -1);
				return null;
			}

			if (result > 99)
			{
				DM.DbxLineEx(0,"end 2 (too many selections)", 0, -1);
				return null;
			}

			Console.Write("\n");

			foreach (KeyValuePair<string, Tuple<string, string>> kvp in files)
			{
				Console.WriteLine($"> {kvp.Key,-3}  | *** {kvp.Value.Item1}");
			}

			Console.WriteLine($"> {'X',-3}  | *** Exit");
			Console.Write("\n? ");

			string c = Console.ReadKey().KeyChar.ToString().ToUpper();

			Console.Write($"{c}");

			if (c.Equals("X"))
			{
				DM.DbxLineEx(0,"end 3", 0, -1);
				return null;
			}

			string c1 = null;

			if (files.Count > 9)
			{
				c1 = Console.ReadKey().KeyChar.ToString().ToUpper();
				Console.Write($"{c1}\n\n");
			}

			if (!files.ContainsKey(c + c1))
			{
				DM.DbxLineEx(0,"end 4", 0, -1);
				return null;
			}

			DM.DbxLineEx(0,"end", 0, -1);

			return files[c + c1].Item2;
		}

		// data manager operations

		public bool RemoveSheet(string sheet)
		{
			DM.DbxLineEx(0,"start", 1);
			DM.DbxLineEx(0,$"remove sheet {sheet}");

			bool result = SheetDataManager2.Data.SheetDataList.Remove(sheet);

			if (!result)
			{
				DM.DbxLineEx(0,"end 1", 0, -1);
				return false;
			}

			SheetDataManager2.Write();

			DM.DbxLineEx(0,"end", 0, -1);

			return true;
		}

		public void ResetDataFile()
		{
			SheetDataManager2.ResetSheetDataList();
			SheetDataManager2.Close();
		}

		public void ResetDataManager()
		{
			// clear the sheet data
			SheetDataManager2.Reset();
		}

		public void QueryShts()
		{
			if (!SheetDataManager2.GotDataSheets) return;
		}

		public void RemoveShts()
		{
			if (!SheetDataManager2.GotDataSheets) return;
		}

		public bool InitDataManager()
		{
			DM.DbxLineEx(0,"start", 1);

			if (SheetDataManager2.Configured)
			{
				DM.DbxLineEx(0,"end 1", 0, -1);
				return false;
			}

			SheetDataManager2.Init(sfm2.DataFilePath);

			DM.DbxLineEx(0,"end", 0, -1);

			return true;
		}

		public bool CloseDataManager()
		{
			DM.DbxLineEx(0, "start");

			SheetDataManager2.Close();

			DM.DbxLineEx(0, "end", 0, -1);

			return true;
		}

		public void CreateDataManager()
		{
			DM.DbxLineEx(0,"start", 1);

			// if (!SheetDataManager2.SettingsFileExists)
			// {
			// }
			SheetDataManager2.Create();

			DM.DbxLineEx(0,"end", 0, -1);
		}

		public bool OpenDataManager()
		{
			DM.DbxLineEx(0,"start", 1);

			if (!InitDataManager())
			{
				DM.DbxLineEx(0,"end 1", 0, -1);
				return false;
			}

			SheetDataManager2.Open();

			DM.DbxLineEx(0,"end", 0, -1);

			return true;
		}

		// public bool configDataManager()
		// {
		// 	DM.DbxLineEx(0, "start", 1);
		//
		// 	if (!InitDataFile())
		// 	{
		// 		DM.DbxLineEx(0, "end 2", 0, -1);
		// 		return false;
		// 	}
		//
		// 	if (!initDataManager())
		// 	{
		// 		DM.DbxLineEx(0, "end 3", 0, -1);
		// 		return false;
		// 	}
		// 	
		// 	DM.DbxLineEx(0, "end", 0, -1);
		//
		// 	return true;
		// }

		// public bool ConfigSheetFileInfo(int idx)
		// {
		// 	DM.DbxLineEx(0, "start", 1);
		//
		// 	if (!InitSheetFiles(idx))
		// 	{
		// 		DM.DbxLineEx(0, "end 1 - (", -1);
		// 		return false;
		// 	}
		//
		// 	DM.DbxLineEx(0, "end", 0, -1);
		//
		// 	return true;
		// }

		public bool SheetDataContainsSheet(string name)
		{
			return SheetDataManager2.Data.SheetDataList.ContainsKey(name);
		}

	#endregion

	#region private operations

		private int getAvailableSheetDataFiles(out Dictionary<string, Tuple<string, string>> outFiles)
		{
			outFiles = new Dictionary<string, Tuple<string, string>>();
			Dictionary<string, Tuple<string, string>> inFiles = sfm2.SheetFileDictionary;

			foreach (KeyValuePair<string, Tuple<string, string>> kvp in inFiles)
			{
				if (!SheetDataContainsSheet(kvp.Value.Item1))
				{
					outFiles.Add(kvp.Key, kvp.Value);
				}
			}

			return outFiles.Count;
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
