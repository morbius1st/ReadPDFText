#region + Using Directives

using System.Collections.Generic;
using iText.Layout.Properties;
using ShSheetData.SheetData;
using ShSheetData.Support;

#endregion

// user name: jeffs
// created:   6/16/2024 9:26:58 AM

namespace ReadPDFManager.Process
{
	public class TbOriginAdjustSamples
	{
		public static Dictionary<string, SheetRectData<SheetRectId>> SrdSamples { get; set; }


		public static void CreateSamples(List<SampleBox> boxes)
		{
			SrdSamples = new  Dictionary<string, SheetRectData<SheetRectId>>();

			SheetRectData<SheetRectId> srd;

			foreach (SampleBox box in boxes)
			{
				srd = new SheetRectData<SheetRectId>(SheetRectType.SRT_BOX, box.Id, null);
				srd.InfoText = box.NameEx;
				srd.Rect = box.TbRect;
				srd.TextBoxRotation = box.TbRotation;
				srd.TextHorizAlignment = (HorizontalAlignment) box.HorizAlign;
				srd.TextVertAlignment = (VerticalAlignment) box.VertAlign;
				SrdSamples.Add(box.NameEx, srd);
			}
		}


	}
}
