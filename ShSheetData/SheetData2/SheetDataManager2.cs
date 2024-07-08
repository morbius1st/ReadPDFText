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

		public static bool Initialized => Manager != null;
		public static bool Configured => Initialized && GotDataPath;

		public static bool GotDataPath => Path?.SettingFolderPathIsValid ?? false;
		public static bool GotDataSheets => SheetsCount > 0;
		public static int SheetsCount => Data?.SheetDataList?.Count ?? -1;
		public static bool SettingsFileExists => Path?.SettingFileExists ?? false;

		// public static Dictionary<string, SheetData2.SheetData2> SheetDataList => Data?.SheetDataList;
		
		public static DataManager<SheetDataSet2> Manager { get; private set; }

		public static SheetDataSet2 Data => Manager?.Data ?? null;
		public static SettingsMgr<StorageMgrPath, StorageMgrInfo<SheetDataSet2>, SheetDataSet2> Admin => Manager?.Admin ?? null;
		public static StorageMgrInfo<SheetDataSet2> Info => Manager?.Info ?? null;
		public static StorageMgrPath Path => Admin?.Path ?? null;

		// enumerators

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

		// info

		public static bool RectIsType(SheetRectType type, SheetRectType test)
		{
			return (type & test) != 0;
		}

		// operations

		public static void ResetSheetDataList()
		{
			Data.SheetDataList = new Dictionary<string, SheetData2.SheetData2>();
		}

		public static void UpdateHeader()
		{
			DM.DbxLineEx(0, "\tdata manager - update header");
			Info.FileType = SettingFileType.SETTING_MGR_DATA;
			Info.DataClassVersion = Data.DataFileVersion;
			Info.Description = Data.DataFileDescription;
		}

		public static bool Init(FilePath<FileNameSimple> filePath)
		{
			if (Configured) return false;

			// if (Manager != null || filePath == null) return false;

			DM.DbxLineEx(0, "\t\tdata manager - init");

			Manager = new DataManager<SheetDataSet2>(filePath);

			return true;
		}

		// public static void Create(FilePath<FileNameSimple> filePath)
		public static void Create()
		{
			if (Manager == null) return;

			DM.DbxLineEx(0, "\tdata manager - create");
			
			// Manager = new DataManager<SheetDataSet2>(filePath);

			UpdateHeader();

			ResetSheetDataList();

			Write();
		}
		
		// public static void Open(FilePath<FileNameSimple> filePath)
		public static void Open()
		{
			if (!Configured) return;

			DM.DbxLineEx(0, "\t\tdata manager - open");

			Read();
		}

		public static void Read()
		{
			DM.DbxLineEx(0, "\t\tdata manager - read");
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

			Manager.ResetPath();

			Manager.Reset();
			
			// UpdateHeader();
			//
			// Write();

			// Initialized = false;
		}

		public static void Close()
		{
			DM.DbxLineEx(0, "\tdata manager - close");

			UpdateHeader();

			Write();

			Manager.Reset();
			Manager.ResetPath();

			// Initialized = false;
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

