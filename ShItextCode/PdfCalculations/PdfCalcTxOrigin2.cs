// Solution:     ReadPDFText
// Project:       CreatePDFSamples
// File:             PdfCalcTxOrigin2.cs
// Created:      2024-07-08 (8:49 PM)

using System;
using System.Diagnostics;
using iText.Kernel.Geom;
using ShSheetData.SheetData;
using ShSheetData.ShSheetData2;
using UtilityLibrary;

namespace ShItextCode.PdfCalculations
{
	public class PdfCalcTxOrigin2
	{
		// private static SheetRectData<SheetRectId> srdx;
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

		private static Rectangle rect;

		private static float shtRotation;
		private static float tbRotation;

		private static float TbOriginX;
		private static float TbOriginY;

		private static TextSettings txs;


		public static void GetTextOrigin(Rectangle r, float tox, float toy, float sr, float tr,
			TextSettings ts, out float x, out float y)
		{
			// x / y is the "origin" of the text box - which is the LB corner
			// srd = sd;
			rect = r;
			shtRotation = sr;
			tbRotation = tr;
			TbOriginX = tox;
			TbOriginY = toy;

			txs = ts;

			setAdjustments();
			setValues();

			float mx1 = cosTbA * w1;
			float mx2 = sinTbA * h1;

			float my1 = sinTbA * w1;
			float my2 = cosTbA * h1;


			x1 = TbOriginX + cosTbA * w1;
			x = x1 - sinTbA * h1;

			y1 = TbOriginY + sinTbA * w1;
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

			float xf = TbOriginX + xa - xb;
			float yf = TbOriginY + ya + yb;

			Debug.WriteLine($"\n\t{wAdj:F0} * {cosW:F2} = {xa:F2} | {hAdj:F0} * {sinH:F2} = {xb:F2} | {TbOriginX} + {xa:F2} - {xb:F2} = {xf:F2}");
			Debug.WriteLine($"\t{wAdj:F0} * {sinW:F2} = {ya:F2} | {hAdj:F0} * {cosH:F2} = {yb:F2} | {TbOriginY} + {ya:F2} + {yb:F2} = {yf:F2}");
		}

		private static void showInfo(float x, float y,
			float mx1, float mx2, float my1, float my2
			)
		{
			Debug.Write($"\n\t{w:F2} {h:F2} | {w1:F2} {h1:F2}");
			Debug.Write($" | {wAdj:F2} {hAdj:F2}");
			Debug.Write($" | {txs.TextHorizAlignment.ToString()[0]}-{txs.TextVertAlignment.ToString()[0]}");
			Debug.Write($" | {shtRotation} {tbRotation:F2}");
			Debug.WriteLine($" | sin {sinTbA}  cos {cosTbA}");

			Debug.WriteLine($"\t{TbOriginX:F2} + {w1:F2} * {cosTbA:F2} ({mx1:F2}) = {x1:F2} - {h1:F2} * {sinTbA:F2} ({mx2:F2}) = {x:F2}");
			Debug.WriteLine($"\t{TbOriginY:F2} + {w1:F2} * {sinTbA:F2} ({my1:F2}) = {y1:F2} + {h1:F2} * {cosTbA:F2} ({my2:F2}) = {y:F2}");
		}

		private static void setValues()
		{
			w = rect.GetWidth();
			h = rect.GetHeight();

			if (shtRotation == 0 )
			{
				if ((tbRotation == 90 || tbRotation == 270)) (w, h) = (h, w);
			}
			else if (shtRotation == 90)
			{
				if ((tbRotation != 90 && tbRotation != 270)) (w, h) = (h, w);
			}
			else if (shtRotation == 270)
			{
				if ((tbRotation != 90 && tbRotation != 270)) (w, h) = (h, w);
			}

			w1 = w * wAdj;
			h1 = h * hAdj;

			angleRadA = FloatOps.ToRad(tbRotation + shtRotation);
			// angleB = srd.TextBoxRotation;
			// angleRadB = FloatOps.ToRad(angleB);

			sinTbA = (float) Math.Sin(angleRadA);
			cosTbA = (float) Math.Cos(angleRadA);

			// sinTbB = MathF.Sin(angleRadB);
			// cosTbB = MathF.Cos(angleRadB);
		}

		private static void setAdjustments()
		{
			int idx = (int) txs.TextHorizAlignment;


			wAdj = Constants.TextHorzAlignment[idx].Item4;

			idx = (int) txs.TextVertAlignment;


			hAdj = Constants.TextVertAlignment[idx].Item4;
		}
	}
}