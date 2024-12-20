﻿#region + Using Directives
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using SharedCode.ShPdfSupport;
using ShItextCode;
using ShSheetData.SheetData;
using ShSheetData.Support;
using Path = System.IO.Path;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   5/24/2024 8:08:00 PM

namespace ScanPDFBoxes.Process
{
	public class ScanSheet
	{
		private ProcessManager pm;

		private PdfFreeTextRead pft;
		private PdfSupport ps;

		// private SheetRects sm;
		// private SheetRectData<SheetRectId> srd;

		private PdfDocument src;
		private PdfPage page;
		private PdfDictionary pd;

		private Rectangle pageSize;
		private Rectangle rect;

		private string shtDataName;
		public string? subject;
		public string rectname;
		public SheetRectId smId;
		public SheetRectType rectType;

		public ScanSheet(ProcessManager pm)
		{
			this.pm = pm;
			ps = new PdfSupport();

			pft = new PdfFreeTextRead();
		}

		// /// <summary>  scan and extract the information from one sheet
		// /// </summary>
		// public bool ProcessSheet(string file)
		// {
		// 	bool result = true;
		//
		// 	src = new PdfDocument(new PdfReader(file));
		//
		// 	shtDataName = Path.GetFileNameWithoutExtension(file);
		//
		// 	sm = new SheetRects();
		// 	sm.Name = shtDataName;
		// 	sm.Description = $"Sheet Rectangles for {shtDataName}";
		// 	sm.CreatedDt = DateTime.Now;
		//
		// 	scanSheet();
		//
		// 	SheetDataManager.Data.SheetRectangles.Add(sm.Name, sm);
		//
		// 	if (!sm.AllShtRectsFound)
		// 	{
		// 		pm.errors.Add(new Tuple<string?, string?, ErrorLevel>(
		// 			shtDataName, "Missing one or more boxes", ErrorLevel.ERROR_IS_FATAL));
		// 		result = false;
		// 	}
		//
		// 	src.Close();
		//
		// 	return result;
		// }

		// private void scanSheet()
		// {
		// 	Dictionary<SheetRectId, SheetRectData<SheetRectId>> rects;
		//
		// 	page = src.GetPage(1);
		// 	pageSize = page.GetPageSize();
		// 	sm.PageSizeWithRotation = page.GetPageSizeWithRotation();
		// 	sm.SheetRotation = page.GetRotation();
		//
		// 	IList<PdfAnnotation> a = page.GetAnnotations();
		//
		// 	// Debug.Write("\n\n");
		// 	// Debug.WriteLine($"for {sm.Name}");
		// 	// Debug.WriteLine($"page size with rotation | {FormatItextData.FormatRectangle(sm.PageSizeWithRotation)}");
		//
		// 	foreach (PdfAnnotation anno in a)
		// 	{
		// 		if (!getSubject(anno)) continue;
		//
		// 		if (!getBasicAnnoData(anno)) continue;
		//
		// 		if (smId >= SheetRectId.SM_OPT0)
		// 		{
		// 			rects =  sm.OptRects;
		// 			if (!getAnnoData(anno,rects)) continue;
		// 		}
		// 		else
		// 		{
		// 			rects = sm.ShtRects;
		// 			if (!getAnnoData(anno, rects)) continue;
		// 		}
		//
		// 		if (!getPdfAnnoData(anno)) continue;
		//
		// 		// verifyRectSize2();
		//
		// 		rects.Add(smId, srd);
		// 	}
		// }
//
// 		private bool getSubject(PdfAnnotation anno)
// 		{
// 			pd = anno.GetPdfObject();
//
// 			subject = pd.GetAsString(PdfName.Subj)?.GetValue() ?? null;
//
// 			if (subject == null || !subject.StartsWith(Constants.BOX_SUBJECT))
// 			{
// 				return false;
// 			}
//
// 			return true;
// 		}
//
// 		private bool getBasicAnnoData(PdfAnnotation anno)
// 		{
// 			rectname = anno.GetContents().GetValue().Trim().ToUpper();
//
// 			// Debug.WriteLine($"name >{rectname}< | width {rectname.Length}");
//
// 			if (rectname == null) return false;
//
// 			rectType = SheetRectConfigDataSupport.GetRecType(rectname, out smId);
//
// 			if (rectType == SheetRectType.SRT_NA || smId == SheetRectId.SM_NA) return false;
//
//
// 			rect = anno.GetRectangle().ToRectangle();
// 			
// 			
// 			// verifyRectSize();
//
// 			return true;
// 		}
//
// 		private bool getAnnoData(PdfAnnotation anno, Dictionary<SheetRectId, SheetRectData<SheetRectId>> rects)
// 		{
// 			if (!verifyRect(anno, rects)) return false;
//
// 			srd = new SheetRectData<SheetRectId>(rectType, smId, rect);
//
// 			srd.TextBoxRotation = 360 - (pd.GetAsNumber(new PdfName("Rotation"))?.FloatValue() ?? 360);
// 			
// 			return true;
// 		}
//
// 		private bool getPdfAnnoData(PdfAnnotation anno)
// 		{
// 			bool result = true;
//
// 			pft.ExtractPfaData((PdfFreeTextAnnotation) anno, srd);
//
// 			/* eliminated
//
// 			// intent is that the text info and the link info is provided when
// 			// the actual PDF is created and not saved with the sheet design information
//
// 			if (srd.HasType(SheetRectType.SRT_TEXT) && srd.InfoText.IsVoid())
// 			{
// 				pm.errors.Add(new Tuple<string?, string?, ErrorLevel>(
// 					rectname, "Required box text is missing", ErrorLevel.ERROR_IS_FATAL));
// 				result = false;
// 			}
//
// 			if (srd.HasType(SheetRectType.SRT_LINK) && srd.UrlLink.IsVoid())
// 			{
// 				pm.errors.Add(new Tuple<string?, string?, ErrorLevel>(
// 					rectname, "Required URL Link text is missing", ErrorLevel.ERROR_IS_FATAL));
// 				result = false;
// 			}
// 			*/
//
// 			return result;
// 		}

		// private bool verifyRect(PdfAnnotation anno, Dictionary<SheetRectId, SheetRectData<SheetRectId>> rects)
		// {
		// 	// check for duplicate rectangles or extra rectangles
		//
		// 	if (smId == SheetRectId.SM_NA)
		// 	{
		// 		pm.extraRects.Add(new Tuple<string, string, 
		// 			Rectangle>(shtDataName, rectname, ps.convertCoordToPage(sm.PageSizeWithRotation, rect)));
		// 		return false;
		// 	}
		//
		// 	if (rects.ContainsKey(smId))
		// 	{
		// 		pm.duplicateRects.Add(new Tuple<string, string, 
		// 			Rectangle>(shtDataName, rectname, ps.convertCoordToPage(sm.PageSizeWithRotation, rect)));
		// 		return false;
		// 	}
		//
		// 	return true;
		// }
		//
		// private void verifyRectSize()
		// {
		// 	float right = rect.GetX() + rect.GetWidth();
		// 	float top = rect.GetY() + rect.GetHeight();
		//
		// 	if (right <= pageSize.GetWidth() && top <= pageSize.GetHeight()) return;
		//
		// 	float width = right > pageSize.GetWidth() ? pageSize.GetWidth() - rect.GetX() : rect.GetWidth();
		// 	float height = top > pageSize.GetHeight() ? pageSize.GetHeight()-rect.GetY() : rect.GetHeight();
		//
		// 	// this is the correct box when rotation is NEWS but
		// 	// when not, it may need to be adjusted
		// 	rect=new Rectangle(rect.GetX(), rect.GetY(), width, height);
		// 	
		// }
		//
		// private void verifyRectSize2()
		// {
		// 	// Debug.WriteLine($"{rectname,24} | before  {FormatItextData.FormatRectangle(rect)}");
		//
		//
		// 	float right = rect.GetX() + rect.GetWidth();
		// 	float top = rect.GetY() + rect.GetHeight();
		//
		// 	if (right <= pageSize.GetWidth() && top <= pageSize.GetHeight()) return;
		//
		// 	float width = right > pageSize.GetWidth() ? pageSize.GetWidth() - rect.GetX() : rect.GetWidth();
		// 	float height = top > pageSize.GetHeight() ? pageSize.GetHeight()-rect.GetY() : rect.GetHeight();
		//
		// 	// this is the correct box when rotation is NEWS but
		// 	// when not, it may need to be adjusted
		// 	srd.Rect=new Rectangle(rect.GetX(), rect.GetY(), width, height);
		//
		// 	// however, if a rotated text box is being saved, the text box dimensions need
		// 	// to be adjusted to represent the true size
		//
		// 	if (srd.TextBoxRotation % 90 == 0 || !srd.HasType(SheetRectType.SRT_TEXT))
		// 	{
		// 		// Debug.WriteLine($"\t\t\t | after 1 (srd)  {FormatItextData.FormatRectangle(srd.Rect)}");
		// 		// Debug.WriteLine($"\t\t\t | after  (rect)  {FormatItextData.FormatRectangle(rect)}");
		//
		// 		return;
		// 	}
		//
		// 	float rad = FloatOps.ToRad(srd.TextBoxRotation);
		//
		// 	// float t = (float) (srd.TextSize * 1.05);
		// 	float t = (float) (srd.TextSize);
		// 	// float th = (t/ 2);
		// 	float wt = (float) (t * Math.Sin(rad));
		// 	
		// 	float w1 = srd.Rect.GetWidth() - wt;
		//
		// 	float wb = (float) (w1 / Math.Cos(rad));
		// 	float wy = (float) (w1 / Math.Sin(rad));
		//
		// 	float x = srd.Rect.GetX();
		// 	// float y = srd.Rect.GetY() + wy;
		// 	float y = srd.Rect.GetY();
		//
		// 	if (sm.SheetRotation == 270) (wb, t) = (t, wb);
		//
		// 	srd.Rect = new Rectangle(x, y, wb, t);
		//
		// 	// Debug.WriteLine($"\t\t\t | after 2 (srd)  {FormatItextData.FormatRectangle(srd.Rect)}");
		// 	// Debug.WriteLine($"\t\t\t | after  (rect)  {FormatItextData.FormatRectangle(rect)}");
		// }

		public override string ToString()
		{
			return $"this is {nameof(ScanSheet)}";
		}
		
	}
}
