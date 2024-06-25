// Solution:     ReadPDFText
// Project:       ReadPDFTextTests
// File:             ShtRectData.cs
// Created:      2024-05-16 (8:07 PM)

// using SharedCode.ShDataSupport;
// using SharedCode.ShPdfSupport;

using System;
using System.Runtime.Serialization;
using System.Text;

using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout.Properties;
using Newtonsoft.Json;
using UtilityLibrary;

namespace ShSheetData.SheetData
{



	[DataContract(Namespace = "")]
	public class SheetRectData<T> : ICloneable
	{
		private float[] fillColor;
		private float[] bdrColor;
		private float[] textColor;
		private float[] rectangleA;

		public SheetRectData(SheetRectType type, T id)
		{
			Type = type;
			Id = id;
			Rect = null;

			Reset();
		}

		public SheetRectData(SheetRectType type, T id, Rectangle rect)
		{
			Type = type;
			Id = id;
			Rect = rect;

			Reset();
		}

		public SheetRectData(string name, T id, Rectangle rect)
		{
			Id = id;
			Rect = rect;

			Type = SheetRectSupport.GetShtRectType(name);

			if (Type == SheetRectType.SRT_NA)
			{
				Type = SheetRectSupport.GetOptRectType(name);
			}

			Reset();
		}

		[DataMember(Order =  0)]
		public SheetRectType Type { get; private set; }

		[DataMember(Order =  1)]
		public T Id { get; private set; }

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

		[IgnoreDataMember]
		public float TbOriginX {get; set; }

		[IgnoreDataMember]
		public float TbOriginY {get; set; }

		[DataMember(Order =  11)]
		public string InfoText { get; set; }

		[DataMember(Order =  12)]
		public string UrlLink { get; set; }

		[IgnoreDataMember]
		public int SheetRotation { get; set; }

		[DataMember(Order =  21)]
		public float TextBoxRotation { get; set; }


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


		public void Reset()
		{
			// not reset:
			// Id, Type, Rect

			InfoText = null;
			UrlLink = null;

			SheetRotation = 0;
			TextBoxRotation = 0;

			FillColor = null;
			FillOpacity = 1f;

			BdrWidth = 1;
			BdrColor = null;
			BdrOpacity = 1f;
			BdrDashPattern = null;

			FontFamily = "Arial";
			FontStyle = iText.IO.Font.Constants.FontStyles.NORMAL;  // 1 = bold // 2 = italic

			TextSize = 12f;

			TextHorizAlignment = HorizontalAlignment.LEFT;
			TextVertAlignment = VerticalAlignment.TOP; // must be top as this is a PDF default

			TextWeight = iText.IO.Font.Constants.FontWeights.NORMAL;
			TextDecoration = TextDecorations.NORMAL;
			TextColor = ColorConstants.BLACK;
			TextOpacity = 1f;
		}

		public float GetAdjTextRotation(float pageAdjust)
		{
			return FloatOps.ToRad(TextBoxRotation + pageAdjust);
		}

		public bool HasType(SheetRectType test)
		{
			return (Type & test) != 0;
		}

		public object Clone()
		{
			SheetRectData<T> copy = new SheetRectData<T>(Type, Id, Rect);

			copy.InfoText = InfoText;
			copy.UrlLink = UrlLink;
			copy.SheetRotation = SheetRotation;
			copy.TextBoxRotation = TextBoxRotation;
			copy.FillColor = FillColor;
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
			copy.TextDecoration = TextDecoration;

			return copy;
		}

		public SheetRectData<T> Clone2()
		{
			return (SheetRectData<T>) Clone();
		}
	}
}