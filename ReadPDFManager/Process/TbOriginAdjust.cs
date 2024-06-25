#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using iText.Kernel.Geom;
using ShCode;
using ShSheetData.SheetData;
using static ShSheetData.SheetData.SheetRectId;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   6/13/2024 5:50:39 PM

namespace ReadPDFManager.Process
{
	// revised - use this:
	// double checked

	// ha
	// left   = 0 => 0
	// center = 1 => 0.5
	// right  = 2  => 1

	// va
	// top    = 0 => 0.0
	// middle = 1 => 0.5
	// bottom = 2 => 1.0


	public struct SampleBox
	{
		public string Name { get; set; }
		public string NameEx { get; set; }
		public SheetRectId Id { get; set; }
		public float ShtRotation { get; set; }
		public float TbRotation { get; set; }
		public Rectangle TbRect { get; set; }
		public int HorizAlign { get; set; }
		public int VertAlign { get; set; }

		public float CorrectX { get; set; }
		public float CorrectY { get; set; }

		public SampleBox(string name, SheetRectId id,
			float shtRotation, 
			float tbRotation, Rectangle tbRect, 
			int horizAlign, int vertAlign, float correctX, float correctY)
		{
			Name = name;
			NameEx = $"{name}_{shtRotation:F0}_{tbRotation:F0}";
			Id = id ;
			ShtRotation = shtRotation;
			TbRotation = tbRotation;
			TbRect = tbRect;
			HorizAlign = horizAlign;
			VertAlign = vertAlign;
			CorrectX = correctX;
			CorrectY = correctY;
		}
	}

	public class TbOriginAdjust
	{
		// 1 = new settings - LB
		// 2 = orig settings = LT
		public static int which = 1;

		public List<SampleBox> SampleBoxes { get; set; }

		public TbOriginAdjust()
		{
			init();
		}

		private void init()
		{
			// int L = (int) HorizontalAlignment.LEFT;
			// int C = (int) HorizontalAlignment.CENTER;
			// int R = (int) HorizontalAlignment.RIGHT;
			//
			// int T = (int) VerticalAlignment.TOP;
			// int M = (int) VerticalAlignment.MIDDLE;
			// int B = (int) VerticalAlignment.BOTTOM;


			if (which==1) makeSampleBoxes1();
			if (which==2) makeSampleBoxes2();
		}

		public void Process()
		{
			ShowInfo.StartMsg($"origin adjustments - {(which==1?"new":"orig")}", DateTime.Now.ToString());

			foreach (SampleBox sb in SampleBoxes)
			{
				calcValues(sb);
			}
		}
		//                              0    1    2
		private string[] ha = new [] { "L", "C", "R" };
		private string[] va = new [] { "T", "M", "B" };

		private float priorRotation = -1;
		private string rectname;


		private void calcValues(SampleBox sb)
		{
			if (sb.ShtRotation != priorRotation)
			{
				priorRotation = sb.ShtRotation;
				Debug.WriteLine($"\n\nsheet rotation == {sb.ShtRotation:F2}\n");
			}

			rectname = sb.Name;

			Debug.Write($"{sb.Name,-14}");
			Debug.Write($" | sht rotation {sb.ShtRotation} | tb rotation {sb.TbRotation}");
			Debug.WriteLine($" | w {sb.TbRect.GetWidth(),8:F2} h {sb.TbRect.GetHeight(),8:F2}");
			Debug.Write($"\talign| {ha[(int) sb.HorizAlign]}-{va[(int) sb.VertAlign]} ({sb.HorizAlign})-({sb.VertAlign})");

			Tuple<int, int, int, int> factors = getFactors(sb);

			Debug.WriteLine($" | factors| {factors.Item1} {factors.Item2} {factors.Item3} {factors.Item4}");

			sinTb = (float) Math.Abs(Math.Sin(FloatOps.ToRad(sb.TbRotation)));
			cosTb = (float) Math.Abs(Math.Cos(FloatOps.ToRad(sb.TbRotation)));

			adjWandH(sb, out len1, out len2, out len3, out len4);

			start = sb.TbRect.GetX();
			adj1B = factors.Item1;
			adj2B = factors.Item2;

			float finalX = calcValue();

			start = sb.TbRect.GetY();
			len1 = len3;
			len2 = len4;
			adj1B = factors.Item3;
			adj2B = factors.Item4;

			float finalY = calcValue();

			bool resultx = sb.CorrectX.Equals(finalX);
			bool resulty = sb.CorrectY.Equals(finalY);

			string xmatch = resultx ? ":)" : ":(";
			string ymatch = resulty ? ":)" : ":(";
			string results =  resultx && resulty ? "✓" : "✗";

			Debug.WriteLine($"{results}\tcorrect {sb.CorrectX,8:f2} {sb.CorrectY,8:F2} | calc'd {finalX,8:F2} {xmatch} {finalY,8:F2} {ymatch}\n");
		}


		/* removed - simplified
		private void adjWandH2(SampleBox sb, out float wx, out float hx, out float wy, out float hy)
		{

			wx = sb.TbRect.GetWidth();
			hx = sb.TbRect.GetHeight();

			wy = wx;
			hy = hx;

			if (sb.ShtRotation == 0)
			{
				(wy, hy) = (hy, wy);

				if (sb.TbRotation < 90)
				{
					(wx, hx) = (hx, wx);
				}
			}
			else if (sb.ShtRotation == 90)
			{
				if (sb.TbRotation < 90)
				{
					(wy, hy) = (hy, wy);
				}
				else
				{
					(wx, hx) = (hx, wx);
				}
			} 
			else if (sb.ShtRotation == 270)
			{
				if (sb.TbRotation < 90)
				{
					(wy, hy) = (hy, wy);
				}
				else
				{
					(wx, hx) = (hx, wx);
				}
			}
		}
		*/


		private void adjWandH(SampleBox sb, out float wx, out float hx, out float wy, out float hy)
		{
			wx = sb.TbRect.GetWidth();
			hx = sb.TbRect.GetHeight();

			wy = wx;
			hy = hx;

			if (sb.ShtRotation == 0)
			{
				(wy, hy) = (hy, wy);

				if (sb.TbRotation < 90)
				{
					(wx, hx) = (hx, wx);
				}
			}
			else
			{
				if (sb.TbRotation < 90)
				{
					(wy, hy) = (hy, wy);
				}
				else
				{
					(wx, hx) = (hx, wx);
				}
			}
		}

		private float start;
		private float len1;
		private float cosTb;
		private float adj1B;
		private float len2;
		private float sinTb;
		private float adj2B;

		private float len3;
		private float len4;

		// private int trig2A;
		// private int trig1A;

		private float calcValue()
		{
			float adj1 = len1 * cosTb * adj1B;
			float adj2 = len2 * sinTb * adj2B;

			float answer = start + adj1 + adj2;

			showCalc(adj1, adj2, answer);

			return (float) Math.Round(answer, 2);
		}

		private Tuple<int, int, int, int> getFactors(SampleBox sb)
		{
			int shtIdx = sheetRotationIdx(sb.ShtRotation);
			int tbIdx = tbRotationIdx(sb.TbRotation);

			if (which==1) return factors1[shtIdx, tbIdx];
			if (which==2) return factors2[shtIdx, tbIdx];

			return factors2[shtIdx, tbIdx];
		}

		private int sheetRotationIdx(float rotation)
		{
			if (rotation == 0) return 0;
			if (rotation == 90) return 1;

			return 2;
		}

		private int tbRotationIdx(float rotation)
		{
			return (int) ((rotation % 360) / 90);
		}

		private void showCalc(float adj1, float adj2, float answer)
		{
			Debug.Write($"\t{start,8:F2} (1) {len1,8:F2} {cosTb,8:F2} {adj1B,8:F2} = {adj1,8:F2}");
			Debug.Write($"  (2) {len2,8:F2} {sinTb,8:F2} {adj2B,8:F2} = {adj2,8:F2}");
			Debug.WriteLine($" | = answer {answer,8:F2}");
		}

		
		// basic values - need to be adjusted by horiz alignment & vertical alignment
		// new values
		// do not use
		private Tuple<int, int, int, int>[,] factors1 = new[,]
		{
			{
				// sheet rotation == 0
				new Tuple<int, int, int, int>(0,1,0,0), // 0 to include 90
				new Tuple<int, int, int, int>(1,0,1,0), // 90+ to 180
				new Tuple<int, int, int, int>(1,0,1,1), // 180 to 270
				new Tuple<int, int, int, int>(1,0,0,0), // 270 to 360
			},

			{
				// sheet rotation == 90
				new Tuple<int, int, int, int>(0,1,1,1), // 0 to 90
				new Tuple<int, int, int, int>(1,0,1,1), // 90 to 180
				new Tuple<int, int, int, int>(0,0,1,0), // 180 to 270
				new Tuple<int, int, int, int>(0,1,0,0), // 270 to 360
			},

			{
				// sheet rotation == 270
				new Tuple<int, int, int, int>(0,1,1,1), // 0 to 90
				new Tuple<int, int, int, int>(0,1,0,0), // 90 to 180
				new Tuple<int, int, int, int>(1,1,0,1), // 180 to 270
				new Tuple<int, int, int, int>(1,0,1,1), // 270 to 360
			}
		};

		// basic values - need to be adjusted by horiz alignment & vertical alignment
		// original values
		// do not use
		private Tuple<int, int, int, int>[,] factors2 = new[,]
		{
			{
				// sheet rotation == 0
				new Tuple<int, int, int, int>(0, 0, 1, 0), // 0 to include 90
				new Tuple<int, int, int, int>(1, 0, 0, 0), // 90+ to 180
				new Tuple<int, int, int, int>(1, 1, 0, 1), // 180 to 270
				new Tuple<int, int, int, int>(0, 1, 1, 1), // 270 to 360
			},

			{
				// sheet rotation == 90
				new Tuple<int, int, int, int>(0, 1, 0, 0), // 0 to 90
				new Tuple<int, int, int, int>(1, 1, 1, 0), // 90 to 180
				new Tuple<int, int, int, int>(1, 0, 1, 1), // 180 to 270
				new Tuple<int, int, int, int>(0, 0, 0, 1), // 270 to 360
			},

			{
				// sheet rotation == 270
				new Tuple<int, int, int, int>(1, 0, 1, 1), // 0 to 90
				new Tuple<int, int, int, int>(0, 0, 0, 1), // 90 to 180
				new Tuple<int, int, int, int>(0, 1, 0, 0), // 180 to 270
				new Tuple<int, int, int, int>(1, 1, 1, 0), // 270 to 360
			}
		};

		private void makeSampleBoxes1()
		{
			SampleBoxes = new List<SampleBox>();

			SampleBox sb;

			// all are L & B
			SampleBoxes.Add(new SampleBox("optional 7", SM_OPT7      , 0f  , 0f  , new Rectangle(500f    , 300f    , 600f    , 200f)     , 0, 2,  500.0f, 300.0f));
			SampleBoxes.Add(new SampleBox("optional10", SM_OPT10     ,  0f ,  30f, new Rectangle(1800.00f, 2058.79f, 600.00f , 200.00f)  , 0, 2, 1900.00f, 2058.79f));

			SampleBoxes.Add(new SampleBox("optional 8", SM_OPT8      , 0f  , 90f , new Rectangle(1300f   , 300f    , 200f    , 600f)     , 0, 2, 1500.0f, 300.0f));
			SampleBoxes.Add(new SampleBox("optional 4", SM_OPT4      ,   0f, 120f, new Rectangle(564.18f , 1656.68f, 600.00f , 200.00f)  , 0, 2, 1037.39f, 1756.68f));

			SampleBoxes.Add(new SampleBox("optional 9", SM_OPT9      , 0f  , 180f, new Rectangle(1872.00f, 288.00f , 576.00f , 144.00f)  , 0, 2, 2448.0f, 432.0f));
			SampleBoxes.Add(new SampleBox("optional 3", SM_OPT3      ,   0f, 210f, new Rectangle(533.07f , 1139.82f, 600.00f , 200.00f)  , 0, 2, 1052.69f, 1613.03f));

			SampleBoxes.Add(new SampleBox("optional 0", SM_OPT0      , 0f  , 270f, new Rectangle(2736.00f, 288.00f , 144.00f , 576.00f)  , 0, 2, 2736.0f, 864.0f));
			SampleBoxes.Add(new SampleBox("optional 6", SM_OPT6      ,   0f, 300f, new Rectangle(1122.98f, 1829.07f, 600.00f , 200.00f)  , 0, 2, 1122.98f, 2348.69f));


			SampleBoxes.Add(new SampleBox("optional 7", SM_OPT7      , 90f , 0f  , new Rectangle(2092.00f, 500.00f , 200.00f , 600.00f)  , 0, 2, 2292.00f,  500.0f));
			SampleBoxes.Add(new SampleBox("optional10", SM_OPT10     , 90f ,  30f, new Rectangle( 60.00f , 1800.00f, 200.00f , 600.00f)  , 0, 2,  533.21f, 1900.00f));

			SampleBoxes.Add(new SampleBox("optional 8", SM_OPT8      , 90f , 90f , new Rectangle(1692.00f, 1300.00f, 600.00f , 200.00f)  , 0, 2, 2292.00f, 1300.0f ));
			SampleBoxes.Add(new SampleBox("optional 4", SM_OPT4      ,  90f, 120f, new Rectangle(316.39f , 564.00f , 200.00f , 600.00f)  , 0, 2,  836.01f, 1037.21f));

			SampleBoxes.Add(new SampleBox("optional 9", SM_OPT9      , 90f , 180f, new Rectangle(2160.00f, 1872.00f, 144.00f , 576.00f)  , 0, 2, 2160.00f, 2448.0f));
			SampleBoxes.Add(new SampleBox("optional 3", SM_OPT3      ,  90f, 210f, new Rectangle(978.79f , 532.39f , 200.00f , 600.00f)  , 0, 2,  978.79f, 1052.01f));

			SampleBoxes.Add(new SampleBox("optional 0", SM_OPT0      , 90f , 270f, new Rectangle(1728.00f, 2736.00f, 576.00f , 144.00f)  , 0, 2, 1728.00f, 2736.0f));
			SampleBoxes.Add(new SampleBox("optional 6", SM_OPT6      ,  90f, 300f, new Rectangle(144.00f , 1122.80f, 200.00f , 600.00f)  , 0, 2,  244.00f, 1122.80f));


			SampleBoxes.Add(new SampleBox("optional 7", SM_OPT7      , 270f, 0f  , new Rectangle(300.00f , 2356.00f, 200.00f , 600.00f)  , 0, 2,  300.00f, 2956.0f));
			SampleBoxes.Add(new SampleBox("optional10", SM_OPT10     ,270f ,  30f, new Rectangle(2058.79f, 1036.39f, 200.00f , 600.00f)  , 0, 2, 2058.79f, 1556.01f));

			SampleBoxes.Add(new SampleBox("optional 8", SM_OPT8      , 270f, 90f , new Rectangle(300.00f , 1956.00f, 600.00f , 200.00f)  , 0, 2,  300.00f, 1956.0f));
			SampleBoxes.Add(new SampleBox("optional 4", SM_OPT4      , 270f, 120f, new Rectangle(1656.00f, 2418.79f, 200.00f , 600.00f)  , 0, 2, 1756.00f, 2418.79f));

			SampleBoxes.Add(new SampleBox("optional 9", SM_OPT9      , 270f, 180f, new Rectangle(288.00f , 1008.00f, 144.00f , 576.00f)  , 0, 2,  288.00f, 1584.0f));
			SampleBoxes.Add(new SampleBox("optional 3", SM_OPT3      , 270f, 210f, new Rectangle(1140.00f, 2304.00f, 200.00f , 600.00f)  , 0, 2, 1613.21f, 2404.00f));

			SampleBoxes.Add(new SampleBox("optional 0", SM_OPT0      , 270f, 270f, new Rectangle(288.00f ,  576.00f, 576.00f , 144.00f)  , 0, 2,  288.00f,  576.0f));
			SampleBoxes.Add(new SampleBox("optional 6", SM_OPT6      , 270f, 300f, new Rectangle(1828.39f, 1860.00f, 200.00f , 600.00f)  , 0, 2, 2348.01f, 2333.21f));

			// SampleBoxes.Add(new SampleBox());
		}

		private void makeSampleBoxes2()
		{
			SampleBoxes = new List<SampleBox>();

			SampleBox sb;

			// all are L & T
			SampleBoxes.Add(new SampleBox("optional 7", SM_OPT7      , 0f  , 0f  , new Rectangle(500f    , 300f    , 600f    , 200f)     , 0, 0, 500.0f  , 500.0f));
			SampleBoxes.Add(new SampleBox("optional 8", SM_OPT8      , 0f  , 90f , new Rectangle(1300f   , 300f    , 600f    , 200f)     , 0, 0, 1300.0f , 300.0f));

			SampleBoxes.Add(new SampleBox("optional10", SM_OPT10     , 0f  ,  30f, new Rectangle(1800.00f, 2058.79f, 600.00f , 200.00f)  , 0, 0, 1900.00f, 2058.79f));
			SampleBoxes.Add(new SampleBox("optional 4", SM_OPT4      , 0f  , 120f, new Rectangle(564f    , 1656f   , 600f    , 200f)     , 0, 0, 864.0f  , 1656.0f));
			SampleBoxes.Add(new SampleBox("optional 3", SM_OPT3      , 0f  , 210f, new Rectangle(532.39f , 1140.0f , 600f    , 200f)     , 0, 0, 1152.01f, 1440.0f));
			SampleBoxes.Add(new SampleBox("optional 6", SM_OPT6      , 0f  , 300f, new Rectangle(1122.80f, 1828.39f, 600.00f , 200.00f)  , 0, 0, 1296.01f, 2448.01f));

			SampleBoxes.Add(new SampleBox("optional 7", SM_OPT7      , 90f , 0f  , new Rectangle(2092.00f, 500.00f , 200.00f , 600.00f)  , 0, 0, 2092.00f, 500.00f));
			SampleBoxes.Add(new SampleBox("optional 8", SM_OPT8      , 90f , 90f , new Rectangle(1692.00f, 1300.00f, 600.00f , 200.00f)  , 0, 0, 2292.00f, 1300.00f ));

			SampleBoxes.Add(new SampleBox("optional10", SM_OPT10     , 90f ,  30f, new Rectangle(60.00f  , 1800.00f, 200.00f , 600.00f)  , 0, 0, 533.21f , 1900.00f));
			SampleBoxes.Add(new SampleBox("optional 4", SM_OPT4      , 90f , 120f, new Rectangle(316.39f , 564.00f , 200.00f , 600.00f)  , 0, 0, 936.01f , 864.00f));
			SampleBoxes.Add(new SampleBox("optional 3", SM_OPT3      , 90f , 210f, new Rectangle(978.79f , 532.39f , 200.00f , 600.00f)  , 0, 0, 1152.00f, 1152.01f));
			SampleBoxes.Add(new SampleBox("optional 6", SM_OPT6      , 90f , 300f, new Rectangle(144.00f , 1122.80f, 200.00f , 600.00f)  , 0, 0, 144.00f , 1296.01f));


			SampleBoxes.Add(new SampleBox("optional 7", SM_OPT7      , 270f, 0f  , new Rectangle(300.00f , 2356.00f, 200.00f , 600.00f)  , 0, 0, 500.00f , 2956.00f));
			SampleBoxes.Add(new SampleBox("optional 8", SM_OPT8      , 270f, 90f , new Rectangle(300.00f , 1956.00f, 600.00f , 200.00f)  , 0, 0, 300.00f , 2156.00f));

			SampleBoxes.Add(new SampleBox("optional10", SM_OPT10     , 270f,  30f, new Rectangle(2058.79f, 1036.39f, 200.00f , 600.00f)  , 0, 0, 2058.58f, 1556.01f));
			SampleBoxes.Add(new SampleBox("optional 4", SM_OPT4      , 270f, 120f, new Rectangle(1656.00f, 2418.79f, 200.00f , 600.00f)  , 0, 0, 1656.00f, 2592.00f));
			SampleBoxes.Add(new SampleBox("optional 3", SM_OPT3      , 270f, 210f, new Rectangle(1140.00f, 2304.00f, 200.00f , 600.00f)  , 0, 0, 1440.00f, 2304.00f));
			SampleBoxes.Add(new SampleBox("optional 6", SM_OPT6      , 270f, 300f, new Rectangle(1828.39f, 1860.00f, 200.00f , 600.00f)  , 0, 0, 2448.01f, 2160.00f));



			// SampleBoxes.Add(new SampleBox("banner 2"  , SM_BANNER_2ND, 0f  , 90f , new Rectangle(3422.41f, 48.68f  , 28.00f  , 615.83f) , 0, 0, 3422.41f, 48.68f));
			// SampleBoxes.Add(new SampleBox("watermark1", SM_WATERMARK1, 0f  , 38f , new Rectangle(195.76f , 169.85f , 3367.35f, 508.50f) , 0, 0, 195.76f, 570.55f));
			// SampleBoxes.Add(new SampleBox("optional 2", SM_OPT2      , 0f  , 30f , new Rectangle(500f    , 300f    , 600f    , 200f)    , 0, 0, 500.0f, 473.21f));

			// SampleBoxes.Add(new SampleBox("optional 2", SM_OPT2      , 90f , 30f , new Rectangle(1819.39f, 500.00f , 200.00f , 600.00f) , 0, 0, 2119.39f, 500.00f));

			// SampleBoxes.Add(new SampleBox("watermark1", SM_WATERMARK1, 270f, 38f , new Rectangle(169.21f , 292.24f , 508.50f , 3367.35f), 0, 0, 569.91f, 3258.81f));
			// SampleBoxes.Add(new SampleBox("optional 2", SM_OPT2      , 270f, 30f , new Rectangle(300.00f , 2336.64f, 200.00f , 600.00f) , 0, 0, 473.21f, 2956.26f));


			// SampleBoxes.Add(new SampleBox());
		}
	}
}