#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Layout.Properties;
using ShSheetData.SheetData;
using ShSheetData.ShSheetData2;
using ShSheetData.Support;
using UtilityLibrary;
using static ShItextCode.Constants;

#endregion

// user name: jeffs
// created:   6/27/2024 7:09:26 PM

namespace ShItextCode.ElementExtraction
{
	public class PdfFreeTextExtract
	{
		private PdfSupport pSupport = new PdfSupport();
		private ExtractSupport exs = new ExtractSupport();

		private PdfDictionary p;
		private string subType;

		private PdfFreeTextAnnotation pfa;

		private TextSettings tsx;
		private BoxSettings bsx;

		// private SheetRectData2<SheetRectId> srd;

		// private SheetRectId id;
		//
		// private SheetRectType type;

		// public bool ExtractData(PdfFreeTextAnnotation pfa, SheetRectData2<SheetRectId> sd)
		public void ExtractPfaData(PdfFreeTextAnnotation pfa, SheetRectType type,
			BoxSettings bs, TextSettings ts)
		{
			DM.Start0();

			bsx = bs;
			tsx = ts;

			// srd = sd;
			this.pfa = pfa;

			adjustRect();

			// Debug.Write(" a");

			if (type == SheetRectType.SRT_NA ||
				type == SheetRectType.SRT_LOCATION)
			{
				DM.End0("end 1");
				return;
			}


			p = pfa.GetPdfObject();
			subType = pfa.GetSubtype().GetValue();

			// Debug.Write(" b");
			if (SheetDataManager2.RectIsType(type, SheetRectType.SRT_BOX)) getBoxData(); // must be first
			// Debug.Write(" c");
			if (SheetDataManager2.RectIsType(type, SheetRectType.SRT_TEXT)) getTextData();
			// Debug.Write(" d");
			if (SheetDataManager2.RectIsType(type, SheetRectType.SRT_LINK)) getUrlText();
			// Debug.Write(" e");

			DM.End0("end");
		}

		private void adjustRect()
		{
			DM.Start0();

			if (bsx.TextBoxRotation % 90 != 0)
			{
				PdfArray pa = PdfSupport.GetBBoxFromAnnotation(pfa);

				if (pa != null)
				{
					Rectangle r = pa.ToRectangle();

					bsx.Rect = new Rectangle(
						bsx.Rect.GetX(), bsx.Rect.GetY(), r.GetWidth(), r.GetHeight());
				}
			}

			DM.End0("end");
		}


		// primary 

		private void getBoxData()
		{
			DM.Start0();
			getFillColor();
			getFillOpacity();
			getBorderData();
			getBorderStyle();

			DM.End0("end");
		}

		private void getTextData()
		{
			DM.Start0();

			getExtraText();
			
			getTextSettings();

			DM.End0("end");
		}

		private void getUrlText()
		{
			DM.InOut0();

			tsx.UrlLink = exs.GetUrlText(subType);
		}


		// box helpers

		private void getFillColor()
		{
			DM.InOut0();

			bsx.FillColor = pSupport.makeColor(pfa.GetColorObject());
		}

		private void getFillOpacity()
		{
			DM.InOut0();

			bsx.FillOpacity =  p.GetAsNumber(new PdfName("FillOpacity"))?.FloatValue() ?? 100;
		}

		private void getBorderData()
		{
			DM.InOut0();

			bsx.BdrColor = pSupport.makeColor(pfa.GetDefaultAppearance());
			bsx.BdrOpacity = pfa.GetStrokingOpacity();
		}

		private void getBorderStyle()
		{
			DM.InOut0();

			PdfDictionary bs = pfa.GetBorderStyle();

			if (bs == null)
			{
				bsx.BdrDashPattern = new [] { 1f };
				bsx.BdrWidth = 1f;
			}

			foreach (KeyValuePair<PdfName, PdfObject> kvp in bs.EntrySet())
			{
				if (kvp.Key.Equals(PdfName.D))
				{
					bsx.BdrDashPattern = ((PdfArray) kvp.Value).ToFloatArray();
				}
				else if (kvp.Key.Equals(PdfName.W))
				{
					bsx.BdrWidth = float.Parse(kvp.Value?.ToString() ?? "1");
				}
			}
		}


		// text style helpers prime

		private void getExtraText()
		{
			DM.InOut0();
			
			tsx.InfoText = null;

			int pos1 = subType.IndexOf('(');
			int pos2 = subType.IndexOf(')');

			if (pos2 - pos1 > 2) tsx.InfoText = subType.Substring(pos1 + 1, pos2 - pos1 - 1);
		}


		// regex: (?<!<p\s*)(?<foo>style="").*?(?<style-foo>"")
		//(?<!<p\s*)style=\"\"(?<style>.*?)\"\"

		private Regex style = new Regex(@"(?<!<p\s*)style=""(?<style>.*?)""");


		public void getTextSettings()
		{
			DM.Start0();

			getStrokeOpacity();

			PdfString ps = pfa.GetPdfObject().GetAsString(PdfName.RC);

			DM.Stat0($"text info string | {ps.GetValue()}");

			Match m = style.Match(ps.GetValue());

			if (!m.Success)
			{
				DM.Stat0($"get style match failed {ps.GetValue()}");
				return;
			}

			string s = m.Groups["style"].Value;
			
			DM.Stat0($"text style string| {s}");

			// string[] ss = ps.GetValue().Split(new [] { '"', '<', '>', '=', ';' });
			string[] ss = s.Split(new [] { '"', '<', '>', '=', ';' });
			string[] s2;

			foreach (string s1 in ss)
			{
				s2 = s1.Split(':');

				if (s2.Length == 2)
				{
					getTextStyleValue(s2);
				}
			}

			DM.End0("end");
		}


		// text helpers secondary

		private void getStrokeOpacity()
		{
			DM.InOut0();

			tsx.TextOpacity = pfa.GetStrokingOpacity();
		}


		// todo add?  "color" "line-height" "margin" "xfa" "font-family" "margin-left" "Margin-right" (etc)
		private void getTextStyleValue(string[] s2)
		{
			DM.Start0($"switching on {s2[0]}");

			switch (s2[0].Trim())
			{
			case "font":
				{
					setFontData(s2[1]);
					break;
				}
			case "font-size":
				{
					getFontSize(s2[1]);
					break;
				}
			case "color":
				{
					DM.InOut0("get color");
					tsx.TextColor = pSupport.makeColor(s2[1]);
					break;
				}
			case "text-align":
				{
					getTextHorizAlign(s2[1]);
					break;
				}
			case "text-valign":
				{
					getTextVertAlign(s2[1]);
					break;
				}
			case "font-weight":
				{
					DM.InOut0("get font-weight");
					if (s2[1] == "bold") tsx.TextWeight = iText.IO.Font.Constants.FontWeights.BOLD;
					break;
				}
			case "font-style":
				{
					DM.InOut0("get font-style");
					if (s2[1] == "italic") tsx.FontStyle = iText.IO.Font.Constants.FontStyles.ITALIC;
					break;
				}
			case "text-decoration":
				{
					getTextDecoration(s2[1]);
					break;
				}
			}

			DM.End0();
		}

		private void getTextHorizAlign(string s2)
		{
			DM.InOut0();

			for (int i = 1; i < TextHorzAlignment.Length; i++)
			{
				if (s2.ToLower().Equals(TextHorzAlignment[i].Item1.ToLower()))
				{
					tsx.TextHorizAlignment= TextHorzAlignment[i].Item2;
					return;
				}
			}

			tsx.TextHorizAlignment = TextHorzAlignment[0].Item2;
		}

		private void getTextVertAlign(string s2)
		{
			DM.InOut0();

			foreach (Tuple<string, VerticalAlignment, char, float> va in TextVertAlignment)
			{
				if (s2.ToLower().Equals(va.Item1.ToLower()))
				{
					tsx.TextVertAlignment = va.Item2;
				}
			}
		}

		private void setFontData(string s2)
		{
			DM.InOut0();

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
					tsx.FontFamily = s2.Trim();
				}
				else
				{
					tsx.FontFamily= s2.Substring(pos2+1,s2.Length-pos2);
				}
			}
			else
			{
				// has >'< so multiple word font name
				pos2 = s2.IndexOf("'", pos1+1);
				if (pos2 != -1)
				{
					tsx.FontFamily = s2.Substring(pos1 + 1, pos2 - pos1 - 1);
				}

				
			}

			if (fontSize > tsx.TextSize)
			{
				tsx.TextSize = fontSize;
			}
		}

		private void getTextDecoration(string s2)
		{
			DM.InOut0();

			string[] s3 = s2.Split(' ');
			int len = s3.Length;

			foreach (string s in s3)
			{
				if (s.Equals("underline"))
				{
					tsx.TextDecoration &= TextDecorations.UNDERLINE;
				}
				else if (s.Equals("line-through"))
				{
					tsx.TextDecoration &= TextDecorations.LINETHROUGH;
				}
			}
		}

		private void getFontSize(string s2)
		{
			DM.InOut0();

			float fontSize = float.Parse(s2.Substring(0, s2.Length - 2));

			if (fontSize > tsx.TextSize)
			{
				tsx.TextSize = fontSize;
			}
		}


		public override string ToString()
		{
			return $"{tsx?.InfoText ?? "null"}";
		}

	}
}
