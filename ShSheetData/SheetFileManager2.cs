#region + Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SettingsManager;
using ShSheetData.ShSheetData2;
using ShTempCode.DebugCode;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   6/22/2024 6:23:15 AM

namespace ShSheetData
{
	public class SheetFileManager2
	{
		// public const string DATA_FILE_NAME  = "SheetData.xml";

		private FilePath<FileNameSimple> dataFilePath;
		private string dataFileFolder2;
		private string dataFileName2;
		private string sheetFileFolder;
		private string outputFileFolder;
		private string outputFileName;


		// public bool? ScanOkToProceed { get; private set; }
		//
		// public bool ConfigScan(bool? reqdGotDataFileValue)
		// {
		// 	if (ScanOkToProceed == true) return true;
		//
		// 	ScanOkToProceed = false;
		//
		// 	DM.DbxLineEx(0, "start", 1);
		// 	// presumption is that the settings have been made before this is run
		// 	// do not check, just proceed as if this is true;
		//
		// 	if (!configScanSheetFiles())
		// 	{
		// 		DM.DbxLineEx(0, "end 1", 0, -1);
		// 		return false;
		// 	}
		//
		// 	bool result = configScanDataFile(reqdGotDataFileValue);
		// 	
		// 	// if reqdGotDataFileValue == true && configScanDataFile() == true - final answer is true
		// 	//		if does not match, final answer is false
		// 	// if reqdGotDataFileValue == false && configScanDataFile() == false - final answer is true
		// 	//		if does not match, final answer is false
		// 	// if reqdGotDataFileValue == null - result can be true or false
		// 	//		if result == true, scanready == true, else scan ready == null
		//
		// 	if (reqdGotDataFileValue.HasValue)
		// 	{
		// 		if (reqdGotDataFileValue != result)
		// 		{
		// 			DM.DbxLineEx(0, "end 2", 0, -1);
		// 			return false;
		// 		}
		//
		// 		ScanOkToProceed = true;
		// 	}
		// 	else
		// 	{
		// 		ScanOkToProceed = null;
		// 	}
		//
		// 	DM.DbxLineEx(0, "end", 0, -1);
		//
		// 	return true;
		// }
		//
		// private bool configScanSheetFiles()
		// {
		// 	DM.DbxLineEx(0, "start", 1);
		//
		// 	SetSheetFileFolder(UserSettings.Data.ScanPDfFolder.FolderPath);
		//
		// 	if (!GotSheetFolder)
		// 	{
		// 		DM.DbxLineEx(0, "end 1", 0, -1);
		// 		return false;
		// 	}
		//
		// 	GetSheetFiles();
		//
		// 	if (!GotSheetFiles)
		// 	{
		// 		DM.DbxLineEx(0, "end 2", 0, -1);
		// 		return false;
		// 	}
		//
		// 	DM.DbxLineEx(0, "end", 0, -1);
		//
		// 	return true;
		// }
		//
		// /// <summary>
		// /// determine if the data file does or does not exist<br/>
		// /// return: true, does and must exist, false, does not exist
		// /// </summary>
		// /// <param name="reqdGotDataFileValue"></param>
		// /// <returns></returns>
		// private bool configScanDataFile(bool? reqdGotDataFileValue)
		// {
		// 	DM.DbxLineEx(0, "start", 1);
		//
		// 	SetDataFileFolder(UserSettings.Data.DataFilePath.FolderPath);
		// 	SetDataFileName(UserSettings.Data.DataFilePath.FileName);
		//
		// 	// gotdatafile does not matter, return the status of the data file
		// 	if (!reqdGotDataFileValue.HasValue)
		// 	{
		// 		ScanOkToProceed = null;
		// 		DM.DbxLineEx(0, "end B", -1);
		// 		return GotDataFile == true;
		// 	}
		//
		// 	// if reqdGotDataFileValue == true, GotDataFile must == true
		// 	// if reqdGotDataFileValue == false, GotDataFile must == false
		// 	if (reqdGotDataFileValue != GotDataFile)
		// 	{
		// 		DM.DbxLineEx(0, "end 1", 0, -1);
		// 		return false;
		// 	}
		//
		// 	// per the above, reqdGotDataFileValue and GotDataFile are both false
		// 	if (reqdGotDataFileValue == false)
		// 	{
		// 		DM.DbxLineEx(0, "end C", -1);
		// 		return false;
		// 	}
		//
		// 	// must be last settings to keep scan ready correct
		// 	// get here when both are true
		//
		// 	// todo is this correct
		// 	// SheetDataManager2.Open(DataFilePath);
		//
		// 	DM.DbxLineEx(0, "end A", -1);
		//
		// 	return true;
		// }


	#region data file operations

		// select the location of the data file
		// select the location of the sheet files
		// process the sheet files - make a list of only the PDF files
		//		nothing fancy - just use the list of files as is

		// select folder
		// get file
		// delete file

		/// <summary>  data file status - true, exists / false, not found or location set / null, new file, location set
		/// </summary>
		public bool? GotDataFile
		{
			get
			{
				if (DataFilePath == null) return null;

				return DataFilePath.Exists;
			}
		}

		public bool GotDataFilePath => DataFilePath != null;

		public string DataFileFolder => dataFilePath.FolderPath;

		public FilePath<FileNameSimple> DataFilePath
		{
			get => dataFilePath;
			set
			{
				dataFilePath = value;
			}
		}

		public bool SetDataFileFolder(string folder)
		{
			if (dataFilePath != null) return false;

			dataFileFolder2 = folder;

			makeDataFilePath();

			return true;
		}

		public bool SetDataFileName(string name)
		{
			if (GotDataFile.Equals(true) || name == null) return false;

			dataFileName2 = name;

			makeDataFilePath();

			return true;
		}

		private void makeDataFilePath()
		{
			if (dataFileName2 == null || dataFileFolder2 == null) return;

			DataFilePath = new FilePath<FileNameSimple>(new [] { dataFileFolder2, dataFileName2 });

		}

		public bool DeleteDataFile()
		{
			if (!GotDataFile.Equals(true) ||
				!DataFilePath.Exists ) return false;

			File.Delete(DataFilePath.FullFilePath);

			ResetDataFile();

			return true;
		}

		public void ResetDataFile()
		{
			dataFileFolder2 = null;
			dataFileName2 = null;
			DataFilePath = null;
		}

	#endregion

	#region sheet file operations

		// select folder
		// get the list of files

		/// <summary>  sheet files status - true, got files / false, not selected or not found
		/// </summary>
		public bool GotSheetFileList
		{
			get
			{
				if (sheetFileFolder == null) return false;

				return SheetFileList != null && SheetFileList.Count > 0;
			}
		}

		/// <summary> Folder status - true, got and exists / false not got or does not exist
		/// </summary>
		public bool GotSheetFolder
		{
			get
			{
				return sheetFileFolder != null;
			}
		}

		public bool SheetFolderExists
		{
			get
			{
				return GotSheetFolder && 
					Directory.Exists(sheetFileFolder);
			}
		}

		/// <summary> access to folder
		/// </summary>
		public string SheetFileFolder
		{
			get => sheetFileFolder;
			private set => sheetFileFolder = value;
		}

		/// <summary> access to file list
		/// </summary>
		public List<string> SheetFileList { get; set; }

		/// <summary> get the SheetFileList as a dictionary with
		/// index number (string, 1 based) as the key and the value is a
		/// tuple (filename, filepath) 
		/// </summary>
		public Dictionary<string, Tuple<string, string>> SheetFileDictionary
		{
			get
			{
				int idx = 1;
				string fmt;

				Tuple<string, string> t1;

				string filename;

				Dictionary<string, Tuple<string, string>> dict = new ();

				fmt = SheetFileList.Count > 9 ? "00" : "0";

				foreach (string s in SheetFileList)
				{
					filename = Path.GetFileNameWithoutExtension(s);

					t1 = new Tuple<string, string>(filename, s);

					dict.Add(idx++.ToString(fmt), t1);
				}

				return dict;
			}
		}
		
		// allow a non-existent folder with the expectation that
		// it will exist before being used
		public bool SetSheetFileFolder(string folder = null)
		{
			if (sheetFileFolder != null) return false;

			bool result = true;
			// if folder is not null, use it.  else, request

			if (folder != null)
			{
				sheetFileFolder = folder;
			}
			// develop later
			// else
			// {
			// 	// request folder
			// 	// if cancel request, result = false
			// }

			return result;
		}

		public bool GetSheetFiles()
		{
			if (!SheetFolderExists) return false;

			try
			{
				// get the list of PDF files
				SheetFileList = new List<string>(Directory.GetFiles(SheetFileFolder, "*.pdf"));
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return false;
			}

			return SheetFileList != null && SheetFileList.Count > 0;
		}

		/// <summary>clears the current list of PDF files and the sheetFileFolder
		/// </summary>
		public void ResetSheetFiles()
		{
			if (GotSheetFileList)
			{
				SheetFileList.Clear();
			}

			sheetFileFolder = null;
		}

	#endregion

	#region output file operations

		// select folder and name
		// file exists
		// set option - overwrite or not

		/// <summary>
		/// output file status - true, got location and file does NOT exist / false, got location and file EXISTS / null, not got location or filename
		/// </summary>
		public bool? GotOutputFile
		{
			get
			{
				if (outputFileFolder == null) return null;

				if (OutputFilePath == null) return null;

				bool result = OutputFilePath.Exists;

				if (!result) return true;

				if (OverwriteOutputFile) return true;

				return false;
			}
		}

		public bool OverwriteOutputFile { get; set; } = true;

		public string OutputFileName
		{
			get => outputFileName;
			set
			{
				if (value == null)
				{
					outputFileName = null;
					return;
				}

				outputFileName = value;

				setOutputFilePath();
			}
		}

		public string OutputFileFolder
		{
			get => outputFileFolder;
			set
			{
				if (value == null ||
					!Directory.Exists(value))
				{
					outputFileFolder = null;
					return;
				}

				outputFileFolder = value;

				setOutputFilePath();
			}
		}

		public static FilePath<FileNameSimple> OutputFilePath { get; set; }

		/// <summary>Deletes the Output PDF file<br/>true, deleted or does not exist<br/>false, could not delete
		/// </summary>
		public bool DeleteOutputFile()
		{
			if (!OutputFilePath.Exists) return true;

			try
			{
				File.Delete(OutputFilePath.FullFilePath);
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>   set the file path value - no judgement about the file itself
		/// </summary>
		private bool setOutputFilePath()
		{
			if (outputFileFolder == null || OutputFileName == null) return false;

			// both have values

			try
			{
				OutputFilePath = new FilePath<FileNameSimple>(new [] { outputFileFolder, OutputFileName });
			}
			catch
			{
				return false;
			}

			return true;
		}

	#endregion

		public override string ToString()
		{
			return $"data {DataFilePath?.FileName ?? "not set"} | sht count {SheetFileList?.Count.ToString() ?? "not set"} | output {OutputFilePath?.FileName ?? "not set"}";
		}
	}
}