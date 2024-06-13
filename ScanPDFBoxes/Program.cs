using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using ScanPDFBoxes.Process;
using Settings;
using ShCode.ShDebugInfo;
using ShCommonCode.ShSheetData;
using UtilityLibrary;

namespace ScanPDFBoxes
{
	internal class Program
	{
		private static bool? lastOp = null;

		public const string SHEET_METRIC_FOLDER = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\";
		private static string DataFilePath { get; } = SHEET_METRIC_FOLDER + SheetDataSet.DataFileName;

		private string rootPath = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\TestBoxes\";

		// order matters
		private string[] filesNames = new []
		{
			"TestBoxes.pdf",                               // 0
			"A0.1.0 - COVER SHEET.pdf",                    // 1
			"A2.2-G - LEVEL G - OVERALL FLOOR PLAN.pdf",   // 2
			"TestBoxes 2.pdf",                             // 3
			"A0.1.0 - COVER SHEET b.pdf",                  // 4
			"A2.2-G - LEVEL G - OVERALL FLOOR PLAN b.pdf", // 5
		};

		private ShSamples ss;

		private static ProcessManager pm;

		private string[] files;


		private static Program p;

		static void Main(string[] args)
		{
			p = new Program();
			pm = new ProcessManager(new FilePath<FileNameSimple>(DataFilePath));
			pm.ConfigSheetData();

			p.ss = new ShSamples();

			bool result;

			if (lastOp.HasValue)
			{
				Console.Write($"Last operation | {(lastOp.Value ? "worked" : "failed" )}");
			}

			do
			{
				// Program.DS("@1");

				result = p.switchBoardMain();

				// Program.DS("@2");
			}
			while (result);
		}

		private bool switchBoardMain()
		{
			bool repeat;
			bool result = true;

			Console.Write("\n");
			ShowSheetRectInfo.showStatus(ShowWhere.DBG_CONS);
			Console.Write("\n");
			Console.WriteLine("> A  | *** Add Files (add 4 sheets) ***");
			Console.WriteLine("> B  | *** Add Files (add existing files) ***");
			Console.WriteLine("> C  | *** Add Files (add bad files) ***");
			Console.WriteLine("> D  | *** Add Files (add three good file) ***");
			Console.WriteLine("> E  | *** Add Files (add file removed ***");
			Console.WriteLine("> F  | *** Add (3) Special Files ***");
			Console.WriteLine("> R1 | *** Remove Sheet (fail - no match) ***");
			Console.WriteLine("> R2 | *** Remove Sheet (good - match) ***");
			Console.WriteLine("> Q  | *** Query Sheets ***");
			Console.WriteLine("> L  | *** List Sheet Types ***");
			Console.WriteLine("> 0  | *** Reset the list ***");
			Console.WriteLine("> X  | Exit");

			do
			{
				repeat = false;

				Console.WriteLine("> ? > ");

				ConsoleKeyInfo key = Console.ReadKey();

				string c = key.KeyChar.ToString().ToUpper();


				if (c == "R")
				{
					key = Console.ReadKey();
					c = key.KeyChar.ToString();

					Console.Write("\n");

					if (c == "1") processRemove_A();
					else if (c == "2") processRemove_B();
					else
					{
						Console.WriteLine("\n*****************\nInvalid Entry\n***************\n");
						repeat = true;
					}

					continue;
				}

				Console.Write("\n");

				switch (c)
				{
				case "A":
					{
						processAdd_A();
						break;
					}
				case "B":
					{
						processAdd_B();
						break;
					}
				case "C":
					{
						processAdd_C();
						break;
					}
				case "D":
					{
						processAdd_D();
						break;
					}
				case "E":
					{
						processAdd_E();
						break;
					}
				case "F":
					{
						processAdd_F();
						break;
					}
				case "L":
					{
						switchBoardListTypes();
						break;
					}
				case "Q":
					{
						string s = switchBoardSampleFolders();
						processQuery(s);
						break;
					}
				case "X":
					{
						result = false;
						break;
					}
				case "0":
					{
						SheetDataManager.Remove();
						break;
					}
				default:
					{
						Console.WriteLine("\n*****************\nInvalid Entry\n***************\n");
						repeat = true;
						break;
					}
				}
			}
			while (repeat);

			Console.Write("\n");

			// ShowSheetRectInfo.showStatus(ShowWhere.DEBUG, "@09");

			return result;
		}

		private void switchBoardListTypes()
		{
			Console.Write("\n");
			Console.WriteLine("> 1  | *** List Names ***");
			Console.WriteLine("> 2  | *** List Basic Info (rectangles) ***");
			Console.WriteLine("> 3  | *** List Complete info ***");

			ConsoleKeyInfo key = Console.ReadKey();
			Console.Write("\n\n");

			if (key.KeyChar == '1') showSheetNames();
			if (key.KeyChar == '2') showBasicRects();
			if (key.KeyChar == '3') showRectValues();

			// Program.DS("@22");
		}

		private string switchBoardSampleFolders()
		{
			Console.Write("\n");

			foreach (KeyValuePair<int, Sample> kvp in ss.SampleData)
			{
				Console.WriteLine($"> {kvp.Value.Index}  | *** {kvp.Value.Description}");
			}

			Console.Write("\n? ");

			ConsoleKeyInfo key = Console.ReadKey();
			Console.Write("\n\n");

			return key.KeyChar.ToString();
		}


		// setup

		private void processAdd_C()
		{
			Console.WriteLine("\n*** Add Files C ***");
			// ShowSheetRectInfo.showStatus(ShowWhere.CONSOLE);
			Console.WriteLine("Adding a file that does not exists ***");

			p.getFiles2();

			p.proceedAdd();
		}

		private void processAdd_B()
		{
			Console.WriteLine("\n*** Add Files B ***");
			// ShowSheetRectInfo.showStatus(ShowWhere.CONSOLE);
			Console.WriteLine("Adding a file that exists ***");

			p.getFiles4();

			p.proceedAdd();
		}

		private void processAdd_A()
		{
			Console.WriteLine("\n*** Add Files A ***");
			// ShowSheetRectInfo.showStatus(ShowWhere.CONSOLE);
			Console.WriteLine("Adding all files ***");

			p.getFiles1();

			p.proceedAdd();
		}

		private void processAdd_D()
		{
			Console.WriteLine("\n*** Add Files D ***");
			// ShowSheetRectInfo.showStatus(ShowWhere.CONSOLE);
			Console.WriteLine("Adding all files ***");

			p.getFiles_D();

			p.proceedAdd();
		}

		private void processAdd_E()
		{
			Console.WriteLine("\n*** Add Files E ***");
			// ShowSheetRectInfo.showStatus(ShowWhere.CONSOLE);
			Console.WriteLine("Adding all files ***");

			p.getFiles_R2();

			p.proceedAdd();
		}

		private void processAdd_F()
		{
			Console.WriteLine("\n*** Add Files F ***");
			Console.WriteLine("Adding all files ***");

			string idx = switchBoardSampleFolders();

			configSampleFiles(idx);

			proceedAdd();
		}

		private void testEnumerators()
		{
			Debug.WriteLine($"\n\nname list ({SheetDataManager.Data.SheetRectangles.Count})\n");


			foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.GetSheets())
			{
				string s = kvp.Value.Name;
				Debug.WriteLine($"{s}");
			}

			string fname = Path.GetFileNameWithoutExtension(filesNames[4]);

			Debug.WriteLine($"\n\nrect list ({SheetDataManager.Data.SheetRectangles[fname].ShtRects.Count})\n");

			foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp in SheetDataManager.GetRects(fname))
			{
				string s = kvp.Value.Id.ToString();
				Debug.WriteLine($"{s}");
			}
		}


		private void processRemove_A()
		{
			Console.WriteLine("\n*** Remove Files A ***");

			p.getFiles3();

			proceedRemove();
		}

		private void processRemove_B()
		{
			Console.WriteLine("\n*** Remove Files B ***");

			p.getFiles_R2();

			proceedRemove();
		}

		private void processQuery(string idx)
		{
			configSampleFiles(idx);

			proceedQuery();
		}

		// action

		private void proceedAdd()
		{
			lastOp = pm.ScanSheets(files);
		}

		private void proceedRemove()
		{
			lastOp = pm.RemoveSheets(files);
		}

		private void proceedQuery()
		{
			pm.QuerySheets(files);
		}

		// config

		private void configSampleFiles(string idx)
		{
			int index ;

			if (!int.TryParse(idx, out index)) return;

			// if (!ss.SampleData.TryGetValue(index, out sample)) return;

			if (!ss.GetFilesFromPdfFolder(index)) return;

			files = ss.FilePathList.ToArray();
		}

		private void getFiles1()
		{
			files = new string[4];

			files[0] = rootPath + filesNames[0];
			files[1] = rootPath + filesNames[1];
			files[2] = rootPath + filesNames[2];
			files[3] = rootPath + filesNames[3];
		}

		private void getFiles2()
		{
			files = new string[3];

			files[0] = rootPath + filesNames[2];
			files[1] = rootPath + filesNames[3];
			files[2] = rootPath + "no name.pdf";
		}

		private void getFiles4()
		{
			files = new string[1];

			files[0] = rootPath + filesNames[2];
		}

		private void getFiles_D()
		{
			files = new string[3];

			files[0] = rootPath + filesNames[0];
			files[1] = rootPath + filesNames[4];
			files[2] = rootPath + filesNames[5];
		}

		// remove 1 - bad - fails
		private void getFiles3()
		{
			files = new string[3];

			files[0] = rootPath + filesNames[3];
			files[1] = rootPath + filesNames[4];
			files[2] = rootPath + "no file.pdf";
		}


		// remove 2 - good - works
		private void getFiles_R2()
		{
			files = new string[1];

			files[0] = rootPath + filesNames[4];
		}

		private void getFiles_Q()
		{
			files = new string[1];

		files[0] = rootPath + filesNames[4];
		}


		private void showBasicRects()
		{
			ShowSheetRectInfo.StartMsg("Basic Rectangles", ShowWhere.CONSOLE, DateTime.Now.ToString());
			ShowSheetRectInfo.showShtRects(ShowWhere.CONSOLE);

			// ShowSheetRectInfo.showSinAndCos();
		}



		private void showRectValues()
		{
			ShowSheetRectInfo.StartMsg("All Rectangle Values", ShowWhere.CONSOLE, DateTime.Now.ToString());
			ShowSheetRectInfo.ShowValues(ShowWhere.CONSOLE);
		}

		private void showSheetNames()
		{
			ShowSheetRectInfo.StartMsg("Sheet Names", ShowWhere.CONSOLE, DateTime.Now.ToString());
			ShowSheetRectInfo.ShowSheetNames(ShowWhere.CONSOLE);
		}



	}
}