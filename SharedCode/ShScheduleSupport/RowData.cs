using System.Collections.Generic;
using CommonPdfCodeShCode;
using UtilityLibrary;

namespace SharedCode.ShDataSupport.ScheduleListSupport
{
	public class RowData
	{
		public PdfTreeItemType RowType { get; set; }
		public int PageNum { get; set; }
		public int PageCount => 1;
		// public string Discipline { get; set; }

		// discipline is 0 / 1+ are headings
		public List<string> Headings { get; set; }
		public string FileName { get; set; } // including extension
		public string SheetNumber { get; set; }
		public string SheetName { get; set; }
		public SheetBorderType SheetBorder { get; set; }
		public bool[] IgnoreList { get; set; }

		public FilePath<FileNameAsSheetFile> InPdfFile { get; set; }

		public bool Found => !InPdfFile.IsFolderPath && InPdfFile.Exists;
		public bool Valid {get; set; }

		public string SheetBookMark => $"{SheetNumber} - {SheetName}";
		// public string FirstBranchBookMark => Discipline;
		// public string SecondBranchBookMark => Headings[0];
		// public string ThirdBranchBookMark => Headings[1];


		// public RowData(
		// 	string discipline, 
		// 	string[] headings, 
		// 	string fileName, 
		// 	string sheetNumber, 
		// 	string sheetName, 
		// 	string sheetFormat, 
		// 	bool[] options) : this()
		// {
		// 	Discipline = discipline;
		// 	Headings = headings;
		// 	FileName = fileName;
		// 	SheetNumber = sheetNumber;
		// 	SheetName = sheetName;
		// 	SheetFormat = sheetFormat;
		// 	Options = options;
		// 	InPdfFile = null;
		// 	Valid = false;
		//
		// }

		public void SetFile(string path)
		{
			InPdfFile = new FilePath<FileNameAsSheetFile>(path + "\\" + FileName);

			if (InPdfFile == null || !Found || !InPdfFile.Extension.Equals(Constants.XL_FILE_EXTN) )
			{
				Valid = false;
			}
			else
			{
				Valid = true;
			}
		}
	}
}