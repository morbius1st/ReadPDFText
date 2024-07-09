#region + Using Directives

#endregion

// user name: jeffs
// created:   6/2/2024 11:22:51 PM

using System.IO;
using UtilityLibrary;

namespace ShTempCode.DebugCode
{
	public struct Sample
	{
		private const string TEMP_CONFIG_FILE = "PdfAssemblerSettings.xlsx";

		/// <summary>
		/// index number & key
		/// </summary>
		public int Index { get; }
		/// <summary>
		/// entry description
		/// </summary>
		public string Description { get; }
		
		// // for assembly
		//
		// /// <summary>
		// /// root folder<br/>
		// /// ** for assembly: base folder for various files<br/>
		// /// ** for scan: not used
		// /// </summary>
		// public string BaseFolder { get; set; }
		// /// <summary>
		// /// ** for assembly: folder with the PDF files to process<br/>
		// /// final location is base folder + pdf folder name (PdfFolder)<br/>
		// /// ** for scan: not used
		// /// </summary>
		// public string PdfFolderName { get; set; }
		// /// <summary>
		// /// ** for assembly: name of the excel file with the PDF files to process<br/>
		// /// final location is base folder + sheet list file name (SheetListFilePath)<br/>
		// /// * for scan: not used
		// /// </summary>
		// public string SheetListFileName { get; set; }
		// /// <summary>
		// /// ** for assembly: name of the output file<br/>
		// /// final location is base folder + dest file name (DestFilePath)<br/>
		// /// ** for scan: not used
		// /// </summary>
		// // public string DestFileName { get; set; }
		//
		// // for scan and assembly
		//
		// /// <summary>
		// /// ** for assembly: file path for the input data file (with the sheet box info)<br/>
		// /// final location is DataFileFolder + DataFileName<br/>
		// /// ** for scan: file name for the output data file<br/>
		// /// /// final location is DataFileFolder + DataFileName
		// /// </summary>
		// public string DataFilePathString { get; set; }
		//
		// // for scan
		//
		// /// <summary>
		// /// ** for assembly: not used<br/>
		// /// ** for scan: where the scan input PDF are located
		// /// </summary>
		// public string ScanPdfFolder { get; set; }
		//
		//
		// // for create
		//
		// /// <summary>
		// /// ** for assembly: not used<br/>
		// /// ** for scan: where the scan output PDF are located
		// /// </summary>
		// public string CreatePdfFilePathString { get; set; }



		public Sample(int idx,
			string desc,
			// for assembly
			string baseFolder,
			string pdfFolderName,
			string sheetListFileName,
			string destFileName,
			// for scan, create, and assembly
			string dataFilePathString,
			// for scan
			string scanPdfFolder,
			// for create
			string createPdfFilePathString
			) : this()
		{
			Index = idx;
			Description = desc;

			// for assembly
			// BaseFolder = baseFolder;
			// PdfFolderName = pdfFolderName;
			// SheetListFileName = sheetListFileName;
			// DestFileName = destFileName;

			// for assembly
			

			if (baseFolder != null)
			{
				if (pdfFolderName != null) PdfFolder = new FilePath<FileNameSimple>(baseFolder + pdfFolderName);
				if (sheetListFileName != null)SheetListFilePath = new FilePath<FileNameSimple>(baseFolder + sheetListFileName);
				if (destFileName != null)DestFilePath = new FilePath<FileNameSimple>(baseFolder + destFileName);
				if (SheetListFilePath !=null) 
					ConfigSettingFilePath = new FilePath<FileNameSimple>(new [] { SheetListFilePath.FolderPath, TEMP_CONFIG_FILE });
			}

			// for scan, create, & assembly
			// DataFilePathString = dataFilePathString;
			if (dataFilePathString != null) DataFilePath = new FilePath<FileNameSimple>(dataFilePathString);

			// for scan
			// ScanPdfFolder = scanPdfFolder;
			if (scanPdfFolder != null) ScanPDfFolder = new FilePath<FileNameSimple>(scanPdfFolder);

			// for create
			// CreatePdfFilePathString = createPdfFilePathString;
			if (createPdfFilePathString != null) CreatePdfFilePath = new FilePath<FileNameSimple>(createPdfFilePathString);
		}

		// for assembly

		/// <summary>
		/// for assembly: file path for the configuration file / assembly settings
		/// </summary>
		public FilePath<FileNameSimple> ConfigSettingFilePath { get; }

		// /// <summary>
		// /// for assembly: base folder for various files
		// /// </summary>
		// public FilePath<FileNameSimple> BaseFolderPath { get; }

		/// <summary>
		/// for assembly: folder with the PDF files to process<br/>
		/// for scan: not used
		/// </summary>
		public FilePath<FileNameSimple> PdfFolder { get; }

		/// <summary>
		/// for assembly: path for the sheet list excel file<br/>
		/// for scan: not used
		/// </summary>
		public FilePath<FileNameSimple> SheetListFilePath { get; }

		/// <summary>
		/// for assembly: the complied output file<br/>
		/// for scan: not used
		/// </summary>
		public FilePath<FileNameSimple> DestFilePath { get; }

		// for scan, create, and assembly

		/// <summary>
		/// for assembly: not used<br/>
		/// for scan: the output list of sheet boxes
		/// </summary>
		public FilePath<FileNameSimple> DataFilePath  { get; }

		// for scan

		/// <summary>
		/// for assembly: not used<br/>
		/// for scan: the location for the PDF's to scan
		/// </summary>
		public FilePath<FileNameSimple> ScanPDfFolder  { get; }

		// for create

		/// <summary>
		/// for assembly: not used<br/>
		/// for scan: where the scan output PDF are located
		/// </summary>
		public FilePath<FileNameSimple> CreatePdfFilePath  { get; }

	}



	public class ShSamples
	{
		public const string DATA_FILE_FOLDER = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\";

		// public const string DATA_FILE_NAME  = "SheetData.xml";
		// public const string DATA_FILE_PATH = DATA_FILE_FOLDER + DATA_FILE_NAME;

		public const string ROOT_PATH_01 = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\TestBoxes\";

		static ShSamples() { }

		private static void init()
		{
			for (int i = 0; i < FilesNames_01.Length; i++)
			{
				FilesNames_01[i] = ROOT_PATH_01 + FilesNames_01[i];
			}
		}

		public bool GetFilesFromPdfFolder(int idx)
		{
			Sample s = SampleAssembleData[idx];

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

		public bool SelectScanSample(int def, bool selectDefault)
		{
			bool repeat = true;
			bool result;
			string c;
			int idx;
			Sample s;
			string desc = null;

			result = SampleScanData.ContainsKey(def);

			if (result)
			{
				if (selectDefault)
				{
					Selected = SampleScanData[def];
					return true;
				}
			}
			else
			{
				def = 7;
			}
			
			Selected = SampleScanData[def];

			Console.Write("\n");
			Console.WriteLine("Select Sample");

			do
			{
				// foreach (KeyValuePair<int, Sample> kvp in SampleScanData)
				// {
				// 	if (kvp.Key == def) desc = "*** current | ";
				//
				// 	desc += $"* {kvp.Value.Description}";
				//
				// 	Console.WriteLine($">{kvp.Key,3:F0} | {desc}");
				//
				// 	desc = null;
				// }
				//
				// Console.WriteLine($">{'c',3} | *** Select Current");
				// Console.WriteLine($">{'x',3} | *** Exit");
				//
				// Console.Write("\n");
				// Console.Write("** > ");

				showSampleOptions(def);

				c = Console.ReadKey().KeyChar.ToString().ToLower();
				
				Console.Write($"{c}\n");

				if (c.Equals("x")) return false;
				if (c.Equals("c")) return true;

				result = Int32.TryParse(c, out idx);

				if (result)
				{
					result = SampleScanData.TryGetValue(idx, out s);

					if (result)
					{
						Selected = s;
						repeat = false;
					}
					else
					{
						Console.WriteLine("**********  Invalid Selection ************");
					}
				}
				else
				{
					Console.WriteLine($"c = {c}, parse = {idx} **********  Invalid Selection ************");
				}
			}
			while (repeat);

			return true;
		}

		private void showSampleOptions(int def)
		{
			string desc = null;

			foreach (KeyValuePair<int, Sample> kvp in SampleScanData)
			{
				if (kvp.Key == def) desc = "*** current | ";

				desc += $"* {kvp.Value.Description}";

				Console.WriteLine($">{kvp.Key,3:F0} | {desc}");

				desc = null;
			}

			Console.WriteLine($">{'c',3} | *** Select Current");
			Console.WriteLine($">{'x',3} | *** Exit");

			Console.Write("\n");
			Console.Write("** > ");
		}

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

		public List<string> FilePathList { get; private set; }

		public Sample Selected { get; private set; }

		// static data
		public Dictionary<int, Sample> SampleAssembleData = new Dictionary<int, Sample>()
		{
			{
				2,
				new Sample(2, 
					"Large Coliseum Set",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Coliseum\",
					@"PDF Files",
					@"Sheet List.xlsx",
					@"Combined.pdf",
					null, null, null

					)
			},
			{
				3,
				new Sample(3, "Small Sample Set (7) of Coliseum and Simon Brea",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test3\",
					@"PDF Files",
					@"Sheet List.xlsx",
					@"Combined-3.pdf",
					null, null, null
					)
			},
			{
				4,
				new Sample(4, "Small Sample Set (8) of Coliseum and Simon Brea",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test4\",
					@"PDF Files",
					@"Sheet List.xlsx",
					@"Combined-6.pdf",
					null, null, null
					)
			},
			{
				5,
				new Sample(5, "Small Sample Set (6) with various rotations",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test5\",
					@"PDF Files",
					@"Sheet List.xlsx",
					@"Combined-7.pdf",
					null, null, null
					)
			},
			{
				6,
				new Sample(6, "Small Sample Set (3)",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test6\",
					@"PDF Files",
					@"Sheet List.xlsx",
					@"Combined-6.pdf",
					null, null, null
					)
			},
			{
				7,
				new Sample(7, 
					"Special Small Sample Set (3) with (3) different Rotations",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test7\",
					@"PDF Files",
					@"Sheet List.xlsx",
					@"Combined-7.pdf",
					null, null, null
					)
			}
		};

		// for scan and create
		public Dictionary<int, Sample> SampleScanData = new Dictionary<int, Sample>()
		{
			{
				1,
				new Sample(1, 
					"Sample TestBoxes",
					null, null, null, null,
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\SheetData1.xml",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\TestBoxes\",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Sheet Sample.pdf"
					)
			},
			{
				2,
				new Sample(2, 
					"Special Small Sample Set (3) with (3) different Rotations (new)",
					null, null, null, null,
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\SheetData2.xml",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test7\PDF Files\",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Sheet Sample2.pdf"
					)
			},
			{
				7,
				new Sample(7, 
					"Special Small Sample Set (3) with (3) different Rotations (new)",
					null, null, null, null,
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\SheetData7.xml",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Test7\PDF Files\",
					@"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Sheet Sample7.pdf"
					)
			},
		};


	}
}