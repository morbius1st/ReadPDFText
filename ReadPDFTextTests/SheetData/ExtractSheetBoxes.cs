#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Path = System.IO.Path;

using SharedCode.ShDataSupport;
using static SharedCode.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Layout.Properties;
using SettingsManager;
using SharedCode.ShPdfSupport;
using ShCommonCode.ShSheetData;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   5/18/2024 10:23:02 AM

namespace ReadPDFTextTests.SheetData
{
	public class ExtractSheetBoxes
	{


		private string rootPath = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\TestBoxes\";
		
		private string[] files = new []
		{
			"TestBoxes.pdf", "A0.1.0 - COVER SHEET.pdf",
			"A2.2-G - LEVEL G - OVERALL FLOOR PLAN.pdf", "TestBoxes 2.pdf"
		};

		private PdfDocument src;
		private string shtDataName;

		private SheetRects sm;
		private SheetRectData<SheetRectId> srd;

		private PdfFreeTextSupport pft;

		public List<Tuple<string, string, Rectangle>> duplicates { get; private set; }
		public List<Tuple<string, string, Rectangle>> extras { get; private set; }

		public string? subject;
		public string rectname;
		public SheetRectId smId;
		public SheetRectType rectType;

		public void PreProcess()
		{
			
			Process(rootPath, files);
		}

		public void Process(string rootFolder, string[] fileList)
		{
			pft = new PdfFreeTextSupport();

			duplicates = new List<Tuple<string, string, Rectangle>>();
			extras = new List<Tuple<string, string, Rectangle>>();

			if (!SheetDataManager.SettingsFileExists)
			{
				Debug.WriteLine("sheet data file does not exist");
			}

			startMsg();

			foreach (string fileName in fileList)
			{
				Console.Write(".");

				src = new PdfDocument(new PdfReader(rootFolder + fileName));

				shtDataName = Path.GetFileNameWithoutExtension(fileName);

				sm = new SheetRects();
				sm.Name = shtDataName;
				sm.Description = $"Sheet Rectangles for {shtDataName}";
				sm.CreatedDt = DateTime.Now;

				scanForSheetBoxes();

				SheetDataManager.Data.SheetRectangles.Add(sm.Name, sm);

				src.Close();

				break;

			}
		}

		private void scanForSheetBoxes()
		{
			PdfPage page = src.GetPage(1);
			sm.PageSizeWithRotation = page.GetPageSizeWithRotation();

			IList<PdfAnnotation> a = page.GetAnnotations();

			foreach (PdfAnnotation anno in a)
			{
				PdfDictionary p = anno.GetPdfObject();

				subject = p.GetAsString(PdfName.Subj)?.GetValue() ?? null;

				if (subject == null || !subject.StartsWith(BOX_SUBJECT))
				{
					continue;
				}

				rectname = anno.GetContents().GetValue().ToUpper();

				rectType = SheetRectSupport.GetRecType(rectname, out smId);

				getAnnoData(anno);
			}
		}

		// private bool getBasicAnnoData(PdfAnnotation anno)
		// {
		// 	bool results = true;
		//
		// 	PdfDictionary p = anno.GetPdfObject();
		//
		// 	subject = p.GetAsString(PdfName.Subj)?.GetValue() ?? null;
		//
		// 	if (subject == null || !subject.StartsWith(BOX_SUBJECT))
		// 	{
		// 		continue;
		// 	}
		//
		// 	rectname = anno.GetContents().GetValue().ToUpper();
		//
		// 	rectType = SheetRectSupport.GetRecType(rectname, out smId);
		//
		//
		// 	return results;
		// }


		private void getAnnoData(PdfAnnotation anno)
		{
			rectname = anno.GetContents().GetValue().ToUpper();

			Rectangle r = anno.GetRectangle().ToRectangle();

			rectType = SheetRectSupport.GetOptRectType(rectname.ToUpper());
			smId = SheetRectSupport.GetOptRectId(rectname);

			if (smId!= SheetRectId.SM_NA) 
			{
				if (addAnnoData(anno, r, sm.OptRects)) return;
			}

			rectType = SheetRectSupport.GetShtRectType(rectname.ToUpper());
			smId = SheetRectSupport.GetShtRectId(rectname);

			addAnnoData(anno, r, sm.ShtRects);
		}

		private bool addAnnoData(PdfAnnotation anno, Rectangle r,
			Dictionary<SheetRectId, SheetRectData<SheetRectId>> rects)
		{
			if (!verifyShtRect(r, rects)) return false;

			srd = new SheetRectData<SheetRectId>(rectType, smId, r);

			srd.Rotation = 360 - (anno.GetPdfObject().GetAsNumber(new PdfName("Rotation"))?.FloatValue() ?? 360);

			getAnnoSpecs((PdfFreeTextAnnotation) anno);

			rects.Add(smId, srd);

			return true;
		}

		private bool verifyShtRect(Rectangle r, Dictionary<SheetRectId, SheetRectData<SheetRectId>> rects)
		{
			if (smId == SheetRectId.SM_NA)
			{
				extras.Add(new Tuple<string, string, Rectangle>(shtDataName, rectname, ProcessSheetBoxes.convertCoordToPage(sm, r)));
				return false;
			}

			if (rects.ContainsKey(smId))
			{
				duplicates.Add(new Tuple<string, string, Rectangle>(shtDataName, rectname, ProcessSheetBoxes.convertCoordToPage(sm, r)));
				return false;
			}

			return true;
		}

		private void getAnnoSpecs(PdfFreeTextAnnotation pfa)
		{
			// all information get loaded into srd;
			// rectname, id, and rectangle loaded when object created;

			// at this point
			// rotation
			// subject set
			// rectType set
			// smid set
			// srd set
			// sm set

			// this sets the below
			pft.ExtractPfaData(pfa, srd);

			// srd.InfoText = null;
			// srd.UrlLink = null;
			// srd.FillColor = ColorConstants.CYAN;
			// srd.FillOpacity = 0.3f;
			// srd.BdrWidth = 0;
			// srd.BdrColor = ColorConstants.BLUE;
			// srd.BdrOpacity = 1f;
			// srd.BdrDashPattern = null;
			// srd.FontFamily = null;
			// srd.TextSize = 0f;
			// srd.TextColor = ColorConstants.BLACK;
			// srd.TextHorizAlignment = TextAlignment.LEFT;
			// srd.TextVertAlignment = VerticalAlignment.MIDDLE;
			// srd.TextWeight = iText.IO.Font.Constants.FontWeights.NORMAL;
			// srd.FontStyle = iText.IO.Font.Constants.FontStyles.NORMAL;
			// srd.TextDecoration = TextDecorations.NORMAL;
			// srd.TextOpacity = 1f;
			// this does the above
			
		}

		public override string ToString()
		{
			return $"this is {nameof(ExtractSheetBoxes)}";
		}

		// temp / debug routines

		private void startMsg()
		{
			Debug.WriteLine("\n\n*************************");
			Debug.WriteLine("***  begin scan boxes ***");
			Debug.WriteLine("*************************\n");
			Debug.WriteLine($"{DateTime.Now}\n");
		}


		/*
		public void showValues()
		{
			foreach (KeyValuePair<string, SheetMetric> kvp in ShtData.Data.SheetMetrics)
			{
				Debug.WriteLine($"sheet name| {kvp.Key}");

				foreach (KeyValuePair<SheetMetricId, ShtRectData<SheetMetricId, Rectangle>> kvp2 in kvp.Value.ShtRects)
				{
					Debug.WriteLine($"\n\tbox name | {kvp2.Key}");

					showBoxValues(kvp2.Value);
				}

				foreach (KeyValuePair<SheetMetricId, ShtRectData<SheetMetricId, Rectangle>> kvp3 in kvp.Value.OptRects)
				{
					Debug.WriteLine($"\n\tbox name | {kvp3.Key}");

					showBoxValues(kvp3.Value);
				}
			}
		}

		private void showBoxValues(ShtRectData<SheetMetricId, Rectangle> box)
		{
			Debug.WriteLine($"\t\tbox id              | {box.Id}");
			Debug.WriteLine($"\t\tbox type            | {box.Type}");
			Debug.WriteLine($"\t\trectangle           | {PdfSupport.FormatRectangle(box.Rect)}");

			if (box.Id == SheetMetricId.SM_XREF)
			{
				showBoundingBoxValues(box);
				return;
			}

			if (box.Type == SheetRectType.SRT_NA ||
				box.Type == SheetRectType.SRT_LOCATION
				) return;


			if (box.HasType(SheetRectType.SRT_BOX))
			{
				showBoundingBoxValues(box);
			}

			if (box.HasType(SheetRectType.SRT_LINK)) 
				Debug.WriteLine($"\tUrlLink             | {box.UrlLink }");

			if (box.HasType(SheetRectType.SRT_TEXT))
			{
				showTextValues(box);
			}
		}

		private void showBoundingBoxValues(ShtRectData<SheetMetricId, Rectangle> box)
		{
			Debug.WriteLine($"\tbounding box info");
			Debug.WriteLine($"\t\tRotation            | {box.Rotation }");
			Debug.WriteLine($"\t\tFillColor           | {box.FillColor }");
			Debug.WriteLine($"\t\tBdrWidth            | {box.BdrWidth }");
			Debug.WriteLine($"\t\tBdrColor            | {box.BdrColor }");
			Debug.WriteLine($"\t\tBdrOpacity          | {box.BdrOpacity }");
			Debug.WriteLine($"\t\tBdrDashPattern      | {box.BdrDashPattern }");
		}

		private void showTextValues(ShtRectData<SheetMetricId, Rectangle> box)
		{
			Debug.WriteLine($"\ttext info");
			Debug.WriteLine($"\t\tInfoText            | {box.InfoText }");
			Debug.WriteLine($"\t\tFontFamily          | {box.FontFamily }");
			Debug.WriteLine($"\t\tFontStyle           | {box.FontStyle }");
			Debug.WriteLine($"\t\tTextSize            | {box.TextSize }");
			Debug.WriteLine($"\t\tTextHorizAlignment  | {box.TextHorizAlignment }");
			Debug.WriteLine($"\t\tTextVertAlignment   | {box.TextVertAlignment }");
			Debug.WriteLine($"\t\tTextWeight          | {box.TextWeight }");
			Debug.WriteLine($"\t\tTextDecoration      | {box.TextDecoration }");
			Debug.WriteLine($"\t\tTextColor           | {box.TextColor }");
			Debug.WriteLine($"\t\tTextOpacity         | {box.TextOpacity }");
		}
		*/

		/* private void createAnnoData2(PdfAnnotation anno)
		{
			rectname = anno.GetContents().GetValue().ToUpper();

			Rectangle r = anno.GetRectangle().ToRectangle();

			rectType = SheetMetricsSupport.GetOptRectType(rectname.ToUpper());
			smId = SheetMetricsSupport.GetOptRectId(rectname);


			if (verifyShtRect(r, sm.OptRects))
			{
				srd = new ShtRectData<SheetMetricId, Rectangle>(rectname, smId, r);
				sm.OptRects.Add(smId, srd);
			}
			else
			{
				rectType = SheetMetricsSupport.GetShtRectType(rectname.ToUpper());
				smId = SheetMetricsSupport.GetShtRectId(rectname);
				
				if (!verifyShtRect(r, sm.ShtRects)) return;

				srd = new ShtRectData<SheetMetricId, Rectangle>(rectname, smId, r);
				sm.ShtRects.Add(smId, srd);
			}
		}
		*/

		/* *******  temp values
		// do not need to save

		

		// extra text / link
		public string extraText;
		public string linkText;

		// text information
		public float fontSize;
		public string fontName;
		public Color txtColor;
		public TextAlignment alignment;
		public VerticalAlignment valignment;
		public bool bold = false;
		public bool italic = false;
		public bool underline = false;
		public bool linethrough = false;


		// box info
		public float boxRotation;
		public Color boxColor;
		public Color bdrColor;
		public float bdrWidth;
		public float[] bdrDashPattern;


		// need to parse
		public float bdrOpacity;
		public float fillOpacity;

		// ********* temp values
		*/

		// util methods

		// private void resetBoxValues()
		// {
		// 	fontSize = 0f;
		// 	fontName = null;
		// 	txtColor = ColorConstants.BLACK;
		// 	alignment = TextAlignment.LEFT;
		// 	valignment = VerticalAlignment.MIDDLE;
		// 	bold = false;
		// 	italic = false;
		// 	underline = false;
		// 	linethrough = false;
		//
		// 	boxColor = ColorConstants.CYAN;
		// 	bdrColor = ColorConstants.BLUE;
		// 	bdrWidth = 0;
		// 	bdrDashPattern = null;
		//
		// 	bdrOpacity = 1f;
		// 	fillOpacity = 0.3f;
		// }

	}
}
