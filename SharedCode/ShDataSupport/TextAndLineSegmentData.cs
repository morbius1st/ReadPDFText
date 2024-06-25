// Solution:     ReadPDFText
// Project:       ReadPDFText
// File:             TextAndLineSegmentData.cs
// Created:      2024-02-09 (5:14 PM)

using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;


namespace SharedCode.ShDataSupport
{
/*
	public enum QUADRANT
	{
		Q1 = 0,
		Q2 = 1,
		Q3 = 2,
		Q4 = 3,
	}

	public struct Corner
	{
		public Point BaseCorner { get; set; }
		public Point OffsetPoint { get; set; }
		public Point OffsetCorner { get; set; }

		public float Angle { get; set; }

		public float OffsetDistance { get; set; }

		public Corner(Point baseCorner, float offsetDistance)
		{
			BaseCorner = baseCorner;
			OffsetDistance = offsetDistance;
			OffsetPoint = new Point();
			OffsetCorner = new Point();

			Angle = 0;
		}
	}
*/

	/// <summary>
	/// corners{} 0=BL / 1=BR / 2=TR / 3=TL (CCW)
	/// quadrant sheet orientation 0=BL / 1 = BR / 2 = TR / 3= TL (CCW)
	/// </summary>
	public class TextAndLineSegmentData
	{
		private string process;
		
		public string Process
		{
			get => process;
			set
			{
				process += " + " + value;
			}
		}

		public string Text { get; set; }

		// public LineSegment Top { get; set; }
		// public LineSegment Bott { get; set; }

		public Vector TSV { get; private set; }
		public Vector TEV { get; private set; }

		public Vector BSV { get; private set; }
		public Vector BEV { get; private set; }

		public float AngleRad { get; private set; }
		// public float BaseAngleRad { get; private set; }

		public float Width { get; private set; }
		public float Height { get; private set; }

		public float OAwidth { get; private set; }
		public float OAheight { get; private set; }

		public List<Point> CornerPoints { get; private set; }

		// public List<Point> CornerPointsAdjusted { get; private set; }
		//
		// public List<Corner> Corners {get; private set; }

		public float rise1 { get; private set; }
		public float rise2 { get; private set; }
		public float run1 { get; private set; }
		public float run2 { get; private set; }

		public double MinX { get; private set; }
		public double MinY { get; private set; }

		public bool IsRect { get; private set; }
		public bool IsValid { get; private set; }


		public int OnPageNumber { get; set; }
		public int ToPageNumber { get; set; }


		// public bool NormalOrientationIsVertical { get; private set; }

		// below for margin adjustments
		// public QUADRANT Quadrant { get; set; }

		// public float marginPageTbDir { get; set; }
		// public float marginPageLrDir { get; set; }


		public TextAndLineSegmentData( string text,
			LineSegment bott,
			LineSegment top,
			bool normalOrientationIsVertical,
			int onOnPage = -1)
		{
			init(text, bott, top, onOnPage);
		}


		public TextAndLineSegmentData( string text,
			LineSegment bott,
			LineSegment top,
			int onOnPage = -1)
		{
			init(text, bott, top, onOnPage);
		}

		private void init( string text,
			LineSegment bott,
			LineSegment top,
			int onOnPage)
		{
			process = "";

			Text = text;

			AngleRad = 0.0f;

			Width = 0f;
			Height = 0f;

			IsRect = true;

			OnPageNumber = onOnPage;
			ToPageNumber = -1;

			CornerPoints = new List<Point>();

			if (top == null || bott == null)
			{
				IsValid = false;
				TSV = null;
				TEV = null;
				BSV = null;
				BEV = null;
			}
			else
			{
				IsValid = true;

				TSV = top.GetStartPoint();
				TEV = top.GetEndPoint();

				BSV = bott.GetStartPoint();
				BEV = bott.GetEndPoint();

				calcCorners();
			}
		}

		// public double GetAngle()
		// {
		// 	return AngleRad * (180 / Math.PI);
		// }

		public void SetOverAll()
		{
			if (CornerPoints == null || CornerPoints.Count == 0)
			{
				OAheight = 0;
				OAwidth = 0;
		
				MinX = 0;
				MinY = 0;
				return;
			}
		
			OAheight = Math.Abs(rise1) + Math.Abs(rise2);
			OAwidth = Math.Abs(run1) + Math.Abs(run2);
		
			MinX = CornerPoints[0].x;
			MinY = CornerPoints[0].y;
		
			for (var i = 1; i < CornerPoints.Count; i++)
			{
				if (CornerPoints[i].x < MinX) MinX = CornerPoints[i].x;
				if (CornerPoints[i].y < MinY) MinY = CornerPoints[i].y;
			}
		}

		// the bounding box rectangle
		public Rectangle GetOArectangle()
		{
			SetOverAll();
		
			return new Rectangle((float) MinX, (float) MinY, OAwidth, OAheight);
		}

		// public PdfArray GetRectPolyPath2()
		// {
		// 	float[] vertices = new float[8];
		//
		// 	for (int i = 0; i < 4; i++)
		// 	{
		// 		vertices[i * 2] = (float) CornerPoints[i].x;
		// 		vertices[i * 2 + 1] = (float) CornerPoints[i].y;
		// 	}
		//
		// 	return new PdfArray(vertices);
		// }

		public PdfArray GetRectPolyPath()
		{
			// float[] vertices = new float[8];
			//
			// for (int i = 0; i < 4; i++)
			// {
			// 	vertices[i * 2] = (float) CornerPoints[i].x;
			// 	vertices[i * 2 + 1] = (float) CornerPoints[i].y;
			// }

			return new PdfArray(GetRectPath());
		}

		// public double[] GetRectPath2()
		// {
		// 	double[] vertices = new double[8];
		//
		// 	for (int i = 0; i < 4; i++)
		// 	{
		// 		vertices[i * 2] = CornerPoints[i].x;
		// 		vertices[i * 2 + 1] = CornerPoints[i].y;
		// 	}
		//
		// 	return vertices;
		// }

		public float[] GetRectPath()
		{
			float[] vertices = new float[8];

			for (int i = 0; i < 4; i++)
			{
				vertices[i * 2] =(float) CornerPoints[i].x;
				vertices[i * 2 + 1] = (float) CornerPoints[i].y;
			}

			return vertices;
		}

		public Rectangle GetRectangle()
		{
			float bLx  = (float) CornerPoints[0].x;
			float bLy  = (float) CornerPoints[0].y;
			float w    = Width;
			float h    = Height;

			return new Rectangle(bLx, bLy, w, h);
		}

		// get a simple rectangle - ignore any rotation
		public static Rectangle GetRectangle(LineSegment bott, LineSegment top)//  , MergePdfConfig cfg = null)
		{
			Vector bl = bott.GetEndPoint();
			Vector tr = top.GetStartPoint();

			float bLx = bl.Get(0); // - m1;
			float bLy = bl.Get(1); // - m2;
			float tRx = tr.Get(0); // + m1;
			float tRy = tr.Get(1); // + m2;


			Rectangle r = new Rectangle(bLx, bLy, tRx - bLx, tRy - bLy);
			// Rectangle r = new Rectangle(bl.Get(0), bl.Get(1), tr.Get(0) - bl.Get(0), tr.Get(1) - bl.Get(1));

			return r;
		}

		public static TextAndLineSegmentData Invalid()
		{
			return new TextAndLineSegmentData(null, null, null, false);
		}

		private void setCorners()
		{
			// BL = 0
			CornerPoints.Add( new Point(BSV.Get(0), BSV.Get(1)));
			// BR = 1
			CornerPoints.Add( new Point(BEV.Get(0), BEV.Get(1)));

			// TR = 2
			CornerPoints.Add( new Point(TEV.Get(0), TEV.Get(1)));
			// TL = 3
			CornerPoints.Add( new Point(TSV.Get(0), TSV.Get(1)));


			// if (NormalOrientationIsVertical)
			// {
			// }
			// else
			// {
			// 	// BL = 0
			// 	CornerPoints.Add( new Point(BEV.Get(0), BEV.Get(1)));
			// 	// BR = 1
			// 	CornerPoints.Add( new Point(TEV.Get(0), TEV.Get(1)));
			//
			// 	// TR = 2
			// 	CornerPoints.Add( new Point(BSV.Get(0), BSV.Get(1)));
			// 	// TL = 3
			// 	CornerPoints.Add( new Point(TSV.Get(0), TSV.Get(1)));
			// }
		}

		private void calcCorners()
		{
			setCorners();

			// calc angle
			// if no rise, flat line, no angle

			rise1 = (float) (CornerPoints[1].y - CornerPoints[0].y);
			run1 = (float) (CornerPoints[1].x - CornerPoints[0].x);

			rise2 = (float) (CornerPoints[3].y - CornerPoints[0].y);
			run2 = (float) (CornerPoints[3].x - CornerPoints[0].x);

			if (rise1 == 0)
			{
				configHorizElement();
				return;
			}

			if (run1 == 0)
			{
				configVertElement();
				return;
			}

			// assignQuadrant();

			IsRect = false;
			
			int sign = Math.Sign(rise1) & Math.Sign(run1);
			
			this.AngleRad = (float) Math.Atan(sign * (rise1 / run1));
			
			Height = (float) Math.Sqrt(rise2 * rise2 + run2 * run2);
			Width = (float) Math.Sqrt(rise1 * rise1 + run1 * run1);
		}

/*
		private void assignQuadrant()
		{
			if (run1 > 0)
			{
				if (rise1 > 0)
				{
					Quadrant = QUADRANT.Q1;
				}
				else
				{
					Quadrant = QUADRANT.Q4;
				}
			}
			else
			{
				if (rise1 > 0)
				{
					Quadrant = QUADRANT.Q2;
				}
				else
				{
					Quadrant = QUADRANT.Q3;
				}
			}
		}
*/

		private void configHorizElement()
		{
			Height = rise2;
			Width = run1;

			if (Width > 0)
			{
				AngleRad = 0;
				// Quadrant = QUADRANT.Q1;
				// marginPageLrDir = PdfText10.cfg.marginM1;
				// marginPageTbDir = PdfText10.cfg.marginM2;
			}
			else
			{
				AngleRad = Constants.PI180;
				// Quadrant = QUADRANT.Q3;
				// marginPageLrDir = -1 * PdfText10.cfg.marginM1;
				// marginPageTbDir = -1 * PdfText10.cfg.marginM2;
			}
			// Debug.WriteLine($"w x h| {Width,8:F2} x {Height,-8:F2} | angle| {(Constants.RadToDegrees(AngleRad)),-8:F2}");
		}

		private void configVertElement()
		{
			Height = rise1;
			Width = run2;

			this.AngleRad = Height > 0 ? Constants.PI90 : Constants.PI270;

			if (Height > 0)
			{
				AngleRad = Constants.PI90;
				// Quadrant = QUADRANT.Q2;
				// marginPageLrDir = -1 * PdfText10.cfg.marginM2;
				// marginPageTbDir = PdfText10.cfg.marginM1;
			}
			else
			{
				AngleRad = Constants.PI270;
				// Quadrant = QUADRANT.Q4;
				// marginPageLrDir = PdfText10.cfg.marginM2;
				// marginPageTbDir = -1 * PdfText10.cfg.marginM1;
			}

			// Debug.WriteLine($"w x h| {Width,8:F2} x {Height,-8:F2} | angle| {(Constants.RadToDegrees(AngleRad)),-8:F2}");
		}

		// private float RadToDeg(float rad)
		// {
		// 	return (rad / Constants.PI180) * 180;
		// }
	}
}

/* usage notes    
 * notes
 *
 *  [2] TL     [3] TR     [3] TR  [1] BR             [3] TR              
 *     +--------+             +---+			    	   +		     
 *     |        |             |   | 		    	  /  \	     
 *     +--------+             |   |			    	 /     + [1] BR	     
 *  [0] BL     [1] BR         +---+			    	/     /		     
 *                        [2] TL  [0] BL	[2] TL +     /		     
 *											    	 \  /			     
 *	                                                   + [0] BL		     
 *  Angle = 0             angle = 90	       angle = ? 								    				     
 *	H = [2] - [0]         H = [1] - [0]
 *	W = [1] - [0]         W = [2] - [0]
 *
 *
 *
 *            +.......... +
 *            .         /   \
 *            .       /       \
 *            .     /           +
 *            .   /           / .
 *            . /           /   .
 *            +           /     .
 *            .  \ H     / W    .
 *      rise2 . A  \   /        . Rise1
 *            +......+..........+
 *             run2      run2
 *
 *  A = angle = inttan(rise1 / run1)
 *  H = sqrt(sq(rise2)+sq(run2))
 *  W = sqrt(sq(rise1)+sq(run1))
 *
 *
 */

// failed / incomplete offset intersection calculations


// public int RectRegion { get; set; }
//
// public float A0 { get; private set; }
// public float A1 { get; private set; }
// public float A2 { get; private set; }
// public float A3 { get; private set; }
// public float A4 { get; private set; }
// public float A5 { get; private set; }
//
// // m1, m2, A2, & L1 from assembly settings (constants)
//
// private float h1 { get; set; }
// private float b1 { get; set; }
//
// private float h2 { get; set; }
// private float b2 { get; set; }
//
// private float[] v { get; set; }
// private int[] rz { get; set; } = new [] { 1, 1, -1, -1 };
// private int[] rx { get; set; } = new [] { 1, 1, -1, -1 };
//
// private int[][] vi { get; set; } = new [] { new []{1,0,3,2}, new []{0,1,2,3}, new []{1,0,3,2}, new []{0,1,2,3} };
//
// private int[][] si { get; set; }= new [] { new []{-1,-1,+1,+1}, new []{+1,-1,-1,+1},new []{+1,+1,-1,-1},new []{-1,+1,+1,-1}, };
//
// public float margAdjX_1_3 { get; set; }
// public float margAdjY_1_3 { get; set; }
//
// public float margAdjX_0_2 { get; set; }
// public float margAdjY_0_2 { get; set; }


/*
// get a simple rectangle - ignore any rotation
public static Rectangle GetRectangle(LineSegment bott, 
	LineSegment top, bool normalOrientationIsVertical)
{
	Vector bl;
	Vector br;
	Vector tl;
	Vector tr;

	if (normalOrientationIsVertical)
	{
		bl = bott.GetStartPoint();
		// br = bott.GetEndPoint();
		// tl = top.GetStartPoint();
		tr = top.GetEndPoint();
	}
	else
	{
		bl = bott.GetEndPoint();
		// br = top.GetEndPoint();
		// tl = bott.GetStartPoint();
		tr = top.GetStartPoint();
	}

	Rectangle r = new Rectangle(bl.Get(0), bl.Get(1), tr.Get(0) - bl.Get(0), tr.Get(1) - bl.Get(1));

	return r;

	// return new Rectangle(
	// 	bl.Get(0), bl.Get(1),
	// 	bott.GetLength(),
	// 	top.GetStartPoint().Get(1) - bl.Get(1)
	// 	);

}
*/



// // get a simple rectangle - ignore any rotation
// public static Rectangle GetRectangle(LineSegment bott, LineSegment top) //  , MergePdfConfig cfg = null)
// {
// 	Vector bl = bott.GetEndPoint();
// 	Vector tr = top.GetStartPoint();
//
// 	// float m1 = 0;
// 	// float m2 = 0;
// 	//
// 	// if (cfg != null)
// 	// {
// 	// 	m1 = cfg.marginM1;
// 	// 	m2 = cfg.marginM2;
// 	// }
//
// 	float bLx = bl.Get(0); // - m1;
// 	float bLy = bl.Get(1); // - m2;
// 	float tRx = tr.Get(0); // + m1;
// 	float tRy = tr.Get(1); // + m2;
//
//
// 	Rectangle r = new Rectangle(bLx, bLy, tRx - bLx, tRy - bLy);
// 	// Rectangle r = new Rectangle(bl.Get(0), bl.Get(1), tr.Get(0) - bl.Get(0), tr.Get(1) - bl.Get(1));
//
// 	return r;
// }

// public Rectangle GetRectangle(MergePdfConfig cfg = null)
// {
// 	float m1 = 0;
// 	float m2 = 0;
// 	float adj = 1;
//
// 	// if (cfg != null)
// 	// {
// 	// 	if (AngleRad == 0)
// 	// 	{
// 	// 		m1 = cfg.marginTB;
// 	// 		m2 = cfg.marginLR;
// 	// 	}
// 	// 	else if (AngleRad == Constants.PI90)
// 	// 	{
// 	// 		m1 = cfg.marginLR;
// 	// 		m2 = -1 * cfg.marginTB;
// 	// 	}
// 	// 	else if (AngleRad == Constants.PI180)
// 	// 	{
// 	// 		m1 = -1 * cfg.marginTB;
// 	// 		m2 = -1 * cfg.marginLR;
// 	// 	}
// 	// 	else
// 	// 	{
// 	// 		m1 = -1 * cfg.marginLR;
// 	// 		m2 = cfg.marginTB;
// 	// 	}
// 	// }
//
//
// 	// if (cfg != null)
// 	// {
// 	// 	Debug.WriteLine($"angle| {AngleRad}");
// 	//
// 	// 	m1 = adj*cfg.marginLR;
// 	// 	m2 = adj*cfg.marginTB;
// 	// }
//
//
// 	// float bLx  ; // = (float) Corners[0].x - m2;
// 	// float bLy  ; // = (float) Corners[0].y - m1;
// 	// float w    ; // = Width + m2 + m2;
// 	// float h    ; // = Height + m1 + m1;
// 	//
// 	// bLy = (float) CornerPoints[0].y - marginPageTbDir;
// 	// bLx = (float) CornerPoints[0].x - marginPageLrDir;
// 	// w = Width + marginPageLrDir + marginPageLrDir;
// 	// h = Height + marginPageTbDir + marginPageTbDir;
//
//
// 	float bLx  = (float) CornerPoints[0].x;
// 	float bLy  = (float) CornerPoints[0].y;
// 	float w    = Width;
// 	float h    = Height;
//
//
// 	return new Rectangle(bLx, bLy, w, h);
//
//
// 	// return new Rectangle((float) Corners[0].x, (float) Corners[0].y, Width, Height);
// }
//
//



//
// public static short[][,] dirAdjust = new []
// {
// 	new short[,]
// 	{
// 		{ -1, -1 },
// 		{ +1, +1 },
// 		{ +1, +1 },
// 		{ -1, -1 }
// 	},
// 	new short[,]
// 	{
// 		{ -1, +1 },
// 		{ +1, +1 },
// 		{ +1, -1 },
// 		{ -1, -1 }
// 	},
// 	new short[,]
// 	{
// 		{ +1, +1 },
// 		{ -1, +1 },
// 		{ -1, -1 },
// 		{ +1, -1 }
// 	},
// 	new short[,]
// 	{
// 		{ +1, -1 },
// 		{ -1, -1 },
// 		{ -1, +1 },
// 		{ +1, +1 }
// 	}
// };




		//
		// private void calcCorners()
		// {
		// 	setCorners();
		//
		// 	// calc angle
		// 	// if no rise, flat line, no angle
		//
		// 	rise1 = (float) (Corners[1].y - Corners[0].y);
		// 	run1 = (float) (Corners[1].x - Corners[0].x);
		//
		// 	rise2 = (float) (Corners[3].y - Corners[0].y);
		// 	run2 = (float) (Corners[3].x - Corners[0].x);
		//
		// 	// Debug.WriteLine($"\n{Text,-12} | Corners set| anchor| {Corners[0].x,8:F2} x {Corners[0].y,-8:F2} | [1] BR| {Corners[1].x,8:F2} x {Corners[1].y,-8:F2} | [3] TL| {Corners[3].x,8:F2} x {Corners[3].y,-8:F2}");
		// 	// Debug.Write($"\trun1 x rise 1| {run1,8:F2} x {rise1,-8:F2} || run2 x rise 2| {run2,8:F2} x {rise2,-8:F2} | ");
		//
		// 	// if rise is zero, horizontal line
		// 	if (rise1 == 0)
		// 	{
		// 		configHorizElement();
		// 		return;
		// 	}
		// 	// {
		// 	// 	Height = rise2;
		// 	// 	Width = run1;
		// 	//
		// 	// 	if (Width > 0)
		// 	// 	{
		// 	// 		AngleRad = 0;
		// 	// 		Quadrant = 0;
		// 	// 		marginPageLrDir = PdfText10.cfg.marginLR;
		// 	// 		marginPageTbDir = PdfText10.cfg.marginTB;
		// 	// 	}
		// 	// 	else
		// 	// 	{
		// 	// 		AngleRad = Constants.PI180;
		// 	// 		Quadrant = 2;
		// 	// 		marginPageLrDir = -1 * PdfText10.cfg.marginLR;
		// 	// 		marginPageTbDir = -1 * PdfText10.cfg.marginTB;
		// 	// 	}
		// 	//
		// 	//
		// 	// 	// Debug.WriteLine($"w x h| {Width,8:F2} x {Height,-8:F2} | angle| {(Constants.RadToDegrees(AngleRad)),-8:F2}");
		// 	//
		// 	// 	return;
		// 	// }
		//
		// 	// got height but if no width, vertical line, no angle
		// 	if (run1 == 0)
		// 	{
		// 		configVertElement();
		// 		return;
		// 	}
		// 	// {
		// 	// 	Height = rise1;
		// 	// 	Width = run2;
		// 	//
		// 	// 	this.AngleRad = Height > 0 ? Constants.PI90 : Constants.PI270;
		// 	//
		// 	// 	if (Height > 0)
		// 	// 	{
		// 	// 		AngleRad = Constants.PI90;
		// 	// 		Quadrant = 1;
		// 	// 		marginPageLrDir = -1 * PdfText10.cfg.marginTB;
		// 	// 		marginPageTbDir = PdfText10.cfg.marginLR;
		// 	// 	}
		// 	// 	else
		// 	// 	{
		// 	// 		AngleRad = Constants.PI270;
		// 	// 		Quadrant = 3;
		// 	// 		marginPageLrDir = PdfText10.cfg.marginTB;
		// 	// 		marginPageTbDir = -1 * PdfText10.cfg.marginLR;
		// 	// 	}
		// 	//
		// 	// 	// Debug.WriteLine($"w x h| {Width,8:F2} x {Height,-8:F2} | angle| {(Constants.RadToDegrees(AngleRad)),-8:F2}");
		// 	// 	return;
		// 	// }
		// 	// configRotatedElement();
		//
		// 	assignQuadrant();
		//
		//
		// 	// IsRect = false;
		// 	//
		// 	// int sign = Math.Sign(rise1) & Math.Sign(run1);
		// 	//
		// 	// this.AngleRad = (float) Math.Atan(sign * (rise1 / run1));
		// 	//
		// 	// Height = (float) Math.Sqrt(rise2 * rise2 + run2 * run2);
		// 	// Width = (float) Math.Sqrt(rise1 * rise1 + run1 * run1);
		//
		// 	// Debug.WriteLine($"origin BL [0]| {Corners[0].x:F2}, {Corners[0].y:F2}");
		// 	// Debug.WriteLine($"   run BR [1]| {Corners[1].x:F2}, {Corners[1].y:F2}");
		// 	// Debug.WriteLine($"  rise TL [3]| {Corners[3].x:F2}, {Corners[3].y:F2}");
		// 	// Debug.WriteLine($"       TR [2]| {Corners[2].x:F2}, {Corners[2].y:F2}");
		// 	//
		// 	// int a = 1;
		// 	// Debug.WriteLine($"w x h| {Width,8:F2} x {Height,-8:F2} | angle| {(Constants.RadToDegrees(AngleRad)),-8:F2} ({AngleRad})");
		// }

		// private void configRotatedElement()
		// {
		// 	IsRect = false;
		//
		// 	Height = (float) Math.Sqrt(rise2 * rise2 + run2 * run2);
		// 	Width = (float) Math.Sqrt(rise1 * rise1 + run1 * run1);
		//
		// 	int sign = Math.Sign(rise1) & Math.Sign(run1);
		//
		// 	// this.AngleRad = (float) Math.Atan(sign * (Math.Abs(rise1) / Math.Abs(run1)));
		// 	// BaseAngleRad = (float) Math.Atan((Math.Abs(rise1) / Math.Abs(run1)));
		//
		// 	// float deg1 = (AngleRad / Constants.PI180) * 180;
		//
		// 	assignQuadrant();
		// 	// calcRotationalAdjustments();
		// 	//
		// 	// showmarginValues();
		// 	//
		// 	// calcMargAdjustments();
		// }
		//
		// private void showmarginValues()
		// {
		// 	float a0 = RadToDeg(A0);
		// 	float a1 = RadToDeg(A1);
		// 	float a2 = RadToDeg(A2);
		// 	float a3 = RadToDeg(A3);
		// 	float a4 = RadToDeg(A4);
		// 	float a5 = RadToDeg(A5);
		//
		// 	int r1 = RectRegion;
		//
		// 	float hh1 = h1;
		// 	float bb1 = b1;
		// 	float hh2 = h2;
		// 	float bb2 = b2;
		//
		// 	Debug.Write($"\n");
		// 	Debug.WriteLine($"  A0 {a0:F2}| A1 {a1:F2}| A2 {a2:F2}| A3 {a3:F2}| A4 {a4:F2}| A5 {a5:F2}");
		// 	Debug.WriteLine($"  h1 {h1:F2}| b1 {b1:F2}| h2 {h2:F2}| b2 {b2:F2}");
		// 	Debug.WriteLine($"  R1 {r1+1}");
		// 	Debug.WriteLine($"(0)| h1 {dirAdjust[r1][0,0],2:+#;-#;0}| b1 {dirAdjust[r1][0,1],2:+#;-#;0}");
		// 	Debug.WriteLine($"(1)| h2 {dirAdjust[r1][1,0],2:+#;-#;0}| b2 {dirAdjust[r1][1,1],2:+#;-#;0}");
		// 	Debug.WriteLine($"(2)| h1 {dirAdjust[r1][2,0],2:+#;-#;0}| b1 {dirAdjust[r1][2,1],2:+#;-#;0}");
		// 	Debug.WriteLine($"(3)| h2 {dirAdjust[r1][3,0],2:+#;-#;0}| b2 {dirAdjust[r1][3,1],2:+#;-#;0}");
		// }
		//


//
// 		private void calcRotationalAdjustments()
// 		{
// 			if (!PdfText10.cfg.IncludeMargins) return;
//
//
// 			// A1 = AngleRad;
// 			// A2 = PdfText10.cfg.marginA2;
// 			// A3 ignore
// 			A4 = ((A0 + Constants.PI90) - A1) - A2;
// 			A5 = A3 - A2;
//
// 			h1 = (float) (PdfText10.cfg.marginL1 * Math.Cos(A4));
// 			b1 = (float) (PdfText10.cfg.marginL1 * Math.Sin(A4));
// 			;
//
// 			h2 = (float) (PdfText10.cfg.marginL1 * Math.Sin(A5));
// 			b2 = (float) (PdfText10.cfg.marginL1 * Math.Cos(A5));
// 		}
//
//
// 		private void calcMargAdjustments()
// 		{
// 			CornersAdjusted = new List<Point>();
//
// 			for (int i = 0; i < 4; i++)
// 			{
// 				CornersAdjusted.Add(Corners[i]);
// 			}
//
// 			if (!PdfText10.cfg.IncludeMargins) return;
//
// 			int r1 = RectRegion - 1;
//
//
//
// 			// float L1 = (float) Math.Sqrt(PdfText10.cfg.marginM1 * PdfText10.cfg.marginM1 + PdfText10.cfg.marginM2 * PdfText10.cfg.marginM2);
// 			// float A3 = (float) Math.Atan(PdfText10.cfg.marginM1 / PdfText10.cfg.marginM2);
// 			// float A4 = A3 - BaseAngleRad;
// 			// float A5 = A3 - (Constants.PI90 - BaseAngleRad);
// 			//
// 			// // for points 
// 			// margAdjX_0_2 = (float) (L1 * Math.Sin(A4)); // x direction
// 			// margAdjY_0_2 = (float) (L1 * Math.Cos(A4)); // y direction
//
// 			/*
// 			CornersAdjusted[0].x = Corners[0].x - margAdjX_0_2;
// 			CornersAdjusted[0].y = Corners[0].y - margAdjY_0_2;
//
// 			CornersAdjusted[1].x = Corners[1].x + margAdjX_1_3;
// 			CornersAdjusted[1].y = Corners[1].y + margAdjY_1_3;
//
// 			CornersAdjusted[2].x = Corners[2].x + margAdjX_0_2;
// 			CornersAdjusted[2].y = Corners[2].y + margAdjY_0_2;
//
// 			// margAdjX_1_3 = (float) (L1 * Math.Cos(A5)); // x direction
// 			// margAdjY_1_3 = (float) (L1 * Math.Sin(A5)); // y direction
//
// 			CornersAdjusted[1].x = Corners[1].x + margAdjX_1_3;
// 			CornersAdjusted[1].y = Corners[1].y + margAdjY_1_3;
//
// 			CornersAdjusted[3].x = Corners[3].x - margAdjX_1_3;
// 			CornersAdjusted[3].y = Corners[3].y - margAdjY_1_3;
//
// 			float A51d = Constants.PI90;
// 			float A3d = (A3 / Constants.PI180) * 180;
// 			float A4d = (A4 / Constants.PI180) * 180;
// 			float A5d = (A5 / Constants.PI180) * 180;
//
//
// 			int a = 0;
// 			*/
// 		}


		// private void assignQuadrant()
		// {
		// 	// A2 = PdfText10.cfg.marginA2;
		// 	// float Ax;
		//
		// 	if (run1 > 0)
		// 	{
		// 		if (rise1 > 0)
		// 		{
		// 			Quadrant = QUADRANT.Q1;
		// 			// AngleRad = BaseAngleRad;
		// 			// // angle does not change
		// 			// A0 = 0;
		// 			// A1 = AngleRad;
		// 			// A3 = BaseAngleRad;
		// 			// Ax = A1 + A2;
		// 			// RectRegion = (A1 - A0) <= Ax ? 0 : 1;
		// 		}
		// 		else
		// 		{
		// 			Quadrant = QUADRANT.Q4;
		// 			// AngleRad = Constants.PI360 - BaseAngleRad;
		// 			// A0 = Constants.PI270;
		// 			// A1 = AngleRad;
		// 			// A3 = Constants.PI90 - BaseAngleRad;
		// 			// Ax = A1 + A2;
		// 			// RectRegion = (A1 - A0) <= Ax ? 3 : 0;
		// 		}
		// 	}
		// 	else
		// 	{
		// 		if (rise1 > 0)
		// 		{
		// 			Quadrant = QUADRANT.Q2;
		// 			// AngleRad = Constants.PI180 - BaseAngleRad;
		// 			// A0 = Constants.PI90;
		// 			// A1 = AngleRad;
		// 			// A3 = Constants.PI90 - BaseAngleRad;
		// 			// Ax = A1 + A2;
		// 			// RectRegion = (A1 - A0) <= Ax ? 1 : 2;
		// 		}
		// 		else
		// 		{
		// 			Quadrant = QUADRANT.Q3;
		// 			// AngleRad = Constants.PI180 + BaseAngleRad;
		// 			// A0 = Constants.PI180;
		// 			// A1 = AngleRad;
		// 			// A3 = BaseAngleRad;
		// 			// Ax = A1 + A2;
		// 			// RectRegion = (A1 - A0) <= Ax ? 2 : 3;
		// 		}
		// 	}
		// }
