#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;

using UtilityLibrary;


#endregion

// user name: jeffs
// created:   5/10/2024 6:29:04 PM

namespace SharedCode.ShDataSupport
{
	public enum SheetMetricId
	{
		SM_NA              = -1,
		SM_SHT             = 0 ,
		
		SM_SHT_NUM             ,
		SM_SHT_TITLE           ,
		SM_SHT_TITLE_BLOCK     ,

		SM_AUTHOR              ,
		SM_DISCLAIMER          ,
		SM_FOOTER              ,

		SM_BANNER_1ST          ,
		SM_BANNER_2ND          ,
		SM_BANNER_3RD          ,
		SM_BANNER_4TH          ,

		SM_WATERMARK           ,
	}



	/* box information needed
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
*/

	public class SheetMetric
	{
		public SheetMetric()
		{
			ShtRects = new Dictionary<SheetMetricId, Rectangle>();
			OptRects = new Dictionary<int, Rectangle>();
		}

		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime Created { get; set; }

		public Dictionary<SheetMetricId, Rectangle> ShtRects { get; set; }
		public Dictionary<int, Rectangle> OptRects { get; set; }

		public bool AllShtRectsFound => ShtRects.Count == SheetMetricsSupport.ShtRectsQty;
		public bool AnyOptRectsFound => OptRects.Count > 0;
	}

	[DataContract(Namespace = "")]
	public class SheetMetricA
	{
		/* process notes
		 * this only provides the information to be serialized
		 * this uses AltRectangle because IText rectangle does not serialize
		 * this provides the translation methods between the
		 * this, the altrectangles and the regular sheetmetrics class
		 */

		[DataMember]
		public string Name { get; set; }
		
		[DataMember]
		public string Description { get; set; }
		
		[DataMember]
		public DateTime Created { get; set; }

		// basic set of rectangles
		[DataMember]
		public List<AltRectangle> ShtRectsA { get; set; }

		// optional rectangles
		[DataMember]
		public List<AltRectangle> OptRectsA { get; set; }

		public override string ToString()
		{
			return $"this is {nameof(SheetMetric)}| sht rects {ShtRectsA.Count}| opt rects {OptRectsA.Count}";
		}
	}



}
