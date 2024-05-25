#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iText.Kernel.Colors;
using iText.Kernel.Geom;

#endregion

// user name: jeffs
// created:   2/18/2024 7:15:19 AM

namespace SharedCode.ShDataSupport
{
	// public class FormatItextData
	// {
	// 	public static string FormatDashArray(float[] dashes)
	// 	{
	// 		float cvtFactor = 1;
	// 		int numWidth = 5;
	// 		string numFmt = "F2";
	// 		int descWidth = 2;
	//
	// 		return FormatFloatArray(
	// 			dashes,
	// 			numWidth, numFmt,
	// 			new [] { "d", "s"},
	// 			descWidth);
	// 	}
	//
	// 	public static string FormatColor(Color c)
	// 	{
	// 		float cvtFactor = 255;
	// 		int numWidth = 4;
	// 		string numFmt = "F0";
	// 		int descWidth = 2;
	//
	// 		string[] colorTags = null;
	//
	// 		if (c.GetNumberOfComponents() == 3)
	// 		{
	// 			colorTags = new [] { "R", "G", "B" };
	// 		}
	// 		else if (c.GetNumberOfComponents() == 4)
	// 		{
	// 			colorTags = new [] { "C", "M", "Y", "K"};
	// 		}
	// 		else
	// 		{
	// 			colorTags = new [] { "G"};
	// 		}
	//
	// 		return FormatFloatArray(
	// 			ArrayMultiply(c.GetColorValue(),cvtFactor),
	// 			numWidth, numFmt,
	// 			colorTags,
	// 			descWidth);
	// 	}
	//
	// 	public static string FormatRectangle(Rectangle r)
	// 	{
	// 		float cvtFactor = 1;
	// 		int numWidth = 9;
	// 		string numFmt = "F2";
	// 		int descWidth = 2;
	//
	// 		return FormatFloatArray(
	// 			new [] { r?.GetX()*cvtFactor ?? -1, r?.GetY()*cvtFactor ?? -1, r?.GetWidth()*cvtFactor  ?? -1, r?.GetHeight()*cvtFactor  ?? -1,},
	// 			numWidth, numFmt,
	// 			new [] { "x", "y", "w", "h" },
	// 			descWidth);
	// 	}
	//
	// 	public static string FormatRectangle(Rectangle r, int numWidth, string numFmt, int descWidth, float cvtFactor = 1.0f)
	// 	{
	// 		return FormatFloatArray(
	// 			new [] { r?.GetX()*cvtFactor ?? -1, r?.GetY()*cvtFactor ?? -1, r?.GetWidth()*cvtFactor  ?? -1, r?.GetHeight()*cvtFactor  ?? -1,},
	// 			numWidth, numFmt,
	// 			new [] { "x", "y", "w", "h" },
	// 			descWidth);
	//
	//
	// 		// return $"X| {r.GetX(),9:F2}| Y| {r.GetY(),9:F2}| W| {r.GetWidth(),9:F2}| H| {r.GetHeight(),9:F2}";
	// 	}
	//
	// 	public static string FormatFloatArray(float[] array, int quadWidth, string quadNumFmt, string[] desc, int descWidth)
	// 	{
	// 		if (array == null || array.Length == 0) return "empty";
	//
	// 		int j = 0;
	//
	// 		StringBuilder sb = new StringBuilder();
	//
	// 		string fmt1 = $"{{0,{descWidth}}}| {{1,{quadWidth}:{quadNumFmt}}}| ";
	//
	// 		for (int i = 0; i < array.Length; i++)
	// 		{
	// 			j = j + 1 == desc.Length ? 0 : j++;
	//
	// 			sb.Append(String.Format(fmt1, desc[i], array[i]));
	// 		}
	//
	// 		return sb.ToString();
	// 	}
	// 	
	// }
}
