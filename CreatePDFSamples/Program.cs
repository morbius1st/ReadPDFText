using CreatePDFSamples.PdfSupport;
using DebugCode;
using Settings;
using ShItextCode;
using ShTempCode.DebugCode;
using UtilityLibrary;


namespace CreatePDFSamples
{
	internal class Program 
	{
		public const string SHEET_METRIC_FOLDER = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\";
		private static string DataFilePath { get; set; }= SHEET_METRIC_FOLDER + "SheetData7.xml";

		private static string SampleTitleBlock { get; set; }
		private static string SamplePdfFilePath { get; set; }

		private static Program p;

		private ShSamples samp;

		private static int choice = 1;  // 1, 2, or 3

		static void Main(string[] args)
		{
			DM.init(5);
			DM.DbxSetIdx(0,0);
			DM.DbxLineEx(0, "start", 0,1);

			p = new Program();

			p.samp = new ShSamples();

			// p.showAlignment();
			//
			// p.showSinAndCos();

			// p.begin(choice);

			p.select();
			// p.beginNewPage();
			p.beginExistPage();

			Console.Write("Waiting| ");

			DM.DbxLineEx(0, "End",-1);

			Console.ReadKey();
		}

		private void show()
		{
			DM.DbxLineEx(0, "start", 0,1);

			ProcessPdfs pp = new ProcessPdfs();
			pp.Show(choice);

			DM.DbxLineEx(0, "End",-1);
		}

		private void beginNewPage()
		{
			DM.DbxLineEx(0, "start", 0,1);

			ProcessPdfs pp = new ProcessPdfs();

			if (pp.ProcessNewPage(DataFilePath, SamplePdfFilePath))
			{
				goodMsg();
			}
			else
			{
				failMsg();
			}

			DM.DbxLineEx(0, "End",-1);
		}

		private void beginExistPage()
		{
			DM.DbxLineEx(0, "start", 0,1);

			ProcessPdfs pp = new ProcessPdfs();

			if (pp.ProcessExistPage(DataFilePath, SamplePdfFilePath, SampleTitleBlock))
			{
				goodMsg();
			}
			else
			{
				failMsg();
			}

			DM.DbxLineEx(0, "End",-1);
		}

		private void select()
		{
			if (samp.SelectScanSample(-1, false) !=true) return;

			DataFilePath = samp.Selected.DataFilePath.FullFilePath;
			SampleTitleBlock = samp.Selected.BlankSamplesFilePath.FullFilePath;
			SamplePdfFilePath = samp.Selected.CreatePdfFilePath.FullFilePath;

			Sample a = samp.Selected;
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

		private void showAlignment()
		{
			PdfShowInfo.StartMsg("Text Alignment", DateTime.Now.ToString(), ShowWhere.DEBUG);

			PdfShowInfo.showAlignment();
		}

	}
}
