using System.Runtime.Serialization;
using UtilityLibrary;

// User settings (per user) 
// - user's settings for a specific app
//	- located in the user's app data folder / app name


// ReSharper disable once CheckNamespace


namespace SettingsManager
{
#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Namespace = "")]
	public class UserSettingDataFile : IDataFile
	{
		[IgnoreDataMember]
		public string DataFileVersion {get; set; } = "1.0";

		[IgnoreDataMember]
		public string DataFileDescription {get; set; } = "user setting file forScanPDFBoxes";

		[IgnoreDataMember]
		public string DataFileNotes {get; set; } = "notes go here";

		// [DataMember(Order = 1)]
		// public int UserSettingsValue { get; set; } = 7;
		
/*	save for now

		// save last used information
		// assemble PDF's
			// PDF folder
			// sheet list file name
			// output folder / file name
		// assembly, create, and scan
			// data file folder & filename
		// scan
			// sheet file folder
		// create
			// output PDF file path

		// for assembly
		[DataMember(Order = 5)]
		public string ConfigSettingFilePathString 
		{
			get => ConfigSettingFilePath.FullFilePath;
			set => ConfigSettingFilePath = new FilePath<FileNameSimple>(value);
		}
		[DataMember(Order = 10)]
		public string PdfFolderName
		{
			get => PdfFolder.FullFilePath;
			set => PdfFolder = new FilePath<FileNameSimple>(value);
		}
		[DataMember(Order = 15)]
		public string SheetListFileName 
		{
			get => SheetListFilePath.FullFilePath;
			set => SheetListFilePath = new FilePath<FileNameSimple>(value);
		}

		[DataMember(Order = 20)]
		public string DestFileName 
		{
			get => DestFilePath.FullFilePath;
			set => DestFilePath = new FilePath<FileNameSimple>(value);
		}
		// for scan, create, and assembly
		[DataMember(Order = 25)]
		public string DataFilePathString 
		{
			get => DataFilePath.FullFilePath;
			set => DataFilePath = new FilePath<FileNameSimple>(value);
		}
		// for scan
		[DataMember(Order = 30)]
		public string ScanPdfFolder 
		{
			get => ScanPDfFolder.FullFilePath;
			set => ScanPDfFolder = new FilePath<FileNameSimple>(value);
		}
		// for create
		[DataMember(Order = 35)]
		public string CreatePdfFilePathString
		{
			get => CreatePdfFilePath.FullFilePath;
			set => CreatePdfFilePath = new FilePath<FileNameSimple>(value);
		}


		[IgnoreDataMember]
		public FilePath<FileNameSimple> ConfigSettingFilePath { get; set;}
		[IgnoreDataMember]
		public FilePath<FileNameSimple> PdfFolder { get; set;}
		[IgnoreDataMember]
		public FilePath<FileNameSimple> SheetListFilePath { get; set;}
		[IgnoreDataMember]
		public FilePath<FileNameSimple> DestFilePath { get; set;}

		// for scan, create, and assembly
		[IgnoreDataMember]
		public FilePath<FileNameSimple> DataFilePath { get; set;}

		// for scan
		[IgnoreDataMember]
		public FilePath<FileNameSimple> ScanPDfFolder { get; set;}

		// for create
		[IgnoreDataMember]
		public FilePath<FileNameSimple> CreatePdfFilePath { get; set; }
*/
	}

#endregion
}


// , APP_SETTINGS, SUITE_SETTINGS, MACH_SETTINGS, SITE_SETTINGS