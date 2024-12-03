#region + Using Directives

using System.Collections.Generic;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using ShSheetData.SheetData;
using ShSheetData.ShSheetData2;
using ShSheetData.Support;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   5/27/2024 10:13:51 AM

namespace ShItextCode.ElementCreation
{
	public static class CreateSupport
	{
		public const string CS = @"https://www.cyberstudio.pro/";
		public const string DefaultFontFile = @"c:\windows\fonts\arialn.ttf";
		public static PdfFont DefaultFont;


		static CreateSupport()
		{
			DefaultFont = PdfFontFactory.CreateFont(DefaultFontFile);
		}

		/*// page support*/

		public static float PdfPageRotation { get; set; }
		public static Rectangle PageSizeWithRotation { get; set; }

		/*// rectangle support*/

		public static void GetRectCenter(Rectangle r, out float x, out float y)
		{
			x = r.GetX() + r.GetWidth() / 2;
			y = r.GetY() + r.GetHeight() / 2;
		}

		public static Rectangle rotatSheetRectangleIfNeeded(Rectangle r)
		{
			return r;
		}

		public static Rectangle rotatSheetRectangleIfNeeded3(Rectangle r)
		{
			if (PdfPageRotation == 0) return r;

			Rectangle rr;
			Rectangle ps = PageSizeWithRotation;

			// if (PageRotation == 90)
			if (PdfPageRotation != 0)
			{
				rr = new Rectangle(ps.GetHeight() - r.GetY(), r.GetX(), -1 * r.GetHeight(), r.GetWidth());
			}
			else
			{
				rr = new Rectangle(r.GetY(), ps.GetWidth() - r.GetX(), r.GetHeight(), -1 * r.GetWidth());
			}

			return rr;
		}

		public static void GetRotationPoint(Rectangle r, int origin, out float x, out float y)
		{
			// origin - one of 9 locations - from 1 = top-left to 9 = bottom right
			// only 2 options thus far - 3 = bottom-right & 5 = center-center

			if (origin == 5)
			{
				GetRectCenter(r, out x, out y);
				return;
			}

			x = r.GetX();
			y= r.GetY();
		}


		/*// polygon help*/

		// using a factor of 10 makes a noticeable improvement 
		// in the accuracy of the returned points
		public static void getOffsetPath(float[] verts, double offset, 
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

		public static List<IntPoint> convertToIntPoints(float[] verts, float factor = 1)
		{
			List<IntPoint> pintlist = new List<IntPoint>();

			for (int i = 0; i < verts.Length; i+=2)
			{
				pintlist.Add(new IntPoint((double) verts[i]*factor, (double) verts[i + 1]*factor));
			}

			return pintlist;
		}

		public static float[] convertToFloatArray(List<IntPoint> pathOut, float factor = 1)
		{
			float[] verts = new float[pathOut.Count * 2];

			for (var i = 0; i < pathOut.Count; i++)
			{
				verts[i * 2] = pathOut[i].X / factor;
				verts[i * 2 + 1] = pathOut[i].Y / factor;
			}

			return verts;
		}

		public static void drawPath(float[] verts, PdfCanvas canvas)
		{
			canvas.MoveTo(verts[0], verts[1]);

			for (var i = 2; i < verts.Length; i+=2)
			{
				canvas.LineTo(verts[i], verts[i+1]);
			}

		}

		public static Rectangle getBoundingRect(Point[]  pts)
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

		public static Rectangle getBoundingRect(float[] verts)
		{
			int count = 0;
			if (verts.Length < 4) return null;

			double minX = verts[0];
			double minY = verts[1];
			double maxX = verts[0];
			double maxY = verts[1];

			for (var i = 2; i < verts.Length; i+=2)
			{
				count++;

				if (verts[i] < minX) minX = verts[i];
				else if (verts[i] > maxX) maxX = verts[i];

				if (verts[i+1] < minY) minY = verts[i+1];
				else if (verts[i+1] > maxY) maxY = verts[i+1];
			}

			return new Rectangle((float) minX, (float) minY, (float) (maxX - minX), (float) (maxY - minY));
		}

		public static Rectangle getBoundingRect( List<List<IntPoint>> pathout)
		{
			if (pathout.Count < 3 || pathout.Count > 1) return null;

			double minX = double.MaxValue;
			double minY = double.MaxValue;
			double maxX = double.MinValue;
			double maxY = double.MinValue;

			foreach (IntPoint pt in pathout[0])
			{
				if (pt.X < minX) minX = pt.X;
				else if (pt.X > maxX) maxX = pt.X;

				if (pt.Y < minY) minY = pt.Y;
				else if (pt.Y > maxY) maxY = pt.Y;
			}

			return new Rectangle((float) minX, (float) minY, (float) (maxX - minX), (float) (maxY - minY));
		}

		public static Rectangle getOffsetBoundingRect(float[] verts, float offset)
		{
			float[] pathout;

			getOffsetPath(verts, offset, EndType.CLOSED_POLYGON,out pathout, 10.0f);
			return getBoundingRect(pathout);
		}

		public static Rectangle convertArrayToRect(PdfArray array)
		{
			float[] pts = array.ToFloatArray();

			if (pts.Length != 4) return null;

			return new Rectangle(pts[0], pts[1], pts[2]-pts[0], pts[3]-pts[1]);
		}

		public static Point getRectCenter(Rectangle rect)
		{
			return new Point(
				rect.GetX() + rect.GetWidth() / 2,
				rect.GetY() + rect.GetHeight() / 2);
		}

		public static void setDashPattern(PdfCanvas canvas, PdfDictionary bs)
		{
			float[] f = bs.GetAsArray(PdfName.D).ToFloatArray();

			if (f.Length != 0)
			{
				canvas.SetLineDash(f, 0);
			}
			
		}


		/*// text help*/

		public static PdfFont GetPdfFont(string fontFamily)
		{
			PdfFont pf ;
			string fontPath;

			if (fontFamily.IsVoid() ||
				fontFamily.Equals("default"))
			{
				pf = DefaultFont;
			}
			else
			{
				fontPath = CsWindowHelpers.GetFontFilePath(fontFamily);
				pf = PdfFontFactory.CreateFont(fontPath);
			}

			return pf;
		}

		public static Style MakeTextStyle(TextSettings txs)
		{
			PdfFont pf = CreateSupport.GetPdfFont(txs.FontFamily);

			Style s = new Style();

			s.SetFont(pf);
			s.SetOpacity(txs.TextOpacity);
			s.SetFontColor(txs.TextColor);
			s.SetWidth(txs.TextWeight * 72);
			s.SetFontSize(txs.TextSize /2 );

			if (txs.FontStyle == 1 || txs.FontStyle == 3)
			{
				s.SetBold();
			}
			else if (txs.FontStyle == 2 || txs.FontStyle == 3)
			{
				s.SetItalic();
			}

			if (TextDecorations.HasLinethrough(txs.TextDecoration) )
			{
				s.SetLineThrough();
			}

			if (TextDecorations.HasUnderline(txs.TextDecoration))
			{
				s.SetUnderline();
			}

			return s;
		}

		/*// paragraph help */

		public static Paragraph MakeParagraph(Text[] text)
		{
			Paragraph pg = new Paragraph();
			pg.SetWidth(200f);

			foreach (Text t in text)
			{
				pg.Add(t);
			}

			Paragraph pg1 = new Paragraph();

			pg1.SetHorizontalAlignment(HorizontalAlignment.CENTER);
			pg1.SetVerticalAlignment(VerticalAlignment.MIDDLE);
			pg1.SetWidth(400f);
			pg1.SetHeight(400f);

			pg1.Add(pg);

			return pg1;
		}

		public static Paragraph MakeParagraph(string[] text, Style s)
		{
			Paragraph pg = new Paragraph();
			pg.SetWidth(200f);

			foreach (string t in text)
			{
				pg.Add(t);
			}

			pg.AddStyle(s);

			return pg;
		}


	}
}

 /*  voided
		public static PdfPolyGeomAnnotation MakePolygonAnnox(float[] verts)
		{
			Rectangle r = GetVertRect(verts);

			r = new Rectangle(r.GetX() - 50f, r.GetY() - 50f, r.GetWidth() + 100f, r.GetHeight() + 100f);

			PdfPolyGeomAnnotation poly = 
				PdfPolyGeomAnnotation.CreatePolygon(r, verts);

			return poly;
		}

		public static Rectangle GetVertRect(float[] vert)
		{
			if (vert.Length < 2) return null;

			float minX = 1000f * 72f;
			float minY = minX;
			float maxX = -1 * minX;
			float maxY = -1 * minY;

			for (var i = 0; i < vert.Length; i+=2)
			{
				if (vert[i] < minX)
				{
					minX = vert[i];
				}
				else
				if (vert[i] > maxX)
				{
					maxX = vert[i];
				}

				if (vert[i + 1] < minY)
				{
					minY = vert[i + 1];
				}
				else
				if (vert[i+1] > maxY)
				{
					maxY = vert[i+1];
				}
			}

			return new Rectangle(minX, minY, maxX - minX, maxY - minY);
		}


*/
