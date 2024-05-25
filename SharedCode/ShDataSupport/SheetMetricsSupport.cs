#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using SettingsManager;
using SharedCode.ShDataSupport;
using UtilityLibrary;

using static SharedCode.ShDataSupport.SheetMetricId;

#endregion

// user name: jeffs
// created:   5/11/2024 1:22:54 PM

namespace SharedCode.ShDataSupport
{
	public static class ShtData
	{
		public static DataManager<SheetMetricDataSet> Manager { get; private set; }

		public static void Init(FilePath<FileNameSimple> filePath)
		{
			if (Manager != null) return;

			Manager = new DataManager<SheetMetricDataSet>(filePath);

		}

		public static SheetMetricDataSet Data => Manager.Data;
		public static SettingsMgr<StorageMgrPath, StorageMgrInfo<SheetMetricDataSet>, SheetMetricDataSet> Admin => Manager.Admin;
		public static StorageMgrInfo<SheetMetricDataSet> Info => Manager.Info;
		public static StorageMgrPath Path => Admin.Path;

		public static bool SettingsFileExists => Path.SettingFileExists;

		public static void Read() => Admin.Read();
		public static void Write() => Admin.Write();
	}

	public class SheetMetricsSupport
	{

		// sheet rects
		public static int ShtRectsQty => ShtRectIdXref.Count;
		public static SheetMetricId GetShtRectId(string name)
		{
			if (!ShtRectIdXref.ContainsKey(name)) return SM_NA;

			return ShtRectIdXref[name];
		}
		public static string GetShtRectName(SheetMetricId id)
		{
			foreach (KeyValuePair<string, SheetMetricId> kvp in ShtRectIdXref)
			{
				if (id == kvp.Value) return kvp.Key;
			}

			return null;

		}
		private static Dictionary<string, SheetMetricId> ShtRectIdXref { get; }= new Dictionary<string, SheetMetricId> ()
		{
			{"Not Available"    , SM_NA},
			{"SHEET BOUNDARY"   , SM_SHT},
			{"SHEET NUMBER"     , SM_SHT_NUM},
			{"SHEET TITLE"      , SM_SHT_TITLE},
			{"SHEET TITLE BLOCK", SM_SHT_TITLE_BLOCK},
			{"AUTHOR"           , SM_AUTHOR},
			{"DISCLAIMER"       , SM_DISCLAIMER},
			{"FOOTER"           , SM_FOOTER},
			{"FIRST BANNER"     , SM_BANNER_1ST},
			{"SECOND BANNER"    , SM_BANNER_2ND},
			{"THIRD BANNER"     , SM_BANNER_3RD},
			{"FOURTH BANNER"    , SM_BANNER_4TH},
			{"WATERMARK"        , SM_WATERMARK},
		};

		// public static bool SetShtRectFound(string name)
		// {
		// 	if (!ShtRectIdXref.ContainsKey(name)) return false;
		//
		// 	ShtRectIdXref[name].Found = true;
		//
		// 	return true;
		// }
		//
		// private static bool ValidateShtRectsFound()
		// {
		// 	foreach (KeyValuePair<string, RectXrefInfo> kvp in ShtRectIdXref)
		// 	{
		// 		if (!kvp.Value.Found) return false;
		// 	}
		//
		// 	return true;
		// }
		//
		// public static bool ShtRectsAllFound => ValidateShtRectsFound();



		// optional rectangles

		public static int OptRectsQty => OptRectIdXref.Count;
		public static int GetOptRectIdx(string name)
		{
			if (!OptRectIdXref.ContainsKey(name)) return -1;

			return OptRectIdXref[name];
		}
		public static string GetOptRectName(int idx)
		{
			if ((idx < 0) || (idx >= OptRectIdXref.Count)) return null;

			foreach (KeyValuePair<string, int> kvp in OptRectIdXref)
			{
				if (idx == kvp.Value) return kvp.Key;
			}

			return null;

		}
		private static Dictionary<string, int> OptRectIdXref { get; } = new Dictionary<string, int>()
		{
			{"OPTIONAL 1", 0},
			{"OPTIONAL 2", 1},
			{"OPTIONAL 3", 2},
			{"OPTIONAL 4", 3},
			{"OPTIONAL 5", 4},
			{"OPTIONAL 6", 5},
			{"OPTIONAL 7", 6},
			{"OPTIONAL 8", 7},
			{"OPTIONAL 9", 9},
		};

		// public static bool SetOptRectFound(string name)
		// {
		// 	if (!OptRectIdXref.ContainsKey(name)) return false;
		//
		// 	OptRectIdXref[name].Found = true;
		//
		// 	return true;
		// }
		//
		// private static bool ValidateOptRectsAnyFound()
		// {
		// 	foreach (KeyValuePair<string, RectXrefInfo> kvp in ShtRectIdXref)
		// 	{
		// 		if (kvp.Value.Found) return true;
		// 	}
		//
		// 	return false;
		// }
		//
		// public static bool OptRectsAnyFound => ValidateOptRectsAnyFound();

		public static void Init(FilePath<FileNameSimple> filePath)
		{
			FilePath = filePath;
			// SmDataManager = new DataManager<SheetMetricDataSet>(FilePath);

			ShtData.Init(filePath);
		}

		public static FilePath<FileNameSimple> FilePath { get; private set; }

		// public static DataManager<SheetMetricDataSet> SmDataManager { get; private set; }

		public static void WriteSheetMetrics()
		{
			ShtData.Data.SheetMetricsA = Convert(ShtData.Data.SheetMetrics);
			ShtData.Write();
		}

		public static void ReadSheetMetrics()
		{
			ShtData.Read();
			ShtData.Data.SheetMetrics = Convert(ShtData.Data.SheetMetricsA);
		}

		public static Rectangle Convert(AltRectangle ar)
		{
			return new Rectangle(ar.X, ar.Y, ar.Width, ar.Height);
		}

		public static AltRectangle Convert(Rectangle ar)
		{
			return new AltRectangle(ar.GetX(), ar.GetY(), ar.GetWidth(), ar.GetHeight());
		}

		public static SheetMetric Convert(SheetMetricA sma)
		{
			SheetMetric sm = new SheetMetric();

			sm.Name = sma.Name;
			sm.Description = sma.Description;
			sm.Created = sma.Created;

			for (int i = 0; i < sma.ShtRectsA.Count; i++)
			{
				sm.ShtRects.Add((SheetMetricId) i, Convert(sma.ShtRectsA[i]));
			}

			for (var i = 0; i < sma.OptRectsA.Count; i++)
			{
				sm.OptRects.Add(i, Convert(sma.OptRectsA[i]));
			}

			return sm;
		}

		public static SheetMetricA Convert(SheetMetric sm)
		{
			SheetMetricA sma = new SheetMetricA();

			sma.Name = sm.Name;

			for (int i = 0; i < sm.ShtRects.Count; i++)
			{
				sma.ShtRectsA.Add(Convert(sm.ShtRects[(SheetMetricId) i]));
			}

			for (var i = 0; i < sm.OptRects.Count; i++)
			{
				sma.OptRectsA.Add(Convert(sm.OptRects[i]));
			}

			return sma;
		}

		public static Dictionary<string, SheetMetricA> Convert(Dictionary<string, SheetMetric> sm)
		{
			Dictionary<string, SheetMetricA> sma = new Dictionary<string, SheetMetricA>();

			foreach (KeyValuePair<string, SheetMetric> kvp in sm)
			{
				sma.Add(kvp.Key, Convert(kvp.Value));
			}

			return sma;
		}

		public static Dictionary<string, SheetMetric> Convert(Dictionary<string, SheetMetricA> sma)
		{
			Dictionary<string, SheetMetric> sm = new Dictionary<string, SheetMetric>();

			foreach (KeyValuePair<string, SheetMetricA> kvp in sma)
			{
				sm.Add(kvp.Key, Convert(kvp.Value));
			}

			return sm;
		}





		public override string ToString()
		{
			return $"this is {nameof(SheetMetricsSupport)}";
		}
	}
}