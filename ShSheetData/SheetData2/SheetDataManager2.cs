using System.Collections.Generic;
using System.Diagnostics;
using Settings;
using SettingsManager;
using ShSheetData.SheetData;
using ShSheetData.SheetData2;
using ShSheetData.ShSheetData2;
using ShTempCode.DebugCode;
using UtilityLibrary;

namespace ShSheetData.ShSheetData2
{
	public static class SheetDataManager2
	{
		public static string DataFileName => SheetDataSet2.DataFileName;

		public static bool Initialized { get; private set; } = false;
		public static bool SettingsFileExists => Path?.SettingFileExists ?? false;
		public static bool HasSheets => SheetsCount > 0;
		public static int SheetsCount => Data?.SheetDataList?.Count ?? -1;

		// public static Dictionary<string, SheetData2.SheetData2> SheetDataList => Data?.SheetDataList;
		
		public static DataManager<SheetDataSet2> Manager { get; private set; }

		public static SheetDataSet2 Data => Manager?.Data ?? null;
		public static SettingsMgr<StorageMgrPath, StorageMgrInfo<SheetDataSet2>, SheetDataSet2> Admin => Manager?.Admin ?? null;
		public static StorageMgrInfo<SheetDataSet2> Info => Manager?.Info ?? null;
		public static StorageMgrPath Path => Admin?.Path ?? null;

		public static DataManager<SheetDataSet2> Dmx;

		public static IEnumerable<KeyValuePair<string, SheetData2.SheetData2>> GetSheets()
		{
			foreach (KeyValuePair<string, SheetData2.SheetData2> kvp in Data.SheetDataList)
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

		public static bool RectIsType(SheetRectType type, SheetRectType test)
		{
			return (type & test) != 0;
		}

		public static void updateHeader()
		{
			DM.DbxLineEx(0, "\tdata manager - update header");
			Info.FileType = SettingFileType.SETTING_MGR_DATA;
			Info.DataClassVersion = Data.DataFileVersion;
			Info.Description = Data.DataFileDescription;
		}

		public static void Open(FilePath<FileNameSimple> filePath)
		{
			if (Manager != null) return;

			// Debug.WriteLine($"@2 {SheetsCount}");

			DM.DbxLineEx(0, "\tdata manager - open");

			Manager = new DataManager<SheetDataSet2>(filePath);

			Initialized = true;

			// Debug.WriteLine($"@4 {SheetsCount}");

			Read();

			// updateHeader();
		}

		public static void Read()
		{
			DM.DbxLineEx(0, "\tdata manager - read");
			Admin.Read();
		}

		public static void Write()
		{
			DM.DbxLineEx(0, "\tdata manager - write");
			Admin.Write();
		}

		public static void Reset()
		{
			DM.DbxLineEx(0, "\tdata manager - reset");

			Manager.Reset();

			updateHeader();

			Write();

			Initialized = false;
		}

		public static void Close()
		{
			DM.DbxLineEx(0, "\tdata manager - close");

			updateHeader();

			Write();

			Manager.Reset();
			Manager.ResetPath();

			Initialized = false;
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

