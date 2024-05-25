using System.Diagnostics;
using System.Text.RegularExpressions;
using ReadPDFTextTests.SheetData;

namespace ReadPDFTextTests
{


	internal class Program
	{
		private static TextExtractTest3  te1;
		// private static  ProcessBoxes pb;
		private static ProcessSheetBoxes psb;

		private static SampleData sd;

		private static Program me;

		static void Main(string[] args)
		{
			me = new Program();
			

			Regex rx = new Regex(@"A\d\.\d\-\d");

			TextExtractorFilter3 filter = new TextExtractorFilter3(rx);

			psb = new ProcessSheetBoxes();
			// pb=new ProcessBoxes();
			te1 = new TextExtractTest3(filter);
			sd=new SampleData();

			// sd.showSample();

			// Debug.WriteLine("\n\n*********  Running test ********* \n");
			//
			// te1.Process(sd.Cl);

			// me.runCharTest();
			// me.runSentenceTest();
			// me.runWordTest();

			psb.Process();

			// pb.Process();

			Console.Write("Waiting| ");

			Console.ReadKey();

		}


		private void runCharTest()
		{
			Debug.WriteLine("\n\n*********  Running Character Test ********* \n");

			te1.Process(sd.Cl);
		}

		private void runSentenceTest()
		{
			Debug.WriteLine("\n\n*********  Running Sentence Test ********* \n");

			te1.Process(sd.Sl);
		}

		private void runWordTest()
		{
			Debug.WriteLine("\n\n*********  Running Word Test ********* \n");

			te1.Process(sd.Wl);
		}



	}
}
