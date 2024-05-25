#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Drawing.Text;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf.Lookuptype8;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css.Font;
using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Utilities.Encoders;
using ReadPDFTextTests.SheetData;
using SettingsManager;
using SharedCode.ShDataSupport;
using SharedCode.ShPdfSupport;
using ShCommonCode.ShSheetData;
using UtilityLibrary;
using Path = System.IO.Path;

using static SharedCode.Constants;

#endregion

// user name: jeffs
// created:   5/9/2024 7:38:40 PM

namespace ReadPDFTextTests
{
	/*

	public class ScanBoxes
	{
		

		private string rootPath = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\TestBoxes\";

		private string[] files = new []
		{
			"TestBoxes.pdf", "A0.1.0 - COVER SHEET.pdf",
			"A2.2-G - LEVEL G - OVERALL FLOOR PLAN.pdf", "TestBoxes 2.pdf"
		};

		private PdfDocument src;
		private string shtDataName;
		SheetRects sm;

		public List<Tuple<string, string, Rectangle>> duplicates { get; private set; }
		public List<Tuple<string, string, Rectangle>> extras { get; private set; }

		private float pageRotation;

		// do not need to save
		public string? subject;
		public string rectname;

		// general
		// public SheetMetricId smIdx;
		public SheetRectId smId;
		public SheetRectType rectType;

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


		public void process()
		{
			duplicates = new List<Tuple<string, string, Rectangle>>();
			extras = new List<Tuple<string, string, Rectangle>>();

			if (!SheetDataManager.SettingsFileExists)
			{
				Debug.WriteLine("sheet data file does not exist");
			}

			Debug.WriteLine("\n\n*************************");
			Debug.WriteLine("***  begin scan boxes ***");
			Debug.WriteLine("*************************\n");
			Debug.WriteLine($"{DateTime.Now}\n");

			foreach (string file in files)
			{
				Console.Write(".");

				src = new PdfDocument(new PdfReader(rootPath + file));

				shtDataName = Path.GetFileNameWithoutExtension(file);

				Debug.WriteLine($"file| {file} ({shtDataName})");

				sm = new SheetRects();
				sm.Name = shtDataName;
				sm.Description = $"Sheet Rectangles for {shtDataName}";
				sm.CreatedDt = DateTime.Now;

				scanPdfForBoxes();

				SheetDataManager.Data.SheetRectangles.Add(sm.Name, sm);

				src.Close();

				Debug.WriteLine("\n");

				break;
			}
		}

		private void scanPdfForBoxes()
		{
			SheetRectData<SheetRectId> srdMid;
			SheetRectData<int> srdIid;

			PdfPage page = src.GetPage(1);
			sm.PageSizeWithRotation = page.GetPageSizeWithRotation();
			pageRotation = page.GetRotation();

			Debug.WriteLine($"page rotation | {pageRotation}");

			IList<PdfAnnotation> a = page.GetAnnotations();

			foreach (PdfAnnotation anno in a)
			{
				resetBoxValues();

				PdfDictionary p = anno.GetPdfObject();

				subject = p.GetAsString(PdfName.Subj)?.GetValue() ?? null;

				if (subject == null || !subject.StartsWith(BOX_SUBJECT))
				{
					continue;
				}

				scanBox(anno);

// 				
// 				rectname = anno.GetContents().GetValue().ToUpper();
// 				boxRotation= p.GetAsNumber(new PdfName("Rotation"))?.FloatValue() ?? -1;
// 				
// 				Debug.WriteLine($"\n\ngot box| {rectname}");
// 				
// 				srt= SheetMetricsSupport.GetOptRectType(rectname.ToUpper());
// 				
// 				r = anno.GetRectangle().ToRectangle();
// 				
// 				if (addOptRect(r)) continue;
// 				
// 				addShtRect(r);
// 				
// 				saveBoxData(anno, r);
//
//
// 				// Debug.WriteLine($"\nsub type| {anno.GetSubtype().GetValue()}");
// 				// Debug.WriteLine($"contents| {anno.GetContents().GetValue()}");
// 				// Debug.WriteLine($"subject | {anno.GetPdfObject().GetAsString(PdfName.Subj)}");
// 				//
// 				// r = anno.GetRectangle().ToRectangle();
// 				// Debug.WriteLine($"rect| x {r.GetX():F2}| y {r.GetY():F2}| w {r.GetWidth():F2}| h {r.GetHeight()}");
// 				//
// 				// float rotation = anno.GetPdfObject().GetAsNumber(new PdfName("Rotation"))?.FloatValue() ?? -1;
// 				// Debug.WriteLine($"rotation| {rotation}");
// 				// rotation = anno.GetPdfObject().GetAsNumber(new PdfName("/Rotation"))?.FloatValue() ?? -1;
// 				// Debug.WriteLine($"rotation| {rotation}");
//
// 				// ICollection<PdfName> rz = anno.GetPdfObject().KeySet();
// 				//
// 				// foreach (PdfName pdfName in rz)
// 				// {
// 				// 	Debug.WriteLine($">{pdfName.ToString()} == {anno.GetPdfObject().Get(pdfName)}< ");
// 				// }
//
// 				// Debug.WriteLine($"object  | {anno.GetPdfObject().ToString()}");
// 				//
// 				// Debug.WriteLine($"name    | {anno.GetName().GetValue()}");
// 				// Debug.WriteLine($"type    | {anno.GetType().ToString()}");
// 				// Debug.WriteLine($"title   | {anno.GetTitle().GetValue()}");
// 				// Debug.WriteLine($"a state | {anno.GetAppearanceState()?.GetValue() ?? "null"}");
// 				//
// 				// ICollection<KeyValuePair<PdfName, PdfObject>> e = anno.GetNormalAppearanceObject().EntrySet();
// 				//
// 				// foreach (KeyValuePair<PdfName, PdfObject> kvp in e)
// 				// {
// 				// 	Debug.WriteLine($"names  | {kvp.Key} | {kvp.Value.ToString()}");
// 				// 	
// 				// }
// 				
			}
		}

		private void scanBox(PdfAnnotation anno)
		{
			Rectangle r;

			// PdfDictionary p = anno.GetPdfObject();

			rectname = anno.GetContents().GetValue().ToUpper();
			boxRotation =  360 - (anno.GetPdfObject().GetAsNumber(new PdfName("Rotation"))?.FloatValue() ?? 360);
			// boxRotation = boxRotation == 360 ? 0 : boxRotation;

			Debug.WriteLine($"\n\ngot box| {rectname}");

			r = anno.GetRectangle().ToRectangle();

			rectType = SheetRectSupport.GetOptRectType(rectname.ToUpper());
			if (!addOptRect(r))
			{
				rectType = SheetRectSupport.GetShtRectType(rectname.ToUpper());
				if (!addShtRect(r)) return;
			}

			saveBoxData(anno);
		}

		private bool addOptRect(Rectangle r)
		{
			smId = SheetRectSupport.GetOptRectId(rectname);

			if (smId == SheetRectId.SM_NA)
			{
				extras.Add(new Tuple<string, string, Rectangle>(shtDataName, rectname, ProcessBoxes.convertCoordToPage(sm, r)));
				return false;
			}

			if (sm.OptRects.ContainsKey(smId))
			{
				duplicates.Add(new Tuple<string, string, Rectangle>(shtDataName, rectname, ProcessBoxes.convertCoordToPage(sm, r)));
				return false;
			}

			Debug.WriteLine($"of type| {rectType}");

			Debug.WriteLine($"\n{FormatItextData.FormatRectangle(r)}");

			sm.OptRects.Add(smId, new SheetRectData<SheetRectId>(rectname, smId, r));

			return true;
		}

		private bool addShtRect(Rectangle r)
		{
			smId = SheetRectSupport.GetShtRectId(rectname);

			if (smId == SheetRectId.SM_NA)
			{
				extras.Add(new Tuple<string, string, Rectangle>(shtDataName, rectname, ProcessBoxes.convertCoordToPage(sm, r)));
				return false;
			}

			if (sm.ShtRects.ContainsKey(smId))
			{
				duplicates.Add(new Tuple<string, string, Rectangle>(shtDataName, rectname, ProcessBoxes.convertCoordToPage(sm, r)));
				return false;
			}

			Debug.WriteLine($"of type| {rectType}");

			Debug.WriteLine($"\n{FormatItextData.FormatRectangle(r)}");

			sm.ShtRects.Add(smId,  new SheetRectData<SheetRectId>(rectname, smId, r));

			return true;
		}

		private void saveBoxData(PdfAnnotation a)
		{
			// getFreeTextInfo((PdfFreeTextAnnotation) a);

			// rectType = SheetMetricsSupport.GetShtRectType(rectname);
			// SheetMetricId smId = SheetMetricsSupport.GetShtRectId(rectname);

			// ShtRectData<SheetMetricId, Rectangle> srd = new (rectType, smId);

			Debug.WriteLine($"\nfor {rectname}");

			getFreeTextInfo((PdfFreeTextAnnotation) a);

			// PdfDictionary p = a.GetPdfObject();

			// boxRotation= p.GetAsNumber(new PdfName("Rotation"))?.FloatValue() ?? -1;

			// getAnnoInfo(a);

			// ICollection<KeyValuePair<PdfName, PdfObject>> e = p.EntrySet();

			// PdfObject po = p.Get(PdfName.RC);
			//
			// PdfString ps = p.GetAsString(PdfName.RC);
			//
			//
			// string s = ps.GetValue();
			//
			// string[] ss = s.Split(new char[] {'<', '>', '=', ';'});

			// PdfDictionary pd = p.GetAsDictionary(PdfName.RC);
			//
			// ICollection<KeyValuePair<PdfName, PdfObject>> e = pd.EntrySet();
			//
			// foreach (KeyValuePair<PdfName, PdfObject> kvp in e)
			// {
			// 	Debug.WriteLine($"{kvp.Key.ToString()} | {kvp.Value}");
			// }
		}

		private void resetBoxValues()
		{
			// subject = null;
			// rectname = null;

			// extraText = null;
			// linkText = null;

			// boxRotation = 0;
			// smId = SheetMetricId.SM_NA;
			// rectType = SheetRectType.SRT_NA;

			fontSize = 0f;
			fontName = null;
			txtColor = ColorConstants.BLACK;
			alignment = TextAlignment.LEFT;
			valignment = VerticalAlignment.MIDDLE;
			bold = false;
			italic = false;
			underline = false;
			linethrough = false;

			boxColor = ColorConstants.CYAN;
			bdrColor = ColorConstants.BLUE;
			bdrWidth = 0;
			bdrDashPattern = null;

			bdrOpacity = 1f;
			fillOpacity = 0.3f;
		}


		private void getAnnoInfo(PdfAnnotation pa)
		{
			Debug.WriteLine($"{pa.GetContents()            ?.ToString() ?? "null"}");

			Debug.WriteLine($"{pa.GetTitle   ()            ?.ToString() ?? "null"}");

			Debug.WriteLine($"{pa.GetBorder()              ?.ToString() ?? "null"}");
			Debug.WriteLine($"{pa.GetBorder()?.Get(0)      ?.ToString() ?? "null"}");


			Debug.WriteLine($"{pa.GetName()                ?.ToString() ?? "null"}");

			Debug.WriteLine($"{pa.GetAppearanceDictionary()?.ToString() ?? "null"}");
			ICollection<KeyValuePair<PdfName, PdfObject>> e = pa.GetAppearanceDictionary().EntrySet();

			foreach (KeyValuePair<PdfName, PdfObject> kvp in e)
			{
				Debug.WriteLine($"{kvp.Key}  <> {kvp.Value}");
			}


			Debug.WriteLine($"{pa.GetStrokingOpacity()     .ToString()}");
			Debug.WriteLine($"{pa.GetFlags()               .ToString()}");

			Debug.WriteLine($"{pa.GetColorObject()         ?.ToString() ?? "null"}");
			Debug.WriteLine($"{pa.GetColorObject()?.Get(0) ?.ToString() ?? "null"}");
		}

		private void getFreeTextInfo(PdfFreeTextAnnotation pa)
		{
			// the contents of the text
			// Debug.WriteLine($"contents | {pa.GetContents()            ?.ToString() ?? "null"}");

			// the annotation "subject" == josh box
			// Debug.WriteLine($"subject  | {pa.GetSubject()             ?.ToString() ?? "null"}");

			parseSubject(pa.GetSubject().ToString());

			Debug.WriteLine($"extra tx | {(extraText ?? "none")}");
			Debug.WriteLine($"link tx  | {(linkText ?? "none")}");

			// border information -
			// /D array is the dash setting
			// /W is the line width
			parseBorderStyle(pa.GetBorderStyle());

			Debug.WriteLine($"bdr width| {bdrWidth}");
			Debug.WriteLine($"bdr Dash | {showArray(bdrDashPattern ?? null)}");

			// Debug.WriteLine($"bodr sty");
			// showPdfDictionary(pa.GetBorderStyle());

			// the text style information - needs to be parsed
			// Debug.WriteLine($"def style|{pa.GetDefaultStyleString()?.ToString() ?? "null"}");

			bdrColor = makeColor(pa.GetDefaultAppearance());

			// the color for the border - must be parsed
			// Debug.WriteLine($"def apper|{pa.GetDefaultAppearance()?.ToString() ?? "null"}");
			// Debug.WriteLine($"bdr color|{pa.GetDefaultAppearance()?.ToString() ?? "null"}");
			Debug.WriteLine($"bdr color| {showArray((bdrColor?.GetColorValue() ?? null))}");

			// the annotation opacity - but not the fill opacity - applies to the border and text
			bdrOpacity = pa.GetStrokingOpacity();
			Debug.WriteLine($"strk opac| {bdrOpacity.ToString()}");


			boxColor = makeColor(pa.GetColorObject());

			// the fill color - as RGB when count == 3
			Debug.WriteLine($"box color| {showArray(boxColor?.GetColorValue() ?? null)}");


			// fill opacity
			PdfDictionary p = pa.GetPdfObject();
			fillOpacity = p.GetAsNumber(new PdfName("FillOpacity"))?.FloatValue() ?? -1;
			Debug.WriteLine($"fill opac| {fillOpacity.ToString()}");

			Debug.WriteLine($"bx rotat | {boxRotation}");

			getFreeTextStyleInfo(pa);

			Debug.WriteLine($"font name| {fontName}");
			Debug.WriteLine($"font size| {fontSize}");
			Debug.WriteLine($"font bold| {bold}");
			Debug.WriteLine($"font ital| {italic}");
			Debug.WriteLine($"font ulin| {underline}");
			Debug.WriteLine($"font line| {linethrough}");
			Debug.WriteLine($"tx color | {showArray(txtColor?.GetColorValue())}");
			Debug.WriteLine($"tx  align| {alignment}");
			Debug.WriteLine($"tx valign| {valignment}");


// 
//
// 			// need?
// 			Debug.WriteLine($"flags    | {pa.GetFlags()               .ToString()}");
//
// 			Debug.WriteLine($"rich txt | {pa.GetRichText()            ?.ToString() ?? "null"}");
// 			Debug.WriteLine($"app obj  |{pa.GetAppearanceObject(new PdfName("FillOpacity"))?.ToString() ?? "null"}");
//
// 			Debug.WriteLine($"opacity  | {pa.GetOpacity()             ?.ToString() ?? "null"}");
// 			Debug.WriteLine($"nsopacity| {pa.GetNonStrokingOpacity()  .ToString() ?? "null"}");
//
//
// 			Debug.WriteLine($"rotation | {pa.GetRotation()            ?.ToString() ?? "null"}");
// 			Debug.WriteLine($"justif   | {pa.GetJustification()       .ToString()}");
// 			Debug.WriteLine($"bodr eff | {pa.GetBorderEffect()        ?.ToString() ?? "null"}");
// 			Debug.WriteLine($"border   | {pa.GetBorder()              ?.ToString() ?? "null"}");
// 			Debug.WriteLine($"name     | {pa.GetName()                ?.ToString() ?? "null"}");
//
// 			Debug.WriteLine($"app state|{pa.GetAppearanceState()?.ToString() ?? "null"}");
// 			Debug.WriteLine($"norm app |{pa.GetNormalAppearanceObject()?.ToString() ?? "null"}");
// 			showPdfDictionary(pa.GetNormalAppearanceObject());
//
// 			Debug.WriteLine($"appear d |{pa.GetAppearanceDictionary()?.ToString() ?? "null"}");
// 			ICollection<KeyValuePair<PdfName, PdfObject>> e = pa.GetAppearanceDictionary().EntrySet();
//
// 			foreach (KeyValuePair<PdfName, PdfObject> kvp in e)
// 			{
// 				Debug.WriteLine($"{kvp.Key} <> {kvp.Value.GetType()}  <> {kvp.Value}");
// 			}
// 
		}

		private void showPdfDictionary(PdfDictionary pd)
		{
			if (pd == null)
			{
				Debug.WriteLine("is null");
				return;
			}

			foreach (KeyValuePair<PdfName, PdfObject> kvp in pd.EntrySet())
			{
				Debug.WriteLine($"{kvp.Key} <> {kvp.Value}");
			}
		}
// 
// 		private void getFreeTextInfo(PdfAnnotation a)
// 		{
// 			getFreeTextInfo((PdfFreeTextAnnotation) a);
//
// 			PdfDictionary p = a.GetPdfObject();
//
// 			float rotation = p.GetAsNumber(new PdfName("Rotation"))?.FloatValue() ?? -1;
// 			string cont = a.GetContents().ToString();
//
// 			PdfString ps = p.GetAsString(PdfName.RC);
//
// 			string[] s2;
//
// 			string s = ps.GetValue();
//
// 			Debug.WriteLine($"{s}");
//
// 			string[] ss = s.Split(new char[] { '"', '<', '>', '=', ';' });
//
// 			foreach (string s1 in ss)
// 			{
// 				s2 = s1.Split(new [] { ':' });
//
// 				// Debug.WriteLine($">{s1}< ");
//
// 				if (s2.Length == 2)
// 				{
// 					Debug.WriteLine($"{s2[0]} & {s2[1]}");
// 				}
// 			}
//
// 			Debug.WriteLine($"rotation| {(rotation == 360 ? 0 : rotation)}");
// 			Debug.WriteLine($"contents| {cont}");
// 		}
// 

		private void getFreeTextStyleInfo(PdfFreeTextAnnotation a)
		{
			PdfDictionary p = a.GetPdfObject();
			PdfString ps = p.GetAsString(PdfName.RC);

			string[] s2;

			string s = ps.GetValue();

			// Debug.WriteLine($"{s}");

			string[] ss = s.Split(new char[] { '"', '<', '>', '=', ';' });

			foreach (string s1 in ss)
			{
				s2 = s1.Split(new [] { ':' });

				if (s2.Length == 2)
				{
					getTextStyleValue(s2);
				}
			}
		}

		public Tuple<string, TextAlignment>[] textAlignment = new []
		{
			new Tuple<string, TextAlignment>("justified", TextAlignment.JUSTIFIED),
			new Tuple<string, TextAlignment>("left", TextAlignment.LEFT),
			new Tuple<string, TextAlignment>("right", TextAlignment.RIGHT),
			new Tuple<string, TextAlignment>("center", TextAlignment.CENTER),
		};

		public Tuple<string, VerticalAlignment>[] textVAlignment = new []
		{
			new Tuple<string, VerticalAlignment>("top"    , VerticalAlignment.TOP),
			new Tuple<string, VerticalAlignment>("middle" , VerticalAlignment.MIDDLE),
			new Tuple<string, VerticalAlignment>("bottom" , VerticalAlignment.BOTTOM),
		};

		private void parseSubject(string s)
		{
			int pos1 = s.IndexOf('(');
			int pos2 = s.IndexOf(')');
			int pos3 = s.IndexOf('[');
			int pos4 = s.IndexOf(']');

			extraText = null;
			linkText = null; // http://a.com

			if (pos2 - pos1 > 2) extraText = s.Substring(pos1 + 1, pos2 - pos1 - 1);
			if (pos4 - pos3 > 11) linkText = s.Substring(pos3 + 1, pos4 - pos3 - 1);
		}

		private void parseBorderStyle(PdfDictionary bs)
		{
			if (bs == null)
			{
				bdrDashPattern = new [] { 1f };
				bdrWidth = 1f;
			}

			foreach (KeyValuePair<PdfName, PdfObject> kvp in bs.EntrySet())
			{
				if (kvp.Key.Equals(PdfName.D))
				{
					bdrDashPattern = ((PdfArray) kvp.Value).ToFloatArray();
				}
				else if (kvp.Key.Equals(PdfName.W))
				{
					bdrWidth = float.Parse(kvp.Value?.ToString() ?? "1");
				}
			}
		}

		private void getTextStyleValue(string[] s2)
		{
			int len;
			string[] s3;
			bool result;

			switch (s2[0].Trim())
			{
			case "text-align":
				{
					result = false;
					for (int i = 1; i < textAlignment.Length; i++)
					{
						if (s2[1].Equals(textAlignment[i].Item1))
						{
							alignment = textAlignment[i].Item2;
							result = true;
							break;
						}
					}

					if (!result) alignment = textAlignment[0].Item2;
					break;
				}

			case "text-valign":
				{
					foreach (Tuple<string, VerticalAlignment> va in textVAlignment)
					{
						if (va.Item1.Equals(s2[1]))
						{
							valignment = va.Item2;
						}
					}

					break;
				}

			case "font":
				{
					Debug.WriteLine($"{s2[1]}");

					parseFontName(s2[1]);

					// s3 = s2[1].Split(' ');
					// len = s3.Length;
					//
					// fontName = s3[len - 2];
					//
					// int pos = s2[1].LastIndexOf(' ');
					//
					// float fontSize =
					// 	float.Parse(s3[len - 1].Substring(0, s3[len - 1].Length - 2));
					//
					// if (fontSize > this.fontSize)
					// {
					// 	this.fontSize = fontSize;
					// }

					break;
				}

			case "font-weight":
				{
					if (s2[1] == "bold") bold = true;
					break;
				}

			case "font-style":
				{
					if (s2[1] == "italic") italic = true;
					break;
				}

			case "text-decoration":
				{
					s3 = s2[1].Split(' ');
					len = s3.Length;

					foreach (string s in s3)
					{
						if (s.Equals("underline"))
						{
							underline = true;
						}
						else if (s.Equals("line-through"))
						{
							linethrough = true;
						}
					}

					break;
				}

			case "color":
				{
					// color = 
					// int c = Int32.Parse(s2[1].Substring(1), NumberStyles.AllowHexSpecifier);
					txtColor = makeColor(s2[1]);
					break;
				}

			case "font-size":
				{
					float fontSize = float.Parse(s2[1].Substring(0, s2[1].Length - 2));

					if (fontSize > this.fontSize)
					{
						this.fontSize = fontSize;
					}

					break;
				}
			}
		}

		private void parseFontName(string s2)
		{
			// two options
			// font name is one or multiple words
			// if has >'< - multiple work font name

			int pos1 = s2.IndexOf("'");
			float fontSize;
			
			int pos2 = s2.LastIndexOf(' ');

			// string temp = s2.Substring(pos2 + 1, s2.Length - pos2 - 3);

			fontSize =
				float.Parse(s2.Substring(pos2 + 1, s2.Length - pos2 - 3));

			if (pos1 == -1)
			{
				// no multiple word name
				s2 = s2.Substring(0, pos2);
				pos2= s2.LastIndexOf(' ');

				if (pos2 == -1)
				{
					fontName = s2.Trim();
				}
				else
				{
					fontName= s2.Substring(pos2+1,s2.Length-pos2);
				}
			}
			else
			{
				// has >'< so multiple word font name
				pos2 = s2.IndexOf("'", pos1+1);
				if (pos2 != -1)
				{
					fontName = s2.Substring(pos1 + 1, pos2 - pos1 - 1);
				}
			}

			if (fontSize > this.fontSize)
			{
				this.fontSize = fontSize;
			}
		}

		private Color makeColor(string h)
		{
			float[] c = new float[3];
			c[0] = Int32.Parse(h.Substring(1, 2), NumberStyles.AllowHexSpecifier);
			c[1] = Int32.Parse(h.Substring(3, 2), NumberStyles.AllowHexSpecifier);
			c[2] = Int32.Parse(h.Substring(5, 2), NumberStyles.AllowHexSpecifier);

			return Color.CreateColorWithColorSpace(c);
		}

		private Color makeColor(PdfArray pa)
		{
			float[] c = pa.ToFloatArray();

			return Color.CreateColorWithColorSpace(c);
		}

		private Color makeColor(PdfString ps)
		{
			string[] s = ps.ToString().Split(' ');

			float[] c = new float[3];

			c[0] = float.Parse(s[0]);
			c[1] = float.Parse(s[1]);
			c[2] = float.Parse(s[2]);

			return Color.CreateColorWithColorSpace(c);
		}

		private string showArray(float[]? ar)
		{
			if (ar == null)
			{
				return "transparent";
			}

			StringBuilder sb = new StringBuilder();

			foreach (float f in ar)
			{
				sb.Append($"{f:F2} ");
			}

			return sb.ToString();
		}

		public override string ToString()
		{
			return $"this is {nameof(ScanBoxes)}";
		}
	}

	*/
}