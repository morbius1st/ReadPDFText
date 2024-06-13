#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreatePDFBoxes.PdfSupport;
using Settings;
using SettingsManager;
using ShCommonCode.ShSheetData;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   5/21/2024 11:12:29 PM

namespace CreatePDFBoxes.SheetData
{
	/*
	public class ProcessRects
	{
		public void Process(string datafilepath)
		{
			SheetDataManager.Init(new FilePath<FileNameSimple>(datafilepath));

			ReadRects();
		}

		private void ReadRects()
		{
			SheetDataManager.Read();

			ProcessPdfs.showStatus("@read rects");
		}

		public SheetRects? GetSheetRects(string sheetName)
		{
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
	*/
}
