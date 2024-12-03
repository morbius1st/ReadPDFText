using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using iText.Kernel.Geom;
using ShSheetData.SheetData;
using ShSheetData.ShSheetData2;
using ShSheetData.Support;

namespace ShSheetData.SheetData2
{
	[DataContract(Namespace = "")]
	public class SheetData2
	{
		private DateTime created;
		private float[] sheetSizeWithRotationA;

		public SheetData2(string name, string desc)
		{
			Name=name;
			Description = desc;
			CreatedDt = DateTime.Now;

			ShtRects = new Dictionary<SheetRectId, SheetRectData2<SheetRectId>>();
			OptRects = new Dictionary<SheetRectId, SheetRectData2<SheetRectId>>();
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
			set { created = value; }
		}

		[DataMember(Order = 4)]
		public int SheetRotation { get; set; }

		/// <summary>
		/// sets the rotated page size in the sheet rect<br/>
		/// will add a sheet rect if one does not exist
		/// </summary>
		[IgnoreDataMember]
		public Rectangle PageSizeWithRotation
		{
			get => ShtRects[SheetRectId.SM_SHT].BoxSettings.Rect;
			set
			{
				if (ShtRects == null) return;

				if (!ShtRects.ContainsKey(SheetRectId.SM_SHT))
				{
					ShtRects.Add(SheetRectId.SM_SHT, new SheetRectData2<SheetRectId>(SheetRectType.SRT_NA, SheetRectId.SM_SHT));
				}

				ShtRects[SheetRectId.SM_SHT].BoxSettings.Rect = value;

				if (value != null)
				{
					sheetSizeWithRotationA = new []
					{
						value.GetX(), value.GetY(), value.GetWidth(), value.GetHeight()
					};
				}
				else
				{
					sheetSizeWithRotationA = null;
				}
			}
		}

		[DataMember(Order = 5)]
		public float[] SheetSizeWithRotationA
		{
			get => sheetSizeWithRotationA;
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
		public bool IsComplete => AllShtRectsFound;

		[IgnoreDataMember]
		public bool AllShtRectsFound => ShtRects.Count >= SheetRectConfigDataSupport.ShtRectsMinQty;

		[IgnoreDataMember]
		public bool AnyOptRectsFound => OptRects.Count > 0;

		[DataMember(Order = 5)]
		public Dictionary<SheetRectId, SheetRectData2<SheetRectId>> ShtRects { get; set; }

		[DataMember(Order = 6)]
		public Dictionary<SheetRectId, SheetRectData2<SheetRectId>> OptRects { get; set; }
	}

}