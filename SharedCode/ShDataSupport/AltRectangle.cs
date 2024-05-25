// Solution:     ReadPDFText
// Project:       ReadPDFManager
// File:             AltRectangle.cs
// Created:      2024-05-11 (9:06 AM)

using System.Runtime.Serialization;
using iText.Kernel.Geom;

namespace SharedCode.ShDataSupport
{
	[DataContract (Namespace = "")]
	public class AltRectangle
	{
		private float x;

		private float y;

		private float width;

		private float height;
		// private float x, y;
		// private float width, height;

		public AltRectangle(float x,  float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		[DataMember(Order = 1)]
		public float X
		{
			get => x;
			set => x = value;
		}

		[DataMember(Order = 2)]
		public float Y
		{
			get => y;
			set => y = value;
		}

		[DataMember(Order = 3)]
		public float Width
		{
			get => width;
			set => width = value;
		}

		[DataMember(Order = 4)]
		public float Height
		{
			get => height;
			set => height = value;
		}

		public float GetX() => x;
		public float GetY() => y;
		public float GetWidth() =>width; 
		public float GetHeight() =>height;

		public static Rectangle MakeRectangle(AltRectangle a)
		{
			return new Rectangle(a.X, a.Y, a.Width, a.Height);
		}

		public static Rectangle[] MakeRectangles(AltRectangle[] a)
		{
			Rectangle[] r = new Rectangle[a.Length];

			for (var i = 0; i < a.Length; i++)
			{
				r[i]= MakeRectangle(a[i]);
			}

			return r;
		}

		public static AltRectangle MakeAltRectangle(Rectangle r)
		{
			return new AltRectangle(r.GetX(), r.GetY(), r.GetWidth(), r.GetHeight());
		}

		public static AltRectangle[] MakeAltRectangles(Rectangle[] r)
		{
			AltRectangle[] a = new AltRectangle[r.Length];

			for (var i = 0; i < r.Length; i++)
			{
				a[i] = MakeAltRectangle(r[i]);
			}

			return a;
		}

		public static Rectangle[,][] MakeRectanglesMd(AltRectangle[] a)
		{
			Rectangle[,][] bannerRects	= new Rectangle[SheetData.BAN_RECT_HV, SheetData.BAN_RECT_TB][];

			int idx;

			for (int i = 0; i < SheetData.BAN_RECT_HV; i++)
			{
				for (int j = 0; j < SheetData.BAN_RECT_TB; j++)
				{
					bannerRects[i, j] = new Rectangle[SheetData.BAN_RECT_NR];

					for (int k = 0; k < SheetData.BAN_RECT_NR; k++)
					{
						idx = i*4 + j*2 + k;

						if (a[idx]==null) continue;

						bannerRects[i, j][k] = AltRectangle.MakeRectangle(a[idx]);;
					}
				}
			}

			return bannerRects;
		}

		public static AltRectangle[] MakeAltRectangledMd(Rectangle[,][] r)
		{
			AltRectangle[] bannerRectsA = new AltRectangle[SheetData.BAN_RECT_QTY];

			int idx;

			for (int i = 0; i < SheetData.BAN_RECT_HV; i++)
			{
				for (int j = 0; j < SheetData.BAN_RECT_TB; j++)
				{
					for (int k = 0; k < SheetData.BAN_RECT_NR; k++)
					{
						idx = i*4 + j*2 + k;

						if (r[i,j]?[k] == null) continue;

						bannerRectsA[idx] = AltRectangle.MakeAltRectangle(r[i,j][k]);
					}
				}
			}

			return bannerRectsA;
		}


		public override string ToString()
		{
			return $"x| {x:F2} | y| {y:F2} | w| {width:F2} | h| {height:F2}";
		}
	}
}