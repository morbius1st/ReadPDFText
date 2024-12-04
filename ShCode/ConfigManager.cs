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
			DM.Start0(false, $"option {id}");


			switchboardIdx = id;

			bool answer1 = true;
			bool answer2 = true;
			bool answer3 = true;
			bool answer9 = true;

			bool answerFinal;


			// so far, always true
			// working on answer 1, answer 2, and answer 3
			// DM.Stat0($"1 data file path status (mustHaveDataFilePath) {mustHaveDataFilePath}");
			
			if (mustHaveDataFilePath == true)
			{
				// DM.Stat0($"1a required");

				answer1 = sm2.GotDataFilePath && SheetDataManager2.GotDataPath;

				// DM.Stat0($"1b answer1 is {answer1}");

				if (!answer1)
				{
					// DM.Stat0($"1b1 answer1 is false - exit");

					SetStatus(StatusCodes.SC_CFG_DATA_FILE_PATH_MISSING);
					DM.End0("end 1");

					showVerifyConfigResults(false, 
						mustHaveDataFilePath, 
						mustHaveDataFile, 
						mustHaveDataFileSheets, 
						mustHaveSheetFileList,
						answer1);

					return false;
				}

				// DM.Stat0($"1b2 answer1 is true - cont");

				// answer 1 good
				// work on answer 2

				// if null - does not matter as much but must
				// check answer 3

				// DM.Stat0($"2 data file status (mustHaveDataFile) {mustHaveDataFile}");

				if (!mustHaveDataFile.HasValue) // ie == null
				{
					// DM.Stat0($"2a is null");

					answer2 = SheetDataManager2.SheetsCount >= 0;

					// DM.Stat0($"2a1 sheet count >= 0 (answer2) {answer2}");

					if (answer2)
					{
						// DM.Stat0($"2b1 answer1 is true - check (mustHaveDataFileSheets = {mustHaveDataFileSheets})");

						if (mustHaveDataFileSheets == false)
						{
							// DM.Stat0($"2c1 is false - (answer3) check sheet count");

							answer3 = SheetDataManager2.SheetsCount == 0;

							// DM.Stat0($"3 sheet count == 0? (answer3) {answer3}");
						} 
						// else
						// {
						// 	DM.Stat0($"2c1 is true - (answer3) no change == {answer3}");
						// }
					} 
					// else
					// {
					// 	DM.Stat0($"2b2 answer1 is false - ignore (mustHaveDataFileSheets)");
					// }
				}
				else 
				// answer 2
				if (mustHaveDataFile == true)
				{
					// DM.Stat0($"2a must be true");

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


			// work on answer9
			if (mustHaveSheetFileList == true)
			{
				answer9 = sm2.GotSheetFolder;

				if (answer9)
				{
					answer9 = sm2.GotSheetFileList;
				}
			}

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

			showVerifyConfigResults(answerFinal, 
				mustHaveDataFilePath, mustHaveDataFile, 
				mustHaveDataFileSheets, mustHaveSheetFileList,
				answer1, answer2, answer3, answer9);

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
		private void showVerifyConfigResults(bool finalAnswer, 
			bool? mustHaveDataFilePath,
			bool? mustHaveDataFile, 
			bool? mustHaveDataFileSheets, 
			bool? mustHaveSheetFileList,
			bool A1,
			bool? A2 = null,
			bool? A3 = null,
			bool? A9 = null
			)
		{

			// string s1 = sfm2.ScanOkToProceed.HasValue ?  sfm2.ScanOkToProceed.Value.ToString() : "is null";
			string s1 = mustHaveDataFilePath.HasValue    ?  mustHaveDataFilePath.Value.ToString()    : "null (T or F)";
			string s2 = mustHaveDataFile.HasValue        ?  mustHaveDataFile.Value.ToString()        : "null (T or F)";
			string s3 = mustHaveDataFileSheets.HasValue  ?  mustHaveDataFileSheets.Value.ToString()  : "null (T or F)";
			string s9 = mustHaveSheetFileList.HasValue   ?  mustHaveSheetFileList.Value.ToString()   : "null (T or F)";

			string a1 = A1.ToString();
			string a2 = A2.HasValue ? A2.Value.ToString() : "undefined";
			string a3 = A3.HasValue ? A3.Value.ToString() : "undefined";
			string a9 = A9.HasValue ? A9.Value.ToString() : "undefined";

			DM.Stat0($"required");
			DM.Stat0($"{" ".Repeat(24)}required       actual");
			DM.Stat0($"1) must have data file path    {s1,-16}vs {a1}");
			DM.Stat0($"2) must have data file         {s2,-16}vs {a2}");
			DM.Stat0($"3) must have data file sheets  {s3,-16}vs {a3}");
			DM.Stat0($"9) must have data file list    {s9,-16}vs {a9}");
			DM.Stat0($"Final answer| {finalAnswer}");

			string s12a = sm2.GotDataFile.HasValue ?  sm2.GotDataFile.Value.ToString() : "is null";
			string s12b = (SheetDataManager2.SheetsCount >= 0).ToString();
			string s13a = SheetDataManager2.GotDataSheets.ToString();
			string s13b = (SheetDataManager2.GotDataSheets == mustHaveDataFileSheets).ToString();
			string s19a = sm2.GotSheetFileList.ToString();
			string s19b = sm2.GotSheetFolder.ToString();

			DM.DbxLineEx(0, $"start", 1);
			DM.DbxLineEx(0, $"option {switchboardIdx}", 1);

			DM.DbxLineEx(0, $"{"final answer",-32}{finalAnswer}");
			DM.DbxLineEx(0, $"{"1 must have data file path"     ,-32}{s1,-8} | got path?   {SheetDataManager2.GotDataPath}");
			DM.DbxLineEx(0, $"{"2a must have data file?"        ,-32}{s2,-8} | got file?   {s12a}");
			DM.DbxLineEx(0, $"{"2a and can have sheets?"        ,-32}{s2,-8} | sht count   {s12b}");
			DM.DbxLineEx(0, $"{"3a data file may need sheets?"  ,-32}{s3,-8} | got sheets? {s13a ,-8}| count {SheetDataManager2.SheetsCount}");
			DM.DbxLineEx(0, $"{"3b but must match"              ,-32}{s3,-8} | got match?  {s13b}");
			DM.DbxLineEx(0, $"{"9a must have sht file list?"    ,-32}{s9,-8} | got list?   {s19a ,-8}| count {sm2.SheetFileList?.Count.ToString() ?? "null" }");
			DM.DbxLineEx(0, $"{"9b and must have folder?"       ,-32}{s9,-8} | got path?   {s19b}");

			DM.DbxLineEx(0, $"{SuiteSettings.Data.DataFilePath?.FullFilePath ?? "data file path is null"}");
			DM.DbxLineEx(0, $"{SuiteSettings.Data.ScanPDfFolder?.FullFilePath ?? "PDF folder path is null"}", -1);

			DM.DbxLineEx(0, "end", 0, -1);
		}
	}
}