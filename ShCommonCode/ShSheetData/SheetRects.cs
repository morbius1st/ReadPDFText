using System.Globalization;
using System.Runtime.Serialization;
using iText.Kernel.Geom;
// using ReadPDFTextTests.SheetData;
using SharedCode.ShDataSupport;

namespace ShCommonCode.ShSheetData;

[DataContract(Namespace = "")]
public class SheetRects
{
	private DateTime created;
	private float[] pageSizeWithRotationA;

	public SheetRects()
	{
		// ShtRects = new Dictionary<SheetMetricId, Rectangle>();
		ShtRects = new Dictionary<SheetRectId, SheetRectData<SheetRectId>>();
		OptRects = new Dictionary<SheetRectId, SheetRectData<SheetRectId>>();

		// ShtRects.Add(SheetMetricId.SM_SHT, new ShtRectData<SheetMetricId, Rectangle>(SheetRectType.SRT_NA, SheetMetricId.SM_SHT));
	}

	[DataMember(Order = 1)]
	public string Name { get; set; }
	[DataMember(Order = 2)]
	public string Description { get; set; }
	[IgnoreDataMember]
	public string Created
	{ 
		get => created.ToString("O");
		set => created = DateTime.Parse(value, CultureInfo.InvariantCulture);
	}

	[DataMember(Order = 3)]
	public DateTime CreatedDt
	{
		get => created;
		set
		{
			created = value;
		}
	}

	[IgnoreDataMember]
	public Rectangle PageSizeWithRotation
	{
		get => ShtRects[SheetRectId.SM_SHT].Rect!;
		set
		{
			if (ShtRects == null) return;

			if (!ShtRects.ContainsKey(SheetRectId.SM_SHT))
			{
				ShtRects.Add(SheetRectId.SM_SHT, new SheetRectData<SheetRectId>(SheetRectType.SRT_NA, SheetRectId.SM_SHT));
			}

			ShtRects[SheetRectId.SM_SHT].Rect = value;

			if (value != null)
			{
				pageSizeWithRotationA = new []
				{
					value.GetX(), value.GetY(), value.GetWidth(), value.GetHeight()
				};
			}
			else
			{
				pageSizeWithRotationA = null;
			}
		}
	}

	[DataMember(Order = 4)]
	public float[] PageSizeWithRotationA
	{
		get => pageSizeWithRotationA;
		set
		{
			if (value != null)
			{
				PageSizeWithRotation = new Rectangle(value[0], value[1], value[2], value[3]);
			}
			else
			{
				PageSizeWithRotation = null;
			}
			
		}
	}

	[IgnoreDataMember]
	public bool AllShtRectsFound => ShtRects.Count == SheetRectSupport.ShtRectsQty;
	[IgnoreDataMember]
	public bool AnyOptRectsFound => OptRects.Count > 0;

	// public Dictionary<SheetMetricId, Rectangle> ShtRects { get; set; }
	
	[DataMember(Order = 5)]
	public Dictionary<SheetRectId, SheetRectData<SheetRectId>> ShtRects { get; set; }
	[DataMember(Order = 6)]
	public Dictionary<SheetRectId, SheetRectData<SheetRectId>> OptRects { get; set; }
}