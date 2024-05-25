#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Settings;
using SettingsManager;
using ShCommonCode.ShSheetData;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   5/21/2024 11:12:29 PM

namespace CreatePDFBoxes.SheetData
{
	public class ProcessRects
	{
		private ReadData rd;


		public void Process()
		{
			SheetRectSupport.FilePath = new FilePath<FileNameSimple>(SheetDataSetConsts.DataFilePath);

			SheetDataManager.Init(SheetRectSupport.FilePath);

			ReadRects();
		}

		private void ReadRects()
		{
			rd=new ReadData();

			rd.Read();
		}

		public SheetRects? GetSheetRects(string sheetName)
		{
			if (!SheetDataManager.Initialized) return null;

			bool result;
			SheetRects? sheetRects;

			result=SheetDataManager.Data!.SheetRectangles.TryGetValue(sheetName, out sheetRects);

			if (!result) return null;

			return sheetRects;
		}


		public override string ToString()
		{
			return $"this is {nameof(ProcessRects)}";
		}
	}
}
