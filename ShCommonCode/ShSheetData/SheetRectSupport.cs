#region + Using Directives

#endregion

// user name: jeffs
// created:   5/11/2024 1:22:54 PM

using System.Diagnostics;
using iText.Kernel.Geom;
// using ReadPDFTextTests.SheetData;
using static ShCommonCode.ShSheetData.SheetRectType;
using static ShCommonCode.ShSheetData.SheetRectId;
using SharedCode.ShPdfSupport;
using UtilityLibrary;

namespace ShCommonCode.ShSheetData
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

		SM_OPT1                ,
		SM_OPT2                ,
		SM_OPT3                ,
		SM_OPT4                ,
		SM_OPT5                ,
		SM_OPT6                ,
		SM_OPT7                ,
		SM_OPT8                ,
		SM_OPT9                ,
	}

	public class SheetRectInfo<T>
	{
		public SheetRectType Type { get; private set; }
		public T Id { get; private set; }

		public SheetRectInfo(SheetRectType type, T id)
		{
			Type = type;
			Id = id;
		}
	}

	public class SheetRectSupport
	{
		private const int TITLE_WIDTH = 24;
		private const int TYPE_WIDTH = -20;

		// public static void Init(FilePath<FileNameSimple> filePath)
		// {
		// 	FilePath = filePath;
		// 	// SmDataManager = new DataManager<SheetMetricDataSet>(FilePath);
		//
		// 	SheetDataManager.Init(filePath);
		// }

		// public static FilePath<FileNameSimple> FilePath { get; set; }

		// common

		public static SheetRectType GetRecType(string name, out SheetRectId id)
		{
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
		public static int ShtRectsQty => ShtRectIdXref.Count - 1; // don't count NA

		public static SheetRectId GetShtRectId(string name)
		{
			if (!ShtRectIdXref.ContainsKey(name)) return SM_NA;

			return ShtRectIdXref[name].Id;
		}

		public static string? GetShtRectName(SheetRectId id)
		{
			foreach (KeyValuePair<string, SheetRectInfo<SheetRectId>> kvp in ShtRectIdXref)
			{
				if (id == kvp.Value.Id) return kvp.Key;
			}

			return null;
		}

		public static SheetRectType GetShtRectType(string name)
		{
			if (!ShtRectIdXref.ContainsKey(name)) return SRT_NA;

			return ShtRectIdXref[name].Type;
		}

		public static Dictionary<string, SheetRectInfo<SheetRectId>> ShtRectIdXref { get; } = new ()
		{
			{ "Not Available"    , new (SRT_NA, SM_NA) }                    ,  // default / not configured
			{ "Sheet Boundary"   , new (SRT_NA, SM_SHT) }                    , // size of sheet
			{ "SHEET XREF"       , new (SRT_LOCATION, SM_XREF) }             , // the box style for sheet xrefs
			{ "SHEET NUMBER FIND", new (SRT_LOCATION, SM_SHT_NUM_FIND) }     , // limits to find the sheet number
			{ "SHEET TITLE"      , new (SRT_LOCATION, SM_SHT_TITLE) }        , // where to find the title of the sheet

			{ "SHEET NUMBER"     , new (SRT_BOX, SM_SHT_NUM) }               , // where to place the sheet number box
			{ "AUTHOR"           , new (SRT_LINK_N_BOX, SM_AUTHOR) }         , // the information for the author box
			{ "DISCLAIMER"       , new (SRT_TEXT_LINK_N_BOX, SM_DISCLAIMER) }, // the information for the disclaimer
			{ "FOOTER"           , new (SRT_TEXT_N_BOX, SM_FOOTER) }         , // the information for the footer
			{ "FIRST BANNER"     , new (SRT_TEXT_N_BOX, SM_BANNER_1ST) }     , // the information for the banner
			{ "SECOND BANNER"    , new (SRT_TEXT_N_BOX, SM_BANNER_2ND) }     , // ditto
			{ "THIRD BANNER"     , new (SRT_TEXT_N_BOX, SM_BANNER_3RD) }     , // ditto
			{ "FOURTH BANNER"    , new (SRT_TEXT_N_BOX, SM_BANNER_4TH) }     , // ditto
			{ "WATERMARK1"       , new (SRT_TEXT, SM_WATERMARK1) }           , // the information for the main watermark
			{ "WATERMARK2"       , new (SRT_TEXT, SM_WATERMARK2) }           , // the information for the title block watermark
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

		public static string? GetOptRectName(SheetRectId id)
		{
			foreach (KeyValuePair<string, SheetRectInfo<SheetRectId>> kvp in OptRectIdXref)
			{
				if (id == kvp.Value.Id) return kvp.Key;
			}

			return null;
		}

		public static SheetRectType GetOptRectType(string name)
		{
			if (!OptRectIdXref.ContainsKey(name)) return SRT_NA;

			return OptRectIdXref[name].Type;
		}

		public static Dictionary<string, SheetRectInfo<SheetRectId>> OptRectIdXref { get; } = new ()
		{
			{ "Not Available" , new (SRT_NA, SM_NA) }               , // default / not configured
			{ "OPTIONAL 1"    , new (SRT_TEXT_LINK_N_BOX, SM_OPT1) }, // the information for an optional box
			{ "OPTIONAL 2"    , new (SRT_TEXT_LINK_N_BOX, SM_OPT2) }, // ditto
			{ "OPTIONAL 3"    , new (SRT_TEXT_LINK_N_BOX, SM_OPT3) }, // ditto
			{ "OPTIONAL 4"    , new (SRT_TEXT_LINK_N_BOX, SM_OPT4) }, // ditto
			{ "OPTIONAL 5"    , new (SRT_TEXT_LINK_N_BOX, SM_OPT5) }, // ditto
			{ "OPTIONAL 6"    , new (SRT_TEXT_LINK_N_BOX, SM_OPT6) }, // ditto
			{ "OPTIONAL 7"    , new (SRT_TEXT_LINK_N_BOX, SM_OPT7) }, // ditto
			{ "OPTIONAL 8"    , new (SRT_TEXT_LINK_N_BOX, SM_OPT8) }, // ditto
			{ "OPTIONAL 9"    , new (SRT_TEXT_LINK_N_BOX, SM_OPT9) }, // ditto
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


		public static void showSheetRects()
		{

		}
		
		public static void showShtRects()
		{
			if (SheetDataManager.Data.SheetRectangles == null || SheetDataManager.Data.SheetRectangles.Count == 0)
			{
				Debug.WriteLine("\nno metric data");
				return;
			}


			foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.Data.SheetRectangles)
			{
				int missing;

				Debug.WriteLine($"\n\nfor {kvp.Key}");

				Debug.Write($"{"sheet rectangles",TITLE_WIDTH}| found {kvp.Value.ShtRects.Count}");
				missing = SheetRectSupport.ShtRectsQty - kvp.Value.ShtRects.Count;
				if (missing > 0)
				{
					Debug.WriteLine($" | missing {missing}");
				}
				else
				{
					Debug.Write("\n");
				}

				Debug.WriteLine($"{"optional rectangles",TITLE_WIDTH}| found {kvp.Value.OptRects.Count}");
				
				foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp2 in kvp.Value.ShtRects)
				{
					

					string name = SheetRectSupport.GetShtRectName(kvp2.Key)!;
					string type = kvp2.Value.Type.ToString();

					Debug.WriteLine($"{name,TITLE_WIDTH}| {type,TYPE_WIDTH}| {FormatItextData.FormatRectangle(kvp2.Value.Rect)}");

					Debug.WriteLine($"sneeker| type       | {kvp2.Value.GetValue("type", kvp2.Value)}");
					Debug.WriteLine($"sneeker| rotation   | {kvp2.Value.GetValue("rotation", kvp2.Value)}");
					Debug.WriteLine($"sneeker| FillOpacity| {kvp2.Value.GetValue("FillOpacity", kvp2.Value)}");
				}

				Debug.Write("\n");
			}
		}

		public static void ShowValues()
		{
			foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.Data.SheetRectangles)
			{
				Debug.WriteLine($"sheet name| {kvp.Key}");

				foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp2 in kvp.Value.ShtRects)
				{
					Debug.WriteLine($"\n{kvp2.Key}");

					showBoxValues(kvp2.Value);
				}

				foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp3 in kvp.Value.OptRects)
				{
					Debug.WriteLine($"\n {kvp3.Key}");

					showBoxValues(kvp3.Value);
				}
			}
		}

		private static void showBoxValues(SheetRectData<SheetRectId> box)
		{
			Debug.WriteLine($"\tbox data                | ");
			Debug.WriteLine($"\t\tbox id              | {box.Id}");
			Debug.WriteLine($"\t\tbox type            | {box.Type}");
			Debug.WriteLine($"\t\trectangle           | {FormatItextData.FormatRectangle(box.Rect)}");
			Debug.WriteLine($"\t\tRotation            | {box.Rotation }");

			if (box.Id == SheetRectId.SM_XREF)
			{
				showBoundingBoxValues(box);
				return;
			}

			if (box.Type == SheetRectType.SRT_NA ||
				box.Type == SheetRectType.SRT_LOCATION
				) return;


			Debug.  Write($"\tbounding box info       | ");
			if (box.HasType(SheetRectType.SRT_BOX))
			{
				Debug.Write("\n");
				showBoundingBoxValues(box);
			}
			else
			{
				Debug.WriteLine("n/a");
			}

			Debug.Write($"\tlink info               | ");
			if (box.HasType(SheetRectType.SRT_LINK))
			{
				Debug.Write("\n");
				Debug.WriteLine($"\t\tUrlLink             | {box.UrlLink }");
			}
			else
			{
				Debug.WriteLine("n/a");
			}


			Debug.Write($"\ttext info               | ");
			if (box.HasType(SheetRectType.SRT_TEXT))
			{
				Debug.Write("\n");
				showTextValues(box);
			}
			else
			{
				Debug.WriteLine("n/a");
			}
		}

		private static void showBoundingBoxValues(SheetRectData<SheetRectId> box)
		{
			Debug.WriteLine($"\t\tFillColor           | {FormatItextData.FormatColor(box.FillColor)}");
			Debug.WriteLine($"\t\tFillOpacity         | {box.FillOpacity}");
			Debug.WriteLine($"\t\tBdrWidth            | {box.BdrWidth }");
			Debug.WriteLine($"\t\tBdrColor            | {FormatItextData.FormatColor(box.BdrColor)}");
			Debug.WriteLine($"\t\tBdrOpacity          | {box.BdrOpacity }");
			Debug.WriteLine($"\t\tBdrDashPattern      | {FormatItextData.FormatDashArray(box.BdrDashPattern)}");
		}

		private static void showTextValues(SheetRectData<SheetRectId> box)
		{
			Debug.WriteLine($"\t\tInfoText            | {box.InfoText }");
			Debug.WriteLine($"\t\tFontFamily          | {box.FontFamily }");
			Debug.WriteLine($"\t\tFontStyle           | {FormatItextData.FormatFontStyle(box.FontStyle)}");
			Debug.WriteLine($"\t\tTextSize            | {box.TextSize }");
			Debug.WriteLine($"\t\tTextHorizAlignment  | {box.TextHorizAlignment }");
			Debug.WriteLine($"\t\tTextVertAlignment   | {box.TextVertAlignment }");
			Debug.WriteLine($"\t\tTextWeight          | {box.TextWeight}");
			Debug.WriteLine($"\t\tTextDecoration      | {TextDecorations.FormatTextDeco(box.TextDecoration)}");
			Debug.WriteLine($"\t\tTextColor           | {FormatItextData.FormatColor(box.TextColor)}");
			Debug.WriteLine($"\t\tTextOpacity         | {box.TextOpacity }");
		}

		public override string ToString()
		{
			return $"this is {nameof(SheetRectSupport)}";
		}
	}


}