// Solution:     ReadPDFText
// Project:       ReadPDFTextTests
// File:             ShtRectData.cs
// Created:      2024-05-16 (8:07 PM)

// using SharedCode.ShDataSupport;
// using SharedCode.ShPdfSupport;

using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.Text;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout.Properties;
using Newtonsoft.Json;
using ShSheetData.SheetData;
using UtilityLibrary;

namespace ShSheetData.ShSheetData2
{
	[DataContract]
	public class RGBx
	{
		[DataMember]
		public int R { get; set; }

		[DataMember]
		public int G { get; set; }

		[DataMember]
		public int B { get; set; }

		public RGBx(int r, int g, int b)
		{
			R = r;
			G = g;
			B = b;
		}

		public DeviceRgb GetDeviceRgb => new DeviceRgb(R, G, B);
	}

	[DataContract(Namespace = "")]
	public class TextSettings : ICloneable
	{
		private float[] textColor;

		public TextSettings() {}

		public TextSettings(string fontFamily, int fontStyle, float textSize)
		{
			Reset();

			FontFamily = fontFamily;
			FontStyle = fontStyle;
			TextSize = textSize;
		}

		public TextSettings(
			string infoText, string urlLink,
			string fontFamily, int fontStyle,
			float textSize, Color textColor, 
			VerticalAlignment textVertAlignment,
			HorizontalAlignment textHorizAlignment, int textWeight,
			float textOpacity)
		{
			InfoText = infoText;
			UrlLink = urlLink;

			FontFamily = fontFamily;
			FontStyle = fontStyle;
			TextSize = textSize;

			TextColor = textColor;
			TextVertAlignment = textVertAlignment;
			TextHorizAlignment = textHorizAlignment;
			TextWeight = textWeight;
			TextOpacity = textOpacity;
		}

		[DataMember(Order =  50)]
		public string InfoText { get; set; }

		[DataMember(Order =  50)]
		public string UrlLink { get; set; }

		// text info ...
		[DataMember(Order =  51)]
		public string FontFamily { get; set; }

		[DataMember(Order =  52)]
		public int FontStyle { get; set; } = iText.IO.Font.Constants.FontStyles.NORMAL;

		[DataMember(Order =  53)]
		public float TextSize { get; set; }

		[DataMember(Order =  54)]
		public VerticalAlignment TextVertAlignment { get; set; }

		[DataMember(Order =  55)]
		public HorizontalAlignment TextHorizAlignment { get; set; }

		[DataMember(Order =  61)]
		public int TextWeight { get; set; } = iText.IO.Font.Constants.FontWeights.NORMAL;

		[IgnoreDataMember]
		public Color TextColor
		{
			get => Color.CreateColorWithColorSpace(textColor);
			set => textColor = value?.GetColorValue() ?? new [] { 0f, 0f, 0f };
		}

		[DataMember(Order =  63)]
		public float[] TextColorA
		{
			get => textColor;
			set => textColor = value;
		}

		[DataMember(Order =  64)]
		public float TextOpacity { get; set; } = 1f;

		[DataMember(Order =  62)]
		public int TextDecoration { get; set; } = TextDecorations.NORMAL;

		public void Reset()
		{
			InfoText = null;
			UrlLink = null;

			FontFamily = "Arial";
			FontStyle = iText.IO.Font.Constants.FontStyles.NORMAL; // 1 = bold // 2 = italic

			TextSize = 12f;

			TextVertAlignment = VerticalAlignment.TOP; // must be top as this is a PDF default
			TextHorizAlignment = HorizontalAlignment.LEFT;

			TextWeight = iText.IO.Font.Constants.FontWeights.NORMAL;
			TextDecoration = TextDecorations.NORMAL;
			TextColor = ColorConstants.BLACK;
			TextOpacity = 1f;
		}

		public object Clone()
		{
			TextSettings copy = new TextSettings();

			copy.InfoText = InfoText;
			copy.UrlLink = UrlLink;

			copy.FontFamily = FontFamily;
			copy.FontStyle = FontStyle;
			copy.TextSize = TextSize;
			copy.TextHorizAlignment = TextHorizAlignment;
			copy.TextVertAlignment = TextVertAlignment;
			copy.TextWeight = TextWeight;
			copy.TextColor = TextColor;
			copy.TextOpacity = TextOpacity;
			copy.TextDecoration = TextDecoration;

			return copy;
		}

		public TextSettings Clone2()
		{
			return (TextSettings) Clone();
		}
	}

	[DataContract(Namespace = "")]
	public class BoxSettings : ICloneable
	{
		private float[] fillColor;
		private float[] bdrColor;
		private float[] rectangleA;

		public BoxSettings(Rectangle rect)
		{
			Reset();

			Rect = rect;
		}

		public BoxSettings(Rectangle rect,
			float tbRotation,
			Color fillColor, Color bdrColor,
			float fillOpacity, float bdrWidth, float bdrOpacity,
			float[] bdrDashPattern)
		{
			Rect = rect;
			TextBoxRotation = tbRotation;

			FillColor = fillColor;
			BdrColor = bdrColor;
			FillOpacity = fillOpacity;
			BdrWidth = bdrWidth;
			BdrOpacity = bdrOpacity;
			BdrDashPattern = bdrDashPattern;
		}

		private BoxSettings() { }

		// bounding box info ...
		// box location and dimensions

		// **
		[IgnoreDataMember]
		public Rectangle Rect
		{
			get => new Rectangle(rectangleA[0], rectangleA[1], rectangleA[2], rectangleA[3]);
			set
			{
				if (value != null)
				{
					rectangleA = new [] { value.GetX(), value.GetY(), value.GetWidth(), value.GetHeight() };
				}
				else
				{
					rectangleA = null;
				}
			}
		}

		[DataMember(Order =  2)]
		public float[] RectangleA
		{
			get => rectangleA;
			set => rectangleA = value;
		}


		[DataMember(Order =  0)]
		public float TextBoxRotation { get; set; }

		[DataMember(Order = 1)]
		public float BdrWidth { get; set; }

		[IgnoreDataMember]
		public Color BdrColor
		{
			get => Color.CreateColorWithColorSpace(bdrColor);
			set => bdrColor = value?.GetColorValue() ?? new [] { 0f, 0f, 1f };
		}

		[DataMember(Order = 2)]
		public float[] BdrColorA
		{
			get => bdrColor;
			set => bdrColor = value;
		}

		[DataMember(Order = 3)]
		public float BdrOpacity { get; set; }

		[DataMember(Order = 4)]
		public float[] BdrDashPattern { get; set; }

		[IgnoreDataMember]
		public Color FillColor
		{
			get => Color.CreateColorWithColorSpace(fillColor);
			set => fillColor = value?.GetColorValue() ?? new [] { 0f, 1f, 1f };
		}

		[DataMember(Order = 5)]
		public float[] FillColorA
		{
			get => fillColor;
			set => fillColor = value;
		}

		[DataMember(Order = 6)]
		public float FillOpacity { get; set; }

		public void Reset()
		{
			TextBoxRotation = 0;

			BdrWidth = 1f;
			BdrColor = DeviceRgb.BLACK;
			BdrOpacity = 1f;
			BdrDashPattern = null;

			FillColor = null;
			FillOpacity = 0f; // none / transparent
		}

		public object Clone()
		{
			BoxSettings copy = new BoxSettings();

			copy.TextBoxRotation = TextBoxRotation;

			copy.BdrWidth = BdrWidth;
			copy.BdrColor = BdrColor;
			copy.BdrOpacity = BdrOpacity;
			copy.BdrDashPattern = BdrDashPattern = (float[]) BdrDashPattern?.Clone() ?? null;
			copy.FillColor = FillColor;
			copy.FillOpacity = FillOpacity;

			return copy;
		}

		public BoxSettings Clone2()
		{
			return (BoxSettings) Clone();
		}
	}

	[DataContract(Namespace = "")]
	public class SheetRectData2<T> : ICloneable
	{
		private float[] fillColor;
		private float[] bdrColor;
		private float[] textColor;
		

		public SheetRectData2(SheetRectType type, T id)
		{
			Type = type;
			Id = id;

			TextSettings = new TextSettings();
			BoxSettings = new BoxSettings(null);

			Reset();
		}

		public SheetRectData2(SheetRectType type, T id, Rectangle rect)
		{
			Type = type;
			Id = id;

			TextSettings = new TextSettings();
			BoxSettings = new BoxSettings(rect);

			Reset();
		}

		public SheetRectData2(string name, T id, Rectangle rect)
		{
			Id = id;

			TextSettings = new TextSettings();
			BoxSettings = new BoxSettings(rect);

			Reset();

			Type = SheetRectSupport.GetShtRectType(name);

			if (Type == SheetRectType.SRT_NA)
			{
				Type = SheetRectSupport.GetOptRectType(name);
			}
		}

		[DataMember(Order =  0)]
		public SheetRectType Type { get; private set; }

		[DataMember(Order =  1)]
		public T Id { get; private set; }


		[IgnoreDataMember]
		public float TbOriginX { get; set; }

		[IgnoreDataMember]
		public float TbOriginY { get; set; }

		[IgnoreDataMember]
		public int SheetRotation { get; set; }

		[DataMember(Order = 31)]
		public TextSettings TextSettings { get; set; }

		[DataMember(Order = 32)]
		public BoxSettings BoxSettings { get; set; }

/*		removed to use objects instead
		[JsonIgnore]
		[IgnoreDataMember]
		public Color FillColor
		{
			get => Color.CreateColorWithColorSpace(fillColor);
			set => fillColor = value?.GetColorValue() ?? new [] { 0f, 1f, 1f };
		}

		[DataMember(Order =  31)]
		public float[] FillColorA
		{
			get => fillColor;
			set => fillColor = value;
		}

		[DataMember(Order =  32)]
		public float FillOpacity { get; set; }


		[DataMember(Order =  41)]
		public float BdrWidth { get; set; }

		[JsonIgnore]
		[IgnoreDataMember]
		public Color BdrColor
		{
			get => Color.CreateColorWithColorSpace(bdrColor);
			set => bdrColor = value?.GetColorValue() ?? new [] { 0f, 0f, 1f };
		}

		[DataMember(Order =  42)]
		public float[] BdrColorA
		{
			get => bdrColor;
			set => bdrColor = value;
		}


		[DataMember(Order =  43)]
		public float BdrOpacity { get; set; }

		[DataMember(Order =  44)]
		public float[] BdrDashPattern { get; set; }


		// text info ...
		[DataMember(Order =  51)]
		public string FontFamily { get; set; }

		[DataMember(Order =  52)]
		public int FontStyle { get; set; } = iText.IO.Font.Constants.FontStyles.NORMAL;

		[DataMember(Order =  53)]
		public float TextSize { get; set; }

		[DataMember(Order =  54)]
		public HorizontalAlignment TextHorizAlignment { get; set; }

		[DataMember(Order =  55)]
		public VerticalAlignment TextVertAlignment { get; set; }

		[DataMember(Order =  61)]
		public int TextWeight { get; set; } = iText.IO.Font.Constants.FontWeights.NORMAL;

		[JsonIgnore]
		[IgnoreDataMember]
		public Color TextColor
		{
			get => Color.CreateColorWithColorSpace(textColor);
			set => textColor = value?.GetColorValue() ?? new [] { 0f, 0f, 0f };
		}

		[DataMember(Order =  63)]
		public float[] TextColorA
		{
			get => textColor;
			set => textColor = value;
		}

		[DataMember(Order =  64)]
		public float TextOpacity { get; set; } = 1f;

		[DataMember(Order =  62)]
		public int TextDecoration { get; set; } = TextDecorations.NORMAL;
*/

		public void Reset()
		{
			// not reset:
			// Id, Type, Rect

			SheetRotation = 0;

			TextSettings.Reset();

			BoxSettings.Reset();

			/* moved to object FillColor = null;
			FillOpacity = 1f;

			BdrWidth = 1;
			BdrColor = null;
			BdrOpacity = 1f;
			BdrDashPattern = null;

			FontFamily = "Arial";
			FontStyle = iText.IO.Font.Constants.FontStyles.NORMAL; // 1 = bold // 2 = italic

			TextSize = 12f;

			TextHorizAlignment = HorizontalAlignment.LEFT;
			TextVertAlignment = VerticalAlignment.TOP; // must be top as this is a PDF default

			TextWeight = iText.IO.Font.Constants.FontWeights.NORMAL;
			TextDecoration = TextDecorations.NORMAL;
			TextColor = ColorConstants.BLACK;
			TextOpacity = 1f;*/
		}

		public float GetAdjTextRotation(float pageAdjust)
		{
			return FloatOps.ToRad(BoxSettings.TextBoxRotation + pageAdjust);
		}

		public bool HasType(SheetRectType test)
		{
			return (Type & test) != 0;
		}

		public object Clone()
		{
			SheetRectData2<T> copy = new SheetRectData2<T>(Type, Id, BoxSettings.Rect);
			
			copy.SheetRotation = SheetRotation;

			copy.TextSettings = TextSettings.Clone2();

			copy.BoxSettings = BoxSettings.Clone2();

			/* moved to object copy.FillColor = FillColor;
			copy.FillOpacity = FillOpacity;
			copy.BdrWidth = BdrWidth;
			copy.BdrColor = BdrColor;
			copy.BdrOpacity = BdrOpacity;
			copy.BdrDashPattern = (float[]) BdrDashPattern?.Clone() ?? null;
			copy.FontFamily = FontFamily;
			copy.FontStyle = FontStyle;
			copy.TextSize = TextSize;
			copy.TextHorizAlignment = TextHorizAlignment;
			copy.TextVertAlignment = TextVertAlignment;
			copy.TextWeight = TextWeight;
			copy.TextColor = TextColor;
			copy.TextOpacity = TextOpacity;
			copy.TextDecoration = TextDecoration;*/

			return copy;
		}

		public SheetRectData2<T> Clone2()
		{
			return (SheetRectData2<T>) Clone();
		}
	}
}