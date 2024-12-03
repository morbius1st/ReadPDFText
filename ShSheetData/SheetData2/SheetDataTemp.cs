#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using iText.Layout.Properties;
using Settings;
using ShSheetData.ShSheetData2;
using ShSheetData.Support;
using UtilityLibrary;

#endregion

// username: jeffs
// created:  11/27/2024 5:25:59 PM

namespace ShSheetData.SheetData2
{
	public class SheetDataTemp
	{
	#region private fields

	#endregion

	#region ctor

		public SheetDataTemp() { }

	#endregion

	#region public properties

	#endregion

	#region private properties

	#endregion

	#region public methods

		public static void CreateTempSheetData2(SheetDataSet2 data, int shtQty, int[] shtBoxes, int optBoxes)
		{
			DM.Start0();

			if (data == null || shtBoxes.Length <= 0 || optBoxes < 0) return;

			data.SheetDataList = new Dictionary<string, SheetData2>();

			for (int i = 0; i < shtQty; i++)
			{
				SheetData2 sd2 = createSheetData2(i, shtBoxes, optBoxes);

				data.SheetDataList.Add(sd2.Name, sd2);
			}

			DM.End0();
		}

	#endregion

	#region private methods

		private static SheetData2 createSheetData2(int idx, int[] boxes, int optBoxes)
		{
			string name = tempSheetData[idx].Item1;
			string desc = tempSheetData[idx].Item2;

			DM.Stat0($"making sheet file | {name} - {desc}");

			SheetData2 sd2 = new SheetData2(name, desc);

			int i = 0;
			int j = 0;

			SheetRectData2<SheetRectId>  srd;

			foreach (KeyValuePair<string, SheetRectConfigData<SheetRectId>> 
						srcd in SheetRectConfigDataSupport.ShtRectIdXref)
			{
				if (boxes[j] != i++) continue;

				srd = makeTempShtRecData2(srcd.Value.Type, srcd.Value.Id, i, "Std");

				sd2.ShtRects.Add(srcd.Value.Id, srd);
				
				j++;

				if (j == boxes.Length) break;
			}

			i = 0;

			foreach (KeyValuePair<string, SheetRectConfigData<SheetRectId>> 
						srcd in SheetRectConfigDataSupport.OptRectIdXref)
			{
				if (i == optBoxes) break;

				if (srcd.Value.Id == SheetRectId.SM_NA) continue;

				srd = makeTempShtRecData2(srcd.Value.Type, srcd.Value.Id, i, "Opt");

				sd2.OptRects.Add(srcd.Value.Id, srd);

				i++;
			}

			sd2.SheetRotation = 0;
			sd2.PageSizeWithRotation = new Rectangle(tempSheetData[idx].Item3, tempSheetData[idx].Item4);

			return sd2;
		}

		private static SheetRectData2<SheetRectId> makeTempShtRecData2(SheetRectType type, SheetRectId id, int idx, string boxType)
		{
			float x = idx * 10 * 72;
			float y = x;
			float w = x;
			float h = x;

			SheetRectData2<SheetRectId> srd = new SheetRectData2<SheetRectId>(type, id);

			srd.SheetRotation = idx * 10;

			srd.BoxSettings = makeTempShtRectBoxSetg(x, y, w, h);

			srd.TextSettings = makeTempShtRectTextSetg(idx, boxType);

			return srd;
		}


		private static BoxSettings makeTempShtRectBoxSetg(float x, float y, float w, float h)
		{
			BoxSettings bxs = new BoxSettings(new Rectangle(x, y, w, h));
			bxs.BdrOpacity = (float) 0.5;
			bxs.FillOpacity = (float) 0.7;

			return bxs;
		}

		private static TextSettings makeTempShtRectTextSetg(int i, string boxType)
		{
			TextSettings txs = new TextSettings("Arial", 1, 12);
			txs.InfoText = $"this is {boxType} text {i}";
			txs.TextHorizAlignment = HorizontalAlignment.LEFT;

			return txs;
		}

	#endregion

	#region private data

		private static List<Tuple<string, string, float, float>> tempSheetData = 
			new List<Tuple<string, string, float, float>>()
		{
			new Tuple<string, string, float, float>("SHT_24x36_v1", "Standard 24 x 36 Sheet", (24*72), (36*72)),
			new Tuple<string, string, float, float>("SHT_30x42_v1", "Standard 30 x 42 Sheet", (30*72), (42*72)),
			new Tuple<string, string, float, float>("SHT_36x48_v1", "Standard 36 x 48 Sheet", (36*72), (48*72)),
			new Tuple<string, string, float, float>("SHT_22x34_v1", "Standard 22 x 34 Sheet", (22*72), (34*72)),
			new Tuple<string, string, float, float>("SHT_34x44_v1", "Standard 34 x 44 Sheet", (34*72), (44*72)),
		};

	#endregion

	#region event consuming

	#endregion

	#region event publishing

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(SheetDataTemp)}";
		}

	#endregion
	}
}