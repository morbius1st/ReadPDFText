#region + Using Directives

using System;
using System.Collections.Generic;
using System.Text;
using UtilityLibrary;
using static ShSheetData.Support.SheetRectType;
using static ShSheetData.Support.SheetRectId;

#endregion

// user name: jeffs
// created:   5/11/2024 1:22:54 PM

namespace ShSheetData.Support
{
	[Flags]
	public enum SheetRectType
	{
		SRT_NA              = 0,    // 0 - not available
		SRT_LOCATION        = 2<<0, // 1 - location (boundary - e.g. scanning zone)
		SRT_TEXT            = 2<<1, // 2 - has extra text - record text information
		SRT_LINK            = 2<<2, // 4 - has link text - does not use text information
		SRT_BOX             = 2<<3, // 8 - record box information
		SRT_TEXT_N_BOX      = SRT_TEXT | SRT_BOX,
		SRT_LINK_N_BOX      = SRT_LINK | SRT_BOX,  
		SRT_TEXT_LINK_N_BOX = SRT_TEXT |SRT_LINK| SRT_BOX,
	}

	public enum SheetRectId
	{
		SM_NA              = -1,
		SM_SHT             = 0 ,
		SM_XREF                ,

		SM_SHT_NUM             ,
		SM_SHT_NUM_FIND        ,
		SM_SHT_TITLE           ,

		SM_AUTHOR              ,
		SM_DISCLAIMER          ,
		SM_FOOTER              ,

		SM_BANNER_1ST          ,
		SM_BANNER_2ND          ,
		SM_BANNER_3RD          ,
		SM_BANNER_4TH          ,

		SM_WATERMARK1          ,
		SM_WATERMARK2          ,

		SM_OPT0                ,
		SM_OPT1                ,
		SM_OPT2                ,
		SM_OPT3                ,
		SM_OPT4                ,
		SM_OPT5                ,
		SM_OPT6                ,
		SM_OPT7                ,
		SM_OPT8                ,
		SM_OPT9                ,
		SM_OPT10               ,

		SM_PAGE_TITLE			// to add a lable on a created page
	}

	public class TextDecorations
	{
		public static int NORMAL { get; } = 0;
		public static int UNDERLINE { get; } = 1 << 1;
		public static int LINETHROUGH { get; } = 1 << 2;

		public static bool HasUnderline(int decoration)
		{
			return (decoration & UNDERLINE) > 0;
		}

		public static bool HasLinethrough(int decoration)
		{
			return (decoration & LINETHROUGH) > 0;
		}

		public static string FormatTextDeco(int deco)
		{
			if (deco == NORMAL) { return nameof(NORMAL); }

			StringBuilder result = new StringBuilder();

			if (HasUnderline(deco)) result.Append(nameof(UNDERLINE));
			if (HasLinethrough(deco)) result.Append(nameof(LINETHROUGH));

			return result.ToString();
		}
	}

	public class SheetRectConfigData<T>
	{
		public SheetRectType Type { get; private set; }
		public T Id { get; private set; }

		public SheetRectConfigData(SheetRectType type, T id)
		{
			Type = type;
			Id = id;
		}
	}

	public class SheetRectConfigDataSupport
	{
		// common

		public static SheetRectType GetRecType(string name, out SheetRectId id)
		{
			DM.InOut0();

			id = SM_NA;

			if (OptRectIdXref.ContainsKey(name))
			{
				id = OptRectIdXref[name].Id;
				return OptRectIdXref[name].Type;
			}

			if (!ShtRectIdXref.ContainsKey(name)) return SRT_NA;

			id = ShtRectIdXref[name].Id;
			return ShtRectIdXref[name].Type;
		}

		// sheet rects
		public static int ShtRectsMinQty => ShtRectIdXref.Count - 1 - 3 - 1; // don't count NA, incl only 1 banner and only 1 watermark

		public static SheetRectId GetShtRectId(string name)
		{
			if (!ShtRectIdXref.ContainsKey(name)) return SheetRectId.SM_NA;

			return ShtRectIdXref[name].Id;
		}

		public static string GetShtRectName(SheetRectId id)
		{
			foreach (KeyValuePair<string, SheetRectConfigData<SheetRectId>> kvp in ShtRectIdXref)
			{
				if (id == kvp.Value.Id) return kvp.Key;
			}

			return null;
		}

		public static SheetRectType GetShtRectType(string name)
		{
			if (!ShtRectIdXref.ContainsKey(name)) return SheetRectType.SRT_NA;

			return ShtRectIdXref[name].Type;
		}

		public static Dictionary<string, SheetRectConfigData<SheetRectId>> ShtRectIdXref { get; } = new Dictionary<string, SheetRectConfigData<SheetRectId>>()
		{
			{ "Not Available"    , new SheetRectConfigData<SheetRectId>(SRT_NA,SM_NA) }                     , // 0  default / not configured
			{ "Sheet Boundary"   , new SheetRectConfigData<SheetRectId>(SRT_NA,SM_SHT) }                    , // 1  size of sheet
			{ "SHEET XREF"       , new SheetRectConfigData<SheetRectId>(SRT_BOX,SM_XREF) }                  , // 2  the box style for sheet xrefs
			{ "SHEET NUMBER FIND", new SheetRectConfigData<SheetRectId>(SRT_LOCATION,SM_SHT_NUM_FIND) }     , // 3  limits to find the sheet number
			{ "SHEET TITLE"      , new SheetRectConfigData<SheetRectId>(SRT_LOCATION,SM_SHT_TITLE) }        , // 4  where to find the title of the sheet

			{ "SHEET NUMBER"     , new SheetRectConfigData<SheetRectId>(SRT_LINK_N_BOX,SM_SHT_NUM) }        , // 5  where to place the sheet number box
			{ "AUTHOR"           , new SheetRectConfigData<SheetRectId>(SRT_LINK_N_BOX,SM_AUTHOR) }         , // 6  the information for the author box
			{ "DISCLAIMER"       , new SheetRectConfigData<SheetRectId>(SRT_TEXT_LINK_N_BOX,SM_DISCLAIMER) }, // 7  the information for the disclaimer
			{ "FOOTER"           , new SheetRectConfigData<SheetRectId>(SRT_TEXT_N_BOX,SM_FOOTER) }         , // 8  the information for the footer
			{ "FIRST BANNER"     , new SheetRectConfigData<SheetRectId>(SRT_TEXT_N_BOX,SM_BANNER_1ST) }     , // 9  the information for the banner
			{ "SECOND BANNER"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_N_BOX,SM_BANNER_2ND) }     , // 10 ditto
			{ "THIRD BANNER"     , new SheetRectConfigData<SheetRectId>(SRT_TEXT_N_BOX,SM_BANNER_3RD) }     , // 11 ditto
			{ "FOURTH BANNER"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_N_BOX,SM_BANNER_4TH) }     , // 12 ditto
			{ "WATERMARK1"       , new SheetRectConfigData<SheetRectId>(SRT_TEXT,SM_WATERMARK1) }           , // 13 the information for the main watermark
			{ "WATERMARK2"       , new SheetRectConfigData<SheetRectId>(SRT_TEXT,SM_WATERMARK2) }           , // 14 the information for the title block watermark
		};

		/*
		public static int ShtRectsQty2 => ShtRectIdXref2.Count - 1; // don't count NA
		public static SheetMetricId GetShtRectId2(string name)
		{
			if (!ShtRectIdXref2.ContainsKey(name)) return SM_NA;

			return ShtRectIdXref2[name];
		}
		public static string? GetShtRectName2(SheetMetricId id)
		{
			foreach (KeyValuePair<string, SheetMetricId> kvp in ShtRectIdXref2)
			{
				if (id == kvp.Value) return kvp.Key;
			}

			return null;

		}

		public static Dictionary<string, SheetMetricId> ShtRectIdXref2 { get; }= new ()
		{
			{"Not Available"    , SM_NA},
			{"Sheet Boundary"   , SM_SHT},
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
		*/

		// optional rectangles
		public static int OptRectsQty => OptRectIdXref.Count - 1; // don't count NA

		public static SheetRectId GetOptRectId(string name)
		{
			if (!OptRectIdXref.ContainsKey(name)) return SM_NA;

			return OptRectIdXref[name].Id;
		}

		public static string GetOptRectName(SheetRectId id)
		{
			foreach (KeyValuePair<string, SheetRectConfigData<SheetRectId>> kvp in OptRectIdXref)
			{
				if (id == kvp.Value.Id) return kvp.Key;
			}

			return null;
		}

		public static SheetRectType GetOptRectType(string name)
		{
			if (!OptRectIdXref.ContainsKey(name)) return SheetRectType.SRT_NA;

			return OptRectIdXref[name].Type;
		}

		public static Dictionary<string, SheetRectConfigData<SheetRectId>> OptRectIdXref { get; } = new Dictionary<string, SheetRectConfigData<SheetRectId>>()
		{
			{ "Not Available" , new SheetRectConfigData<SheetRectId>(SRT_NA,SM_NA) }               , // default / not configured
			{ "OPTIONAL 0"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_LINK_N_BOX,SM_OPT0) }, // the information for an optional box
			{ "OPTIONAL 1"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_LINK_N_BOX,SM_OPT1) }, // ditto
			{ "OPTIONAL 2"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_LINK_N_BOX,SM_OPT2) }, // ditto
			{ "OPTIONAL 3"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_LINK_N_BOX,SM_OPT3) }, // ditto
			{ "OPTIONAL 4"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_LINK_N_BOX,SM_OPT4) }, // ditto
			{ "OPTIONAL 5"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_LINK_N_BOX,SM_OPT5) }, // ditto
			{ "OPTIONAL 6"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_LINK_N_BOX,SM_OPT6) }, // ditto
			{ "OPTIONAL 7"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_LINK_N_BOX,SM_OPT7) }, // ditto
			{ "OPTIONAL 8"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_LINK_N_BOX,SM_OPT8) }, // ditto
			{ "OPTIONAL 9"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_LINK_N_BOX,SM_OPT9) }, // ditto
			{ "OPTIONAL10"    , new SheetRectConfigData<SheetRectId>(SRT_TEXT_LINK_N_BOX,SM_OPT10) }, // ditto
		};

		/*
		public static int OptRectsQty => OptRectIdXref.Count; 
		public static int GetOptRectIdx(string name)
		{
			if (!OptRectIdXref.ContainsKey(name)) return -1;

			return OptRectIdXref[name];
		}
		public static string? GetOptRectName(int idx)
		{
			if ((idx < 0) || (idx >= OptRectIdXref.Count)) return null;

			foreach (KeyValuePair<string, int> kvp in OptRectIdXref)
			{
				if (idx == kvp.Value) return kvp.Key;
			}

			return null;

		}

		public static Dictionary<string, int> OptRectIdXref2 { get; } = new ()
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
		*/

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



		// public static DataManager<SheetMetricDataSet> SmDataManager { get; private set; }

		// public static void WriteSheetMetrics()
		// {
		// 	ShtData.Data.SheetMetricsA = Convert(ShtData.Data.SheetMetrics);
		// 	ShtData.Write();
		// }
		//
		// public static void ReadSheetMetrics()
		// {
		// 	ShtData.Read();
		// 	ShtData.Data.SheetMetrics = Convert(ShtData.Data.SheetMetricsA);
		// }

/*
		public static Rectangle Convert(AltRectangle ar)
		{
			return new Rectangle(ar.X, ar.Y, ar.Width, ar.Height);
		}

		public static AltRectangle Convert(Rectangle ar)
		{
			return new AltRectangle(ar.GetX(), ar.GetY(), ar.GetWidth(), ar.GetHeight());
		}

		public static SheetRects Convert(SheetMetricA sma)
		{
			SheetRects sm = new SheetRects();

			SheetRectData<SheetMetricId> srdMid;
			SheetRectData<int> srdIid;

			sm.Name = sma.Name;
			sm.Description = sma.Description;
			sm.CreatedDt = sma.CreatedDt;

			foreach (KeyValuePair<SheetMetricId, SheetRectData<SheetMetricId>> kvp in sma.ShtRectsA)
			{
				sm.ShtRects.Add(kvp.Key,
					new SheetRectData<SheetMetricId>(kvp.Value.Type, kvp.Value.Id, kvp.Value.Rect));
			}

			foreach (KeyValuePair<SheetMetricId, SheetRectData<SheetMetricId>> kvp in sma.OptRectsA)
			{
				sm.OptRects.Add(kvp.Key,
					new SheetRectData<SheetMetricId>(kvp.Value.Type, kvp.Value.Id, kvp.Value.Rect));
			}

			return sm;
		}

		public static SheetMetricA Convert(SheetRects sm)
		{
			SheetMetricA sma = new SheetMetricA();

			SheetRectData<SheetMetricId> srdMid;
			SheetRectData<int> srdIid;

			sma.Name = sm.Name;
			sma.Description = sm.Description;
			sma.CreatedDt = sm.CreatedDt;

			foreach (KeyValuePair<SheetMetricId, SheetRectData<SheetMetricId>> kvp in sm.ShtRects)
			{
				sma.ShtRectsA.Add(kvp.Key,
					new SheetRectData<SheetMetricId>(kvp.Value.Type, kvp.Value.Id, kvp.Value.Rect));
			}

			foreach (KeyValuePair<SheetMetricId, SheetRectData<SheetMetricId>> kvp in sm.OptRects)
			{
				sma.OptRectsA.Add(kvp.Key,
					new SheetRectData<SheetMetricId>(kvp.Value.Type, kvp.Value.Id,  kvp.Value.Rect));
			}

			return sma;
		}

		public static Dictionary<string, SheetMetricA> Convert(Dictionary<string, SheetRects> sm)
		{
			Dictionary<string, SheetMetricA> sma = new Dictionary<string, SheetMetricA>();

			foreach (KeyValuePair<string, SheetRects> kvp in sm)
			{
				sma.Add(kvp.Key, Convert(kvp.Value));
			}

			return sma;
		}

		public static Dictionary<string, SheetRects> Convert(Dictionary<string, SheetMetricA> sma)
		{
			Dictionary<string, SheetRects> sm = new Dictionary<string, SheetRects>();

			foreach (KeyValuePair<string, SheetMetricA> kvp in sma)
			{
				sm.Add(kvp.Key, Convert(kvp.Value));
			}

			return sm;
		}
*/




		public override string ToString()
		{
			return $"this is {nameof(SheetRectConfigDataSupport)}";
		}
	}

}