#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Properties;
using SharedCode;
using ShCode.ShDebugInfo;
using ShCommonCode.ShSheetData;
using ShItextCode.ElementCreation;

#endregion

// user name: jeffs
// created:   5/22/2024 8:45:02 PM

namespace CreatePDFBoxes.PdfSupport
{
	/* box formatting info
	 * str_na                == always ignore (e.g. sheet size)
	 * srt_box               == the perimeter of a box - no text / no link  (e.g. sheet number)
	 * str_text              == where to place some text
	 * str_text_n_box        == where to place some text in a box (e.g. footer / banner) - ** special processing
	 * str_text_link_n_box   == where to place text in a box that has a link
	 * srt_link_n_box        == where to place a box with a link (no text) (e.g. author)
	 * str_location          == box to provide a location - 
	 *
	 *
	 * special processing
	 *  if the border is black and the fill is white, no border just opaque fill (regardless of the fill opaque value)
	 */


	public class CreatePdfSample
	{
		private SheetRects sheetRects;
		private SheetRectData<SheetRectId> rectData;


		private PdfWriter pw;
		private PdfDocument pdfDoc;
		private Document document;
		private PdfPage pdfPage;
		private PdfCanvas pdfCanvas;
		private Canvas canvas;

		private float pdfPageRotation;
		private Rectangle pageSize;
		private Rectangle pageSizeWithRotation;

		private CreateRectangle cr;
		public static CreateText ct;

		private string pdfFilePath;

		private SheetRectData<SheetRectId> sampleSrd;

		private string sampleName;
		private bool specialBorder;

		private int pageNum;


		public CreatePdfSample(string pdfFilePath)
		{
			this.pdfFilePath = pdfFilePath;

			cr = new CreateRectangle();
			ct = new CreateText(null);
		}

		public void BeginSample()
		{
			// create the pdf document and make the pdf writer
			pw = new PdfWriter(pdfFilePath);
			pw.SetCompressionLevel(7);
			pw.SetSmartMode(true);

			pdfDoc = new PdfDocument(pw);
			document = new Document(pdfDoc);

			pageNum = 0;

		}

		public void AppendSample(SheetRects sheetRects)
		{
			PdfShowInfo.StartMsg($"for {sheetRects.Name}");

			this.sheetRects= sheetRects;

			initPdfPage();

			CreateElement.PlaceDatum(0, 0, pdfCanvas, 20, DeviceRgb.RED);

			addAnnotations();

			pdfPage.SetRotation(this.sheetRects.SheetRotation);

		}

		public void CompleteSample()
		{
			pdfPage.Flush();
			pdfDoc.Close();

		}


		public void CreateSample(SheetRects sheetRects)
		{

			PdfShowInfo.StartMsg($"for {sheetRects.Name}");

			this.sheetRects= sheetRects;

			// create the pdf document and make the pdf writer
			pw = new PdfWriter(pdfFilePath);
			pw.SetCompressionLevel(7);
			pw.SetSmartMode(true);

			pdfDoc = new PdfDocument(pw);
			document = new Document(pdfDoc);

			initPdfPage();


			addAnnotations();

			pdfPage.Flush();

			pdfDoc.Close();
		}

		private void initPdfPage()
		{
			PageSize ps;
			int pr = sheetRects.SheetRotation;

			if (pr == 0)
			{
				ps = 
					new PageSize(sheetRects.SheetSizeWithRotation.GetWidth(), 
						sheetRects.SheetSizeWithRotation.GetHeight());
			}
			else
			{
				ps = 
					new PageSize(sheetRects.SheetSizeWithRotation.GetHeight(), 
						sheetRects.SheetSizeWithRotation.GetWidth());
			}

			pageNum++;

			pdfPage = pdfDoc.AddNewPage(ps);

			// pdfPage.SetRotation(90);
			// pdfPage.SetRotation(sheetRects.PageRotation==0?90:sheetRects.PageRotation);

			pdfPageRotation = pdfPage.GetRotation();
			pageSize = pdfPage.GetPageSize();
			pageSizeWithRotation = pdfPage.GetPageSizeWithRotation();

			PdfShowInfo.ShowPageInfo(pageSize, pageSizeWithRotation, pdfPageRotation);

			pdfCanvas = new PdfCanvas(pdfPage);

			initSupport();
			
		}

		private void initSupport()
		{
			CreateSupport.PdfPageRotation = pdfPageRotation;
			CreateSupport.PageSizeWithRotation = pageSizeWithRotation;

			ct.pdfCanvas= pdfCanvas;

		}

		private void addAnnotations()
		{
			// ct.PlaceSheetText(document, 1, makeSheetTitle($"{sheetRects.Name} {sheetRects.Description}"));

			// ct.placeTests1(document, 1, new Rectangle(500f, 300f, 500f, 100f), 0, 10);
			// ct.placeTests1(document, 1, new Rectangle(300f, 500f, 100f, 500f), 90, 20);
			// ct.placeTests1(document, 1, new Rectangle(600f, 600f, 100f, 500f), 38, 20, true);

			// ct.placeTests2(document, pageNum, new Rectangle(194f, 563f, 508f, 3367f), 38, 20, true);
			// ct.placeTest3(document, pageNum);
			//
			// return;
			
			foreach (KeyValuePair<SheetRectId, 
						SheetRectData<SheetRectId>> kvp in sheetRects.ShtRects)
			{
				addAnnotations(kvp);
			}

			foreach (KeyValuePair<SheetRectId, 
						SheetRectData<SheetRectId>> kvp in sheetRects.OptRects)
			{
				addAnnotations(kvp);
			}
		}

		private void addAnnotations(KeyValuePair<SheetRectId, SheetRectData<SheetRectId>> kvp)
		{
			rectData = kvp.Value;

			SheetRectType rt = kvp.Value.Type;

			if (rt == SheetRectType.SRT_NA) return;

			// Debug.WriteLine($"\nmaking this rect| {rectData.Id}  ({rectData.Type})");

			// if (kvp.Key == SheetRectId.SM_WATERMARK1)
			// {
			// 	int a = 1;
			// }

			configSampleData();
			placeRectangleIf();
			placeTextIf();

		}

		private void placeRectangleIf()
		{
			if (sampleSrd.Type == SheetRectType.SRT_TEXT) return;

			cr.PlaceSheetRectangleRaw(pdfCanvas, rectData, rectData.Rect, pdfPage);
		}

		private void placeTextIf()
		{
			ct.PlaceSheetText(document, pageNum, sampleSrd, sheetRects.SheetRotation);
		}

		private void configSampleData()
		{
			sampleSrd = rectData.Clone2();

			sampleName = getName();
			specialBorder = useSpecialBorder();
			SheetRectType t = sampleSrd.Type;

			
			configBorder();
			configText();
			configUrl();

		}

		private string getName()
		{
			string name = SheetRectSupport.GetShtRectName(sampleSrd.Id);

			if (name == null ) name = SheetRectSupport.GetOptRectName(sampleSrd.Id);

			return name;
		}

		private void configBorder()
		{
			if ((sampleSrd.Type != SheetRectType.SRT_TEXT && specialBorder) || 
				sampleSrd.Type == SheetRectType.SRT_BOX)
			{
				sampleSrd.BdrColor = sampleSrd.TextColor;
				sampleSrd.BdrDashPattern = new [] { 5.0f, 3.0f };
				sampleSrd.BdrWidth = 4;
			}
		}

		private void configText()
		{
			if (sampleSrd.Type == SheetRectType.SRT_BOX ||
				sampleSrd.Type == SheetRectType.SRT_LINK_N_BOX ||
				sampleSrd.Type == SheetRectType.SRT_LOCATION) 
			{
				sampleSrd.InfoText = sampleName;
				sampleSrd.FontFamily = "default";
				sampleSrd.TextSize = 10f;
				sampleSrd.TextColor = DeviceRgb.BLACK;
				sampleSrd.TextHorizAlignment = HorizontalAlignment.LEFT;
				sampleSrd.TextVertAlignment = VerticalAlignment.BOTTOM;
			}
		}

		private void configUrl()
		{
			if (sampleSrd.Type == SheetRectType.SRT_TEXT_LINK_N_BOX ||
				sampleSrd.Type == SheetRectType.SRT_LINK_N_BOX)
			{
				sampleSrd.UrlLink = CreateSupport.CS;
			}
		}

		private bool useSpecialBorder()
		{


			return false;
		}

		private SheetRectData<SheetRectId> makeSheetTitle(string title)
		{
			Rectangle r = new Rectangle(250f, 150f, 250f, 50f);

			SheetRectData<SheetRectId> tSrd = new SheetRectData<SheetRectId>(SheetRectType.SRT_TEXT, SheetRectId.SM_PAGE_TITLE, r);

			tSrd.FontFamily= "default";
			tSrd.TextColor = DeviceRgb.RED;
			tSrd.TextHorizAlignment = HorizontalAlignment.LEFT;
			tSrd.TextVertAlignment = VerticalAlignment.BOTTOM;
			tSrd.TextOpacity = 1;
			tSrd.TextSize = 24f;
			tSrd.InfoText =  $"{title}";

			return tSrd;
		}


		public override string ToString()
		{
			return $"for {sheetRects.Name} {sheetRects.Description}";
		}
	}
}
