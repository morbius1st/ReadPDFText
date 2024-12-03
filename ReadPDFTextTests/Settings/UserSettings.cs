using System.Runtime.Serialization;
using UtilityLibrary;

// User settings (per user) 
// - user's settings for a specific app
//	- located in the user's app data folder / app name


// ReSharper disable once CheckNamespace
// ; APP_SETTINGS; SUITE_SETTINGS; MACH_SETTINGS; SITE_SETTINGS

// refer to user data class in ScanPDFBoxes for the original properties 
// these properties were moved to the suite settings file

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
		public string DataFileDescription {get; set; } = "user setting file for ReadPDFTextTests";

		[IgnoreDataMember]
		public string DataFileNotes {get; set; } = "User notes here";

		// [DataMember(Order = 1)]
		// public int UserSettingsValue { get; set; } = 7;


	}

#endregion
}


