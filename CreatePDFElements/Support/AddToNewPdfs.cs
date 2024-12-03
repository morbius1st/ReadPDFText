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
using ShSheetData.Support;
using ShTempCode.DebugCode;

using UtilityLibrary;
#endregion

// user name: jeffs
// created:   7/9/2024 9:00:16 PM

namespace CreatePDFElements.Support
{
	public class AddToNewPdfs
	{
		private SheetData2 sheetRects;
		private SheetRectData2<SheetRectId> rectData;
		private SheetRectData2<SheetRectId> sampleSrd;

		private string pdfFilePath;

		private PdfWriter pw;
		private PdfDocument pdfDoc;
		private Document document;
		private PdfPage pdfPage;
		private PdfCanvas pdfCanvas;

		private float pdfPageRotation;
		private Rectangle pageSize;
		private Rectangle pageSizeWithRotation;

		private int pageNum;
		private int sheetRotation;

		// add sheet information & rectangles to created
		// pdfs
		public FilePath<FileNameSimple> DataFilePath { get; private set; }

		public bool Process(string dataFilePath)
		{
			DM.DbxLineEx(0, "start", 1);

			DataFilePath = new FilePath<FileNameSimple>(dataFilePath);

			init();

			initData();

			beginPdf();

			processSheets();

			endPdf();

			DM.DbxLineEx(0, "end", 0, -1);

			return true;
		}

		private void processSheets()
		{
			DM.DbxLineEx(0, "start", 1);

			foreach (KeyValuePair<string, SheetData2> kvp 
					in SheetDataManager2.Data!.SheetDataList)
			{
				sheetRects = kvp.Value;

				Console.Write($"{sheetRects.Name} ");

				beginPdfPage();

				appendElements();
			}
			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void beginPdf()
		{
			DM.DbxLineEx(0, "start", 1);

			
			pw = new PdfWriter(pdfFilePath);
			pw.SetCompressionLevel(7);
			pw.SetSmartMode(true);

			pdfDoc = new PdfDocument(pw);
			document = new Document(pdfDoc);

			pageNum = 0;

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void beginPdfPage()
		{
			DM.DbxLineEx(0, "start", 1);

			initPdfPage();

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void appendElements()
		{
			DM.DbxLineEx(0, "start", 1);

			CreateElement.PlaceDatum(100, 100, pdfCanvas, 20, DeviceRgb.RED);

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void endPdf()
		{
			pdfPage.Flush();
			pdfDoc.Close();
		}

		private void init()
		{
			pdfFilePath = SheetDataSetConsts.SHEET_DATA_FOLDER+ProcessManager.PDF_FILE_NAME;
		}

		private void initData()
		{
			DM.DbxLineEx(0, "start", 1);

			SheetDataManager2.Init(DataFilePath);

			SheetDataManager2.Read();

			Dictionary<string, SheetData2> a = SheetDataManager2.Data.SheetDataList;

			DM.DbxLineEx(0, "end",0, -1);
		}

		private void initPdfPage()
		{
			DM.DbxLineEx(0, "start", 1);

			PageSize ps;
			sheetRotation = sheetRects.SheetRotation;

			Console.WriteLine($"| page rotation {sheetRotation}");

			if (sheetRotation == 0)
			{
				ps = 
					new PageSize(sheetRects.PageSizeWithRotation.GetWidth(), 
						sheetRects.PageSizeWithRotation.GetHeight());
			}
			else
			{
				ps = 
					new PageSize(sheetRects.PageSizeWithRotation.GetHeight(), 
						sheetRects.PageSizeWithRotation.GetWidth());
			}

			pageNum++;

			pdfPage = pdfDoc.AddNewPage(ps);


			// pdfPage.SetRotation(90);
			// pdfPage.SetRotation(sheetRects.PageRotation==0?90:sheetRects.PageRotation);

			pdfPageRotation = pdfPage.GetRotation();
			pageSize = pdfPage.GetPageSize();
			pageSizeWithRotation = pdfPage.GetPageSizeWithRotation();

			pdfCanvas = new PdfCanvas(pdfPage);

			DM.DbxLineEx(0, "end",0, -1);
			
		}

	}
}
