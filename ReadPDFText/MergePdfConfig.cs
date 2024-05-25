#region using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;

#endregion

// username: jeffs
// created:  2/21/2024 7:25:58 PM

namespace SharedCode.ShDataSupport
{
	/*
	 *    + <---_| applies to document (store in config properties)
	 *    | + <--| applies to specific sheets (store in sheet list)
	 *    | |
	 *    v v data needed
	 * √    * > sheet data with sheet rectangles
	 * √  *   > page number or sheet number that is the TOC page
	 *        > hyperlinks
	 * √    *   > ignore sheets
	 *    * *   > general - include / do not include
	 *    *   > banner - text
	 * √  *     > general - include / do not include banner
	 * √    *   > ignore sheets
	 * √  *     > banner font information / font name / size / color / weight
	 * √  *     > banner location / orientation
	 * √  *   > footer - extra text
	 *    *   > author
	 * √  *     > url
	 * √  *     > general - provide
	 * √    *   > ignore sheets
	 *    *   > disclaimer
	 * √  *     > url
	 * √  *     > general - provide
	 * √    *   > ignore sheets
	 *    *   > document meta data
	 * √  *     > project name -> title
	 * √  *     > document type -> subject
	 * √  *     > user's name -> author
	 *    *   > hyperlink rectangle:
	 * √  *     > line: color, width, dash pattern
	 * √  *     > fill: color, opacity
	 *    * *   > general - include / do not include
	 *   
	 *   
	 *   
	 *   
	 *    notes
	 *    > ignore sheets: sheet numbers to not process the referenced item
	 *    > this will have the list of files / sheets to merge?
	 *    > needs master sheet list cross-referenced between the list
	 *   		of planned sheets and the list of actual sheets
	 *   
	 *   
	 *    fixed data
	 * √    > app description -> creator (fixed)
	 *
	 *
	 */



	public class MergePdfConfig
	{
		// meta fields
		public string          UserName { get; private set; }
		public string          ProjectNumber => "2024-xxxx";
		public string          ProjectName => "Project Name";

		public string          SetDescription => "Progress Set";

		public string          AppName => "Josh";
		public string          AppDeveloper => "CyberStudio";

		public string          SetProducer => $"{AppName} by {AppDeveloper}";

		public string          SetAuthor => "Project Manager";

		// misc fields
		// private             float pi180 = (float) Math.PI;
		// private             float pi90;
		// private             float pi270;

		// color
		public Color           HlinkBdr => new DeviceRgb(0, 192, 255);
		public Color           HlinkFil => new DeviceRgb(75, 214, 254);
		public Color           BannerontColor => new DeviceRgb(255, 0, 0);
		

		// transforms
		public AffineTransform BannerTrans => new AffineTransform();

		// style

		// links
		public bool            LinksInclude { get; set; } = true;
		public DeviceRgb       LinkFillColor { get; set; }
		public DeviceRgb       LinkStrokeColor { get; set; }
		public PdfDashPattern  LinkDashPattern { get; set; }
		public float           LinkLineWidth { get; set; }
		public float           LinkFillOpacity { get; set; }
		public float           LinkRectangleMargin { get; set; }

		// author
		public bool            AuthorLinkInclude { get; set; } = true;
		public string          AuthorLinkUrl { get; set; } = "https://www.aoarchitects.com/";

		// banners

		public bool            BannerVerticalInclude { get; set; } = true;
		public BannerPosition  BannerVerticalPosition { get; set; }
		public float           BannerVerticalFontHeight { get; set; } = 18f;
		public string          BannerVertical { get; set; }
		public Style           BannerVerticalPgStyle { get; private set; }
		public string          BannerVerticalFontFile => @"c:\windows\fonts\arial.ttf";
		public PdfFont         BannerVerticalFont { get; set; }
		public Paragraph       BannerVerticalPg { get; set; }

		public bool            BannerHorizontalInclude { get; set; } = true;
		public BannerPosition  BannerHorizontalPosition { get; set; }
		public float           BannerHorizontalFontHeight { get; set; } = 18f;
		public string          BannerHorizontal { get; set; }
		public Style           BannerHorizontalPgStyle { get; private set; }
		public string          BannerHorizontalFontFile => @"c:\windows\fonts\arial.ttf";
		public PdfFont         BannerHorizontalFont { get; set; }
		public Paragraph       BannerHorizontalPg { get; set; }

		// footer
		public float           FooterFontHeight { get; set; } = 3f;
		// public              float FooterFontHeight { get; set; } = 20f;
		public string          FooterUser { get; set; } = "This is a custom Footer";
		public string          FooterFormat => "This document assembled on {0} at {1} by {2} | {3}";
		public Style           FooterPgStyle { get; private set; }
		public string          FooterFontFile => @"c:\windows\fonts\arial.ttf";
		public PdfFont         FooterFont { get; set; }
		public Paragraph       FooterPg { get; set; }

		// disclaimer
		public bool            DisclaimerInclude { get; set; } = true;
		public float           DisclaimerFontHeight { get; set; } = 8f;
		public string          Disclaimer { get; private set; } = "DISCLAIMER: Click to view disclaimers that apply to this document and associated documents.";
		public string          DisclaimerUrl { get; set; } = "https://www.aoarchitects.com/legal-disclaimer/";
		public Style           DisclaimerPgStyle { get; private set; }
		public string          DisclaimerFontFile => @"c:\windows\fonts\arial.ttf";
		public PdfFont         DisclaimerFont { get; set; }
		public Paragraph       DisclaimerPg { get; set; }


		// settings

		public bool            ExtractAndValidateSheetNames = false;


		public bool            TocInclude { get; set; } = true;
		public int             TocPage { get; set; } = 1;
		public string          TocSheet { get; set; } = "T1.0-0";

		public string          DefaultFontFile => @"c:\windows\fonts\arialn.ttf";
		public PdfFont         DefaultFont { get; set; }

		public bool            CorrectSheetRotation { get; set; } = false;
		public bool            NormalPgOrientationIsVert { get; set; } = true;

		public bool			   IgnoreRotationFails { get; set; } = true;
		public bool			   IgnoreSheetNumberValidationFails { get; set; } = true;

		public DeviceRgb       TopLevelBookmarkColor { get; set; }
		public int             TopLevelBookmarkTextStyle { get; set; }

		public bool            IncludeMargins { get; private set; } = false;
		public float           marginM1 { get; private set; } = 0;
		public float           marginM2 { get; private set; } = 0;
		public float           marginL1 { get; private set; }
		public float           marginA2 { get; private set; }


		
		// test settings

		public bool            TestMakeRects { get; set; } = false;
		public float           TestRectFillOpacity { get; set; }
		public DeviceRgb       TestRectBorder { get; set; }
		public DeviceRgb       TestRecFillHlink { get; set; }
		public DeviceRgb       TestRecFillFind { get; set; }
		public DeviceRgb       TestRecFillReturn { get; set; }
		public DeviceRgb       TestRecFillBanner { get; set; }
		public DeviceRgb       TestRecFillAuthor { get; set; }
		public DeviceRgb       TestRecFillDisclaimer { get; set; }
		public DeviceRgb       TestRecFillFooter { get; set; }
		
		public MergePdfConfig()
		{
			config();
		}

		private void config()
		{
			UserName = UserPrincipal.Current.DisplayName ?? "No Name Found";

			SetValues();
			CreateStyles();

			BannerVerticalInclude = true;
			BannerVerticalPosition = BannerPosition.BP_TOP;
			BannerVertical = "this is a vertical banner";

			BannerHorizontalInclude = true;
			BannerHorizontalPosition = BannerPosition.BP_BOTTOM;
			BannerHorizontal = "this is a horizontal banner";

			marginL1 = (float) Math.Sqrt(marginM2 * marginM2 + marginM1 * marginM1);
			marginA2 = (float) Math.Atan(marginM2 / marginM1);

		}

		private void SetValues()
		{
			TopLevelBookmarkColor = new DeviceRgb(255, 117, 0);
			TopLevelBookmarkTextStyle = PdfOutline.FLAG_BOLD;

			LinkFillColor = new DeviceRgb(75, 214, 255);
			LinkStrokeColor = new DeviceRgb(0, 192, 255);
			LinkDashPattern = new PdfDashPattern(6, 3);
			LinkLineWidth = 1;
			LinkFillOpacity = 0.4f;

			LinkRectangleMargin = 4.0f;

			TestRectFillOpacity   = 0.3f;
			TestRectBorder        = new DeviceRgb(0, 0, 0);        // black
			TestRecFillHlink      = new DeviceRgb(255, 157, 255);  // pale magenta
			TestRecFillFind       = new DeviceRgb(0, 247, 232);    // blue-green
			TestRecFillReturn     = new DeviceRgb(255, 157, 157);  // pale red
			TestRecFillBanner     = new DeviceRgb(157, 195, 255);  // pale blue
			TestRecFillAuthor     = new DeviceRgb(206, 206, 0);  // dk yellow
			TestRecFillDisclaimer = new DeviceRgb(255, 195, 157);  // pale orange
			TestRecFillFooter     = new DeviceRgb(195, 176, 235);  // pale purple  
			
		}

		private void CreateStyles()
		{
			DefaultFont = PdfFontFactory.CreateFont(DefaultFontFile);
			FooterFont = PdfFontFactory.CreateFont(FooterFontFile);
			DisclaimerFont = PdfFontFactory.CreateFont(DisclaimerFontFile);
			BannerHorizontalFont = PdfFontFactory.CreateFont(BannerHorizontalFontFile);
			BannerVerticalFont = PdfFontFactory.CreateFont(BannerVerticalFontFile);

			FooterPgStyle = new Style()
			// .SetMargin(0)
			.SetRotationAngle(0)
			.SetFont(FooterFont)
			.SetFontColor(DeviceRgb.BLACK)
			.SetFontSize(FooterFontHeight);

			BannerHorizontalPgStyle = new Style()
			// .SetRotationAngle(0)
			.SetMargin(0f)
			.SetFont(BannerHorizontalFont)
			.SetFontColor(DeviceRgb.BLACK)
			.SetFontSize(BannerHorizontalFontHeight);

			BannerVerticalPgStyle = new Style()
			// .SetRotationAngle(pi90)
			.SetMargin(0f)
			.SetFont(BannerVerticalFont)
			.SetFontColor(DeviceRgb.BLACK)
			.SetFontSize(BannerVerticalFontHeight);

			DisclaimerPgStyle = new Style()
			// .SetRotationAngle(0)
			.SetMargin(0f)
			.SetFont(DisclaimerFont)
			.SetFontColor(DeviceRgb.BLACK)
			.SetFontSize(DisclaimerFontHeight);

		}

		public void SetMargins(float m1, float m2)
		{
			marginM1 = m1;
			marginM2 = m2;

			if (m1 <= 0 || m2 <= 0)
			{
				IncludeMargins = false;
				return;
			}

			marginA2 = (float) Math.Atan(m2 / m1);
			marginL1 = (float) Math.Sqrt(m1 * m1 + m2 * m2);

			IncludeMargins = true;
		}

		#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(MergePdfConfig)}";
		}

		#endregion
	}
}
