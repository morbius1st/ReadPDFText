#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DebugCode;
using SettingsManager;
using ShSheetData;
using ShSheetData.SheetData;
using ShSheetData.SheetData2;
using ShSheetData.ShSheetData2;
using ShTempCode.DebugCode;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   7/1/2024 7:58:54 PM

namespace ShCode
{
	// to be run first to get the folders / files
	// needed
	// step 1 - read saved info from a setting file - use this as a starting point
	// step 2 - use step 1 to request folder / file location - present current 
	//			allow select which to change, if any
	// debug, present pre-configured list and select
	public class ConfigManager
	{
		private string switchboardIdx;

		private SheetFileManager2 sfm2;
		private SheetManager2 sm2;

		public ConfigManager(SheetFileManager2 sfm2, SheetManager2 sm2)
		{
			this.sfm2 = sfm2;

			this.sm2 = sm2;
		}

		/// <summary>initialize the paths & variables / make sure
		/// the needed information is found and loaded<br/>
		/// id = switchboard id (which file to use)
		/// def = default
		/// </summary>
		public bool initialize(string id, int def)
		{
			switchboardIdx = id;

			SetStatus(StatusCodes.SC_G_NONE);

			DM.Start0($"cm-1a: start");

			// simulates getting the information from the user setting file
			if (!configGetScanInfo(def))
			{
				showStatus(true);
				SetStatus(StatusCodes.SC_INIT_GET_PATHS_FAIL);
				DM.Stat0("cm-1b: end 1");
				return false;
			}

			// init sheet data manager based on the info from the user settings file
			// init just creates an instance using the provided file path
			if (configSheetDataFilePath() != true)
			{
				showStatus(true);
				SetStatus(StatusCodes.SC_INIT_CFG_DATA_PATH_FAIL);
				DM.Stat0("cm-1c: end 2");
				return false;
			}

			// sets the path for the sheet folder
			if (configSheetPdfScanFolderPath() != true)
			{
				showStatus(true);
				SetStatus(StatusCodes.SC_INIT_CFG_SHT_DATA_PATH_FAIL);
				DM.Stat0("cm-1d: end 3");
				return false;
			}

			// "starts" the sheet data manager - probably not needed
			// but left, for now ...
			if (!sm2.StartDataManager())
			{
				showStatus( true);
				SetStatus(StatusCodes.SC_INIT_START_DATA_MGR_FAIL);
				DM.Stat0("cm-1e: end 4");
				return false;
			}

			// reads the sheet files from the folder in prep
			// for scanning
			if (!sm2.LoadSheetFiles())
			{
				showStatus( true);
				SetStatus(StatusCodes.SC_INIT_LOAD_SHT_DATA_FILES_FAIL);
				DM.Stat0("cm-1f: end 5");
				return false;
			}

			DM.End0("cm-1z: end - config manager initialized");

			SetStatus(StatusCodes.SC_G_GOOD);

			return true;
		}

		// which 0 or 2 get data file | 1 or 2 get sheetPDF folder
		/// <summary>currently a temp routine - but will be the
		/// starting point to have the user select the config
		/// folders and files
		/// </summary>
		public bool SelectScanConfigFiles(int which, int def, bool selectDefault = false)
		{
			if (def == 2) return true;

			DM.DbxLineEx(0, "\tSelectScanConfigFiles");
			if (!getScanData(which, def, selectDefault)) return false;

			return true;
		}

		/// <summary>temp routine 
		/// 
		/// </summary>
		private bool getScanData(int which, int def, bool selectDefault)
		{
			DM.Stat0("\tselectScanFromSample");

			ShSamples samp = new ShSamples();

			if (samp.SelectScanSample(def, selectDefault) != true) return false;
			
			Sample s = samp.Selected;

			if (which == 0 || which == 2)
			{
				SuiteSettings.Data.DataFilePath = s.DataFilePath;
			}

			if (which == 1 || which == 2)
			{
				SuiteSettings.Data.ScanPDfFolder = s.ScanPDfFolder;
			}

			DM.End0();

			return true;
		}


		public bool verifyConfig1(string id,
			bool? mustHaveDataFilePath, bool? mustHaveDataFile, 
			bool? mustHaveDataFileSheets, bool? mustHaveSheetFileList)
		{
			DM.Start0();

			switchboardIdx = id;

			bool answer1 = true;
			bool answer2 = true;
			bool answer3 = true;
			bool answer9 = true;

			bool answerFinal = false;


			return answerFinal;
		}


		/// <summary>depending on the planned operation, verify that the
		/// various parts are configured
		/// </summary>
		public bool verifyConfig2(string id,
			bool? mustHaveDataFilePath, bool? mustHaveDataFile, 
			bool? mustHaveDataFileSheets, bool? mustHaveSheetFileList)
		{
			DM.Start0($"option {id}");

			DM.Stat0($"1) must have data file path   {mustHaveDataFilePath}");
			DM.Stat0($"2) must have data file        {mustHaveDataFile}");
			DM.Stat0($"3) must have data file sheets {mustHaveDataFileSheets}");
			DM.Stat0($"9) must have data file list   {mustHaveSheetFileList}");

			switchboardIdx = id;

			bool answer1 = true;
			bool answer2 = true;
			bool answer3 = true;
			bool answer9 = true;

			bool answerFinal;


			// so far, always true
			if (mustHaveDataFilePath == true)
			{
				answer1 = sm2.GotDataFilePath && SheetDataManager2.GotDataPath;

				if (!answer1)
				{
					SetStatus(StatusCodes.SC_CFG_DATA_FILE_PATH_MISSING);
					DM.DbxLineEx(0, $"end 1", -1);

					showVerifyConfigResults(false, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

					return false;
				}

				// answer1 is true;

				// bool b1 = SheetDataManager2.Initialized;
				// bool b2 = SheetDataManager2.GotDataSheets;
				// bool b3 = SheetDataManager2.SettingsFileExists;
				// bool b4 = SheetDataManager2.GotDataPath;

				if (!mustHaveDataFile.HasValue) // ie == null
				{
					answer2 = SheetDataManager2.SheetsCount >= 0;

					if (answer2)
					{
						if (mustHaveDataFileSheets == false)
						{
							answer3 = SheetDataManager2.SheetsCount == 0;
						}
					}
				}
				else 
				if (mustHaveDataFile == true)
				{
					answer2 = SheetDataManager2.SheetsCount >= 0;

					if (answer2)
					{
						if (mustHaveDataFileSheets.HasValue)
						{
							answer3 = SheetDataManager2.GotDataSheets;
							answer3 = answer3 == mustHaveDataFileSheets.Value;
						}
					}
				}
			} 
			else 
			if (mustHaveDataFilePath == false)
			{
				answer1 = !(sm2.GotDataFilePath && SheetDataManager2.GotDataPath);
				answer2 = SheetDataManager2.SheetsCount > 0 == mustHaveDataFile;
				answer3 = !(SheetDataManager2.SheetsCount <=0) == mustHaveDataFileSheets;
			}

			if (mustHaveSheetFileList == true)
			{
				answer9 = sm2.GotSheetFolder;

				if (answer9)
				{
					answer9 = sm2.GotSheetFileList;
				}
			}

			DM.DbxLineEx(0, $"ans1 {answer1} | ans2 {answer2} | ans3 {answer3} | ans9 {answer9}");

			answerFinal = answer1 && answer2 && answer3 && answer9;

			if (answerFinal)
			{
				SetStatus(StatusCodes.SC_G_GOOD);
			}
			else
			{
				if (!answer1)
				{
					SetStatus(StatusCodes.SC_CFG_DATA_HAS_FILE_PATH);
				}
				else
				if (!answer2)
				{
					SetStatus(StatusCodes.SC_CFG_DATA_FILE_MISSING);
				}
				else if (!answer3)
				{
					SetStatus(StatusCodes.SC_CFG_DATA_FILE_HAS_SHEETS_INVALID);
				}
				else if (!answer9)
				{
					SetStatus(StatusCodes.SC_CFG_DATA_FILE_SHEET_LIST_INVALID);
				}
			}

			showVerifyConfigResults(answerFinal, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

			DM.DbxLineEx(0, $"end", -1);

			return answerFinal;
		}

		/// <summary>
		/// select the configuration settings
		/// </summary>
		public bool configGetScanInfo(int def)
		{
			DM.Start0();

			if (def < 0 || SheetDataManager2.GotDataPath)
			{
				DM.End0("end 1");
				return false;
			}

			if (!SelectScanConfigFiles(2, def, true))
			{
				DM.End0("end 2");
				return false;
			}

			DM.End0();

			return true;
		}

		/// <summary>
		/// ** config the path / file name for the data manager
		/// </summary>
		public bool? configSheetDataFilePath()
		{
			// tasks
			// use configmanager to set usersettings / datafilepath
			// use sheetmanager to move usersettings to sheetFileManager
			// use sheetFileManager to define folder / file status
			// return true if folderpath is good & file exists
			// return false if folderpath is bad
			// return null if folderpath is good & file does not exist

			DM.DbxLineEx(0, "start", 1);

			// if (!cm.SelectScanConfigFiles(0, def))
			// {
			// 	DM.DbxLineEx(0, "end 1", 0, -1);
			// 	return false;
			// }

			if (!sm2.InitDataFilePath())
			{
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			DM.DbxLineEx(0, "end", 0, -1);

			return SheetDataManager2.GotDataPath;
		}

		/// <summary>
		/// ** config the path for the scan Pdf folder
		/// </summary>
		public bool configSheetPdfScanFolderPath()
		{
			// tasks
			// use configmanager to set usersettings / ScanPDfFolder
			// use sheetmanager to move usersettings to sheetFileManager
			// use sheetFileManager to define folder / file status
			// return true if folderpath is good
			// return false if folderpath is bad

			DM.DbxLineEx(0, "start", 1);

			if (sfm2.GotSheetFolder)
			{
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			if (!sm2.InitSheetFileFolder())
			{
				DM.DbxLineEx(0, "end 2", 0, -1);
				return false;
			}

			DM.DbxLineEx(0, "end", 0, -1);

			return sfm2.SheetFolderExists;
		}

		/// <summary>
		/// ** config get list of PDF files
		/// </summary>
		public bool configSheetPdfScanFiles(int def)
		{
			SetStatus(StatusCodes.SC_G_NONE);

			DM.DbxLineEx(0, $"start", 1);

			if (!configGetScanInfo(def))
			{
				showStatus(true);
				SetStatus(StatusCodes.SC_INIT_GET_PATHS_FAIL);
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			if (!configSheetPdfScanFolderPath())
			{
				showStatus(true);
				SetStatus(StatusCodes.SC_INIT_CFG_SHT_DATA_PATH_FAIL);
				DM.DbxLineEx(0, "end 3", 0, -1);
				return false;
			}

			if (!sm2.LoadSheetFiles())
			{
				showStatus( true);
				SetStatus(StatusCodes.SC_INIT_LOAD_SHT_DATA_FILES_FAIL);
				DM.DbxLineEx(0, "end 5", 0, -1);
				return false;
			}

			Console.Write("\n");
			Console.WriteLine($"\nScan PDf Folder {SuiteSettings.Data.ScanPDfFolder}");
			Console.WriteLine($"got sheet file list {sm2.GotSheetFileList}");
			Console.WriteLine($"number of sheet file {sm2.SheetFileListCount}");
			Console.Write("\n");


			DM.DbxLineEx(0, "end", 0, -1);

			SetStatus(StatusCodes.SC_G_GOOD);

			return true;
		}

		/// <summary>
		/// shortcut to set the operation status
		/// </summary>
		private void SetStatus(StatusCodes code,  string note = null,
			[CallerMemberName] string mx = null)
		{
			StatusMgr.SetStatCode(code, note, mx);
		}

		// show status information
		private void showStatus(bool showFrom = false, bool showOk = false)
		{
			StatusMgr.ShowStatus(showFrom, showOk);
		}

		// show the results of the verify config method
		private void showVerifyConfigResults(bool answer, bool? mustHaveDataFilePath,
			bool? mustHaveDataFile, bool? mustHaveDataFileSheets, bool? mustHaveSheetFileList)
		{
			// string s1 = sfm2.ScanOkToProceed.HasValue ?  sfm2.ScanOkToProceed.Value.ToString() : "is null";
			string s2 = sm2.GotDataFile.HasValue ?  sm2.GotDataFile.Value.ToString() : "is null";
			string s3 = mustHaveDataFile.HasValue ?  mustHaveDataFile.Value.ToString() : "is null";
			string s4 = mustHaveSheetFileList.HasValue ?  mustHaveSheetFileList.Value.ToString() : "is null";
			string s5 = mustHaveDataFilePath.HasValue ?  mustHaveDataFilePath.Value.ToString() : "is null";
			string s6 = mustHaveDataFileSheets.HasValue ?  mustHaveDataFileSheets.Value.ToString() : "is null";


			DM.DbxLineEx(0, $"start", 1);
			DM.DbxLineEx(0, $"option {switchboardIdx}", 1);

			DM.DbxLineEx(0, $"{"get config?",-32}{answer}");
			DM.DbxLineEx(0, $"{"must have data file path",-32}{s5,-8} | got path?   {SheetDataManager2.GotDataPath}");
			DM.DbxLineEx(0, $"{"must have data file?",-32}{s3,-8} | got file?   {s2}");
			DM.DbxLineEx(0, $"{"data file must have sheets?",-32}{s6,-8} | got sheets? {s2} | count {SheetDataManager2.SheetsCount}");

			DM.DbxLineEx(0, $"{"must have sht file path?",-32}{s4,-8} | got path?  {sm2.GotSheetFolder}");
			DM.DbxLineEx(0, $"{"must have sht Files?",-32}{s4,-8} | got files? {sm2.GotSheetFileList}");

			DM.DbxLineEx(0, $"{"got sheet folder?",-32}{sm2.GotSheetFolder}");
			DM.DbxLineEx(0, $"{"got sheet file?",-32}{sm2.GotSheetFileList} ({sm2.SheetFileList?.Count.ToString() ?? "null" })");

			DM.DbxLineEx(0, $"{"got data file?",-32}{s2}");
			DM.DbxLineEx(0, $"{"data file got path?",-32}{SheetDataManager2.GotDataPath}");
			DM.DbxLineEx(0, $"{"data file got sheets?",-32}{SheetDataManager2.GotDataSheets}");
			DM.DbxLineEx(0, $"{SuiteSettings.Data.DataFilePath?.FullFilePath ?? "data file path is null"}");
			DM.DbxLineEx(0, $"{SuiteSettings.Data.ScanPDfFolder?.FullFilePath ?? "PDF folder path is null"}", -1);

			DM.DbxLineEx(0, "end", 0, -1);
		}
	}
}