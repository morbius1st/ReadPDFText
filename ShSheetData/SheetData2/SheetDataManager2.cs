using System.Collections.Generic;
using Settings;
using SettingsManager;
using ShSheetData.SheetData;
using ShSheetData.SheetData2;
using ShSheetData.ShSheetData2;
using UtilityLibrary;

namespace ShSheetData.ShSheetData
{
	public static class SheetDataManager2
	{
		public static bool Initialized { get; private set; } = false;
		public static bool SettingsFileExists => Path?.SettingFileExists ?? false;
		public static bool HasSheets => SheetsCount > 0;
		public static int SheetsCount => Data?.SheetDataList?.Count ?? -1;
		
		public static DataManager<SheetDataSet2> Manager { get; private set; }

		public static SheetDataSet2 Data => Manager?.Data ?? null;
		public static SettingsMgr<StorageMgrPath, StorageMgrInfo<SheetDataSet2>, SheetDataSet2> Admin => Manager?.Admin ?? null;
		public static StorageMgrInfo<SheetDataSet2> Info => Manager?.Info ?? null;
		public static StorageMgrPath Path => Admin?.Path ?? null;


		public static IEnumerable<KeyValuePair<string, SheetData2.SheetData>> GetSheets()
		{
			foreach (KeyValuePair<string, SheetData2.SheetData> kvp in Data.SheetDataList)
			{
				yield return kvp;

			}
		}

		public static IEnumerable<KeyValuePair<SheetRectId, SheetRectData2<SheetRectId>>> GetRects(string sheet)
		{
			foreach (KeyValuePair<SheetRectId, SheetRectData2<SheetRectId>> kvp in Data.SheetDataList[sheet].ShtRects)
			{
				yield return kvp;

			}
		}

		public static void Init(FilePath<FileNameSimple> filePath)
		{
			if (Manager != null) return;

			Manager = new DataManager<SheetDataSet2>(filePath);

			Initialized = true;
		}

		public static void Remove()
		{
			Manager.Reset();

			Write();

			Initialized = false;
		}

		public static void Read()
		{
			Admin.Read();
		}

		public static void Write()
		{
			Admin.Write();
		}

		public static void Reset()
		{
			Manager.Reset();
		}

		public static void SeeData()
		{
			StorageMgrInfo<SheetDataSet2> i1 = Manager.Info;
			StorageMgrInfo<SheetDataSet2> i2 = Admin.Info;
			
			SheetDataSet2 d1 = Manager?.Data;
			SheetDataSet2 d2 = Info.Data;
			
			i1.Description += " + i1 (manager)";
			i2.Description += " + i2 (admin)";
			
			d1.DataFileDescription += " + d1 (manager)";
			d2.DataFileDescription += " + d2 (info)";

			// int a = 1;
		}
	}
}

