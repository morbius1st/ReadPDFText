#region + Using Directives
using System;
using iText.Layout.Properties;

#endregion

// user name: jeffs
// created:   3/9/2024 11:32:02 PM

namespace SharedCode
{
	public static class Constants
	{
		public const string XL_FILE_EXTN = ".xlsx";
		public const string PDF_FILE_EXTN = ".pdf";

		public const string BOX_SUBJECT = "Josh Box";

		public static float PI90 { get; private set; }
		public static float PI180 { get; private set; } = (float) Math.PI;
		public static float PI270 { get; private set; }
		public static float PI360 { get; private set; }

		static Constants()
		{
			PI90 = PI180 / 2;
			PI270 = PI90 * 3;
			PI360 = PI180 * 2;
		}

		public static double RadToDegrees(double deg)
		{
			return deg / Math.PI * 180;

		}

		// public static Tuple<string, TextAlignment>[] TextHorzAlignment = new []
		// {
		// 	new Tuple<string, TextAlignment>("justified", TextAlignment.JUSTIFIED),
		// 	new Tuple<string, TextAlignment>("left", TextAlignment.LEFT),
		// 	new Tuple<string, TextAlignment>("right", TextAlignment.RIGHT),
		// 	new Tuple<string, TextAlignment>("center", TextAlignment.CENTER),
		// };


		public static Tuple<string, HorizontalAlignment, char, float>[] TextHorzAlignment = new []
		{
			new Tuple<string, HorizontalAlignment, char, float>("Left"  , HorizontalAlignment.LEFT  , 'L', 0.0f),
			new Tuple<string, HorizontalAlignment, char, float>("Center", HorizontalAlignment.CENTER, 'C', 0.5f),
			new Tuple<string, HorizontalAlignment, char, float>("Right" , HorizontalAlignment.RIGHT , 'R', 1.0f),
		};

		public static Tuple<string, VerticalAlignment, char, float>[] TextVertAlignment = new []
		{
			new Tuple<string, VerticalAlignment, char, float>("Top"    , VerticalAlignment.TOP   , 'T', 1.0f),
			new Tuple<string, VerticalAlignment, char, float>("Middle" , VerticalAlignment.MIDDLE, 'M', 0.5f),
			new Tuple<string, VerticalAlignment, char, float>("Bottom" , VerticalAlignment.BOTTOM, 'B', 0.0f),
		};
	}
}
