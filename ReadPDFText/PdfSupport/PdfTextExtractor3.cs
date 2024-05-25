#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommonPdfCodeShCode;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using SharedCode.ShDataSupport;

using UtilityLibrary;
using static ReadPDFText.PdfSupport.PdfExtensions;

#endregion

// user name: jeffs
// created:   4/21/2024 5:40:52 PM

namespace ReadPDFText.PdfSupport
{
	public class LocationTextExtraction3 : LocationTextExtractionStrategy
	{
		public const bool SHOW_DEBUG_INFO = false;

		public const float SEP = 0.5f;
		public const float ADJ = 0.25f;
		public const float TOL = ADJ * 2;

		public const float MAX_SEP = SEP + ADJ;
		public const float MIN_SEP = SEP - ADJ;

		private float bsX = -1;
		private float bsY = -1;
		private float teX;
		private float teY;

		private ITextExtractFilter3 Filter;
		// private bool NormalOrientationIsVertical;

		private TextExtractor3 tx3;
		public List<TextAndLineSegmentData> list;
		private TextAndLineSegmentData sheetNumberTals;

		public LocationTextExtraction3(ITextExtractFilter3 filter
			  /*, bool normalOrientationIsVertical*/
			)
		{
			// Debug.WriteLine("starting extractor 3");

			Filter = filter;
			// NormalOrientationIsVertical = normalOrientationIsVertical;

			tx3 = new TextExtractor3(this);

			list = new List<TextAndLineSegmentData>();

			OnPage = 0;
			ShowTextStream = false;
			ShowSegmentText = false;
		}

		public TextAndLineSegmentData SheetNumberTals
		{
			get => sheetNumberTals;
			set
			{
				sheetNumberTals = value;

				if (value == null) return;

				bsX = SheetNumberTals.BSV.Get(0);
				bsY = SheetNumberTals.BSV.Get(1);

				teX = SheetNumberTals.TEV.Get(0);
				teY = SheetNumberTals.TEV.Get(1);

				// Debug.WriteLine($"sht num tals| {sheetNumberTals.Text} | bsx {bsX} tex {teX}");

			}
		}

		public int OnPage { get; set; }

		public bool ShowTextStream
		{
			set
			{
				tx3.ShowTextStream = value;
			}
		}

		public bool ShowSegmentText { get; set; }

		public void Finalize()
		{
			tx3.finalize();
		}

		public void Reset()
		{
			tx3 = new TextExtractor3(this);
			list = new List<TextAndLineSegmentData>();

			OnPage = 0;

		}

		private bool matchSheetNumber(TextAndLineSegmentData tals)
		{
			// todo restore
			return false;

			if (tals==null) return false;
			if (FloatOps.Equals(bsX, -1f) && FloatOps.Equals(bsY, -1)) return false;

			bool result;

			// Debug.Write($" sht num match| bsx {tals.BSV.Get(0)} tex {tals.TEV.Get(0)} ");

			bool b1 = FloatOps.Equals(tals.BSV.Get(0), bsX);
			bool b2 = FloatOps.Equals(tals.BSV.Get(1), bsY);
			bool b3 = FloatOps.Equals(tals.TEV.Get(0), teX);
			bool b4 = FloatOps.Equals(tals.TEV.Get(1), teY);

			// result = tals.BSV.Get(0) == bsX && tals.BSV.Get(1) == bsY
			// 	&& tals.TEV.Get(0) == teX && tals.TEV.Get(1) == teY;

			result = b1 && b2 && b3 && b4;

			return result;
		}

		public void OnGotTextSegment(LocationTextExtractor3Data data)
		{
			// string s = data.Text;
			// string sh = sheetNumberTals?.Text ?? null;

			// if (s.StartsWith("T1.0"))
			// {
			// 	int a = 1;
			// 	// Debug.WriteLine($"{s}");
			// }

			if (Filter != null && !Filter.Match(data.Text)) return;

			TextAndLineSegmentData tals = getTals(data);

			if (ShowSegmentText)
			{
				string bs = $"{tals.BSV.Get(0):F2}, {tals.BSV.Get(1):F2}";
				string be = $"{tals.BEV.Get(0):F2}, {tals.BEV.Get(1):F2}";
				
				Debug.WriteLine($"{bs} {$" >{tals.Text}<",-10} {be} ");
			}

			if (matchSheetNumber(tals))
			{
				// Debug.WriteLine("- failed match sheet number");
				return;
			}

			// Debug.WriteLine("tx3 match ***");

			list.Add(tals);
		}

		private TextAndLineSegmentData getTals(LocationTextExtractor3Data data)
		{
			Vector TS = data.StartRi.GetAscentLine().GetStartPoint();
			Vector TE = data.EndRi.GetAscentLine().GetEndPoint();

			Vector BS = data.StartRi.GetDescentLine().GetStartPoint();
			Vector BE = data.EndRi.GetDescentLine().GetEndPoint();

			LineSegment top = new LineSegment(TS, TE);
			LineSegment bottom = new LineSegment(BS, BE);

			TextAndLineSegmentData result = 
				new TextAndLineSegmentData(data.Text, bottom, top, true);

			return result;
		}

		public override void EventOccurred(IEventData data, EventType type)
		{
			if (type != EventType.RENDER_TEXT) return;

			tx3.SetRenderText((TextRenderInfo) data);
		}
	}

	public class TextExtractor3
	{
		private const float TOL = LocationTextExtraction3.TOL;

		private const bool SHOW_DEBUG = LocationTextExtraction3.SHOW_DEBUG_INFO;

		private TextRenderInfo riCurr;
		private TextRenderInfo riPrior;
		private TextRenderInfo riStart = null;

		public TextExtractor3(LocationTextExtraction3 ltx3)
		{
			this.ltx3 = ltx3;
		}

		public LocationTextExtraction3 ltx3 { get; set; }

		public bool ShowTextStream
		{
			set
			{
				showTextStream = value;
			}
		}

		private bool showTextStream;
		private string text;
		private string result;
		private string allStrings;

		private string temp1;
		private string temp2;

		private int idx;

		public void SetRenderText(TextRenderInfo data)
		{
			IList<TextRenderInfo> ris = data.GetCharacterRenderInfos();

			for (idx = 0; idx < ris.Count; idx++)
			{
				process(ris[idx]);
			}

			// finalize();
		}

		private void process(TextRenderInfo ri)
		{
			float x =
				ri.GetStartX();

			text = ri.GetText();

			// allStrings += text;

			if (showTextStream)
			{
				temp1 = $"({ri.GetStartX():F2}, {ri.GetStartY():F2})";
				temp2 = $"({ri.GetEndX():F2}, {ri.GetEndY():F2})";
				Debug.Write($" ri {temp1,20} >{text}< {temp2} ");
			}

			// if (SHOW_DEBUG) Debug.Write($" | ri ({ri.GetStartX():F2}) >{text}< ({ri.GetEndX():F2})");

			// if (riPrior != null)
			// {
			// 	if (SHOW_DEBUG) Debug.Write($" | prior ({riPrior.GetStartX():F2}) >{riPrior.GetText()}< ({riPrior.GetEndX():F2}) ");
			// }
			// else
			// {
			// 	if (SHOW_DEBUG) Debug.Write(" | prior is null");
			// }

			if (@" ,\/".IndexOf(text) != -1)
			{
				// space at the begining of a sequence
				if (riStart == null) return;

				// if (SHOW_DEBUG) Debug.WriteLine($"\n{" ",-12}got EOL");

				// found the end of a "word"
				finalize();
				riPrior = ri;
				riPrior = null;
				text = null;
				return;
			}

			if (riPrior != null &&
				(!FloatOps.EqualWithInTolerance(ri.GetStartX(), riPrior.GetEndX(), TOL) ||
				!FloatOps.EqualWithInTolerance(ri.GetStartY(), riPrior.GetEndY(), TOL)))
			{
				// if (SHOW_DEBUG)
				// 	Debug.WriteLine($"\n{" ",-12}got EOW"); // X| (ri {ri.GetStartX:F2}  vs prior {riPrior.GetEndX:F2})\n"); // dif {(riPrior.GetEndX - ri.GetStartX):F2})\n");

				finalize();
			}

			if (riStart == null)
			{
				riStart = ri;
				riStart.PreserveGraphicsState();
			}

			riPrior = ri;
			riPrior.PreserveGraphicsState();

			result += text;
		}

		public void finalize()
		{
			// Debug.WriteLine(allStrings);
			// allStrings = null;

			if (result != null && result.Length > 0)
			{
				LocationTextExtractor3Data data = new LocationTextExtractor3Data(result, riStart, riPrior ?? riStart);

				ltx3.OnGotTextSegment(data);
			}
			// else
			// {
			// 	// if (SHOW_DEBUG) Debug.Write($"\n{" ",-12}Start| ");
			// }

			riStart = null;
			riCurr = null;
			result = null;
		}
	}

	public class ListTextExtractionFilter3 : ITextExtractFilter3
	{
		private Dictionary<string, PdfTreeLeaf> ToMatch { get; set; }

		public ListTextExtractionFilter3(Dictionary<string, PdfTreeLeaf> toMatch, int minLength, int maxLength)
		{
			ToMatch = toMatch;
			MinLength = minLength;
			MaxLength = maxLength;
		}


		public bool Match(string toMatch)
		{
			if (toMatch.Length > MaxLength || toMatch.Length < MinLength) return false;

			if (ToMatch.ContainsKey(toMatch)) return true;

			return false;
		}

		public int MinLength { get; set; } = 2;
		public int MaxLength { get; set; } = 12;
	}

	public class RegexTextExtractionFilter3 : ITextExtractFilter3
	{
		public int MinLength { get; set; } = 2;
		public int MaxLength { get; set; } = 10;

		public List<Regex> FilterRegexes { get; set; }
		public string[] FilterStrings { get; set; }

		public RegexTextExtractionFilter3( List<Regex> filterRegexes)
		{
			FilterRegexes = filterRegexes;
		}

		public RegexTextExtractionFilter3(string[] filterStrings)
		{
			FilterStrings = filterStrings;

			FilterRegexes = new List<Regex>();

			foreach (string filterString in filterStrings)
			{
				FilterRegexes.Add(new Regex(filterString));
			}
		}


		public bool Match(string toMatch)
		{
			bool result;

			if (toMatch.Length > MaxLength || toMatch.Length < MinLength) return false;

			foreach (Regex regex in FilterRegexes)
			{
				MatchCollection m = regex.Matches(toMatch);

				if (m.Count == 1 ) return true;
			}

			return false;
		}
	}

	public struct LocationTextExtractor3Data
	{
		private string process;
		public TextRenderInfo StartRi { get; set; }
		public TextRenderInfo EndRi { get; set; }
		public string Text { get; set; }
		public int Index { get; set; }
		public int Length => Text.Length;
		public bool IsValid { get; private set; }

		public string Process
		{
			get => process;
			set { process += " | " + value; }
		}

		public LocationTextExtractor3Data(string text, TextRenderInfo startRi, TextRenderInfo endRi)
		{
			Text = text;
			StartRi = startRi;
			EndRi = endRi;
			Index = -1;
			IsValid = true;
			process = "i";
		}

		public static LocationTextExtractor3Data Invalid
		{
			get
			{
				LocationTextExtractor3Data results = new LocationTextExtractor3Data();
				results.IsValid = false;

				return results;
			}
		}
	}

	public interface ITextExtractFilter3
	{
		// Dictionary<string, PdfTreeLeaf> ToFind { get; set; } 

		bool Match(string toMatch);

		int MinLength { get; set; }
		int MaxLength { get; set; }
	}
}