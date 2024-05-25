using SharedCode.ShDataSupport.ExcelSupport;
using SharedCode.ShDataSupport.PdfTree;
using SharedCode.ShDataSupport.Process;
using SharedCode.ShDataSupport.ScheduleListSupport;
using UtilityLibrary;




namespace ExtractPdfText
{
	internal class Program
	{
		private const string TEMP_CONFIG_FILE = "PdfAssemblerSettings.xlsx";

		private static Program me;


		private static MakePdfTree mkTree;
		private static ScheduleListManager schMgr;
		private static ExcelManager xlMgr;
		private static ValidateFilesInFolder validate;

		private Process101 p101;

		static void Main(string[] args)
		{
			me = new Program();

			me.process(101);

			Console.Write("press enter to continue| "); 
			string answer = Console.ReadLine();
		}

		private static FilePath<FileNameSimple>? pdfFolder { get; set; }
		private static FilePath<FileNameSimple>? xlsxFilePath { get; set; }
		private static FilePath<FileNameSimple>? destFolderPath { get; set; }
		private static FilePath<FileNameSimple>? destFilePath { get; set; }
		private static FilePath<FileNameSimple>? configSettingFilePath { get; set; }


		public void process(int test)
		{
			xlMgr = new ExcelManager();
			validate = new ValidateFilesInFolder();


			if (test == 101)
			{
				run101();
			}


		}

		private void run101()
		{
			setFilesAndFolders(1);

			
			if (! readSchedule())
			{
				Console.WriteLine("read schedule failed");
				return;
			}

			mkTree = new MakePdfTree(schMgr, null);

			if (!mkTree.MakeTree())
			{
				Console.WriteLine("make PDF tree failed");
				return;
			}

			p101 = new Process101();

			p101.Process(mkTree.Tree, destFilePath.FullFilePath);
		}


		private void setFilesAndFolders(int which)
		{

			string baseFolder = null;
			string pdfFolderName = null;
			string sheetFileName = null;
			string destFileName = null;

			if (which == 1)
			{
				// test files (5 total)
				// 6
				baseFolder = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Simon Brea\";
				pdfFolderName = "PDF Files";
				sheetFileName = "{Simon Brea} Sheet List_04102024.xlsx";
				destFileName = "Combined-1.pdf";



				// pdfFolder =      new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Simon Brea\PDF Files");
				// xlsxFilePath =   new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Simon Brea\{Simon Brea} Sheet List_04102024.xlsx");
				// destFilePath =   new FilePath<FileNameSimple>(@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Simon Brea\Combined.pdf");
				// destFolderPath = new FilePath<FileNameSimple>(destFilePath.FolderPath);
				// configSettingFilePath = new FilePath<FileNameSimple>(new [] { UserSettings.Data.xlsxFolder, TEMP_CONFIG_FILE });
			}





			if (which == 6)
			{
				// test files (5 total)
				// 6
				baseFolder = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test4\";
				sheetFileName = "Sheet List.xlsx";
				destFileName = "Combined-6.pdf";
			}

			if (which == 7)
			{
				// test files (3 total)
				// 7
				baseFolder = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test5\";
				sheetFileName = "Sheet List.xlsx";
				destFileName = "Combined-7.pdf";
			}

			pdfFolder = new FilePath<FileNameSimple>(baseFolder+pdfFolderName);
			xlsxFilePath = new FilePath<FileNameSimple>(baseFolder+sheetFileName);
			destFilePath = new FilePath<FileNameSimple>(baseFolder+destFileName);
			destFolderPath = new FilePath<FileNameSimple>(destFilePath.FolderPath);
			configSettingFilePath = new FilePath<FileNameSimple>(new [] {xlsxFilePath.FolderPath, TEMP_CONFIG_FILE });
		}


		private bool readSchedule()
		{
			bool result;
			
			schMgr = new ScheduleListManager(xlMgr);

			result = schMgr.ReadSchedule(xlsxFilePath, pdfFolder, validate);

			if (!result)
			{
				Console.WriteLine("Cannot read the Sheet Schedule");
				return false;
			}

			// System.Data.DataSet schedule = xlMgr.Schedule;

			schMgr.ParseRows();

			return true;
		}


	}
}
