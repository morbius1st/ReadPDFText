#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using UtilityLibrary;

#endregion

// user name: jeffs
// created:   4/18/2024 7:34:37 PM

namespace ReadPDFTextTests
{

	public class TextExtractTest3
	{
		public const bool SHOW_DEBUG = false;

		private int row = -1;

		public TextExtractTest3(ITextExtractFilter3 filter)
		{
			this.filter = filter;
		}

		private ITextExtractFilter3 filter { get; set; }

		private TextExtractor3 te1 = new TextExtractor3();

		private List<List<string>> results;
		private List<LocationTextExtractor3Data> answers;

		public void Process(List<TextRenderInfo> ris)
		{
		// 	testList(list);
		//
		// }
		//
		// private void testList(List<TextRenderInfo> ris)
		// {
			int idx = 0;
			te1 = new TextExtractor3();
			te1.tx1 = this;

			results = new List<List<string>>();
			answers = new List<LocationTextExtractor3Data>();

			if (SHOW_DEBUG) Debug.Write($"{" ",-12}Start| ");

			foreach (TextRenderInfo ri in ris)
			{
				te1.SetRenderText(ri);

				// if (idx++ > 30) return;
			}

			te1.finalize();

			showResults();
			showAnswers();
		}

		private float yPrior = 0;

		public void OnGotTextSegment(LocationTextExtractor3Data data )
		{
			if (SHOW_DEBUG) Debug.WriteLine($"{$"{data.Text ?? "null"}",-20} | on got|  ({data.Text?.Length ?? 0})  ({(data.StartRi?.GetStartX ?? 0):F2}) >{data.Text ?? "null"}< ({(data.EndRi?.GetEndX ?? 0):F2})");

			if (SHOW_DEBUG) Debug.Write($"\n{" ",-12}Start| ");

			if (!FloatOps.EqualWithInTolerance(data.StartRi.GetStartY, yPrior, TextRenderInfo.TOL))
			{
				results.Add(new List<string>());
				row = results.Count - 1;
			}

			results[row].Add(data.Text);

			yPrior = data.StartRi.GetStartY;

			if (filter.Match(data.Text))
			{
				answers.Add(data);
			}

		}

		private void showResults()
		{
			row = 0;

			Debug.WriteLine("*** show results ***\n");

			foreach (List<string> listA in results)
			{
				Debug.Write($"row| {row++}");

				foreach (string s in listA)
				{
					Debug.Write($" >{s}<");
				}

				Debug.Write("\n");

			}
		}

		private void showAnswers()
		{
			int idx = 0;

			Debug.WriteLine("\n*** show answers ***\n");

			foreach (LocationTextExtractor3Data data in answers)
			{
				Debug.WriteLine($"answer {idx++} | {data.Text}");
			}
		}

		public override string ToString()
		{
			return $"this is {nameof(TextExtractTest3)}";
		}
	}

	public class TextExtractor3
	{
		private const float TOL = TextRenderInfo.TOL;

		private const bool SHOW_DEBUG = TextExtractTest3.SHOW_DEBUG;

		private TextRenderInfo riCurr;
		private TextRenderInfo riPrior;
		private TextRenderInfo riStart = null;

		public TextExtractTest3 tx1 { get; set; }

		private string text;
		private string result;

		private int idx;

		public void SetRenderText(TextRenderInfo data)
		{
			IList<TextRenderInfo> ris = data.GetCharacterRenderInfos();

			for (idx = 0; idx < ris.Count; idx++)
			{
				process(ris[idx]);
			}
		}

		private void process(TextRenderInfo ri)
		{
			text = ri.GetText();

			if (SHOW_DEBUG) Debug.Write($" | ri ({ri.GetStartX:F2}) >{text}< ({ri.GetEndX:F2})");

			if (riPrior != null)
			{
				if (SHOW_DEBUG) Debug.Write($" | prior ({riPrior.GetStartX:F2}) >{riPrior.GetText()}< ({riPrior.GetEndX:F2}) ");
			}
			else
			{
				if (SHOW_DEBUG) Debug.Write(" | prior is null");
			}

			if (@" ,\/".IndexOf(text) != -1)
			{
				// space at the begining of a sequence
				if (riStart == null) return;

				if (SHOW_DEBUG) Debug.WriteLine($"\n{" ",-12}got EOL");

				// found the end of a "word"
				finalize();
				riPrior = ri;
				riPrior = null;
				text = null;
				return;
			}

			if (riPrior != null &&
				(!FloatOps.EqualWithInTolerance(ri.GetStartX, riPrior.GetEndX, TOL) ||
				!FloatOps.EqualWithInTolerance(ri.GetStartY, riPrior.GetEndY, TOL)))
			{
				if (SHOW_DEBUG) Debug.WriteLine($"\n{" ",-12}got EOW"); // X| (ri {ri.GetStartX:F2}  vs prior {riPrior.GetEndX:F2})\n"); // dif {(riPrior.GetEndX - ri.GetStartX):F2})\n");

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
			if (result !=null && result.Length >0)
			{
				LocationTextExtractor3Data data = new LocationTextExtractor3Data(result, riStart, riPrior ?? riStart);

				tx1.OnGotTextSegment(data);
			}
			else
			{
				if (SHOW_DEBUG) Debug.Write($"\n{" ",-12}Start| ");
			}

			riStart = null;
			riCurr = null;
			result = null;
		}

	}

	public class TextExtractorFilter3 : ITextExtractFilter3
	{
		public int MinLength { get; set; }
		public int MaxLength { get; set; }

		public Regex FilterRegex { get; set; }
		public string FilterString { get; set; }

		public TextExtractorFilter3(Regex filterRegex)
		{
			FilterRegex = filterRegex;
		}

		public TextExtractorFilter3(string filterString)
		{
			FilterString = filterString;
			FilterRegex = new Regex(filterString);
		}


		public bool Match(string toMatch)
		{
			MatchCollection m = FilterRegex.Matches(toMatch);

			if (m.Count == 0 || m.Count > 1) return false;
			
			return true;
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
			set
			{
				process += " | " + value;
			}
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
		bool Match(string toMatch);

		int MinLength { get; set; }
		int MaxLength { get; set; }
	}
	
}
