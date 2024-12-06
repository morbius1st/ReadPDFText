using System.Runtime.Serialization;
using UtilityLibrary;

// Suite settings (per user)
//	- applies to a specific app suite (multiple programs)
//	- holds information needed by all programs in the suite, but not all users of the suite
//	- provides the pointer to the site settings file (why here: allows each user to be associated with different site files)
//	- located in the user's app data folder

// ReSharper disable once CheckNamespace

namespace SettingsManager
{
#region suite data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Namespace = "")]
	public class SuiteSettingDataFile : IDataFile
	{
		[IgnoreDataMember]
		public string DataFileVersion => "2.0 2024-12-01";

		[IgnoreDataMember]
		public string DataFileDescription => "suite setting file for suite Andy";

		[IgnoreDataMember]
		public string DataFileNotes => "contains shared folder and path information";

		// [DataMember(Order = 1)]
		// public int SuiteSettingsValue { get; set; } = 7;

		[DataMember(Order = 2)]
		public string SiteRootPath { get; set; }
			= @"C:\Users\jeffs\AppData\Roaming\CyberStudio\SettingsManager\SettingsManagerv75\SiteSettings" ;


				// for assembly
		[DataMember(Order = 5)]
		public string ConfigSettingFilePathString 
		{
			get => ConfigSettingFilePath.FullFilePath;
			set => ConfigSettingFilePath = new FilePath<FileNameSimple>(value);
		}
		[DataMember(Order = 10)]
		public string PdfFolderPathString
		{
			get => PdfFolder.FullFilePath;
			set => PdfFolder = new FilePath<FileNameSimple>(value);
		}
		[DataMember(Order = 15)]
		public string SheetListFilePathString
		{
			get => SheetListFilePath.FullFilePath;
			set => SheetListFilePath = new FilePath<FileNameSimple>(value);
		}

		[DataMember(Order = 20)]
		public string DestFilePathString
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
		public string ScanPdfFolderPathString
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

		[DataMember(Order = 35)]
		public string BlankTitleBlockSamplesFilePathString
		{
			get => BlankTitleBlockSamplesFilePath.FullFilePath;
			set => BlankTitleBlockSamplesFilePath = new FilePath<FileNameSimple>(value);
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

		[IgnoreDataMember]
		public FilePath<FileNameSimple> BlankTitleBlockSamplesFilePath { get; set;}

	}

#endregion
}