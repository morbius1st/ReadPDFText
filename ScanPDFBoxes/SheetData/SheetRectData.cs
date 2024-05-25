// Solution:     ReadPDFText
// Project:       ReadPDFTextTests
// File:             ShtRectData.cs
// Created:      2024-05-16 (8:07 PM)

using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout.Properties;
// using SharedCode.ShDataSupport;
// using SharedCode.ShPdfSupport;
using ColorA = System.Drawing.Color;
using Path = iText.Kernel.Geom.Path;

namespace ShCommonCode.ShSheetData
{
	public class TextDecorations
	{
		public static int NORMAL { get; } = 0;
		public static int UNDERLINE { get; } = 1 << 1;
		public static int LINETHROUGH { get; } = 1 << 2;

		public static bool HasUnderline(int decoration)
		{
			return (decoration & UNDERLINE) > 0;
		}

		public static bool HasLinethrough(int decoration)
		{
			return (decoration & LINETHROUGH) > 0;
		}

		public static string FormatTextDeco(int deco)
		{
			if (deco == NORMAL) { return nameof(NORMAL); }

			StringBuilder result = new StringBuilder();

			if (HasUnderline(deco)) result.Append(nameof(UNDERLINE));
			if (HasLinethrough(deco)) result.Append(nameof(LINETHROUGH));

			return result.ToString();
		}
	}

	[DataContract(Namespace = "")]
	public class SheetRectData<T>
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



		[DataMember(Order =  11)]
		public string InfoText { get; set; }

		[DataMember(Order =  12)]
		public string UrlLink { get; set; }



		[DataMember(Order =  21)]
		public float Rotation { get; set; }



		[JsonIgnore]
		[IgnoreDataMember]
		public Color? FillColor
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
		public Color? BdrColor
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
		public TextAlignment TextHorizAlignment { get; set; }

		[DataMember(Order =  55)]
		public VerticalAlignment TextVertAlignment { get; set; }

		[DataMember(Order =  61)]
		public int TextWeight { get; set; } = iText.IO.Font.Constants.FontWeights.NORMAL;
		
		[JsonIgnore]
		[IgnoreDataMember]
		public Color? TextColor
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

			Rotation = 0;

			FillColor = null;
			FillOpacity = 1f;

			BdrWidth = 1;
			BdrColor = null;
			BdrOpacity = 1f;
			BdrDashPattern = null;

			FontFamily = "Arial";
			FontStyle = iText.IO.Font.Constants.FontStyles.NORMAL;

			TextSize = 12f;

			TextHorizAlignment = TextAlignment.LEFT;
			TextVertAlignment = VerticalAlignment.MIDDLE;

			TextWeight = iText.IO.Font.Constants.FontWeights.NORMAL;
			TextDecoration = TextDecorations.NORMAL;
			TextColor = ColorConstants.BLACK;
			TextOpacity = 1f;
		}

		public bool HasType(SheetRectType test)
		{
			return (Type & test) != 0;
		}
	}
}