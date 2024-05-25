#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using iText.Forms.Form.Renderer;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;
using iText.StyledXmlParser.Jsoup.Parser;
using SharedCode.ShDataSupport;
using SharedCode.ShDataSupport.PdfSupport;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   4/12/2024 12:47:16 PM

namespace ReadPDFText.Process
{
	public class PdfText100
	{
		private PdfDocument destPdfDoc ;
		private Document destDoc;

		private string[] sources = new []
		{
			@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test3\PDF files\T1.0-0 - TITLE SHEET.pdf",
			@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test3\PDF files\A0.1.0 - COVER SHEET.pdf"
		};

		private string dest = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test3\combined.pdf";

		public void ProcessPdf()
		{
			Rectangle r;
			PdfPage page;
			string result;

			WriterProperties wp = new WriterProperties();
			wp.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
			wp.UseSmartMode();
			wp.SetFullCompressionMode(true);

			PdfWriter w = new PdfWriter(dest, wp);

			destPdfDoc = new PdfDocument(w);
			destDoc = new Document(destPdfDoc);

			merge(sources);

			TextAndLineSegmentData tals;

			// document 1 - line by line
			// 36x48
			r = new Rectangle(40, 55, 180, 157);

			page = destPdfDoc.GetPage(1);

			result = Extract(page, r);
			Debug.WriteLine($"*** got result 0| {result ?? "null"}");

			tals = Extract1(page, r);
			Debug.WriteLine($"*** got result 0| {tals?.Text ?? "null"}");


			// result = Extract3(page);
			// Debug.WriteLine($"*** got result 3| {result ?? "null"}");

			// works good - turn in to the actual extractor
			// result = Extract10(page);

			List<TextAndLineSegmentData> answer;
			List<string> sheetNumbers;

			sheetNumbers = new List<string>()
			{
				"T1.0", "T2.0", "T3.0", "T4.0", "T1.1", "T1.2", "T1.3"
			};

			answer = Extract12(page, sheetNumbers);
			showFoundList(answer);




			// document 2 - character by character
			// 30x42
			r = new Rectangle(2793, 40, 180, 140);

			page = destPdfDoc.GetPage(2);

			result = Extract(page, r);
			Debug.WriteLine($"*** got result 1| {result ?? "null"}");

			tals = Extract1(page, r);
			Debug.WriteLine($"*** got result 0| {tals?.Text ?? "null"}");

			// this gets all of the text on the page as sentences
			// result = Extract2(page);
			// Debug.WriteLine($"*** got result 2| {result ?? "null"}");

			// result = Extract3(page);
			// Debug.WriteLine($"*** got result 3| {result ?? "null"}");

			// this works - gets all of the text on the page as sentences
			// result = Extract4(page);
			// Debug.WriteLine($"*** got result 4| {result ?? "null"}");

			// did not work
			// result = Extract5(page);
			// Debug.WriteLine($"*** got result 5| {result ?? "null"}");

			// did not work - returned nothing
			// result = Extract6(page);
			// Debug.WriteLine($"*** got result 6| {result ?? "null"}");

			// did not work - gets letters not words
			// result = Extract7(page);
			// Debug.WriteLine($"*** got result 7| {result ?? "null"}");

			// works but does not get begin / end text events
			// Debug.WriteLine($"\n*** | run 8 -  start | ***");
			// result = Extract8(page);
			// Debug.WriteLine($"\n*** | run 8 - complete | ***\n");

			// good so far
			// Debug.WriteLine($"\n*** | run 9 -  start | ***");
			// result = Extract9(page);
			// Debug.WriteLine($"\n*** | run 9 - complete | ***\n");

			// good so far
			/*
			Debug.WriteLine($"\n*** | run 9 -  start | ***");
			result = Extract10(page);
			Debug.WriteLine($"\n*** | run 9 - complete | ***\n");
			*/

			// result = Extract11(page);

			// works good - turn in to the actual extractor
			// result = Extract10(page);

			sheetNumbers = new List<string>()
			{
				"A0.1.0", "A0.2.0", "TA0.3.0", "A0.4.0"
			};

			answer = Extract12(page, sheetNumbers);
			showFoundList(answer);

		}

		private void merge(string[] sources)
		{
			PdfReader reader;
			PdfDocument srcPdf;
			int numPages;

			foreach (string source in sources)
			{
				reader = new PdfReader(source);
				reader.SetStrictnessLevel(PdfReader.StrictnessLevel.LENIENT);

				srcPdf = new PdfDocument(new PdfReader(source));

				numPages = srcPdf.GetNumberOfPages();

				srcPdf.CopyPagesTo(1, numPages, destPdfDoc);

				srcPdf.Close();
			}
		}

		private void showFoundList(List<TextAndLineSegmentData> found)
		{
			if (found == null)
			{
				Debug.WriteLine("*** found nothing ***");
				return;
			}

			Debug.WriteLine($"found {found.Count}");

			foreach (TextAndLineSegmentData data in found)
			{
				string startX = $"{data.BSV.Get(0):F3}";
				string startY = $"{data.BSV.Get(1):F3}";

				string endX = $"{data.BEV.Get(0):F3}";
				string endY = $"{data.BEV.Get(1):F3}";

				Debug.WriteLine($"{data.Text} | {data.Process,-20} | ({startX} , {startY}) -> to -> ({endX} , {endY})");
			}
		}

		private PdfCanvasProcessor parser;

		private string Extract(PdfPage page, Rectangle r)
		{
			string result = null;

			TextRegionEventFilter filter = new TextRegionEventFilter(r);

			FilteredEventListener listner = new FilteredEventListener();

			LocationTextExtractionStrategy strat = 
				listner.AttachEventListener(new LocationTextExtractionStrategy(), filter);

			if (parser != null) parser.Reset();

			parser = new PdfCanvasProcessor(listner);

			parser.ProcessPageContent(page);

			result = strat.GetResultantText();
			
			Debug.WriteLine($"\n*** found *** | >>{result}<<");

			return result;
		}

		private TextAndLineSegmentData Extract1(PdfPage page, Rectangle r)
		{
			TextAndLineSegmentData result = null;

			TextRegionEventFilter filter = new TextRegionEventFilter(r);

			FilteredEventListener listner = new FilteredEventListener();

			LocationTextExtractor4 strategy = new LocationTextExtractor4(null);

			LocationTextExtractor4 strat =
				listner.AttachEventListener(new LocationTextExtractor4(null), filter);

			if (parser != null) parser.Reset();

			parser = new PdfCanvasProcessor(listner);

			parser.ProcessPageContent(page);

			// make sure that last item in the queue is processed;
			strat.FinalizeProcessing();

			result = strat.list[0];

			return result;
		}

		private string Extract2(PdfPage page)
		{
			string result = null;

			CustomFilter cf = new CustomFilter();

			FilteredEventListener listener = new FilteredEventListener();

			SimpleTextExtractionStrategy strat = listener.AttachEventListener(new SimpleTextExtractionStrategy(), cf);

			if (parser != null) parser.Reset();

			parser = new PdfCanvasProcessor(listener);

			parser.ProcessPageContent(page);

			result = strat.GetResultantText();

			// Debug.WriteLine($"\n*** found *** | >>{result}<<");

			return result;
		}

		private string Extract3(PdfPage page)
		{
			string result = null;

			CustomFilter cf = new CustomFilter();

			FilteredEventListener listener = new FilteredEventListener();

			LocationTextExtractionStrategy strat = listener.AttachEventListener(new LocationTextExtractionStrategy(), cf);

			if (parser != null) parser.Reset();

			parser = new PdfCanvasProcessor(listener);

			parser.ProcessPageContent(page);

			result = strat.GetResultantText();

			// Debug.WriteLine($"\n*** found *** | >>{result}<<");

			return result;
		}

		private string Extract4(PdfPage page)
		{
			string result;

			SimpleTextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

			result = PdfTextExtractor.GetTextFromPage(page, strategy);

			return result;
		}

		private string Extract5(PdfPage page)
		{
			string result;

			SimpleTextExtractor strategy = new SimpleTextExtractor();

			result = PdfTextExtractor.GetTextFromPage(page, strategy);

			return result;
		}


		private string Extract6(PdfPage page)
		{
			string result;

			LocationTextExtractor strategy = new LocationTextExtractor();

			result = PdfTextExtractor.GetTextFromPage(page, strategy);

			return result;
		}


		private string Extract7(PdfPage page)
		{
			string result = null;

			CustomFilter cf = new CustomFilter();

			FilteredTextEventListener listener = new FilteredTextEventListener(new SimpleTextExtractionStrategy(), cf);

			if (parser != null) parser.Reset();

			parser = new PdfCanvasProcessor(listener);

			parser.ProcessPageContent(page);

			result = listener.GetResultantText();

			Debug.WriteLine($"\n*** found *** | >>{result}<<");

			return result;
		}

		// good - but does not get start / end text
		private string Extract8(PdfPage page)
		{
			string result;

			SimpleTextExtractor2 strategy = new SimpleTextExtractor2();

			result = PdfTextExtractor.GetTextFromPage(page, strategy);

			Debug.WriteLine($"\n\nx min| {strategy.xDifMin:F4} | x max| {strategy.xDifMax:F4}");
			// Debug.WriteLine($"y min| {strategy.yDifMin:F4} | y max| {strategy.yDifMax:F4}");

			return result;
		}

		private string Extract9(PdfPage page)
		{
			string result;

			LocationTextExtractor2 strategy = new LocationTextExtractor2();

			result = PdfTextExtractor.GetTextFromPage(page, strategy);

			Debug.WriteLine($"\n\nx min| {strategy.xDifMin:F4} | x max| {strategy.xDifMax:F4}");
			// Debug.WriteLine($"y min| {strategy.yDifMin:F4} | y max| {strategy.yDifMax:F4}");

			return result;
		}

		private string Extract10(PdfPage page)
		{
			string result;

			LocationTextExtractor3 strategy = new LocationTextExtractor3();

			result = PdfTextExtractor.GetTextFromPage(page, strategy);

			Debug.WriteLine($"\n\nx min| {strategy.xDifMin:F4} | x max| {strategy.xDifMax:F4}");
			// Debug.WriteLine($"y min| {strategy.yDifMin:F4} | y max| {strategy.yDifMax:F4}");

			return result;
		}

		// interesting - but cannot help
		private string Extract11(PdfPage page)
		{
			Regex r = new Regex(@".?\d\.\d\.\d.??|.?\d\.\d.??|.?\d.??");

			RegexBasedLocationExtractionStrategy strategy = new RegexBasedLocationExtractionStrategy(r);

			if (parser != null) parser.Reset();

			parser = new PdfCanvasProcessor(strategy);

			parser.ProcessPageContent(page);

			ICollection < IPdfTextLocation > locations = strategy.GetResultantLocations();

			return locations.Count.ToString();

		}


		
		private List<TextAndLineSegmentData>  Extract12(PdfPage page, List<string> shtNums)
		{
			string result;

			TextExtractorFilter filter = new TextExtractorFilter(3, 8, shtNums);

			LocationTextExtractor4 strategy = new LocationTextExtractor4(filter);

			result = PdfTextExtractor.GetTextFromPage(page, strategy);

			List<TextAndLineSegmentData> list = strategy.list;

			return list;
		}
	}

	public class TextExtractorFilter : ITextExtractorFilter
	{
		public TextExtractorFilter(int minLength, int maxLength, List<string> toFindList)
		{
			MinLength = minLength;
			MaxLength = maxLength;
			ToFindList = toFindList;
		}


		public List<string> ToFindList { get; set; }
		public int MinLength { get; set; }
		public int MaxLength { get; set; }


		public bool Test(string word)
		{
			if (word.Length < MinLength || word.Length > MaxLength) return false;

			if (ToFindList.Contains(word))
			{
				return true;
			}

			return false;
		}
		//
		// public LocationTextExtractor4Data TestString(string data)
		// {
		// 	LocationTextExtractor4Data result;
		//
		// 	int pos;
		//
		// 	string[] words = data.Split(new char[] { ' ', '/', ',' } );
		//
		// 	foreach (string word in words)
		// 	{
		// 		if (Test(word))
		// 		{
		// 			pos = data.IndexOf(word);
		//
		// 			result = new LocationTextExtractor4Data(word, null, null);
		// 			result.Index = pos;
		// 		}
		// 	}
		//
		// }

	}


	public  class PassThruFilter : IEventFilter
	{
		public bool Accept(IEventData data, EventType type)
		{
			return true;
		}

	}

	public  class CustomFilter : IEventFilter
	{
		private int idx = 0;
		private string text;
		private bool result = true;
		private float xDif;
		private float yDif;

		private TextRenderInfo ri;
		private TextRenderInfo riPrior;

		public bool Accept(IEventData data, EventType type)
		{
			if (type != EventType.RENDER_TEXT
				&& type != EventType.BEGIN_TEXT
				&& type != EventType.END_TEXT
				) return false;

			if (idx++ < 500)
			{
				if (type == EventType.BEGIN_TEXT)
				{
					Debug.Write($"*** >> ");
				}
				else if (type == EventType.END_TEXT)
				{
					Debug.Write($"<< ***");
				}
				else
				{
					riPrior = ri;
					if (ri != null) ri.ReleaseGraphicsState();

					ri = (TextRenderInfo) data;

					ri.PreserveGraphicsState();

					text = ri.GetText();

					if (!string.IsNullOrEmpty(text))
					{
						Debug.Write($"{text}");

						if (text.Length == 1)
						{
							Debug.Write($" | ({(int) text[0]})");

							if (riPrior != null)
							{
								xDif = ri.GetDescentLine().GetStartPoint().Get(0) -
									riPrior.GetDescentLine().GetEndPoint().Get(0);

								yDif = ri.GetDescentLine().GetStartPoint().Get(1) -
									riPrior.GetDescentLine().GetEndPoint().Get(1);

								Debug.Write($" | x diff| {xDif} | y diff| {yDif}");
							}
						}
					}
				}

				Debug.Write("\n");
			}

			return result;
		}
	}

	public class SimpleTextExtractor : SimpleTextExtractionStrategy
	{
		public override void EventOccurred(IEventData data, EventType type)
		{
			if (!type.Equals(EventType.RENDER_TEXT)) return;

			TextRenderInfo ri = ( TextRenderInfo) data;

			if (ri.GetText().Length < 3 ||  ri.GetText().Length < 10 ) return;

			base.EventOccurred(data, type);
		}
	}


	public class SimpleTextExtractor2 : SimpleTextExtractionStrategy
	{
		private int idx = 0;
		private string text;
		private bool result = true;
		private float xDif;
		private float yDif;

		public float xDifMin { get; set; } = 1;
		public float xDifMax { get; set; } = -1;

		public float yDifMin { get; set; } = 1;
		public float yDifMax { get; set; } = -1;


		private TextRenderInfo ri;
		private TextRenderInfo riPrior;

		public override void EventOccurred(IEventData data, EventType type)
		{
			if (type != EventType.RENDER_TEXT
				&& type != EventType.BEGIN_TEXT
				&& type != EventType.END_TEXT
				) return;

			if (idx++ < 500)
			{
				if (type == EventType.BEGIN_TEXT)
				{
					Debug.Write($"*** >> ");
				}
				else if (type == EventType.END_TEXT)
				{
					Debug.Write($"<< ***");
				}
				else
				{
					riPrior = ri;
					if (ri != null) ri.ReleaseGraphicsState();

					ri = (TextRenderInfo) data;

					ri.PreserveGraphicsState();

					text = ri.GetText();

					if (!string.IsNullOrEmpty(text))
					{
						Debug.Write($"{text}");

						if (text.Length == 1)
						{
							Debug.Write($" | ({(int) text[0]})");

							if (riPrior != null)
							{
								xDif = ri.GetDescentLine().GetStartPoint().Get(0) -
									riPrior.GetDescentLine().GetEndPoint().Get(0);

								yDif = ri.GetDescentLine().GetStartPoint().Get(1) -
									riPrior.GetDescentLine().GetEndPoint().Get(1);

								Debug.Write($" | x diff| {xDif:F4} | y diff| {yDif:F4}");

								if (xDif < 0 &&
									xDif > -1 &&
									xDif < xDifMin)
								{
									xDifMin = xDif;
								}
								else if (
									xDif > 0 &&
									xDif < 1 &&
									xDif > xDifMax)
								{
									xDifMax = xDif;
								}

								// if (
								// 	yDif<0 &&
								// 	yDif > -1 &&
								// 	yDif < yDifMin)
								// {
								// 	yDifMin = yDif;
								// } 
								// else if (
								// 	yDif > 0 &&
								// 	yDif < 1 &&
								// 	yDif > yDifMax)
								// {
								// 	yDifMax = yDif;
								// }
							}
						}
					}
				}

				Debug.Write("\n");
			}

			base.EventOccurred(data, type);
		}
	}


	public class LocationTextExtractor : LocationTextExtractionStrategy
	{
		public override void EventOccurred(IEventData data, EventType type)
		{
			if (!type.Equals(EventType.RENDER_TEXT)) return;

			TextRenderInfo ri = ( TextRenderInfo) data;

			if (ri.GetText().Length < 3 ||  ri.GetText().Length < 10 ) return;

			base.EventOccurred(data, type);
		}
	}

// good so far - gets words
	public class LocationTextExtractor2 : LocationTextExtractionStrategy
	{
		private int idx = 0;
		private string text;
		private bool result = true;
		private float xDif;
		private float yDif;

		private bool foundStart = false;
		private bool gotWB = false;

		private string msg;

		public float xDifMin { get; set; } = 1;
		public float xDifMax { get; set; } = -1;

		public float yDifMin { get; set; } = 1;
		public float yDifMax { get; set; } = -1;


		private TextRenderInfo ri;
		private TextRenderInfo riPrior;

		public override void EventOccurred(IEventData data, EventType type)
		{
			if (type != EventType.RENDER_TEXT
				&& type != EventType.BEGIN_TEXT
				&& type != EventType.END_TEXT
				) return;

			if (idx++ < 1000)
			{
				if (type == EventType.BEGIN_TEXT)
				{
					Debug.Write($"*** >> ");
					foundStart = true;
				}
				else if (type == EventType.END_TEXT)
				{
					Debug.Write($"<< ***");
				}
				else
				{
					riPrior = ri;
					if (ri != null) ri.ReleaseGraphicsState();

					ri = (TextRenderInfo) data;

					ri.PreserveGraphicsState();

					text = ri.GetText();

					if (!string.IsNullOrEmpty(text))
					{
						if (text.Length == 1)
						{
							if (text[0] == ' ')
							{
								gotWB = true;

								Debug.Write($"<< www\n");
								Debug.Write($"www >>");
							}
							else
							{
								gotWB = false;

								msg = $"{($" | ({(int) text[0]})"),-8}";

								if (riPrior != null)
								{
									xDif = ri.GetDescentLine().GetStartPoint().Get(0) -
										riPrior.GetDescentLine().GetEndPoint().Get(0);

									yDif = ri.GetDescentLine().GetStartPoint().Get(1) -
										riPrior.GetDescentLine().GetEndPoint().Get(1);

									if (!foundStart &&
										(xDif > 0.5f || xDif < -0.5f || yDif != 0f))
									{
										// base.EventOccurred(null, EventType.END_TEXT);
										// base.EventOccurred(null, EventType.BEGIN_TEXT);
										Debug.Write($"<< +++\n");
										Debug.Write($"+++ >>\n");
									}

									msg += $"{($" | x diff| {xDif:F4}"),-24}{($" | y diff| {yDif:F4}")}";

									if (xDif < 0 &&
										xDif > -1 &&
										xDif < xDifMin)
									{
										xDifMin = xDif;
									}
									else if (
										xDif > 0 &&
										xDif < 1 &&
										xDif > xDifMax)
									{
										xDifMax = xDif;
									}


									// if (
									// 	yDif<0 &&
									// 	yDif > -1 &&
									// 	yDif < yDifMin)
									// {
									// 	yDifMin = yDif;
									// } 
									// else if (
									// 	yDif > 0 &&
									// 	yDif < 1 &&
									// 	yDif > yDifMax)
									// {
									// 	yDifMax = yDif;
									// }
								}

								Debug.Write($"{text}{msg}");
							}

							foundStart = false;
						}
					}
				}

				Debug.Write("\n");
			}


			base.EventOccurred(data, type);
		}
	}

// good so far - gets words
	public class LocationTextExtractor3 : LocationTextExtractionStrategy
	{
		private int idx = -1;
		private int count = 0;
		private int startIdx = 0;
		private string text;

		private string result;

		// private bool result = true;
		private float xDif;
		private float yDif;

		private int spaceCount;
		private int numSpaces;
		private int spaceIdx = -1;

		private bool foundStart = false;
		// private bool gotWB = false;

		private string msg;

		public float xDifMin { get; set; } = 1;
		public float xDifMax { get; set; } = -1;

		public float yDifMin { get; set; } = 1;
		public float yDifMax { get; set; } = -1;


		private TextRenderInfo ri;
		private TextRenderInfo riPrior;
		private TextRenderInfo riStart;

		public override void EventOccurred(IEventData data, EventType type)
		{
			if (type != EventType.RENDER_TEXT
				&& type != EventType.BEGIN_TEXT
				&& type != EventType.END_TEXT
				) return;

			if (idx++ < 300)
			{
				if (type == EventType.BEGIN_TEXT)
				{
					Debug.Write($"*** >> begin text\n");
					foundStart = true;
					// gotWB = false;
					result = null;
					text = null;
					if (ri != null) ri.ReleaseGraphicsState();
					ri = null;
					startIdx = count;
					// spaceIdx = -1;
					// spaceCount = 0;
				}
				else if (type == EventType.END_TEXT)
				{
					if (result != null && result.Length > 0)
					{

						Debug.Write($"\tA ");
						Debug.Write($"{startIdx,4:D} | len {($"({(result?.Length ?? -1):D})"),-6} | sp.c {($"({spaceCount:D})"),-6} | end {($"({(startIdx+result.Length):D}))"),-6} | {result}");
						Debug.Write($"\n");
						Debug.Write($"<< *** end text\n");
					}

					// foundStart = false;
					// gotWB = false;
					result = null;
					text = null;
					// spaceIdx = -1;
					// spaceCount = 0;
				}
				else
				{
					riPrior = ri;
					if (ri != null) ri.ReleaseGraphicsState();

					ri = (TextRenderInfo) data;

					ri.PreserveGraphicsState();

					if (foundStart)
					{
						riStart = ri;
						riStart.PreserveGraphicsState();
					}


					text = ri.GetText();

/*
					Debug.Write($"Xb ");
					Debug.Write($">{text,-2}< ct {$"({count})",5} ");
					Debug.Write($"| st.i {$"({startIdx})",5} | sp.i {$"({spaceIdx})",5} | sp.c {$"({spaceCount})",5} | len {$"({result?.Length ?? -1})",5} | {result ?? "null"}");
					Debug.Write($"\n");
*/
					
					count++;

					if (!string.IsNullOrEmpty(text))
					{
						if (text.Length == 1)
						{
							if (text[0] == ' ')
							{
								// startIdx++;

								// Debug.WriteLine("got space");

								if (spaceIdx == -1)
								{
									Debug.Write($"\tB ");
									Debug.Write($"{startIdx,4:D} | len {($"({(result?.Length ?? -1):D})"),-6} | sp.c {($"({spaceCount:D})"),-6} | end {($"({(startIdx+result.Length):D}))"),-6} | {result}");
									Debug.Write($"\n");
									Debug.Write($"<< ***  got space\n");

									result = null;
									startIdx = count - 1;
									spaceIdx = count;
									spaceCount = 0;

								}

								startIdx++;
								spaceCount++;
								// return;
							}
							else
							{

								if (riPrior != null)
								{
									xDif = ri.GetDescentLine().GetStartPoint().Get(0) -
										riPrior.GetDescentLine().GetEndPoint().Get(0);

									yDif = ri.GetDescentLine().GetStartPoint().Get(1) -
										riPrior.GetDescentLine().GetEndPoint().Get(1);

									// Debug.WriteLine($"{text,-3} | x dif {xDif,7:F3} | y dif {yDif:F3}");

									if ((xDif > 0.5f || xDif < -0.5f || yDif != 0f))
									{
										Debug.Write($"\tC ");
										Debug.Write($"{startIdx,4:D} | len {($"({(result?.Length ?? -1):D})"),-6} | sp.c {($"({spaceCount:D})"),-6} | end {($"({(startIdx+result.Length):D}))"),-6} | {result}");
										Debug.Write($"\n");
										Debug.Write($"<< ***  got pos change\n");

										result = null;
										startIdx = count;
										// spaceIdx = -1;
										spaceCount = 0;

									}
								}

								spaceIdx = -1;
								// spaceCount = 0;
							}

							// foundStart = false;
						}

						if (text[0] != ' ') result += text;

/*
						Debug.Write($"Xa ");
						Debug.Write($">{text,-2}<          ");
						Debug.Write($"| st.i {$"({startIdx})",5} | sp.i {$"({spaceIdx})",5} | sp.c {$"({spaceCount})",5} | len {$"({result?.Length ?? -1})",5} | {result ?? "null"}");
						Debug.Write($"\n");
*/
					}
					else
					{
						Debug.Write($"<< ***  got null\n");
					}

					
				}

				// Debug.Write("\n");
			}


			// base.EventOccurred(data, type);
		}
	}





	public class LocationTextExtractor4 : LocationTextExtractionStrategy
	{
		private int idx = 0;
		private int lastPos;
		private TextSegmentExtractor tse;

		private ITextExtractorFilter filter;

		public LocationTextExtractor4(ITextExtractorFilter filter)
		{
			this.filter = filter;

			tse = new TextSegmentExtractor();
			list = new List<TextAndLineSegmentData>();

			// tse.GotSegment += TseOnGotSegment;

			tse.lte4 = this;
		}

		public List<TextAndLineSegmentData> list;

		public override void EventOccurred(IEventData data, EventType type)
		{
			// if (idx++ > 600) return;

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
			else  // (type == EventType.RENDER_TEXT)
			{
				tse.SetRenderText((TextRenderInfo) data);
			}
		}

		public void FinalizeProcessing()
		{
			tse.FinalizeText("X");
		}


		public void TseOnGotSegment(object sender, LocationTextExtractor4Data data)
		{
			TextAndLineSegmentData result;
			lastPos = 0;

			string[] words = data.Text.Split(new char[] { ' ', '/', ',' } );
			int pos;

			if (words.Length == 1)
			{
				if (filter==null || filter.Test(data.Text))
				{
					result = getTals(data.Text, data);
					result.Process = data.Process;
					result.Process = "og-A";
					list.Add(result);
				}

				return;
			}

			foreach (string word in words)
			{
				if (filter == null || filter.Test(word))
				{
					result = ExtractTals(word, lastPos, data);
					result.Process = data.Process;
					result.Process = "og-B";
					list.Add(result);
				}
			}
		}

		private TextAndLineSegmentData getTals(string word, TextRenderInfo start, TextRenderInfo end)
		{
			Vector TS = start.GetAscentLine().GetStartPoint();
			Vector TE = end.GetAscentLine().GetEndPoint();

			Vector BS = start.GetDescentLine().GetStartPoint();
			Vector BE = end.GetDescentLine().GetEndPoint();

			LineSegment top = new LineSegment(TS, TE);
			LineSegment bottom = new LineSegment(BS, BE);

			TextAndLineSegmentData result = new TextAndLineSegmentData(word, bottom, top, false);

			return result;
		}

		private TextAndLineSegmentData getTals(string word, LocationTextExtractor4Data data)
		{
			return getTals(word, data.StartRi, data.EndRi);
		}

		// get from a phrase - which may have more than one match
		private TextAndLineSegmentData ExtractTals( string word, int start, LocationTextExtractor4Data data)
		{
			int startIdx =  data.Text.IndexOf(word, start);
			int endIdx = startIdx + word.Length - 1;
			lastPos = endIdx;

			IList<TextRenderInfo> charRis = data.StartRi.GetCharacterRenderInfos();

			return getTals(word, charRis[startIdx], charRis[endIdx]);
		}
	}

	public class TextSegmentExtractor
	{
		private const float MAX_POS_X_DIF = 0.5f;
		private const float MAX_NEG_X_DIF = -0.5f;

		private TextRenderInfo ri;
		private TextRenderInfo riPrior;
		private TextRenderInfo riStart;

		public LocationTextExtractor4 lte4 { get; set; }

		private string text;
		private string result;
		
		private bool atBegining = true;
		private bool spaceStart = false;

		private float xDif;
		private float yDif;

		private int idx = 0;

		private string process;

		// public TextSegmentExtractor() { }

		// methods

		public void SetBeginText()
		{
			atBegining = true;

			if (ri != null)
			{
				ri.ReleaseGraphicsState();
				ri = null;
			}
		}

		public void SetEndText()
		{
			process = "lte-A";
			if (result != null) FinalizeText("A");
		}

		public void SetRenderText(TextRenderInfo data)
		{
			if (data.GetText().Length == 1 && 
				(data.GetText()[0] == ' '
				|| data.GetText()[0] == '/'
				|| data.GetText()[0] == ',')
				)
			{
				process = "lte-D";
				if (result!=null) FinalizeText("D");

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

			idx++;
			
			// if (idx > 31)
			// {
			// 	int b = 1;
			// }

			// Debug.WriteLine($">{text}<");

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
							FinalizeText("C");
						}
					}
				}

			} 

			result += text;
		}

		private string priorResult;

		public void FinalizeText(string id)
		{ 
			// if (result == null || result.Length == 0)
			// {
			// 	int a = 1;
			// 	return;
			// }
			//
			// Debug.WriteLine($"{id}  | >{result}<");

			priorResult = result;

			LocationTextExtractor4Data data = 
				new LocationTextExtractor4Data(result, riStart, riPrior ?? riStart);

			data.Process = process;

			// RaiseGotSegmentEvent(data);

			lte4.TseOnGotSegment(this, data);

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

	public interface ITextExtractorFilter
	{
		List<string> ToFindList { get; set; }

		int MinLength { get; set; }
		int MaxLength { get; set; }

		bool Test(string word);
	}


}