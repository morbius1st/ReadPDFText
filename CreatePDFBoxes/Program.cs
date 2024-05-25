using System.Runtime.CompilerServices;
using CreatePDFBoxes.PdfSupport;
using CreatePDFBoxes.SheetData;

namespace CreatePDFBoxes
{
	internal class Program
	{
		private static Program p;

		static void Main(string[] args)
		{
			p = new Program();

			p.begin();


			Console.Write("Waiting| ");

			Console.ReadKey();
		}

		private void begin()
		{
			ProcessPdfs pp = new ProcessPdfs();

			pp.Process();
		}
	}
}
