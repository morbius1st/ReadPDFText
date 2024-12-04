#region + Using Directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using ShSheetData.ShSheetData2;
using UtilityLibrary;
using Rectangle = iText.Kernel.Geom.Rectangle;

#endregion

// user name: jeffs
// created:   6/26/2024 9:26:22 PM

namespace ShItextCode.ElementExtraction
{
	public class ExtractSupport
	{

		public string GetSubject(PdfDictionary pd)
		{
			return pd.GetAsString(PdfName.Subj)?.GetValue() ?? null;
		}

		public string GetRectName(PdfAnnotation anno)
		{
			DM.InOut0();

			return anno.GetContents()?.GetValue()?.Trim().ToUpper() ?? null;
		}

		public Rectangle GetAnnoRect(PdfAnnotation anno)
		{
			DM.InOut0();

			return anno.GetRectangle().ToRectangle();
		}

		public string GetUrlText(string subType)
		{
			if (subType == null) return null;

			string result = null;

			int pos3 = subType.IndexOf('[');
			int pos4 = subType.IndexOf("]");

			if (pos4-pos3 > "http://a.com".Length)
			{
				result = subType.Substring(pos3 + 1, pos4 - pos3 - 1);
			}

			return result;
		}





	}
}
