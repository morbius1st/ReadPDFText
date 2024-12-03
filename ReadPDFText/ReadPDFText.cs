using CommonPdfCodeShCode;

using iText.Kernel.Geom;

using Microsoft.WindowsAPICodePack.Dialogs;
using SettingsManager;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using ReadPDFText.Process;
using SharedCode.ShDataSupport.ExcelSupport;
using SharedCode.ShDataSupport.PdfTree;
using SharedCode.ShDataSupport.Process;
using SharedCode.ShDataSupport.ScheduleListSupport;
using UtilityLibrary;


namespace SharedCode.ShDataSupport
{

	public interface IPdfTextXX
	{
		string[] Elt { get; }

		int Mergedone { get; }
		int Validatedone { get; }
		int Bookmarksdone { get; }

		List<PdfTreeLeaf> RotFailList { get; }
	}


	class ReadPDFText
	{
		// [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		// private static extern bool GetOpenFileName(ref OpenFileName ofn);
		//
		// [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		// private static extern bool SHBrowseForFolderA();

		[Flags]
		public enum ValidateCodes
		{
			None       = 0,
			HasMissing = 1 << 0,
			HasFound   = 1 << 1,
			HasDups    = 1 << 2,
		}


		private string pdfFolder_settings = "c:\\";
		private string xlsxFolder_settings = "c:\\";
		private string xlsxFile_settings = "file.xlsx";
		private string destFolder_settings = "c:\\";



		// private static int  test = 11;
		private static int  test = 303;


		public const string XL_FILE_EXTN = ".xlsx";
		public const string PDF_FILE_EXTN = ".pdf";


		public const float PH = 11.0F; // page height
		public const float PW = 8.5F; // page width

		private const string DEST_FILE = "Combined.pdf";
		const string DEST = @"..\..\..\target\modified.pdf";

		const string SRC1 = @"..\..\..\src\hello.pdf";
		const string SRC2 = @"..\..\..\src\links1.pdf";
		const string SRC3 = @"..\..\..\src\links2.pdf";

		// const string SRC_LOC = @"..\..\..\src2";
		const string SRC2_LOC = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\ReadPDFText\src2";
		const string SRCT1 = @"test1-1.pdf";
		const string SRCT2 = @"test1-2.pdf";
		const string SRCT3 = @"test1-3.pdf";
		const string SRCT4 = @"test1-4.pdf";

		private const string SRCR3a = "Cx.pdf";
		const string SRC3_LOC = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\ReadPDFText\src3";

		const string SRC4_LOC = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\ReadPDFText\src4";
		private const string SRCR4a = "ax.pdf";

		const string SRC102_LOC = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\ReadPDFText\src5";
		private const string SRCR102a = "Cx.pdf";

		private const string SHEET_METRICS_FILE = "sheetmetrics.xml";
		private const string TEMP_CONFIG_FILE = "PdfAssemblerSettings.xlsx";

		public Info info;
		// public PdfText1 pt1;
		// public PdfText2 pt2;
		// public PdfText3 pt3;
		// public PdfText4 pt4;
		// public PdfText5 pt5;
		public PdfText10 pt10;
		public PdfText11 pt11;
		public PdfText12 pt12;
		public PdfText301 pt301;
		public PdfText302 pt302;
		public PdfText303 pt303;

		// public PdfRotate1 pr1;
		// public PdfRotate2 pr2;

		// public PdfTest1 ptst1;

		private static PdfNodeTree tree;

		private static CommonOpenFileDialog cfd;

		private static DataManager<DataStorageSet> dm1;
		private static DataManager<DataStorageSet> dm2;

		private static string SettingsFolderPath;
		private static string SheetDataFilePath;
		private static FilePath<FileNameSimple> dataFile;

		private bool selectFiles = true;

		public static ExcelManager xlMgr;
		private static ScheduleListManager schMgr;
		private static ValidateFilesInFolder validate;
		private static MakePdfTree mkTree;
		private static PdfAssemblerSettingSupport pass;

		public static Dictionary<SheetBorderType, SheetData> ShtData => dm2.Data.SheetData;



		// located here:
		// ScheduleListManager
		public static int maxFilesToCombine = 30;

		[STAThread]
		static void Main(string[] args)
		{
			Console.WriteLine("starting");

			string answer = null;
			int count = 3;

			ReadPDFText me = new ReadPDFText();
			me.info = new Info();

			UserSettings.Admin.Read();

			string assemblyPath = CsUtilities.AssemblyDirectory;

			SettingsFolderPath = UserSettings.Admin.Path.SettingFolderPath;
			// SheetDataFilePath = SettingsFolderPath + "\\" + SHEET_METRICS_FILE;
			SheetDataFilePath = assemblyPath + "\\" + SHEET_METRICS_FILE;
			dataFile = new FilePath<FileNameSimple>(SheetDataFilePath);

			cfd = new CommonOpenFileDialog();

			
			
			// Console.WriteLine("got user name");

/*
			if (test == 0)
			{
				for (int i = 0; i < count; i++)
				{
					me.GetTextFromPDF();
				}
			} 
			else if (test == 20)
			{
				me.readTextFromPdf();
			} 
			else if (test == 1)
			{
				me.manipulatePdf(DEST);
			}
			else if (test == 2)
			{
				me.makeHyperLinks(); 
			}
			else if (test == 3)
			{
				me.readPdfContents(); 
			}
			else if (test == 4)
			{
				me.readPdfContents4(); 
			}
			// else if (test == 5)
			// {
			// 	me.readPdfContents5(); 
			// }
			else if (test == 6)
			{
				me.readPdfContents6(); 
			}
			else if (test == 101)
			{
				me.rotatePdf1(); 
			}
			else if (test == 102)
			{
				me.rotatePdf2(); 
			}
			else if (test == 201)
			{
				me.testPdf1(); 
			}

			else 

*/

			if (test == 10)
			{
				getUserName();

				me.readSheetData();
				me.readPdfContents10(); 
			}
			else 
			if (test == 11)
			{
				getUserName();

				try
				{
					me.readSheetData();
					me.readPdfContents11();
				}
				catch (Exception e)
				{
					StringBuilder sb = new StringBuilder();

					sb.AppendLine(e.Message);
					sb.AppendLine(e.StackTrace);

					Console.WriteLine("\nfail\n");
					Console.WriteLine(sb.ToString());
					

					MessageBoxResult mr = MessageBox.Show("waiting", "Error");
				}
			}
			else 
			if (test == 12)
			{
				try
				{
					me.readSheetData();
					me.readPdfContents12();
				}
				catch (Exception e)
				{
					StringBuilder sb = new StringBuilder();

					sb.AppendLine(e.Message);
					sb.AppendLine(e.StackTrace);

					Console.WriteLine("\nfail\n");
					Console.WriteLine(sb.ToString());
					

					MessageBoxResult mr = MessageBox.Show("waiting", "Error");
				}
			}
			else 
			if (test == 301)
			{
				me.readSheetData();
				me.readPdfContents301();

			}
			else 
			if (test == 302)
			{
				me.readSheetData();
				me.readPdfContents302();

			}
			else 
			if (test == 303)
			{
				me.readSheetData();
				me.readPdfContents303();

			}
			else 
			if (test == 20)
			{
				CommonFileDialogFilter filter = null;

				Console.WriteLine("run test 20 ->CFD research");

				string file = me.getFileCfd("Get a File", @"c:\", 
					new CommonFileDialogFilter("Sheet Schedules", ".xlsx"), "file.xlsx");

				Console.WriteLine(file);

				file = me.getFolderCfd("Get a File", @"c:\");

				Console.WriteLine(file);

				cfd.Dispose();
			}
			else 
			if (test == 30)
			{
				me.test30();
			}
			else 
			if (test == 202)
			{
				Console.WriteLine("run test 202"); 
				me.writeSheetData();
				me.readSheetData();
			}
			else if (test == 100)
			{
				PdfText100 p100 = new PdfText100();
				p100.ProcessPdf();
			}

			else 
			if (test == 101)
			{
				PdfText101 p101 = new PdfText101();
				p101.ProcessPdf();
			}
			else 
			if (test == 201)
			{
				Console.WriteLine("run test 201 - trim test"); 
				me.test201();
			}

			Console.Write("press enter to continue| "); 
			answer = Console.ReadLine();
		}

		

		public static string UserName { get; set; }
		
		public static FilePath<FileNameSimple> pdfFolder { get; set; }
		public static FilePath<FileNameSimple> xlsxFilePath { get; set; }
		public static FilePath<FileNameSimple> destFolderPath { get; set; }
		public static FilePath<FileNameSimple> destFilePath { get; set; }
		public static FilePath<FileNameSimple> configSettingFilePath { get; set; }


		private void test201()
		{
			string result;

			result = " ".Trim();
			Console.WriteLine($"       trim one space | >{result ?? "null"}<");

			result = "  ".Trim();
			Console.WriteLine($"       trim two spaces| >{result ?? "null"}<");

			result = " A".Trim();
			Console.WriteLine($"     trim space + char| >{result ?? "null"}<");;

			result = "A ".Trim();
			Console.WriteLine($"     trim char + space| >{result ?? "null"}<");

			
			result = " ".TrimStart();
			Console.WriteLine($"  trimStart one space | >{result ?? "null"}<");

			result = "  ".TrimStart();
			Console.WriteLine($"  trimStart two spaces| >{result ?? "null"}<");

			result = " A".TrimStart();
			Console.WriteLine($"trimStart space + char| >{result ?? "null"}<");;

			result = "A ".TrimStart();
			Console.WriteLine($"trimStart char + space| >{result ?? "null"}<");


		}

		private void test30()
		{

			// pdfFolder = new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Coliseum");
			xlsxFilePath = new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Coliseum Sheet List.xlsx");
			// destFilePath = new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Combined.pdf");
			// destFolderPath = new FilePath<FileNameSimple>(destFilePath.FolderPath);

			string xlsxFolder = xlsxFilePath.FolderPath;
			configSettingFilePath = new FilePath<FileNameSimple>(new [] { xlsxFolder, TEMP_CONFIG_FILE });
			
			PdfAssemblerSettingSupport pass = new PdfAssemblerSettingSupport(configSettingFilePath);
			pass.Process();
		}

		private void readPdfContents10()
		{
			bool result = true;

			result = getFoldersAndFiles();

			// if (result)
			// {
			// 	result = readSchedule();
			// }
			//
			// if (!result)
			// {
			// 	Console.WriteLine("\n*** FAILED ***");
			// 	Console.WriteLine("Exiting");
			// }
			//
			// Console.WriteLine("\n*** WORKED ***");
			// Console.WriteLine("Continuing");


			// List<SampleData> sampleData = SampleSupport.fileNameListShort;

			// Debug.WriteLine("\n\nshow info\n");
			// SampleSupport.showSampleDataLong();

			// the short list
			// **** active
			tree = SampleSupport.MakePdfTree();

			// the long list
			// tree = SampleSupport.MakePdfTree2();



			//
			// *** active
			SampleSupport.showPdfNodeTree(tree);
			// SampleSupport.ShowPdfNodeTreeInOrder();

			return;

			SampleSupport.fileLocation = pdfFolder.FullFilePath;

			pt10 = new PdfText10();
			pt10.UpdateConfig();

			Stopwatch sw = Stopwatch.StartNew();

			result = pt10.AddLinks(tree, destFilePath.FullFilePath);

			sw.Stop();

			if (result)
			{
				Console.WriteLine("*** Worked ***");
			}
			else
			{
				Console.WriteLine("*** Failed ***");
				processFailCode(PdfText10.FailCode);
			}

			if (PdfText10.RotationFailList.Count > 0)
			{
				Console.WriteLine("The rotation of some sheets could not be corrected:");

				foreach (PdfTreeLeaf leaf in PdfText10.RotationFailList)
				{
					Console.WriteLine(leaf.File.FileName);
				}
			}

			if (PdfText10.ValidationFailList.Count > 0)
			{
				Console.WriteLine("The sheet number on some sheets could not be validated:");

				foreach (PdfTreeLeaf leaf in PdfText10.ValidationFailList)
				{
					Console.WriteLine(leaf.File.FileName);
				}
			}

			Console.WriteLine("PDF Assembler Complete (created by CyberStudio / Jeff Stuyvesant");

		}

		private void readPdfContents11()
		{
			Debug.WriteLine("\n**** running test 11 ****\n");


			ValidateCodes outCode;
			bool result = true;

			xlMgr = new ExcelManager();
			validate = new ValidateFilesInFolder();

			selectFiles = true;

			// the complete list (331 total)
			// selectFiles = false;
			// pdfFolder = new FilePath<FileNameSimple>(@"C:\Users\jeffs\OneDrive\Office\2021-073 Simon Brea\Files for hyperlink");
			// xlsxFilePath = new FilePath<FileNameSimple>(@"C:\Users\jeffs\OneDrive\Office\2021-073 Simon Brea\{Simon Brea} Sheet List_04102024 -use.xlsx");
			// destFilePath = new FilePath<FileNameSimple>(@"C:\Users\jeffs\OneDrive\Office\2021-073 Simon Brea\Combined.pdf");
			// destFolderPath = new FilePath<FileNameSimple>(destFilePath.FolderPath);
			// configSettingFilePath = new FilePath<FileNameSimple>(new [] { UserSettings.Data.xlsxFolder, TEMP_CONFIG_FILE });

			// test files (22 total)
			// selectFiles = false;
			// pdfFolder = new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Simon Brea\PDF Files");
			// xlsxFilePath = new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Simon Brea\{Simon Brea} Sheet List_04102024.xlsx");
			// destFilePath = new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Simon Brea\Combined.pdf");
			// destFolderPath = new FilePath<FileNameSimple>(destFilePath.FolderPath);
			// configSettingFilePath = new FilePath<FileNameSimple>(new [] { UserSettings.Data.xlsxFolder, TEMP_CONFIG_FILE });

			// coliseum (242 total)
			// selectFiles = false;
			// pdfFolder = new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Coliseum");
			// xlsxFilePath = new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Coliseum Sheet List.xlsx");
			// destFilePath = new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Combined.pdf");
			// destFolderPath = new FilePath<FileNameSimple>(destFilePath.FolderPath);
			// configSettingFilePath = new FilePath<FileNameSimple>(new [] {xlsxFilePath.FolderPath, TEMP_CONFIG_FILE });

			// test2
			// string baseFolder = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test2";
			// pdfFolder = new FilePath<FileNameSimple>(baseFolder);
			// xlsxFilePath = new FilePath<FileNameSimple>(baseFolder +"\\" + @"\Test2 Sheet List.xlsx");
			// destFilePath = new FilePath<FileNameSimple>(baseFolder +"\\" + @"Combined.pdf");
			// destFolderPath = new FilePath<FileNameSimple>(baseFolder);
			// configSettingFilePath = new FilePath<FileNameSimple>(new [] { baseFolder, TEMP_CONFIG_FILE });

			
			// test files (5 total)
			// selectFiles = false;
			// string baseFolder = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test3\";
			// string pdfFolderName = "PDF Files";
			// string sheetFileName = "Sheet List.xlsx";
			// string destFileName = "Combined.pdf";


			// pdfFolder = new FilePath<FileNameSimple>(baseFolder+pdfFolderName);
			// xlsxFilePath = new FilePath<FileNameSimple>(baseFolder+sheetFileName);
			// destFilePath = new FilePath<FileNameSimple>(baseFolder+destFileName);
			// destFolderPath = new FilePath<FileNameSimple>(destFilePath.FolderPath);
			// configSettingFilePath = new FilePath<FileNameSimple>(new [] {xlsxFilePath.FolderPath, TEMP_CONFIG_FILE });



			// test files (5 total)
			selectFiles = false;
			string baseFolder = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test4\";
			// string pdfFolderName = "PDF Files";
			string sheetFileName = "Sheet List.xlsx";
			string destFileName = "Combined.pdf";

			pdfFolder = new FilePath<FileNameSimple>(baseFolder);
			xlsxFilePath = new FilePath<FileNameSimple>(baseFolder+sheetFileName);
			destFilePath = new FilePath<FileNameSimple>(baseFolder+destFileName);
			destFolderPath = new FilePath<FileNameSimple>(destFilePath.FolderPath);
			configSettingFilePath = new FilePath<FileNameSimple>(new [] {xlsxFilePath.FolderPath, TEMP_CONFIG_FILE });

			result = getFoldersAndFiles();

			if (!result)
			{
				Console.WriteLine("\n*** FAIL ***");
				Console.WriteLine("Did not get correct file and / or folder");
				Console.WriteLine("Exiting");
				return;
			}

			result = readSchedule();

			if (!result)
			{
				Console.WriteLine("\n*** FAIL ***");
				Console.WriteLine("Could not read Sheet Schedule");
				Console.WriteLine("Exiting");
				return;
			}
			
			result = validateFilesLists(out outCode);
			if (!result)
			{
				bool hasMissing = (outCode & ValidateCodes.HasMissing) == ValidateCodes.HasMissing;
				bool hasFound = (outCode & ValidateCodes.HasFound) == ValidateCodes.HasFound;
				bool hasDups = (outCode & ValidateCodes.HasDups) == ValidateCodes.HasDups;

				if (hasFound)
				{
					Console.WriteLine("continue - ignore extra found PDFs");
				}

				if (hasMissing || hasDups)
				{
					Console.WriteLine("\n*** FAIL ***");
					if (hasDups) Console.WriteLine("continue - ignore duplicate PDFs");
					if (hasMissing) Console.WriteLine("cannot continue - there are missing PDFs");
					Console.WriteLine("Exiting");
					return;
				}
			}


			PdfAssemblerSettingSupport pass = new PdfAssemblerSettingSupport(configSettingFilePath);
			
			result = pass.Process();
			if (!result)
			{
				Console.WriteLine("\n*** FAIL ***");
				Console.WriteLine("Could not read config settings");
				Console.WriteLine("Exiting");
				return;
			}

			// Console.WriteLine("\n*** WORKED ***");
			// Console.WriteLine("Continuing");

			mkTree = new MakePdfTree(schMgr, ShtData);
			result = mkTree.MakeTree();

			if (!result)
			{
				Console.WriteLine("\n*** FAIL ***");
				Console.WriteLine("\n*** Could not create the PDF Tree ***");
				Console.WriteLine("Exiting");
				return;
			}

			pt11 = new PdfText11();
			pt11.UpdateConfig(pass);

			Stopwatch sw = Stopwatch.StartNew();

			Debug.WriteLine("start add links");

			result = pt11.AddLinks(mkTree.Tree, destFilePath.FullFilePath);

			sw.Stop();

			if (result)
			{
				Console.WriteLine("*** Worked ***");
			}
			else
			{
				Console.WriteLine("*** Failed ***");
				processFailCode(PdfText11.FailCode);
			}

			if (PdfText11.RotationFailList.Count > 0)
			{
				Console.WriteLine("The rotation of some sheets could not be corrected:");

				foreach (PdfTreeLeaf leaf in PdfText11.RotationFailList)
				{
					Console.WriteLine(leaf.File.FileName);
				}
			}

			if (PdfText11.ValidationFailList.Count > 0)
			{
				Console.WriteLine("The sheet number on some sheets could not be validated:");

				foreach (PdfTreeLeaf leaf in PdfText11.ValidationFailList)
				{
					Console.WriteLine(leaf.File.FileName);
				}
			}

			Console.WriteLine("PDF Assembler Complete (created by CyberStudio / Jeff Stuyvesant");

		}

		private void readPdfContents12()
		{
			Debug.WriteLine("\n**** running process 12 ****\n");

			ValidateCodes outCode;
			bool result = true;

			xlMgr = new ExcelManager();
			validate = new ValidateFilesInFolder();


			result = getFoldersAndFiles(false, 7);

			if (!result)
			{
				Console.WriteLine("\n*** FAIL ***");
				Console.WriteLine("Did not get correct file and / or folder");
				Console.WriteLine("Exiting");
				return;
			}

			result = readSchedule();

			if (!result)
			{
				Console.WriteLine("\n*** FAIL ***");
				Console.WriteLine("Could not read Sheet Schedule");
				Console.WriteLine("Exiting");
				return;
			}
			
			result = validateFilesLists(out outCode);
			if (!result)
			{
				bool hasMissing = (outCode & ValidateCodes.HasMissing) == ValidateCodes.HasMissing;
				bool hasFound = (outCode & ValidateCodes.HasFound) == ValidateCodes.HasFound;
				bool hasDups = (outCode & ValidateCodes.HasDups) == ValidateCodes.HasDups;

				if (hasFound)
				{
					Console.WriteLine("continue - ignore extra found PDFs");
				}

				if (hasMissing || hasDups)
				{
					Console.WriteLine("\n*** FAIL ***");
					if (hasDups) Console.WriteLine("continue - ignore duplicate PDFs");
					if (hasMissing) Console.WriteLine("cannot continue - there are missing PDFs");
					Console.WriteLine("Exiting");
					return;
				}
			}
			
			PdfAssemblerSettingSupport pass = new PdfAssemblerSettingSupport(configSettingFilePath);
			
			result = pass.Process();
			if (!result)
			{
				Console.WriteLine("\n*** FAIL ***");
				Console.WriteLine("Could not read config settings");
				Console.WriteLine("Exiting");
				return;
			}

			// Console.WriteLine("\n*** WORKED ***");
			// Console.WriteLine("Continuing");

			mkTree = new MakePdfTree(schMgr, ShtData);
			result = mkTree.MakeTree();

			if (!result)
			{
				Console.WriteLine("\n*** FAIL ***");
				Console.WriteLine("\n*** Could not create the PDF Tree ***");
				Console.WriteLine("Exiting");
				return;
			}

			pt12 = new PdfText12();
			pt12.UpdateConfig(pass);

			Stopwatch sw = Stopwatch.StartNew();

			// Debug.WriteLine("start add links");

			result = pt12.AddLinks(mkTree.Tree, destFilePath.FullFilePath);

			sw.Stop();

			if (result)
			{
				Console.WriteLine("*** Worked ***");
			}
			else
			{
				Console.WriteLine("*** Failed ***");
				procFailCode(PdfText12.FailCode, pt12);

				// processFailCode(PdfText12.FailCode);
			}

			if (PdfText12.RotationFailList.Count > 0)
			{
				Console.WriteLine("The rotation of some sheets could not be corrected:");

				foreach (PdfTreeLeaf leaf in PdfText12.RotationFailList)
				{
					Console.WriteLine(leaf.File.FileName);
				}
			}

			if (PdfText12.ValidationFailList.Count > 0)
			{
				Console.WriteLine("The sheet number on some sheets could not be validated:");

				foreach (PdfTreeLeaf leaf in PdfText12.ValidationFailList)
				{
					Console.WriteLine(leaf.File.FileName);
				}
			}

			Console.WriteLine("PDF Assembler Complete (created by CyberStudio / Jeff Stuyvesant");

		}

		private void readPdfContents301()
		{
			Debug.WriteLine("\n**** running test 301 ****\n");

			xlMgr = new ExcelManager();
			validate = new ValidateFilesInFolder();


			if (!getFoldersAndFiles(false, 7))
			{
				Console.WriteLine("files and folders failed");
				return;
			}

			if (! readSchedule())
			{
				Console.WriteLine("read schedule failed");
				return;
			}

			mkTree = new MakePdfTree(schMgr, ShtData);

			if (!mkTree.MakeTree())
			{
				Console.WriteLine("make PDF tree failed");
				return;
			}

			pt301 = new PdfText301();

			pt301.Process(mkTree.Tree, destFilePath.FullFilePath);

		}

		private void readPdfContents302()
		{
			Console.WriteLine("\n**** running test 302 ****\n");
			Debug.WriteLine("\n**** running test 302 ****\n");

			xlMgr = new ExcelManager();
			validate = new ValidateFilesInFolder();
			
			if (!getFoldersAndFiles(false, 7))
			{
				Console.WriteLine("files and folders failed");
				return;
			}

			if (! readSchedule())
			{
				Console.WriteLine("read schedule failed");
				return;
			}

			mkTree = new MakePdfTree(schMgr, ShtData);

			if (!mkTree.MakeTree())
			{
				Console.WriteLine("make PDF tree failed");
				return;
			}

			pt302 = new PdfText302();

			pt302.Process(mkTree.Tree, destFilePath.FullFilePath);

		}

		private void readPdfContents303()
		{
			Console.WriteLine("\n**** running test 303 ****\n");
			Debug.WriteLine("\n**** running test 303 ****\n");

			xlMgr = new ExcelManager();
			validate = new ValidateFilesInFolder();
			
			if (!getFoldersAndFiles(false, 7))
			{
				Console.WriteLine("files and folders failed");
				return;
			}

			if (! readSchedule())
			{
				Console.WriteLine("read schedule failed");
				return;
			}

			mkTree = new MakePdfTree(schMgr, ShtData);

			if (!mkTree.MakeTree())
			{
				Console.WriteLine("make PDF tree failed");
				return;
			}

			pt303 = new PdfText303();

			pt303.Process(mkTree.Tree, destFilePath.FullFilePath);
		}




		private bool getFoldersAndFiles(bool useDialog, int which = 0)
		{
			if (useDialog)
			{
				return getFoldersAndFiles();
			}

			string baseFolder = null;
			string pdfFolderName = null;
			string sheetFileName = null;
			string destFileName = null;


			// the complete list (331 total)
			// 0
			// pdfFolder = new FilePath<FileNameSimple>(@"C:\Users\jeffs\OneDrive\Office\2021-073 Simon Brea\Files for hyperlink");
			// xlsxFilePath = new FilePath<FileNameSimple>(@"C:\Users\jeffs\OneDrive\Office\2021-073 Simon Brea\{Simon Brea} Sheet List_04102024 -use.xlsx");
			// destFilePath = new FilePath<FileNameSimple>(@"C:\Users\jeffs\OneDrive\Office\2021-073 Simon Brea\Combined.pdf");
			// destFolderPath = new FilePath<FileNameSimple>(destFilePath.FolderPath);
			// configSettingFilePath = new FilePath<FileNameSimple>(new [] { UserSettings.Data.xlsxFolder, TEMP_CONFIG_FILE });

			// test files (22 total)
			// 1
			// pdfFolder = new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Simon Brea\PDF Files");
			// xlsxFilePath = new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Simon Brea\{Simon Brea} Sheet List_04102024.xlsx");
			// destFilePath = new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Simon Brea\Combined.pdf");
			// destFolderPath = new FilePath<FileNameSimple>(destFilePath.FolderPath);
			// configSettingFilePath = new FilePath<FileNameSimple>(new [] { UserSettings.Data.xlsxFolder, TEMP_CONFIG_FILE });

			// test2
			// 2
			// string baseFolder = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test2";
			// pdfFolder = new FilePath<FileNameSimple>(baseFolder);
			// xlsxFilePath = new FilePath<FileNameSimple>(baseFolder +"\\" + @"\Test2 Sheet List.xlsx");
			// destFilePath = new FilePath<FileNameSimple>(baseFolder +"\\" + @"Combined.pdf");
			// destFolderPath = new FilePath<FileNameSimple>(baseFolder);
			// configSettingFilePath = new FilePath<FileNameSimple>(new [] { baseFolder, TEMP_CONFIG_FILE });


			// test files (5 total)
			// 3
			// string baseFolder = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test3\";
			// string pdfFolderName = "PDF Files";
			// string sheetFileName = "Sheet List.xlsx";
			// string destFileName = "Combined.pdf";


			
			// coliseum (242 total)
			if (which == 4)
			{
				baseFolder = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Coliseum";
				pdfFolderName = "\\PDF Files";
				sheetFileName = "\\Sheet List.xlsx";
				destFileName = "\\Combined.pdf";
			}

			// 5
			// string baseFolder = @"C:\Users\jeffs\OneDrive\Office\2021-073 Simon Brea\Final 1 A\PDFs";
			// // string pdfFolderName = "PDF Files";
			// string sheetFileName = "\\A{Simon Brea} Sheet List_04222024 -use.xlsx";
			// string destFileName = "\\Combined.pdf";

			if (which == 6)
			{
				// test files (5 total)
				// 6
				baseFolder = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test4\";
				pdfFolderName = "PDF Files";
				sheetFileName = "Sheet List.xlsx";
				destFileName = "Combined-6.pdf";
			}



			if (which == 7)
			{
				// test files (3 total)
				// 7
				baseFolder = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test5\";
				pdfFolderName = "PDF Files";
				sheetFileName = "Sheet List.xlsx";
				destFileName = "Combined-7.pdf";
			}

			selectFiles = false;

			pdfFolder = new FilePath<FileNameSimple>(baseFolder+pdfFolderName);
			xlsxFilePath = new FilePath<FileNameSimple>(baseFolder+sheetFileName);
			destFilePath = new FilePath<FileNameSimple>(baseFolder+destFileName);
			destFolderPath = new FilePath<FileNameSimple>(destFilePath.FolderPath);
			configSettingFilePath = new FilePath<FileNameSimple>(new [] {xlsxFilePath.FolderPath, TEMP_CONFIG_FILE });

			return true;
		}


		private bool getFoldersAndFiles()
		{

			CommonFileDialogFilter filter = new CommonFileDialogFilter("Sheet Schedule Files", "*.xlsx" );

			Console.WriteLine("running PDF Assembler");

			string response;


			if (selectFiles)
			{

				response = getFolderCfd("Select the Folder with the PDF Files",  pdfFolder_settings);

				if (response == null) return false;

				pdfFolder = new FilePath<FileNameSimple>(response);

				if (!pdfFolder.Exists)
				{
					Console.WriteLine("Folder does not exist - Exiting");
					return false;
				}

				response = getFileCfd("Select the .xlsx sheet list file",
					xlsxFolder_settings, filter, xlsxFile_settings);

				if (response == null) return false;

				xlsxFilePath = 
					new FilePath<FileNameSimple>(response);

				if (!xlsxFilePath.Exists)
				{
					Console.WriteLine("File does not exist - Exiting");
					return false;
				}



				response = getFolderCfd("Select the Destination Folder", destFolder_settings);

				if (response == null) return false;

				destFolderPath =
					new FilePath<FileNameSimple>(response);

				if (!destFolderPath.Exists)
				{
					Console.WriteLine("Folder does not exist - Exiting");
					return false;
				}
				
				configSettingFilePath = new FilePath<FileNameSimple>(new [] { xlsxFolder_settings, TEMP_CONFIG_FILE });

			}



			destFilePath = new FilePath<FileNameSimple>(
				new string[] { destFolderPath.FullFilePath, DEST_FILE } );

			if (!configSettingFilePath.Exists) configSettingFilePath = null;

			Console.WriteLine($"{"Folder with Source PDF files", -30}| {pdfFolder.FullFilePath}");

			Console.WriteLine($"{"Sheet Schedule Folder",-30}| {xlsxFilePath.FolderPath}");
			Console.WriteLine($"{"Sheet Schedule File",-30}| {xlsxFilePath.FileName}");

			Console.WriteLine($"{"Temp Config Folder",-30}| {configSettingFilePath?.FolderPath ?? "*** MISSING ***"}");
			Console.WriteLine($"{"Temp Config File",-30}| {configSettingFilePath?.FileName ?? "*** MISSING ***"}");

			Console.WriteLine($"{"PDF Destination Folder",-30}| {destFolderPath.FullFilePath}");
			Console.WriteLine($"{"PDF Destination file",-30}| {destFilePath.FileName}");

			Console.WriteLine($"{"Sheet Metrics Folder",-30}| {dataFile.FullFilePath}");
			Console.WriteLine($"{"Sheet Metrics file",-30}| {dataFile.FileName}");

			pdfFolder_settings = pdfFolder.FullFilePath;
			xlsxFolder_settings = xlsxFilePath.FolderPath;
			xlsxFile_settings = xlsxFilePath.FileName;
			destFolder_settings = destFolderPath.FullFilePath;

			UserSettings.Admin.Write();

			return true;
		}



		// return false if any of the invalidate conditions are found
		// return true if ok

		private bool validateFilesLists(out ValidateCodes outCode)
		{
			outCode = 0;

			validate.getSheetNumbersFromFolder(pdfFolder);
			validate.validateFoundPdfs(schMgr.RowData);
			validate.validateMissingPdfs(schMgr.RowData);

			bool hasFound = showFound(validate.FoundPdfs);
			bool hasMissing = showMissing(validate.MissingPdfs);
			bool hasDups = showDups(validate.Duplicates);

			outCode |= hasMissing ? ValidateCodes.HasMissing : 0;
			outCode |= hasFound ? ValidateCodes.HasFound : 0;
			outCode |= hasDups ? ValidateCodes.HasDups : 0;

			return !(hasFound || hasMissing || hasDups);
		}

		private bool readConfigSettings()
		{
			pass = new PdfAssemblerSettingSupport(configSettingFilePath);

			return true;
		}

		private bool readSchedule()
		{
			bool result;
			
			schMgr = new ScheduleListManager(xlMgr);

			result = schMgr.ReadSchedule(xlsxFilePath, pdfFolder, validate);

			if (!result)
			{
				Console.WriteLine("Cannot reading the Sheet Schedule");
				return false;
			}

			// System.Data.DataSet schedule = xlMgr.Schedule;

			schMgr.ParseRows();

			return true;
		}

		private bool showMissing(List<string> missing)
		{
			if (missing == null || missing.Count == 0)
			{
				Console.WriteLine("No missing PDFs");
				return false;
			}

			Console.WriteLine($"\n*** there are some missing PDF files ({missing.Count}) ***");

			foreach (string s in missing)
			{
				Console.WriteLine($"{s}");
			}

			return true;
		}

		private bool showFound(List<string> found)
		{
			if (found == null || found.Count == 0)
			{
				Console.WriteLine("No found PDFs");
				return false;
			}

			Console.WriteLine($"\n*** there are some found PDF files ({found.Count}) ***");

			foreach (string s in found)
			{
				Console.WriteLine($"{s}");
			}

			return true;
		}

		private bool showDups(List<string> dups)
		{
			if (dups == null || dups.Count == 0)
			{
				Console.WriteLine("No duplicate PDFs");
				return false;
			}

			Console.WriteLine($"\n*** there are some duplicate PDFs listed ({dups.Count}) ***");

			foreach (string s in dups)
			{
				Console.WriteLine($"{s}");
			}

			return true;
		}

		private static void getUserName()
		{
			UserName = null;
			return;
			// UserName = UserPrincipal.Current.DisplayName;
		}


		private string getFolderCfd(string title, string initFolder)
		{


			cfd.Title = title;
			cfd.InitialDirectory = initFolder;
			cfd.IsFolderPicker=true;
			cfd.ShowPlacesList = true;
			cfd.Multiselect = false;
			cfd.AllowNonFileSystemItems = false;
			cfd.EnsurePathExists = true;
			cfd.Filters.Clear();
			cfd.DefaultFileName = null;

			if (cfd.ShowDialog() == CommonFileDialogResult.Ok) return cfd.FileName;

			return null;
		}

		private string getFileCfd(string title, string initFolder, 
			CommonFileDialogFilter filter, string defaultFileName)
		{

			cfd.Title = title;
			cfd.InitialDirectory = initFolder;
			cfd.IsFolderPicker=false;
			cfd.ShowPlacesList = true;
			cfd.Multiselect = false;
			cfd.AllowNonFileSystemItems = false;
			cfd.EnsureFileExists = true;
			cfd.DefaultFileName = defaultFileName;
			cfd.Filters.Add(filter);

			if (cfd.ShowDialog() == CommonFileDialogResult.Ok) return cfd.FileName;

			return null;
		}

		private void writeSheetData()
		{
			Dictionary<SheetBorderType, SheetData> shtData = SheetConfig.SheetConfigData;

			// SheetConfig.ShowSheetData();

			// string a1 = CsUtilities.AssemblyName;
			//
			// string p1 = UserSettings.Admin.Path.RootFolderPath;
			// string p2 = UserSettings.Admin.Path.SettingFolderPath;

			// string filePath = @"C:\Users\jeffs\Documents\temp\sheetmetrics.xml";
			
			dm1 = new DataManager<DataStorageSet>(dataFile);

			dm1.Data.SheetData = shtData;

			dm1.Info.Description = "Sheet Metric Data";
			dm1.Info.DataClassVersion = "0.2";

// ***		// dm1.Data.SheetData = SheetConfig.SheetConfigData;

			AltRectangle a = dm1.Data.SheetData[SheetBorderType.ST_AO_36X48_WITH_PACKAGE].SheetRectA[0];

			Console.WriteLine($"write| x| {a.X} | y| {a.Y} | w| {a.Width} | h| {a.Height}");

			dm1.Admin.Write();

			//
			// dm2.Admin.Read();
			//
			// a = dm2.Data.SheetData[SheetBorderType.ST_AO_36X48_WITH_PACKAGE].SheetRectA[0];
			// Rectangle r = dm2.Data.SheetData[SheetBorderType.ST_AO_36X48_WITH_PACKAGE].SheetRect[0];
			//
			// Console.WriteLine($"read| x| {a.X} | y| {a.Y} | w| {a.Width} | h| {a.Height}");
			//
			// Console.WriteLine($"read| x| {r.GetX()} | y| {r.GetY()} | w| {r.GetWidth()} | h| {r.GetHeight()}");

			
		}

		private void readSheetData()
		{
			dm2 = new DataManager<DataStorageSet>(dataFile);
			dm2.Admin.Read();

			adjustRects();
		}

		private void adjustRects()
		{
			Rectangle ps =  dm2.Data.SheetData[SheetBorderType.ST_AO_36X48].SheetRect[0];
			Rectangle r;

			Debug.WriteLine("\nao 36x48");

			r = dm2.Data.SheetData[SheetBorderType.ST_AO_36X48].SheetNumberLinkRect[0];
			Debug.WriteLine($"sht link before rotate| {ReadPDFText.fmtRect(r, 1)}");

			r = rotateRect(r, ps);
			dm2.Data.SheetData[SheetBorderType.ST_AO_36X48].SheetNumberLinkRect[0] = r;


			r = dm2.Data.SheetData[SheetBorderType.ST_AO_36X48].PrimaryAuthorRect[0];
			Debug.WriteLine($"author before rotate| {ReadPDFText.fmtRect(r, 1)}");
			
			r = rotateRect(r, ps);
			dm2.Data.SheetData[SheetBorderType.ST_AO_36X48].PrimaryAuthorRect[0] = r;


			Debug.WriteLine("\nresi 30x42");

			ps =dm2.Data.SheetData[SheetBorderType.ST_AO_30X42_RESI].SheetRect[0];

			r = dm2.Data.SheetData[SheetBorderType.ST_AO_30X42_RESI].SheetNumberLinkRect[0];
			Debug.WriteLine($"sht link before rotate| {ReadPDFText.fmtRect(r, 1)}");

			r = rotateRect(r, ps);
			dm2.Data.SheetData[SheetBorderType.ST_AO_30X42_RESI].SheetNumberLinkRect[0] = r;

			r = dm2.Data.SheetData[SheetBorderType.ST_AO_30X42_RESI].PrimaryAuthorRect[0];
			Debug.WriteLine($"author before rotate| {ReadPDFText.fmtRect(r, 1)}");

			r = rotateRect(r, ps);
			dm2.Data.SheetData[SheetBorderType.ST_AO_30X42_RESI].PrimaryAuthorRect[0] = r;

		}

		private Rectangle rotateRect(Rectangle r , Rectangle pageSize)
		{
			float w = r.GetHeight();
			float h = r.GetWidth();
			float y = r.GetX();

			float x = pageSize.GetHeight() - r.GetY() - w;

			return new Rectangle(x, y, w, h);
		}


		// ***********************

		private void procFailCode<T>(int failCode, T pdfText)
			where T : IPdfTextXX
		{
			Console.WriteLine($"{pdfText.Elt[failCode]} failed");

			if (failCode == pdfText.Mergedone)
			{
				Console.WriteLine("nothing additional");
			}
			else
			if (failCode == pdfText.Validatedone)
			{
				foreach (PdfTreeLeaf leaf in pdfText.RotFailList)
				{
					Console.Write($"leaf| {leaf.Bookmark}");

					if (leaf.SheetNumberTals != null)
					{
						Console.WriteLine($" sheet number did not match| {leaf.SheetNumber} (set) vs. {leaf.SheetNumberTals.Text} (read)");
					}
					else
					{
						Console.WriteLine($"  failed");
					}
				}
			}
			else
			if (failCode == pdfText.Bookmarksdone)
			{
				Console.WriteLine("nothing additional");
			}
		}

		private void processFailCode(int failCode)
		{

			Console.WriteLine($"{PdfText10.elt[failCode]} failed");

			if (failCode == PdfText10.MERGEDONE)
			{
				Console.WriteLine("nothing additional");
			}
			else
			if (failCode == PdfText10.VALIDATEDONE)
			{
				foreach (PdfTreeLeaf leaf in PdfText10.RotationFailList)
				{
					Console.Write($"leaf| {leaf.Bookmark}");

					if (leaf.SheetNumberTals != null)
					{
						Console.WriteLine($" sheet number did not match| {leaf.SheetNumber} (set) vs. {leaf.SheetNumberTals.Text} (read)");
					}
					else
					{
						Console.WriteLine($"  failed");
					}
				}
			}
			else
			if (failCode == PdfText10.BOOKMARKSDONE)
			{
				Console.WriteLine("nothing additional");
			}
		}



		public static string fmtRect(Rectangle r, float divisor = 1)
		{
			return $"x| {(r.GetX()/divisor),7:F2}| y| {(r.GetY()/divisor),7:F2}| w| {(r.GetWidth()/divisor),7:F2}| h| {(r.GetHeight()/divisor),7:F2}";
		}





		/*


		private string getFolder(string desc, string initFolder)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.Description = desc;
			fbd.ShowNewFolderButton = false;
			fbd.SelectedPath = initFolder;

			if (fbd.ShowDialog() == DialogResult.OK)
			{
				return fbd.SelectedPath;
			}

			return null;
		}

		private string getFile()
		{

			System.Windows.Forms.OpenFileDialog ofd = new OpenFileDialog();


			ofd.ShowDialog();


			// var ofn = new OpenFileName();
			//
			// ofn.lStructSize = Marshal.SizeOf(ofn);
			// // Define Filter for your extensions (Excel, ...)
			// ofn.lpstrFilter = "Excel Files (*.xlsx)\0*.xlsx\0All Files (*.*)\0*.*\0";
			// ofn.lpstrFile = new string(new char[256]);
			// ofn.nMaxFile = ofn.lpstrFile.Length;
			// ofn.lpstrFileTitle = new string(new char[64]);
			// ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
			// ofn.lpstrTitle = "Open File Dialog...";
			// if (GetOpenFileName(ref ofn))
			// 	return ofn.lpstrFile;
			// return string.Empty;

			return ofd.FileName;
		}




		private void readTextFromPdf()
		{
			PdfReader origReader = new PdfReader(SRC1);
			PdfWriter origWriter = new PdfWriter(DEST);

			// PdfDocument pdfDoc = new PdfDocument(origReader, origWriter);
			PdfDocument pdfDoc = new PdfDocument(origReader);


			pt1 = new PdfText1();

			pt1.TestGetText(pdfDoc);

			pdfDoc.Close();


		}

		private void makeHyperLinks()
		{
			PdfReader origReader = new PdfReader(SRCT1);
			PdfWriter newWriter = new PdfWriter(DEST);

			PdfDocument pdfDoc = new PdfDocument(origReader, newWriter);
			

			pt2 = new PdfText2();

			pt2.Test2AddLinks(pdfDoc);

			pdfDoc.Close();
		}

		private void readPdfContents()
		{
			List<string> files = new List<string>()
			{
				SRCT1, SRCT2, SRCT3
			};

			pt3 = new PdfText3();

			pt3.GetPdfContents(files, DEST);

		}

		private void readPdfContents4()
		{
			List<string> files = new List<string>()
			{
				SRCT1, SRCT2, SRCT3
			};

			pt4 = new PdfText4();

			pt4.AddLinks(files, SRC2_LOC, DEST);

		}


		private void readPdfContents5()
		{
			tree = SampleSupport.MakePdfTree();
			SampleSupport.showPdfNodeTree();

			List<string> files = new List<string>()
			{
				SRCT1, SRCT2, SRCT3, SRCT4
			};

			Rectangle shtNumLoc = new Rectangle(426f, 27f, 161f, 123f);

			pt5 = new PdfText5(true);

			Stopwatch sw = Stopwatch.StartNew();

			pt5.AddLinks(tree, DEST);

			sw.Stop();

			Console.WriteLine($"\nelapsed time: {sw.ElapsedMilliseconds:F2}");

		}
		*/

		/*
		private void readPdfContents6()
		{
			Console.WriteLine("running test 7");

			tree = SampleSupport.MakePdfTree();
			SampleSupport.showPdfNodeTree();

			List<SampleData> sampleData = SampleSupport.fileNameListShort;
			SampleSupport.showSampleDataShort();


			pt5 = new PdfText5(false);

			Stopwatch sw = Stopwatch.StartNew();

			pt5.AddLinks(sampleData, DEST);

			sw.Stop();

			Console.WriteLine($"elapsed milisecs| {sw.Elapsed.TotalMilliseconds,7:F4}");
			Console.WriteLine($"elapsed     secs| {sw.Elapsed.TotalSeconds,7:F4}");
		}


		private void rotatePdf1()
		{
			bool result;

			string source = string.Concat(SRC3_LOC, @"\", SRCR3a);

			pr1 = new PdfRotate1();

			result = pr1.Process(source, DEST);
		}

		private void rotatePdf2()
		{
			bool result;

			string source = string.Concat(SRC102_LOC, @"\", SRCR102a);

			pr2 = new PdfRotate2();

			result = pr2.Process(source, DEST);
		}

		private void testPdf1()
		{
			bool result;

			string source = string.Concat(SRC4_LOC, @"\", SRCR4a);

			ptst1 = new PdfTest1();

			result = ptst1.Process(source, DEST);
		}





		// merge the documents
		public void manipulatePdf(string destFolder)
		{
			PdfDocument pdfDoc = new PdfDocument(new PdfWriter(DEST));

			// merge bookmarks must be set to false
			// default behavor is to merge bookmarks
			PdfMerger merger = new PdfMerger(pdfDoc, false, false);

			int page = 1;

			PdfDocument srcDoc1 = new PdfDocument(new PdfReader(SRC1));
			merger.Merge(srcDoc1, 1, srcDoc1.GetNumberOfPages());

			PdfDocument srcDoc2 = new PdfDocument(new PdfReader(SRC2));
			PdfDocument srcDoc3 = new PdfDocument(new PdfReader(SRC3));
			merger.SetCloseSourceDocuments(false)
			.Merge(srcDoc2, 1, srcDoc2.GetNumberOfPages())
			.Merge(srcDoc3, 1, srcDoc3.GetNumberOfPages());


			// this must occur after the documents are merged
			// otherwise the bookmarks get copied anyway
			PdfOutline rootOutline = pdfDoc.GetOutlines(false);

			PdfOutline helloWorld = rootOutline.AddOutline("Hello World");
			helloWorld.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(page)));
			page += srcDoc1.GetNumberOfPages();

			PdfOutline link1 = helloWorld.AddOutline("link1");
			link1.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(page)));
			page += srcDoc2.GetNumberOfPages();

			PdfOutline link2 = rootOutline.AddOutline("Link 2");
			link2.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(page)));

			srcDoc1.Close();
			srcDoc2.Close();
			srcDoc3.Close();

			pdfDoc.Close();
		}

		// get text from the documents based on the provided rectangles
		public void GetTextFromPDF()
		{
			TextLocation number = new TextLocation(0.3359F, 0.3714F, 2.2510F, 1.7218F, AnchorPoint.BR, "Sheet Number");
			TextLocation title = new TextLocation(0.4067F, 2.7305F, 1.2878F, 5.8397F, AnchorPoint.BR, "Sheet Title");

			number.ShowTextLocation(PW, PH);
			title.ShowTextLocation(PW, PH);


			TextLocation[] textLocs = new TextLocation[info.NumberFields];

			textLocs[(int) FieldId.SHT_NUM] = number;
			textLocs[(int) FieldId.SHT_TITLE] = title;


			getTextFromPDF(SRC1, textLocs);
			getTextFromPDF(SRC2, textLocs);
			getTextFromPDF(SRC3, textLocs);
		}

		private void getTextFromPDF(string src, TextLocation[] textLocs)
		{
			PdfDocument pdfDoc = new PdfDocument(new PdfReader(src));

			// page , textlocation
			string[,] shtData = getTextViaRect(pdfDoc, textLocs);

			Console.WriteLine();
			Console.WriteLine($"getinfo from: {src}\n");

			for (var i = 0; i < shtData.GetLength(0); i++)
			{
				Console.WriteLine($"page {i}");

				for (var j = 0; j < shtData.GetLength(1); j++)
				{
					Console.WriteLine($"\t{info.SheetData[j]} is| >{shtData[i,j]}<");
					
				}

				Console.WriteLine("\t" + makeName(shtData, i));
			}

		}

		private string makeName(string[,] data, int page )
		{
			string sheetNumber = data[page, (int) FieldId.SHT_NUM];
			string sheetTitle = data[page, (int) FieldId.SHT_TITLE];

			sheetNumber = cleanName(sheetNumber);
			sheetTitle = cleanName(sheetTitle);

			return $"{sheetNumber} - {sheetTitle}";
		}

		// rectangle units = 72 units per inch
		private string[,] getTextViaRect(PdfDocument pdfDoc, TextLocation[] textLocs)
		{
			int numPages = pdfDoc.GetNumberOfPages();

			string[,] results = new string[numPages, info.NumberFields];

			for (int page = 1;
				page <= numPages;
				page++)
			{
				for (var i = 0; i < textLocs.Length; i++)
				{
					results[page-1,i]=getTextAtRect(pdfDoc, page, textLocs[i]);
				}
			}

			return results;
		}

		private string getTextAtRect(PdfDocument pdfDoc, int page, TextLocation textLoc)
		{
			Rectangle rect = textLoc.GetRect( PW, PH);
			TextRegionEventFilter regionFilter = new TextRegionEventFilter(rect);

			ITextExtractionStrategy strategy = new FilteredTextEventListener(new LocationTextExtractionStrategy(), regionFilter);

			return PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
		}

		private string cleanName(string name)
		{
			name = makeNameSerial(name);

			int i = name.IndexOfAny(Path.GetInvalidFileNameChars());

			if (i == -1) return name;

			StringBuilder sb = new StringBuilder();

			while (i > 0)
			{
				sb.Append(name.Substring(0, i)).Append(".");
				name = name.Substring(i+1);
				i = name.IndexOfAny(Path.GetInvalidFileNameChars());
			}

			if (name.Length > 0)
			{
				sb.Append(name);
			}



			return sb.ToString();
		}

		private string makeNameSerial(string name)
		{
			string result = name.Trim();

			int i = result.IndexOf("\n");
			int r = result.IndexOf("\r");

			if (i == -1 && r == -1) return result;

			i = i > 0 ? i : r;
			r = r > 0 ? r : i;

			StringBuilder sb = new StringBuilder();

			while (r>0)
			{
				sb.Append(result.Substring(0, r).TrimEnd()).Append(" ");

				result = result.Substring(i + 1);

				i = result.IndexOf("\n");
				r = result.IndexOf("\r");
				
				i = i > 0 ? i : r;
				r = r > 0 ? r : i;
			}

			if (result.Length > 0)
			{
				sb.Append(result);
			}

			return sb.ToString();
		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct OpenFileName
		{
			public int lStructSize;
			public IntPtr hwndOwner;
			public IntPtr hInstance;
			public string lpstrFilter;
			public string lpstrCustomFilter;
			public int nMaxCustFilter;
			public int nFilterIndex;
			public string lpstrFile;
			public int nMaxFile;
			public string lpstrFileTitle;
			public int nMaxFileTitle;
			public string lpstrInitialDir;
			public string lpstrTitle;
			public int Flags;
			public short nFileOffset;
			public short nFileExtension;
			public string lpstrDefExt;
			public IntPtr lCustData;
			public IntPtr lpfnHook;
			public string lpTemplateName;
			public IntPtr pvReserved;
			public int dwReserved;
			public int flagsEx;
		}

		private delegate int BFFCALLBACK(IntPtr hwnd, uint uMsg, IntPtr lParam, IntPtr lpData);

		// Struct to pass parameters to the SHBrowseForFolder function.
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		private struct BROWSEINFO
		{
			public IntPtr hwndOwner;
			public IntPtr pidlRoot;
			public IntPtr pszDisplayName;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpszTitle;
			public int ulFlags;
			[MarshalAs(UnmanagedType.FunctionPtr)]
			public BFFCALLBACK lpfn;
			public IntPtr lParam;
			public int iImage;
		}
		*/
	}

		
	public enum AnchorPoint
	{
		BOTTOM = 10,
		CENTER = 20,
		TOP = 30,
		LEFT = 3,
		MIDDLE = 2,
		RIGHT = 1,

		TL = TOP + LEFT,
		TM = TOP + MIDDLE, 
		TR = TOP + RIGHT,
		CL = CENTER + LEFT,
		CM = CENTER + MIDDLE, 
		CR = CENTER + RIGHT,
		BL = BOTTOM + LEFT,
		BM = BOTTOM + MIDDLE, 
		BR = BOTTOM + RIGHT

	}

	public enum FieldId
	{
		SHT_NUM = 0,
		SHT_TITLE = 1,
	}
	
	class Info 
	{
		public const int UNIT_FACTOR = 72;

		public List<string>  SheetData= new List<string>();

		public int NumberFields = Enum.GetNames(typeof(FieldId)).Length;

		public Info()
		{
			initSheetData(NumberFields);

			SheetData[(int) FieldId.SHT_NUM] = "Sheet Number";
			SheetData[(int) FieldId.SHT_TITLE] = "Sheet Title";

		}

		private void initSheetData(int count)
		{
			for (int i = 0; i < count; i++)
			{
				SheetData.Add("");
			}
		}

	}


	class TextLocation
	{
		// if anchor point is TL, X = distance from page top to text area top
		//		and Y = distance from page left edge to text area left edge
		// if anchor point is BR, X = distance from page bottom to text area bottom
		//		and Y = distance from page right edge to text area right edge

		public string Description { get; }

		public float X { get; }  // anchor point x location (left to right distance)
		public float Y { get; }  // anchor point y location (from top to bottom distance)
		public float W { get; }  // text area width (inches)
		public float H { get; }  // text area height (inches)
		public AnchorPoint AP { get; }  // page anchor point

		public TextLocation(float x, float y, float w, float h, AnchorPoint ap, string desc)
		{
			X = x;
			Y = y;
			W = w;
			H = h;
			AP = ap;
			Description= desc;
		}

		/// <summary>
		/// get the rectangle for the text area based on the text location information
		/// adjusting for the anchor point
		/// </summary>
		/// <param name="pw"></param>
		/// <param name="ph"></param>
		/// <param name="pageHeight"></param>
		/// <param name="pageWidth"></param>
		/// <returns></returns>
		public Rectangle GetRect(  float pw, float ph)
		{

			
			Rectangle rect;

			float bottom = Y;
			float left   = X;


			int anchorVert = ((int) AP) % 10;

			if (AP > AnchorPoint.TOP)
			{
				// is top
				bottom = ph - Y - H;

			} 
			else if (AP > AnchorPoint.CENTER)
			{
				// is center
				bottom = ph / 2 - Y - H / 2;
			}


			if (anchorVert == (int) AnchorPoint.RIGHT)
			{
				// is right
				left = pw- X - W;
			} 
			else if (anchorVert == (int) AnchorPoint.MIDDLE)
			{
				// is middle
				left = pw / 2 - X - W / 2;
			}


			return new Rectangle(left * Info.UNIT_FACTOR, bottom * Info.UNIT_FACTOR, W * Info.UNIT_FACTOR, H * Info.UNIT_FACTOR);
		}

		public void ShowTextLocation(float pw, float ph)
		{
			Rectangle r = GetRect(pw, ph);

			// string b = r.GetBottom().ToString();
			// string l = r.GetLeft().ToString();
			// string w = r.GetWidth().ToString();
			// string h = r.GetHeight().ToString();

			Console.WriteLine("\n");
			Console.WriteLine($"Desc | {Description}");
			Console.WriteLine($"AP | {AP}");
			Console.WriteLine("as inches| page | provided");
			Console.WriteLine($"PW x PH | {pw} x {ph}");
			Console.WriteLine("as inches | text location | provided");
			Console.WriteLine($"X x Y | {X} x {Y}");
			Console.WriteLine($"W x H | {W} x {H}");
			Console.WriteLine("as inches | derived");
			Console.WriteLine($"B x L | {r.GetBottom()/Info.UNIT_FACTOR} x {r.GetLeft()/Info.UNIT_FACTOR}");
			Console.WriteLine($"w x h | {r.GetWidth()/Info.UNIT_FACTOR} x {r.GetHeight()/Info.UNIT_FACTOR}");
			Console.WriteLine("as points | derived");
			Console.WriteLine($"B x L | {r.GetBottom()} x {r.GetLeft()}");
			Console.WriteLine($"w x h | {r.GetWidth()} x {r.GetHeight()}");
		}

	}



	// class GetTextRectangle : ITextExtractionStrategy
	// {
	// 	private ITextExtractionStrategy innerStrategy = null;
	// 	private Rectangle rect;
	//
	// 	public GetTextRectangle(ITextExtractionStrategy strategy, Rectangle rectangle)
	// 	{
	// 		this.innerStrategy = strategy;
	// 		this.rect = rectangle;
	// 	}
	//
	// 	public void EventOccurred(IEventData iEventData, EventType eventType)
	// 	{
	// 		if (eventType != EventType.RENDER_TEXT)
	// 		{
	// 			return;
	// 		}
	//
	// 		TextRenderInfo textInfo = (TextRenderInfo) iEventData;
	//
	// 		foreach (TextRenderInfo subTextInfo in textInfo.GetCharacterRenderInfos())
	// 		{
	// 			Rectangle rt = new CharacterRenderInfo(subTextInfo).GetBoundingBox();
	//
	// 			if (intersects(rt))
	// 			{
	// 				innerStrategy.EventOccurred(subTextInfo, EventType.RENDER_TEXT);
	// 			}
	// 		}
	// 	}
	//
	// 	private bool intersects(Rectangle r)
	// 	{
	// 		return true;
	// 	}
	//
	//
	// 	public ICollection<EventType> GetSupportedEvents()
	// 	{
	// 		return null;
	// 	}
	//
	// 	public string GetResultantText()
	// 	{
	// 		return innerStrategy.GetResultantText();
	// 	}
	// }
}