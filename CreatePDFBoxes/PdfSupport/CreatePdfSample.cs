#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using ShCommonCode;
using ShCommonCode.ShDebugShow;
using ShCommonCode.ShPdfSupport;
using ShCommonCode.ShSheetData;

#endregion

// user name: jeffs
// created:   5/22/2024 8:45:02 PM

namespace CreatePDFBoxes.PdfSupport
{
	public class CreatePdfSample
	{
		private ShPdfSupport sps;


		private SheetRects sheetRects;
		private SheetRectData<SheetRectId> rectData;

		private PdfWriter pw;
		private PdfDocument pdfDoc;
		private PdfPage pdfPage;
		private PdfCanvas pdfCanvas;
		private Canvas canvas;

		private float pageRotation;
		private Rectangle pageSize;
		private Rectangle pageSizeWithRotation;

		private string pdfFilePath;

		public CreatePdfSample(string pdfFilePath)
		{
			this.pdfFilePath = pdfFilePath;


		}

		public bool CreateSample(SheetRects sheetRects)
		{
			
			DebugShowInfo.StartMsg($"for {sheetRects.Name}");

			bool result = true;

			this.sheetRects= sheetRects;

			// create the pdf document and make the pdf writer
			pw = new PdfWriter(pdfFilePath);
			pw.SetCompressionLevel(7);
			pw.SetSmartMode(true);

			pdfDoc = new PdfDocument(pw);

			initPdf();
			initSupport();

			result = addAnnotations();

			pdfPage.Flush();

			pdfDoc.Close();

			return result;
		}

		private void initPdf()
		{
			PageSize ps = 
				new PageSize(sheetRects.PageSizeWithRotation.GetHeight(), 
					sheetRects.PageSizeWithRotation.GetWidth());

			pdfPage = pdfDoc.AddNewPage(ps);
			pdfPage.SetRotation(90);

			pageRotation = pdfPage.GetRotation();
			pageSize = pdfPage.GetPageSize();
			pageSizeWithRotation = pdfPage.GetPageSizeWithRotation();

			DebugShowInfo.ShowPageInfo(pageSize, pageSizeWithRotation, pageRotation);

			pdfCanvas = new PdfCanvas(pdfPage);
			// canvas = new Canvas(pdfCanvas);
		}

		private void initSupport()
		{
			sps = new ShPdfSupport();

			sps.PageRotation = pageRotation;
			sps.PageSizeWithRotation = pageSizeWithRotation;

		}

		private bool addAnnotations()
		{
			bool result = true;
			PdfRectParams mb;

			foreach (KeyValuePair<SheetRectId, 
						SheetRectData<SheetRectId>> kvp in sheetRects.ShtRects)
			{
				rectData = kvp.Value;
				if (kvp.Value.Type == SheetRectType.SRT_NA) continue;

				Debug.WriteLine($"\nmaking this rect| {rectData.Id}  ({rectData.Type})");

				mb = new PdfRectParams();
				mb.R = rectData.Rect;
				mb.StrokeOpacity = rectData.BdrOpacity;
				mb.StrokeColor = rectData.BdrColor;
				mb.FillOpacity = rectData.FillOpacity;
				mb.FillColor = rectData.FillColor;
				mb.StrokeThickness = rectData.BdrWidth;

				sps.PlaceSheetRectangle(pdfCanvas, mb);

				break;
			}

			return true;
		}



		public override string ToString()
		{
			return $"this is {nameof(CreatePdfSample)}";
		}
	}
}
