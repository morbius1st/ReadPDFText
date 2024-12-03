#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;

#endregion

// user name: jeffs
// created:   6/25/2024 11:05:50 PM

namespace ShItextCode.ElementExtraction
{
	public class ScanStatus
	{
		static ScanStatus()
		{
			DuplicateRects = new List<Tuple<string, string, Rectangle>>();
			ExtraRects = new List<Tuple<string, string, Rectangle>>();
			Errors = new List<Tuple<string, string, ScanErrorLevel>>();

			HasFatalErrors = false;
		}

		public static bool HasFatalErrors { get; set; }

		private static List<Tuple<string, string, Rectangle>> DuplicateRects { get; set; }
		private static List<Tuple<string, string, Rectangle>> ExtraRects { get; set; }
		private static List<Tuple<string, string, ScanErrorLevel>> Errors { get; set; }

		public static int DupsCount => DuplicateRects.Count;
		public static int XtraCount => ExtraRects.Count;
		public static int ErrCount => Errors.Count;

		public static void AddError(string title, string description, ScanErrorLevel errorLevel)
		{
			Errors.Add(new Tuple<string, string, ScanErrorLevel>(title, description, errorLevel));
		}

		public static void AddExtra(string title, string description, Rectangle rect)
		{
			ExtraRects.Add(new Tuple<string, string, Rectangle>(title, description, rect));
		}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     

		public static void AddDup(string title, string description, Rectangle rect)
		{
			DuplicateRects.Add(new Tuple<string, string, Rectangle>(title, description, rect));
		}


		public static void ShowScanErrorReport()
		{
			Console.Write("\nPDF Sheet Box Scan Error Report");

			int dups = DuplicateRects.Count;
			int xtra = ExtraRects.Count;
			int errs = Errors.Count;

			if (dups > 0 || xtra > 0 || errs > 0)
			{
				Console.WriteLine($" | issues | Errors {errs}, Dups {dups}, Extras {xtra}");
			}
			else
			{
				Console.WriteLine($" | no issues | all boxes found");
				return;
			}

			if (errs > 0)
			{
				showErrors();
			}

			if (dups > 0)
			{
				showDuplicates();
			}

			if (xtra > 0)
			{
				showExtras();
			}

		}

		private static void showDuplicates()
		{
			Console.Write("\n");

			if (DuplicateRects.Count == 0)
			{
				Console.WriteLine($"no duplicate boxes found | {DuplicateRects.Count}");
				return;
			}

			Console.WriteLine($"duplicate boxes found | {DuplicateRects.Count}");

			foreach (Tuple<string, string, Rectangle> dups in DuplicateRects)
			{
				Console.WriteLine($"\tfile {dups.Item1,-20} | name {dups.Item2,-20} | location {dups.Item3.GetX():F2}, {dups.Item3.GetY():F2}");
			}

			// Console.WriteLine("\nplease eliminate the duplicate boxes and try again\n");
		}

		private static void showExtras()
		{
			Console.Write("\n");

			if (ExtraRects.Count == 0)
			{
				Console.WriteLine($"no extra boxes found | {ExtraRects.Count}");
				return;
			}

			Console.WriteLine($"extra boxes found | {ExtraRects.Count}");

			foreach (Tuple<string, string, Rectangle> xtra in ExtraRects)
			{
				Console.WriteLine($"\tfile {xtra.Item1,-20} | name {xtra.Item2,-20} | location {xtra.Item3.GetX():F2}, {xtra.Item3.GetY():F2}");
			}

			// Console.WriteLine("\nplease eliminate the extra boxes and try again\n");
		}

		private static void showErrors()
		{
			Console.Write("\n");

			if (Errors.Count == 0)
			{
				Console.WriteLine("All good - no error encountered");
				return;
			}

			if ( Errors.Count == 1)
			{
				Console.WriteLine("An error was encountered|");
			}
			else
			{
				Console.WriteLine("Some errors were encountered|");
			}

			foreach (Tuple<string, string, ScanErrorLevel> fail in Errors)
			{
				Console.WriteLine($"file| {fail.Item1} | error level {fail.Item3} | issue| {fail.Item2}");
			}
		}

		public override string ToString()
		{
			return $"$status fatal errs {HasFatalErrors} | errs {Errors?.Count.ToString() ?? "none"} | dups {DuplicateRects?.Count.ToString() ?? "none"} | xtra {ExtraRects?.Count.ToString() ?? "none"}";
		}
	}
}
