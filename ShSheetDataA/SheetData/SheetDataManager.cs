using Settings;
using SettingsManager;
using UtilityLibrary;

namespace ShCommonCode.ShSheetData
{
	public static class SheetDataManager
	{
		public static bool Initialized { get; private set; } = false;

		public static DataManager<SheetDataSet>? Manager { get; private set; }

		public static void Init(FilePath<FileNameSimple> filePath)
		{
			if (Manager != null) return;

			Manager = new DataManager<SheetDataSet>(filePath);

			Initialized = true;
		}

		public static SheetDataSet? Data => Manager?.Data ?? null;
		public static SettingsMgr<StorageMgrPath, StorageMgrInfo<SheetDataSet>, SheetDataSet>? Admin => Manager?.Admin ?? null;
		public static StorageMgrInfo<SheetDataSet>? Info => Manager?.Info ?? null;
		public static StorageMgrPath? Path => Admin?.Path ?? null;

		public static bool SettingsFileExists => Path.SettingFileExists;

		public static int SheetsCount => Data?.SheetRectangles?.Count ?? -1;
		// public static int SheetMetricsACount => Data?.SheetMetricsA?.Count ?? -1;

		public static void Read()
		{
			Admin.Read();

			// Data.SheetMetrics = SheetMetricsSupport.Convert(Data.SheetMetricsA);

		}

		public static void Write()
		{
			// Data.SheetMetricsA = SheetMetricsSupport.Convert(Data.SheetMetrics);

			Admin.Write();
		}
	}
}

