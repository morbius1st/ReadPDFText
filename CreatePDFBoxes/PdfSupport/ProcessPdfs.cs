#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreatePDFBoxes.SheetData;
using Settings;
using SettingsManager;
using ShCommonCode.ShSheetData;
using UtilityLibrary;

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

		public string DataFilePath { get; private set; }

		
		public bool Process(string datafilepath)
		{
			DataFilePath= datafilepath;

			initData();

			pdfFilePath = SheetDataSetConsts.SHEET_DATA_FOLDER+PDF_FILE_NAME;

			createPdfSample = new CreatePdfSample(pdfFilePath);

			createPdfSample.BeginSample();

			foreach (KeyValuePair<string, SheetRects> kvp in SheetDataManager.Data!.SheetRectangles)
			{
				sheetRects= kvp.Value;

				createPdfSample.AppendSample(sheetRects);
			}

			createPdfSample.CompleteSample();

			return true;
		}

		private void initData()
		{
			SheetDataManager.Init(new FilePath<FileNameSimple>(DataFilePath));

			SheetDataManager.Read();

			showStatus("@read rects");
		}

		public override string ToString()
		{
			return $"this is {nameof(ProcessPdfs)}";
		}

		public static void showStatus(string msg)
		{
			string qty = $"{SheetDataManager.SheetsCount}";

			Debug.WriteLine($"sheet count| {qty} | ({msg})");
		}
	}
}
