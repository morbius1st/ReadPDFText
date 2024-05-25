#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SettingsManager;
using ShCommonCode.ShSheetData;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   5/21/2024 11:11:51 PM

namespace CreatePDFBoxes.SheetData
{
	public class ReadData
	{
		public void Read()
		{
			// DataManager<SheetDataSet>?  Manager = 
			// 	new DataManager<SheetDataSet>(new FilePath<FileNameSimple>(SheetDataSetConsts.DataFilePath));
			//
			// Manager.Admin.Read();

			SheetDataManager.Read();
			
			SheetRectSupport.showShtRects();
			SheetRectSupport.ShowValues();
		}


		public override string ToString()
		{
			return $"this is {nameof(ReadData)}";
		}
	}
}
