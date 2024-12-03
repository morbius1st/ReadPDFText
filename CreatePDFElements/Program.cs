using CreatePDFElements.Support;
using DebugCode;
using ShTempCode.DebugCode;
using UtilityLibrary;

namespace CreatePDFElements
{
	internal class Program
	{
		public static string[] Folders = new []
		{
			@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\TestBoxes\",
			@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test8\PDF Files\"
		};

		public const string SHEET_METRIC_FOLDER = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\";
		private static string DataFilePath { get; } = SHEET_METRIC_FOLDER + "SheetData7.xml";

		private static Program me;

		private static ProcessManager pm;

		private static InvestigateManager im = new InvestigateManager();

		static void Main(string[] args)
		{
			DM.init(5);
			DM.DbxSetIdx(0, 0);
			DM.DbxLineEx(0, "start", 1);
			DM.DbxSetIdx(1, 0);

			me = new Program();
			pm = new ProcessManager();
			im = new InvestigateManager();

			bool repeat;

			// me.proceed();
			do
			{
				repeat = me.SwitchboardMain();
			}
			while (repeat);

			DM.DbxLineEx(0, "end", 0, -1);

			// Console.Write("\nWaiting| ");
			// Console.ReadKey();
		}

		// selection
		private bool SwitchboardMain()
		{
			DM.DbxLineEx(0, "start", 1);

			bool repeat = true;

			string o = me.switchboard(0);

			Console.Write("\n");

			switch (o)
			{
			case "A1":
				{
					Console.WriteLine("add annotations 1\n");
					proceed();
					break;
				}

			case "A2":
				{
					Console.WriteLine("add annotations 1\n");
					add2();
					// proceed();
					break;
				}

			
			case "Q1":
				{
					Console.WriteLine("query 1\n");
					query1();
					break;
				}


			case "Q2":
				{
					Console.WriteLine("query 1\n");
					query2();
					break;
				}

			case "L":
				{
					Console.WriteLine("listing - not implemented\n");
					break;
				}

			case "X":
				{
					Console.WriteLine("Exiting\n");
					repeat = false;
					break;
				}
			}

			DM.DbxLineEx(0, "end", 0, -1);

			return repeat;
		}

		// setup

		// operations

		private void listing()
		{
			DM.DbxLineEx(0, "start", 1);

			bool repeat = true;

			do
			{
				Console.WriteLine("\n*** Select an option ***\n");
				string o = switchboard(1);

				if (o.Equals("S"))
				{
					// do short listing}

					repeat = false;
				} 
				else if (o.Equals("L"))
				{
					// do short listing}

					repeat = false;
				}
				else if (o.Equals("X"))
				{
					repeat = false;
				}
				else
				{
					Console.WriteLine($"\n{"*".Repeat(30)}\n   invalid option\n{"*".Repeat(30)}");
				}
			}
			while (repeat);


			DM.DbxLineEx(0, "end", 0, 1);
		}

		private void query1()
		{
			DM.DbxLineEx(0, "start", 1);

			bool repeat = true;

			do
			{
				Console.WriteLine("\n*** Select an option ***\n");
				string o = switchboard(1);

				if (o.Equals("S"))
				{
					// do short listing}
					me.investigate1(o);
					repeat = false;
				} 
				else if (o.Equals("L"))
				{
					// do short listing}
					me.investigate1(o);
					repeat = false;
				}
				else if (o.Equals("X"))
				{
					repeat = false;
				}
				else
				{
					Console.WriteLine($"\n{"*".Repeat(30)}\n   invalid option\n{"*".Repeat(30)}");
				}
			}
			while (repeat);


			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void query2()
		{
			DM.DbxLineEx(0, "start", 1);

			bool repeat = true;

			do
			{
				Console.WriteLine("\n*** Select an option ***\n");
				string o = switchboard(1);

				if (o.Equals("S"))
				{
					// do short listing}
					me.investigate(o);
					repeat = false;
				} 
				else if (o.Equals("L"))
				{
					// do short listing}
					me.investigate(o);
					repeat = false;
				}
				else if (o.Equals("X"))
				{
					repeat = false;
				}
				else
				{
					Console.WriteLine($"\n{"*".Repeat(30)}\n   invalid option\n{"*".Repeat(30)}");
				}
			}
			while (repeat);


			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void proceed()
		{
			DM.DbxLineEx(0, "start", 1);

			pm.AddToNewPdfs(DataFilePath);

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void add2()
		{
			pm.AddToExistPdf();
		}

		private void investigate(string idx)
		{
			DM.DbxLineEx(0, "start", 1);

			im.Process(idx);

			DM.DbxLineEx(0, "end", 0, -1);
		}

		
		private void investigate1(string idx)
		{
			DM.DbxLineEx(0, "start", 1);

			im.Process(im.GetSheets(1)[1], idx);

			DM.DbxLineEx(0, "end", 0, -1);
		}


		private List<char> need2ndKey;

		private string switchboard(int which)
		{
			DM.DbxLineEx(0, "start", 1);

			string result = null;

			need2ndKey = new List<char>();

			foreach (KeyValuePair<string, Tuple<string, bool, int>> kvp in sbItems[which])
			{
				add2ndKey(kvp.Key);

				if (kvp.Key[0] == '>')
				{
					Console.WriteLine($"\n{" ".Repeat(4)} +{"-".Repeat(20)}");
					continue;
				}
				Console.WriteLine($"{kvp.Key,-4} | {kvp.Value.Item1}");
			}

			Console.WriteLine($"\n{" ".Repeat(4)} +{"-".Repeat(20)}");
			Console.WriteLine($"{"X",-4} | Exit");
			Console.Write("\n > ");

			string c = Console.ReadKey(false).KeyChar.ToString().ToUpper();

			if (has2ndKey(c))
			{
				c += Console.ReadKey(false).KeyChar.ToString().ToUpper();
			}
			Console.Write("\n");

			DM.DbxLineEx(0, $"selection {c}");

			DM.DbxLineEx(0, "end", 0, -1);

			return c;
		}

		private void add2ndKey(string s)
		{
			if (s.Length == 1) return;

			if (!need2ndKey.Contains(s[0])) need2ndKey.Add(s[0]);
		}

		private bool has2ndKey(string s)
		{
			return need2ndKey.Contains(s[0]);
		}

		private Dictionary<string, Tuple<string, bool, int>>[] sbItems = new []
		{
			new Dictionary<string, Tuple<string, bool, int>>()
			{
				{ ">01"  , new Tuple<string, bool, int>("Add Annotations", true, 1) },
				{ "A1" , new Tuple<string, bool, int>("Add to New", true, 1) },
				{ "A2" , new Tuple<string, bool, int>("Add to Exist", true, 1) },
				{ ">02"  , new Tuple<string, bool, int>("Query", true, 1) },
				{ "Q1" , new Tuple<string, bool, int>("Query one sheet"   , true, 1) },
				{ "Q2" , new Tuple<string, bool, int>("Query Sheets"   , true, 1) },
				{ ">03"  , new Tuple<string, bool, int>("Query", true, 1) },
				{ "L"  ,  new Tuple<string, bool, int>("List items"     , false, 1) },
			},
			new Dictionary<string, Tuple<string, bool, int>>()
			{
				{ "S", new Tuple<string, bool, int>("short"  , true, 1) },
				{ "L", new Tuple<string, bool, int>("long"   , true, 1) },
			},
		};
	}
}