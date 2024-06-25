using System.Runtime.Serialization;
using iText.Kernel.Colors;


// User settings (per user) 
//  - user's settings for a specific app
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
		public string DataFileVersion => "user 7.4u";

		[IgnoreDataMember]
		public string DataFileDescription => "user setting file for SettingsManager v7.4";

		[IgnoreDataMember]
		public string DataFileNotes => "user / any notes go here";

		[DataMember(Order = 1)]
		public string pdfFolder { get; set; } = "c:\\";

		[DataMember(Order = 2)]
		public string xlsxFolder { get; set; } = "c:\\";

		[DataMember(Order = 3)]
		public string xlsxFile { get; set; } = "file.xlsx";

		[DataMember(Order = 4)]
		public string destFolder { get; set; } = "c:\\";

		// [DataMember(Order = 5)]
		// public RGB outlineColor { get; set; } = new RGB(255, 117, 0);

	}

#endregion

	[DataContract]
	public class RGB
	{
		[DataMember]
		public int R { get; set; }

		[DataMember]
		public int G { get; set; }

		[DataMember]
		public int B { get; set; }

		public RGB(int r, int g, int b)
		{
			R = r;
			G = g;
			B = b;
		}

		public DeviceRgb GetDeviceRgb => new DeviceRgb(R, G, B);
	}

}




// , APP_SETTINGS, SUITE_SETTINGS, MACH_SETTINGS, SITE_SETTINGS