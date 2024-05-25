using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadPDFManager.SheetDataSupport;

namespace ReadPDFManager
{
	internal class Program
	{
		private static Program me;

		private SheetDataManager sdMgr;

		[STAThread]
		static void Main(string[] args)
		{
			me = new Program();

			config();

			process();

		}

		private static void config()
		{
			me.sdMgr = new SheetDataManager();
		}

		private static void process()
		{
			string answer = null;

			int idx = 0;

			try
			{

				switch (idx)
				{
				case 0:
					{
						Console.WriteLine("*** Run test| ProcessA");
						me.ProcessA();
						break;
					}

				}

			}
			catch (Exception e) 
			{
				Console.WriteLine($"error| {e.Message}");
				Console.WriteLine($"error| {e.StackTrace}");
			}

			finally
			{
				Console.Write("press enter to continue| "); 
				answer = Console.ReadLine();				
			}

		}


		private void ProcessA()
		{
			sdMgr.Process();

		}


	}
}
