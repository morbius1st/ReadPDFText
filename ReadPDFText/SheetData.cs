#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using static SharedCode.ShDataSupport.SheetMargin;
using static SharedCode.ShDataSupport.SheetZone;
using static SharedCode.ShDataSupport.BoxZone;
using static SharedCode.ShDataSupport.SheetBorderType;
using static SharedCode.ShDataSupport.ZoneAnchor;
using static SharedCode.ShDataSupport.RectangleSide;
using static SharedCode.ShDataSupport.BannerPosition;
using static SharedCode.ShDataSupport.BannerOrientation;

#endregion

// user name: jeffs
// created:   2/17/2024 7:51:41 AM

//
// namespace ReadPDFText
// {
// 	[DataContract]
// 	public class AltRectangle
// 	{
// 		private float x;
//
// 		private float y;
//
// 		private float width;
//
// 		private float height;
// 		// private float x, y;
// 		// private float width, height;
//
// 		public AltRectangle(float x,  float y, float width, float height)
// 		{
// 			this.x = x;
// 			this.y = y;
// 			this.width = width;
// 			this.height = height;
// 		}
//
// 		[DataMember(Order = 1)]
// 		public float X
// 		{
// 			get => x;
// 			set => x = value;
// 		}
//
// 		[DataMember(Order = 2)]
// 		public float Y
// 		{
// 			get => y;
// 			set => y = value;
// 		}
//
// 		[DataMember(Order = 3)]
// 		public float Width
// 		{
// 			get => width;
// 			set => width = value;
// 		}
//
// 		[DataMember(Order = 4)]
// 		public float Height
// 		{
// 			get => height;
// 			set => height = value;
// 		}
//
// 		public float GetX() => x;
// 		public float GetY() => y;
// 		public float GetWidth() =>width; 
// 		public float GetHeight() =>height;
//
// 		public static Rectangle MakeRectangle(AltRectangle a)
// 		{
// 			return new Rectangle(a.X, a.Y, a.Width, a.Height);
// 		}
//
// 		public static Rectangle[] MakeRectangles(AltRectangle[] a)
// 		{
// 			Rectangle[] r = new Rectangle[a.Length];
//
// 			for (var i = 0; i < a.Length; i++)
// 			{
// 				r[i]= MakeRectangle(a[i]);
// 			}
//
// 			return r;
// 		}
//
// 		public static AltRectangle MakeAltRectangle(Rectangle r)
// 		{
// 			return new AltRectangle(r.GetX(), r.GetY(), r.GetWidth(), r.GetHeight());
// 		}
//
// 		public static AltRectangle[] MakeAltRectangles(Rectangle[] r)
// 		{
// 			AltRectangle[] a = new AltRectangle[r.Length];
//
// 			for (var i = 0; i < r.Length; i++)
// 			{
// 				a[i] = MakeAltRectangle(r[i]);
// 			}
//
// 			return a;
// 		}
//
// 		public static Rectangle[,][] MakeRectanglesMd(AltRectangle[] a)
// 		{
// 			Rectangle[,][] bannerRects	= new Rectangle[SheetData.BAN_RECT_HV, SheetData.BAN_RECT_TB][];
//
// 			int idx;
//
// 			for (int i = 0; i < SheetData.BAN_RECT_HV; i++)
// 			{
// 				for (int j = 0; j < SheetData.BAN_RECT_TB; j++)
// 				{
// 					bannerRects[i, j] = new Rectangle[SheetData.BAN_RECT_NR];
//
// 					for (int k = 0; k < SheetData.BAN_RECT_NR; k++)
// 					{
// 						idx = i*4 + j*2 + k;
//
// 						if (a[idx]==null) continue;
//
// 						bannerRects[i, j][k] = AltRectangle.MakeRectangle(a[idx]);;
// 					}
// 				}
// 			}
//
// 			return bannerRects;
// 		}
//
// 		public static AltRectangle[] MakeAltRectangledMd(Rectangle[,][] r)
// 		{
// 			AltRectangle[] bannerRectsA = new AltRectangle[SheetData.BAN_RECT_QTY];
//
// 			int idx;
//
// 			for (int i = 0; i < SheetData.BAN_RECT_HV; i++)
// 			{
// 				for (int j = 0; j < SheetData.BAN_RECT_TB; j++)
// 				{
// 					for (int k = 0; k < SheetData.BAN_RECT_NR; k++)
// 					{
// 						idx = i*4 + j*2 + k;
//
// 						if (r[i,j]?[k] == null) continue;
//
// 						bannerRectsA[idx] = AltRectangle.MakeAltRectangle(r[i,j][k]);
// 					}
// 				}
// 			}
//
// 			return bannerRectsA;
// 		}
//
// 		public override string ToString()
// 		{
// 			return $"x| {x} | y| {y} | w| {width} | h| {height}";
// 		}
// 	}
// }
//
//
// namespace ReadPDFText
// {
// 	public enum SheetBorderType
// 	{
// 		ST_NA,
// 		ST_AO_COMMON,
// 		ST_AO_36X48_WITH_PACKAGE,
// 		ST_AO_36X48,
// 		ST_AO_30X42_WITH_PACKAGE,
// 		ST_AO_30X42,
// 		ST_AO_24X36_WITH_PACKAGE,
// 		ST_AO_24X36,
// 		ST_CS_COMMON,
// 		ST_CS_11X17,
// 	}
//
// 	public enum SheetMargin
// 	{
// 		SM_LEFT,
// 		SM_TOP,
// 		SM_RIGHT,
// 		SM_BOTTOM
// 	}
//
// 	public enum SheetZone
// 	{
// 		SZ_NA,
// 		SZ_SHT_RECT,
// 		SZ_SHT_TITLE_BLOCK_RECT,
// 		SZ_SHT_MARGINS_RECT,
// 		SZ_SHT_CONTENT_RECT,
//
// 		SZ_SHT_NUM_FIND_RECT,
// 		SZ_SHT_NUM_LINK_RECT,
//
// 		SZ_FOOTER_RECT,
// 		SZ_DISCLAIMER_RECT,
//
// 		SZ_PRIME_AUTHOR_RECT,
//
// 		// this one must come first or must revise below
// 		SZ_BANNER_BR_VERT_RECT,
// 		SZ_BANNER_BR_HORIZ_RECT,
// 		SZ_BANNER_TR_VERT_RECT,
// 		SZ_BANNER_TR_HORIZ_RECT,
//
// 		SZ_MARGINS,
// 	}
//
// 	public enum BoxZone
// 	{
// 		SZD_SHT_NUM_W_PKG_FIND_BOX,
// 		SZD_SHT_NUM_W_PKG_LINK_BOX,
//
// 		SZD_SHT_NUM_NOPKG_FIND_BOX,
// 		SZD_SHT_NUM_NOPKG_LINK_BOX,
//
// 		SZD_FOOTER_BOX,
// 		SZD_DISCLAIMER_BOX,
//
// 		SZD_PRIME_AUTHOR_BOX,
//
// 		SZD_BANNER_BR_VERT_BOX,
// 		SZD_BANNER_BR_HORIZ_BOX,
// 		SZD_BANNER_TR_VERT_BOX,
// 		SZD_BANNER_TR_HORIZ_BOX,
//
// 		SZD_SHT_MARGINS,
// 		SZD_SHT_CONTENT_MARGINS,
// 	}
//
// 	public enum ZoneAnchor
// 	{
// 		ZA_NA,
// 		ZA_BL,
// 		ZA_TL,
// 		ZA_TR,
// 		ZA_BR
// 	}
//
// 	public enum BannerPosition
// 	{
// 		BP_BOTTOM   = 0,
// 		BP_TOP      = 1,
// 	}
//
// 	public enum BannerOrientation
// 	{
// 		BO_VERTICAL	  = 0,
// 		BO_HORIZONTAL = 1,
// 	}
//
// 	public enum RectangleSide
// 	{
// 		RS_X = 0,
// 		RS_LEFT = RS_X,
// 		RS_Y = 1,
// 		RS_TOP = RS_Y,
// 		RS_W = 2,
// 		RS_RIGHT = RS_W,
// 		RS_H = 3,
// 		RS_BOTTOM = RS_H,
// 	}
//
// 	[DataContract(Namespace = "")]
// 	public struct SheetData
// 	{
// 		public const int CVT_FACTOR = 72;
//
// 		public const int BAN_RECT_HV = 2; // horiz / vert numbers
// 		public const int BAN_RECT_TB = 2; // tot  / bott numbers
// 		public const int BAN_RECT_NR = 2; // not-rot / rot numbers
// 		public const int BAN_RECT_QTY = BAN_RECT_HV * BAN_RECT_TB*BAN_RECT_NR;
//
//
// 		[DataMember(Order = 1)]
// 		public SheetBorderType SheetBorderType
// 		{
// 			get => sheetBorderType;
// 			private set => sheetBorderType = value;
// 		}
//
// 		[DataMember(Order = 2)]
// 		public string Name
// 		{
// 			get => name;
// 			private set => name = value;
// 		}
//
// 		[DataMember(Order = 3)]
// 		public string Description
// 		{
// 			get => description;
// 			private set => description = value;
// 		}
//
// 		// public BannerLocations BannerLocation
// 		// {
// 		// 	get => bannerLocation;
// 		// 	set
// 		// 	{
// 		// 		if (value == bannerLocation) return;
// 		//
// 		// 		bannerLocation = value;
// 		// 		// configBannerRect();
// 		// 	}
// 		// }
//
// 		[DataMember(Order = 100)]
// 		public Dictionary<SheetZone, float[]> zones;
//
// 		[DataMember(Order = 4)]
// 		public float Height
// 		{
// 			get => height;
// 			private set => height = value;
// 		}
//
// 		[DataMember(Order = 5)]
// 		public float Width
// 		{
// 			get => width;
// 			private set => width = value;
// 		}
//
// 		[DataMember(Order = 6)]
// 		public float[] Margins
// 		{
// 			get => margins;
// 			private set => margins = value;
// 		}
//
//
//
// 		[DataMember(Order = 7)]
// 		public AltRectangle[] SheetRectA
// 		{
// 			get => sheetRectA;
// 			set
// 			{
// 				sheetRectA = value;
//
// 				sheetRect = AltRectangle.MakeRectangles(value);
//
// 			}
// 		}
//
// 		[DataMember(Order = 8)]
// 		public AltRectangle[] TitleBlockRectA
// 		{
// 			get => titleBlockRectA;
// 			private set
// 			{
// 				titleBlockRectA = value;
// 				titleBlockRect	= AltRectangle.MakeRectangles(value);
// 			}
// 		}
//
// 		[DataMember(Order = 9)]
// 		public AltRectangle[] ContentRectA
// 		{
// 			get => contentRectA;
// 			private set
// 			{
// 				contentRectA = value;
// 				contentRect	= AltRectangle.MakeRectangles(value);
// 			}
// 		}
//
// 		[DataMember(Order = 10)]
// 		public AltRectangle[] MarginRectA
// 		{
// 			get => marginRectA;
// 			private set
// 			{
// 				marginRectA = value;
// 				marginRect	= AltRectangle.MakeRectangles(value); }
// 		}
//
// 		[DataMember(Order = 11)]
// 		public AltRectangle[] SheetNumberLinkRectA
// 		{
// 			get => sheetNumberLinkRectA;
// 			private set
// 			{ 
// 				sheetNumberLinkRectA = value;
// 				sheetNumberLinkRect	= AltRectangle.MakeRectangles(value);
// 			}
// 		}
//
// 		[DataMember(Order = 12)]
// 		public AltRectangle[] SheetNumberFindRectA
// 		{
// 			get => sheetNumberFindRectA;
// 			private set
// 			{
// 				sheetNumberFindRectA = value;
// 				sheetNumberFindRect	= AltRectangle.MakeRectangles(value);
// 			}
// 		}
//
// 		[DataMember(Order = 13)]
// 		public AltRectangle[] DisclaimerRectA
// 		{
// 			get => disclaimerRectA;
// 			private set
// 			{
// 				disclaimerRectA = value;
// 				disclaimerRect	= AltRectangle.MakeRectangles(value);
// 			}
// 		}
//
// 		[DataMember(Order = 14)]
// 		public AltRectangle[] FooterRectA
// 		{
// 			get => footerRectA;
// 			private set
// 			{
// 				footerRectA = value;
// 				footerRect	= AltRectangle.MakeRectangles(value);
// 			}
// 		}
//
// 		[DataMember(Order = 15)]
// 		public AltRectangle[] PrimaryAuthorRectA
// 		{
// 			get => primaryAuthorRectA;
// 			private set
// 			{
// 				primaryAuthorRectA = value;
// 				primaryAuthorRect	= AltRectangle.MakeRectangles(value);
// 			}
// 		}
//
//
// 		// [?,] = bannerorientation
// 		// [,?] = bannerposition
// 		// [ bannerorientation , bannerposition ]
// 		[DataMember(Order = 21)]
// 		public AltRectangle[] BannerRectsA
// 		{
// 			get => bannerRectsA;
// 			private set
// 			{
// 				bannerRectsA = value;
//
// 				bannerRects = AltRectangle.MakeRectanglesMd(value);
//
// 				/*
// 				bannerRects	= new Rectangle[BAN_RECT_HV, BAN_RECT_TB][];
//
// 				int idx;
//
// 				for (int i = 0; i < BAN_RECT_HV; i++)
// 				{
// 					for (int j = 0; j < BAN_RECT_TB; j++)
// 					{
// 						bannerRects[i, j] = new Rectangle[BAN_RECT_NR];
//
// 						for (int k = 0; k < BAN_RECT_NR; k++)
// 						{
// 							idx = i + j + k;
//
// 							bannerRects[i, j][k] = AltRectangle.MakeRectangle(value[idx]);;
// 						}
// 					}
// 				}
// 				*/
//
// 			}
// 		}
//
//
//
// 		[IgnoreDataMember]
// 		public Rectangle[] SheetRect
// 		{
// 			get => sheetRect;
// 			set
// 			{
// 				sheetRect = value;
//
// 				sheetRectA = AltRectangle.MakeAltRectangles(value);
// 			}
// 		}
//
// 		[IgnoreDataMember]
// 		public Rectangle[] TitleBlockRect
// 		{
// 			get => titleBlockRect;
// 			private set
// 			{
// 				titleBlockRect = value;
// 				titleBlockRectA	= AltRectangle.MakeAltRectangles(value);
// 			}
// 		}
//
// 		[IgnoreDataMember]
// 		public Rectangle[] ContentRect
// 		{
// 			get => contentRect;
// 			private set
// 			{
// 				contentRect = value;
// 				contentRectA	= AltRectangle.MakeAltRectangles(value);
// 			}
// 		}
//
// 		[IgnoreDataMember]
// 		public Rectangle[] MarginRect
// 		{
// 			get => marginRect;
// 			private set
// 			{ 
// 				marginRect = value;
// 				marginRectA	= AltRectangle.MakeAltRectangles(value);
// 			}
// 		}
//
// 		[IgnoreDataMember]
// 		public Rectangle[] SheetNumberLinkRect
// 		{
// 			get => sheetNumberLinkRect;
// 			private set
// 			{
// 				sheetNumberLinkRect = value;
// 				sheetNumberLinkRectA	= AltRectangle.MakeAltRectangles(value);
// 			}
// 		}
//
// 		[IgnoreDataMember]
// 		public Rectangle[] SheetNumberFindRect
// 		{
// 			get => sheetNumberFindRect;
// 			private set
// 			{
// 				sheetNumberFindRect = value;
// 				sheetNumberFindRectA	= AltRectangle.MakeAltRectangles(value);
// 			}
// 		}
//
// 		[IgnoreDataMember]
// 		public Rectangle[] DisclaimerRect
// 		{
// 			get => disclaimerRect;
// 			private set
// 			{
// 				disclaimerRect = value;
// 				disclaimerRectA	= AltRectangle.MakeAltRectangles(value);
// 			}
// 		}
//
// 		[IgnoreDataMember]
// 		public Rectangle[] FooterRect
// 		{
// 			get => footerRect;
// 			private set
// 			{
// 				footerRect = value;
// 				footerRectA	= AltRectangle.MakeAltRectangles(value);
// 			}
// 		}
//
// 		[IgnoreDataMember]
// 		public Rectangle[] PrimaryAuthorRect
// 		{
// 			get => primaryAuthorRect;
// 			private set
// 			{
// 				primaryAuthorRect = value;
// 				primaryAuthorRectA	= AltRectangle.MakeAltRectangles(value);
// 			}
// 		}
//
//
// 		// [?,] = bannerorientation
// 		// [,?] = bannerposition
// 		// [ bannerorientation , bannerposition ]
// 		[IgnoreDataMember]
// 		public Rectangle[,][] BannerRects
// 		{
// 			get => bannerRects;
// 			private set
// 			{
// 				bannerRects = value;
//
// 				bannerRectsA = AltRectangle.MakeAltRectangledMd(value);
//
// 				/*
// 				bannerRectsA = new AltRectangle[BAN_RECT_QTY];
//
// 				int idx;
//
// 				for (int i = 0; i < BAN_RECT_HV; i++)
// 				{
// 					for (int j = 0; j < BAN_RECT_TB; j++)
// 					{
// 						for (int k = 0; k < BAN_RECT_NR; k++)
// 						{
// 							idx = i + j + k;
//
// 							bannerRectsA[idx] = AltRectangle.MakeAltRectangle(value[i,j][k]);
// 						}
// 					}
// 				}
// 				*/
//
// 			}
// 		}
//
//
// 		private int cvt;
//
// 		private int x, l;
// 		private int y, t;
// 		private int w, r;
// 		private int h, b;
// 		private SheetBorderType sheetBorderType;
// 		private string  name;
// 		private string  description;
// 		private float   height;
// 		private float   width;
// 		private float[] margins;
//
// 		private AltRectangle[]    sheetRectA;
// 		private AltRectangle[]    titleBlockRectA;
// 		private AltRectangle[]    contentRectA;
// 		private AltRectangle[]    marginRectA;
// 		private AltRectangle[]    sheetNumberLinkRectA;
// 		private AltRectangle[]    sheetNumberFindRectA;
// 		private AltRectangle[]    disclaimerRectA;
// 		private AltRectangle[]    footerRectA;
// 		private AltRectangle[]    primaryAuthorRectA;
// 		private AltRectangle[]    bannerRectsA;
//
// 		private Rectangle[]    sheetRect;
// 		private Rectangle[]    titleBlockRect;
// 		private Rectangle[]    contentRect;
// 		private Rectangle[]    marginRect;
// 		private Rectangle[]    sheetNumberLinkRect;
// 		private Rectangle[]    sheetNumberFindRect;
// 		private Rectangle[]    disclaimerRect;
// 		private Rectangle[]    footerRect;
// 		private Rectangle[]    primaryAuthorRect;
// 		private Rectangle[,][] bannerRects;
//
//
// 		public SheetData(SheetBorderType borderType, 
// 			string name, string desc, Dictionary<SheetZone, float[]> shtZoneData) : this()
// 		{
// 			sheetRectA = null;
// 			sheetRect = null;
//
// 			titleBlockRectA = null;
// 			contentRectA = null;
// 			marginRectA = null;
// 			sheetNumberLinkRectA = null;
// 			sheetNumberFindRectA = null;
// 			disclaimerRectA = null;
// 			footerRectA = null;
// 			primaryAuthorRectA = null;
// 			bannerRectsA = null;
//
//
// 			titleBlockRect = null;
// 			contentRect = null;
// 			marginRect = null;
// 			sheetNumberLinkRect = null;
// 			sheetNumberFindRect = null;
// 			disclaimerRect = null;
// 			footerRect = null;
// 			primaryAuthorRect = null;
// 			bannerRects = null;
//
// 			SheetBorderType = borderType;
// 			Name = name;
// 			Description = desc;
//
// 			this.zones = shtZoneData;
//
// 			l = x = (int) SM_LEFT;
// 			t = y = (int) SM_TOP;
// 			r = w = (int) SM_RIGHT;
// 			b = h = (int) SM_BOTTOM;
//
// 			cvt = CVT_FACTOR;
// 			convertZoneData();
//
// 			Width = zones[SZ_SHT_RECT][w];
// 			Height = zones[SZ_SHT_RECT][h];
//
// 			configRects();
// 		}
//
// 		private SheetData(SheetBorderType st)
// 		{
// 			this.zones          = null;
// 			this.cvt            = 0;
//
// 			name = null;
// 			description = null;
// 			height = 0;
// 			width = 0;
// 			margins = null;
//
// 			sheetBorderType = st;
//
// 			l = x = (int) SM_LEFT;
// 			t = y = (int) SM_TOP;
// 			r = w = (int) SM_RIGHT;
// 			b = h = (int) SM_BOTTOM;
//
// 			sheetRectA = null;
// 			titleBlockRectA = null;
// 			contentRectA = null;
// 			marginRectA = null;
// 			sheetNumberLinkRectA = null;
// 			sheetNumberFindRectA = null;
// 			disclaimerRectA = null;
// 			footerRectA = null;
// 			primaryAuthorRectA = null;
// 			bannerRectsA = null;
//
//
// 			sheetRect = null;
// 			titleBlockRect = null;
// 			contentRect = null;
// 			marginRect = null;
// 			sheetNumberLinkRect = null;
// 			sheetNumberFindRect = null;
// 			disclaimerRect = null;
// 			footerRect = null;
// 			primaryAuthorRect = null;
// 			bannerRects = null;
//
// 			/*
// 			SheetBorderType     = st;
// 			Name                = null;
// 			Description         = null;
// 			Height              = 0f;
// 			Width               = 0f;
// 			Margins             = null;
//
// 			SheetRectA          = null;
//
// 			TitleBlockRectA      = null;
// 			ContentRectA         = null;
// 			MarginRectA          = null;
// 			SheetNumberLinkRectA = null;
// 			SheetNumberFindRectA = null;
// 			DisclaimerRectA      = null;
// 			FooterRectA          = null;
// 			PrimaryAuthorRectA   = null;
// 			BannerRectsA         = null;
//
// 			SheetRect           = null;
// 			TitleBlockRect      = null;
// 			ContentRect         = null;
// 			MarginRect          = null;
// 			SheetNumberLinkRect = null;
// 			SheetNumberFindRect = null;
// 			DisclaimerRect      = null;
// 			FooterRect          = null;
// 			PrimaryAuthorRect   = null;
// 			BannerRects         = null;
// 			*/
//
// 		}
//
// 		public SheetData NotUsed()
// 		{
// 			return new SheetData(ST_NA);
// 		}
//
// 		private void configRects()
// 		{
// 			Margins = zones[SZ_MARGINS];
//
// 			// SheetRect = new Rectangle[2];
// 			// TitleBlockRect  = new Rectangle[2];
// 			// ContentRect  = new Rectangle[2];
// 			// MarginRect  = new Rectangle[2];
// 			// SheetNumberLinkRect  = new Rectangle[2];
// 			// SheetNumberFindRect = new Rectangle[2];
// 			// DisclaimerRect  = new Rectangle[2];
// 			// FooterRect  = new Rectangle[2];
// 			// PrimaryAuthorRect = new Rectangle[2];
//
// 			SheetRect           = makeRectangles(getZoneRectangle(SZ_SHT_RECT));
// 			TitleBlockRect      = makeRectangles(getZoneRectangle(SZ_SHT_TITLE_BLOCK_RECT));
// 			MarginRect          = makeRectangles(getZoneRectangle(SZ_SHT_MARGINS_RECT));
// 			ContentRect         = makeRectangles(getZoneRectangle(SZ_SHT_CONTENT_RECT));
// 			SheetNumberFindRect = makeRectangles(getZoneRectangle(SZ_SHT_NUM_FIND_RECT));
// 			SheetNumberLinkRect = makeRectangles(getZoneRectangle(SZ_SHT_NUM_LINK_RECT));
// 			FooterRect          = makeRectangles(getZoneRectangle(SZ_FOOTER_RECT));
// 			DisclaimerRect      = makeRectangles(getZoneRectangle(SZ_DISCLAIMER_RECT));
// 			PrimaryAuthorRect   = makeRectangles(getZoneRectangle(SZ_PRIME_AUTHOR_RECT));
//
// 			configBannerRect();
// 		}
//
// 		private void configBannerRect()
// 		{
// 			BannerRects = new Rectangle[BAN_RECT_HV, BAN_RECT_TB][];
//
// 			BannerRects[(int) BO_VERTICAL, (int) BP_BOTTOM] =
// 				makeRectangles(getZoneRectangle((SheetZone) SZ_BANNER_BR_VERT_RECT));
//
// 			BannerRects[(int) BO_HORIZONTAL, (int) BP_BOTTOM] =
// 				makeRectangles(getZoneRectangle((SheetZone) SZ_BANNER_BR_HORIZ_RECT));
//
// 			BannerRects[(int) BO_VERTICAL, (int) BP_TOP] =
// 				makeRectangles(getZoneRectangle((SheetZone) SZ_BANNER_TR_VERT_RECT));
//
// 			BannerRects[(int) BO_HORIZONTAL, (int) BP_TOP] =
// 				makeRectangles(getZoneRectangle((SheetZone) SZ_BANNER_TR_HORIZ_RECT));
//
// 			bannerRectsA = AltRectangle.MakeAltRectangledMd(BannerRects);
//
// 		}
//
//
// 		private Rectangle[] makeRectangles(Rectangle zoneRect)
// 		{
// 			return new [] { zoneRect, rotateRectangle90(zoneRect) };
// 		}
//
// 		private Rectangle getZoneRectangle(SheetZone zone)
// 		{
// 			float[] z = this.zones[zone];
// 			return new Rectangle(z[x] , z[y] , z[w] , z[h] );
// 		}
//
// 		// public float Margin(SheetMargin side)
// 		// {
// 		// 	return Margins[(int) side];
// 		// }
//
// 		private void convertZoneData()
// 		{
// 			foreach (SheetZone sz in Enum.GetValues(typeof(SheetZone)))
// 			{
// 				if (sz == SZ_NA) continue;
//
// 				float[] f = zones[sz];
//
// 				zones[sz] = new float[]
// 				{
// 					f[0] * cvt,
// 					f[1] * cvt,
// 					f[2] * cvt,
// 					f[3] * cvt,
// 				};
// 			}
// 		}
//
// 		private Rectangle rotateRectangle90(Rectangle r)
// 		{
// 			return new Rectangle(
// 				Height - r.GetY() - r.GetHeight(),
// 				r.GetX(), r.GetHeight(), r.GetWidth());
// 		}
// 	}
//
// 	/*
// 	public struct BoxZoneData
// 	{
// 		private SheetZone  sheetZone;
// 		private ZoneAnchor anchor;
// 		private float[]    zoneRect;
//
// 		public SheetZone SheetZone
// 		{
// 			get => sheetZone;
// 			private set => sheetZone = value;
// 		}
//
// 		public ZoneAnchor Anchor
// 		{
// 			get => anchor;
// 			private set => anchor = value;
// 		}
//
// 		public float[] ZoneRect
// 		{
// 			get => zoneRect;
// 			private set => zoneRect = value;
// 		}
//
// 		public BoxZoneData(SheetZone zone, ZoneAnchor anchor, float[] zoreRect)
// 		{
// 			sheetZone = zone;
// 			this.anchor = anchor;
// 			this.zoneRect = zoreRect;
// 		}
// 	}
// 	*/
//
// 	/*
// 	public class SheetConfig
// 	{
// 		public const int CVT_FACTOR = 72;
//
// 		private static int x, l;
// 		private static int y, t;
// 		private static int w, r;
// 		private static int h, b;
// 		private static Dictionary<SheetBorderType, SheetData> sheetConfigData;
// 		private static Dictionary<SheetBorderType, Dictionary<SheetZone, float[]>> sheetZones;
// 		private static Dictionary<SheetBorderType, Dictionary<BoxZone, BoxZoneData>> boxZones;
// 		private static Tuple<SheetBorderType, string, string>[] shtTypeNames1 = new []
// 		{
// 			new Tuple<SheetBorderType, string, string>(ST_AO_36X48_WITH_PACKAGE, "AO-36x48p", "AO 36x48 Sheet with Package Box" ),
// 			new Tuple<SheetBorderType, string, string>(ST_AO_36X48,              "AO-36x48" , "AO 36x48 Sheet" ),
// 			new Tuple<SheetBorderType, string, string>(ST_AO_30X42_WITH_PACKAGE, "AO-30x42p", "AO 30x42 Sheet with Package Box" ),
// 			new Tuple<SheetBorderType, string, string>(ST_AO_30X42,              "AO-30x42" , "AO 30x42 Sheet" ),
// 			new Tuple<SheetBorderType, string, string>(ST_AO_24X36_WITH_PACKAGE, "AO-24x36p", "AO 24x36 Sheet with Package Box" ),
// 			new Tuple<SheetBorderType, string, string>(ST_AO_24X36,              "AO-24x36" , "AO 24x36 Sheet" ),
// 			new Tuple<SheetBorderType, string, string>(ST_CS_11X17,              "CS-11x17" , "CS 11x17 Sheet" ),
// 		};
//
// 		public static Dictionary<SheetBorderType, SheetData> SheetConfigData
// 		{
// 			get => sheetConfigData;
// 			private set => sheetConfigData = value;
// 		}
//
// 		// this is the list of actual rectangle data
// 		// Dictionary<SheetBorderType, Dictionary<BoxZone, BoxZoneData>> ZonesPerSheetType = new Dictionary<SheetBorderType, Dictionary<BoxZone, BoxZoneData>>();
// 		private static Dictionary<SheetBorderType, Dictionary<SheetZone, float[]>> SheetZones
// 		{
// 			get => sheetZones;
// 			set => sheetZones = value;
// 		}
//
// 		private static Dictionary<SheetBorderType, Dictionary<BoxZone, BoxZoneData>> BoxZones
// 		{
// 			get => boxZones;
// 			set => boxZones = value;
// 		}
//
// 		private static Tuple<SheetBorderType, string, string>[] shtTypeNames
// 		{
// 			get => shtTypeNames1;
// 			set => shtTypeNames1 = value;
// 		}
//
// 		static SheetConfig()
// 		{
// 			l = x = (int) RS_X;
// 			t = y = (int) RS_Y;
// 			r = w = (int) RS_W;
// 			b = h = (int) RS_H;
//
// 			config();
// 		}
//
// 		private static void config()
// 		{
// 			SheetConfigData = new Dictionary<SheetBorderType, SheetData>();
// 			SheetZones = new Dictionary<SheetBorderType, Dictionary<SheetZone, float[]>>();
// 			BoxZones = new Dictionary<SheetBorderType, Dictionary<BoxZone, BoxZoneData>>();
//
// 			setSheetZones();
//
// 			foreach (Tuple<SheetBorderType, string, string> stid in shtTypeNames)
// 			{
// 				SheetData sd = new SheetData(stid.Item1, stid.Item2, stid.Item3, SheetZones[stid.Item1]);
//
// 				SheetConfigData.Add(stid.Item1, sd);
// 			}
// 		}
//
// 		private static BoxZoneData getZoneData(SheetZone zone, ZoneAnchor anchor, float[] qf)
// 		{
// 			return new BoxZoneData(zone, anchor,
// 				new []
// 				{
// 					qf[0]
// 					,
// 					qf[1]
// 					,
// 					qf[2]
// 					,
// 					qf[3]
// 	
// 				});
// 		}
//
// 		private static void setSheetZones()
// 		{
// 			Dictionary<BoxZone, BoxZoneData> boxes;
//
// 			float banLen = 16.0f;
//
// 			// boxes is the information used to create the rectangles
//
// 			// primary sheet configurations
// 			boxes = new Dictionary<BoxZone, BoxZoneData>();
//
// 			// common values - to be used when noted
// 			// zones not provided are always sheet type specific                                     l            t         r         b
// 			boxes.Add(SZD_SHT_CONTENT_MARGINS,    getZoneData(SZ_SHT_MARGINS_RECT , ZA_BL, new [] { 0f          , 0f      , 0f      , 2.72f   }));
// 			// box data                                                                              x            y         w         h
// 			boxes.Add(SZD_BANNER_BR_VERT_BOX,     getZoneData(SZ_SHT_MARGINS_RECT , ZA_BL, new [] { 0f          , -0.375f , banLen  , 0.375f  }));
// 			boxes.Add(SZD_BANNER_BR_HORIZ_BOX,    getZoneData(SZ_SHT_MARGINS_RECT , ZA_BL, new [] { -0.375f     , 0f      , 0.375f  , banLen  }));
// 			boxes.Add(SZD_BANNER_TR_VERT_BOX,     getZoneData(SZ_SHT_MARGINS_RECT , ZA_BR, new [] { -1 * banLen , -0.375f , banLen  , 0.375f  }));
// 			boxes.Add(SZD_BANNER_TR_HORIZ_BOX,    getZoneData(SZ_SHT_MARGINS_RECT , ZA_BR, new [] { 0f          , 0f      , 0.375f  , banLen  }));
// 			boxes.Add(SZD_FOOTER_BOX,             getZoneData(SZ_SHT_RECT         , ZA_TL, new [] { 0.04f       , -3.03f  , 0.05f   , 3.0f    }));
// 			boxes.Add(SZD_DISCLAIMER_BOX,         getZoneData(SZ_SHT_MARGINS_RECT , ZA_TR, new [] { -5.0f       , 0.16f   , 5.0f  , 0.13f   }));
// 			boxes.Add(SZD_PRIME_AUTHOR_BOX,       getZoneData(SZ_SHT_MARGINS_RECT , ZA_BR, new [] { -2.25f      , 0.22f   , 2.10f   , 2.125f  }));
// 			boxes.Add(SZD_SHT_NUM_W_PKG_LINK_BOX, getZoneData(SZ_SHT_MARGINS_RECT , ZA_BL, new [] { 0F          , 0F      , 1.25F   , 2.50F   }));
// 			boxes.Add(SZD_SHT_NUM_W_PKG_FIND_BOX, getZoneData(SZ_SHT_MARGINS_RECT , ZA_BL, new [] { 0.0694f     , 0.0694f , 1.15f   , 2.64f   }));
// 			boxes.Add(SZD_SHT_NUM_NOPKG_LINK_BOX, getZoneData(SZ_SHT_MARGINS_RECT , ZA_BL, new [] { 0F          , 0F      , 2.25F   , 2.50F   }));
// 			boxes.Add(SZD_SHT_NUM_NOPKG_FIND_BOX, getZoneData(SZ_SHT_MARGINS_RECT , ZA_BL, new [] { 0.0694f     , 0.0694f , 2.18f   , 2.64f   }));
//
// 			BoxZones.Add(ST_AO_COMMON, boxes);
//
//
// 			// alternate sheet configuration 1
// 			boxes = new Dictionary<BoxZone, BoxZoneData>();
//
// 			boxes.Add(SZD_SHT_CONTENT_MARGINS,    getZoneData(SZ_SHT_MARGINS_RECT , ZA_BL, new [] { 0f       , 0f        , 0f       , 1.62f }));
// 			// box data                                                                             x              y            w            h
// 			boxes.Add(SZD_BANNER_BR_VERT_BOX,     getZoneData(SZ_SHT_MARGINS_RECT , ZA_BL, new [] { 0f       , -0.087f   , 8.00f    , 0.087f }));
// 			boxes.Add(SZD_BANNER_BR_HORIZ_BOX,    getZoneData(SZ_SHT_MARGINS_RECT , ZA_BL, new [] { -0.087f  , 0f        , 0.087f   , 8.0f }));
// 			boxes.Add(SZD_BANNER_TR_VERT_BOX,     getZoneData(SZ_SHT_MARGINS_RECT , ZA_BR, new [] { -8.0f    , -.087f    , 8.0f     , 0.08f }));
// 			boxes.Add(SZD_BANNER_TR_HORIZ_BOX,    getZoneData(SZ_SHT_MARGINS_RECT , ZA_BR, new [] { 0f       , 0f        , 0.087f   , 8.0f }));
// 			boxes.Add(SZD_FOOTER_BOX,             getZoneData(SZ_SHT_RECT         , ZA_TL, new [] { 0.04f    , -3.03f    , 0.05f    , 3.0f  }));
// 			boxes.Add(SZD_DISCLAIMER_BOX,         getZoneData(SZ_SHT_MARGINS_RECT , ZA_TR, new [] { -5.0f    , 0.12f     , 5.0f     , 0.1f }));
// 			boxes.Add(SZD_PRIME_AUTHOR_BOX,       getZoneData(SZ_SHT_MARGINS_RECT , ZA_BR, new [] { -1.39f   , 0.057f    , 1.32f    , 1.45f }));  
// 			boxes.Add(SZD_SHT_NUM_W_PKG_LINK_BOX, new BoxZoneData(SZ_NA, ZA_NA, null));					     			   				       
// 			boxes.Add(SZD_SHT_NUM_W_PKG_FIND_BOX, new BoxZoneData(SZ_NA, ZA_NA, null));					     			   				       
// 			boxes.Add(SZD_SHT_NUM_NOPKG_LINK_BOX, getZoneData(SZ_SHT_MARGINS_RECT , ZA_BL, new [] { 0.06f    , 0.06f     , 0.75f    , 1.4f }));   
// 			boxes.Add(SZD_SHT_NUM_NOPKG_FIND_BOX, getZoneData(SZ_SHT_MARGINS_RECT , ZA_BL, new [] { 0f       , 0f        , 0.831f   , 1.5f }));
//
// 			BoxZones.Add(ST_CS_COMMON, boxes);
//
// 			// 36x48 - box data
// 			// package
// 			//
// 			boxes = new Dictionary<BoxZone, BoxZoneData>();
// 			//                                                                  L         T         R      B
// 			boxes.Add(SZD_SHT_MARGINS, getZoneData(SZ_SHT_RECT, ZA_BL, new [] { 0.70f, 1.1485f, 0.70f, 0.50f }));
// 			BoxZones.Add(ST_AO_36X48_WITH_PACKAGE, boxes);
//
// 			// not package
// 			boxes = cloneBoxes(boxes);
// 			BoxZones.Add(ST_AO_36X48, boxes);
//
// 			// 30x42 - box data
// 			// package
// 			//
// 			boxes = new Dictionary<BoxZone, BoxZoneData>();
// 			//                                                                   L      T      R      B
// 			boxes.Add(SZD_SHT_MARGINS, getZoneData(SZ_SHT_RECT, ZA_BL, new [] { 0.575f, 1.3672f, 0.575f, 0.50f }));
// 			BoxZones.Add(ST_AO_30X42_WITH_PACKAGE, boxes);
//
// 			boxes = cloneBoxes(boxes);
// 			BoxZones.Add(ST_AO_30X42, boxes);
//
// 			// 24x36 - box data
// 			// package
// 			//
// 			boxes = new Dictionary<BoxZone, BoxZoneData>();
// 			//                                                                  L      T      R      B
// 			boxes.Add(SZD_SHT_MARGINS, getZoneData(SZ_SHT_RECT, ZA_BL, new [] { 0.45f, 1.586f, 0.45f, 0.50f }));
// 			BoxZones.Add(ST_AO_24X36_WITH_PACKAGE, boxes);
//
// 			boxes = cloneBoxes(boxes);
// 			BoxZones.Add(ST_AO_24X36, boxes);
//
//
// 			boxes = new Dictionary<BoxZone, BoxZoneData>();
// 			//                                                                     L      T      R      B
// 			boxes.Add(SZD_SHT_MARGINS, getZoneData(SZ_SHT_RECT, ZA_BL, new [] { 0.106f, 0.442f, 0.091f, 0.12f }));
// 			BoxZones.Add(ST_CS_11X17, boxes);
//
//
// 			// boxes configured
//
// 			// zones has the actual / final rectangles
//
// 			makeZones(ST_AO_36X48_WITH_PACKAGE, ST_AO_COMMON,
// 				new [] { 0f, 0f, 36.0f, 48.0f },
// 				new [] { 0.7f, 0.5f, 34.6f, 2.5f },
// 				SZD_SHT_NUM_W_PKG_FIND_BOX,
// 				SZD_SHT_NUM_W_PKG_LINK_BOX
// 				);
//
// 			makeZones(ST_AO_36X48, ST_AO_COMMON,
// 				new [] { 0f, 0f, 36.0f, 48.0f },
// 				new [] { 0.7f, 0.5f, 34.6f, 2.5f },
// 				SZD_SHT_NUM_NOPKG_FIND_BOX,
// 				SZD_SHT_NUM_NOPKG_LINK_BOX
// 				);
//
// 			// 30 x 42
// 			//
// 			makeZones(ST_AO_30X42_WITH_PACKAGE, ST_AO_COMMON,
// 				new [] { 0.0f, 0.0f, 30.0f, 42.0f },
// 				new [] { 0.575f, 0.50f, 28.85f, 2.5f },
// 				SZD_SHT_NUM_W_PKG_FIND_BOX,
// 				SZD_SHT_NUM_W_PKG_LINK_BOX
// 				);
//
// 			makeZones(ST_AO_30X42, ST_AO_COMMON,
// 				new [] { 0.0f, 0.0f, 30.0f, 42.0f },
// 				new [] { 0.575f, 0.50f, 28.85f, 2.5f },
// 				SZD_SHT_NUM_NOPKG_FIND_BOX,
// 				SZD_SHT_NUM_NOPKG_LINK_BOX
// 				);
//
// 			// 24 x 36
// 			//
// 			makeZones(ST_AO_24X36_WITH_PACKAGE, ST_AO_COMMON,
// 				new [] { 0.0f, 0.0f, 24.0f, 36.0f },
// 				new [] { 0.45f, 0.50f, 23.1f, 2.5f },
// 				SZD_SHT_NUM_W_PKG_FIND_BOX,
// 				SZD_SHT_NUM_W_PKG_LINK_BOX
// 				);
//
// 			makeZones(ST_AO_24X36, ST_AO_COMMON,
// 				new [] { 0.0f, 0.0f, 24.0f, 36.0f },
// 				new [] { 0.45f, 0.50f, 23.1f, 2.5f },
// 				SZD_SHT_NUM_NOPKG_FIND_BOX,
// 				SZD_SHT_NUM_NOPKG_LINK_BOX
// 				);
//
// 			float[] sheetRect = new float[] { 0f, 0f, 11.0f, 17.0f };
// 			float[] ttlBlkRect = new float[] { 0.106f, 0.12f, 10.806f, 1.5f };
//
// 			makeZones(ST_CS_11X17, ST_CS_COMMON,
// 				sheetRect,
// 				ttlBlkRect,
// 				SZD_SHT_NUM_NOPKG_FIND_BOX,
// 				SZD_SHT_NUM_NOPKG_LINK_BOX
// 				);
// 		}
//
// 		private static void makeZones( SheetBorderType st,
// 			SheetBorderType common,
// 			float[] shtRect, float[] ttlBlkRect,
// 			BoxZone find, BoxZone link)
// 		{
// 			Dictionary<SheetZone, float[]> zones = new Dictionary<SheetZone, float[]>();
//
// 			// Debug.WriteLine($"make zones| A {st} | {common}");
//
// 			zones.Add(SZ_SHT_RECT,                shtRect);
// 			zones.Add(SZ_SHT_TITLE_BLOCK_RECT,    ttlBlkRect);
//
// 			SheetZones.Add(st, zones);
// 			// Debug.WriteLine($"make zones| B1");
//
// 			zones.Add(SZ_MARGINS, BoxZones[st][SZD_SHT_MARGINS].ZoneRect);
//
// 			// Debug.WriteLine($"make zones| B2");
//
// 			zones.Add(SZ_SHT_MARGINS_RECT, calcMargRect(st));
// 			// Debug.WriteLine($"make zones| B3");
// 			zones.Add(SZ_SHT_CONTENT_RECT, calcContRect(st, common));
//
// 			// Debug.WriteLine($"make zones| C");
// 			// Console.Write($"ZA for {SZ_SHT_NUM_FIND_RECT,-25}");
//
// 			zones.Add(SZ_SHT_NUM_FIND_RECT,
// 				getRectData(st, BoxZones[common][find]));
//
// 			// Debug.WriteLine($"make zones| D");
//
// 			// Console.Write($"ZA for {SZ_SHT_NUM_LINK_RECT,-25}");
//
// 			zones.Add(SZ_SHT_NUM_LINK_RECT,
// 				getRectData(st, BoxZones[common][link]));
//
// 			// Debug.WriteLine($"make zones| E");
//
// 			for (int i = 0; i <= (int) SZ_BANNER_TR_HORIZ_RECT - (int)SZ_FOOTER_RECT; i++)
// 			{
// 				// Console.Write($"ZA for {(SheetZone) (i + (int) SZ_FOOTER_RECT),-25}");
//
// 				zones.Add((SheetZone) (i + (int) SZ_FOOTER_RECT),
// 					getRectData(st,
// 						BoxZones[common][(BoxZone) (i + SZD_FOOTER_BOX)]));
// 			}
// 			// Debug.WriteLine($"make zones| F");
// 		}
//
// 		private static Dictionary<BoxZone, BoxZoneData> cloneBoxes(Dictionary<BoxZone, BoxZoneData> zones)
// 		{
// 			Dictionary<BoxZone, BoxZoneData> zones2 = new Dictionary<BoxZone, BoxZoneData>();
//
// 			float[] dupCopy;
//
// 			foreach (KeyValuePair<BoxZone, BoxZoneData> kvp in zones)
// 			{
// 				dupCopy = new float[kvp.Value.ZoneRect.Length];
//
// 				kvp.Value.ZoneRect.CopyTo(dupCopy, 0);
//
// 				BoxZoneData zd = new BoxZoneData(kvp.Value.SheetZone, kvp.Value.Anchor, dupCopy);
//
// 				zones2.Add(kvp.Key, zd);
// 			}
//
// 			return zones2;
// 		}
//
// 		
// 		private static Dictionary<SheetZone, float[]> cloneZones(Dictionary<SheetZone, float[]> zones)
// 		{
// 			Dictionary<SheetZone, float[]> zones2 = new Dictionary<SheetZone, float[]>();
//
// 			float[] dupCopy;
//
// 			foreach (KeyValuePair<SheetZone, float[]> kvp in zones)
// 			{
// 				dupCopy = new float[kvp.Value.Length];
//
// 				kvp.Value.CopyTo(dupCopy, 0);
//
// 				zones2.Add(kvp.Key, dupCopy);
// 			}
//
// 			return zones2;
// 		}
// 		
//
// // 		/*
// // 		private static void processZoneData()
// // 		{
// // 			// post processing to create the full data set
// //
// // 			// create 
// // 			// SZ_SHT_MARGINS_BOX - take sheet data and adjust based on margins
// // 			// SZ_SHT_CONTENT_BOX - is relative to marg bl - take margin data and adjust based on content margins
// // 			// for each sheet type
// // 			calcBoxes();
// //
// // 			// calc 
// // 			// footer rect
// // 			// disclaimer rect
// // 			// prime author rect
// // 			// find rect
// // 			// link rect
// // 		}
// //
// // 		private static void calcBoxes()
// // 		{
// // 			float x1;
// // 			float y1;
// // 			float w1;
// // 			float h1;
// //
// // 			float[] sht;
// // 			float[] marg;
// // 			float[] cont = BoxZones[ST_COMMON][SZD_SHT_CONTENT_MARGINS].ZoneRect;
// //
// // 			foreach (KeyValuePair<SheetType,
// // 						Dictionary<SheetZone, float[]>> kvp in SheetZones)
// // 			{
// // 				// calc margin box;
// // 				sht =  kvp.Value[SZ_SHT_RECT];
// // 				marg = BoxZones[kvp.Key][SZD_SHT_MARGINS].ZoneRect;
// //
// // 				x1 = marg[l];
// // 				y1 = marg[b];
// // 				w1 = sht[w] - x1 - marg[r];
// // 				h1 = sht[h] - y1 - marg[t];
// //
// //
// // 				SheetZones[kvp.Key].Add(SZ_SHT_MARGINS_RECT, new [] { x1, y1, w1, h1 });
// //
// // 				// calc content box
// // 				x1 += cont[l];
// // 				y1 += cont[b];
// // 				w1 -= cont[r];
// // 				h1 -= cont[t];
// //
// // 				SheetZones[kvp.Key].Add(SZ_SHT_CONTENT_RECT, new [] { x1, y1, w1, h1 });
// // 			}
// // 		}
// // 		
//
// 		private static float[] calcMargRect(SheetBorderType st)
// 		{
// 			Dictionary<SheetZone, float[]> a =  SheetZones[st];
// 			Dictionary<BoxZone, BoxZoneData> c = BoxZones[st];
//
// 			// float[] sht = SheetZones[st][SZ_SHT_RECT];
// 			float[] sht = a[SZ_SHT_RECT];
// 			float[] mrg = c[SZD_SHT_MARGINS].ZoneRect;
//
// 			float x1 = sht[x] + mrg[l];
// 			float y1 = sht[y] + mrg[b];
// 			float w1 = sht[w] - x1 - mrg[r];
// 			float h1 = sht[h] - y1 - mrg[t];
//
//
// 			return new [] { x1, y1, w1, h1 };
// 		}
//
// 		private static float[] calcContRect(SheetBorderType st, SheetBorderType common)
// 		{
// 			float[] mrgRect = SheetZones[st][SZ_SHT_MARGINS_RECT];
// 			// Debug.WriteLine($"calc cont rect| {st}");
// 			float[] contMrg = BoxZones[common][SZD_SHT_CONTENT_MARGINS].ZoneRect;
// 			// Debug.WriteLine($"calc cont rect| {common}");
//
// 			float x1 = mrgRect[x] + contMrg[l];
// 			float y1 = mrgRect[y] + contMrg[b];
// 			float w1 = mrgRect[w] - contMrg[l] - contMrg[r];
// 			float h1 = mrgRect[h] - contMrg[b] - contMrg[t];
//
// 			// Debug.WriteLine($"returning");
// 			return new [] { x1, y1, w1, h1 };
// 		}
//
// 		private static float[] getRectData(SheetBorderType st, BoxZoneData zd)
// 		{
// 			float[] result = new float[4];
//
// 			float[] anchorR = SheetZones[st][zd.SheetZone];
//
// 			float[] boxR = zd.ZoneRect;
//
// 			float anchorX;
// 			float anchorY;
// 			getAnchorXy(zd.Anchor, anchorR, out anchorX, out anchorY);
//
// 			string s = $"{st}/{zd.SheetZone}";
//
// 			// Console.WriteLine($" | {s,-45}| {zd.Anchor,-8}| x| {anchorX:F4}| y| {anchorY:F4}");
//
//
// 			result[x] = anchorX + boxR[x];
// 			result[y] = anchorY + boxR[y];
// 			result[w] = boxR[w];
// 			result[h] = boxR[h];
//
// 			return result;
// 		}
//
// 		private static void getAnchorXy(ZoneAnchor za, float[]  anchorRectData, out float ax, out float ay)
// 		{
// 			ax = 0f;
// 			ay = 0f;
//
// 			switch (za)
// 			{
// 			case ZA_BL:
// 				{
// 					ax = anchorRectData[x];
// 					ay = anchorRectData[y];
// 					break;
// 				}
//
// 			case ZA_TL:
// 				{
// 					ax = anchorRectData[x];
// 					ay = anchorRectData[y] + anchorRectData[h];
// 					break;
// 				}
//
// 			case ZA_TR:
// 				{
// 					ax = anchorRectData[x] + anchorRectData[w];
// 					ay = anchorRectData[y] + anchorRectData[h];
// 					break;
// 				}
//
// 			case ZA_BR:
// 				{
// 					ax = anchorRectData[x] + anchorRectData[w];
// 					ay = anchorRectData[y];
// 					break;
// 				}
// 			}
// 		}
//
// 		public static void ShowSheetData()
// 		{
// 			foreach (Tuple<SheetBorderType, string, string> nameData in shtTypeNames)
// 			{
// 				showSheetData(nameData.Item1);
// 			}
//
// 			Console.WriteLine($"");
// 		}
//
// 		private static void showSheetData(SheetBorderType borderType)
// 		{
// 			SheetData sd = SheetConfigData[borderType];
//
// 			int numWidth = 9;
// 			string numFmt = "F2";
// 			int descWidth = 2;
//
// 			float[] margs = new []
// 			{
// 				sd.Margins[0] / CVT_FACTOR,
// 				sd.Margins[1] / CVT_FACTOR,
// 				sd.Margins[2] / CVT_FACTOR,
// 				sd.Margins[3] / CVT_FACTOR,
// 			};
//
// 			string m = FormatItextData.FormatQuadFloat(
// 				cvtFloats(sd.Margins), numWidth, numFmt, new [] { "l", "t", "r", "b" }, descWidth);
//
// 			Console.WriteLine($"");
// 			Console.WriteLine($"** sheet information for | {sd.Name} ({sd.Description}) ***");
// 			Console.WriteLine($"{"sheet size",26}|  w| {sd.Width / CVT_FACTOR, 9:F2}|  h| {sd.Height / CVT_FACTOR, 9:F2}");
// 			Console.WriteLine($"{"sheet margins", 26}| {m}");
// 			
// 			Console.WriteLine($"\nNOT Rotated rectangles");
//
// 			Console.WriteLine($"{"sheet rect", 26}| {FormatItextData.FormatRectangle(sd.SheetRect[0], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"ttl blk rect", 26}| {FormatItextData.FormatRectangle(sd.TitleBlockRect[0], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"margins rect", 26}| {FormatItextData.FormatRectangle(sd.MarginRect[0], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"content rect", 26}| {FormatItextData.FormatRectangle(sd.ContentRect[0], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"find rect", 26}| {FormatItextData.FormatRectangle(sd.SheetNumberFindRect[0], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"link rect", 26}| {FormatItextData.FormatRectangle(sd.SheetNumberLinkRect[0], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"footer rect", 26}| {FormatItextData.FormatRectangle(sd.FooterRect[0], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"disclaimer rect", 26}| {FormatItextData.FormatRectangle(sd.DisclaimerRect[0], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"author rect", 26}| {FormatItextData.FormatRectangle(sd.DisclaimerRect[0], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
//
// 			for (var i = 0; i < 2; i++)
// 			{
// 				for (int j = 0; j < 2; j++)
// 				{
// 					Console.WriteLine($"{"banner location",26}|{(BannerOrientation) i} /  {(BannerPosition) j}");
// 					Console.WriteLine($"{"banner rect", 26}| {FormatItextData.FormatRectangle(sd.BannerRects[i,j][0], numWidth, numFmt, descWidth, 1f / CVT_FACTOR)}");
// 				}
// 			}
//
// 			
// 			Console.WriteLine($"\nRotated rectangles");
// 			
// 			Console.WriteLine($"{"sheet rect (rot)", 26}| {FormatItextData.FormatRectangle(sd.SheetRect[1], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"ttl blk rect (rot)", 26}| {FormatItextData.FormatRectangle(sd.TitleBlockRect[1], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"margins rect (rot)", 26}| {FormatItextData.FormatRectangle(sd.MarginRect[1], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"content rect (rot)", 26}| {FormatItextData.FormatRectangle(sd.ContentRect[1], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"find rect (rot)", 26}| {FormatItextData.FormatRectangle(sd.SheetNumberFindRect[1], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"link rect (rot)", 26}| {FormatItextData.FormatRectangle(sd.SheetNumberLinkRect[1], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"footer rect (rot)", 26}| {FormatItextData.FormatRectangle(sd.FooterRect[1], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"disclaimer rect (rot)", 26}| {FormatItextData.FormatRectangle(sd.DisclaimerRect[1], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
// 			Console.WriteLine($"{"author rect (rot)", 26}| {FormatItextData.FormatRectangle(sd.DisclaimerRect[1], numWidth, numFmt, descWidth,  1f / CVT_FACTOR)}");
//
//
// 			for (var i = 0; i < 2; i++)
// 			{
// 				for (int j = 0; j < 2; j++)
// 				{
// 					Console.WriteLine($"{"banner location",26}|{(BannerOrientation) i} /  {(BannerPosition) j}");
// 					Console.WriteLine($"{"banner rect (rot)", 26}| {FormatItextData.FormatRectangle(sd.BannerRects[i,j][1], numWidth, numFmt, descWidth, 1f / CVT_FACTOR)}");
// 				}
// 			}
//
// 		}
//
// 		private static float[] cvtFloats(float[] v )
// 		{
// 			return new []
// 			{
// 				v[0] / CVT_FACTOR,
// 				v[1] / CVT_FACTOR,
// 				v[2] / CVT_FACTOR,
// 				v[3] / CVT_FACTOR,
// 			};
// 		}
//
// 		public override string ToString()
// 		{
// 			return $"this is {nameof(SheetConfig)}";
// 		}
// 	}
// 	*/
// 	
// }

