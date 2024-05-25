using System.Diagnostics;
using System.Reflection;
using ScanPDFBoxes.Process;
using Settings;
using ShCommonCode.ShSheetData;
using UtilityLibrary;

namespace ScanPDFBoxes
{
	internal class Program
	{
		public const string SHEET_METRIC_FOLDER = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\";
		private static string DataFilePath { get; }= SHEET_METRIC_FOLDER + SheetDataSet.DataFileName;


		private string rootPath = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\TestBoxes\";
		
		private string[] filesNames = new []
		{
			"TestBoxes.pdf", "A0.1.0 - COVER SHEET.pdf",
			"A2.2-G - LEVEL G - OVERALL FLOOR PLAN.pdf", 
			"TestBoxes 2.pdf"
		};

		private static ProcessManager pm;

		private string[] files;


		private static Program p;

		static void Main(string[] args)
		{
			p = new Program();
			pm = new ProcessManager(new FilePath<FileNameSimple>(DataFilePath));

			Debug.WriteLine($"@1 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");

			p.processAdd();

			Debug.WriteLine($"@2 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");

			pm.ResetSheetData();

			Debug.WriteLine($"@3 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");

			p.processRemove();

			Debug.WriteLine($"@4 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");

		}

		private void processAdd()
		{
			Console.WriteLine("\n*** Add Files ***\n");

			Console.WriteLine("\n*** Process Files 1 ***\n");

			p.getFiles1();

			p.proceedAdd();

			Debug.WriteLine($"@11 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");

			Console.Write("Waiting A1| ");

			Console.ReadKey();


			Console.WriteLine("\n*** Process Files 2 ***\n");

			p.getFiles2();

			p.proceedAdd();

			Debug.WriteLine($"@12 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");

			Console.Write("Waiting A2| ");

			Console.ReadKey();
		}

		private void getFiles1()
		{
			files = new string[2];

			files[0] = rootPath + filesNames[0];
			files[1] = rootPath + filesNames[1];
		}

		private void getFiles2()
		{
			files = new string[3];

			files[0] = rootPath + filesNames[2];
			files[1] = rootPath + filesNames[3];
			files[2] = rootPath + filesNames[0];
		}

		private void proceedAdd()
		{
			bool result;

			result = pm.ScanSheets(files);
		}

		private void processRemove()
		{
			Console.WriteLine("\n*** Remove Files ***\n");

			Console.WriteLine("\n*** Process Files ***\n");

			p.getFiles3();

			p.proceedRemove();

			Console.Write("Waiting R1| ");

			Console.ReadKey();
		}

		private void proceedRemove()
		{
			bool result;

			result = pm.RemoveSheets(files);
		}

		private void getFiles3()
		{
			files = new string[3];

			files[0] = rootPath + filesNames[1];
			files[1] = rootPath + filesNames[2];
			files[1] = rootPath + "no file.pdf";
		}
	}
}
