#region + Using Directives
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using Org.BouncyCastle.Crypto.Parameters;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   3/31/2024 6:46:17 PM

namespace SharedCode.ShDataSupport.ExcelSupport
{
	public class PdfAssemblerSettingSupport
	{
		private ExcelManager xlMgr;
		private PdfAssemblerSettingsData pass;

		public FilePath<FileNameSimple> configSettingFilePath { get; set; }


		public bool ConfigFileFound => configSettingFilePath!=null && configSettingFilePath.Exists;

		public PdfAssemblerSettingSupport(FilePath<FileNameSimple> configSettingFilePath)
		{
			this.configSettingFilePath = configSettingFilePath;

			xlMgr = new ExcelManager();
			pass = new PdfAssemblerSettingsData();
		}


		public bool Process()
		{
			if (!parseSchedule()) return false;

			// showSchedule();

			return true;
		}

		public void updateConfig(MergePdfConfig cfg)
		{
			AssemblerSettingCollection asc;

			string key;

			key = "[general settings]";

			if (pass.AssemblerSettingCollection.ContainsKey(key))
			{
				asc = pass.AssemblerSettingCollection[key];

				cfg.CorrectSheetRotation = asc.SettingsValues["[fix rotation]"].Bool;
			}


			key = "[top level bookmark]";

			if (pass.AssemblerSettingCollection.ContainsKey(key))
			{
				asc = pass.AssemblerSettingCollection[key];

				cfg.TopLevelBookmarkColor = asc.SettingsValues["[bookmark color]"].DeviceRgb;
				cfg.TopLevelBookmarkTextStyle = asc.SettingsValues["[style]"].TextStyle;

				// the vertical banner
				asc = pass.AssemblerSettingCollection["[banner vertical]"];

				if (!asc.SettingsValues["[location]"].Value.IsVoid())
				{
					cfg.BannerVerticalInclude = true;
					cfg.BannerVertical = asc.SettingsValues["[text]"].Text;
					cfg.BannerVerticalPosition = asc.SettingsValues["[location]"].BannerPosition;
				}
				else
				{
					cfg.BannerVerticalInclude = false;
				}
			}

			key = "[banner horizontal]";

			if (pass.AssemblerSettingCollection.ContainsKey(key))
			{
				// the horizontal banner
				asc = pass.AssemblerSettingCollection[key];

				if (!asc.SettingsValues["[location]"].Value.IsVoid())
				{
					cfg.BannerHorizontalInclude = true;
					cfg.BannerHorizontal = asc.SettingsValues["[text]"].Text;
					cfg.BannerHorizontalPosition = asc.SettingsValues["[location]"].BannerPosition;
				}
				else
				{
					cfg.BannerHorizontalInclude = false;
				}
			}

			key = "[hyperlinks]";

			if (pass.AssemblerSettingCollection.ContainsKey(key))
			{
				asc = pass.AssemblerSettingCollection[key];

				cfg.LinkDashPattern = asc.SettingsValues["[dash pattern]"].DashPattern;
				cfg.LinkLineWidth = asc.SettingsValues["[thickness]"].Thickness;

				cfg.LinkFillOpacity = asc.SettingsValues["[opaque]"].Opacity;

				cfg.LinkStrokeColor = asc.SettingsValues["[border color]"].DeviceRgb;
				cfg.LinkFillColor = asc.SettingsValues["[fill color]"].DeviceRgb;

				cfg.SetMargins( asc.SettingsValues["[margin lr]"].Float, asc.SettingsValues["[margin tb]"].Float);
			}

			key = "[custom footer]";

			if (pass.AssemblerSettingCollection.ContainsKey(key))
			{
				asc = pass.AssemblerSettingCollection[key];

				cfg.FooterUser = asc.SettingsValues["[text]"].Text;
			}

			key = "[toc sheet]";

			if (pass.AssemblerSettingCollection.ContainsKey(key))
			{
				asc = pass.AssemblerSettingCollection[key];

				cfg.TocInclude = asc.SettingsValues["[include]"].Bool;
				cfg.TocSheet = asc.SettingsValues["[sheet number]"].Text;
				// cfg.TocPage = asc.SettingsValues["[page number]"].Int;
			}

			key = "[test settings]";

			if (pass.AssemblerSettingCollection.ContainsKey(key))
			{
				asc = pass.AssemblerSettingCollection[key];

				cfg.TestMakeRects = asc.SettingsValues["[make rectangles]"].Bool;
				cfg.TestRectFillOpacity = asc.SettingsValues["[rect opacity]"].Opacity;

			}

			key = "[test rect colors]";

			if (pass.AssemblerSettingCollection.ContainsKey(key))
			{
				asc = pass.AssemblerSettingCollection[key];

				cfg.TestRecFillHlink      = asc.SettingsValues["[hlink fill]"]     .DeviceRgb;
				cfg.TestRecFillFind       = asc.SettingsValues["[find fill]"]      .DeviceRgb;
				cfg.TestRecFillReturn     = asc.SettingsValues["[return fill]"]    .DeviceRgb;
				cfg.TestRecFillBanner     = asc.SettingsValues["[banner fill]"]    .DeviceRgb;
				cfg.TestRecFillAuthor     = asc.SettingsValues["[author fill]"]    .DeviceRgb;
				cfg.TestRecFillDisclaimer = asc.SettingsValues["[disclaimer fill]"].DeviceRgb;
				cfg.TestRecFillFooter     = asc.SettingsValues["[footer fill]"]    .DeviceRgb;
			}

			/*
			*/

		}

		private bool parseSchedule()
		{
			if (!ConfigFileFound) return false;

			if (!xlMgr.ReadSchedule(configSettingFilePath)) return false;

			bool result;
			string typeIdTag;

			AssemblerSettingCollection asc;

			xlMgr.GetRows();

			foreach (DataRow row in xlMgr.Rows)
			{
				typeIdTag = row[0].ToString();

				if (typeIdTag.IsVoid() || typeIdTag.StartsWith("<<")) continue;

				typeIdTag = typeIdTag.Trim().ToLower();
				if (!pass.GetFieldCollection(typeIdTag, out asc)) continue;

				parseRow(row.ItemArray, asc);
			}

			return true;
		}

		private void parseRow(object[] rowItems, AssemblerSettingCollection asc)
		{
			AssemblerFieldSetting afs;
			string value;

			for (int i = 1; i < rowItems.Length; i++)
			{
				value = rowItems[i].ToString();

				if (value.IsVoid() ||
					!pass.GetField(value, asc, out afs)) continue;

				value = rowItems[++i].ToString();

				afs.SetValue(value);
			}
		}

		private void showSchedule()
		{
			string t1;
			foreach (KeyValuePair<string, AssemblerSettingCollection> kvp in pass.AssemblerSettingCollection)
			{
				t1 = kvp.Key;

				foreach (KeyValuePair<string, AssemblerFieldSetting> kvp2 in kvp.Value.SettingsValues)
				{
					showFields(kvp2.Value, t1);
				}
			}
		}

		private void showFields(AssemblerFieldSetting afs, string t1)
		{
			string f1 = afs.FieldId;
			string f2 = afs.FieldFormat.ToString();

			string v1 = afs.Value;

			Debug.WriteLine($"{t1,-24}| {f1,-24} | {f2,-24}| {v1}");

		}

		public override string ToString()
		{
			return $"this is {nameof(PdfAssemblerSettingSupport)}";
		}
	}
}
