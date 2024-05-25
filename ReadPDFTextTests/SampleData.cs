#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using UtilityLibrary;

using static UtilityLibrary.CsExtensions;

#endregion

// user name: jeffs
// created:   4/18/2024 7:33:59 PM

namespace ReadPDFTextTests
{
	public struct Ri
	{
		public float X { get; set; }
		public float Y { get; set; }
		public string Text;

		public Ri(string text, float x, float y)
		{
			Text = text;
			X = x;
			Y = y;
		}
	}

	public class TextRenderInfo
	{
		public const float SEP = 0.05f;
		public const float ADJ = 0.01f;
		public const float TOL = ADJ*2;

		public const float MAX_SEP = SEP + ADJ;
		public const float MIN_SEP = SEP - ADJ;

		private string? Text;
		public string GetText() => Text;

		private IList<TextRenderInfo> RenderInfos { get; set; }
		public IList<TextRenderInfo> GetCharacterRenderInfos() => RenderInfos;

		private LineSegment? DescentLine { get; set; } = null!;
		public LineSegment GetDescentLine() => DescentLine;

		private bool preserveGraphicsState { get; set; } = false;
		public void PreserveGraphicsState() { preserveGraphicsState = true; }

		public TextRenderInfo(List<TextRenderInfo>? ris)
		{
			Text = null;
			RenderInfos = new List<TextRenderInfo>();

			if (ris == null || ris.Count == 0) { return; }

			if (ris.Count == 1)
			{
				addRenderInfo(ris[0]);
			}
			else
			{
				addRenderInfo(ris);
			}

		}

		private void addRenderInfo(TextRenderInfo ri)
		{
			Text = ri.GetText();
			DescentLine = ri.GetDescentLine();
			RenderInfos.Add(ri);
		}

		private void addRenderInfo(List<TextRenderInfo>? ris)
		{
			StringBuilder sb = new StringBuilder();

			foreach (TextRenderInfo ri in ris)
			{
				sb.Append(ri.GetText());

				RenderInfos.Add(ri);
			}

			Text = sb.ToString();

			Vector start = RenderInfos[0].DescentLine.GetStartPoint();
			Vector end = RenderInfos[ris.Count-1].DescentLine.GetEndPoint();

			DescentLine = new LineSegment(start.Get(0), start.Get(1), end.Get(0), end.Get(1));
		}

		public TextRenderInfo(char c, float x, float y, float xNext)
		{
			Text = c.ToString();
			RenderInfos = new List<TextRenderInfo>();

			addChildRenderInfo(c, x, y, xNext, y);

			DescentLine = new LineSegment(x, y, xNext, y);
		}

		public TextRenderInfo() {}

		private void addChildRenderInfo(char c, float xStart, float yStart, float xEnd, float yEnd)
		{
			TextRenderInfo tri = new TextRenderInfo();
			tri.Text = c.ToString();
			tri.DescentLine = new LineSegment(xStart, yStart, xEnd, yEnd);
			
			RenderInfos.Add(tri);
		}


		public float GetStartX => DescentLine.StartEnd[0].Get(0);
		public float GetStartY => DescentLine.StartEnd[0].Get(1);

		public float GetEndX => DescentLine.StartEnd[1].Get(0);
		public float GetEndY => DescentLine.StartEnd[1].Get(1);


	}

	public class LineSegment
	{
		public Vector[] StartEnd { get; set; }

		public LineSegment(float xStart, float yStart, float xEnd, float yEnd)
		{
			StartEnd = new []
			{
				new Vector(xStart, yStart),
				new Vector(xEnd, yEnd)

			};
		}

		public Vector GetStartPoint() => StartEnd[0];
		public Vector GetEndPoint() => StartEnd[1];
	}

	public class Vector
	{
		public float[] XY { get; set; }

		public float Get(int index) => XY[index];

		public Vector(float x, float y)
		{
			XY = new [] { x, y };
		}
	}


	public class SampleData
	{
		private const int MAX = 5;
		private const float SEP = TextRenderInfo.SEP;
		private const float ADJ = TextRenderInfo.ADJ;
		private const float TOL = TextRenderInfo.TOL;

		private const float MAX_SEP = TextRenderInfo.MAX_SEP;
		private const float MIN_SEP = TextRenderInfo.MIN_SEP;

		private int adj = 1;

		private string [,] firstLast = new string[,]
		{
			{null, null },
			{" ", null },
			{null, " " },
			{" ", " " },
			{"       ", null }
			
		};


		/*samples
		 */

		// sample2[] is same Y
		// sample2[][] same Y and X > character gap
		private string[][] sample2 = new string[][]
		{
			new []{"Lorem ipsum dolor","sit amet,", "consectetur adipiscing elit."},
			new []{" Sed do eiusmod   ", "   "},
			new []{"  tempor"},
			new []{"incididunt ut labore "},
			new []{"enim ad \\ minim", "veniam, quis nostrud"},
			new []{"  a", "a  ", "  be", "be  ", "  can", "can  "},
			new []{"A1.0-1", "  A1.0-2", "A1.0-3  ", "  A1.0-4  "},
			new []{"text A1.0-5 embedded", "text  A1.0-5 embedded", "text A1.0-5  embedded"}
		};

		/* sample data
		// primary cases
		// A - character flow  (process stream in 1 to 2, 3, 4) sized bits
		// B - word flow (process stream in 1+ words)

		// note, ignore begin_text and end_text
		*/

		public List<TextRenderInfo> BaseRenderInfo {get; set; }

		// character list
		public List<TextRenderInfo> Cl { get; set; }

		// sentence list
		public List<TextRenderInfo> Sl { get; set; }

		// word List
		public List<TextRenderInfo> Wl { get; set; }


		public SampleData()
		{
			config();
		}


		private void config()
		{
			assign();
			
			// show();
		}

		private void assign()
		{
			assignBaseRenderInfo();
			assignCharList();
			assignSentList();
			assignWordList2();

			// assignWordList();
		}

		public void show()
		{
			showSample();
			showBaseRenderInfo();
			showCharList(false);
			showSentList(false);
			showWordList();
		}
		
		private void assignBaseRenderInfo()
		{
			bool showDebug = false;

			BaseRenderInfo = new List<TextRenderInfo>();

			TextRenderInfo ri;

			int i = 0;
			int j = 0;

			float y = 100;
			float x = 50;
			float xStart = 50;
			float xNext = 50;

			float[] adjs = new [] {0, -0.01f, 0.01f, 0.01f, -0.01f };

			int idx = -1;

			if (showDebug) Debug.WriteLine("each on a different line (Y is different)");

			foreach (string[] strings in sample2)
			{
				if (showDebug) Debug.WriteLine("everything on the same line (Y is the same)");
				if (showDebug) Debug.Write($"\tline {i++}| y {y}\n");

				x = 50;
				xStart = 50;
				xNext = 50;

				foreach (string s in strings)
				{
					if (showDebug) Debug.Write($"\t\titem {j++}| ");

					char[] c = s.ToCharArray();

					foreach (char c1 in c)
					{
						idx++;

						if (showDebug) Debug.Write($">{c1.ToString()}< ({x:F2}) ");

						x = xStart + idx * SEP + adjs[idx % 5];
						xNext = x + SEP;

						ri=new TextRenderInfo(c1, x, y, xNext);
						BaseRenderInfo.Add(ri);

					}

					xStart += SEP * 10;

					if (showDebug) Debug.Write("\n");
				}

				idx = -1;

				if (showDebug) Debug.Write("\n");

				j = 0;

				y -= 10;
			}
		}

		private void assignCharList()
		{
			TextRenderInfo rx;

			int idx = 0;
			float xPrior = BaseRenderInfo[0].GetStartX;
			float yPrior = BaseRenderInfo[0].GetStartY;

			bool addOne = false;

			List<TextRenderInfo> ris = new List<TextRenderInfo>();

			Cl = new List<TextRenderInfo>();

			foreach (TextRenderInfo ri in BaseRenderInfo)
			{
				// string s = ri.GetText();
				// float x = ri.GetStartX;
				// float y = ri.GetStartY;


				if (FloatOps.Equals(yPrior, ri.GetStartY) &&
						FloatOps.EqualWithInTolerance(xPrior, ri.GetStartX, TOL)
					)
				{
					idx++;

					ris.Add(ri);

					if (idx % 13 == 0) 
					{
						addOne = true;

						xPrior = ri.GetEndX;
						yPrior = ri.GetEndY;

						continue;
					}
					
					if (ris.Count< 3 && (idx % 7 == 0 || addOne))
					{
						addOne = false;

						xPrior = ri.GetEndX;
						yPrior = ri.GetEndY;

						continue;
					}
				}
				else
				{
					if (ris.Count > 0)
					{
						rx = new TextRenderInfo(ris);

						Cl.Add(rx);

						ris=new List<TextRenderInfo>();
					}

					ris.Add(ri);
				}

				
				addOne = false;

				xPrior = ri.GetEndX;
				yPrior = ri.GetEndY;

				if (ris.Count == 4) Debug.WriteLine($"got 4| idx ({idx})");

				rx = new TextRenderInfo(ris);

				Cl.Add(rx);

				ris=new List<TextRenderInfo>();

				if (idx > 80) idx = 0;
			}
		}

		private void showCharList(bool showRis)
		{
			float xStart = 0;
			float yStart = 0;

			float xEnd = 0;
			float yEnd = 0;

			float xPrior = 0;
			float yPrior = 0;

			int i = 0; // lines
			int j = 0; // sentences / words

			Debug.WriteLine("\n*** Character List ***");

			foreach (TextRenderInfo ri in Cl)
			{
				xStart = ri.GetStartX;
				yStart = ri.GetStartY;

				xEnd = ri.GetEndX;
				yEnd = ri.GetEndY;

				if (!FloatOps.Equals(yPrior, yStart))
				{
					Debug.Write($"\nline {i++} | y {yStart}");
					j = 0;
				}

				if (!FloatOps.EqualWithInTolerance(xPrior, xStart, TOL))
				{
					Debug.Write($"\n\titem {j++} |");
				}

				xPrior = xEnd;
				yPrior = yEnd;

				string t1 = $"({xStart:F2}) >{ri.GetText()}< ({xEnd:F2})";

				if (showRis)
				{
					Debug.Write($"\n\t\t");
				} 

				Debug.Write($"{t1,-25}");

				if (showRis)
				{
					IList<TextRenderInfo> ris = ri.GetCharacterRenderInfos();

					Debug.Write($"{$"({ris.Count})",-5}");

					foreach (TextRenderInfo cri in ris)
					{
						string xs = $"{cri.GetStartX:F2}";
						string xe = $"{cri.GetEndX:F2}";
						Debug.Write($"({xs}) >{cri.GetText()}< ({xe})  ");
					}
				}
			}

			Debug.Write("\n");
		}


		private void assignSentList()
		{
			TextRenderInfo rx;

			int idx = 0;
			float xPrior = BaseRenderInfo[0].GetStartX;
			float yPrior = BaseRenderInfo[0].GetStartY;

			List<TextRenderInfo> ris = new List<TextRenderInfo>();

			Sl = new List<TextRenderInfo>();

			foreach (TextRenderInfo ri in BaseRenderInfo)
			{
				if (FloatOps.Equals(yPrior, ri.GetStartY) &&
					FloatOps.EqualWithInTolerance(xPrior, ri.GetStartX, TOL)
					)
				{
					ris.Add(ri);

					xPrior = ri.GetEndX;
					yPrior = ri.GetEndY;

					continue;
				}

				if (ris.Count > 0)
				{
					rx = new TextRenderInfo(ris);

					Sl.Add(rx);

					ris = new List<TextRenderInfo>();
				}

				ris.Add(ri);

				xPrior = ri.GetEndX;
				yPrior = ri.GetEndY;
			}

			if (ris.Count > 0)
			{
				rx = new TextRenderInfo(ris);

				Sl.Add(rx);
			}
		}

		private void showSentList(bool showRis)
		{
			float xStart = 0;
			float yStart = 0;

			float xEnd = 0;
			float yEnd = 0;

			float xPrior = 0;
			float yPrior = 0;

			int i = 0; // lines
			int j = 0; // sentences / words

			Debug.WriteLine("\n*** Sentence List ***");

			foreach (TextRenderInfo ri in Sl)
			{
				xStart = ri.GetStartX;
				yStart = ri.GetStartY;

				xEnd = ri.GetEndX;
				yEnd = ri.GetEndY;

				if (!FloatOps.Equals(yPrior, yStart))
				{
					Debug.Write($"\nline {i++} | y {yStart}");
					j = 0;
				}

				if (!FloatOps.EqualWithInTolerance(xPrior, xStart, TOL))
				{
					Debug.Write($"\n\titem {j++} |");
				}

				xPrior = xEnd;
				yPrior = yEnd;

				Debug.Write($" ({xStart:F2}) >{ri.GetText()}< ({xEnd:F2})");

				if (showRis)
				{
					IList<TextRenderInfo> ris = ri.GetCharacterRenderInfos();

					Debug.Write($"\n{" ",-15}{$"({ris.Count})",-4}");

					foreach (TextRenderInfo cri in ris)
					{
						string xs = $"{cri.GetStartX:F2}";
						string xe = $"{cri.GetEndX:F2}";
						Debug.Write($"  ({xs}) >{cri.GetText()}< ({xe})");
					}
				}

			}

			Debug.Write("\n");
		}


		private void assignWordList()
		{
			TextRenderInfo rx;
			int idx = 1;
			float xPrior = BaseRenderInfo[0].GetStartX;
			float yPrior = BaseRenderInfo[0].GetStartY;

			List<TextRenderInfo> ris = new List<TextRenderInfo>();

			Wl = new List<TextRenderInfo>();

			foreach (TextRenderInfo ri in BaseRenderInfo)
			{
				if (FloatOps.Equals(yPrior, ri.GetStartY) &&
					FloatOps.EqualWithInTolerance(xPrior, ri.GetStartX, TOL))
				{
					if (ri.GetText().Equals(" ") && idx++ % 6 != 0)
					{
						if (ris.Count > 0)
						{
							rx = new TextRenderInfo(ris);
					
							Wl.Add(rx);
					
							ris = new List<TextRenderInfo>();
						}
					
						xPrior = ri.GetEndX;
						yPrior = ri.GetEndY;
					
						continue;
					}

					ris.Add(ri);

					xPrior = ri.GetEndX;
					yPrior = ri.GetEndY;

					continue;
				}


				if (ris.Count > 0)
				{
					rx = new TextRenderInfo(ris);

					Wl.Add(rx);

					ris = new List<TextRenderInfo>();
				}


				if (ri.GetText().Equals(" ") && idx++ % 3 != 0)
				{
					if (ris.Count > 0)
					{
						rx = new TextRenderInfo(ris);
					
						Wl.Add(rx);
					
						ris = new List<TextRenderInfo>();
					}
					
					xPrior = ri.GetEndX;
					yPrior = ri.GetEndY;
					
					continue;
				}

				ris.Add(ri);

				xPrior = ri.GetEndX;
				yPrior = ri.GetEndY;
			}

			if (ris.Count > 0)
			{
				rx = new TextRenderInfo(ris);

				Wl.Add(rx);
			}
		}

		
		private void assignWordList2()
		{
			TextRenderInfo rx;
			int idx = 1;
			float xPrior = BaseRenderInfo[0].GetStartX;
			float yPrior = BaseRenderInfo[0].GetStartY;

			string subStr;

			TextRenderInfo ri;

			List<TextRenderInfo> ris = new List<TextRenderInfo>();

			Wl = new List<TextRenderInfo>();


			for (var i = 0; i < BaseRenderInfo.Count; i++)
			{
				ri = BaseRenderInfo[i];

				subStr = getSubString(i);

				if (FloatOps.Equals(yPrior, ri.GetStartY) &&
					FloatOps.EqualWithInTolerance(xPrior, ri.GetStartX, TOL))
				{
					if (ri.GetText().Equals(" "))
					{
						if (i + 2 < BaseRenderInfo.Count)
						{
							TextRenderInfo bplus1 = BaseRenderInfo[i + 1];
							TextRenderInfo bplus2 = BaseRenderInfo[i + 2];

							string s = ">"+subStr+"<";

							bool b1 = bplus1.GetText().Equals(" ");
							bool b2 = !bplus2.GetText().Equals(" ");
							bool b3 = FloatOps.Equals(ri.GetEndY, bplus1.GetStartY);
							bool b4 = FloatOps.Equals(bplus1.GetEndY, bplus2.GetStartY);


							if (bplus1.GetText().Equals(" ") &&
								!bplus2.GetText().Equals(" ") &&
								FloatOps.Equals(ri.GetEndY, bplus1.GetStartY) &&
								FloatOps.Equals(bplus1.GetEndY, bplus2.GetStartY)
								)
							{
								ris.Add(bplus1);
								i++;

								xPrior = bplus1.GetEndX;
								yPrior = bplus1.GetEndY;
								continue;
							}

							TextRenderInfo bminus1 = BaseRenderInfo[i - 1];

							bool b5 = bminus1.GetText().Equals(" ");

							if (!bminus1.GetText().Equals(" ") &&
								bplus1.GetText().Equals(" ") &&
								FloatOps.Equals(bminus1.GetEndY, ri.GetStartY) &&
								FloatOps.Equals(ri.GetEndY, bplus1.GetStartY)
								)
							{
								ris.Add(ri);
								i++;

								xPrior = bplus1.GetEndX;
								yPrior = bplus1.GetEndY;
								continue;
							}

						}


						if (ris.Count > 0)
						{
							rx = new TextRenderInfo(ris);
					
							Wl.Add(rx);
					
							ris = new List<TextRenderInfo>();
						}
					
						xPrior = ri.GetEndX;
						yPrior = ri.GetEndY;
					
						continue;

					}

					ris.Add(ri);

					xPrior = ri.GetEndX;
					yPrior = ri.GetEndY;

					continue;
				}


				if (ris.Count > 0)
				{
					rx = new TextRenderInfo(ris);

					Wl.Add(rx);

					ris = new List<TextRenderInfo>();
				}


				if (ri.GetText().Equals(" "))
				{

					if (i + 2 < BaseRenderInfo.Count)
					{
						TextRenderInfo bplus1 = BaseRenderInfo[i + 1];
						TextRenderInfo bplus2 = BaseRenderInfo[i + 2];

						string s = ">"+subStr+"<";

						bool b1 = bplus1.GetText().Equals(" ");
						bool b2 = !bplus2.GetText().Equals(" ");
						bool b3 = FloatOps.Equals(ri.GetEndY, bplus1.GetStartY);
						bool b4 = FloatOps.Equals(bplus1.GetEndY, bplus2.GetStartY);


						if (bplus1.GetText().Equals(" ") &&
							!bplus2.GetText().Equals(" ") &&
							FloatOps.Equals(ri.GetEndY, bplus1.GetStartY) &&
							FloatOps.Equals(bplus1.GetEndY, bplus2.GetStartY)
							)
						{
							ris.Add(bplus1);
							i++;

							xPrior = bplus1.GetEndX;
							yPrior = bplus1.GetEndY;
							continue;
						}
					}

					if (ris.Count > 0)
					{
						rx = new TextRenderInfo(ris);
					
						Wl.Add(rx);
					
						ris = new List<TextRenderInfo>();
					}
					
					xPrior = ri.GetEndX;
					yPrior = ri.GetEndY;
					
					continue;
				}

				ris.Add(ri);

				xPrior = ri.GetEndX;
				yPrior = ri.GetEndY;
			}

			if (ris.Count > 0)
			{
				rx = new TextRenderInfo(ris);

				Wl.Add(rx);
			}
		}

		private string getSubString(int i)
		{
			int start = i - 3;
			start = start < 0 ? 0 : start;

			int end = start + 6;
			end = end > BaseRenderInfo.Count - 1 ? BaseRenderInfo.Count - 1 : end;

			StringBuilder sb = new StringBuilder();

			for (int j = start; j < end+1; j++)
			{
				sb.Append(BaseRenderInfo[j].GetText());
			}

			return sb.ToString();
		}

		private void showWordList()
		{
			float xStart = 0;
			float yStart = 0;

			float xEnd = 0;
			float yEnd = 0;

			float xPrior = 0;
			float yPrior = 0;

			int i = 0; // lines
			int j = 0; // sentences / words

			Debug.WriteLine("\n*** Word List ***");

			foreach (TextRenderInfo ri in Wl)
			{
				xStart = ri.GetStartX;
				yStart = ri.GetStartY;

				xEnd = ri.GetEndX;
				yEnd = ri.GetEndY;

				if (!FloatOps.Equals(yPrior, yStart))
				{
					Debug.Write($"\nline {i++} | y {yStart}");
					j = 0;
				}

				if (!FloatOps.EqualWithInTolerance(xPrior, xStart, TOL))
				{
					Debug.Write($"\n\titem {j++} |");
				}

				xPrior = xEnd;
				yPrior = yEnd;

				Debug.Write($" ({xStart:F2}) >{ri.GetText()}< ({xEnd:F2})    ");

			}

			Debug.Write("\n");
		}



		public void showSample()
		{
			int c = 0;
			int i = 0;
			int j = 0;

			Debug.WriteLine("\n*** Sample ***\n");

			foreach (string[] strings in sample2)
			{
				Debug.Write($"line {i++}|\n");

				foreach (string s in strings)
				{
					Debug.Write($"\titem {j++}| {$">{s}<",-40} |  pos {c,-3} to {c+s.Length-1}");
					Debug.Write("\n");

					c += s.Length;
				}

				Debug.Write("\n");

				j = 0;
			}
		}

		private void showBaseRenderInfo()
		{
			// float x = BaseRenderInfo[0].DescentLine.GetStartPoint().Get(0);
			// float y = BaseRenderInfo[0].DescentLine.GetStartPoint().Get(1);

			float xStart = 0;
			float yStart = 0;

			float xEnd = 0;
			float yEnd = 0;

			float xPrior = 0;
			float yPrior = 0;

			float xDif;

			int i = 0;
			int j = 0;

			Debug.WriteLine("\n*** base render info ***");


			foreach (TextRenderInfo ri in BaseRenderInfo)
			{
				xStart = ri.GetStartX;
				yStart = ri.GetStartY;

				xEnd = ri.GetEndX;
				yEnd = ri.GetEndY;

				if (!FloatOps.Equals(yPrior, yStart))
				{
					Debug.Write($"\nline {i++} | y {yStart}");
					j = 0;
				}

				if (!FloatOps.EqualWithInTolerance(xPrior, xStart, TOL))
				{
					Debug.Write($"\n\titem {j++} |");
				}

				xDif = xStart - xPrior;

				xPrior = xEnd;
				yPrior = yEnd;

				Debug.Write($"{$"({xDif:F2})",8}  ({xStart:F2}) >{ri.GetText()}< ({xEnd:F2})  ");


			}

			Debug.Write("\n");
		}

		public override string ToString()
		{
			return $"this is {nameof(SampleData)}";
		}

/*

		
		private string[] sample = new []
		{
			"Lorem ipsum dolor  sit amet, consectetur adipiscing elit.",
			" Sed do eiusmod   ",
			"  tempor",
			"incididunt ut labore ",
			"et dolore magna aliqua. Ut",
			"enim ad minim veniam, quis nostrud"
		};


		// character
		// [] array of individual samples
		// List<>[] list of sample sentence or word
		// List<List<>>[] characters with character spacing
		//
		// public List<List<string>>[] Cl { get; set; }
		//
		// public List<Ri>[] CriList { get; set; }
		//
		// // // word
		// // // [] array of individual samples
		// // // List<>[] list of sample words
		// // public List<string>[] Wl { get; set; }
		//
		// sentence
		// // [] array of individual samples
		// // List<>[] list of sentence or word
		// 
		// public List<string>[] Sl { get; set; }
		// public List<Ri>[] SriList { get; set; }
		// public List<Ri>[] WriList { get; set; }


		// 			assignClist();
		//
		// 			assignSlist();
		//
		// 			assignCris();
		//
		// 			assignSris();

		private void assignClist()
		{
			string first;
			string last;
			int j = 0;

			Cl = new List<List<string>>[MAX];

			for (int i = 0; i < MAX; i++)
			{
				Cl[i] = new List<List<string>>();
				j = 0;

				foreach (string s in sample)
				{
					if (j++ == 0)
					{
						first =  firstLast[i, 0];
						last =  firstLast[i, 1];
					}
					else
					{
						first = null;
						last = null;
					}

					char[] sc = s.ToCharArray();

					Cl[i].Add(charList(sc, first, last));
				}
			}
		}

		private void assignSlist()
		{
			string first;
			string last;
			int j = 0;

			Sl = new List<string>[MAX];

			for (int i = 0; i < MAX; i++)
			{
				Sl[i] = new List<string>();
				j = 0;

				foreach (string s in sample)
				{
					if (j++ == 0)
					{
						first =  firstLast[i, 0];
						last =  firstLast[i, 1];
					}
					else
					{
						first = null;
						last = null;
					}

					Sl[i].Add(words(s, first, last));
				}
			}
		}

		private void assignCris()
		{
			CriList = new List<Ri>[MAX];

			int i = 0;
			int j = 0;
			int k = 0;

			float X = 0.0f;
			float Y = 0.0f;

			int sidx = 0;


			foreach (List<List<string>> listA in Cl)
			{
				// process each sample set

				Y = 90f;

				CriList[j] = new List<Ri>();

				foreach (List<string> listB in listA)
				{
					// process each sentence - all get the same y but x+=0.05
					X = 10f;
					k = 0;

					foreach (string s in listB)
					{
						if (s.Equals(" "))
						{
							if (++sidx == 5)
							{
								X += 2f;
							}
						}


						Ri r = new Ri(s, X, Y);

						CriList[j].Add(r);

						X += SEP * s.Length;

					}

					Y -= 10f;
				}

				j++;
			}
		}

		private void assignSris()
		{
			SriList = new List<Ri>[MAX];

			int i = 0;
			int j = 0;

			float X = 10.0f;
			float Y = 0.0f;

			foreach (List<string> listA in Sl)
			{
				Y = 90f;

				SriList[j] = new List<Ri>();


				foreach (string s in listA)
				{
					Ri r = new Ri(s, X, Y);

					SriList[j].Add(r);

					// no need to increment X

					Y -= 10f;
				}

				j++;
			}
		}

		private new List<string> charList(char[] sc, string? first, string? last)
		{
			List<string> cl = new List<string>();

			string? s;
			int idx = 1;
			string prior2 = null;
			string prior = null;

			if (!string.IsNullOrEmpty(first)) cl.Add(first);

			foreach (char c in sc)
			{
				s = null;

				if (++idx % 11 == 0)
				{
					prior2 = c.ToString();
					continue;
				}

				if (prior2 != null)
				{
					prior = prior2 + c.ToString();
					prior2 = null;
				}

				if (idx % 4 == 0) {prior  = c.ToString(); continue;}

				if (prior != null)
				{
					s = prior;
					prior = null;
				}

				s += c.ToString();

				cl.Add(s);
			}

			if (!string.IsNullOrEmpty(last)) cl.Add(last);

			return cl;
		}


		private string words(string wordString, string? first, string? last)
		{
			string result = null;

			if (!string.IsNullOrEmpty(first)) result = first;

			result += wordString;

			if (!string.IsNullOrEmpty(last)) result +=  last;

			return result;
		}



		public void Show()
		{
			showCharacters();
			showSentences();

			ShowRiLists();
			showCriList();
		}

		private void showCharacters()
		{
			int i = 0;
			int j = 0;
			int k = 0;

			foreach (List<List<string>> listA in Cl)
			{
				Debug.WriteLine($"\ncharacter sample set| {i++}");
				j = 0;

				foreach (List<string> listB in listA)
				{
					Debug.Write($"sample string| {j++} | ");
					k = 0;

					foreach (string s in listB)
					{
						Debug.Write($"({k++} >{s}<)");
					}

					Debug.Write("\n");
				}
				Debug.Write("\n");
			}
		}

		private void showSentences()
		{
			int i = 0;
			int j = 0;

			foreach (List<string> listA in Sl)
			{
				Debug.WriteLine($"word sample set| {i++}");
				j = 0;

				foreach (string s in listA)
				{
					Debug.WriteLine($"sample string| ({j++}) | >{s}<");
				}
				Debug.Write("\n");
			}
		}


		public void ShowRiLists()
		{
			int i = 0;
			int j = 0;
			int k = 0;

			float Xprior = 0;
			float Yprior = 0;

			float sepDist;

			foreach (List<Ri> listA in CriList)
			{
				Debug.WriteLine($"\ncharacter sample set| {i++}");

				foreach (Ri ri in listA)
				{
					if (Yprior != ri.Y)
					{
						Debug.WriteLine("\nnew line");
					}

					sepDist = ri.X - Xprior;

					

					if (sepDist > SEP * ri.Text.Length)
					{
						Debug.WriteLine("\nnew sentence");
					}

					Xprior = ri.X;
					Yprior = ri.Y;

					Debug.Write($"x {ri.X:F3} | y {ri.Y:F3} | {ri.Text}");
				}
			}
		}

		private void showCriList()
		{
			int i = 0;
			int j = 0;

			float Xprior = 0;
			float Yprior = 0;

			foreach (List<Ri> listA in SriList)
			{
				Debug.WriteLine($"\nsentence sample set {i++}");

				foreach (Ri ri in listA)
				{
					if (Yprior != ri.Y)
					{
						Debug.WriteLine("new line");
					}

					if (ri.X - Xprior > SEP * ri.Text.Length)
					{
						Debug.WriteLine("new sentence");
					}

					Xprior = ri.X;
					Yprior = ri.Y;

					Debug.Write($"x {ri.X:F3} | y {ri.Y:F3} | {ri.Text}");
				}
			}
		}

		*/


	}
}
