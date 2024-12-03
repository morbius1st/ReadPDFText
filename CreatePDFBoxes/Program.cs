using CreatePDFBoxes.PdfSupport;
using Settings;
using ShItextCode;
using ShTempCode.DebugCode;
using UtilityLibrary;


namespace CreatePDFBoxes
{
	internal class Program
	{
		public const string SHEET_METRIC_FOLDER = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\";
		private static string DataFilePath { get; set; }= SHEET_METRIC_FOLDER + SheetDataSet.DataFileName;


		private static Program p;

		private ShSamples samp;

		static void Main(string[] args)
		{
			p = new Program();

			p.samp = new ShSamples();

			// p.showAlignment();

			// p.showSinAndCos();

			// p.begin();

			p.select();

			Console.Write("Waiting| ");

			Console.ReadKey();
		}

		private void begin()
		{
			ProcessPdfs pp = new ProcessPdfs();

			Console.WriteLine($"\nProcessing| {DataFilePath}\n");

			if (pp.Process(DataFilePath))
			{
				goodMsg();
			}
			else
			{
				failMsg();
			}
		}

		private void select()
		{
			if (samp.SelectAssemblySample(-1, false) !=true) return;

			DataFilePath = samp.Selected.SheetListFilePath.FullFilePath;

			Sample a = samp.Selected;

			begin();
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
