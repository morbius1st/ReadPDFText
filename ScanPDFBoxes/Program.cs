using System.Diagnostics;
using System.Runtime.CompilerServices;
using ShItextCode;
using ShSheetData;
using ShSheetData.SheetData;
using ShSheetData.SheetData2;
using ShTempCode.DebugCode;
using UtilityLibrary;
using ScanPDFBoxes.Process;
using ScanPDFBoxes.SheetData;
using ShSheetData.ShSheetData2;
using DebugCode;
using ShCode;
using ShSheetData.Support;
using iText.StyledXmlParser.Jsoup.Select;
using Settings;


namespace ScanPDFBoxes
{
	internal class Program : IWin
	{
		// [0] thru [4] == program - [0] show location msg
		// [10] thru [19] = SheetManager - [10] show location msg
		public static int[,] dmx;

		private static int sbIdx;

		private string sbSelecteItem;

		// true = worked / good | false = fail / error | null = cannot proceed / config issue
		private static bool? lastOp = null;

		public const string SHEET_DATA_FILE_TEST = "Sheet Data";

		public const string SHEET_METRIC_FOLDER =
			@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\";
		// private static string DataFilePath { get; } = SHEET_METRIC_FOLDER + SheetDataSet.DataFileName;

		private string rootPath =
			@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\TestBoxes\";

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

		// public static StatCodes StatusCode { get; private set; }

		public static SheetManager2 sm2;
		private static ConfigManager cm;
		private static ProcessManager pm;

		private static ShtDataSupport sds;

		// private static SheetFileManager2 sfm2;

		private int dataSetIdx = 1;
		private string dataSetName = "original";
		private string switchboardIdx;

		private bool mustHaveDataFile = true;

		private string[] files;

		private static Program p;

		private static Guid clsid = new Guid("FBD3B83F-DAC1-431E-9C22-42C3F593620D");
		private static IntPtr unk;


		static Program()
		{
			ConsoleFile.Init(
				@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\ReadPDFText\Console.txt");

			DM.init(5);
			DM.DbxSetDefaultWhere(0, ShowWhere.DEBUG);
			DM.DbxSetIdx(0, 0);
		}


		static void Main(string[] args)
		{
			bool repeat;

			Debug.WriteLine("\n".Repeat(10));

			PdfShowInfo.StartMsg("starting ScanPDFBoxes", $"{DateTime.Now}");

			DM.Start0();

			p = new Program();

			SheetFileManager2 sfm2 = new SheetFileManager2();

			// sfm2 = new SheetFileManager2();
			sm2 = new SheetManager2(sfm2);

			cm = new ConfigManager(sfm2, sm2);

			// pm = new ProcessManager(SHEET_METRIC_FOLDER);

			// pm.ConfigSheetData();

			sds = new ShtDataSupport(sm2);

			// sm2 = new SheetManager2();
			// p.configSheetDataManager();

			// p.ss = new ShSamples();

			DM.Stat0("ready");

			if (lastOp.HasValue)
			{
				Console.Write($"Last operation | {(lastOp.Value ? "worked" : "failed" )}");
			}

			do
			{
				// Debug.Write("\n");

				DM.DbxLineEx(0, "main switchboard"); // reset to level 1

				repeat = p.switchboardMain();
			}
			while (repeat);

			DM.End0();
		}

		// switchboards
		// select

		private bool switchboardMain()
		{
			bool repeat;
			bool result = true;
			bool? answer = false;

			Console.Write("\n");

			ShowSheetRectInfo.showStatus(ShowWhere.CONSOLE);

			sbOptionMainList(0);

			do
			{
				repeat = false;

				Console.Write("\n> ? > ");

				ConsoleKeyInfo key = Console.ReadKey();

				string c1 = null;

				string c = key.KeyChar.ToString().ToUpper();
				Console.Write(c);

				string test = "ACILORT";

				if (test.Contains(c))
				{
					key = Console.ReadKey();
					c1 = key.KeyChar.ToString().ToUpper();
					c += c1;
					Console.WriteLine(c1);
				}

				dataSetName = "original";
				Tuple<string, bool?, bool?, bool?, bool?> selected = findSbOptionMain(c);

				// if (selected != null) DM.DbxLineEx(0, $">>> selected | {selected.Item1}  ({selected.Item2})", 1);
				if (selected != null) DM.Start0(true,  $"*** switchboard choice {c} | >>> selected | {selected.Item1}  ({selected.Item2})");

				bool? mustHaveDataFilePath = selected?.Item2 ?? null;
				bool? mustHaveDataFile = selected?.Item3 ?? null;
				bool? mustHaveDataFileSheets  = selected?.Item4 ?? null;
				bool? mustHaveSheetFileList  = selected?.Item5 ?? null;

				Console.Write("\n");

				int scanConfigIdxOrig = 2;
				int scanConfigIdxNew = 7;


				switch (c)
				{
				// case "A1":
				// 	{
				// 		int choice =  switchBoardTestAdd();
				//
				// 		if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
				// 				mustHaveSheetFileList) != true) break;
				//
				//
				// 		switchAddTestOrig(choice);
				// 		break;
				// 	}
				case "A2":
					{
						dataSetName = "new";

						if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
								mustHaveSheetFileList) != true) break;

						processAdd_F_Alt();

						break;
					}

				case "AS":
					{
						dataSetName = "new";

						if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
								mustHaveSheetFileList) != true) break;


						processAdd1_F_Alt();

						break;
					}

				// case "L1":
				// 	{
				// 		dataSetIdx = 1;
				// 		switchBoardListTypes();
				// 		break;
				// 	}
				case "L2":
					{
						if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
								mustHaveSheetFileList) != true) break;

						dataSetName = "new";
						dataSetIdx = 2;
						switchBoardListTypes();
						break;
					}


				// case "O1":
				// 	{
				// 		if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
				// 				mustHaveSheetFileList) != true) break;
				//
				// 		dataSetIdx = 1;
				// 		break;
				// 	}
				case "O2":
					{
						dataSetName = "new";

						answer = cm.verifyConfig2(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
							mustHaveSheetFileList);

						if (StatusMgr.Current != StatusCodes.SC_CFG_DATA_FILE_PATH_MISSING)
						{
							showStatus(true, true);
							showScanReadyStatus(answer == true);

							if (answer != true) break;
						}

						proceedOpen2();

						showStatus(true, true);

						dataSetIdx = 2;
						break;
					}
				// case "Q":
				// 	{
				// 		if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
				// 				mustHaveSheetFileList) != true) break;
				//
				// 		// todo fix
				// 		// string s = switchBoardSampleFolders();
				// 		// processQuery(s);
				// 		break;
				// 	}
				// case "R1":
				// 	{
				// 		if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
				// 				mustHaveSheetFileList) != true) break;
				//
				// 		processRemove_A();
				// 		break;
				// 	}
				// case "R3":
				// 	{
				// 		if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
				// 				mustHaveSheetFileList) != true) break;
				//
				// 		processRemove_B();
				// 		break;
				// 	}

				case "R2":
					{
						dataSetName = "new";

						if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
								mustHaveSheetFileList) != true) break;

						processRemove2();
						break;
					}

				case "RR":
					{
						if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
								mustHaveSheetFileList) != true) break;

						// dataSetIdx = 1;
						switchBoardReports();
						break;
					}

				// case "C1":
				// 	{
				// 		if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
				// 				mustHaveSheetFileList) != true) break;
				//
				// 		proceedClose1();
				// 		break;
				// 	}
				case "C2":
					{
						dataSetName = "new";

						if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
								mustHaveSheetFileList) != true) break;

						proceedClose2();
						break;
					}

				// case "1":
				// 	{
				// 		if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
				// 				mustHaveSheetFileList) != true) break;
				// 		proceedReset1();
				// 		break;
				// 	}
				case "I2":
					{
						dataSetName = "new";

						answer = cm.initialize(c, 0);
						showStatus(true, true);
						showScanReadyStatus(answer == true);
						showInitStatus();
						break;
					}

				case "IS":
					{
						dataSetName = "new";

						if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
								mustHaveSheetFileList) != true) break;

						// int choice =  switchBoardTestAdd();
						//
						// if (choice < 0) break;

						answer = cm.initialize(c, 100);

						showStatus(true, true);
						showScanReadyStatus(answer == true);
						showInitStatus();
						break;
					}

				case "T0":
					{
						dataSetName = "new";

						if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
								mustHaveSheetFileList) != true) break;

						processCreateTestFile();

						break;
					}

				case "T1":
					{
						dataSetName = "new";

						proceedQuery2();
						break;
					}

				case "T2":
					{
						dataSetName = "new";

						testGetConfig();
						break;
					}

				case "0":
					{
						dataSetName = "new";

						if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
								mustHaveSheetFileList) != true) break;

						proceedFullReset2();
						break;
					}

				case "2":
					{
						dataSetName = "new";

						// new reset the data manager

						answer = cm.verifyConfig2(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
							mustHaveSheetFileList);

						showStatus(true, true);
						showScanReadyStatus(answer == true);

						showInitStatus();
						if (answer != true) break;

						proceedResetDataManager2();
						break;
					}
				case "4":
					{
						dataSetName = "new";

						if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
								mustHaveSheetFileList) != true) break;

						proceedResetDataFile2();
						break;
					}

				case "6":
					{
						dataSetName = "new";

						if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
								mustHaveSheetFileList) != true) break;

						proceedResetSheetFileManager();
						break;
					}

				case "8":
					{
						dataSetName = "new";

						if (verifyConfig(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
								mustHaveSheetFileList) != true) break;

						proceedResetForNewSheets();
						break;
					}

				case "X":
					{
						result = false;
						break;
					}
				default:
					{
						Console.WriteLine("\n*****************\nInvalid Entry\n***************\n");
						repeat = true;
						break;
					}
				}


				if (selected != null) DM.DbxLineEx(0, $"<<< selected | {selected.Item1}", 0, -1);
			}
			while (repeat);

			Console.Write("\n");

			return result;
		}

		private bool verifyConfig(string c, bool? mustHaveDataFilePath, bool? mustHaveDataFile,
			bool? mustHaveDataFileSheets,
			bool? mustHaveSheetFileList)
		{
			bool answer = cm.verifyConfig2(c, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets,
				mustHaveSheetFileList);

			showStatus(true, true);
			showScanReadyStatus(answer == true);

			return answer;
		}

		private void switchBoardListTypes()
		{
			Console.Write("\n");
			// Console.WriteLine("> 1  | *** List Names ***");
			// Console.WriteLine("> 2  | *** List Basic Info (rectangles) ***");
			// Console.WriteLine("> 3  | *** List Complete info ***");

			Console.WriteLine($"*** {dataSetName} data set ***");

			sbOptionMinorList(0);

			string c = Console.ReadKey().KeyChar.ToString().ToUpper();

			Console.Write($"{c}\n\n");

			if (c.Equals("X")) return;

			string selected = findSbOptionMinor(c).Item1;

			if (selected != null) DM.DbxLineEx(0, $">>> selected | {selected}", 1);

			if (c.Equals("1")) showSheetNames();
			if (c.Equals("2")) showBasicRects();
			if (c.Equals("3")) showRectValues();

			if (selected != null) DM.DbxLineEx(0, $"<<< selected | {selected}", -1);
		}

		private int switchBoardTestAdd()
		{
			Console.Write("\n");
			Console.WriteLine($"*** {dataSetName} data set ***");

			sbOptionMinorList(1);

			string c = Console.ReadKey().KeyChar.ToString().ToUpper();

			Console.Write($"{c}\n\n");

			int i;

			bool result = Int32.TryParse(c, out i);

			if (!result) i = -1;

			return i;
		}

		private void switchBoardReports()
		{
			bool repeat = true;
			string selected = "";
			string c = "";

			Console.Write("\n");
			Console.WriteLine($"*** {dataSetName} data set ***");

			do
			{
				sbOptionMinorList(2);

				c = Console.ReadKey().KeyChar.ToString().ToUpper();

				Console.Write($"{c}\n\n");
				if (c.Equals("X")) return;

				selected = findSbOptionMinor(c)?.Item1;

				if (selected == null)
				{
					Console.WriteLine("invalid selection - try again\n");
				} else
				{
					repeat = false;
				}
			}
			while (repeat);

			if (selected != null) DM.DbxLineEx(0, $">>> selected | {selected}", 1);

			if (c.Equals("1")) showBoxTypes();
			else if (c.Equals("2")) showLocations();
			else if (c.Equals("3")) sds.ShowClassifFiles(this);
			else if (c.Equals("4")) sds.ShowShtMetricFiles(this);
			else if (c.Equals("5")) sds.ShowScanSamples(this);
			else if (c.Equals("6")) sds.ShowAssemblySamples(this);

			if (selected != null) DM.DbxLineEx(0, $"<<< selected | {selected}", -1);
		}

		private bool switchboardSelectShtDataFile()
		{
			DM.DbxLineEx(0, "start", 1);

			int i;
			int idx = 1;

			sbSelecteItem = null;

			string[] items = new string[SheetDataManager2.Data.SheetDataList.Count + 1];

			Console.Write("\n");

			foreach (KeyValuePair<string, SheetData2> kvp in SheetDataManager2.Data.SheetDataList)
			{
				items[idx] = kvp.Key;
				Console.WriteLine($"> {idx++,-2}  | *** {kvp.Value.Name}");
			}

			Console.WriteLine($"> {'X',-2}  | *** Exit");
			Console.Write("\n? ");

			string c = Console.ReadKey().KeyChar.ToString().ToUpper();

			Console.Write($"{c}\n\n");

			if (c.Equals("X"))
			{
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			Console.Write("\n\n");

			bool result = Int32.TryParse(c, out i);

			if (result)
			{
				sbSelecteItem = items[i];
			}

			DM.DbxLineEx(0, "end", 0, -1);

			return result;
		}

		// private void switchAddTestOrig(int c)
		// {
		// 	string selected = sbOptionsMinor[1][c.ToString()].Item1;
		//
		// 	if (selected != null) DM.DbxLineEx(0, $">>> selected | {selected}", 1);
		//
		// 	Console.Write("\n");
		//
		// 	switch (c)
		// 	{
		// 	case 1:
		// 		{
		// 			processAdd_A();
		// 			break;
		// 		}
		// 	case 2:
		// 		{
		// 			processAdd_B();
		// 			break;
		// 		}
		// 	case 3:
		// 		{
		// 			processAdd_C();
		// 			break;
		// 		}
		// 	case 4:
		// 		{
		// 			processAdd_D();
		// 			break;
		// 		}
		// 	case 5:
		// 		{
		// 			processAdd_E();
		// 			break;
		// 		}
		// 	case 7:
		// 		{
		// 			processAdd_F();
		// 			break;
		// 		}
		// 	default:
		// 		{
		// 			Console.WriteLine("\n*****************\nInvalid Entry\n***************\n");
		// 			break;
		// 		}
		// 	}
		//
		// 	Console.Write("\n");
		//
		// 	if (selected != null) DM.DbxLineEx(0, $"<<< selected | {selected}", -1);
		// }

		private void switchAddTestNew(int c)
		{
			string selected = sbOptionsMinor[1][c.ToString()].Item1;

			if (selected != null) DM.DbxLineEx(0, $">>> selected | {selected}", 1);

			Console.Write("\n");

			switch (c)
			{
			case 1:
				{
					Console.WriteLine("not implemented");
					DM.DbxLineEx(0, $"\toption {c} is not implemented");
					break;
				}
			case 2:
				{
					Console.WriteLine("not implemented");
					DM.DbxLineEx(0, $"\toption {c} is not implemented");
					break;
				}
			case 3:
				{
					Console.WriteLine("not implemented");
					DM.DbxLineEx(0, $"\toption {c} is not implemented");
					break;
				}
			case 4:
				{
					Console.WriteLine("not implemented");
					DM.DbxLineEx(0, $"\toption {c} is not implemented");
					break;
				}
			case 5:
				{
					Console.WriteLine("not implemented");
					DM.DbxLineEx(0, $"\toption {c} is not implemented");
					break;
				}
			case 7:
				{
					processAdd_F_Alt();
					break;
				}
			default:
				{
					Console.WriteLine("\n*****************\nInvalid Entry\n***************\n");
					break;
				}
			}

			Console.Write("\n");

			if (selected != null) DM.DbxLineEx(0, $"<<< selected | {selected}", -1);
		}

		private bool switchBoardGetBoxCount(out int[] stdBoxes, out int optBoxes)
		{
			stdBoxes = new [] { 1 };
			optBoxes = 1;

			Console.Write("\n");
			Console.WriteLine($"*** choose which quantity of temp boxes ***");

			sbOptionMinorList(3);

			string c = Console.ReadKey().KeyChar.ToString().ToUpper();

			Console.Write($"{c}\n\n");

			if (c.Equals('X')) return false;

			stdBoxes = selectTempStdBoxs(c);

			optBoxes = selectTempOptBoxes();

			return true;
		}

		public void DebugMsgLine(string msg)
		{
			Debug.WriteLine(msg);

		}
		public void DebugMsg(string msg)
		{
			Debug.Write(msg);
		}


		// config - setup

		// setup
		//
		// private void processAdd_C()
		// {
		// 	Console.WriteLine("\n*** Add Files C ***");
		// 	// ShowSheetRectInfo.showStatus(ShowWhere.CONSOLE);
		// 	Console.WriteLine("Adding a file that does not exists ***");
		//
		// 	p.getFiles2();
		//
		// 	p.proceedAdd();
		// }
		//
		// private void processAdd_B()
		// {
		// 	Console.WriteLine("\n*** Add Files B ***");
		// 	// ShowSheetRectInfo.showStatus(ShowWhere.CONSOLE);
		// 	Console.WriteLine("Adding a file that exists ***");
		//
		// 	p.getFiles4();
		//
		// 	p.proceedAdd();
		// }
		//
		// private void processAdd_A()
		// {
		// 	Console.WriteLine("\n*** Add Files A ***");
		// 	// ShowSheetRectInfo.showStatus(ShowWhere.CONSOLE);
		// 	Console.WriteLine("Adding all files ***");
		//
		// 	p.getFiles1();
		//
		// 	p.proceedAdd();
		// }
		//
		// private void processAdd_D()
		// {
		// 	Console.WriteLine("\n*** Add Files D ***");
		// 	// ShowSheetRectInfo.showStatus(ShowWhere.CONSOLE);
		// 	Console.WriteLine("Adding all files ***");
		//
		// 	p.getFiles_D();
		//
		// 	p.proceedAdd();
		// }
		//
		// private void processAdd_E()
		// {
		// 	Console.WriteLine("\n*** Add Files E ***");
		// 	// ShowSheetRectInfo.showStatus(ShowWhere.CONSOLE);
		// 	Console.WriteLine("Adding all files ***");
		//
		// 	p.getFiles_R2();
		//
		// 	p.proceedAdd();
		// }
		//
		// private void processAdd_F()
		// {
		// 	Console.WriteLine("\n*** Add Files F ***");
		// 	Console.WriteLine("Adding all files ***");
		//
		// 	// init.
		// 	//
		// 	// string idx = switchBoardSampleFolders();
		// 	//
		// 	// configSampleFiles(idx);
		//
		// 	proceedAdd();
		// }


		private void processAdd_F_Alt()
		{
			PdfShowInfo.StartMsg("Add Files F", DateTime.Now.ToString());

			DM.Start0("*** start | processAdd_F_Alt ***");

			proceedAdd2();

			DM.End0("*** end | processAdd_F_Alt ***");
		}

		private void processAdd1_F_Alt()
		{
			DM.DbxSetIdx(0, 2);
			DM.DbxLineEx(0, "start", 1);

			PdfShowInfo.StartMsg("Add Files F", DateTime.Now.ToString());

			string file = sm2.SwitchboardSelectSheetFile();

			if (file == null)
			{
				DM.DbxLineEx(0, "end 1", 0, -1);
				return;
			}

			string s1 = $"*** selected| {file}";

			Console.WriteLine(s1);

			DM.DbxLineEx(0, s1);

			proceedAdd2_1(file);

			DM.DbxLineEx(0, "end", 0, -1);
		}

		// private void testEnumerators()
		// {
		// 	Debug.WriteLine($"\n\nname list ({SheetDataManager.Data.SheetRectangles.Count})\n");
		//
		//
		// 	foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.GetSheets())
		// 	{
		// 		string s = kvp.Value.Name;
		// 		Debug.WriteLine($"{s}");
		// 	}
		//
		// 	string fname = Path.GetFileNameWithoutExtension(filesNames[4]);
		//
		// 	Debug.WriteLine($"\n\nrect list ({SheetDataManager.Data.SheetRectangles[fname].ShtRects.Count})\n");
		//
		// 	foreach (KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp in SheetDataManager.GetRects(fname))
		// 	{
		// 		string s = kvp.Value.Id.ToString();
		// 		Debug.WriteLine($"{s}");
		// 	}
		// }
		//
		// private void processRemove_A()
		// {
		// 	Console.WriteLine("\n*** Remove Files A ***");
		//
		// 	p.getFiles3();
		//
		// 	proceedRemove1();
		// }
		//
		// private void processRemove_B()
		// {
		// 	Console.WriteLine("\n*** Remove Files B ***");
		//
		// 	p.getFiles_R2();
		//
		// 	proceedRemove1();
		// }
		//
		// private void processQuery(string idx)
		// {
		// 	// configSampleFiles(idx);
		//
		// 	proceedQuery1();
		// }

		private void processRemove2()
		{
			DM.DbxLineEx(0, "start", 1);

			if (!switchboardSelectShtDataFile()) return;

			DM.DbxLineEx(0, $"*** selected for removal {sbSelecteItem}");

			proceedRemove2(sbSelecteItem);

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void processCreateTestFile()
		{
			int shtQty;
			int[] stdBoxes;
			int optBoxes;

			if (!selectTempShtQty(out shtQty)) return;

			if (!switchBoardGetBoxCount(out stdBoxes, out optBoxes)) return;

			proceedCreateTestFile(shtQty, stdBoxes, optBoxes);
		}

		// action

		// dataset 1

		// private void proceedAdd()
		// {
		// 	lastOp = pm.ScanSheets(files);
		// }
		//
		// private void proceedRemove1()
		// {
		// 	lastOp = pm.RemoveSheets(files);
		// }
		//
		// private void proceedQuery1()
		// {
		// 	pm.QuerySheets(files);
		// }
		//
		// private void proceedClose1()
		// {
		// 	pm.Close();
		// }
		//
		// private void proceedReset1()
		// {
		// 	pm.Reset();
		// }


		// dataset 2

		private void proceedAdd2()
		{
			DM.Start0();

			lastOp = sm2.ScanShts();

			showLastOpResult();

			DM.End0();
		}

		private void proceedAdd2_1(string file)
		{
			lastOp = sm2.ScanShts();

			showLastOpResult();
		}

		private void proceedOpen2()
		{
			SetStatus(StatusCodes.SC_G_NONE);

			lastOp = sm2.OpenDataManager();

			showLastOpResult();
		}

		private void proceedClose2()
		{
			lastOp = sm2.CloseDataManager();

			showLastOpResult();
		}

		private void proceedQuery2()
		{
			SetStatus(StatusCodes.SC_G_NONE);

			DM.DbxLineEx(0, $"start", 1);

			if (!cm.configSheetPdfScanFiles(1))
			{
				showStatus(true);
				SetStatus(StatusCodes.SC_RUN_LOAD_SHT_DATA_FILES_FAIL);
				DM.DbxLineEx(0, "end 1", 0, -1);
				return;
			}

			sm2.QueryShts();

			SetStatus(StatusCodes.SC_G_GOOD);
		}

		// T0
		private void proceedCreateTestFile(int shtQty, int[] stdBoxes, int optBoxes)
		{
			DM.Start0();

			SetStatus(StatusCodes.SC_G_NONE);

			sds.createTempSheetFile(shtQty, stdBoxes, optBoxes);

			DM.End0();
		}

		/// <summary>
		/// reset the data file so that new data can be added but<br/>
		/// do not change the paths or filenames
		/// </summary>
		private void proceedResetDataManager2()
		{
			DM.DbxLineEx(0, "start");

			sm2.ResetDataManager();

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void proceedResetDataFile2()
		{
			sm2.ResetDataFile();
		}

		private void proceedFullReset2()
		{
			DM.DbxLineEx(0, "start");

			sm2.ResetFull();

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void proceedResetSheetFileManager()
		{
			DM.DbxLineEx(0, "start");

			sm2.ResetShtFileMgr();

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void proceedResetForNewSheets()
		{
			DM.DbxLineEx(0, "start");

			sm2.ResetForNewSheets();

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void proceedRemove2(string sheet)
		{
			lastOp = sm2.RemoveSheet(sheet);

			showLastOpResult();
		}

		// config

		// private bool getScanConfigInfo(int def, bool? mustHaveDataFile)
		// {
		// 	DM.DbxLineEx(0, "\tgetScanConfigInfo");
		//
		// 	if (!cm.SelectScanConfigFiles(def)) return false;
		//
		// 	return sfm2.ConfigScan(mustHaveDataFile);
		// }
		//
		// private void configSampleFiles(string idx)
		// {
		// 	int index ;
		//
		// 	if (!int.TryParse(idx, out index)) return;
		//
		// 	// if (!ss.SampleData.TryGetValue(index, out sample)) return;
		//
		// 	if (!ss.GetFilesFromPdfFolder(index)) return;
		//
		// 	files = ss.FilePathList.ToArray();
		// }

		/*

		/// <summary>
		/// select the configuration settings
		/// </summary>
		private bool configGetScanInfo(int def)
		{
			DM.DbxLineEx(0, "start", 1);

			if (!cm.SelectScanConfigFiles(2, def, true))
			{
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			DM.DbxLineEx(0, "end", 0, -1);

			return true;
		}

		/// <summary>
		/// ** config the path / file name for the data manager
		/// </summary>
		private bool? configSheetDataFilePath(int def)
		{
			// tasks
			// use configmanager to set usersettings / datafilepath
			// use sheetmanager to move usersettings to sheetFileManager
			// use sheetFileManager to define folder / file status
			// return true if folderpath is good & file exists
			// return false if folderpath is bad
			// return null if folderpath is good & file does not exist

			DM.DbxLineEx(0, "start", 1);

			// if (!cm.SelectScanConfigFiles(0, def))
			// {
			// 	DM.DbxLineEx(0, "end 1", 0, -1);
			// 	return false;
			// }

			if (!sm2.InitDataFilePath())
			{
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			DM.DbxLineEx(0, "end", 0, -1);

			return sfm2.GotDataFilePath;
		}

		/// <summary>
		/// starts and opens the data manager using the collected path information
		/// </summary>
		/// <returns></returns>
		private bool? StartDataManager()
		{
			// tasks
			// use configmanager to set usersettings / datafilepath
			// use sheetmanager to move usersettings to sheetFileManager
			// use sheetFileManager to define folder / file status
			// return true if folderpath is good & file exists
			// return false if folderpath is bad
			// return null if folderpath is good & file does not exist

			DM.DbxLineEx(0, "start", 1);

			if (!sm2.StartDataManager())
			{
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			DM.DbxLineEx(0, "end", 0, -1);

			return sfm2.GotDataFilePath;
		}

		/// <summary>
		/// ** config the path for the scan Pdf folder
		/// </summary>
		private bool configSheetPdfScanFolderPath(int def)
		{
			// tasks
			// use configmanager to set usersettings / ScanPDfFolder
			// use sheetmanager to move usersettings to sheetFileManager
			// use sheetFileManager to define folder / file status
			// return true if folderpath is good
			// return false if folderpath is bad

			DM.DbxLineEx(0, "start", 1);

			// if (!cm.SelectScanConfigFiles(1, def))
			// {
			// 	DM.DbxLineEx(0, "end 1", 0, -1);
			// 	return false;
			// }

			if (!sm2.InitSheetFileFolder())
			{
				DM.DbxLineEx(0, "end 2", 0, -1);
				return false;
			}

			DM.DbxLineEx(0, "end", 0, -1);

			return sfm2.SheetFolderExists;
		}

		/// <summary>
		/// ** config list of scan PDF files
		/// </summary>
		private bool configSheetPdfScanFiles()
		{
			DM.DbxLineEx(0, "start", 1);

			if (!sfm2.SheetFolderExists)
			{
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			DM.DbxLineEx(0, "end", 0, -1);

			return sfm2.GetSheetFiles();
		}

		*/
		
		// /// <summary>
		// /// run second
		// /// configure the paths and file names for the<br/>
		// /// output file, pdf folder, etc.<br/>
		// /// initialize the sheet data file
		// /// </summary>
		// // private bool configSheetInfo(string idx)
		// // {
		// // 	DM.DbxLineEx(0, "start", 1);
		// //
		// // 	bool? result;
		// //
		// // 	int index;
		// //
		// // 	if (!int.TryParse(idx, out index)) return false;
		// //
		// // 	if (!sm2.InitSheetFiles(index))
		// // 	{
		// // 		DM.DbxLineEx(0, "end 1 - (", -1);
		// // 		return false;
		// // 	}
		// //
		// // 	result = sm2.InitSheetData();
		// //
		// // 	if (result == false)
		// // 	{
		// // 		Console.WriteLine("\n*** cannot proceed, the data file is not initialized ***");
		// // 		DM.DbxLineEx(0, "end 2 (data file not init)", -1);
		// // 		return false;
		// // 	}
		// // 	else if (!result.HasValue)
		// // 	{
		// // 		Console.WriteLine("\n*** cannot proceed, sheet data exists ***");
		// // 		DM.DbxLineEx(0, "end 3 (sheet data exists)", -1);
		// // 		return false;
		// // 	}
		// //
		// // 	DM.DbxLineEx(0, "end", 0, -1);
		// //
		// // 	return true;
		// // }
		//
		// // /// <summary>
		// // /// run first or only
		// // /// config the path / file name for the data manager
		// // /// </summary>
		// // private bool configSheetDataManager()
		// // {
		// // 	DM.DbxLineEx(0, "start", 1);
		// //
		// //
		// // 	// if (!sm2.InitDataFile())
		// // 	// {
		// // 	// 	DM.DbxLineEx(0, "end 2", 0, -1);
		// // 	// 	return false;
		// // 	// }
		// //
		// // 	if (!sm2.initDataManager())
		// // 	{
		// // 		DM.DbxLineEx(0, "end 3", 0, -1);
		// // 		return false;
		// // 	}
		// //
		// // 	DM.DbxLineEx(0, "end", 0, -1);
		// //
		// // 	return true;
		// // }
		//
		// // get files
		// private void getFiles1()
		// {
		// 	files = new string[4];
		//
		// 	files[0] = rootPath + filesNames[0];
		// 	files[1] = rootPath + filesNames[1];
		// 	files[2] = rootPath + filesNames[2];
		// 	files[3] = rootPath + filesNames[3];
		// }
		//
		// private void getFiles2()
		// {
		// 	files = new string[3];
		//
		// 	files[0] = rootPath + filesNames[2];
		// 	files[1] = rootPath + filesNames[3];
		// 	files[2] = rootPath + "no name.pdf";
		// }
		//
		// private void getFiles4()
		// {
		// 	files = new string[1];
		//
		// 	files[0] = rootPath + filesNames[2];
		// }
		//
		// private void getFiles_D()
		// {
		// 	files = new string[3];
		//
		// 	files[0] = rootPath + filesNames[0];
		// 	files[1] = rootPath + filesNames[4];
		// 	files[2] = rootPath + filesNames[5];
		// }
		//
		// // remove 1 - bad - fails
		// private void getFiles3()
		// {
		// 	files = new string[3];
		//
		// 	files[0] = rootPath + filesNames[3];
		// 	files[1] = rootPath + filesNames[4];
		// 	files[2] = rootPath + "no file.pdf";
		// }
		//
		// // remove 2 - good - works
		// private void getFiles_R2()
		// {
		// 	files = new string[1];
		//
		// 	files[0] = rootPath + filesNames[4];
		// }
		//
		// private void getFiles_Q()
		// {
		// 	files = new string[1];
		//
		// 	files[0] = rootPath + filesNames[4];
		// }



		// helpers

		private string trunc(string s, int max)
		{
			string s1;

			int len = s.Length;

			int pos = len > max - 3 ? len - (max - 3) : 0;

			s1 = s.Substring(pos);

			if (pos > 0) s1 = "..." + s1;

			return s1;
		}

		private void SetStatus(StatusCodes code,  string note = null,
			[CallerMemberName] string mx = null)
		{
			StatusMgr.SetStatCode(code, note, mx);
		}

		private void showStatus(bool showFrom = false, bool showOk = false)
		{
			StatusMgr.ShowStatus(showFrom, showOk);
		}

		private void showScanReadyStatus(bool which)
		{
			string s;
			if (which)
			{
				s = "*** IS ready - continue ***";
				Console.WriteLine(s);
				DM.DbxLineEx(0, s);
			}
			else
			{
				s =  "*** is NOT ready - break ***";
				Console.WriteLine(s);
				DM.DbxLineEx(0, s);
			}
		}

		private void showInitStatus()
		{
			DM.DbxLineEx(0, "start", 1); // 1, 1

			int tw = -30;
			int max = 30;

			string s1;
			string fmt = $"{{0,{tw}}}";

			try
			{

				DM.Stat0("sfm2 (SheetFileManager2");
				DM.DbxInc0();
				DM.Stat0("data manager");
				DM.DbxInc0();
				DM.Stat0("status");
				DM.DbxInc0();
				DM.Stat0( $"{(string.Format(fmt, "GotDataFile"))}| {sm2.GotDataFile?.ToString() ?? "null",-10} (data file path not null & data file path exists)");

				// DM.DbxLineEx(0, "sfm2 (SheetFileManager2", 1); // 1, 1
				// DM.DbxLineEx(0, "data manager", 1);            // 2, 3
				// // Debug.Write("\n");
				// DM.DbxLineEx(0, "status", 1, 1);
				//
				// DM.DbxEx(0, $"{(string.Format(fmt, "GotDataFile"))}| {sm2.GotDataFile?.ToString() ?? "null",-10}");
				// Debug.WriteLine("(data file path not null & data file path exists)");



				DM.DbxEx(0, $"{(string.Format(fmt, "GotDataFilePath"))}| {sm2.GotDataFilePath,-10}");
				Debug.WriteLine($"(data file path is not null)");

				DM.DbxLineEx(0, "");
				//
				// // Debug.Write("\n");
				// DM.DbxLineEx(0, "values", -1, 1);
				//
				// DM.DbxLineEx(0, $"{(string.Format(fmt, "DataFilePath"))}| {sm2.DataFilePath?.FolderPath ?? "null"}");

				// DM.DbxLineEx(0, "", -1);
				DM.DbxLineEx(0, "");

				// Debug.Write("\n");
				DM.DbxLineEx(0, "sheet file", -1);
				// Debug.Write("\n");
				DM.DbxLineEx(0, "status", 1, 1);

				DM.DbxEx(0, $"{(string.Format(fmt, "GotSheetFileList"))}| {sm2.GotSheetFileList,-10}");
				Debug.WriteLine("(sheet folder not null && list not null && count >0 [true])");

				DM.DbxEx(0, $"{(string.Format(fmt, "GotSheetFolder"))}| {sm2.GotSheetFolder,-10}");
				Debug.WriteLine("(sheet folder not null [true])");

				DM.DbxEx(0, $"{(string.Format(fmt, "SheetFolderExists"))}| {sm2.SheetFolderExists,-10}");
				Debug.WriteLine("(sheet folder not null && folder exists [true])");

				DM.DbxLineEx(0, "");

				// Debug.Write("\n");
				DM.DbxLineEx(0, "values", -1, 1);

				DM.DbxLineEx(0, $"{(string.Format(fmt, "SheetFileFolder"))}| {sm2.SheetFileFolder ?? "null"}");
				DM.DbxLineEx(0,
					$"{(string.Format(fmt, "SheetFileList"))}| {sm2.SheetFileList?.Count.ToString() ?? "null"}");

				DM.DbxLineEx(0, "");
				// Debug.Write("\n");
				DM.DbxLineEx(0, "SheetDataManager2", -3);
				// Debug.Write("\n");
				DM.DbxLineEx(0, "status", 1, 1);

				DM.DbxEx(0, $"{(string.Format(fmt, "Initialized"))}| {SheetDataManager2.Initialized,-10}");
				Debug.WriteLine("(Manager is not null (created) )");

				DM.DbxEx(0, $"{(string.Format(fmt, "GotDataPath"))}| {SheetDataManager2.GotDataPath,-10}");
				Debug.WriteLine("(path configured and folder path is valid)");

				DM.DbxEx(0, $"{(string.Format(fmt, "GotDataSheets"))}| {SheetDataManager2.GotDataSheets,-10}");
				Debug.WriteLine("(data list exists and count > 0)");

				DM.DbxEx(0,
					$"{(string.Format(fmt, "SettingsFileExists"))}| {SheetDataManager2.SettingsFileExists,-10}");
				Debug.WriteLine("(file path configured and file exists)");

				DM.DbxEx(0,
					$"{(string.Format(fmt, "Admin.Status"))}| {SheetDataManager2.Admin?.Status.ToString() ?? "null",-10}");
				Debug.WriteLine("(internal status)");

				DM.DbxLineEx(0, "");

				// Debug.Write("\n");
				DM.DbxLineEx(0, "values", -1, 1);

				DM.DbxLineEx(0,
					$"{(string.Format(fmt, "SettingFilePath"))}| {SheetDataManager2.Path?.SettingFilePath ?? "null"}");

				// DM.DbxLineEx(0, "", -1);
				DM.DbxLineEx(0, "", -3);
			}
			catch (Exception e)
			{
				Debug.WriteLine("*** oops - status failed");
				Debug.WriteLine(e.Message);
			}

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private bool? showLastOpResult()
		{
			if (lastOp == false)
			{
				Console.WriteLine("\n*** FAILED ***");
			}
			else if (lastOp == true)
			{
				Console.WriteLine("\n*** WORKED ***");
			}
			else
			{
				Console.WriteLine("\n*** NOT CONFIGURED ***");
			}

			return lastOp;
		}

		private string getWhichDataSet()
		{
			string which;

			if (dataSetIdx == 1)
			{
				which = "orig";
			}
			else
			{
				which = "new";
			}

			return which;
		}

		private void showBasicRects()
		{
			string which = getWhichDataSet();

			ShowSheetRectInfo.StartMsg($"Basic Rectangles ({which})", ShowWhere.CONSOLE, DateTime.Now.ToString());

			// if (dataSetIdx == 1)
			// {
			// 	ShowSheetRectInfo.showShtRects(ShowWhere.CONSOLE);
			// }
			// else
			// {
			// }
			ShowSheetRectInfo.showShtRects2(ShowWhere.CONSOLE);
		}

		private void showRectValues()
		{
			string which = getWhichDataSet();

			ShowSheetRectInfo.StartMsg($"All Rectangle Values ({which})", ShowWhere.CONSOLE, DateTime.Now.ToString());

			// if (dataSetIdx == 1)
			// {
			// 	ShowSheetRectInfo.ShowValues(ShowWhere.CONSOLE);
			// }
			// else
			// {
			// }
			ShowSheetRectInfo.ShowValues2(ShowWhere.CONSOLE);
		}

		private void showSheetNames()
		{
			string which = getWhichDataSet();

			ShowSheetRectInfo.StartMsg($"Sheet Names ({which})", ShowWhere.CONSOLE, DateTime.Now.ToString());

			// if (dataSetIdx == 1)
			// {
			// 	ShowSheetRectInfo.ShowSheetNames(ShowWhere.CONSOLE);
			// }
			// else
			// {
			// }
			ShowSheetRectInfo.ShowSheetNames2(ShowWhere.CONSOLE);
		}

		private void showBoxTypes()
		{
			ShowInfo. showRectIdXrefInfo();
		}

		private void showLocations()
		{
			SettingsManager.FileLocationSupport.ShowLocations(this);
		}

		private void showClassifFiles()
		{

		}

		private void showShtMetricFiles()
		{

		}

		private int[] selectTempStdBoxs(string c)
		{
			int[] stdBoxes;

			switch (c)
			{
			case "A":
				{
					stdBoxes = new int[] { 1};
					break;
				}
			case "B":
				{
					stdBoxes = new int[] { 1, 2, 3, 4, 5 };
					break;
				}
			case "C":
				{
					stdBoxes = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
					break;
				}
			case "Z":
				{
					stdBoxes = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 13};
					break;
				}
			default:
				{
					stdBoxes = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14};
					break;
				}
			}

			return stdBoxes;
		}

		private int selectTempOptBoxes()
		{
			int i;

			Console.Write("\n");
			Console.WriteLine($"*** enter the quantity of optional boxes (0 through 9) ***");

			Console.Write("quantity > ");

			string c = Console.ReadKey().KeyChar.ToString().ToUpper();

			Console.WriteLine($"{c}\n");

			bool result = Int32.TryParse(c, out i);

			if (!result) return 0;

			return i;
		}

		private bool selectTempShtQty(out int qty)
		{
			int i = 0;
			qty = -1;

			Console.Write("\n");

			do
			{
				Console.Write("\n");
				Console.WriteLine($"*** enter the quantity of temp sheets (1 through 5) ***");

				Console.Write("quantity > ");

				string c = Console.ReadKey().KeyChar.ToString().ToUpper();

				Console.WriteLine($"{c}");

				if (c.Equals("X")) return false;

				bool result = Int32.TryParse(c, out i);

				if (!result) i = -1;

			}
			while (i <= 0 || i > 5);

			qty = i;

			return true;
		}

		// private static

		private static void sbOptionMainList(int idx)
		{
			sbIdx = idx;

			foreach (KeyValuePair<string, Tuple<string, bool?, bool?, bool?, bool?>> kvp in sbOptionsMain[sbIdx])
			{
				if (kvp.Key.StartsWith('>'))
				{
					Console.WriteLine($"\n{"",-6}+ --- {kvp.Value.Item1} ---");
				}
				else if (kvp.Key.StartsWith('!'))
				{
					Console.WriteLine($"> {kvp.Key.Substring(1),-4}| *** {kvp.Value.Item1} ***");
				}
				else
				{
					Console.WriteLine(
						$"> {kvp.Key,-4}| *** {kvp.Value.Item1}  ({kvp.Value.Item2?.ToString() ?? "null"} / {kvp.Value.Item3?.ToString() ?? "null"} / {kvp.Value.Item4?.ToString() ?? "null"} / {kvp.Value.Item5?.ToString() ?? "null"}) ***");
				}
			}
		}

		private static void sbOptionMinorList(int idx)
		{
			sbIdx = idx;

			foreach (KeyValuePair<string, Tuple<string, bool?>> kvp in sbOptionsMinor[sbIdx])
			{
				if (kvp.Key.StartsWith('>'))
				{
					Console.WriteLine($"\n{"",-6}+ --- {kvp.Value.Item1} ---");
				}
				else if (kvp.Key.StartsWith('!'))
				{
					Console.WriteLine($"> {kvp.Key.Substring(1),-4}| *** {kvp.Value.Item1} ***");
				}
				else
				{
					Console.WriteLine($"> {kvp.Key,-4}| *** {kvp.Value.Item1} ***");
				}
			}

			Console.Write("choose > ");
		}

		private static new Tuple<string, bool?, bool?, bool?, bool?> findSbOptionMain(string selected)
		{
			Tuple<string, bool?, bool?, bool?, bool?> value;

			bool result = sbOptionsMain[sbIdx].TryGetValue(selected, out value);

			return value;
		}

		private static new Tuple<string, bool?> findSbOptionMinor(string selected)
		{
			Tuple<string, bool?> value;

			bool result = sbOptionsMinor[sbIdx].TryGetValue(selected, out value);

			return value;
		}

	// @formatter:off — disable formatter after this line
		
		// first bool? == data file path must exist (item 2 - mustHaveDataFilePath)
		//		null == ignore / na / do not test | true == path must be assigned | false == path should not be assigned (should not occur)
		// second bool? == data file must exist (item 3 - mustHaveDataFile)
		//		null == data file may or may not exist | true == must be found / exist / be loaded | false == cannot be loaded / exist
		// third bool? == data file sheets must exist (item 4 - mustHaveDataFileSheets)
		//		null == ignore / na / do not test | true == not null && must have 1+ sheets | false == not null && must have zero sheets
		// fourth bool? sheet file list must exist (item 5 - mustHaveSheetFileList)
		//		null == ignore / na / do not test | true == folder config'd & must exist & has 1+ files | false == folder not config'd
		private static Dictionary<string, Tuple<string, bool?, bool?, bool?, bool?>>[] sbOptionsMain =
		{
			// main switchboard / idx = 0
			new Dictionary<string, Tuple<string, bool?, bool?, bool?, bool?>>()
			{
    
				//                                                                                        +------------------------- data file path (the path and filename of the xml file)
				//                                                                                        |                             true = the path to the data file is configured and valid
				//                                                                                        |                             false & null = ignore path does not matter
				//                                                                                        |                             
				//                                                                                        |     +------------------- data file (data file opened and read)
				//                                                                                        |     |                       (data file path must be true and pass)
				//                                                                                        |     |                    |  null = sheet count >= 0 if yes, must be 0 if data file shts == false
				//                                                                                        |     |       +--------->  |  true = sheet count >= 0 if yes, if data file shts != null , must have data sheets
				//                                                                                        |     |       |               false = ignore
				//                                                                                        |     |       |                
				//                                                                                        v     v       v               
				//                                                                                       data  data   data  sheet <- sheet file list = true, must have sheet file folder and, if true, must have sheet file list
				//                                                                                       file  file   file  file          not true, ignore
				//                                                                                       path         shts  list
				// { ">01", new Tuple<string, bool?, bool?, bool?, bool?>("original data file options"    , null, null,  null, null) },
				// { "A1" , new Tuple<string, bool?, bool?, bool?, bool?>("Add Tests Original"            , true, false, null, true) },
				// { "R1" , new Tuple<string, bool?, bool?, bool?, bool?>("Remove Sheet (fail - no match)", true, true,  true, false) },
				// { "R3" , new Tuple<string, bool?, bool?, bool?, bool?>("Remove Sheet (good - match)"   , true, true,  true, false) },
				// { "Q"  , new Tuple<string, bool?, bool?, bool?, bool?>("Query orig sheet list"         , true, true,  true, false) },
				// { "O1" , new Tuple<string, bool?, bool?, bool?, bool?>("Open orig sheet types"         , true, true,  null, false) },
				// { "L1" , new Tuple<string, bool?, bool?, bool?, bool?>("List orig sheet types"         , true, true,  true, false) },
				// { "C1" , new Tuple<string, bool?, bool?, bool?, bool?>("Close the orig data file"      , true, true,  null, null) },
				// { "1"  , new Tuple<string, bool?, bool?, bool?, bool?>("Reset the orig list"           , true, true,  null, false) },

				{ ">02", new Tuple<string, bool?, bool?, bool?, bool?>("new list options"              , null, null,  null, null) },

				{ ">11", new Tuple<string, bool?, bool?, bool?, bool?>("Initialize"                    , null, null,  null, null) },
				{ "I2" , new Tuple<string, bool?, bool?, bool?, bool?>("Initialize (using def data)"   , true, null,  null, null) },
				{ "IS" , new Tuple<string, bool?, bool?, bool?, bool?>("Initialize (select data)"      , false, false,  false, false) },

				{ ">06", new Tuple<string, bool?, bool?, bool?, bool?>("Basic"                         , null, null,  null, null) },
				// based on the paths, in the data manager, read the sheet data file
				// if the data manager paths are null, update the data manager 
				// with the sheet file manager paths
				{ "O2" , new Tuple<string, bool?, bool?, bool?, bool?>("Open sheet types"              , true, true,  null, null) },
				// saves the current information then does a "data manager reset"
				{ "C2" , new Tuple<string, bool?, bool?, bool?, bool?>("Close the data file"           , true, true,  null, null) },
				// does not save the current information then resets the data manager
				// resets the current data manager data to blank then does a "close"
				// resets the sheet file manager
				{ "0"  , new Tuple<string, bool?, bool?, bool?, bool?>("Reset full"                    , true, null,  null, null) },
				// does not save the current information then resets the data manager
				// but does not remove the current data manager paths from the sheet file manager
				{ "2"  , new Tuple<string, bool?, bool?, bool?, bool?>("Reset the data manager"        , true, true,  null, null) },
				// resets the current data manager data to blank then does a "close" which
				// does a data manager reset
				{ "4"  , new Tuple<string, bool?, bool?, bool?, bool?>("Reset the data file"           , true, true,  null, null) },
				{ "6"  , new Tuple<string, bool?, bool?, bool?, bool?>("Reset the sheet file manager"  , true, false,  null, null) },
				{ "8"  , new Tuple<string, bool?, bool?, bool?, bool?>("Reset for new sheets"          , true, false,  null, null) },

				{ ">07", new Tuple<string, bool?, bool?, bool?, bool?>("Add"                           , true, null,  null, null) },
				{ "A2" , new Tuple<string, bool?, bool?, bool?, bool?>("Add to empty only"             , true, true, false, false) },
				{ "R2" , new Tuple<string, bool?, bool?, bool?, bool?>("Remove a sheet"                , true, true,  true, null) },


				{ ">10", new Tuple<string, bool?, bool?, bool?, bool?>("Reports"                       , null, null,  null, null) },
				{ "RR" , new Tuple<string, bool?, bool?, bool?, bool?>("Reports Menu"                  , false, false,  false, false) },
				{ "L2" , new Tuple<string, bool?, bool?, bool?, bool?>("List sheet types"              , true, true,  true, null) },

				{ ">09", new Tuple<string, bool?, bool?, bool?, bool?>("new item"                      , null, null,  null, null) },
				{ "AS" , new Tuple<string, bool?, bool?, bool?, bool?>("Add single sheet"              , true, false, false, true) },


				{ ">03", new Tuple<string, bool?, bool?, bool?, bool?>("tests"                         , null, null,  null, null) },

				{ "T0" , new Tuple<string, bool?, bool?, bool?, bool?>("test create samples"           , false, false,  false, null) },

				{ "T1" , new Tuple<string, bool?, bool?, bool?, bool?>("test query sel sheets"         , null, null,  null, null) },
				{ "T2" , new Tuple<string, bool?, bool?, bool?, bool?>("test get config"               , null, null,  null, null) },

				{ ">04", new Tuple<string, bool?, bool?, bool?, bool?>("completion options"            , null, null,  null, null) },
				{ "!X" , new Tuple<string, bool?, bool?, bool?, bool?>("Exit"                          , null, null,  null, null) },
			}
		};

		// note the bool? is not used at the moment
		private static Dictionary<string, Tuple<string, bool?>>[] sbOptionsMinor =
		{
			// list switchboard // idx = 0
			new Dictionary<string, Tuple<string, bool?>>()
			{
				{ "1", new Tuple<string, bool?>("List Names"                       , true) },
				{ "2", new Tuple<string, bool?>("List Basic Info (rectangles)"     , true) },
				{ "3", new Tuple<string, bool?>("List Complete info"               , true) },
				{ "X", new Tuple<string, bool?>("Exit"                             , true) },
			},
			// add test switchboard // idx = 1
			new Dictionary<string, Tuple<string, bool?>>()
			{
				{ "1" , new Tuple<string, bool?>("Add Files (add 4 sheets)"       , true) },
				{ "2" , new Tuple<string, bool?>("Add Files (add existing files)" , true) },
				{ "3" , new Tuple<string, bool?>("Add Files (add bad files)"      , true) },
				{ "4" , new Tuple<string, bool?>("Add Files (add three good file)", true) },
				{ "5" , new Tuple<string, bool?>("Add Files (add file removed"    , true) },
				{ "7" , new Tuple<string, bool?>("Add (3) Special Files"          , true) },
			},
						// list switchboard // idx = 2
			new Dictionary<string, Tuple<string, bool?>>()
			{
				{ "1", new Tuple<string, bool?>("Rect Id report"                   , true) },
				{ "2", new Tuple<string, bool?>("Locations report"                 , true) },
				{ "3", new Tuple<string, bool?>("Classification Files"             , true) },
				{ "4", new Tuple<string, bool?>("Sheet Metric Files"               , true) },
				{ "5", new Tuple<string, bool?>("Scan Samples"                     , true) },
				{ "6", new Tuple<string, bool?>("Assembly Samples"                 , true) },
				{ "X", new Tuple<string, bool?>("Exit"                             , true) },
			},
						// list switchboard // idx = 3 / std boxes
			new Dictionary<string, Tuple<string, bool?>>()
			{
				{ "A", new Tuple<string, bool?>("1 box - Sheet Boundary"           , true) },
				{ "B", new Tuple<string, bool?>("5 boxes - Sht Bdry to Sht Num"    , true) },
				{ "C", new Tuple<string, bool?>("8 boxes - All Std"                , true) },
				{ "Z", new Tuple<string, bool?>("10 boxes - All Std + Bnr1 + Wm1"  , true) },
				{ "X", new Tuple<string, bool?>("Exit"                             , true) },
			},
		};



		// first bool? == data file path must exist (item 2 - mustHaveDataFilePath)
		//		null == ignore / na / do not test | true == path must be assigned | false == path should not be assigned (should not occur)
		// second bool? == data file must exist (item 3 - mustHaveDataFile)
		//		null == data file may or may not exist | true == must be found / exist / be loaded | false == cannot be loaded / exist
		// third bool? == data file sheets must exist (item 4 - mustHaveDataFileSheets)
		//		null == ignore / na / do not test | true == not null && must have 1+ sheets | false == not null && must have zero sheets
		// fourth bool? sheet file list must exist (item 5 - mustHaveSheetFileList)
		//		null == ignore / na / do not test | true == folder config'd & must exist & has 1+ files | false == folder not config'd

		private bool testHasDataFilePath;
		private bool testHasDataFile;
		private bool testHasDataFileSheets;
		private bool testHasSheetFileFolder;
		private bool testHasSheetFileList;

		private int primeIdx;
		private int subIdx;


		private void testGetConfig()
		{
			DM.DbxLineEx(0, "start", 1);

			bool? answer;
			bool result;
			int idx = 0;

			//                                 dataFilePath  dataFile    dataFileSheets   sheetFileList
			runTest(0, "Open / Close / Reset", true,         true,       null,             null);
			runTest(1, "Add"                 , true,         null,       false,            true);
			// runTest(2, "Remove / Query"      , true,         true,       null,             null);

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void runTest(int idx, string title, bool? b1, bool? b2, bool? b3, bool? b4)
		{
			bool? answer;
			bool result;

			primeIdx = idx;

			DM.DbxLineEx(0, "\n\n");
			DM.DbxLineEx(0, $"start test {idx + 1}", 1);
			DM.DbxLineEx(0, $"running {configTests[idx].GetLength(0)} tests");

			for (var i = 0; i < configTests[idx].GetLength(0); i++)
			{
				subIdx = i;

				DM.DbxLineEx(0, "\n");
				DM.DbxLineEx(0, $"start test {idx + 1}.{i}", 1);

				showRequired(b1, b2, b3, b4);

				answer = configTests[idx][i, 0];
				setTestValues(idx, i);
				//                            dataFilePath  dataFile    dataFileSheets    sheetFileList
				result = getConfig2(title, 0, b1,           b2,         b3,               b4);
				showScanReadyStatus(result);

				showTestResult(answer, result);

				DM.DbxLineEx(0, $"end test {idx + 1}.{i}", -1);
			}

			DM.DbxLineEx(0, "\n");
			DM.DbxLineEx(0, $"end test {idx + 1}", -1);
		}

		private void showRequired(bool? b1, bool? b2, bool? b3, bool? b4)
		{
			DM.DbxLineEx(0, $"must have dataFilePath   {b1?.ToString() ?? "null"}");
			DM.DbxLineEx(0, $"must have dataFile       {b2?.ToString() ?? "null"}");
			DM.DbxLineEx(0, $"must have dataFileSheets {b3?.ToString() ?? "null"}");
			DM.DbxLineEx(0, $"must have sheetFileList  {b4?.ToString() ?? "null"}");
			DM.DbxLineEx(0, "\n");
		}


		private void showTestResult(bool? answer, bool result)
		{
			DM.DbxLineEx(0, $"test result| (should be) answer {answer} vs (is) result {result}");

			if (result == answer.Value)
			{
				DM.DbxLineEx(0, "WORKED ***");
			}
			else
			{
				DM.DbxLineEx(0, "FAILED ***");
			}
		}

		private void setTestValues(int idx0, int idx1)
		{
			testHasDataFilePath   = configTests[idx0][idx1, 1];
			testHasDataFile       = configTests[idx0][idx1, 2];
			testHasDataFileSheets = configTests[idx0][idx1, 3];
			testHasSheetFileFolder = configTests[idx0][idx1, 4];
			testHasSheetFileList  = configTests[idx0][idx1, 5];

			DM.DbxLineEx(0, $"test has data file path     {testHasDataFilePath}");
			DM.DbxLineEx(0, $"test has data file          {testHasDataFile}");
			DM.DbxLineEx(0, $"test has data file sheets   {testHasDataFileSheets}");
			DM.DbxLineEx(0, $"test has sheets file folder {testHasSheetFileFolder}");
			DM.DbxLineEx(0, $"test has sheets file list   {testHasSheetFileList}");

			DM.DbxLineEx(0, "\n");
		}


		private bool[][,] configTests = new bool[][,]
		{
			new bool[,] // open etc tests
			{
				//          --- test values ---------------
				//          ans1   ans2   ans3    ans9a  ans9b
				//          data          data    sht    sht
				//          file   data   file    file   file
				//          path   file   shts    foldr  list       // desired outcome + (4) test values
				{ true,	    true,  true , true,   true,  true      , true, true, true, true },
				{ true,	    true,  true , true,   true,  false     , true, true, true, true },
				{ true,	    true,  true , true,   false, true      , true, true, true, true },
				{ true,	    true,  true , true,   false, false     , true, true, true, true },
				{ true,	    true,  true , false,  true,  true      , true, true, true, true },
				{ true,	    true,  true , false,  true,  false     , true, true, true, true },
				{ true,	    true,  true , false,  false, true      , true, true, true, true },
				{ true,	    true,  true , false,  false, false     , true, true, true, true },
				{ false,    true,  false, false,  false, false     , true, false, true, true },
				{ false,    false, true, false,   false, false     , false, true, true, true },
				{ false,    false, false, false,  false, false     , false, true, true, true },
			},
			new bool[,] // add
			{
				{ true,	    true, true  , false, true , true      , true, true, true, true }, // desired outcome + (4) test values


				{ false,    true, true  , true,  true , true      , true, true, false, true },
				{ false,    true, true  , true,  false, false     , true, true, true, false },
				{ false,	true, false , false, true , true      , true, false, true, true },
				{ false,    true, false , false, false, false     , true, false, true, false },

				{ false,    false, false, false, true , true      , false, true, true, true },
			},
			// new bool[,] // remove / query
			// {
			// 	{ true,		true,  true ,  false, false, false     , true, true, true, true}, // desired outcome + (4) test values
			// 	{ true,		true,  true ,  false, true , true     , true, true, true, true},
			// 	{ false,	false, false,  false, false , false     , false, true, true, true},
			// 	{ false,	true,  false, false, false , false     , true, false, true, true},
			// 	{ false,	true,  true ,  true, false , false     , true, true, false, true},
			// },
		};

		// @formatter:on — enable formatter after this line

		// saved version for tests
		private bool getConfig2(  string id, int def,
			bool? mustHaveDataFilePath, bool? mustHaveDataFile, bool? mustHaveDataFileSheets,
			bool? mustHaveSheetFileList)
		{
			DM.DbxLineEx(0, $"start", 1);
			// DM.DbxLineEx(0, $"{mustHaveDataFilePath?.ToString() ?? "null"} | {mustHaveDataFile?.ToString() ?? "null"} | {mustHaveDataFileSheets?.ToString() ?? "null"} | {mustHaveSheetFileList?.ToString() ?? "null"}", 1);

			switchboardIdx = id;

			bool answer1 = true;
			bool answer2 = true;
			bool answer3 = true;
			bool answer9 = true;

			bool answerFinal;

			// so far, always true
			if (mustHaveDataFilePath == true)
			{
				// answer1 = SheetDataManager2.GotDataPath;
				answer1 = testHasDataFilePath;

				if (!answer1)
				{
					DM.DbxLineEx(0, $"ans1 {answer1} vs {configTests[primeIdx][subIdx, 6]}");
					DM.DbxLineEx(0, $"end 1", -1);
					return false;
				}

				// answer1 is true;

				if (!mustHaveDataFile.HasValue) // ie == null
				{
					// answer2 = SheetDataManager2.SheetsCount >= 0;
					answer2 = testHasDataFile;

					if (answer2)
					{
						if (mustHaveDataFileSheets == false)
						{
							// answer3 = SheetDataManager2.SheetsCount == 0;
							answer3 = testHasDataFileSheets == false;
						}
					}
				}
				else if (mustHaveDataFile == true)
				{
					// answer2 = cm.StartDataManager() == true;
					answer2 = testHasDataFile;

					if (answer2)
					{
						if (mustHaveDataFileSheets.HasValue)
						{
							// answer3 = SheetDataManager2.GotDataSheets;
							// answer3 = answer3 == mustHaveDataFileSheets.Value;

							answer3 = testHasDataFileSheets;
						}
					}
				}
			}

			string which = "ans9";

			if (mustHaveSheetFileList == true)
			{
				// answer3 = cm.configSheetPdfScanFolderPath();
				// answer9 = sfm2.GotSheetFolder;
				answer9 = testHasSheetFileFolder;

				which = "ans9a";

				if (answer9)
				{
					// answer9 = cm.configSheetPdfScanFiles();
					answer9 = testHasSheetFileList;

					which = "ans9b";
				}
			}

			// DM.DbxLineEx(0, $"ans1 {answer1} | ans2 {answer2} | ans3 {answer3} | {which} {answer9}");

			DM.DbxLineEx(0, $" (actual) vs (planned)");
			DM.DbxLineEx(0, $"ans1 {answer1} vs {configTests[primeIdx][subIdx, 6]}");
			DM.DbxLineEx(0, $"ans2 {answer2} vs {configTests[primeIdx][subIdx, 7]}");
			DM.DbxLineEx(0, $"ans3 {answer3} vs {configTests[primeIdx][subIdx, 8]}");
			DM.DbxLineEx(0, $"ans9 {answer9} vs {configTests[primeIdx][subIdx, 9]}");


			answerFinal = (answer1) && (answer2) && (answer3) && (answer9);

			// showGetConfigResults(answerFinal, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

			DM.DbxLineEx(0, $"final answer is {answerFinal}");

			DM.DbxLineEx(0, $"end", -1);

			return answerFinal;
		}


		//
		// [DllImport("ole32")]
		// private static extern int CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] string lpszProgID, out Guid lpclsid);
		//
		// [DllImport("oleaut32")]
		// private static extern int GetActiveObject([MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, IntPtr pvReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);
		//
		// public static object GetActiveObject(string progId, bool throwOnError = false)
		// {
		// 	if (progId == null)
		// 		throw new ArgumentNullException(nameof(progId));
		//
		// 	var hr = CLSIDFromProgIDEx(progId, out var clsid);
		// 	if (hr < 0)
		// 	{
		// 		if (throwOnError)
		// 			Marshal.ThrowExceptionForHR(hr);
		//
		// 		return null;
		// 	}
		//
		// 	hr = GetActiveObject(clsid, IntPtr.Zero, out var obj);
		// 	if (hr < 0)
		// 	{
		// 		if (throwOnError)
		// 			Marshal.ThrowExceptionForHR(hr);
		//
		// 		return null;
		// 	}
		// 	return obj;
		// }
		//
		// private static void clrScn()
		// {
		// 	string name = "Immediate Window";
		//
		// 	DTE2 ide = (DTE2) GetActiveObject("VisualStudio.DTE");
		//
		// 	DTE dte = ide.DTE;
		//
		// 	Windows ws;
		//
		// 	ws = ide.Windows;
		// 	// ws = dte.Windows;
		//
		// 	Window w = null;
		//
		// 	for (int i = 1; i < 100; i++)
		// 	{
		// 		try
		// 		{
		// 			w = ide.Windows.Item(i);
		// 		}
		// 		catch 
		// 		{
		// 			Debug.WriteLine($"{i} is no good");
		// 			continue;
		// 		}
		//
		// 		// string k = w.Kind;
		// 		// var t = w.Type;
		// 		string c = w.Caption;
		//
		// 		if (c.Equals(name)) break;
		//
		// 		// Debug.WriteLine($"{i} is {k} of type {t} with caption {c}");
		// 	}
		//
		// 	if (w == null) return;
		//
		// 	OutputWindow ox = (OutputWindow) w.LinkedWindowFrame.;
		//
		// 	w.
		//
		// 	var x = ide.ToolWindows.GetToolWindow(name);
		//
		// 	string tw = x.GetType().Name;
		//
		// 	OutputWindow ow;
		//
		// 	ow = ide.ToolWindows.OutputWindow;
		//
		// 	OutputWindowPane op;
		//
		// 	op = ow.OutputWindowPanes.Add("my pane");
		//
		// 	op.Activate();
		//
		// 	op.OutputString("this is a test string");
		//
		// 	op.Activate();
		//
		// 	// OutputWindow ow = (OutputWindow) 
		// 	// ow.ActivePane.Clear();
		// }

		/*
		private bool initDataManager()
		{
			if (!sm2.StartDataManager())
			{
				DM.DbxLineEx(0, "end 4", 0, -1);
				return false;
			}

			sm2.OpenDataManager();

			return true;
		}
		*/

		/*
		private bool getConfig0(  string id, int def,
			bool? mustHaveDataFilePath, bool? mustHaveDataFile, bool? mustHaveDataFileSheets, bool? mustHaveSheetFileList)
		{
			DM.DbxLineEx(0, $"start ({mustHaveDataFilePath}) / ({mustHaveDataFile}) / ({mustHaveDataFileSheets}) / ({mustHaveSheetFileList})", 1);

			if (mustHaveDataFile != true && mustHaveSheetFileList != true)
			{
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			switchboardIdx = id;

			bool? answer1 = true;
			bool? answer2 = true;
			bool? answer3 = true;
			bool? answer9 = true;

			bool answerFinal;

			// only do configSheetDataFilePath when checking the path
			if (mustHaveDataFilePath == true)
			{
				// answer1 = cm.configSheetDataFilePath();
				answer1 = SheetDataManager2.GotDataPath;

				if (answer1 == true)
				{
					if (mustHaveDataFile == true)
					{
						answer2 = sm2.StartDataManager();

						if (answer2 == true)
						{
							if (mustHaveDataFileSheets.HasValue)
							{
								answer3 = SheetDataManager2.GotDataSheets;
								answer3 = answer3.Value == mustHaveDataFileSheets.Value;
							}
						}
					}
				}
				else
				{
					answer2 = false;
				}
			}

			if (mustHaveSheetFileList == true)
			{
				// answer3 = cm.configSheetPdfScanFolderPath();
				answer9 = sfm2.GotSheetFolder;

				if (answer9 == true)
				{
					answer9 = cm.configSheetPdfScanFiles();
				}
			}

			answerFinal = (answer1 == true) && (answer2 == true) && (answer3 == true) && (answer9 == true);

			showGetConfigResults(answerFinal, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

			DM.DbxLineEx(0, "end", 0, -1);

			return answerFinal;
		}
		*/
	}
}