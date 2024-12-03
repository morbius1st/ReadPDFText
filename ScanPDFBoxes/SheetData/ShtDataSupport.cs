#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ShCode;
using ShSheetData.SheetData2;
using ShSheetData.ShSheetData2;
using UtilityLibrary;

using SettingsManager;
using ShTempCode.DebugCode;

#endregion

// user name: jeffs
// created:   11/29/2024 9:27:07 PM

namespace ScanPDFBoxes.SheetData
{
	public class ShtDataSupport
	{
		private const int TITLE_WIDTH = -30;

		private IWin w;

		private  SheetManager2 sm2;

		public ShtDataSupport(SheetManager2 sm2)
		{
			this.w = w;
			this.sm2 = sm2;
		}

		private static bool? lastOp { get; set; } = null;

		public void createTempSheetFile(int shtQty, int[] stdBoxes, int optBoxes)
		{
			DM.Start0();

			SetStatus(StatusCodes.SC_G_NONE);

			string id = "Z";

			// string testFile = SHEET_METRIC_FOLDER + "\\" + $"{SHEET_DATA_FILE_TEST}{id}.xml";

			string testFile = SettingsManager.FileLocationSupport.ShtMetricsFilePathUser("Sheet Metrics 01");

			lastOp = sm2.InitDataFilePath(testFile);

			if (lastOp != true)
			{
				DM.End0("end 1");
				return;
			}

			SheetDataTemp.CreateTempSheetData2(SheetDataManager2.Data, shtQty, stdBoxes, optBoxes);

			SheetDataManager2.Write();

			DM.End0();
		}

		private void SetStatus(StatusCodes code,  string note = null,
			[CallerMemberName] string mx = null)
		{
			StatusMgr.SetStatCode(code, note, mx);
		}

	#region classif & metric files

		public void ShowClassifFiles(IWin iw)
		{
			w = iw;

			string userFiles = FileLocationSupport.ClassifFileLocationUser;
			string userFilesSample = FileLocationSupport.ClassifSampleFileLocationUser;

			string defaultFiles = FileLocationSupport.ClassifFileLocationDefault;
			string defaultFilesSample = FileLocationSupport.ClassifSampleFileLocationDefault;

			w.DebugMsgLine($"\n\nUser Classification Files | ({userFiles})");
			showFiles(userFiles, $"*.{FileLocationSupport.DATA_FILE_EXT}");

			w.DebugMsgLine($"\nUser Classification Sample Files | ({userFilesSample})");
			showFiles(userFilesSample, $"*.{FileLocationSupport.DATA_FILE_EXT}");

			w.DebugMsgLine($"\nDefault Classification Files | ({defaultFiles})");
			showFiles(defaultFiles, $"*.{FileLocationSupport.DATA_FILE_EXT}");

			w.DebugMsgLine($"\nDefault Classification Sample Files | ({defaultFilesSample})");
			showFiles(defaultFilesSample, $"*.{FileLocationSupport.DATA_FILE_EXT}");
		}

		public void ShowShtMetricFiles(IWin iw)
		{
			w = iw;

			string userFiles = FileLocationSupport.ShtMetricsFileLocationUser;

			string defaultFiles = FileLocationSupport.ShtMetricsFileLocationDefault;


			w.DebugMsgLine($"\n\nUser Sheet Metric Files | ({userFiles})");
			showFiles(userFiles, $"*.{FileLocationSupport.DATA_FILE_EXT}");

			w.DebugMsgLine($"\nDefault Sheet Metric Files | ({defaultFiles})");
			showFiles(defaultFiles, $"*.{FileLocationSupport.DATA_FILE_EXT}");

		}

		private void showFiles(string path, string pattern)
		{
			if (!Directory.Exists(path))
			{
				w.DebugMsgLine("*** path not found ***\n");
				return;
			}

			int count = 0;

			foreach (string file in Directory.EnumerateFiles(path, pattern, SearchOption.TopDirectoryOnly))
			{
				w.DebugMsgLine($"{++count,4} | {Path.GetFileName(file)}");
			}

			string f = count == 1 ? "file" : "files";

			w.DebugMsgLine($"\n{count} {f} found\n");
		}
		

	#endregion

		public void ShowScanSamples(IWin w)
		{
			this.w = w;

			ShSamples ss = new ShSamples();

			w.DebugMsgLine("\n*** showing scan samples ***\n");

			int len =
				($"{"", TITLE_WIDTH}").Length;

			showDescriptions(ss.SampleAssembleData.First().Value, len, 1);

			foreach (KeyValuePair<int, Sample> kvp in ss.SampleScanData)
			{
				showSampleData(kvp.Key, kvp.Value, 1);
			}
		}

		public void ShowAssemblySamples(IWin w)
		{
			this.w = w;

			ShSamples ss = new ShSamples();

			w.DebugMsgLine("\n*** showing assembly samples ***\n");

			int len =
				($"{"", TITLE_WIDTH}").Length;

			showDescriptions(ss.SampleAssembleData.First().Value, len, 2);

			foreach (KeyValuePair<int, Sample> kvp in ss.SampleAssembleData)
			{
				showSampleData(kvp.Key, kvp.Value, 2);
			}
		}

		private void showSampleData(int key, Sample s, int which)
		{
			w.DebugMsgLine($"key: {key} | {s.Index} {s.Description}");
			w.DebugMsgLine("\t** paths **");


			if (which == 1)  // scans
			{
				w.DebugMsgLine($"\t{"data file path"          , TITLE_WIDTH}| {s.DataFilePath?.FullFilePath ?? "** no path saved **"}");
				w.DebugMsgLine($"\t{"create pdf file path"    , TITLE_WIDTH}| {s.CreatePdfFilePath?.FullFilePath ?? "** no path saved **"}");
				w.DebugMsgLine($"\t{"scan pdf folder"         , TITLE_WIDTH}| {s.ScanPDfFolder?.FullFilePath ?? "** no path saved **"}");
				w.DebugMsgLine($"\t{"blank samples file path" , TITLE_WIDTH}| {s.BlankSamplesFilePath?.FullFilePath ?? "** no path saved **"}");
			}
			else
			if (which == 2)  // assembly
			{
				w.DebugMsgLine($"\t{"base folder"             , TITLE_WIDTH}| {s.BaseFolder ?? "no folder saved"}");
				w.DebugMsgLine($"\t{"config setting file path", TITLE_WIDTH}| {s.ConfigSettingFilePath?.FullFilePath ?? "** no path saved **"}");
				w.DebugMsgLine($"\t{"dest file path"          , TITLE_WIDTH}| {s.DestFilePath?.FullFilePath ?? "** no path saved **"}");
				w.DebugMsgLine($"\t{"pdf folder"              , TITLE_WIDTH}| {s.PdfFolder?.FullFilePath ?? "** no path saved **"}");
				w.DebugMsgLine($"\t{"sheet list file path"    , TITLE_WIDTH}| {s.SheetListFilePath?.FullFilePath ?? "** no path saved **"}");
			}

			w.DebugMsg("\n");

		}


		private void showDescriptions(Sample s, int len, int which)
		{
			w.DebugMsgLine($"property descriptions\n");


			if (which == 1) // scans
			{
				w.DebugMsgLine($"\t{"data file path"          , TITLE_WIDTH}| {showNotes(s.DataFpNotes, len) }");
				w.DebugMsgLine($"\t{"create pdf file path"    , TITLE_WIDTH}| {showNotes(s.CreatePdfFPNotes, len)}");
				w.DebugMsgLine($"\t{"scan pdf folder"         , TITLE_WIDTH}| {showNotes(s.ScanPdfFolderNotes, len)}");
				w.DebugMsgLine($"\t{"blank samples file path" , TITLE_WIDTH}| {showNotes(s.BlankSampleFpNotes, len)}");
			} 
			else
			if (which== 2)  // assembly
			{
				w.DebugMsgLine($"\t{"base folder"             , TITLE_WIDTH}| {showNotes(s.BaseFolderNotes, len) }");
				w.DebugMsgLine($"\t{"config setting file path", TITLE_WIDTH}| {showNotes(s.ConfigStgFpNotes, len)}");
				w.DebugMsgLine($"\t{"dest file path"          , TITLE_WIDTH}| {showNotes(s.DestFpNotes, len)}");
				w.DebugMsgLine($"\t{"pdf folder"              , TITLE_WIDTH}| {showNotes(s.PdfFolderNotes, len)}");
				w.DebugMsgLine($"\t{"sheet list file path"    , TITLE_WIDTH}| {showNotes(s.ShtLstFpNotes, len)}");
			}

			w.DebugMsg("\n");
		}




		private string showNotes(string note, int len)
		{
			if (note.IsVoid()) return null;

			StringBuilder sb = new StringBuilder();

			string[] lines = note.Split("\n");

			if (lines.Length == 0) return null;

			int count = 0;

			foreach (string line in lines)
			{
				sb.Append(formatLine(line, count, len));

				// if (count == 0)
				// {
				// 	sb.Append($"{line}");
				// } 
				// else
				// {
				// 	sb.Append($"\t{" ".Repeat(len)}| {line}");
				// }

				if (count++ != lines.Length) sb.Append("\n");
			}

			return sb.ToString();
		}

		private string formatLine(string line, int count, int len)
		{
			string result;

			if (count == 0)
			{
				result = ($"{line}");
			} 
			else
			{
				result = ($"\t{" ".Repeat(len)}| {line}");
			}

			return result;
		}


		public override string ToString()
		{
			return $"this is {nameof(ShtDataSupport)}";
		}
	}
}
