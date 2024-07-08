using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.VisualStudio.Shell.Interop;
using Settings;
using ShItextCode;
using ShSheetData;
using ShSheetData.SheetData;
using ShSheetData.SheetData2;
using ShTempCode.DebugCode;
using UtilityLibrary;
using ShowWhere = ShTempCode.DebugCode.ShowWhere;
using ScanPDFBoxes.Process;
using ScanPDFBoxes.SheetData;
using SettingsManager;
using ShSheetData.ShSheetData2;
using Constants = EnvDTE.Constants;
using System.Runtime.InteropServices;
using ShCode;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;


namespace ScanPDFBoxes
{
	internal class Program
	{
		// [0] thru [4] == program - [0] show location msg
		// [10] thru [19] = SheetManager - [10] show location msg
		public static int[,] dmx;

		private static int sbIdx;

		// true = worked / good | false = fail / error | null = cannot proceed / config issue
		private static bool? lastOp = null;

		public const string SHEET_METRIC_FOLDER = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\";
		// private static string DataFilePath { get; } = SHEET_METRIC_FOLDER + SheetDataSet.DataFileName;

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

		// public static StatCodes StatusCode { get; private set; }

		public static SheetManager2 sm2;

		private ShSamples ss;

		private static ConfigManager cm;
		private static ProcessManager pm;
		// private static SheetFileManager2 sfm2;

		private int dataSetIdx = 1;
		private string dataSetName = "original";
		private string switchboardIdx;

		private bool mustHaveDataFile = true;

		private string[] files;

		private static Program p;

		private static Guid clsid = new Guid("FBD3B83F-DAC1-431E-9C22-42C3F593620D");
		private static IntPtr unk;

		static void Main(string[] args)
		{
			bool repeat;

			ConsoleFile.Init(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\ReadPDFText\Console.txt");

			DM.configDebugMsgList();

			DM.dmx[0, 0] = 0;
			DM.dmx[0, 1] = (int) ShowWhere.DEBUG;
			DM.dmx[10, 0] = 2;
			DM.dmx[10, 1] = (int) ShowWhere.DEBUG;

			Debug.WriteLine("\n\n");
			Debug.WriteLine("*".Repeat(30));

			DM.DbxLineEx(0, "start");

			p = new Program();

			SheetFileManager2 sfm3 = new SheetFileManager2();

			// sfm2 = new SheetFileManager2();
			sm2 = new SheetManager2(sfm3);

			cm = new ConfigManager(sfm3, sm2);

			pm = new ProcessManager(SHEET_METRIC_FOLDER);
			pm.ConfigSheetData();

			// sm2 = new SheetManager2();
			// p.configSheetDataManager();

			p.ss = new ShSamples();


			DM.DbxLineEx(0, "ready");

			if (lastOp.HasValue)
			{
				Console.Write($"Last operation | {(lastOp.Value ? "worked" : "failed" )}");
			}

			do
			{
				Debug.Write("\n");
				DM.DbxSetIdx(0, 1);
				DM.DbxLineEx(0, "main switchboard"); // reset to level 1

				repeat = p.switchboardMain();
			}
			while (repeat);
		}

		// switchboards
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

				if (selected != null) DM.DbxLineEx(0, $">>> selected | {selected.Item1}  ({selected.Item2})", 1);

				bool? mustHaveDataFilePath = selected?.Item2 ?? null;
				bool? mustHaveDataFile = selected?.Item3 ?? null;
				bool? mustHaveDataFileSheets  = selected?.Item4 ?? null;
				bool? mustHaveSheetFileList  = selected?.Item5 ?? null;

				Console.Write("\n");

				int scanConfigIdxOrig = 2;
				int scanConfigIdxNew = 7;


				switch (c)
				{
				case "A1":
					{
						int choice =  switchBoardTestAdd();

						answer = getConfig(c, choice, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

						showScanReadyStatus(answer == true);
						if (answer != true) break;


						switchAddTestOrig(choice);
						break;
					}
				case "A2":
					{
						dataSetName = "new";

						int choice =  switchBoardTestAdd();

						answer = getConfig(c, choice, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

						showStatus(true, true);

						showScanReadyStatus(answer == true);
						if (answer != true) break;

						switchAddTestNew(choice);

						break;

					}

				case "AA":
					{
						dataSetName = "new";

						// int choice =  switchBoardTestAdd();

						answer = getConfig(c, scanConfigIdxNew, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);
						showStatus(true, true);
						showScanReadyStatus(answer == true);

						if (answer != true) break;

						processAdd1_F_Alt();

						break;

					}

				case "L1":
					{
						answer = getConfig(c, scanConfigIdxOrig, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

						showScanReadyStatus(answer == true);
						if (answer != true) break;

						dataSetIdx = 1;
						switchBoardListTypes();
						break;
					}
				case "L2":
					{
						answer = getConfig(c, scanConfigIdxNew, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

						showScanReadyStatus(answer == true);
						if (answer != true) break;

						dataSetName = "new";
						dataSetIdx = 2;
						switchBoardListTypes();
						break;
					}


				case "O1":
					{
						answer = getConfig(c, scanConfigIdxOrig, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

						showScanReadyStatus(answer == true);
						if (answer != true) break;

						dataSetIdx = 1;
						break;
					}
				case "O2":
					{
						dataSetName = "new";

						answer = getConfig(c, scanConfigIdxNew, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

						if (StatMgr.Current != StatCodes.SC_CFG_DATA_FILE_PATH_MISSING)
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
				case "Q":
					{
						answer = getConfig(c, scanConfigIdxOrig, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

						showScanReadyStatus(answer == true);
						if (answer != true) break;

						// todo fix
						// string s = switchBoardSampleFolders();
						// processQuery(s);
						break;
					}
				case "R1":
					{
						answer = getConfig(c, scanConfigIdxOrig, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

						showScanReadyStatus(answer == true);
						if (answer != true) break;

						processRemove_A();
						break;
					}
				case "R3":
					{
						answer = getConfig(c, scanConfigIdxOrig, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

						showScanReadyStatus(answer == true);
						if (answer != true) break;

						processRemove_B();
						break;
					}

				case "R2":
					{
						dataSetName = "new";

						answer = getConfig(c, scanConfigIdxOrig, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);
						showStatus(true, true);
						showScanReadyStatus(answer == true);
						if (answer != true) break;

						processRemove2();
						break;
					}

				case "C1":
					{
						answer = getConfig(c, scanConfigIdxOrig, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

						showScanReadyStatus(answer == true);
						if (answer != true) break;

						proceedClose1();
						break;
					}
				case "C2":
					{
						dataSetName = "new";

						answer = getConfig(c, scanConfigIdxNew, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

						showScanReadyStatus(answer == true);
						if (answer != true) break;

						proceedClose2();
						break;
					}

				case "1":
					{
						// original reset list
						answer = getConfig(c, -1, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

						showScanReadyStatus(answer == true);
						if (answer != true) break;

						proceedReset1();
						break;
					}
				case "I2":
					{
						dataSetName = "new";

						answer = initialize(c, 0);
						showStatus(true, true);
						showScanReadyStatus(answer == true);
						showInitStatus();
						break;
					}

				case "T2":
					{
						dataSetName = "new";

						testGetConfig();
						break;
					}

				case "2":
					{
						dataSetName = "new";

						// new reset the data manager
						answer = getConfig(c, -1, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);
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

						// new reset the data manager
						answer = getConfig(c, -1, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);
						showStatus(true, true);
						showScanReadyStatus(answer == true);
						if (answer != true) break;

						proceedResetDataFile2();
						break;
					}
				case "6":
					{
						dataSetName = "new";

						// new reset the data manager
						answer = getConfig(c, -1, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);
						showStatus(true, true);
						showScanReadyStatus(answer == true);
						if (answer != true) break;

						proceedFullReset2();
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

			return i;
		}

		private string sbSelecteItem;

		private bool switchboardSelectShtDataFile()
		{
			DM.DbxLineEx(0,"start", 1);

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
				DM.DbxLineEx(0,"end 1", 0, -1);
				return false;
			}
			
			Console.Write("\n\n");

			bool result = Int32.TryParse(c, out i);

			if (result)
			{
				sbSelecteItem = items[i];
			}

			DM.DbxLineEx(0,"end", 0, -1);

			return result;
		}

		private void switchAddTestOrig(int c)
		{
			string selected = sbOptionsMinor[1][c.ToString()].Item1;

			if (selected != null) DM.DbxLineEx(0, $">>> selected | {selected}", 1);

			Console.Write("\n");

			switch (c)
			{
			case 1:
				{
					processAdd_A();
					break;
				}
			case 2:
				{
					processAdd_B();
					break;
				}
			case 3:
				{
					processAdd_C();
					break;
				}
			case 4:
				{
					processAdd_D();
					break;
				}
			case 5:
				{
					processAdd_E();
					break;
				}
			case 7:
				{
					processAdd_F();
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

		private void switchAddSingle(int c)
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
					processAdd1_F_Alt();
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



		// config

		private bool initialize(string id, int def)
		{
			SetStatus(StatCodes.SC_G_NONE);

			DM.DbxLineEx(0, $"start", 1);

			switchboardIdx = id;

			if (!cm.configGetScanInfo(def))
			{
				showStatus(true);
				SetStatus(StatCodes.SC_INIT_GET_PATHS_FAIL);
				DM.DbxLineEx(0, "end 1", 0, -1);
				return false;
			}

			if (cm.configSheetDataFilePath() != true)
			{
				showStatus(true);
				SetStatus(StatCodes.SC_INIT_CFG_DATA_PATH_FAIL);
				DM.DbxLineEx(0, "end 2", 0, -1);
				return false;
			}

			if (cm.configSheetPdfScanFolderPath() != true)
			{
				showStatus(true);
				SetStatus(StatCodes.SC_INIT_CFG_SHT_DATA_PATH_FAIL);
				DM.DbxLineEx(0, "end 3", 0, -1);
				return false;
			}

			if (!sm2.StartDataManager())
			{
				showStatus( true);
				SetStatus(StatCodes.SC_INIT_START_DATA_MGR_FAIL);
				DM.DbxLineEx(0, "end 4", 0, -1);
				return false;
			}

			if (!sm2.LoadSheetFiles())
			{
				showStatus( true);
				SetStatus(StatCodes.SC_INIT_LOAD_SHT_DATA_FILES_FAIL);
				DM.DbxLineEx(0, "end 5", 0, -1);
				return false;
			}

			DM.DbxLineEx(0, "end", 0, -1);

			SetStatus(StatCodes.SC_G_GOOD);

			return true;
		}

		private bool getConfig(  string id, int def,
			bool? mustHaveDataFilePath, bool? mustHaveDataFile, bool? mustHaveDataFileSheets, bool? mustHaveSheetFileList)
		{
			DM.DbxLineEx(0, $"start", 1);

			switchboardIdx = id;

			bool answer1 = true;
			bool answer2 = true;
			bool answer3 = true;
			bool answer9 = true;

			bool answerFinal;

			// so far, always true
			if (mustHaveDataFilePath == true)
			{
				answer1 = sm2.GotDataFilePath && SheetDataManager2.GotDataPath;

				if (!answer1)
				{
					SetStatus(StatCodes.SC_CFG_DATA_FILE_PATH_MISSING);
					DM.DbxLineEx(0, $"end 1", -1);
					return false;
				}

				// answer1 is true;

				// bool b1 = SheetDataManager2.Initialized;
				// bool b2 = SheetDataManager2.GotDataSheets;
				// bool b3 = SheetDataManager2.SettingsFileExists;
				// bool b4 = SheetDataManager2.GotDataPath;
				

				if (!mustHaveDataFile.HasValue) // ie == null
				{
					answer2 = SheetDataManager2.SheetsCount >= 0;

					if (answer2)
					{
						if (mustHaveDataFileSheets == false)
						{
							answer3 = SheetDataManager2.SheetsCount == 0;
						}
					}
				}
				else if (mustHaveDataFile == true)
				{
					// answer2 = cm.StartDataManager() == true;
					answer2 = SheetDataManager2.SheetsCount >= 0;

					if (answer2)
					{
						if (mustHaveDataFileSheets.HasValue)
						{
							answer3 = SheetDataManager2.GotDataSheets;
							answer3 = answer3 == mustHaveDataFileSheets.Value;
						}
					}
				}
			}

			if (mustHaveSheetFileList == true)
			{
				answer9 = sm2.GotSheetFolder;

				if (answer9)
				{
					answer9 = sm2.GotSheetFileList;
				}
			}

			DM.DbxLineEx(0, $"ans1 {answer1} | ans2 {answer2} | ans3 {answer3} | {answer9}");

			answerFinal = answer2 && answer3 && answer9;

			if (answerFinal)
			{
				SetStatus(StatCodes.SC_G_GOOD);
			}
			else
			{
				if (!answer2)
				{
					SetStatus(StatCodes.SC_CFG_DATA_FILE_MISSING);
				}
				else if (!answer3)
				{
					SetStatus(StatCodes.SC_CFG_DATA_FILE_HAS_SHEETS_INVALID);
				}
				else if (!answer9)
				{
					SetStatus(StatCodes.SC_CFG_DATA_FILE_SHEET_LIST_INVALID);
				}
			}

			showGetConfigResults(answerFinal, mustHaveDataFilePath, mustHaveDataFile, mustHaveDataFileSheets, mustHaveSheetFileList);

			DM.DbxLineEx(0, $"end", -1);

			return answerFinal;
		}

		private void showGetConfigResults(bool answer, bool? mustHaveDataFilePath,
			bool? mustHaveDataFile, bool? mustHaveDataFileSheets, bool? mustHaveSheetFileList)
		{
			// string s1 = sfm2.ScanOkToProceed.HasValue ?  sfm2.ScanOkToProceed.Value.ToString() : "is null";
			string s2 = sm2.GotDataFile.HasValue ?  sm2.GotDataFile.Value.ToString() : "is null";
			string s3 = mustHaveDataFile.HasValue ?  mustHaveDataFile.Value.ToString() : "is null";
			string s4 = mustHaveSheetFileList.HasValue ?  mustHaveSheetFileList.Value.ToString() : "is null";
			string s5 = mustHaveDataFilePath.HasValue ?  mustHaveDataFilePath.Value.ToString() : "is null";
			string s6 = mustHaveDataFileSheets.HasValue ?  mustHaveDataFileSheets.Value.ToString() : "is null";


			DM.DbxLineEx(0, $"start", 1);
			DM.DbxLineEx(0, $"option {switchboardIdx}", 1);

			DM.DbxLineEx(0, $"{"get config?",-32}{answer}");
			DM.DbxLineEx(0, $"{"must have data file path",-32}{s5,-8} | got path?   {SheetDataManager2.GotDataPath}");
			DM.DbxLineEx(0, $"{"must have data file?",-32}{s3,-8} | got file?   {s2}");
			DM.DbxLineEx(0, $"{"data file must have sheets?",-32}{s6,-8} | got sheets? {s2} | count {SheetDataManager2.SheetsCount}");

			DM.DbxLineEx(0, $"{"must have sht file path?",-32}{s4,-8} | got path?  {sm2.GotSheetFolder}");
			DM.DbxLineEx(0, $"{"must have sht Files?",-32}{s4,-8} | got files? {sm2.GotSheetFileList}");

			DM.DbxLineEx(0, $"{"got sheet folder?",-32}{sm2.GotSheetFolder}");
			DM.DbxLineEx(0, $"{"got sheet file?",-32}{sm2.GotSheetFileList} ({sm2.SheetFileList?.Count.ToString() ?? "null" })");

			DM.DbxLineEx(0, $"{"got data file?",-32}{s2}");
			DM.DbxLineEx(0, $"{"data file got path?",-32}{SheetDataManager2.GotDataPath}");
			DM.DbxLineEx(0, $"{"data file got sheets?",-32}{SheetDataManager2.GotDataSheets}");
			DM.DbxLineEx(0, $"{UserSettings.Data.DataFilePath?.FullFilePath ?? "data file path is null"}");
			DM.DbxLineEx(0, $"{UserSettings.Data.ScanPDfFolder?.FullFilePath ?? "PDF folder path is null"}", -1);

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void showStatus(bool showFrom = false, bool showOk = false)
		{
			StatMgr.ShowStatus(showFrom, showOk);
		}

		private void SetStatus(StatCodes code,  string note = null,
			[CallerMemberName] string mx = null)
		{
			StatMgr.SetStatCode(code, note, mx);
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
			DM.DbxLineEx(0, "start", 1);		// 1, 1

			int tw = -30;
			int max = 30;

			string s1;
			string fmt = $"{{0,{tw}}}";

			try
			{
				DM.DbxLineEx(0, "sfm2 (SheetFileManager2", 1);   // 1, 1
				DM.DbxLineEx(0, "data manager", 1);   // 2, 3
				// Debug.Write("\n");
				DM.DbxLineEx(0, "status", 1, 1);

				DM.DbxEx(0, $"{(string.Format(fmt, "GotDataFile"))}| {sm2.GotDataFile?.ToString() ?? "null",-10}");
				Debug.WriteLine("(data file path not null & data file path exists)");

				DM.DbxEx(0, $"{(string.Format(fmt, "GotDataFilePath"))}| {sm2.GotDataFilePath,-10}");
				Debug.WriteLine($"(data file path is not null)");

				DM.DbxLineEx(0, "");

				// Debug.Write("\n");
				DM.DbxLineEx(0, "values", -1, 1);

				DM.DbxLineEx(0, $"{(string.Format(fmt, "DataFilePath"))}| {sm2.DataFilePath?.FolderPath ?? "null"}");

				// DM.DbxLineEx(0, "", -1);
				DM.DbxLineEx(0, "");

				// Debug.Write("\n");
				DM.DbxLineEx(0, "sheet file", -2);
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
				DM.DbxLineEx(0, $"{(string.Format(fmt, "SheetFileList"))}| {sm2.SheetFileList?.Count.ToString() ?? "null"}");

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

				DM.DbxEx(0, $"{(string.Format(fmt, "SettingsFileExists"))}| {SheetDataManager2.SettingsFileExists,-10}");
				Debug.WriteLine("(file path configured and file exists)");

				DM.DbxEx(0, $"{(string.Format(fmt, "Admin.Status"))}| {SheetDataManager2.Admin?.Status.ToString() ?? "null",-10}");
				Debug.WriteLine("(internal status)");

				DM.DbxLineEx(0, "");

				// Debug.Write("\n");
				DM.DbxLineEx(0, "values",-1, 1);

				DM.DbxLineEx(0, $"{(string.Format(fmt, "SettingFilePath"))}| {SheetDataManager2.Path?.SettingFilePath ?? "null"}");

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

		private string trunc(string s, int max)
		{
			string s1;

			int len = s.Length;

			int pos = len > max - 3 ? len - (max - 3) : 0;

			s1 = s.Substring(pos);

			if (pos > 0) s1 = "..." + s1;

			return s1;
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

			// init.
			//
			// string idx = switchBoardSampleFolders();
			//
			// configSampleFiles(idx);

			proceedAdd();
		}

		private void processAdd_F_Alt()
		{
			DM.DbxSetIdx(0, 2);
			DM.DbxLineEx(0, "start");

			PdfShowInfo.StartMsg("Add Files F", DateTime.Now.ToString());

			proceedAdd2();

			DM.DbxLineEx(0, "end", 0, -1);
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

			proceedRemove1();
		}

		private void processRemove_B()
		{
			Console.WriteLine("\n*** Remove Files B ***");

			p.getFiles_R2();

			proceedRemove1();
		}

		private void processQuery(string idx)
		{
			// configSampleFiles(idx);

			proceedQuery1();
		}

		private void processRemove2()
		{
			DM.DbxLineEx(0,"start", 1);

			if (!switchboardSelectShtDataFile()) return;

			DM.DbxLineEx(0,$"*** selected for removal {sbSelecteItem}");

			proceedRemove2(sbSelecteItem);

			DM.DbxLineEx(0,"end", 0, -1);
		}

		// action

		// dataset 1

		private void proceedAdd()
		{
			lastOp = pm.ScanSheets(files);
		}

		private void proceedRemove1()
		{
			lastOp = pm.RemoveSheets(files);
		}

		private void proceedQuery1()
		{
			pm.QuerySheets(files);
		}

		private void proceedClose1()
		{
			pm.Close();
		}

		private void proceedReset1()
		{
			pm.Reset();
		}


		// dataset 2

		private void proceedAdd2()
		{
			lastOp = sm2.ScanShts();

			showLastOpResult();
		}

		private void proceedAdd2_1(string file)
		{
			lastOp = sm2.ScanShts();

			showLastOpResult();
		}

		private void proceedOpen2()
		{
			SetStatus(StatCodes.SC_G_NONE);

			lastOp = sm2.OpenDataManager();

			showLastOpResult();
		}

		private void proceedClose2()
		{
			lastOp = sm2.CloseDataManager();

			showLastOpResult();
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

		/// <summary>
		/// run second
		/// configure the paths and file names for the<br/>
		/// output file, pdf folder, etc.<br/>
		/// initialize the sheet data file
		/// </summary>
		// private bool configSheetInfo(string idx)
		// {
		// 	DM.DbxLineEx(0, "start", 1);
		//
		// 	bool? result;
		//
		// 	int index;
		//
		// 	if (!int.TryParse(idx, out index)) return false;
		//
		// 	if (!sm2.InitSheetFiles(index))
		// 	{
		// 		DM.DbxLineEx(0, "end 1 - (", -1);
		// 		return false;
		// 	}
		//
		// 	result = sm2.InitSheetData();
		//
		// 	if (result == false)
		// 	{
		// 		Console.WriteLine("\n*** cannot proceed, the data file is not initialized ***");
		// 		DM.DbxLineEx(0, "end 2 (data file not init)", -1);
		// 		return false;
		// 	}
		// 	else if (!result.HasValue)
		// 	{
		// 		Console.WriteLine("\n*** cannot proceed, sheet data exists ***");
		// 		DM.DbxLineEx(0, "end 3 (sheet data exists)", -1);
		// 		return false;
		// 	}
		//
		// 	DM.DbxLineEx(0, "end", 0, -1);
		//
		// 	return true;
		// }

		// /// <summary>
		// /// run first or only
		// /// config the path / file name for the data manager
		// /// </summary>
		// private bool configSheetDataManager()
		// {
		// 	DM.DbxLineEx(0, "start", 1);
		//
		//
		// 	// if (!sm2.InitDataFile())
		// 	// {
		// 	// 	DM.DbxLineEx(0, "end 2", 0, -1);
		// 	// 	return false;
		// 	// }
		//
		// 	if (!sm2.initDataManager())
		// 	{
		// 		DM.DbxLineEx(0, "end 3", 0, -1);
		// 		return false;
		// 	}
		//
		// 	DM.DbxLineEx(0, "end", 0, -1);
		//
		// 	return true;
		// }

		// get files
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


		// helpers

		private string selectShtDataFileToAdd()
		{

			return "";
		}

		private void showLastOpResult()
		{
			if (lastOp == false)
			{
				Console.WriteLine("\n*** FAILED ***");
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

			if (dataSetIdx == 1)
			{
				ShowSheetRectInfo.showShtRects(ShowWhere.CONSOLE);
			}
			else
			{
				ShowSheetRectInfo.showShtRects2(ShowWhere.CONSOLE);
			}
		}

		private void showRectValues()
		{
			string which = getWhichDataSet();

			ShowSheetRectInfo.StartMsg($"All Rectangle Values ({which})", ShowWhere.CONSOLE, DateTime.Now.ToString());

			if (dataSetIdx == 1)
			{
				ShowSheetRectInfo.ShowValues(ShowWhere.CONSOLE);
			}
			else
			{
				ShowSheetRectInfo.ShowValues2(ShowWhere.CONSOLE);
			}
		}

		private void showSheetNames()
		{
			string which = getWhichDataSet();

			ShowSheetRectInfo.StartMsg($"Sheet Names ({which})", ShowWhere.CONSOLE, DateTime.Now.ToString());

			if (dataSetIdx == 1)
			{
				ShowSheetRectInfo.ShowSheetNames(ShowWhere.CONSOLE);
			}
			else
			{
				ShowSheetRectInfo.ShowSheetNames2(ShowWhere.CONSOLE);
			}
		}


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
				//                                                                                       data  data   data  sheet
				//                                                                                       file  file   file  file
				//                                                                                       path         shts  list
				{ ">01", new Tuple<string, bool?, bool?, bool?, bool?>("original data file options"    , null, null,  null, null) },
				{ "A1" , new Tuple<string, bool?, bool?, bool?, bool?>("Add Tests Original"            , true, false, null, true) },
				{ "R1" , new Tuple<string, bool?, bool?, bool?, bool?>("Remove Sheet (fail - no match)", true, true,  true, false) },
				{ "R3" , new Tuple<string, bool?, bool?, bool?, bool?>("Remove Sheet (good - match)"   , true, true,  true, false) },
				{ "Q"  , new Tuple<string, bool?, bool?, bool?, bool?>("Query orig sheet list"         , true, true,  true, false) },
				{ "O1" , new Tuple<string, bool?, bool?, bool?, bool?>("Open orig sheet types"         , true, true,  null, false) },
				{ "L1" , new Tuple<string, bool?, bool?, bool?, bool?>("List orig sheet types"         , true, true,  true, false) },
				{ "C1" , new Tuple<string, bool?, bool?, bool?, bool?>("Close the orig data file"      , true, true,  null, null) },
				{ "1"  , new Tuple<string, bool?, bool?, bool?, bool?>("Reset the orig list"           , true, true,  null, false) },

				{ ">02", new Tuple<string, bool?, bool?, bool?, bool?>("new list options"              , null, null,  null, null) },
				{ ">05", new Tuple<string, bool?, bool?, bool?, bool?>("General"                       , null, null,  null, null) },

				{ "I2" , new Tuple<string, bool?, bool?, bool?, bool?>("Initialize"                    , true, null,  null, null) },

				{ ">06", new Tuple<string, bool?, bool?, bool?, bool?>("Basic"                         , null, null,  null, null) },
				// based on the paths, in the data manager, read the sheet data file
				// if the data manager paths are null, update the data manager 
				// with the sheet file manager paths
				{ "O2" , new Tuple<string, bool?, bool?, bool?, bool?>("Open sheet types"              , true, true,  null, null) },
				// saves the current information then does a "data manager reset"
				{ "C2" , new Tuple<string, bool?, bool?, bool?, bool?>("Close the data file"           , true, true,  null, null) },
				// does not save the current information then resets the data manager
				// but does not remove the current data manager paths from the sheet file manager
				{ "2"  , new Tuple<string, bool?, bool?, bool?, bool?>("Reset the data manager"        , true, true,  null, null) },
				// resets the current data manager data to blank then does a "close" which
				// does a data manager reset
				{ "4"  , new Tuple<string, bool?, bool?, bool?, bool?>("Reset the data file"           , true, true,  null, null) },
				{ "6"  , new Tuple<string, bool?, bool?, bool?, bool?>("Reset full"                    , null, null,  null, null) },

				{ ">07", new Tuple<string, bool?, bool?, bool?, bool?>("Add"                           , true, null,  null, null) },
				{ "A2" , new Tuple<string, bool?, bool?, bool?, bool?>("Add Tests"                     , true, false, false, true) },
				{ "R2" , new Tuple<string, bool?, bool?, bool?, bool?>("Remove a sheet"                , true, true,  true, null) },

				{ ">08", new Tuple<string, bool?, bool?, bool?, bool?>("Listings"                      , null, null,  null, null) },
				{ "L2" , new Tuple<string, bool?, bool?, bool?, bool?>("List sheet types"              , true, true,  true, null) },

				{ ">09", new Tuple<string, bool?, bool?, bool?, bool?>("new item"                      , null, null,  null, null) },
				{ "AA" , new Tuple<string, bool?, bool?, bool?, bool?>("Add single sheet"              , true, false, false, true) },


				{ ">03", new Tuple<string, bool?, bool?, bool?, bool?>("tests"                         , null, null,  null, null) },

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
			}
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


		
		// saved version for tests
		private bool getConfig2(  string id, int def,
			bool? mustHaveDataFilePath, bool? mustHaveDataFile, bool? mustHaveDataFileSheets, bool? mustHaveSheetFileList)
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