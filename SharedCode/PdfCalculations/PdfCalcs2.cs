#region using

#endregion

// username: jeffs
// created:  6/14/2024 7:56:23 PM

using System;
using System.Collections.Generic;
using System.Diagnostics;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Pdf;
using Microsoft.VisualBasic;
using SharedCode;
using ShSheetData.SheetData;
using UtilityLibrary;

// namespace ShItextCode.PdfCalculations
// {
// 	public class test
// 	{
		/*
		public void PlacePolygon10(PdfDocument doc, PdfPage page,
			float x1, float y1)
		{
			Color fColor = DeviceRgb.RED;
			Color bColor = DeviceRgb.BLUE;

			float bdrOp = 0.6f;
			float filOp = 0.2f;

			float bdrWidth = 5.0f;

			float rotation = 45.0f;

			double rotate = FloatOps.ToRad(rotation);

			float x = x1;
			float y = y1;
			float w = 300f;
			float h = 100f;

			float cenX = x + w / 2;
			float cenY = y + h / 2;

			float[] verts = new []
			{
				x          , y,
				x + w - 50f, y,
				x + w      , y + h,
				x + 50f    , y + h
			};

			float[] pathout;

			getOffsetPath(verts, bdrWidth / 2, EndType.CLOSED_POLYGON, out pathout, 10.0f);
			Rectangle rBB = getBoundingRect(pathout);

			float[] mx = new float[6];
			Point[] pxIn = rBB.ToPointsArray();
			Point[] pxOut = new Point[pxIn.Length];
			AffineTransform af = AffineTransform.GetRotateInstance(-rotate, cenX, cenY);

			af.GetMatrix(mx);
			af.Transform(pxIn, 0, pxOut, 0, 4);

			Rectangle rPoly = getBoundingRect(pxOut);

			PdfPolyGeomAnnotation poly =
				PdfPolyGeomAnnotation.CreatePolygon(rBB, verts);

			poly.SetRectangle(new PdfArray(rPoly));

			PdfArray pdfArray = new PdfArray(mx);

			PdfFormXObject xo = new PdfFormXObject(rBB);
			xo.Put(PdfName.Matrix, pdfArray);
			xo.Put(PdfName.FormType, new PdfNumber(1));

			PdfDictionary pds = new PdfDictionary();
			pds.Put(PdfName.Type, PdfName.Border);
			pds.Put(PdfName.W, new PdfNumber(5));
			pds.Put(PdfName.S, PdfAnnotation.STYLE_DASHED);
			pds.Put(PdfName.D, new PdfArray (new float[] { 5, 3, 10, 3 }));

			PdfExtGState gs = new PdfExtGState();
			gs.Put(PdfName.Type, PdfName.ExtGState);
			gs.SetFillOpacity(filOp);
			gs.SetStrokeOpacity(bdrOp);
			gs.SetLineWidth(bdrWidth);

			PdfResources res = new PdfResources();
			res.AddExtGState(gs);

			res.SetProcSet(new PdfArray(new List<PdfObject>() { new PdfName("PDF") } ));

			xo.Put(PdfName.Resources, res.GetPdfObject());

			PdfCanvas canvas = new PdfCanvas(xo, doc);

			canvas.SaveState();
			canvas.SetExtGState(gs);
			canvas.SetLineDash(new float[] { 5, 3, 10, 3 }, 0);
			canvas.SetFillColor(fColor);
			canvas.SetStrokeColor(bColor);
			drawPath(verts, canvas);
			canvas.ClosePathFillStroke();
			canvas.RestoreState();

			poly.SetNormalAppearance(xo.GetPdfObject());
			poly.SetBorderStyle(pds);
			poly.SetColor(bColor);
			poly.SetInteriorColor(fColor.GetColorValue());
			poly.SetStrokingOpacity(bdrOp);
			poly.Put(new PdfName("FillOpacity"), new PdfNumber(filOp));
			poly.Put(new PdfName("Rotation"), new PdfNumber(rotation));
			poly.Put(PdfName.Type, PdfName.Annot);
			poly.SetFlags(PdfAnnotation.PRINT);
			poly.SetDate (new PdfString(new PdfDate(DateAndTime.Now).GetW3CDate()));
			poly.SetName(new PdfString("Poly 10"));

			page.AddAnnotation(poly);
		}

		private void getOffsetPath(float[] verts, double offset,
			EndType et, out float[] pathout, float factor = 1)
		{
			List<List<IntPoint>> pathin = new List<List<IntPoint>>();
			List<IntPoint> pintlist = convertToIntPoints(verts, factor);

			ClipperOffset co = new ClipperOffset();
			co.MiterLimit = offset * 10;
			co.AddPath(pintlist, JoinType.MITER, et);
			co.Execute(ref pathin, offset * factor);

			pathout = convertToFloatArray(pathin[0], factor);
		}

		private List<IntPoint> convertToIntPoints(float[] verts, float factor = 1)
		{
			List<IntPoint> pintlist = new List<IntPoint>();

			for (int i = 0; i < verts.Length; i += 2)
			{
				pintlist.Add(new IntPoint((double) verts[i] * factor, (double) verts[i + 1] * factor));
			}

			return pintlist;
		}

		private float[] convertToFloatArray(List<IntPoint> pathOut, float factor = 1)
		{
			float[] verts = new float[pathOut.Count * 2];

			for (var i = 0; i < pathOut.Count; i++)
			{
				verts[i * 2] = pathOut[i].X / factor;
				verts[i * 2 + 1] = pathOut[i].Y / factor;
			}

			return verts;
		}

		public Rectangle getBoundingRect(float[] verts)
		{
			int count = 0;
			if (verts.Length < 4) return null;

			double minX = verts[0];
			double minY = verts[1];
			double maxX = verts[0];
			double maxY = verts[1];

			for (var i = 2; i < verts.Length; i += 2)
			{
				count++;

				if (verts[i] < minX) minX = verts[i];
				else if (verts[i] > maxX) maxX = verts[i];

				if (verts[i + 1] < minY) minY = verts[i + 1];
				else if (verts[i + 1] > maxY) maxY = verts[i + 1];
			}

			return new Rectangle((float) minX, (float) minY, (float) (maxX - minX), (float) (maxY - minY));
		}

		private static Rectangle getBoundingRect(Point[]  pts)
		{
			if (pts.Length < 2) return null;

			double minX = pts[0].x;
			double minY = pts[0].y;
			double maxX = pts[0].x;
			double maxY = pts[0].y;

			for (var i = 1; i < pts.Length; i++)
			{
				if (pts[i].x < minX) minX = pts[i].x;
				else if (pts[i].x > maxX) maxX = pts[i].x;

				if (pts[i].y < minY) minY = pts[i].y;
				else if (pts[i].y > maxY) maxY = pts[i].y;
			}

			return new Rectangle((float) minX, (float) minY, (float) (maxX - minX), (float) (maxY - minY));
		}

		private void drawPath(float[] verts, PdfCanvas canvas)
		{
			canvas.MoveTo(verts[0], verts[1]);

			for (var i = 2; i < verts.Length; i += 2)
			{
				canvas.LineTo(verts[i], verts[i + 1]);
			}
		}
	}
	*/

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
	//
	// public class PdfCalcs2
	// {
	// #region fields
	//
	// #endregion
	//
	// #region ctor
	//
	// 	public PdfCalcs2() { }
	//
	// #endregion
	//
	//
	// #region Get Text Box Origin
	//
	// 	private static SheetRectData<SheetRectId> srd;
	//
	// 	// private static string rectname;
	// 	// private static string longname;
	//
	// 	private static float sinTb;
	// 	private static float cosTb;
	//
	// 	private static float start;
	//
	// 	private static int factWidx;
	// 	private static int factHidx;
	// 	private static int[] factors ;
	//
	// 	private static float shtR;  // sheet rotation
	// 	private static float tbR;   // text box rotation
	//
	// 	private static int ShtRidx;   // index to the correct array of array of values
	// 	private static int tbRidx;    // index to the correct array of values
	//
	// 	private static int lenXwIdx; // (4) numbers - idx for xw len, xh len, yw len, yh len
	// 	private static int lenXhIdx; // (4) numbers - idx for xw len, xh len, yw len, yh len
	// 	private static int lenYwIdx; // (4) numbers - idx for xw len, xh len, yw len, yh len
	// 	private static int lenYhIdx; // (4) numbers - idx for xw len, xh len, yw len, yh len
	//
	// 	private static int lenWidx;
	// 	private static int lenHidx;
	// 	// actual lengths
	// 	private static float[] lengths;
	// 	
	// 	// derive the "origin" for a text box based on the 
	// 	// sheet rotation and text box rotation
	//
	// 	public static Tuple<float, float> GetTextBoxOrigin(
	// 		SheetRectData<SheetRectId> sd, float sheetRotation)
	// 	{
	// 		srd = sd;
	// 		shtR = sheetRotation;
	// 		tbR = srd.TextBoxRotation;
	//
	// 		sinTb = (float) Math.Abs(Math.Sin(FloatOps.ToRad(tbR)));
	// 		cosTb = (float) Math.Abs(Math.Cos(FloatOps.ToRad(tbR)));
	//
	// 		// rectname = srd.Id.ToString();
	// 		// longname = srd.InfoText;
	//
	// 		setIndices();
	// 		getAdjFactors();
	// 		getLengthsIndicies();
	//
	// 		// Debug.Write($"\tusing new values | factors| {factors[0]} {factors[1]} {factors[2]} {factors[3]}");
	// 		// Debug.WriteLine($" | swap X {swapValues[ShtRidx, tbRidx].Item1} | swap y {swapValues[ShtRidx, tbRidx].Item2}");
	// 		// Debug.WriteLine($"\tsht rotation {shtR,8:F2} (idx {ShtRidx,2:F0}) tb rotation {tbR,8:F2} (idx {tbRidx,2:F0})");
	//
	// 		start = srd.Rect.GetX();
	// 		lenWidx = lenXwIdx;
	// 		lenHidx = lenXhIdx;
	// 		factWidx = 0;
	// 		factHidx = 1;
	//
	// 		float finalx = calcOriginValue();
	//
	// 		start = srd.Rect.GetY();
	// 		lenWidx = lenYwIdx;
	// 		lenHidx = lenYhIdx;
	// 		factWidx = 2;
	// 		factHidx = 3;
	//
	// 		float finaly = calcOriginValue();
	//
	// 		return new Tuple<float, float>(finalx, finaly);
	// 	}
	// 	
	// #region helper routines
	//
	// 	private static float calcOriginValue()
	// 	{
	// 		float val11 = lengths[lenWidx] * cosTb;
	// 		float val21 = val11 * factors[factWidx];
	//
	// 		float val12 = lengths[lenHidx] * sinTb;
	// 		float val22 = val12 * factors[factHidx];
	//
	// 		float answer = start + val21 + val22;
	//
	// 		// showCalc(val11, val21, val12, val22, answer);
	//
	// 		return (float) Math.Round(answer, 2);
	// 	}
	//
	// 	private static void setIndices()
	// 	{
	// 		// determine indices for sheet rotation and for tbRotation
	// 		ShtRidx = (int) ((shtR % 360) / 90);
	//
	// 		tbRidx = ((int) (tbR / 90)) * 2; // 0° = 0, 90°=2, etc
	//
	// 		if (tbR % 90!=0)
	// 		{
	// 			tbRidx++; //  0° = 0, >0&<90 = 1, 90°=2
	// 		}
	// 	}
	//
	// 	private static void getLengthsIndicies()
	// 	{
	// 		lengths = new []
	// 		{
	// 			srd.Rect.GetWidth(),
	// 			srd.Rect.GetHeight()
	// 		};
	//
	// 		bool swapX = swapValues[ShtRidx, tbRidx].Item1;
	// 		bool swapY = swapValues[ShtRidx, tbRidx].Item2;
	//
	// 		lenXwIdx = 0;
	// 		lenXhIdx = 1;
	// 		lenYwIdx = 0;
	// 		lenYhIdx = 1;
	//
	// 		if (swapX)
	// 		{
	// 			lenXwIdx = 1;
	// 			lenXhIdx = 0;
	// 		}
	//
	// 		if (swapY)
	// 		{
	// 			lenYwIdx = 1;
	// 			lenYhIdx = 0;
	// 		}
	// 	}
	//
	// 	private static void getAdjFactors()
	// 	{
	// 		factors = factors1[ShtRidx, tbRidx];
	// 	}
	//
	// 	private static void showCalc(float adj11, float adj21, float adj12, float adj22, float answer)
	// 	{
	// 		Debug.WriteLine($"{" ".Repeat(4)}{start,8:F2} (1 cos long)  {lengths[lenWidx],8:F2} * {cosTb,4:F2} = {adj11,8:F2} * {factWidx,4:F2} = {adj21,8:F2}");
	// 		Debug.WriteLine($"{" ".Repeat(13)}(2 sin short) {lengths[lenHidx],8:F2} * {sinTb,4:F2} = {adj12,8:F2} * {factHidx,4:F2} = {adj22,8:F2}");
	// 		Debug.WriteLine($"{" ".Repeat(13)}(final)       {adj21,8:F2} + {adj22,8:F2} = answer {answer,8:F2}");
	// 	}
	//
	// #endregion
	//
	// #region static data
	//
	// 	//                   xw   yw (xh==1?0:1 / yh==1?0:1)
	// 	private static Tuple<bool, bool>[,] swapValues = new [,]
	// 	{
	// 		// sheet rotation 0
	// 		{
	// 			new Tuple<bool, bool>(false, true ), // 0 tb rotation 0
	// 			new Tuple<bool, bool>(false, true ), // 1 tb rotation >0 & <90
	// 			new Tuple<bool, bool>(true , false), // 2 tb rotation 90
	// 			new Tuple<bool, bool>(false, true ), // 3 tb rotation >90 & <180
	// 			new Tuple<bool, bool>(false, true ), // 4 tb rotation 180
	// 			new Tuple<bool, bool>(false, true ), // 5 tb rotation >180 & <270
	// 			new Tuple<bool, bool>(true, false ), // 6 tb rotation 270
	// 			new Tuple<bool, bool>(false, true ), // 7 tb rotation >270 & <360
	// 		},
	//
	// 		// sheet rotation 90
	// 		{
	// 			new Tuple<bool, bool>(false, true ), // 0 tb rotation 0
	// 			new Tuple<bool, bool>(false, true ), // 1 tb rotation >0 & <90
	// 			new Tuple<bool, bool>(true , false), // 2 tb rotation 90
	// 			new Tuple<bool, bool>(false, true ), // 3 tb rotation >90 & <180
	// 			new Tuple<bool, bool>(false, true ), // 4 tb rotation 180
	// 			new Tuple<bool, bool>(false, true ), // 5 tb rotation >180 & <270
	// 			new Tuple<bool, bool>(true , false), // 6 tb rotation 270
	// 			new Tuple<bool, bool>(false, true ), // 7 tb rotation >270 & <360
	// 		},
	//
	// 		// sheet rotation 180
	// 		{
	// 			new Tuple<bool, bool>(false, false), // 0 tb rotation 0
	// 			new Tuple<bool, bool>(false, false), // 1 tb rotation >0 & <90
	// 			new Tuple<bool, bool>(false, false), // 2 tb rotation 90
	// 			new Tuple<bool, bool>(false, false), // 3 tb rotation >90 & <180
	// 			new Tuple<bool, bool>(false, false), // 4 tb rotation 180
	// 			new Tuple<bool, bool>(false, false), // 5 tb rotation >180 & <270
	// 			new Tuple<bool, bool>(false, false), // 6 tb rotation 270
	// 			new Tuple<bool, bool>(false, false), // 7 tb rotation >270 & <360
	// 		},
	//
	// 		// sheet rotation 270
	// 		{
	// 			new Tuple<bool, bool>(false, true ), // 0 tb rotation 0
	// 			new Tuple<bool, bool>(false, true ), // 1 tb rotation >0 & <90
	// 			new Tuple<bool, bool>(false, true ), // 2 tb rotation 90
	// 			new Tuple<bool, bool>(false, true ), // 3 tb rotation >90 & <180
	// 			new Tuple<bool, bool>(false, true ), // 4 tb rotation 180
	// 			new Tuple<bool, bool>(false, true ), // 5 tb rotation >180 & <270
	// 			new Tuple<bool, bool>(false, true ), // 6 tb rotation 270
	// 			new Tuple<bool, bool>(false, true ), // 7 tb rotation >270 & <360
	// 		},
	// 	};
	//
	//
	//
	// 	// basic values - need to be adjusted by horiz alignment & vertical alignment
	// 	// new values
	// 	private static int[,][] factors1 = new[,]
	// 	{
	// 		// sheet rotation == 0
	// 		{
	// 			new int[] {0,1,0,0}, // 0 tb rotation 0
	// 			new int[] {0,1,0,0}, // 1 tb rotation >0 & <90
	// 			new int[] {1,1,1,0}, // 2 tb rotation 90
	// 			new int[] {1,1,1,0}, // 3 tb rotation >90 & <180
	// 			new int[] {1,0,1,1}, // 4 tb rotation 180
	// 			new int[] {1,0,1,1}, // 5 tb rotation >180 & <270
	// 			new int[] {0,0,0,1}, // 6 tb rotation 270
	// 			new int[] {0,0,0,1}, // 7 tb rotation >270 & <360
	// 		},
	// 		
	// 		{
	// 			// sheet rotation == 90
	// 			new int[] {1,0,0,0}, // 0 tb rotation 0
	// 			new int[] {1,1,0,1}, // 1 tb rotation >0 & <90
	// 			new int[] {0,1,0,0}, // 2 tb rotation 90
	// 			new int[] {0,1,1,1}, // 3 tb rotation >90 & <180
	// 			new int[] {0,1,1,1}, // 4 tb rotation 180
	// 			new int[] {0,0,1,0}, // 5 tb rotation >180 & <270
	// 			new int[] {1,0,0,0}, // 6 tb rotation 270
	// 			new int[] {1,0,0,0 }, // 7 tb rotation >270 & <360
	// 		},
	// 		
	// 		{
	// 			// sheet rotation == 180 (not used)
	// 			new int[] {-1, -1, -1, -1}, // 0 tb rotation 0
	// 			new int[] {-1, -1, -1, -1},// 1 tb rotation >0 & <90
	// 			new int[] {-1, -1, -1, -1},// 2 tb rotation 90
	// 			new int[] {-1, -1, -1, -1},// 3 tb rotation >90 & <180
	// 			new int[] {-1, -1, -1, -1}, // 4 tb rotation 180
	// 			new int[] {-1, -1, -1, -1}, // 5 tb rotation >180 & <270
	// 			new int[] {-1, -1, -1, -1}, // 6 tb rotation 270
	// 			new int[] {-1, -1, -1, -1 }, // 7 tb rotation >270 & <360
	// 		},
	// 		
	// 		{
	// 			// sheet rotation == 270
	// 			new int[] {0,0,1,0}, // 0 tb rotation 0
	// 			new int[] {0,0,1,0}, // 1 tb rotation >0 & <90
	// 			new int[] {1,0,0,0}, // 2 tb rotation 90
	// 			new int[] {1,0,0,0}, // 3 tb rotation >90 & <180
	// 			new int[] {0,1,1,0}, // 4 tb rotation 180
	// 			new int[] {1,1,0,1}, // 5 tb rotation >180 & <270
	// 			new int[] {0,0,1,0}, // 6 tb rotation 270
	// 			new int[] {0,1,1,1 }, // 7 tb rotation >270 & <360
	// 		}
	// 	};
	//
	// #endregion
	//
	// #endregion
	//
	// }
	//
	//
	//
	// public class PdfCalcTxOrigin2
	// {
	// 	private static SheetRectData<SheetRectId> srd;
	// 	private static float wAdj;
	// 	private static float hAdj;
	//
	// 	private static float w;
	// 	private static float h;
	//
	// 	private static float w1;
	// 	private static float h1;
	//
	// 	private static float angleRadA;
	// 	private static float sinTbA;
	// 	private static float cosTbA;
	// 	private static float angleB;
	// 	private static float angleRadB;
	// 	private static float sinTbB;
	// 	private static float cosTbB;
	//
	// 	private static float x1;
	// 	private static float y1;
	//
	// 	private static float x2;
	// 	private static float y2;
	//
	// 	public static void GetTextOrigin(SheetRectData<SheetRectId> sd, 
	// 		float sheetRotation, out float x, out float y)
	// 	{
	// 		// x / y is the "origin" of the text box - which is the LB corner
	// 		srd = sd;
	//
	// 		setAdjustments();
	// 		setValues();
	//
	// 		x1 = srd.Rect.GetX() + cosTbA * w1;
	// 		y1 = srd.Rect.GetY() + sinTbA * w1;
	//
	// 		x = x1 + cosTbB * h1;
	// 		y = y1 + sinTbB * h1;
	//
	// 	}
	//
	// 	private static void setValues()
	// 	{
	// 		w = srd.Rect.GetWidth();
	// 		h = srd.Rect.GetHeight();
	//
	// 		w1 = w * wAdj;
	// 		h1 = h * hAdj;
	//
	// 		angleRadA = FloatOps.ToRad(srd.TextBoxRotation);
	// 		angleB = 90 - srd.TextBoxRotation;
	// 		angleRadB = FloatOps.ToRad(angleB);
	//
	// 		sinTbA = (float) Math.Sin(angleRadA);
	// 		cosTbA = (float) Math.Cos(angleRadA);
	//
	// 		sinTbB = (float) Math.Sin(angleRadB);
	// 		cosTbB = (float) Math.Cos(angleRadB);
	// 	}
	//
	// 	private static void setAdjustments()
	// 	{
	// 		int idx = (int) srd.TextHorizAlignment;
	//
	// 		wAdj = Constants.TextHorzAlignment[idx].Item4;
	//
	// 		idx = (int) srd.TextVertAlignment;
	//
	// 		wAdj = Constants.TextVertAlignment[idx].Item4;
	//
	// 	}
	//
	//
	// }
// }