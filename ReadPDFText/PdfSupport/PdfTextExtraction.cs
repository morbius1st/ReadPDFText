#region + Using Directives

using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser;
using System;
using System.Collections.Generic;
using System.Text;
using CommonPdfCodeShCode;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf;
using System.Diagnostics;

#endregion

// user name: jeffs
// created:   2/25/2024 7:45:10 PM

namespace SharedCode.ShDataSupport.PdfSupport
{
	/*
	public class PdfTextExtraction
	{
		public override string ToString()
		{
			return $"this is {nameof(PdfTextExtraction)}";
		}
	}
	*/

	// used to filter the sheet number versus a rectangle
	public class SheetNumLocationFilter : TextRegionEventFilter
	{
		private int count = 0;
		private bool found;
		private Rectangle shtNumLoc;

		public TextAndLineSegmentData talData { get; private set; }

		public List<TextAndLineSegmentData> tals { get; private set; }

		// private int pageRotation;
		private bool normalOrientationIsVertical { get; set; }

		public SheetNumLocationFilter(Rectangle shtNumLocation, /*int pageRotation,*/
			bool normalOrientationIsVertical) : base(shtNumLocation)
		{
			// this.pageRotation = pageRotation;
			this.normalOrientationIsVertical = normalOrientationIsVertical;
			shtNumLoc = shtNumLocation;
			talData = TextAndLineSegmentData.Invalid();

			// todo remove?
			tals = new List<TextAndLineSegmentData>();

			found = false;

			// Debug.WriteLine($"\tsht num loc| {ReadPDFText.fmtRect(shtNumLoc)}");
		}

		public override bool Accept(IEventData data, EventType type)
		{

			if (found || type.Equals(EventType.RENDER_TEXT))
			{
				TextRenderInfo ri = data as TextRenderInfo;

				string text = ri.GetText();

				// Debug.Write($">{text}< ");
				//
				// if (count++ > 20)
				// {
				// 	Debug.Write("\n");
				// 	count= 0;
				// }

				
				if (text.Length > 12) return false;

				if (text.Length< 13 && shtNumLoc.Contains(TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine())))
				{
					talData = new TextAndLineSegmentData(ri.GetText(), ri.GetAscentLine(), ri.GetDescentLine(), normalOrientationIsVertical);
					
					found = true;
					
					return true;
				}

				// todo remove??
				// tals.Add(new TextAndLineSegmentData(ri.GetText(), ri.GetDescentLine(), ri.GetAscentLine(), normalOrientationIsVertical));
				

			}

			return false;
		}

		private void showInfo(TextRenderInfo ri /*, int pageRotation */, Rectangle r)
		{
			TextAndLineSegmentData tals = new TextAndLineSegmentData(ri.GetText(), ri.GetDescentLine(), ri.GetAscentLine(), normalOrientationIsVertical);

			Console.WriteLine($"{tals.Text}");

			// Console.WriteLine($"    test rect | {PdfText5.fmtRect(shtNumLoc)}");
			// Console.WriteLine($"     act rect | {PdfText5.fmtRect(r)}\n");

			// Console.WriteLine($"       pg rot | {pageRotation}");
			Console.WriteLine($"norm or'n vert| {normalOrientationIsVertical}");
			// Console.WriteLine($"        angle | {tals.GetAngle():F4}");
			Console.WriteLine($"     top line | {tals.TSV.Get(0):F4}, {tals.TSV.Get(1):F4}  to  {tals.TEV.Get(0):F4}, {tals.TEV.Get(1):F4}");
			Console.WriteLine($"     bott line| {tals.BSV.Get(0):F4}, {tals.BSV.Get(1):F4}  to  {tals.BEV.Get(0):F4}, {tals.BEV.Get(1):F4}");
			Console.WriteLine($"     t corners| [2] | {tals.CornerPoints[2].x:F4}, {tals.CornerPoints[2].y:F4} | [3] | {tals.CornerPoints[3].x:F4}, {tals.CornerPoints[3].y:F4}");
			Console.WriteLine($"     b corners| [0] | {tals.CornerPoints[0].x:F4}, {tals.CornerPoints[0].y:F4} | [1] | {tals.CornerPoints[1].x:F4}, {tals.CornerPoints[1].y:F4}");
			Console.WriteLine($"        min x | {tals.MinX:F4}  | min y | {tals.MinY:F4}");
			Console.WriteLine($"            W | {tals.Width:F4}  |    H | {tals.Height:F4}");
			Console.WriteLine($"         run1 | {tals.run1:F4} | run2 | {tals.run2:F4} | rise1 | {tals.rise1:F4} | rise2 | {tals.rise2:F4}");
			// Console.WriteLine($"         OA W | {tals.OAwidth:F4} | OA H | {tals.OAheight:F4}");
			Console.Write($"\n");
		}
	}

	// gets the sheet number
	class SimpleTextAndLocationXstrategy : SimpleTextExtractionStrategy
	{
		public string SheetNumber { get; private set; }
		private int count = 0;
		private float lastY = -1;
		private float newY = -1;
		private TextRenderInfo riStart;
		private TextRenderInfo riPrior;
		private TextRenderInfo ri = null;
		private string textString = null;
		private bool showText;
		private bool found;
		private bool gotIt;
		private Rectangle shtNumLoc;
		private bool normalOrientationIsVertical { get; set; }

		public StringBuilder Sb { get; set; }
		
		public TextAndLineSegmentData talData { get; private set; }
		
		public SimpleTextAndLocationXstrategy(Rectangle shtNumLocation, bool normalOrientationIsVertical, bool showText = false)
		{
			shtNumLoc = shtNumLocation;
			this.normalOrientationIsVertical = normalOrientationIsVertical;

			talData = TextAndLineSegmentData.Invalid();

			found = false;
			gotIt = false;
			this.showText = showText;

			Sb=new StringBuilder();
		}

		public TextAndLineSegmentData GetShtNumData(PdfPage page)
		{
			PdfTextExtractor.GetTextFromPage(page, this);

			return talData;
		}

		public override void EventOccurred(IEventData data, EventType type)
		{
			// if (found || !type.Equals(EventType.RENDER_TEXT)) return;
			if (gotIt || !type.Equals(EventType.RENDER_TEXT)) return;

			riPrior = ri;
			
			if (riPrior != null) riPrior.PreserveGraphicsState();

			ri = (TextRenderInfo) data;
			ri.PreserveGraphicsState();

			string text = ri.GetText();
			// int val = text == null ? 0 : text[0];

			// string atext = ri.GetActualText();
			// int aval = atext == null ? 0 : atext[0];

			// Sb.Append(text);

			// string text2 = ri.GetActualText();
			// string text3 = ri.GetExpansionText();
			// PdfString ps = ri.GetPdfString();
			// string text4 = ps.GetValue();
			// string text5 = ps.ToUnicodeString();

			/*
			if (text == null) return;

			if (text[0] == ' ')
			{
				if (textString == null) return;
				text = textString;
			}
			else
			{
				textString += text;
				return;
			}


			textString = null;

			*/			

			// if (text.Length > 12) return;


			
			// if (true && count++ < 300)
			// {
			// 	newY = ri.GetBaseline().GetStartPoint().Get(1);
			//
			// 	if (newY != lastY 
			// 		|| val == 10 || val == 13
			// 		|| aval == 10 || aval == 13
			// 		) // || text[0]==' ')
			// 	{
			// 		Debug.WriteLine($"  {newY:F3} vs {lastY:F3}");
			//
			// 		Debug.Write("\n");
			// 	}
			//
			//
			// 	lastY = ri.GetBaseline().GetEndPoint().Get(1);
			//
			// 	Debug.Write($"{text}");
			//
			// 	// if (count++ == 20)
			// 	// {
			// 	// 	Debug.Write("\n");
			// 	// 	count = 0;
			// 	// }
			//
			//
			// 	// showText1(text, "primary", false);
			// 	// showText1(text2, "text2", true);
			// 	// showText1(text3, "text3", true);
			// 	// showText1(text4, "text4", true);
			// 	// showText1(text5, "text5", true);
			//
			//
			// 	// Debug.WriteLine($"{text,-40}| char {(int) text[0],-8}| x| {r.GetX(),8:F2}| y| {r.GetY(),8:F2}| w| {r.GetWidth(),8:F2}| h| {r.GetHeight(),8:F2}");
			// }


			if (!gotIt)
			{
				Rectangle r = TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine());

				if (shtNumLoc.Contains(r))
				{
					// Debug.WriteLine($"\ngot one| {text}\n");

					SheetNumber += text;

					if (!found)
					{
						riStart = ri;
						riStart.PreserveGraphicsState();
						found = true;

						// Debug.Write($"\nstart top| x| {riStart.GetAscentLine().GetStartPoint().Get(0)}");
						// Debug.Write($"| y| {riStart.GetAscentLine().GetStartPoint().Get(1)}\n");
						//
						// Debug.Write($"start bot| x| {riStart.GetDescentLine().GetStartPoint().Get(0)}");
						// Debug.Write($"| y| {riStart.GetDescentLine().GetStartPoint().Get(1)}\n");
					}
				}
				else if (found == true)
				{
					LineSegment al = new LineSegment(riStart.GetAscentLine().GetStartPoint(),  riPrior.GetAscentLine().GetEndPoint());
					LineSegment dl = new LineSegment(riStart.GetDescentLine().GetStartPoint(), riPrior.GetDescentLine().GetEndPoint());

					talData = new TextAndLineSegmentData(SheetNumber, dl, al, normalOrientationIsVertical);

					found = false;
					gotIt= true;

					// Debug.WriteLine($"\ngot one| {SheetNumber}\n");

					// Debug.Write($"\n  end top| x| {riPrior.GetAscentLine().GetEndPoint().Get(0)}");
					// Debug.Write($"| y| {riPrior.GetAscentLine().GetEndPoint().Get(1)}\n");
					//
					// Debug.Write($"  end bot| x| {riPrior.GetDescentLine().GetEndPoint().Get(0)}");
					// Debug.Write($"| y| {riPrior.GetDescentLine().GetEndPoint().Get(1)}\n");
				}
			}			
		}

		private void showText1(string text, string title, bool tab)
		{
			if (tab)
			{
				Debug.Write("\t");
			}

			if (text == null)
			{
				Debug.WriteLine($"{title,-12}| is null");
				return;
			}

			Debug.WriteLine($"{title,-10}| len {text.Length,-4}| {text,-20}| char {(int) text[0]}");  // | x| {r.GetX(),8:F2}| y| {r.GetY(),8:F2}| w| {r.GetWidth(),8:F2}| h| {r.GetHeight(),8:F2}");
		}

	}

	/// <summary>
	/// finds all text per page.  adds the found text along with
	/// its position information to a list for each string that
	/// matches the list of strings to find
	/// </summary>
	// class TextAndLocationXstrategy5 : PdfTextAndLocationStrategy
	// searches the text on a page for matches
	class TextAndLocationXstrategy : SimpleTextExtractionStrategy
	{
		private  Dictionary<string, PdfTreeLeaf> toFind;
		public Rectangle sheetNumberArea { private get; set; }

		private int onPage;

		private int minLen;
		private int maxLen;

		private string priorString;
		private string textString = null;

		private TextRenderInfo riStart;
		private TextRenderInfo riPrior;
		private TextRenderInfo ri = null;

		private bool gotStart = false;

		public List<TextAndLineSegmentData> list;

		private bool normalOrientationIsVertical;

		public TextAndLocationXstrategy(
			Dictionary<string, PdfTreeLeaf> toFindList,
			int minLen, int maxLen,
			bool normalOrientationIsVertical)
		{
			toFind = toFindList;

			this.minLen = minLen;
			this.maxLen = maxLen;

			this.normalOrientationIsVertical = normalOrientationIsVertical;

			priorString = "first time through";
		}

		public List<TextAndLineSegmentData> GetList(PdfPage page, int currPage)
		{
			list = new List<TextAndLineSegmentData>();
			
			onPage = currPage;
		
			PdfTextExtractor.GetTextFromPage(page, this);
		
			return this.list;
		}

		public override void EventOccurred(IEventData data, EventType type)
		{
			if (!type.Equals(EventType.RENDER_TEXT)) return;

			riPrior = ri;
			if (riPrior != null) riPrior.PreserveGraphicsState();

			ri = (TextRenderInfo) data;
			ri.PreserveGraphicsState();

			// IList<TextRenderInfo> t = ri.GetCharacterRenderInfos();

			string text = ri.GetText();

			// Debug.WriteLine($">{text}<");
			// return;

			if (text == null) return;

			if (text[0] == ' ')
			{
				if (textString == null) return;
				text = textString;
				gotStart = false;
			}
			else
			{
				textString += text;

				if (!gotStart)
				{
					gotStart = true;
					riStart = ri;
					riStart.PreserveGraphicsState();
				}

				return;
			}

			textString = null;

			Rectangle r;

			// determine if the text is a sheet number (==1) 
			// or it is too short (len <minLength) (==2)
			// or is anything else (== -1)

			int result = validateText(text);

			// Console.Write($"validation results| {result} | ");

			// if 1 - got a reference - add and move on
			if (result == 1)
			{
				// if (priorString.Length < 5) Debug.WriteLine($"found simple| prior text| {priorString,-5}| found text| {text,-10} ");

				// r = TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine(), normalOrientationIsVertical);
				LineSegment al = new LineSegment(riStart.GetAscentLine().GetStartPoint(),  riPrior.GetAscentLine().GetEndPoint());
				LineSegment dl = new LineSegment(riStart.GetDescentLine().GetStartPoint(), riPrior.GetDescentLine().GetEndPoint());

				r = TextAndLineSegmentData.GetRectangle(dl, al);
				// r = TextAndLineSegmentData.GetRectangle(ri.GetDescentLine(), ri.GetAscentLine());

				// add if not the actual sheet's sheet number - not in the sheet number find area
				if (!sheetNumberArea.Contains(r))
				{
					// Console.WriteLine($"1 adding| {text}");
					list.Add(new TextAndLineSegmentData(text, dl, al, normalOrientationIsVertical, onPage));
				}
			}
			// got a string - check the string parts for a match
			else if (result == -1)
			{
				// Console.Write("found complex | ");

				List<string> matches;

				string[] words = text.Split(new char[] { ' ', '/', ',' });

				if (words.Length>1 && validateTextString(words, out matches))
				{
					// Console.Write($"found {matches.Count} matches | ");

					TextAndLineSegmentData reference;

					foreach (string match in matches)
					{
						// Console.Write("m");

						int len = match.Length;

						int start = text.IndexOf(match);

						while (start >= 0)
						{
							// Console.Write(".");

							reference = getRefFromString(match, start, ri);

							// Console.WriteLine($"2 adding| {text}");
							list.Add(reference);

							start = text.IndexOf(match, start + len);
						}
					}
				}
				// else
				// {
				// 	Console.Write("but no matches");
				// }
			}
			// else
			// {
			// 	Console.Write($"ignore all");
			// }

			// Console.Write("\n");

			priorString = text;

			base.EventOccurred(data, type);
		}

		private int validateText(string findString)
		{
			string testString = findString.Trim();

			// if this - process as a string
			if (testString.IndexOfAny(new char[] { ' ', '/', ',' }) != -1) return -1;

			if (toFind.ContainsKey(testString) ) return 1;

			int len = testString.Length;
			// string findString = testString.Trim();

			// 2 means - ignore
			if (len < minLen || len > maxLen) return 2;

			// -1 means - process the string
			// if (processStrings && len > minLen) return -1;

			// always process the string
			return  -1;

		}


		/*
		private bool validateTextString2(string text, out List<string> matches)
		{
			string[] x = text.Split(new char[] { ' ' });
		
		
			matches = new List<string>();
		
			foreach (KeyValuePair<string, TextAndLineSegmentData> kvp in toFind)
			{
				if (text.Contains(kvp.Key))
				{
					matches.Add(kvp.Key);
				}
			}
		
			return matches.Count > 0;
		}
		*/


		// find each text that is a matches a sheeet number
		private bool validateTextString(string[] words, out List<string> matches)
		{
			matches = new List<string>();

			// string[] words = text.Split(new char[] { ' ', '/', ',' });
			int len;
			bool result;

			foreach (string word in words)
			{
				// Console.Write($" w| >{word}<");

				len = word.Length;
				if (len > maxLen || len < minLen) continue;

				if (toFind.ContainsKey(word))
				{
					matches.Add(word);
				}
			}

			return matches.Count > 0;
		}

		private TextAndLineSegmentData getRefFromString(string found, int startIdx,
			TextRenderInfo ri)
		{
			int endIdx = startIdx + found.Length - 1;

			IList<TextRenderInfo> ris = ri.GetCharacterRenderInfos();

			LineSegment top = new LineSegment(ris[startIdx].GetAscentLine().GetStartPoint(),
				ris[endIdx].GetAscentLine().GetEndPoint());

			LineSegment bott = new LineSegment(ris[startIdx].GetDescentLine().GetStartPoint(),
				ris[endIdx].GetDescentLine().GetEndPoint());

			return new TextAndLineSegmentData(found, bott, top, normalOrientationIsVertical, onPage);
		}
	}

}