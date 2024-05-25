// Solution:     ReadPDFText
// Project:       ReadPDFText
// File:             annoInfo.cs
// Created:      2024-04-27 (4:25 PM)

using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout.Properties;
using UtilityLibrary;

namespace ReadPDFText.Process
{
	public struct annoInfo
	{
		public Rectangle rect { get; set; }
		public DeviceRgb rgbFill { get; set; }
		public DeviceRgb rgbBorder { get; set; }
		public string text { get; set; }

		public float textDx { get; set; }
		public float textDy { get; set; }

		public float textRotation { get; set; }

		public TextAlignment ta { get; set; }
		
		public annoInfo(Rectangle rect,
			DeviceRgb rgbFill,
			DeviceRgb rgbBorder,
			string text,
			float tr = 0,
			float tx = 0,
			float ty = 0,
			TextAlignment ta = TextAlignment.RIGHT)
		{
			textDx = tx;
			textDy = ty;
			textRotation = FloatOps.ToRad(tr);

			this.ta = ta;

			this.rect = rect;
			this.rgbFill = rgbFill;
			this.rgbBorder = rgbBorder;
			this.text = text;

			
		}
	}
}