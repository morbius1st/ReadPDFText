#region + Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Layout;
using iText.Layout.Properties;

using ShCommonCode.ShSheetData;
using static SharedCode.Constants;

#endregion

// user name: jeffs
// created:   5/17/2024 6:13:49 AM

namespace SharedCode.ShPdfSupport
{

	// to extract information from a PDF
	public class PdfFreeTextRead
	{
		private PdfSupport pSupport = new PdfSupport();

		private PdfDictionary p;
		private string subType;

		private PdfFreeTextAnnotation pfa;
		private SheetRectData<SheetRectId> srd;

		private SheetRectId id;

		private SheetRectType type;

		// set info to default before running
		// preceding routine to create srd and configure
		// with type and rectangle
		public void ExtractPfaData(PdfFreeTextAnnotation pfa, SheetRectData<SheetRectId> srdx)
		{
			this.srd = srdx;

			double rt = srdx.TextBoxRotation;

			if (rt != 0 && rt != 90 && rt != 270)
			{
				PdfArray pa = PdfSupport.GetBBoxFromAnnotation(pfa);

				if (pa != null)
				{
					Rectangle rx = pa.ToRectangle();
					srd.Rect = new Rectangle(srd.Rect.GetX(), srd.Rect.GetY(), rx.GetWidth(), rx.GetHeight());
				}
			}

			id = srd.Id;

			type = srd.Type;

			if (srd.Type == SheetRectType.SRT_NA || 
				srd.Type == SheetRectType.SRT_LOCATION) return;

			this.pfa = pfa;

			p = pfa.GetPdfObject();
			subType = pfa.GetSubtype().GetValue();

			if (srd.HasType(SheetRectType.SRT_BOX)) getBoxData();	// must be first
			if (srd.HasType(SheetRectType.SRT_TEXT)) getTextData();
			if (srd.HasType(SheetRectType.SRT_LINK)) getUrlText();

		}


		// primary
		private void getBoxData()
		{
			getFillColor();
			getFillOpacity();
			getBorderData();
			getBorderStyle();
		}

		private void getTextData()
		{
			getExtraText();
			getTextStyleValues();
		}

		private void getUrlText()
		{
			srd.UrlLink = null; // http://a.com

			int pos3 = subType.IndexOf('[');
			int pos4 = subType.IndexOf(']');

			if (pos4 - pos3 > 11) srd.UrlLink = subType.Substring(pos3 + 1, pos4 - pos3 - 1);
		}


		// text helpers
		private void getExtraText()
		{
			srd.InfoText = null;

			int pos1 = subType.IndexOf('(');
			int pos2 = subType.IndexOf(')');

			if (pos2 - pos1 > 2) srd.InfoText = subType.Substring(pos1 + 1, pos2 - pos1 - 1);
		}

		private void getTextStyleValues()
		{
			SheetRectId idx = id;

			PdfString ps = p.GetAsString(PdfName.RC);

			string[] ss = ps.GetValue().Split(new [] { '"', '<', '>', '=', ';' });
			string[] s2;

			// for now, I do not see that they are independent;
			srd.TextOpacity = pfa.GetStrokingOpacity();

			foreach (string s1 in ss)
			{
				s2 = s1.Split(':');

				if (s2.Length == 2)
				{
					getTextStyleValue(s2);
				}
			}
		}


		// box helpers
		private void getFillColor()
		{
			srd.FillColor = pSupport.makeColor(pfa.GetColorObject());
		}

		private void getFillOpacity()
		{
			srd.FillOpacity =  p.GetAsNumber(new PdfName("FillOpacity"))?.FloatValue() ?? 100;
		}

		private void getBorderData()
		{
			srd.BdrColor = pSupport.makeColor(pfa.GetDefaultAppearance());
			srd.BdrOpacity = pfa.GetStrokingOpacity();
		}

		private void getBorderStyle()
		{
			PdfDictionary bs = pfa.GetBorderStyle();

			if (bs == null)
			{
				srd.BdrDashPattern = new [] { 1f };
				srd.BdrWidth = 1f;
			}

			foreach (KeyValuePair<PdfName, PdfObject> kvp in bs.EntrySet())
			{
				if (kvp.Key.Equals(PdfName.D))
				{
					srd.BdrDashPattern = ((PdfArray) kvp.Value).ToFloatArray();
				}
				else if (kvp.Key.Equals(PdfName.W))
				{
					srd.BdrWidth = float.Parse(kvp.Value?.ToString() ?? "1");
				}
			}
		}


		// text style helpers
		private void getTextStyleValue(string[] s2)
		{
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
					srd.TextColor = pSupport.makeColor(s2[1]);
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
					if (s2[1] == "bold") srd.TextWeight = iText.IO.Font.Constants.FontWeights.BOLD;
					break;
				}
			case "font-style":
				{
					if (s2[1] == "italic") srd.FontStyle = iText.IO.Font.Constants.FontStyles.ITALIC;
					break;
				}
			case "text-decoration":
				{
					getTextDecoration(s2[1]);
					break;
				}
			}
		}

		private void getTextHorizAlign(string s2)
		{
			for (int i = 1; i < TextHorzAlignment.Length; i++)
			{
				if (s2.Equals(TextHorzAlignment[i].Item1))
				{
					srd.TextHorizAlignment= TextHorzAlignment[i].Item2;
					return;
				}
			}

			srd.TextHorizAlignment = TextHorzAlignment[0].Item2;
		}

		private void getTextVertAlign(string s2)
		{
			foreach (Tuple<string, VerticalAlignment> va in TextVertAlignment)
			{
				if (va.Item1.Equals(s2))
				{
					srd.TextVertAlignment = va.Item2;
				}
			}
		}

		private void setFontData(string s2)
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
					srd.FontFamily = s2.Trim();
				}
				else
				{
					srd.FontFamily= s2.Substring(pos2+1,s2.Length-pos2);
				}
			}
			else
			{
				// has >'< so multiple word font name
				pos2 = s2.IndexOf("'", pos1+1);
				if (pos2 != -1)
				{
					srd.FontFamily = s2.Substring(pos1 + 1, pos2 - pos1 - 1);
				}
			}

			if (fontSize > srd.TextSize)
			{
				srd.TextSize = fontSize;
			}
		}

		private void getTextDecoration(string s2)
		{
			string[] s3 = s2.Split(' ');
			int len = s3.Length;

			foreach (string s in s3)
			{
				if (s.Equals("underline"))
				{
					srd.TextDecoration &= TextDecorations.UNDERLINE;
				}
				else if (s.Equals("line-through"))
				{
					srd.TextDecoration &= TextDecorations.LINETHROUGH;
				}
			}
		}

		private void getFontSize(string s2)
		{
			float fontSize = float.Parse(s2.Substring(0, s2.Length - 2));

			if (fontSize >srd.TextSize)
			{
				srd.TextSize = fontSize;
			}
		}


		public override string ToString()
		{
			return $"working on type| {id} | {type}";
		}
	}
}