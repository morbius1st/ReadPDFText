#region + Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   6/2/2024 11:22:51 PM

namespace ShCode.ShDebugInfo
{
	public struct Sample
	{
		private const string TEMP_CONFIG_FILE = "PdfAssemblerSettings.xlsx";

		public int Index { get; }
		public string Description { get; }

		public string BaseFolder { get; set; }
		public string PdfFolderName { get; set; }
		public string SheetListFileName { get; set; }
		public string DestFileName { get; set; }

		public Sample(int idx,
			string desc,
			string baseFolder,
			string pdfFolderName,
			string sheetListFileName,
			string destFileName) : this()
		{
			Index = idx;
			Description = desc;
			BaseFolder = baseFolder;
			PdfFolderName = pdfFolderName;
			SheetListFileName = sheetListFileName;
			DestFileName = destFileName;

			PdfFolder = new FilePath<FileNameSimple>(baseFolder + pdfFolderName);
			SheetListFilePath = new FilePath<FileNameSimple>(baseFolder + sheetListFileName);
			DestFilePath = new FilePath<FileNameSimple>(baseFolder + destFileName);
			DestFolderPath = new FilePath<FileNameSimple>(DestFilePath.FolderPath);
			ConfigSettingFilePath = new FilePath<FileNameSimple>(new [] { SheetListFilePath.FolderPath, TEMP_CONFIG_FILE });
		}

		public FilePath<FileNameSimple> PdfFolder { get; }
		public FilePath<FileNameSimple> SheetListFilePath { get; }
		public FilePath<FileNameSimple> DestFilePath { get; }
		public FilePath<FileNameSimple> DestFolderPath { get; }
		public FilePath<FileNameSimple> ConfigSettingFilePath { get; }
	}


	public class ShSamples
	{
		public const string SHEET_METRIC_FOLDER = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\";

		public const string DATA_FILE_NAME  = "SheetData.xml";
		public const string DATA_FILE_PATH = SHEET_METRIC_FOLDER + DATA_FILE_NAME;

		public const string ROOT_PATH_01 = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\TestBoxes\";

		// order matters
		public static string[] FilesNames_01 { get; } = new []
		{
			"TestBoxes.pdf",                               // 0
			"A0.1.0 - COVER SHEET.pdf",                    // 1
			"A2.2-G - LEVEL G - OVERALL FLOOR PLAN.pdf",   // 2
			"TestBoxes 2.pdf",                             // 3
			"A0.1.0 - COVER SHEET b.pdf",                  // 4
			"A2.2-G - LEVEL G - OVERALL FLOOR PLAN b.pdf", // 5
		};

		static ShSamples() { }

		private static void init()
		{
			for (int i = 0; i < FilesNames_01.Length; i++)
			{
				FilesNames_01[i] = ROOT_PATH_01 + FilesNames_01[i];
			}
		}

		public List<string> FilePathList { get; private set; }

		public bool GetFilesFromPdfFolder(int idx)
		{
			Sample s = SampleData[idx];

			try
			{
				FilePathList = new List<string>(Directory.GetFiles(s.PdfFolder.FolderPath));
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return false;
			}

			if (FilePathList == null || FilePathList.Count == 0) return false;

			return true;
		}

		public Dictionary<int, Sample> SampleData = new Dictionary<int, Sample>()
		{
			{
				2,
				new Sample(2, "Large Coliseum Set",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Coliseum\",
					@"PDF Files",
					@"Sheet List.xlsx",
					@"Combined.pdf"
					)
			},
			{
				3,
				new Sample(3, "Small Sample Set (7) of Coliseum and Simon Brea",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test3\",
					@"PDF Files",
					@"Sheet List.xlsx",
					@"Combined-3.pdf"
					)
			},
			{
				4,
				new Sample(4, "Small Sample Set (8) of Coliseum and Simon Brea",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test4\",
					@"PDF Files",
					@"Sheet List.xlsx",
					@"Combined-6.pdf"
					)
			},
			{
				5,
				new Sample(5, "Small Sample Set (6) with various rotations",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test5\",
					@"PDF Files",
					@"Sheet List.xlsx",
					@"Combined-7.pdf"
					)
			},
			{
				6,
				new Sample(6, "Small Sample Set (3)",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test6\",
					@"PDF Files",
					@"Sheet List.xlsx",
					@"Combined-6.pdf"
					)
			},
			{
				7,
				new Sample(7, "Special Small Sample Set (3) with (3) different Rotations",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test7\",
					@"PDF Files",
					@"Sheet List.xlsx",
					@"Combined-7.pdf"
					)
			}
		};
	}
}