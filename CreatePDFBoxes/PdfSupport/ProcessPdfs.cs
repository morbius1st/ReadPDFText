#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreatePDFBoxes.SheetData;
using Settings;
using SettingsManager;
using ShCommonCode.ShSheetData;

#endregion

// user name: jeffs
// created:   5/22/2024 8:01:29 PM

namespace CreatePDFBoxes.PdfSupport
{
	public class ProcessPdfs
	{
		private const string PDF_FILE_NAME = "Sheet Sample.pdf";

		private string exampleToCreate = "TestBoxes";


		private CreatePdfSample createPdfSample;

		private SheetRects? sheetRects;

		private string pdfFilePath;

		public void Process()
		{
			pdfFilePath = SheetDataSetConsts.SHEET_DATA_FOLDER+PDF_FILE_NAME;

			if (!getSampleRects()) return;

			createPdfSample = new CreatePdfSample(pdfFilePath);

			if (!createSample()) return;

		}

		private bool getSampleRects()
		{
			bool result;

			ProcessRects pr = new ProcessRects();
			pr.Process();

			sheetRects = pr.GetSheetRects(exampleToCreate);

			if (sheetRects == null) return false;

			return true;
		}

		private bool createSample()
		{
			bool result = true;

			result = createPdfSample.CreateSample(sheetRects);

			return result;
		}


		public override string ToString()
		{
			return $"this is {nameof(ProcessPdfs)}";
		}
	}
}
