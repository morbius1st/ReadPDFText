#region + Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShSheetData.ShSheetData2;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   6/22/2024 6:23:15 AM

namespace ShSheetData
{
	public class SheetFileManager2
	{
		// public const string DATA_FILE_NAME  = "SheetData.xml";

		private string dataFileFolder;
		private string sheetFileFolder;
		private string outputFileFolder;
		private string outputFileName;


		// select the location of the data file
		// select the location of the sheet files
		// process the sheet files - make a list of only the PDF files
		//		nothing fancy - just use the list of files as is

	#region data file operations

		// select folder
		// get file
		// delete file

		/// <summary>  data file status - true, exists / false, not found or location set / null, new file, location set
		/// </summary>
		public bool? GotDataFile
		{
			get
			{
				if (dataFileFolder == null) return false;

				if (DataFilePath == null) return null;

				return DataFilePath.Exists;
			}
		}

		public bool DataFilePathInit => (dataFileFolder != null && DataFilePath != null);

		public string DataFileFolder
		{
			get => dataFileFolder;
			private set
			{
				if (value == null ||
					!Directory.Exists(value))
				{
					dataFileFolder = null;
					return;
				}

				dataFileFolder = value;
			}
		}

		public static FilePath<FileNameSimple> DataFilePath { get; set; }

		public bool GetDataFileFolder(string folder = null)
		{
			if (dataFileFolder != null) return false;

			bool result = true;

			if (folder != null)
			{
				dataFileFolder = folder;
			}
			// develop later
			// else
			// {
			// 	// request folder
			// 	// if cancel request, result = false
			// }


			// use folder picker to get the location
			// temp - assign a location


			return result;
		}

		public bool GetDataFile(string name = null, string path = null)
		{
			if (GotDataFile.Equals(true)) return false;

			if (name == null)
			{
				name = SheetDataManager2.DataFileName;
			}

			if (path != null)
			{
				DataFileFolder = path;
			}

			if (dataFileFolder == null ) return false;

			DataFilePath = new FilePath<FileNameSimple>(new [] { dataFileFolder, name });

			return GotDataFile.Equals(true);
		}

		public bool DeleteDataFile()
		{
			if (!GotDataFile.Equals(true) ||
				!DataFilePath.Exists ) return false;

			File.Delete(DataFilePath.FullFilePath);

			dataFileFolder = null;
			DataFilePath = null;

			return true;
		}

		public void ResetDataFile()
		{
			dataFileFolder = null;
			DataFilePath = null;
		}

	#endregion

	#region sheet file operations

		// select folder
		// get the list of files

		/// <summary>  sheet files status - true, got files / false, not selected or not found
		/// </summary>
		public bool GotSheetFiles
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
				if (sheetFileFolder == null) return false;

				return Directory.Exists(sheetFileFolder);
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
		public List<string> SheetFileList { get; private set; }

		// allow a non-existent folder with the expectation that
		// it will exist before being used
		public bool GetSheetFileFolder(string folder = null)
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
			if (SheetFileFolder == null) return false;

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
			if (GotSheetFiles)
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