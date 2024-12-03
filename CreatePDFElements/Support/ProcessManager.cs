#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugCode;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using Settings;
using ShItextCode.ElementCreation;
using ShSheetData.SheetData;
using ShSheetData.SheetData2;
using ShSheetData.ShSheetData2;
using ShTempCode.DebugCode;

using UtilityLibrary;
#endregion

// user name: jeffs
// created:   7/9/2024 9:00:16 PM

namespace CreatePDFElements.Support
{
	public class ProcessManager
	{
		public const string PDF_FILE_NAME = "Sheet Sample.pdf";

		private AddToNewPdfs addNew;

		private AddToExistPdfs addExist;

		public ProcessManager()
		{
			addNew = new AddToNewPdfs();

			addExist = new AddToExistPdfs();
		}

		public bool AddToNewPdfs(string dataFilePath)
		{
			DM.DbxLineEx(0, "start / end");

			return addNew.Process(dataFilePath);


		}


		public bool AddToExistPdf()
		{
			DM.DbxLineEx(0, "start / end");

			return addExist.Process();
		}

	}
}
