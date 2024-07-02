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

namespace Settings
{
#region data class

	[DataContract(Namespace = "")]
	public class SheetDataSet : IDataFile
	{
		[IgnoreDataMember]
		public static string DataFileName { get; } = "SheetData.xml";

		[IgnoreDataMember]
		public string DataFileDescription { get; set; } = "Sheet Metric Information";

		[IgnoreDataMember]
		public string DataFileNotes { get; set; } = "Sheet Metrics is user specific / created";

		[IgnoreDataMember]
		public string DataFileVersion { get; set; } = "v1.0x";

		[IgnoreDataMember]
		// public Dictionary<string, SheetMetricA> SheetMetricsA { get; set; } = new Dictionary<string, SheetMetricA>();

		[DataMember(Order = 10)]
		public Dictionary<string, SheetRects> SheetRectangles { get; set; }= new Dictionary<string, SheetRects>();


	}
#endregion
}
