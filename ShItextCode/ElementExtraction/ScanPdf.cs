﻿#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugCode;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using SharedCode.ShPdfSupport;
using ShSheetData.SheetData;
using ShSheetData.SheetData2;
using ShSheetData.ShSheetData2;
using ShSheetData.Support;

using UtilityLibrary;

#endregion

// user name: jeffs
// created:   6/24/2024 8:25:00 PM

namespace ShItextCode.ElementExtraction
{
	public class ScanPdf
	{
		// private ScanStatus ss;

		private SheetData2 sm;
		private SheetRectData2<SheetRectId> srd;

		private ExtractSupport exs;
		private PdfSupport ps; // don't need?
		private PdfFreeTextExtract pftx;

		private PdfDocument src;
		private PdfPage page;
		private PdfDictionary pd;

		private Rectangle pageSize;
		private Rectangle rect;

		private Dictionary<SheetRectId, SheetRectData2<SheetRectId>> rects;

		private IList<PdfAnnotation> annos;

		private string sheetName;
		public string subject;
		public string rectname;
		public SheetRectId smId;
		public SheetRectType rectType;

		public ScanPdf()
		{
			exs = new ExtractSupport();

			pftx = new PdfFreeTextExtract();
		}


		/// <summary>scan a single PDF and extract the needed information and
		/// rectangle and text metrics
		/// </summary>
		/// <param name="file"></param>
		public void ProcessPdf(string file)
		{
			DM.Start0();

			// DM.DbxLineEx(0,"start", 1, 1);

			config(file);
			
			src = new PdfDocument(new PdfReader(file));

			// DM.DbxLineEx(0,$"{sheetName}");

			DM.Stat0($"scanning {sheetName}");

			if (src.GetNumberOfPages() > 1)
			{
				ScanStatus.AddError(sheetName, "Pdf has too many pages", ScanErrorLevel.ERROR_IS_FATAL);

				DM.End0("end 1 - too many pages");

				// DM.DbxLineEx(0,"end 1", -1, -1);
				return;
			}

			scanPdf();

			src.Close();

			if (!checkStatus())
			{
				DM.End0("end 2 - status no good");

				// DM.DbxLineEx(0,"end 2", -1, -1);
				return;
			}

			// DM.DbxLineEx(0,"end", -1, -1);

			DM.End0("end");
		}

		private void config(string file)
		{
			DM.InOut0();

			sheetName = System.IO.Path.GetFileNameWithoutExtension(file);

			sm = new SheetData2(sheetName, $"Sheet Boxes for {sheetName}");
		}

		private bool checkStatus()
		{
			if (!sm.AllShtRectsFound)
			{
				ScanStatus.AddError(sheetName, "Missing one or more boxes", ScanErrorLevel.ERROR_IS_FATAL );
				return false;
			}

			return true;
		}

		private void scanPdf()
		{
			DM.Start0();

			// DM.DbxLineEx(0,"start", 1);

			if (!initSheet())
			{
				ScanStatus.AddError(sheetName, "Page has no annotations to scan", ScanErrorLevel.ERROR_IS_FATAL);
				// DM.DbxLineEx(0,"end 1", -1);

				DM.End0($"end 1 - no annotations");
				return;
			}

			foreach (PdfAnnotation anno in annos)
			{
				pd = anno.GetPdfObject();

				if (!getSubject()) continue;

				// Debug.Write("\n");

				if (!getBasicRectInfo(anno)) continue;

				// Debug.Write(" A passed");

				if (!processRect(anno)) continue;

				// Debug.Write(" B passed");

				getPdfAnnoData(anno);

				// Debug.Write(" C passed");

				rects.Add(smId, srd);
			}

			SheetDataManager2.Data.SheetDataList.Add(sm.Name, sm);

			// DM.DbxLineEx(0,"end", -1);

			DM.End0("end");
		}

		private bool initSheet()
		{
			DM.InOut0("init sheet");

			page = src.GetPage(1);
			pageSize = page.GetPageSize();

			sm.PageSizeWithRotation = page.GetPageSizeWithRotation();
			sm.SheetRotation = page.GetRotation();

			annos = page.GetAnnotations();

			return annos.Count > 0;
		}

		// process information extraction

		private bool processRect(PdfAnnotation anno)
		{
			DM.InOut0();

			rects = smId >= SheetRectId.SM_OPT0 ?  sm.OptRects :  sm.ShtRects;

			return getAnnoData(anno);
		}

		private bool getSubject()
		{
			// DM.InOut0();

			subject = exs.GetSubject(pd);

			// got annotation that is not a box to deal with - normal condition
			if (subject == null || !subject.StartsWith(Constants.BOX_SUBJECT)) return false;

			return true;
		}

		private bool getBasicRectInfo(PdfAnnotation anno)
		{
			DM.Start0();

			rectname = exs.GetRectName(anno);

			if (rectname == null)
			{
				DM.End0("end 1");
				return false;
			}

			rectType = SheetRectConfigDataSupport.GetRecType(rectname, out smId);

			if (rectType == SheetRectType.SRT_NA || smId == SheetRectId.SM_NA)
			{
				DM.End0("end 2");
				return false;
			}

			rect = exs.GetAnnoRect(anno);

			DM.End0("end");

			return true;

		}

		private void getPdfAnnoData(PdfAnnotation anno)
		{
			DM.InOut0();

			pftx.ExtractPfaData((PdfFreeTextAnnotation) anno, 
				srd.Type, srd.BoxSettings, srd.TextSettings);
		}

		private bool getAnnoData(PdfAnnotation anno)
		{
			DM.Start0();

			if (!verifyRect(anno))
			{
				DM.End0("end 1");
				return false;
			}

			srd = new SheetRectData2<SheetRectId>(rectType, smId, rect);

			srd.BoxSettings.TextBoxRotation = 
				360 - (pd.GetAsNumber(new PdfName("Rotation"))?.FloatValue() ?? 360);

			DM.End0("end");

			return true;
		}

		private bool verifyRect(PdfAnnotation anno)
		{
			DM.InOut0();

			if (smId == SheetRectId.SM_NA)
			{
				ScanStatus.AddExtra(sheetName, rectname, 
					ps.convertCoordToPage(sm.PageSizeWithRotation, rect));
				return false;
			}

			if (rects.ContainsKey(smId))
			{
				ScanStatus.AddDup(sheetName, rectname, 
					ps.convertCoordToPage(sm.PageSizeWithRotation, rect));
				return false;
			}

			return true;
		}

		public override string ToString()
		{
			return $"this is {nameof(ScanPdf)}";
		}
	}
}
