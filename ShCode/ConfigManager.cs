#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SettingsManager;
using ShSheetData;
using ShSheetData.SheetData2;
using ShSheetData.ShSheetData2;
using ShTempCode.DebugCode;

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
		private SheetFileManager2 sfm2;
		private SheetManager2 sm2;

		public ConfigManager(SheetFileManager2 sfm2, SheetManager2 sm2)
		{
			this.sfm2 = sfm2;

			this.sm2 = sm2;
		}


		// which 0 or 2 get data file | 1 or 2 get sheetPDF folder
		public bool SelectScanConfigFiles(int which, int def, bool selectDefault = false)
		{
			if (def == 2) return true;

			DM.DbxLineEx(0, "\tSelectScanConfigFiles");
			if (!getScanData(which, def, selectDefault)) return false;

			return true;
		}

		private bool getScanData(int which, int def, bool selectDefault)
		{
			DM.DbxLineEx(0, "\tselectScanFromSample");
			ShSamples ss = new ShSamples();

			if (!ss.SelectScanSample(def, selectDefault)) return false;

			Sample s = ss.Selected;

			if (which == 0 || which == 2)
			{
				UserSettings.Data.DataFilePath = s.DataFilePath;
			}

			if (which == 1 || which == 2)
			{
				UserSettings.Data.ScanPDfFolder = s.ScanPDfFolder;
			}

			return true;
		}

		/// <summary>
		/// select the configuration settings
		/// </summary>
		public bool configGetScanInfo(int def)
		{
			DM.DbxLineEx(0, "start", 1);

			if (def < 0 || SheetDataManager2.GotDataPath)
			{
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			if (!SelectScanConfigFiles(2, def, true))
			{
				DM.DbxLineEx(0, "end 2", 0, -1);
				return false;
			}

			DM.DbxLineEx(0, "end", 0, -1);

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

			return sfm2.GotDataFilePath;
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
		/// ** config list of scan PDF files
		/// </summary>
		public bool configSheetPdfScanFiles()
		{
			DM.DbxLineEx(0, "start", 1);

			if (!sfm2.SheetFolderExists)
			{
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			DM.DbxLineEx(0, "end", 0, -1);

			return sfm2.GetSheetFiles();
		}

	}
}
