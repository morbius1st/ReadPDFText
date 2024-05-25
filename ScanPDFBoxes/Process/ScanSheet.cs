#region + Using Directives
using iText.Kernel.Pdf;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using SharedCode.ShPdfSupport;
using ShCommonCode.ShSheetData;

using static SharedCode.Constants;
using Path = System.IO.Path;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Diagnostics;

#endregion

// user name: jeffs
// created:   5/24/2024 8:08:00 PM

namespace ScanPDFBoxes.Process
{
	public class ScanSheet
	{
		private ProcessManager pm;

		private PdfFreeTextSupport pft;
		private PdfSupport ps;

		private SheetRects sm;
		private SheetRectData<SheetRectId> srd;


		private PdfDocument src;
		private PdfPage page;
		private PdfDictionary pd;

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

			pft = new PdfFreeTextSupport();
		}

		public bool ProcessSheet(string file)
		{
			bool result = true;

			// Debug.WriteLine($"@21 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");

			src = new PdfDocument(new PdfReader(file));

			shtDataName = Path.GetFileNameWithoutExtension(file);

			sm = new SheetRects();
			sm.Name = shtDataName;
			sm.Description = $"Sheet Rectangles for {shtDataName}";
			sm.CreatedDt = DateTime.Now;

			// Debug.WriteLine($"@22 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");

			scanSheet();

			SheetDataManager.Data.SheetRectangles.Add(sm.Name, sm);

			// Debug.WriteLine($"@29 {(SheetDataManager.Data?.SheetRectangles?.Count.ToString() ?? "null")}");

			return result;
		}

		private void scanSheet()
		{
			Dictionary<SheetRectId, SheetRectData<SheetRectId>> rects;

			page = src.GetPage(1);
			sm.PageSizeWithRotation = page.GetPageSizeWithRotation();

			IList<PdfAnnotation> a = page.GetAnnotations();

			foreach (PdfAnnotation anno in a)
			{
				if (!getSubject(anno)) continue;

				if (!setBasicAnnoData(anno)) continue;

				if (smId >= SheetRectId.SM_OPT1)
				{
					rects =  sm.OptRects;
					if (!setAnnoData(anno,rects)) continue;
				}
				else
				{
					rects = sm.ShtRects;
					if (!setAnnoData(anno, rects)) continue;
				}

				pft.ExtractPfaData((PdfFreeTextAnnotation) anno, srd);

				rects.Add(smId, srd);
			}
		}

		private bool getSubject(PdfAnnotation anno)
		{
			pd = anno.GetPdfObject();

			subject = pd.GetAsString(PdfName.Subj)?.GetValue() ?? null;

			if (subject == null || !subject.StartsWith(BOX_SUBJECT))
			{
				return false;
			}

			return true;
		}

		private bool setBasicAnnoData(PdfAnnotation anno)
		{
			rectname = anno.GetContents().GetValue().ToUpper();

			if (rectname == null) return false;

			rectType = SheetRectSupport.GetRecType(rectname, out smId);

			if (rectType == SheetRectType.SRT_NA || smId == SheetRectId.SM_NA) return false;

			rect = anno.GetRectangle().ToRectangle();

			return true;
		}

		private bool verifyRect(PdfAnnotation anno, Dictionary<SheetRectId, SheetRectData<SheetRectId>> rects)
		{
			if (smId == SheetRectId.SM_NA)
			{
				pm.extras.Add(new Tuple<string, string, 
					Rectangle>(shtDataName, rectname, ps.convertCoordToPage(sm.PageSizeWithRotation, rect)));
				return false;
			}

			if (rects.ContainsKey(smId))
			{
				pm.duplicates.Add(new Tuple<string, string, 
					Rectangle>(shtDataName, rectname, ps.convertCoordToPage(sm.PageSizeWithRotation, rect)));
				return false;
			}

			return true;
		}

		private bool setAnnoData(PdfAnnotation anno, Dictionary<SheetRectId, SheetRectData<SheetRectId>> rects)
		{
			if (!verifyRect(anno, rects)) return false;

			srd = new SheetRectData<SheetRectId>(rectType, smId, rect);

			srd.Rotation = 360 - (pd.GetAsNumber(new PdfName("Rotation"))?.FloatValue() ?? 360);
			
			return true;
		}

		public override string ToString()
		{
			return $"this is {nameof(ScanSheet)}";
		}

	}
}
