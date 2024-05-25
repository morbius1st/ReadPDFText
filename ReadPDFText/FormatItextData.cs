#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using iText.Kernel.Geom;

#endregion

// user name: jeffs
// created:   2/18/2024 7:15:19 AM
//
// namespace SharedCode.ShDataSupport
// {
// 	public class FormatItextData
// 	{
// 		public static string FormatRectangle(Rectangle r, int numWidth, string numFmt, int descWidth, float cvtFactor = 1.0f)
// 		{
// 			return FormatQuadFloat(
// 				new [] { r.GetX()*cvtFactor, r.GetY()*cvtFactor, r.GetWidth()*cvtFactor, r.GetHeight()*cvtFactor },
// 				numWidth, numFmt,
// 				new [] { "x", "y", "w", "h" },
// 				descWidth);
//
//
// 			// return $"X| {r.GetX(),9:F2}| Y| {r.GetY(),9:F2}| W| {r.GetWidth(),9:F2}| H| {r.GetHeight(),9:F2}";
// 		}
//
//
// 		public static string FormatQuadFloat(float[] quad, int quadWidth, string quadNumFmt, string[] desc, int descWidth)
// 		{
// 			StringBuilder sb = new StringBuilder();
//
// 			string fmt1 = $"{{0,{descWidth}}}| {{1,{quadWidth}:{quadNumFmt}}}| ";
//
// 			for (int i = 0; i < 4; i++)
// 			{
// 				sb.Append(String.Format(fmt1, desc[i], quad[i]));
// 			}
//
// 			return sb.ToString();
// 		}
//
// 		public override string ToString()
// 		{
// 			return $"this is {nameof(FormatItextData)}";
// 		}
// 	}
// }
