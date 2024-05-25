#region using directives

using System.Collections.Generic;
using System.Runtime.Serialization;
using iText.IO.Source;
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
	public class SheetMetricDataSet : IDataFile
	{
		[IgnoreDataMember]
		public static string DataFileName { get; } = "SheetMetricData.xml";

		[DataMember]
		public string DataFileDescription { get; set; } = "Sheet Metric Information";

		[DataMember]
		public string DataFileNotes { get; set; } = "Sheet Metrics is user specific / created";

		[DataMember]
		public string DataFileVersion { get; set; } = "v1.0";

		[DataMember(Order = 10)]
		public Dictionary<string, SheetMetricA> SheetMetricsA { get; set; }

		[IgnoreDataMember]
		public Dictionary<string, SheetMetric> SheetMetrics { get; set; }

	}
#endregion
}
