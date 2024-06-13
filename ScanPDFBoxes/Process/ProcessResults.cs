#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using ShCode.ShDebugInfo;
using ShCommonCode.ShSheetData;

#endregion

// user name: jeffs
// created:   5/25/2024 6:12:02 AM

namespace ScanPDFBoxes.Process
{
	public class ProcessResults
	{
		private ProcessManager pm;

		public ProcessResults(ProcessManager pm)
		{
			this.pm = pm;
		}

		// public bool ShowBasicRects { get; set; }
		// public bool ShowRectValues { get; set; }

		// public void ProcessAdd()
		// {
		// 	showAddReport();
		// 	// showBasicRects();
		// 	// showRectValues();
		// }

		// private void showBasicRects()
		// {
		// 	if (!ShowBasicRects) return;
		//
		// 	DebugShowInfo.StartMsg("Basic Rectangles", DateTime.Now.ToString());
		// 	ShowSheetRectInfo.showShtRects();
		// }
		//
		// private void showRectValues()
		// {
		// 	if (!ShowRectValues) return;
		//
		// 	DebugShowInfo.StartMsg("All Rectangle Values", DateTime.Now.ToString());
		// 	ShowSheetRectInfo.ShowValues();
		// }

		// public static void showAddReport()
		// {
		// 	ShowSheetRectInfo.showScanRectReport(pm);
		// }
		//
		// public static void ProcessRemove(int initCount)
		// {
		// 	ShowSheetRectInfo.ShowRemoveReport(pm, initCount);
		// }
	}
}
