using System;
using DebugCode;
using ReadPDFManager.Process;
using ReadPDFManager.SheetDataSupport;

namespace ReadPDFManager
{
	internal class Program
	{
		private static Program me;

		private SheetDataManager sdMgr;
		private TbOriginAdjust toa;
		private TbOriginAdjust2 toa2;

		[STAThread]
		static void Main(string[] args)
		{
			me = new Program();

			config();

			me.processB();
			// me.processC();

			Console.WriteLine("waiting ... ");
			Console.ReadKey();

		}

		private static void config()
		{
			me.sdMgr = new SheetDataManager();
			me.toa = new TbOriginAdjust();
			me.toa2 = new TbOriginAdjust2();
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

		private void processB()
		{
			toa.Process();
		}

		
		// private void processC()
		// {
		// 	toa2.Process();
		// }
	}
}
