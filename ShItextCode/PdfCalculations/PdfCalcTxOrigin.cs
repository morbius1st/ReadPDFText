#region + Using Directives

using System;
using System.Diagnostics;
using ShSheetData.SheetData;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   6/19/2024 6:26:56 PM

namespace ShItextCode.PdfCalculations
{

	public class PdfCalcTxOrigin
	{
		private static SheetRectData<SheetRectId> srd;
		private static float wAdj;
		private static float hAdj;

		private static float w;
		private static float h;

		private static float w1;
		private static float h1;

		private static float angleRadA;
		private static float sinTbA;
		private static float cosTbA;
		private static float angleB;
		// private static float angleRadB;
		// private static float sinTbB;
		// private static float cosTbB;

		private static float x1;
		private static float y1;

		private static float x2;
		private static float y2;

		public static void GetTextOrigin(SheetRectData<SheetRectId> sd, out float x, out float y)
		{
			// x / y is the "origin" of the text box - which is the LB corner
			srd = sd;

			setAdjustments();
			setValues();

			float mx1 = cosTbA * w1;
			float mx2 = sinTbA * h1;

			float my1 = sinTbA * w1;
			float my2 = cosTbA * h1;


			x1 = srd.TbOriginX + cosTbA * w1;
			x = x1 - sinTbA * h1;

			y1 = srd.TbOriginY + sinTbA * w1;
			y = y1 + cosTbA * h1;

			showInfo(x, y, mx1, mx2, my1, my2);


			float sinW = w * sinTbA;
			float cosW = w * cosTbA;
			float sinH = h * sinTbA;
			float cosH = h * cosTbA;

			float xa = cosW * wAdj;
			float xb = sinH * hAdj;

			float ya = sinW * wAdj;
			float yb = cosH * hAdj;

			float xf = srd.TbOriginX + xa - xb;
			float yf = srd.TbOriginY + ya + yb;

			Debug.WriteLine($"\n\t{wAdj:F0} * {cosW:F2} = {xa:F2} | {hAdj:F0} * {sinH:F2} = {xb:F2} | {srd.TbOriginX} + {xa:F2} - {xb:F2} = {xf:F2}");
			Debug.WriteLine($"\t{wAdj:F0} * {sinW:F2} = {ya:F2} | {hAdj:F0} * {cosH:F2} = {yb:F2} | {srd.TbOriginY} + {ya:F2} + {yb:F2} = {yf:F2}");
		}

		private static void showInfo(float x, float y, 
			float mx1, float mx2, float my1, float my2
			)
		{
			Debug.Write($"\n\t{w:F2} {h:F2} | {w1:F2} {h1:F2}");
			Debug.Write($" | {wAdj:F2} {hAdj:F2}");
			Debug.Write($" | {srd.TextHorizAlignment.ToString()[0]}-{srd.TextVertAlignment.ToString()[0]}");
			Debug.Write($" | {srd.SheetRotation} {srd.TextBoxRotation:F2}");
			Debug.WriteLine($" | sin {sinTbA}  cos {cosTbA}");

			Debug.WriteLine($"\t{srd.TbOriginX:F2} + {w1:F2} * {cosTbA:F2} ({mx1:F2}) = {x1:F2} - {h1:F2} * {sinTbA:F2} ({mx2:F2}) = {x:F2}");
			Debug.WriteLine($"\t{srd.TbOriginY:F2} + {w1:F2} * {sinTbA:F2} ({my1:F2}) = {y1:F2} + {h1:F2} * {cosTbA:F2} ({my2:F2}) = {y:F2}");

			
		}

		private static void setValues()
		{
			w = srd.Rect.GetWidth();
			h = srd.Rect.GetHeight();

			if (srd.SheetRotation == 0 )
			{
				if ((srd.TextBoxRotation == 90 || srd.TextBoxRotation == 270)) (w, h) = (h, w);
			}
			else
			if (srd.SheetRotation == 90)
			{
				if ((srd.TextBoxRotation != 90 && srd.TextBoxRotation != 270)) (w, h) = (h, w);
			}
			else
			if (srd.SheetRotation == 270)
			{
				if ((srd.TextBoxRotation != 90 && srd.TextBoxRotation != 270))(w, h) = (h, w);
			}

			w1 = w * wAdj;
			h1 = h * hAdj;

			angleRadA = FloatOps.ToRad(srd.TextBoxRotation + srd.SheetRotation);
			// angleB = srd.TextBoxRotation;
			// angleRadB = FloatOps.ToRad(angleB);

			sinTbA = (float) Math.Sin(angleRadA);
			cosTbA = (float) Math.Cos(angleRadA);
			
			// sinTbB = MathF.Sin(angleRadB);
			// cosTbB = MathF.Cos(angleRadB);
		}

		private static void setAdjustments()
		{
			int idx = (int) srd.TextHorizAlignment;


			wAdj = Constants.TextHorzAlignment[idx].Item4;

			idx = (int) srd.TextVertAlignment;


			hAdj = Constants.TextVertAlignment[idx].Item4;

		}


	}

}


/*  angle calc
 *        ^
 *  > x > y
 *.    # (x2,y2)
 *.      \
 *.    B° \ (h1)
 *.        * (x1, y1)
 *.       /
 *.  C°  / (w1)
 *    + (x,y) A°
 *    LB
 * A° = tb rotation
 * B° = 90 - A°
 *
 * x1 = x + cos(A) * w1
 * x2 = x1 + sin(B) * h1
 *
 * y1 = y + sin(A) * w1
 * y2 = y1 + cos(B) * h1
 *
 * alt
 * hyp = sqrt(w1^2 + h1^2)
 * C° = ATAN(w1/h1)
 * D° = A + C
 *
 * x2 = x + sin(D)*hyp
 * y2 = y + cos(D)*hyp
 *
 */