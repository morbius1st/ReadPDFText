#region using

#endregion

// username: jeffs
// created:  6/14/2024 7:56:23 PM

using System;
using System.Diagnostics;
using iText.Kernel.Geom;
using ShSheetData.SheetData;
using UtilityLibrary;

namespace ShItextCode.PdfCalculations
{
	// revised - use these::

	// ha
	// left   = 0 => 0
	// center = 1 => 0.5
	// right  = 2  => 1

	// va
	// top    = 0 => 0;
	// middle = 1 => 0.5
	// bottom = 2 => 1;


	// this is a copy to allow for testing
	// using readPDFManager
	public class PdfCalcTbOrigin
	{
	#region fields

	#endregion

	#region ctor

		public PdfCalcTbOrigin() { }

	#endregion

	#region public properties

	#endregion

	#region private properties

	#endregion

	#region Get Text Box Origin

		// private static SheetRectData<SheetRectId> srdx;

		// private static string rectname;
		// private static string longname;

		private static float sinTb;
		private static float cosTb;

		private static float start;

		private static int factWidx;
		private static int factHidx;
		private static int[] factors ;

		private static float shtR;  // sheet rotation
		private static float tbR;   // text box rotation

		private static int ShtRidx;   // index to the correct array of array of values
		private static int tbRidx;    // index to the correct array of values

		private static int lenXwIdx; // (4) numbers - idx for xw len, xh len, yw len, yh len
		private static int lenXhIdx; // (4) numbers - idx for xw len, xh len, yw len, yh len
		private static int lenYwIdx; // (4) numbers - idx for xw len, xh len, yw len, yh len
		private static int lenYhIdx; // (4) numbers - idx for xw len, xh len, yw len, yh len

		private static int lenWidx;
		private static int lenHidx;
		// actual lengths
		private static float[] lengths;

		public static bool show = false;
		
		// derive the "origin" for a text box based on the 
		// sheet rotation and text box rotation

		public static void GetTextBoxOrigin(
			Rectangle rect, float txBxRotation, float sheetRotation,
			out float x, out float y)
		{
			// srd = sd;
			shtR = sheetRotation;
			tbR = txBxRotation;

			sinTb = (float) Math.Abs(Math.Sin(FloatOps.ToRad(tbR)));
			cosTb = (float) Math.Abs(Math.Cos(FloatOps.ToRad(tbR)));

			// rectname = srd.Id.ToString();
			// longname = srd.InfoText;

			setIndices();
			getAdjFactors();
			getLengthsIndicies(rect);

			if (show)
			{
				Debug.Write($"\tusing new values | factors| {factors[0]} {factors[1]} {factors[2]} {factors[3]}");
				Debug.WriteLine($" | swap X {swapValues[ShtRidx, tbRidx].Item1} | swap y {swapValues[ShtRidx, tbRidx].Item2}");
				Debug.WriteLine($"\tsht rotation {shtR,8:F2} (idx {ShtRidx,2:F0}) tb rotation {tbR,8:F2} (idx {tbRidx,2:F0})");
			}


			start = rect.GetX();
			lenWidx = lenXwIdx;
			lenHidx = lenXhIdx;
			factWidx = 0;
			factHidx = 1;

			float finalx = calcOriginValue();

			start = rect.GetY();
			lenWidx = lenYwIdx;
			lenHidx = lenYhIdx;
			factWidx = 2;
			factHidx = 3;

			float finaly = calcOriginValue();

			// return new Tuple<float, float>(finalx, finaly);

			x = finalx;
			y = finaly;
		}
		
	#region helper routines

		private static float calcOriginValue()
		{
			float val11 = lengths[lenWidx] * cosTb;
			float val21 = val11 * factors[factWidx];

			float val12 = lengths[lenHidx] * sinTb;
			float val22 = val12 * factors[factHidx];

			float answer = start + val21 + val22;

			if (show) showCalc(val11, val21, val12, val22, answer);

			return (float) Math.Round(answer, 2);
		}

		private static void setIndices()
		{
			// determine indices for sheet rotation and for tbRotation
			ShtRidx = (int) ((shtR % 360) / 90);

			tbRidx = ((int) (tbR / 90)) * 2; // 0° = 0, 90°=2, etc

			if (tbR % 90!=0)
			{
				tbRidx++; //  0° = 0, >0&<90 = 1, 90°=2
			}
		}

		private static void getLengthsIndicies(Rectangle rect)
		{
			lengths = new []
			{
				rect.GetWidth(),
				rect.GetHeight()
			};

			bool swapX = swapValues[ShtRidx, tbRidx].Item1;
			bool swapY = swapValues[ShtRidx, tbRidx].Item2;

			lenXwIdx = 0;
			lenXhIdx = 1;
			lenYwIdx = 0;
			lenYhIdx = 1;

			if (swapX)
			{
				lenXwIdx = 1;
				lenXhIdx = 0;
			}

			if (swapY)
			{
				lenYwIdx = 1;
				lenYhIdx = 0;
			}
		}

		private static void getAdjFactors()
		{
			factors = factors1[ShtRidx, tbRidx];
		}

		private static void showCalc(float adj11, float adj21, float adj12, float adj22, float answer)
		{
			Debug.WriteLine($"{" ".Repeat(4)}{start,8:F2} (1 cos long)  {lengths[lenWidx],8:F2} * {cosTb,4:F2} = {adj11,8:F2} * {factors[factWidx],4:F2} = {adj21,8:F2}");
			Debug.WriteLine($"{" ".Repeat(13)}(2 sin short) {lengths[lenHidx],8:F2} * {sinTb,4:F2} = {adj12,8:F2} * {factors[factHidx],4:F2} = {adj22,8:F2}");
			Debug.WriteLine($"{" ".Repeat(13)}(final)       {adj21,8:F2} + {adj22,8:F2} = answer {answer,8:F2}");
		}

	#endregion

	#region static data

		//                   xw   yw (xh==1?0:1 / yh==1?0:1)
		private static Tuple<bool, bool>[,] swapValues = new [,]
		{
			// sheet rotation 0
			{
				new Tuple<bool, bool>(false, true ), // 0 tb rotation 0
				new Tuple<bool, bool>(false, true ), // 1 tb rotation >0 & <90
				new Tuple<bool, bool>(true , false), // 2 tb rotation 90
				new Tuple<bool, bool>(false, true ), // 3 tb rotation >90 & <180
				new Tuple<bool, bool>(false, true ), // 4 tb rotation 180
				new Tuple<bool, bool>(false, true ), // 5 tb rotation >180 & <270
				new Tuple<bool, bool>(true, false ), // 6 tb rotation 270
				new Tuple<bool, bool>(false, true ), // 7 tb rotation >270 & <360
			},

			// sheet rotation 90
			{
				new Tuple<bool, bool>(false, true ), // 0 tb rotation 0
				new Tuple<bool, bool>(false, true ), // 1 tb rotation >0 & <90
				new Tuple<bool, bool>(true , false), // 2 tb rotation 90
				new Tuple<bool, bool>(false, true ), // 3 tb rotation >90 & <180
				new Tuple<bool, bool>(false, true ), // 4 tb rotation 180
				new Tuple<bool, bool>(false, true ), // 5 tb rotation >180 & <270
				new Tuple<bool, bool>(true , false), // 6 tb rotation 270
				new Tuple<bool, bool>(false, true ), // 7 tb rotation >270 & <360
			},

			// sheet rotation 180
			{
				new Tuple<bool, bool>(false, false), // 0 tb rotation 0
				new Tuple<bool, bool>(false, false), // 1 tb rotation >0 & <90
				new Tuple<bool, bool>(false, false), // 2 tb rotation 90
				new Tuple<bool, bool>(false, false), // 3 tb rotation >90 & <180
				new Tuple<bool, bool>(false, false), // 4 tb rotation 180
				new Tuple<bool, bool>(false, false), // 5 tb rotation >180 & <270
				new Tuple<bool, bool>(false, false), // 6 tb rotation 270
				new Tuple<bool, bool>(false, false), // 7 tb rotation >270 & <360
			},

			// sheet rotation 270
			{
				new Tuple<bool, bool>(false, true ), // 0 tb rotation 0
				new Tuple<bool, bool>(false, true ), // 1 tb rotation >0 & <90
				new Tuple<bool, bool>(false, true ), // 2 tb rotation 90
				new Tuple<bool, bool>(false, true ), // 3 tb rotation >90 & <180
				new Tuple<bool, bool>(false, false), // 4 tb rotation 180
				new Tuple<bool, bool>(false, true ), // 5 tb rotation >180 & <270
				new Tuple<bool, bool>(true , false), // 6 tb rotation 270
				new Tuple<bool, bool>(false, true ), // 7 tb rotation >270 & <360
			},
		};



		// basic values - need to be adjusted by horiz alignment & vertical alignment
		// new values
		private static int[,][] factors1 = new[,]
		{
			// sheet rotation == 0
			{
				new int[] {0,1,0,0}, // 0 tb rotation 0
				new int[] {0,1,0,0}, // 1 tb rotation >0 & <90
				new int[] {1,1,1,0}, // 2 tb rotation 90
				new int[] {1,1,1,0}, // 3 tb rotation >90 & <180
				new int[] {1,0,1,1}, // 4 tb rotation 180
				new int[] {1,0,1,1}, // 5 tb rotation >180 & <270
				new int[] {0,0,0,1}, // 6 tb rotation 270
				new int[] {0,0,0,1}, // 7 tb rotation >270 & <360
			},
			
			{
				// sheet rotation == 90
				new int[] {1,0,0,0}, // 0 tb rotation 0
				new int[] {1,1,0,1}, // 1 tb rotation >0 & <90
				new int[] {0,1,0,1}, // 2 tb rotation 90
				new int[] {0,1,1,1}, // 3 tb rotation >90 & <180
				new int[] {0,1,1,1}, // 4 tb rotation 180
				new int[] {0,0,1,0}, // 5 tb rotation >180 & <270
				new int[] {1,0,0,0}, // 6 tb rotation 270
				new int[] {1,0,0,0 }, // 7 tb rotation >270 & <360
			},
			
			{
				// sheet rotation == 180 (not used)
				new int[] {-1, -1, -1, -1}, // 0 tb rotation 0
				new int[] {-1, -1, -1, -1},// 1 tb rotation >0 & <90
				new int[] {-1, -1, -1, -1},// 2 tb rotation 90
				new int[] {-1, -1, -1, -1},// 3 tb rotation >90 & <180
				new int[] {-1, -1, -1, -1}, // 4 tb rotation 180
				new int[] {-1, -1, -1, -1}, // 5 tb rotation >180 & <270
				new int[] {-1, -1, -1, -1}, // 6 tb rotation 270
				new int[] {-1, -1, -1, -1 }, // 7 tb rotation >270 & <360
			},
			
			{
				// sheet rotation == 270
				new int[] {0,0,1,0}, // 0 tb rotation 0
				new int[] {0,0,1,0}, // 1 tb rotation >0 & <90
				new int[] {1,0,0,0}, // 2 tb rotation 90
				new int[] {1,0,0,0}, // 3 tb rotation >90 & <180
				new int[] {1,0,0,1}, // 4 tb rotation 180
				new int[] {1,1,0,1}, // 5 tb rotation >180 & <270
				new int[] {0,1,0,1}, // 6 tb rotation 270
				new int[] {0,1,1,1 }, // 7 tb rotation >270 & <360
			}
		};

	#endregion

	#endregion

	}
}