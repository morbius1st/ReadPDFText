#region using directives

#endregion

// in code, after creating the data file for the first time, set the
// header values for 
// {dataset}.Info.Description
// {dataset}.Info.DataClassVersion
// {dataset}.Info.Notes
// the meaning of all three are up to you, however, the dataclass version
// is used to determine if the dataset has been revised and needs an upgrade

using System.Collections.Generic;
using System.Runtime.Serialization;
using SettingsManager;
using ShSheetData.SheetData;
using ShSheetData.SheetData2;

namespace Settings
{
#region data class

	[DataContract(Namespace = "")]
	public class ScanData: IDataFile
	{
		[IgnoreDataMember]
		public static string DataFileName { get; } = "ScanData.xml";

		[IgnoreDataMember]
		public string DataFileDescription { get; set; } = "Scan Data Information";

		[IgnoreDataMember]
		public string DataFileNotes { get; set; } = "Scan Data is user specific / created";

		[IgnoreDataMember]
		public string DataFileVersion { get; set; } = "v1.0";

		// actual data saved to the data file




	}
#endregion
}
