#region using directives

#endregion


// this is a preface file that will apply to all saved data files
// setting files as well as data files
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

namespace Settings
{
#region data class

	[DataContract(Namespace = "")]
	public class DataSet : IDataFile
	{
		[IgnoreDataMember]
		public string DataFileDescription { get; set; } = "Sheet Box Information";

		[IgnoreDataMember]
		public string DataFileNotes { get; set; } = "Sheet Box Info is user specific / created";

		[IgnoreDataMember]
		public string DataFileVersion { get; set; } = "v1.1";

	}
#endregion
}
