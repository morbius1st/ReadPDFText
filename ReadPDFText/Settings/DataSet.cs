#region using directives

using System.Collections.Generic;
using System.Runtime.Serialization;
using SharedCode.ShDataSupport;
using SettingsManager;
#endregion

// in code, after creating the data file for the first time, set the
// header values for 
// {dataset}.Info.Description
// {dataset}.Info.DataClassVersion
// {dataset}.Info.Notes
// the meaning of all three are up to you, however, the dataclass version
// is used to determine if the dataset has been revised and needs an upgrade

namespace SettingsManager
{

#region data class

	[DataContract(Namespace = "")]
	public class DataStorageSet : IDataFile
	{
		[DataMember]
		public string DataFileDescription { get; set; } = "Sheet Metric Information";

		[DataMember]
		public string DataFileNotes { get; set; } = "Sheet Metrics is user specific / created";

		[DataMember]
		public string DataFileVersion { get; set; } = "v1.0";


		// [DataMember(Order = 1)]
		// public string SampleDataString1 { get; set; } = "this is a sample string";
		//
		// [DataMember(Order = 2)]
		// public int SampleDataInt1 { get; set; } = 72;
		//
		// [DataMember(Order = 3)]
		// public double SampleDataDouble1 { get; set; } = 7.4;

		[DataMember(Order = 10)]
		public Dictionary<SheetBorderType, SheetData> SheetData { get; set; }

		// [DataMember(Order = 10)]
		// public Dictionary<string, SheetMetricA> SheetMetricA { get; set; }
	}





	// // this is the actual data set saved to the user's configuration file
	// // this is unique for each program
	// [DataContract(Namespace = "")]
	// public class DataSet 
	// {
	// 	[DataMember(Order = 1)]
	// 	public string SampleDataString1 { get; set; } = "this is a sample string";
	// 	
	// 	[DataMember(Order = 2)]
	// 	public int SampleDataInt1 { get; set; } = 72;
	// 	
	// 	[DataMember(Order = 3)]
	// 	public double SampleDataDouble1 { get; set; } = 7.4;
	//
	// }

#endregion
}
