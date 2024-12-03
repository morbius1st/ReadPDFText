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
		
	}

#endregion
}


// , APP_SETTINGS, SUITE_SETTINGS, MACH_SETTINGS, SITE_SETTINGS