#region + Using Directives

using System.Collections.Generic;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using UtilityLibrary;
using static SharedCode.ShDataSupport.ExcelSupport.AssemblerSettingFormat;

#endregion

// user name: jeffs
// created:   3/31/2024 3:47:53 PM

namespace SharedCode.ShDataSupport.ExcelSupport
{
	public class PdfAssemblerSettings
	{
		public override string ToString()
		{
			return $"this is {nameof(PdfAssemblerSettings)}";
		}
	}

	public enum AssemblerSettingFormat
	{
		ASF_TEXT,
		ASF_RGB,
		ASF_LINK,
		ASF_BOOL,
		ASF_INT,
		ASF_FLOAT,
		ASF_THICKNESS,
		ASF_DASH,
		ASF_TEXT_STYLE,
		ASF_BANNER_LOCATIONV,
		ASF_BANNER_LOCATIONH,
		ASF_OPACITY,
	}

	public class AssemblerSettingCollection
	{
		// public AssemblerSettingTypeId TypeId { get; set; }
		public string TypeIdTag { get; set; }

		public Dictionary<string, AssemblerFieldSetting> SettingsValues { get; set; }

		public AssemblerSettingCollection(
			// AssemblerSettingTypeId typeId,
			string typeIdTag,
			params AssemblerFieldSetting[]  assembSetg)
		{
			// TypeId = typeId;
			TypeIdTag = typeIdTag;
			SettingsValues = new Dictionary<string, AssemblerFieldSetting>();

			foreach (AssemblerFieldSetting asg in assembSetg)
			{
				SettingsValues.Add(asg.FieldId, asg);
			}
		}
	}

	public class AssemblerFieldSetting
	{
		private DeviceRgb WHITE = new DeviceRgb(255, 255, 255);

		private List<string> BANNER_LOCATIONV_OPTIONS = new List<string> { "", "b", "t" };
		private List<string> BANNER_LOCATIONH_OPTIONS = new List<string> { "", "b", "t" };
		private List<string> TEXT_STYLE_OPTIONS = new List<string> { "r", "b", "i", "bi" };

		private int N0, N1, N2;
		private float F0, F1;
		private bool B0;

		public AssemblerFieldSetting(string tag, AssemblerSettingFormat format)
		{
			FieldId = tag;
			FieldFormat = format;
		}

		public string FieldId { get; set; }
		public AssemblerSettingFormat FieldFormat { get; set; }
		public string Value { get; private set; }

		public void SetValue(string value)
		{
			Value = value;

			switch (FieldFormat)
			{
			case ASF_BANNER_LOCATIONV:
				{
					Value = validateOptionString(value.Trim().ToLower(), BANNER_LOCATIONV_OPTIONS);
					BannerPosition = AsBannerPosition;
					break;
				}

			case ASF_BANNER_LOCATIONH:
				{
					Value = validateOptionString(value.Trim().ToLower(), BANNER_LOCATIONH_OPTIONS);
					BannerPosition = AsBannerPosition;
					break;
				}

			case ASF_TEXT_STYLE:
				{
					Value = validateOptionString(value.Trim().ToLower(), TEXT_STYLE_OPTIONS);
					TextStyle = AsTextStyle;
					break;
				}

			case ASF_OPACITY:
				{
					Opacity = AsOpacity;
					break;
				}

			case ASF_THICKNESS:
				{
					Thickness = AsThickness;
					break;
				}

			case ASF_FLOAT:
				{
					Float = AsFloat;
					break;
				}

			case ASF_INT:
				{
					Int = AsInt;
					break;
				}


			case ASF_BOOL:
				{
					Bool = AsBool;
					break;
				}

			case ASF_LINK:
				{
					Link = AsLink;
					break;
				}
					
			case ASF_RGB:
				{
					DeviceRgb = AsDeviceRgb;
					break;
				}
			
			case ASF_TEXT:
				{
					Text = AsText;
					break;
				}
			
			case ASF_DASH:
				{
					DashPattern = AsDashPattern;
					break;
				}


			}
		}

		public PdfDashPattern DashPattern { get; set; }
		public string Text { get; set; }
		public DeviceRgb DeviceRgb { get; set; }
		public string Link { get; set; }
		public float Float { get; set; }
		public bool Bool { get; set; }
		public int Int { get; set; }
		public float Thickness { get; set; }
		public float Opacity { get; set; }
		public int TextStyle { get; set; }
		public BannerPosition BannerPosition { get; set; }
		// public BannerOrientation BannerOrientation { get; set; }

		private PdfDashPattern AsDashPattern
		{
			get
			{
				PdfDashPattern dp = new PdfDashPattern(6, 3);

				if (FieldFormat != AssemblerSettingFormat.ASF_DASH) return dp;

				string[] dashes = Value.Split(',');
				if (dashes.Length != 2) return dp;

				if (!float.TryParse(dashes[0], out F0)) return dp;
				if (!float.TryParse(dashes[1], out F1)) return dp;

				return new PdfDashPattern(F0, F1);
			}
		}
		private string AsText
		{
			get
			{
				if (FieldFormat != AssemblerSettingFormat.ASF_TEXT) return null;
				return Value;
			}
		}
		private DeviceRgb AsDeviceRgb
		{
			get
			{
				if (FieldFormat != ASF_RGB) return WHITE;

				return getRgb();
			}
		}
		private string AsLink
		{
			get
			{
				if (FieldFormat != AssemblerSettingFormat.ASF_LINK) return null;
				return Value;
			}
		}
		private float AsFloat
		{
			get
			{
				if (FieldFormat != AssemblerSettingFormat.ASF_FLOAT || !float.TryParse(Value, out F0)) return 0;

				return F0;
			}
		}
		private float AsThickness
		{
			get
			{
				if (FieldFormat != ASF_THICKNESS || !float.TryParse(Value, out F0)) return 3;

				return F0;
			}
		}
		private int AsTextStyle
		{
			get
			{
				if (FieldFormat != AssemblerSettingFormat.ASF_TEXT_STYLE) return 0;

				switch (Value)
				{
				case "r":
					return 0;

				case "b":
					return PdfOutline.FLAG_BOLD;

				case "i":
					return PdfOutline.FLAG_ITALIC;

				case "ib":
				case "bi":
					return PdfOutline.FLAG_BOLD & PdfOutline.FLAG_ITALIC;
				}

				return 0;
			}
		}
		private float AsOpacity
		{
			get
			{
				if (FieldFormat != AssemblerSettingFormat.ASF_OPACITY) return  0.30f;
				if (!float.TryParse(Value, out F0)) return 0.30f;

				if (F0 < 10)
				{
					F0 = 10;
				}
				else
				if (F0>70)
				{
					F0 = 70;
				}

				return F0 / 100;
			}

		}
		private BannerPosition AsBannerPosition
		{
			get
			{
				if (FieldFormat != ASF_BANNER_LOCATIONV && FieldFormat != ASF_BANNER_LOCATIONH)
					return  BannerPosition.BP_BOTTOM;

				if (Value.IsVoid() || Value[0] == 'b' ||  Value[0] == 'B') return BannerPosition.BP_BOTTOM;

				return BannerPosition.BP_TOP;
			}
		}
		private bool AsBool
		{
			get
			{
				if (FieldFormat != ASF_BOOL || !bool.TryParse(Value, out B0)) return false;

				return B0;
			}
		}
		private int AsInt
		{
			get
			{
				if (FieldFormat != ASF_INT || !int.TryParse(Value, out N0)) return -1;

				return N0;
			}
		}


		private string validateOptionString (string value, List<string> options)
		{
			return options.Contains(value) ? value: options[0];
		}

		private DeviceRgb getRgb()
		{
			string[] rgb = Value.Split(',');
			if (rgb.Length != 3) return WHITE;

			if (!int.TryParse(rgb[0], out N0)) return WHITE;
			if (!int.TryParse(rgb[1], out N1)) return WHITE;
			if (!int.TryParse(rgb[2], out N2)) return WHITE;

			// DeviceRgb drgb1 =new DeviceRgb(N0, N1, N2);
			// DeviceRgb drgb2 =new DeviceRgb(255,117,0);

			return new DeviceRgb(N0, N1, N2);
		}
	}

	public class PdfAssemblerSettingsData
	{
		public const string PDF_ASSEMB_FILE_NAME = "PdfAssemblerSettings.xlsx";

		public Dictionary<string, AssemblerSettingCollection> AssemblerSettingCollection { get; set; }

		public PdfAssemblerSettingsData()
		{
			config();
		}

		private void config()
		{
			AssemblerSettingCollection = new Dictionary<string, AssemblerSettingCollection>();

			addToSetgColl(makeAssembCollect("[general settings]",
				MkFldStg( "[fix rotation]", ASF_BOOL)
				));


			// bookmark values
			addToSetgColl(makeAssembCollect("[top level bookmark]",
				MkFldStg( "[style]", ASF_TEXT_STYLE),
				MkFldStg( "[bookmark color]", ASF_RGB)
				));

			// banner 1 is the vertical banner
			addToSetgColl(makeAssembCollect("[banner vertical]",
				MkFldStg( "[location]", ASF_BANNER_LOCATIONV),
				MkFldStg( "[text]", ASF_TEXT),
				MkFldStg( "[link]", ASF_LINK)
				));

			// banner 2 is the horizontal banner
			addToSetgColl(makeAssembCollect("[banner horizontal]",
				MkFldStg( "[location]", ASF_BANNER_LOCATIONH),
				MkFldStg( "[text]", ASF_TEXT),
				MkFldStg( "[link]", ASF_LINK)
				));

			addToSetgColl(makeAssembCollect("[hyperlinks]",
				MkFldStg( "[border color]", ASF_RGB),
				MkFldStg( "[dash pattern]", ASF_DASH),
				MkFldStg( "[thickness]", ASF_THICKNESS),
				MkFldStg( "[fill color]", ASF_RGB),
				MkFldStg( "[opaque]", ASF_OPACITY),
				MkFldStg( "[margin lr]", ASF_FLOAT),
				MkFldStg( "[margin tb]", ASF_FLOAT)
				));

			addToSetgColl(makeAssembCollect("[custom footer]",
				MkFldStg( "[text]", ASF_TEXT)
				));

			addToSetgColl(makeAssembCollect("[toc sheet]",
				MkFldStg( "[include]", ASF_BOOL),
				MkFldStg( "[sheet number]", ASF_TEXT)
				));
			
			addToSetgColl(makeAssembCollect("[test settings]",
				MkFldStg( "[make rectangles]", ASF_BOOL),
				MkFldStg( "[rect opacity]", ASF_OPACITY)
				));

			addToSetgColl(makeAssembCollect("[test rect colors]",
				MkFldStg( "[hlink fill]", ASF_RGB),
				MkFldStg( "[find fill]", ASF_RGB),
				MkFldStg( "[return fill]", ASF_RGB),
				MkFldStg( "[banner fill]", ASF_RGB),
				MkFldStg( "[author fill]", ASF_RGB),
				MkFldStg( "[disclaimer fill]", ASF_RGB),
				MkFldStg( "[footer fill]", ASF_RGB)
				));
		}


		public bool GetFieldCollection(string idTag, out AssemblerSettingCollection asc)
		{
			if (!AssemblerSettingCollection.TryGetValue(idTag, out asc)) return false;
			return true;
		}

		public bool GetField(string idTag, string fieldId, out AssemblerFieldSetting afs)
		{
			afs = null;
			AssemblerSettingCollection asc;

			if (!AssemblerSettingCollection.TryGetValue(idTag, out asc)) return false;

			if (asc.SettingsValues.TryGetValue(fieldId, out afs)) return false;

			return true;
		}

		public bool GetField(string fieldId, AssemblerSettingCollection asc, out AssemblerFieldSetting afs)
		{
			if (!asc.SettingsValues.TryGetValue(fieldId, out afs)) return false;
			
			if (afs==null) return false;

			afs=asc.SettingsValues[fieldId];

			return true;
		}

		public bool HasField(string fieldId, AssemblerSettingCollection asc)
		{
			return asc.SettingsValues.ContainsKey(fieldId);
		}

		public bool SetFieldValue(string value, string idTag, string fieldId)
		{
			AssemblerFieldSetting afs;

			if (!GetField(idTag, fieldId, out afs)) return false;

			afs.SetValue(value);

			return true;
		}

		// used to populate the setting config list

		private void addToSetgColl(AssemblerSettingCollection asc)
		{
			AssemblerSettingCollection.Add(asc.TypeIdTag, asc);
		}

		private AssemblerSettingCollection makeAssembCollect(
			// AssemblerSettingTypeId typeId, 
			string typeIdTag, params AssemblerFieldSetting[] fieldSetting)
		{
			AssemblerSettingCollection asc = new AssemblerSettingCollection(typeIdTag, fieldSetting);

			return asc;
		}

		private AssemblerFieldSetting MkFldStg(string fieldTag, AssemblerSettingFormat format)
		{
			return new AssemblerFieldSetting(fieldTag, format);
		}
	}
}