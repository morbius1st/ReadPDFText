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

		private ShSamples ss;

		private static ProcessManager pm;
		private static SheetManager2 sm2;
		private bool sm2Init;
		private int dataSetIdx = 1;
		private string dataSetName = "original";

		private string[] files;

		private static Program p;

		private static Guid clsid = new Guid("FBD3B83F-DAC1-431E-9C22-42C3F593620D");  
		private static IntPtr unk;

		static void Main(string[] args)
		{
			ConsoleFile.Init(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\ReadPDFText\Console.txt");

			DM.configDebugMsgList();

			DM.dmx[0,0] = 0;
			DM.dmx[0,1] = (int) ShowWhere.DEBUG;
			DM.dmx[10,0] = 2;
			DM.dmx[10,1] = (int) ShowWhere.DEBUG;

			Debug.WriteLine("\n\n");
			Debug.WriteLine("*".Repeat(30));

			DM.DbxLineEx(0, "start");

			p = new Program();

			pm = new ProcessManager(SHEET_METRIC_FOLDER);
			pm.ConfigSheetData();

			sm2 = new SheetManager2();
			p.configSheetDataManager();

			p.ss = new ShSamples();

			bool result;

			DM.DbxLineEx(0, "ready");

			if (lastOp.HasValue)
			{
				Console.Write($"Last operation | {(lastOp.Value ? "worked" : "failed" )}");
			}

			do
			{
				Debug.Write("\n");
				// DM.DbxSetIdx(0, 1);
				DM.DbxLineEx(0, "main switchboard"); // reset to level 1

				result = p.switchBoardMain();
			}
			while (result);
		}

		private bool switchBoardMain()
		{
			bool repeat;
			bool result = true;

			Console.Write("\n");
			ShowSheetRectInfo.showStatus(ShowWhere.CONSOLE);
			// Console.Write("\n");
			// Console.WriteLine(">    + ---- original data file options ---");
			// Console.WriteLine("> A  | *** Add Files (add 4 sheets) ***");
			// Console.WriteLine("> B  | *** Add Files (add existing files) ***");
			// Console.WriteLine("> C  | *** Add Files (add bad files) ***");
			// Console.WriteLine("> D  | *** Add Files (add three good file) ***");
			// Console.WriteLine("> E  | *** Add Files (add file removed ***");
			// Console.WriteLine("> F  | *** Add (3) Special Files ***");
			// Console.WriteLine("> R1 | *** Remove Sheet (fail - no match) ***");
			// Console.WriteLine("> R2 | *** Remove Sheet (good - match) ***");
			// Console.WriteLine("> Q  | *** Query orig sheet list ***");
			// Console.WriteLine("> L1 | *** List orig sheet types ***");
			// Console.WriteLine("> 0  | *** Reset the orig list ***");
			// Console.WriteLine("\n>    + ---- new list options ---");
			// Console.WriteLine("> P  | *** test new data file ***");
			// Console.WriteLine("> L2 | *** List new sheet types ***");
			// Console.WriteLine("> 1  | *** Reset the new list ***");
			// Console.WriteLine("\n>    + ---- completion options ---");
			// Console.WriteLine("> X  | Exit");
			// Console.Write("\n");

			sbOptionList(0);

			do
			{
				repeat = false;

				Console.Write("\n> ? > ");

				ConsoleKeyInfo key = Console.ReadKey();

				string c = key.KeyChar.ToString().ToUpper();

				if (c == "R")
				{
					key = Console.ReadKey();
					c += key.KeyChar.ToString();

					Console.Write("\n");
				}

				if (c == "L")
				{
					key = Console.ReadKey();
					c += key.KeyChar.ToString();

					Console.Write("\n");
				}

				if (c == "C")
				{
					key = Console.ReadKey();
					c += key.KeyChar.ToString();

					Console.Write("\n");
				}

				if (c == "O")
				{
					key = Console.ReadKey();
					c += key.KeyChar.ToString();

					Console.Write("\n");
				}

				if (c == "A")
				{
					key = Console.ReadKey();
					c += key.KeyChar.ToString();

					Console.Write("\n");
				}



				dataSetName = "original";
				string selected = findSbOption(c);

				if (selected!=null) DM.DbxLineEx(0, $">>> selected | {selected}", 1);

				Console.Write("\n");

				switch (c)
				{
				case "A1":
					{
						string choice =  switchBoardTestAdd();
						switchAddTestOrig(choice);
						break;
					}
				case "A2":
					{
						dataSetName = "new";
						string choice =  switchBoardTestAdd();
						switchAddTestNew(choice);
						break;
					}
				// case "P":
				// 	{
				// 		dataSetName = "new";
				// 		processAdd_F_Alt();
				// 		break;
				// 	}
				case "L1":
					{
						dataSetIdx = 1;
						switchBoardListTypes();
						break;
					}
				case "L2":
					{
						dataSetName = "new";
						dataSetIdx = 2;
						switchBoardListTypes();
						break;
					}
				case "O1":
					{
						dataSetIdx = 1;
						break;
					}
				case "O2":
					{
						dataSetName = "new";
						dataSetIdx = 2;
						break;
					}
				case "Q":
					{
						string s = switchBoardSampleFolders();
						processQuery(s);
						break;
					}
				case "R1":
					{
						processRemove_A();
						break;
					}
				case "R2":
					{
						processRemove_B();
						break;
					}
				case "X":
					{
						result = false;
						break;
					}
				case "C1":
					{
						proceedClose1();
						break;
					}
				case "C2":
					{
						proceedClose2();
						break;
					}
				case "0":
					{
						proceedReset1();
						break;
					}
				case "1":
					{
						dataSetName = "new";
						proceedReset2();
						break;
					}
				default:
					{
						Console.WriteLine("\n*****************\nInvalid Entry\n***************\n");
						repeat = true;
						break;
					}
				}

				if (selected !=null) DM.DbxLineEx(0, $"<<< selected | {selected}", -1);
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

			sbOptionList(1);

			ConsoleKeyInfo key = Console.ReadKey();
			Console.Write("\n\n");

			string selected = findSbOption(key.KeyChar.ToString());

			if (selected!=null) DM.DbxLineEx(0, $">>> selected | {selected}", 1);

			if (key.KeyChar == '1') showSheetNames();
			if (key.KeyChar == '2') showBasicRects();
			if (key.KeyChar == '3') showRectValues();

			if (selected !=null) DM.DbxLineEx(0, $"<<< selected | {selected}", -1);
		}

		private string switchBoardTestAdd()
		{
			Console.Write("\n");
			Console.WriteLine($"*** {dataSetName} data set ***");

			sbOptionList(2);

			return Console.ReadKey().KeyChar.ToString().ToUpper();
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

		private void switchAddTestOrig(string c)
		{
			
			string selected = findSbOption(c);

			if (selected!=null) DM.DbxLineEx(0, $">>> selected | {selected}", 1);

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
			default:
				{
					Console.WriteLine("\n*****************\nInvalid Entry\n***************\n");
					break;
				}
			}

			Console.Write("\n");

			if (selected !=null) DM.DbxLineEx(0, $"<<< selected | {selected}", -1);
		}

		private void switchAddTestNew(string c)
		{
			
			string selected = findSbOption(c);

			if (selected!=null) DM.DbxLineEx(0, $">>> selected | {selected}", 1);

			Console.Write("\n");

			switch (c)
			{
			case "A":
				{
					Console.WriteLine("not implemented");
					DM.DbxLineEx(0,"\toption A is not implemented");
					break;
				}
			case "B":
				{
					Console.WriteLine("not implemented");
					DM.DbxLineEx(0,"\toption B is not implemented");
					break;
				}
			case "C":
				{
					Console.WriteLine("not implemented");
					DM.DbxLineEx(0,"\toption C is not implemented");
					break;
				}
			case "D":
				{
					Console.WriteLine("not implemented");
					DM.DbxLineEx(0,"\toption D is not implemented");
					break;
				}
			case "E":
				{
					Console.WriteLine("not implemented");
					DM.DbxLineEx(0,"\toption E is not implemented");
					break;
				}
			case "F":
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

			if (selected !=null) DM.DbxLineEx(0, $"<<< selected | {selected}", -1);
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

		private void processAdd_F_Alt()
		{
			DM.DbxSetIdx(0, 2);
			DM.DbxLineEx(0, "start");


			// process
			// config files - if good - continue
			// config sheetdatamanager - if good - continue

			string idx = switchBoardSampleFolders();

			if (!configSheetInfo(idx))
			{
				DM.DbxLineEx(0, "end 2", -1);
				return;
			}

			PdfShowInfo.StartMsg("Add Files F", DateTime.Now.ToString());

			processAdd2();

			DM.DbxLineEx(0, "end", -1);
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
			configSampleFiles(idx);

			proceedQuery1();
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

		private void processAdd2()
		{
			lastOp = sm2.ScanShts();

			showLastOpResult();
		}


		private void proceedClose2()
		{
			lastOp = sm2.Close();

			showLastOpResult();
		}

		/// <summary>
		/// reset the data file so that new data can be added but<br/>
		/// do not change the paths or filenames
		/// </summary>
		private void proceedReset2()
		{
			DM.DbxLineEx(0, "start");

			sm2.ResetShtData();
			// sm2.Reset();

			// do not do this - this is only
			// to be used when switching to a different
			// data file
			// if (!configSheetDataManager())
			// {
			// 	DM.DbxLineEx(0, "end 1", -1);
			// }

			// sm2.ResetSheetData();

			DM.DbxLineEx(0, "end", -1);
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
		

		/// <summary>
		/// run second
		/// configure the paths and file names for the<br/>
		/// output file, pdf folder, etc.<br/>
		/// initialize the sheet data file
		/// </summary>
		private bool configSheetInfo(string idx)
		{
			DM.DbxLineEx(0, "start", 1);

			bool? result;

			int index;

			if (!int.TryParse(idx, out index)) return false;

			if (!sm2.InitSheetFiles(index))
			{
				DM.DbxLineEx(0, "end 1 - (", -1);
				return false;
			}

			result = sm2.InitSheetData();

			if (result == false)
			{
				Console.WriteLine("\n*** cannot proceed, the data file is not initialized ***");
				DM.DbxLineEx(0, "end 2 (data file not init)", -1);
				return false;
			} 
			else 
			if (!result.HasValue)
			{
				Console.WriteLine("\n*** cannot proceed, sheet data exists ***");
				DM.DbxLineEx(0, "end 3 (sheet data exists)", -1);
				return false;
			}

			DM.DbxLineEx(0, "end", -1);

			return true;
		}


		/// <summary>
		/// run first or only
		/// config the path / file name for the data manager
		/// </summary>
		private bool configSheetDataManager()
		{
			DM.DbxLineEx(0, "start", 1);

			if (sm2Init)
			{
				DM.DbxLineEx(0, "end 1", -1);
				return true;
			}

			if (!sm2.InitDataFile())
			{
				DM.DbxLineEx(0, "end 2", -1);
				return false;
			}

			if (!sm2.initDataManager())
			{
				DM.DbxLineEx(0, "end 3", -1);
				return false;
			}

			sm2Init = true;

			DM.DbxLineEx(0, "end", -1);

			return true;
		}


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

		private void showLastOpResult()
		{
			if (lastOp == false)
			{
				Console.WriteLine("\n*** FAILED ***");
			}
			else
			if (lastOp == true)
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


		private static void sbOptionList(int idx)
		{
			sbIdx = idx;

			foreach (KeyValuePair<string, string> kvp in sbOptions[sbIdx])
			{
				if (kvp.Key.StartsWith('>'))
				{
					Console.WriteLine($"\n{"",-6}+ --- {kvp.Value} ---");
				}
				else
				{
					Console.WriteLine($"> {kvp.Key,-4}| *** {kvp.Value} ***");
				}
			}
		}

		private static string findSbOption(string selected)
		{
			string value;
			bool result = sbOptions[sbIdx].TryGetValue(selected, out value);

			return value;
		}


		private static Dictionary<string, string>[] sbOptions = new []
		{
			// main switchboard / idx = 0
			new Dictionary<string, string>()
			{
				{">01", "original data file options" },
				{ "A1" , "Add Tests Original" },
				{ "R1", "Remove Sheet (fail - no match)" },
				{ "R2", "Remove Sheet (good - match)" },
				{ "Q" , "Query orig sheet list" },
				{ "O1", "Open orig sheet types" },
				{ "L1", "List orig sheet types" },
				{ "C1", "Close the orig data file" },
				{ "0" , "Reset the orig list" },
				{">02", "new list options" },
				{ "A2" , "Add Tests new" },
				{ "L2", "List new sheet types" },
				{ "O2", "Open new sheet types" },
				{ "C2" , "Close the new data file" },
				{ "1" , "Reset the new list" },
				{">03", "completion options" },
				{ "X" , "Exit" },
			},
			// list switchboard // idx = 1
			new Dictionary<string, string>()
			{
				{"1", "List Names" },
				{"2", "List Basic Info (rectangles)" },
				{"3", "List Complete info" },
			},
			// add test switchboard // idx = 2
			new Dictionary<string, string>()
			{
				{ "A" , "Add Files (add 4 sheets)" },
				{ "B" , "Add Files (add existing files)" },
				{ "C" , "Add Files (add bad files)" },
				{ "D" , "Add Files (add three good file)" },
				{ "E" , "Add Files (add file removed" },
				{ "F" , "Add (3) Special Files" },
			}
		};



				

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

	}
} 