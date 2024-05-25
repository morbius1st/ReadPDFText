#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.ShDataSupport;

#endregion

// user name: jeffs
// created:   4/3/2024 9:22:10 PM

namespace ReadPDFManager.SheetDataSupport
{
	public class SheetDataManager
	{
		public SheetDataManager()
		{

		}

		public void Process()
		{
			SheetConfig.ShowSheetData();
		}


		public override string ToString()
		{
			return $"this is {nameof(SheetDataManager)}";
		}
	}
}
