#region + Using Directives

using System.Diagnostics;
using System.IO;
using DebugCode;
using Settings;

using ShSheetData.SheetData2;
using ShSheetData.ShSheetData2;
using ShTempCode.DebugCode;
using UtilityLibrary;


#endregion

// user name: jeffs
// created:   5/22/2024 8:01:29 PM

namespace CreatePDFSamples.PdfSupport
{
	public class ProcessPdfs
	{
		private const string PDF_FILE_NAME = "Sheet Sample.pdf";

		private string exampleToCreate = "TestBoxes";

		private CreatePdfSample createPdfSample;

		private SheetData2? sheetRects;

		private string pdfFilePath;

		private List<FilePath<FileNameSimple>> sampleTbFiles;
		private List<string> failList;
		private List<FilePath<FileNameSimple>> goodList;


		public string DataFilePath { get; private set; }

		public bool ProcessNewPage(string datafilepath, string samplePdfFilePath)
		{
			DM.DbxLineEx(0, "Start", 0, 1);

			// the path to the xml file
			DataFilePath= datafilepath;

			initSheetData();
			
			// pdfFilePath = SheetDataSetConsts.SHEET_DATA_FOLDER+PDF_FILE_NAME;
			pdfFilePath = samplePdfFilePath;

			createPdfSample = new CreatePdfSample(pdfFilePath);

			createPdfSample.BeginSample();

			foreach (KeyValuePair<string, SheetData2> kvp in SheetDataManager2.Data!.SheetDataList)
			{
				sheetRects= kvp.Value;

				createPdfSample.AppendSampleNewPage(sheetRects);
			}

			createPdfSample.CompleteSample();


			DM.DbxLineEx(0, "End", 0, -1);

			return true;
		}

		public bool ProcessExistPage(string datafilepath, string samplePdfFilePath, string sampleTbPath)
		{
			DM.DbxLineEx(0, "Start", 0, 1);
			
			DataFilePath = datafilepath;

			initSheetData();

			if (!getSampleTbFileList(sampleTbPath)) return false;

			if (!File.Exists(datafilepath)) return false;
			
			// the path to the xml file

			pdfFilePath = samplePdfFilePath;

			Console.WriteLine($"creating| {samplePdfFilePath}");

			createPdfSample = new CreatePdfSample(pdfFilePath);

			createPdfSample.BeginSample();

			string fileName;
			
			foreach (FilePath<FileNameSimple> filePath in goodList)
			{
				fileName = filePath.FileNameNoExt;

				sheetRects = SheetDataManager2.Data!.SheetDataList[fileName];

				createPdfSample.AppendSampleExistPage(filePath.FullFilePath, sheetRects);
			}

			createPdfSample.CompleteSample();

			DM.DbxLineEx(0, "End", 0, -1);

			return true;
		}


		public void Show(int which)
		{
			showInfo(which);
		}

		private void initSheetData()
		{
			DM.DbxLineEx(0, "Start", 0, 1);

			SheetDataManager2.Init(new FilePath<FileNameSimple>(DataFilePath));

			SheetDataManager2.Read();

			// Dictionary<string, SheetData2> a = SheetDataManager2.Data.SheetDataList;

			showStatus("@read rects");

			DM.DbxLineEx(0, "End",0, -1);
		}

		private bool getSampleTbFileList(string sampleTbPath)
		{
			if (!Directory.Exists(sampleTbPath)) return false;
			if (!getSampleTbFiles(sampleTbPath)) return false;

			string fileName;

			failList = new List<string>();
			goodList = new List<FilePath<FileNameSimple>>();

			foreach (FilePath<FileNameSimple> tb in sampleTbFiles)
			{
				fileName = tb.FileNameNoExt;

				if (!SheetDataManager2.Data!.SheetDataList.ContainsKey(fileName))
				{
					failList.Add(fileName);
				}
				else
				{
					goodList.Add(tb);
				}
			}

			if (goodList.Count < 1) return false;

			return true;
		}

		private bool getSampleTbFiles(string path)
		{
			if (!Directory.Exists(path)) return false;

			sampleTbFiles = new List<FilePath<FileNameSimple>>();

			foreach (string file in Directory.GetFiles(path, "*.pdf"))
			{
				sampleTbFiles.Add(new FilePath<FileNameSimple>(file));
			}

			return sampleTbFiles.Count > 0;
		}

		public override string ToString()
		{
			return $"this is {nameof(ProcessPdfs)}";
		}

		public void showInfo(int which)
		{
			DM.DbxLineEx(0, "Start", 0, 1);

			Debug.Write("\n");
			Debug.WriteLine($"pdf path {pdfFilePath}");
			Debug.WriteLine($"got path {SheetDataManager2.GotDataPath}");
			Debug.WriteLine($"fileName {SheetDataManager2.DataPath.FileName} (only use if \"corrected\"");
			Debug.WriteLine($"filePath {SheetDataManager2.DataFilePath}");


			switch (which)
			{
			case 1:
				{
					ShowInfo.ShowSheetNames2(ShowWhere.DEBUG);
					break;
				}
			case 2:
				{
					ShowInfo.showShtRects2(ShowWhere.DEBUG);
					break;
				}
			case 3:
				{
					ShowInfo.ShowValues2(ShowWhere.DEBUG);
					break;
				}
			}

			Debug.Write("\n");

			DM.DbxLineEx(0, "End",0, -1);
		}

		public static void showStatus(string msg)
		{
			string qty = $"{SheetDataManager2.SheetsCount}";

			DM.DbxLineEx(0, $"sheet count| {qty} | ({msg})", 1, -1);
		}
	}
}
