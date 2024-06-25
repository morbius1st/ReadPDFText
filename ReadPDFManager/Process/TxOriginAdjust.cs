#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ShCode;
using ShItextCode.PdfCalculations;
using ShSheetData.SheetData;

#endregion

// user name: jeffs
// created:   6/19/2024 7:27:00 PM

namespace ReadPDFManager.Process
{
	public class TxOriginAdjust
	{
		private List<SampleBox> SampleBoxes;

		private SheetRectData<SheetRectId> srd;

		private string rectname;
		private float priorRotation;

		private string[] ha = new [] { "L", "C", "R" };
		private string[] va = new [] { "T", "M", "B" };

		public TxOriginAdjust()
		{
			TbOriginAdjust tbo = new TbOriginAdjust();

			SampleBoxes = tbo.SampleBoxes;
		}

		public void Process()
		{
			ShowInfo.StartMsg($"origin adjustments - {(TbOriginAdjust.which == 1 ? "new" : "orig")}", DateTime.Now.ToString());

			TbOriginAdjustSamples.CreateSamples(SampleBoxes);

			Dictionary<string, SheetRectData<SheetRectId>> srdSamples =
				TbOriginAdjustSamples.SrdSamples;


			foreach (SampleBox sb in SampleBoxes)
			{
				rectname = sb.Name;

				srd = srdSamples[sb.NameEx];

				if (sb.ShtRotation != priorRotation)
				{
					priorRotation = sb.ShtRotation;
					Debug.WriteLine($"\n\nsheet rotation == {sb.ShtRotation:F2}\n");
				}

				Debug.Write($"{sb.NameEx,-14} {$"({rectname})",-14}");
				Debug.Write($" | w {srd.Rect.GetWidth(),8:F2} h {srd.Rect.GetHeight(),8:F2}");
				Debug.WriteLine($"\talign| {ha[(int) srd.TextHorizAlignment]}-{va[(int) srd.TextVertAlignment]} ({srd.TextHorizAlignment})-({srd.TextVertAlignment})");


				Tuple<float, float> tbOrigin = PdfCalcs2.GetTextBoxOrigin(srd, sb.ShtRotation);

				srd.TbOriginX = tbOrigin.Item1;
				srd.TbOriginY = tbOrigin.Item2;

				// compareResults(sb, tbOrigin);
			}
		}

		private void calcTxOrigins(SampleBox sb)
		{
			float txOrigX;
			float txOrigY;

			PdfCalcTxOrigin2.GetTextOrigin(srd, sb.ShtRotation, out txOrigX, out txOrigY);

			showResults(srd.TbOriginX, srd.TbOriginY, txOrigX, txOrigY);
		}

		private void showResults(float x, float y, float txx, float txy)
		{
			Debug.WriteLine($"tb origin | {x:,8:F2} {y:,8:F2} | tx origin | {txx,8:F2} {txy,8:F2}");
		}


		private void compareResults(SampleBox sb, Tuple<float, float> tbOrigin)
		{
			bool resultx = sb.CorrectX.Equals(tbOrigin.Item1);
			bool resulty = sb.CorrectY.Equals(tbOrigin.Item2);

			string xmatch = resultx ? "✓" : "✗";
			string ymatch = resulty ? "✓" : "✗";
			string results =  resultx && resulty ? "✓" : "✗";

			Debug.WriteLine($"{results}\tcorrect {sb.CorrectX,8:f2} {sb.CorrectY,8:F2} | calc'd {tbOrigin.Item1,8:F2} {xmatch} {tbOrigin.Item2,8:F2} {ymatch}\n");
		}
	}
}