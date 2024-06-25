using System.Collections.Generic;
using Settings;
using SettingsManager;
using UtilityLibrary;

namespace ShSheetData.SheetData
{
	public static class SheetDataManager
	{
		public static bool Initialized { get; private set; } = false;

		public static DataManager<SheetDataSet> Manager { get; private set; }

		public static SheetDataSet Data => Manager?.Data ?? null;
		public static SettingsMgr<StorageMgrPath, StorageMgrInfo<SheetDataSet>, SheetDataSet> Admin => Manager?.Admin ?? null;
		public static StorageMgrInfo<SheetDataSet> Info => Manager?.Info ?? null;
		public static StorageMgrPath Path => Admin?.Path ?? null;

		public static int SheetsCount => Data?.SheetRectangles?.Count ?? -1;
		public static int SheetMetricsCount => Data?.SheetRectangles?.Count ?? -1;

		public static bool SettingsFileExists => Path.SettingFileExists;

		public static IEnumerable<KeyValuePair<string, SheetRects>> GetSheets()
		{
			foreach (KeyValuePair<string, SheetRects> kvp in Data.SheetRectangles)
			{
				yield return kvp;

			}
		}

		public static IEnumerable<KeyValuePair<SheetRectId, SheetRectData<SheetRectId>>> GetRects(string sheet)
		{
			foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp in Data.SheetRectangles[sheet].ShtRects)
			{
				yield return kvp;

			}
		}

		public static void Init(FilePath<FileNameSimple> filePath)
		{
			if (Manager != null) return;

			Manager = new DataManager<SheetDataSet>(filePath);

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

		public static void SeeData()
		{
			StorageMgrInfo<SheetDataSet> i1 = Manager.Info;
			StorageMgrInfo<SheetDataSet> i2 = Admin.Info;
			
			SheetDataSet d1 = Manager?.Data;
			SheetDataSet d2 = Info.Data;
			
			i1.Description += " + i1 (manager)";
			i2.Description += " + i2 (admin)";
			
			d1.DataFileDescription += " + d1 (manager)";
			d2.DataFileDescription += " + d2 (info)";

			// int a = 1;
		}
	}
}

