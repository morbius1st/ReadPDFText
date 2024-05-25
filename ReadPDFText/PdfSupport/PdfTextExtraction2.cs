#region + Using Directives
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonPdfCodeShCode;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   4/14/2024 2:55:46 PM

namespace SharedCode.ShDataSupport.PdfSupport
{

	public class LocationTextExtractor5 : LocationTextExtractionStrategy
	{
		private int idx = -1;
		private int lastPos;
		private bool normalOrientationIsVertical;

		private TextAndLineSegmentData sheetNumberTals;

		private TextSegmentExtractor tse;
		public List<TextAndLineSegmentData> list;

		private ITextExtractorFilter2 filter;

		private float bsX = -1;
		private float bsY = -1;
		private float teX;
		private float teY;
		
		public LocationTextExtractor5(ITextExtractorFilter2 filter, 
			bool normalOrientationIsVertical)
		{
			Debug.WriteLine("starting extractor 5");
			this.filter = filter;
			this.normalOrientationIsVertical = normalOrientationIsVertical;

			tse = new TextSegmentExtractor();
			tse.lte5 = this;

			list = new List<TextAndLineSegmentData>();

			OnPage = 0;

			tse.breakpoint = false;

			tse.prefaceSpaceCount = 0;
			tse.suffixSpaceCount = 0;

			// tse.GotSegment += TseOnGotSegment;
		}

		public int PrefaceSpaceCounts => tse.prefaceSpaceCount;
		public int SuffixSpaceCounts => tse.suffixSpaceCount;

		public void Flip()
		{
			tse.breakpoint = true;
		}

		public void Reset()
		{
			tse = new TextSegmentExtractor();
			tse.lte5 = this;

			list = new List<TextAndLineSegmentData>();

			tse.prefaceSpaceCount = 0;
			tse.suffixSpaceCount = 0;

		}

		public TextAndLineSegmentData SheetNumberTals
		{
			get => sheetNumberTals;
			set
			{
				sheetNumberTals = value;

				bsX = SheetNumberTals.BSV.Get(0);
				bsY = SheetNumberTals.BSV.Get(1);

				teX = SheetNumberTals.TSV.Get(0);
				teY = SheetNumberTals.TSV.Get(1);

			}
		}


		private bool matchSheetNumber(TextAndLineSegmentData tals)
		{
			if (tals==null) return false;
			if (bsX==-1 && bsY == -1) return true;

			bool result;

			result = tals.BSV.Get(0) == bsX && tals.BSV.Get(1) == bsY
				&& tals.TEV.Get(0) == teX && tals.TEV.Get(1) == teY;

			return result;
		}

		public override void EventOccurred(IEventData data, EventType type)
		{
			// if (idx++ > 600) return;

			try
			{

				if (type != EventType.RENDER_TEXT
					&& type != EventType.BEGIN_TEXT
					&& type != EventType.END_TEXT
					) return;

				if (type == EventType.BEGIN_TEXT)
				{
					tse.SetBeginText();
				}
				else if (type == EventType.END_TEXT)
				{
					tse.SetEndText();
				}
				else // (type == EventType.RENDER_TEXT)
				{
					tse.SetRenderText((TextRenderInfo) data);
				}

			}
			catch (Exception e)
			{
				Console.WriteLine("at event occured");
				Console.WriteLine(e);
				int a = 1;
			}

		}

		public int OnPage { get; set; }

		public void FinalizeProcessing()
		{
			tse.Finalize(  null, "X");
		}

		public void TseOnGotSegment(object sender, LocationTextExtractor4Data data)
		{
			// if (data.Text == null) return;
			
			TextAndLineSegmentData result;
			lastPos = 0;

			idx++;
			// Debug.Write("*");

			string[] words = data.Text.Split(new char[] { ' ', '/', ',' } );
			words2 = words;
			int pos;

			if (words.Length == 1)
			{
				if (tse.breakpoint) Debug.WriteLine($"got word| {words[0]}");

				if (filter == null || filter.Test(data.Text))
				{
					result = getTals(data.Text, data);

					if (!matchSheetNumber(result)) return;

					result.Process = data.Process;
					result.Process = "og-A";
					list.Add(result);
				}
			}
			else
			{
				if (tse.breakpoint) Debug.WriteLine("got words |");
				foreach (string word in words)
				{
					if (tse.breakpoint) Debug.Write($">{word}< ");

					if (filter == null || filter.Test(word))
					{
						result = ExtractTals(word, lastPos, data);

						if (!matchSheetNumber(result)) return;

						result.Process = data.Process;
						result.Process = "og-B";
						list.Add(result);
					}
				}
				if (tse.breakpoint) Debug.WriteLine("\n");
			}
		}

		private string[] words2;

		private TextAndLineSegmentData getTals(string word, TextRenderInfo start, TextRenderInfo end)
		{
			Vector TS = start.GetAscentLine().GetStartPoint();
			Vector TE = end.GetAscentLine().GetEndPoint();

			Vector BS = start.GetDescentLine().GetStartPoint();
			Vector BE = end.GetDescentLine().GetEndPoint();

			LineSegment top = new LineSegment(TS, TE);
			LineSegment bottom = new LineSegment(BS, BE);

			TextAndLineSegmentData result = 
				new TextAndLineSegmentData(word, bottom, top, normalOrientationIsVertical);



			return result;
		}

		private TextAndLineSegmentData getTals(string word, LocationTextExtractor4Data data)
		{
			return getTals(word, data.StartRi, data.EndRi);
		}

		// get from a phrase - which may have more than one match
		private TextAndLineSegmentData ExtractTals( string word, int start, LocationTextExtractor4Data data)
		{
			if (data.StartRi == null) return null;
			if (string.IsNullOrEmpty(word)) return null;

			int startIdx =  data.Text.IndexOf(word, start);
			int endIdx = startIdx + word.Length - 1;
			lastPos = endIdx;
			TextAndLineSegmentData tals = null;
			IList<TextRenderInfo> charRis = null;

			// string[] words = words2;


			TextAndLineSegmentData a = sheetNumberTals;

			try
			{

				charRis = data.StartRi.GetCharacterRenderInfos();

				string t = charRis[startIdx].GetText();

				TextRenderInfo ris = charRis[startIdx];
				TextRenderInfo rie = charRis[endIdx];


				tals = getTals(word, charRis[startIdx], charRis[endIdx]);

			}
			catch (Exception e)
			{
				string text = data.Text;
				int count = charRis.Count;
				Debug.WriteLine(e.Message);
				int x = 1;
			}

			// if (tals == null)
			// {
			// 	int i = 1;
			// }

			return tals;
		}
	}

	public class TextSegmentExtractor
	{
		private const float MAX_POS_X_DIF = 0.5f;
		private const float MAX_NEG_X_DIF = -0.5f;

		private TextRenderInfo ri;
		private TextRenderInfo riPrior;
		private TextRenderInfo riStart;

		public LocationTextExtractor5 lte5 { get; set; }


		private string text;
		private string result;

		private bool? gotStartEnd = null;
		private bool atBegining = true;
		private bool spaceStart = false;

		private float xDif;
		private float yDif;

		private int idx = 0;

		private string process;

		public bool breakpoint = false;

		public int prefaceSpaceCount { get; set; } = 0;
		public int suffixSpaceCount { get; set; } = 0;
		

		// public TextSegmentExtractor() { }

		// methods

		public void SetBeginText()
		{
			gotStartEnd = true;
			atBegining = true;

			if (ri != null)
			{
				ri.ReleaseGraphicsState();
				ri = null;
			}

			// Debug.WriteLine("begin text");
		}

		public void SetEndText()
		{
			gotStartEnd = false;
			process = "lte-A";
			if (result != null) Finalize(  ri, "A");

			// Debug.WriteLine("end text");
		}

		public void SetRenderText(TextRenderInfo data)
		{
			// Debug.WriteLine("render text");

			if (data.GetText().Length == 1 && 
				(data.GetText()[0] == ' '
				|| data.GetText()[0] == '/'
				|| data.GetText()[0] == ',')
				)
			{
				process = "lte-D";
				if (result!=null) Finalize(  ri, "D");

				if (ri != null)
				{
					// ri.ReleaseGraphicsState();
					ri = null;
				}

				return;
			}

			riPrior = ri;
			// if (ri!= null) ri.ReleaseGraphicsState();

			ri = data;
			ri.PreserveGraphicsState();

			// if (riPrior ==  null) riPrior = ri;

			text = ri.GetText();


			// if (text.Length == 1)
			// {
			// 	TextRenderInfo rr = ri;
			// 	IList<TextRenderInfo> rrs = ri.GetCharacterRenderInfos();
			//
			// 	IList<TextRenderInfo> rrs2 = rrs[0].GetCharacterRenderInfos();
			// }

			// if (text.Length>0)
			// {
			// 	if (text[0] == ' ') prefaceSpaceCount++;
			// 	if (text[text.Length-1] == ' ') suffixSpaceCount++;
			// }
			//
			// if (text.Length < 4)
			// {
			// 	Debug.Write($">{text}< ");
			// }
			// else
			// {
			// 	Debug.WriteLine($">{text}<");
			// }
			//
			//
			// if (idx > 50)
			// {
			// 	Debug.Write("\n");
			// 	idx = 0;
			// }


			if (text.Length > 1 && !text.IsVoid())
			{
				text = text.TrimStart();
			}

			idx++;
			
			// if (idx > 31)
			// {
			// 	int b = 1;
			// }

			// if (!gotStartEnd.HasValue)
			// {
			// 	result = text;
			// 	riStart= ri;
			// 	Finalize(ri, "E");
			// 	return;
			// }

			

			// if (string.IsNullOrEmpty(text))
			// {
			// 	Debug.WriteLine("got null or empty");
			// 	return;
			// }

			if (atBegining)
			{
				riStart = riPrior == null? ri : riPrior;
				// riStart = riPrior;
				// riStart = ri;
				riStart.PreserveGraphicsState();

				atBegining = false;
			}

			processText(text);
		}

		private void processText(string text)
		{
			if (text.Length == 1)
			{
				// if (idx > 5940 && idx < 5960)
				// {
				// 	Debug.WriteLine($"{idx} | >{text}<");
				// }
				/*
				if (text[0] == ' ')
				{
					if (!result.IsVoid())
					{


						// got a sequence
						finalizeText("B");
					}

					if (ri != null)
					{
						ri.ReleaseGraphicsState();
						ri = null;
					}

					return;
				}
				else
				*/
				{
					if (riPrior != null)
					{
						xDif = ri.GetDescentLine().GetStartPoint().Get(0) -
							riPrior.GetDescentLine().GetEndPoint().Get(0);

						yDif = ri.GetDescentLine().GetStartPoint().Get(1) -
							riPrior.GetDescentLine().GetEndPoint().Get(1);

						if ((xDif > 0.5f || xDif < -0.5f || yDif != 0f))
						{
							// Debug.WriteLine($"xDif {xDif,-6:F3} | yDif {yDif,-6:F3}");
							process = "lte-C";
							Finalize(  riPrior, "C");
						}
					}
				}

			} 

			result += text;

			// if (breakpoint)
			// {
			// 	if (result.StartsWith("FOR WATER"))
			// 	{
			// 		int c3 = (int) result[3];
			//
			// 		int x = 0;
			// 	}
			// }
		}

		public void Finalize(  TextRenderInfo riEnd, string id)
		{
			if (result == null)
			{
				// Debug.WriteLine("finalize result is null");
				return;
			}

			if (riEnd == null ) riEnd = riPrior;
			
			LocationTextExtractor4Data data = 
				new LocationTextExtractor4Data(result, riStart, riEnd ?? riStart);

			data.Process = process;

			IList<TextRenderInfo> a = ri.GetCharacterRenderInfos();
			IList<TextRenderInfo> p = riPrior?.GetCharacterRenderInfos() ?? null;
			IList<TextRenderInfo> b = riStart?.GetCharacterRenderInfos() ?? null;
			IList<TextRenderInfo> c = riEnd?.GetCharacterRenderInfos() ?? null;

			lte5.TseOnGotSegment(this, data);

			atBegining = true;
			result = null;

			riStart.ReleaseGraphicsState();
		}

		// event when a segment is created
		public delegate void GotSegmentEventHandler(object sender, LocationTextExtractor4Data data);

		public event TextSegmentExtractor.GotSegmentEventHandler GotSegment;

		protected virtual void RaiseGotSegmentEvent(LocationTextExtractor4Data tals)
		{
			GotSegment?.Invoke(this, tals);
		}
	}



	public class TextExtractorFilter2 : ITextExtractorFilter2
	{
		public TextExtractorFilter2(  Dictionary<string, PdfTreeLeaf> toFind, int minLength, int maxLength)
		{
			MinLength = minLength;
			MaxLength = maxLength;
			ToFind = toFind;
		}


		public  Dictionary<string, PdfTreeLeaf> ToFind { get; set; }
		public int MinLength { get; set; }
		public int MaxLength { get; set; }


		public bool Test(string word)
		{
			if (word.Length < MinLength || word.Length > MaxLength) return false;

			if (ToFind.ContainsKey(word))
			{
				return true;
			}

			return false;
		}
	}

	public struct LocationTextExtractor4Data
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
			set
			{
				 process += " | " + value;
			}
		}

		public LocationTextExtractor4Data(string text, TextRenderInfo startRi, TextRenderInfo endRi)
		{
			Text = text;
			StartRi = startRi;
			EndRi = endRi;
			Index = -1;
			IsValid = true;
			process = "i";
		}

		public static LocationTextExtractor4Data Invalid
		{
			get
			{
				LocationTextExtractor4Data results = new LocationTextExtractor4Data();
				results.IsValid = false;

				return results;
			}
		}
	}

	public interface ITextExtractorFilter2
	{
		Dictionary<string, PdfTreeLeaf> ToFind { get; set; } 

		int MinLength { get; set; }
		int MaxLength { get; set; }

		bool Test(string word);
	}

}
