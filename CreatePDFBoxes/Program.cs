using System.Diagnostics;
using System.Runtime.CompilerServices;
using CreatePDFBoxes.PdfSupport;
using CreatePDFBoxes.SheetData;
using Settings;
using System.Drawing;
using iText.Forms.Util;
using iText.Layout.Properties;
using ShCommonCode.ShSheetData;

namespace CreatePDFBoxes
{
	internal class Program
	{
		public const string SHEET_METRIC_FOLDER = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\";
		private static string DataFilePath { get; }= SHEET_METRIC_FOLDER + SheetDataSet.DataFileName;


		private static Program p;

		static void Main(string[] args)
		{
			p = new Program();

			// p.showAlignment();

			// p.showSinAndCos();

			p.begin();

			Console.Write("Waiting| ");

			Console.ReadKey();
		}

		private void begin()
		{
			ProcessPdfs pp = new ProcessPdfs();

			if (pp.Process(DataFilePath))
			{
				goodMsg();
			}
			else
			{
				failMsg();
			}
		}

		private void goodMsg()
		{
			Console.WriteLine($"\nProcess worked for\n{DataFilePath}\n");
		}

		private void failMsg()
		{
			Console.WriteLine($"\nUnable to process\n{DataFilePath}\n");
		}

		private void showSinAndCos()
		{
			PdfShowInfo.StartMsg("Sin and Cos of Angle", DateTime.Now.ToString(), ShowWhere.DEBUG);

			PdfShowInfo.showSinAndCos();
		}

	}
}
