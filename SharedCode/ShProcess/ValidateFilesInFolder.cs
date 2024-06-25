#region using

using System.Collections.Generic;
using System.IO;
using CommonPdfCodeShCode;
using SharedCode.ShDataSupport.ScheduleListSupport;
using UtilityLibrary;

#endregion

// username: jeffs
// created:  3/30/2024 2:02:03 PM

namespace SharedCode.ShDataSupport.Process
{
	public class ValidateFilesInFolder
	{
		public string[] FolderFiles { get; set; }
		public Dictionary<string, string> FolderSheets { get; set; }
		public List<string> MissingPdfs { get; set; }
		public List<string> FoundPdfs {get; set; }
		public List<string> Duplicates { get; set; } = new List<string>();

		public int invalidShtNumIdx = 0;


		public void getSheetNumbersFromFolder(FilePath<FileNameSimple> pdfFolder)
		{
			FolderFiles = Directory.GetFiles(pdfFolder.FolderPath, "*.pdf", SearchOption.TopDirectoryOnly);
			FolderSheets = new Dictionary<string, string>(FolderFiles.Length);

			string shtNum;

			FilePath<FileNameAsSheetFile> sheetFile;

			for (int i = 0; i < FolderFiles.Length; i++)
			{
				sheetFile = new FilePath<FileNameAsSheetFile>(FolderFiles[i]);

				if (!sheetFile.IsValid) continue;

				if (sheetFile.FileNameObject == null || sheetFile.FileNameObject.SheetNumber.IsVoid())
				{
					shtNum = $"temp-{invalidShtNumIdx++:D5}";
				}
				else
				{
					shtNum = sheetFile.FileNameObject.SheetNumber;
				}

				try
				{
					FolderSheets.Add(shtNum, sheetFile.FileName);
				}
				catch 
				{
					// ignore an error here - most likely just a duplicate sheet number
				}
			}

		}

		// files in sheet schedule but not in the folder
		public bool validateMissingPdfs(Dictionary<string, RowData> rowData)
		{
			MissingPdfs = new List<string>();

			foreach (KeyValuePair<string, RowData> kvp in rowData)
			{
				if (kvp.Value.RowType == PdfTreeItemType.PT_BRANCH) continue;

				if (!kvp.Value.Found)
				{
					MissingPdfs.Add(kvp.Value.FileName ?? "name missing");
				}
			}

			return true;
		}

		// files in the folder but not in the sheet schedule
		public bool validateFoundPdfs(Dictionary<string, RowData> rowData)
		{
			bool results;
			FoundPdfs = new List<string>();

			foreach (KeyValuePair<string, string> kvp in FolderSheets)
			{
				results = rowData.ContainsKey(kvp.Key);

				if (!results)
				{
					FoundPdfs.Add(kvp.Value);
				}
			}

			return true;
		}

		#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ValidateFilesInFolder)}";
		}

		#endregion
	}
}
