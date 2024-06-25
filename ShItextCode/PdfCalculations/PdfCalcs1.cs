#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ShCommonCode.ShSheetData;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   6/16/2024 11:23:53 AM

//
// namespace ShItextCode.PdfCalculations.x
// {
// 	// revised - use these - void
//
// 	// ha
// 	// left   = 0 => 0
// 	// center = 1 => 0.5
// 	// right  = 2  => 1
//
// 	// va
// 	// top    = 0 => 0;
// 	// middle = 1 => 0.5
// 	// bottom = 2 => 1;
//
//
// 	// this is a copy to allow for testing
// 	// using readPDFManager
// 	public class PdfCalcs
// 	{
// 	#region fields
//
// 	#endregion
//
// 	#region ctor
//
// 		public PdfCalcs() { }
//
// 	#endregion
//
// 	#region public properties
//
// 	#endregion
//
// 	#region private properties
//
// 	#endregion
//
// 	#region Get Text Box Origin
//
// 		private static SheetRectData<SheetRectId> srd;
//
// 		// private static string rectname;
// 		// private static string longname;
//
// 		private static float sinTb;
// 		private static float cosTb;
//
// 		private static float start;
//
// 		private static float len1;
// 		private static float len2;
// 		private static float len3;
// 		private static float len4;
//
// 		private static float factor1;
// 		private static float factor2;
//
// 		// derive the "origin" for a text box based on the 
// 		// sheet rotation and text box rotation
//
// 		public static void GetTextBoxOrigin(
// 			SheetRectData<SheetRectId> sd, float sheetRotation, 
// 			float sin, float cos,
// 			out float x, out float y)
// 		{
// 			srd = sd;
// 			// sinTb = (float) Math.Abs(Math.Sin(FloatOps.ToRad(srd.TextBoxRotation)));
// 			// cosTb = (float) Math.Abs(Math.Cos(FloatOps.ToRad(srd.TextBoxRotation)));
//
// 			sinTb = MathF.Abs(sin);
// 			cosTb = MathF.Abs(cos);
//
// 			// rectname = srd.Id.ToString();
// 			// longname = srd.InfoText;
//
// 			Tuple<int, int, int, int> factors = getAdjustmentFactors(sheetRotation);
//
// 			// Debug.WriteLine($" | factors| {factors.Item1} {factors.Item2} {factors.Item3} {factors.Item4}");
//
// 			getWH(sheetRotation, out len1, out len2, out len3, out len4);
//
// 			start = srd.Rect.GetX();
// 			factor1 = factors.Item1;
// 			factor2 = factors.Item2;
//
// 			x = calcOriginValue();
//
// 			start = srd.Rect.GetY();
// 			len1 = len3;
// 			len2 = len4;
// 			factor1 = factors.Item3;
// 			factor2 = factors.Item4;
//
// 			y = calcOriginValue();
// 		}
//
// 	#region helper routines
//
// 		private static void getWH(float shtRotation,
// 			out float wx, out float hx,
// 			out float wy, out float hy
// 			)
// 		{
// 			wx = srd.Rect.GetWidth();
// 			hx = srd.Rect.GetHeight();
//
// 			wy = wx;
// 			hy = hx;
//
// 			if (shtRotation == 0)
// 			{
// 				(wy, hy) = (hy, wy);
//
// 				if (srd.TextBoxRotation < 90)
// 				{
// 					(wx, hx) = (hx, wx);
// 				}
// 			}
// 			else
// 			{
// 				if (srd.TextBoxRotation < 90)
// 				{
// 					(wy, hy) = (hy, wy);
// 				}
// 				else
// 				{
// 					(wx, hx) = (hx, wx);
// 				}
// 			}
// 		}
//
// 		private static float calcOriginValue()
// 		{
// 			float val1 = len1 * cosTb * factor1;
// 			float val2 = len2 * sinTb * factor2;
//
// 			float answer = start + val1 + val2;
//
// 			showCalc(val1, val2, answer);
//
// 			return (float) Math.Round(answer, 2);
// 		}
//
// 		private static Tuple<int, int, int, int> getAdjustmentFactors(float shtRotation)
// 		{
// 			int shtIdx = (int) ((shtRotation % 360) / 90);
// 			int tbIdx = (int) ((srd.TextBoxRotation % 360) / 90);
//
// 			return factors[shtIdx, tbIdx];
// 		}
//
// 		private static void showCalc(float adj1, float adj2, float answer)
// 		{
// 			Debug.Write($"\t{start,8:F2} (1) {len1,8:F2} {cosTb,8:F2} {factor1,8:F2} = {adj1,8:F2}");
// 			Debug.Write($"  (2) {len2,8:F2} {sinTb,8:F2} {factor2,8:F2} = {adj2,8:F2}");
// 			Debug.WriteLine($" | = answer {answer,8:F2}");
// 		}
//
// 	#endregion
//
// 	#region static data
//
// 		// basic values - need to be adjusted by horiz alignment & vertical alignment
// 		// used to create "corrections" in the math equations based on the 
// 		// combination of sheet rotation and text box rotation
// 		private static Tuple<int, int, int, int>[,] factors = new[,]
// 		{
// 			{
// 				// sheet rotation == 0
// 				new Tuple<int, int, int, int>(0, 0, 1, 0), // 0 to include 90
// 				new Tuple<int, int, int, int>(1, 0, 0, 0), // 90+ to 180
// 				new Tuple<int, int, int, int>(1, 1, 0, 1), // 180 to 270
// 				new Tuple<int, int, int, int>(0, 1, 1, 1), // 270 to 360
// 			},
//
// 			{
// 				// sheet rotation == 90
// 				new Tuple<int, int, int, int>(0, 1, 0, 0), // 0 to 90
// 				new Tuple<int, int, int, int>(1, 1, 1, 0), // 90 to 180
// 				new Tuple<int, int, int, int>(1, 0, 1, 1), // 180 to 270
// 				new Tuple<int, int, int, int>(0, 0, 0, 1), // 270 to 360
// 			},
//
// 			{
// 				// sheet rotation == 180 (not used)
// 				new Tuple<int, int, int, int>(-1, -1, -1, -1), // 0 to 90
// 				new Tuple<int, int, int, int>(-1, -1, -1, -1), // 90 to 180
// 				new Tuple<int, int, int, int>(-1, -1, -1, -1), // 180 to 270
// 				new Tuple<int, int, int, int>(-1, -1, -1, -1), // 270 to 360
// 			},
//
// 			{
// 				// sheet rotation == 270
// 				new Tuple<int, int, int, int>(1, 0, 1, 1), // 0 to 90
// 				new Tuple<int, int, int, int>(0, 0, 0, 1), // 90 to 180
// 				new Tuple<int, int, int, int>(0, 1, 0, 0), // 180 to 270
// 				new Tuple<int, int, int, int>(1, 1, 1, 0), // 270 to 360
// 			}
// 		};
//
// 	#endregion
//
// 	#endregion
// 	}
// }