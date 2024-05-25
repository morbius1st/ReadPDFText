#region + Using Directives

#endregion

// user name: jeffs
// created:   5/10/2024 6:29:04 PM

using System.Globalization;
using System.Runtime.Serialization;
using ReadPDFTextTests.SheetData;
using ShCommonCode.ShSheetData;
/*
namespace SharedCode.ShDataSupport
{
	*//* box information needed
* - sheet size
* - sheet number
* - sheet title
* - author box
* - disclaimer box
* - footer
* - banner 1
* - banner 2
* - banner 3
* - banner 4
* - watermark 
* - optional url 1 through 9
*//*

	[DataContract(Namespace = "")]
	public class SheetMetricA
	{
		private DateTime created;

		*//* process notes
		 * this only provides the information to be serialized
		 * this uses AltRectangle because IText rectangle does not serialize
		 * this provides the translation methods between the
		 * this, the altrectangles and the regular sheetmetrics class
		 *//*

		[DataMember]
		public string Name { get; set; }
		
		[DataMember]
		public string Description { get; set; }
		
		[DataMember]
		public string Created
		{ 
			get => created.ToString("O");
			set => created = DateTime.Parse(value, CultureInfo.InvariantCulture);
		}

		[IgnoreDataMember]
		public DateTime CreatedDt
		{
			get => created;
			set => created = value;
		}

		// basic set of rectangles
		[DataMember]
		// public List<AltRectangle> ShtRectsA { get; set; }
		// public Dictionary<SheetMetricId, ShtRectInfo<SheetMetricId>> ShtRectsA { get; set; } = new ();
		public Dictionary<SheetMetricId, SheetRectData<SheetMetricId>> ShtRectsA { get; set; }=new ();

		// optional rectangles
		[DataMember]
		public Dictionary<SheetMetricId, SheetRectData<SheetMetricId>> OptRectsA { get; set; }=new ();

		public override string ToString()
		{
			return $"this is {nameof(SheetRects)}| sht rects {ShtRectsA.Count}| opt rects {OptRectsA.Count}";
		}
	}



}
*/