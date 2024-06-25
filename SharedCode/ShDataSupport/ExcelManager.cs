#region + Using Directives

using System.Data;
using System.IO;
using ExcelDataReader;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   3/28/2024 6:58:47 PM

namespace SharedCode.ShDataSupport.ExcelSupport
{
	public class ExcelManager
	{
		private string currDiscipline;
		private string currHeading1;
		private string currHeading2;

		private DataSet schedule;

		public FilePath<FileNameSimple> ScheduleFile { get; set; }

		public DataRowCollection Rows { get; set; }

		public bool ReadSchedule(
			FilePath<FileNameSimple> filePath)
		{
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

			schedule = null;

			ScheduleFile = filePath;

			try
			{
				using ( FileStream stream = File.Open(filePath.FullFilePath, 
							FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
					)
				{
					using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream) )
					{
						schedule = reader.AsDataSet();
					}
				}

			}
			catch
			{
				return false;
			}

			return true;
		}

		public void GetRows()
		{
			Rows = schedule.Tables[0].Rows;
		}


		// public DataRowCollection GetUsedRows()
		// {
		// 	GetRows();
		//
		// 	if (Rows == null || Rows.Count == 0) return null;
		//
		// 	DataTable dt = schedule.Tables[0].Clone();
		// 	DataRowCollection rc = dt.Rows;
		//
		// 	string item1;
		//
		// 	foreach (DataRow row in Rows)
		// 	{
		// 		item1 = row.ItemArray[0].ToString();
		//
		// 		if (item1.IsVoid() ||
		// 			item1.StartsWith("<<")) continue;
		// 		
		// 		rc.Add(row);
		// 	}
		//
		// 	return rc;
		// }


		// ****************

		public override string ToString()
		{
			return $"this is {nameof(ExcelManager)}";
		}

/*
		public void ParseRows()
		{
			GetRows();

			object[] items;
			int idx = 0;
			pageNum = 1;

			RowData = new Dictionary<string, RowData>(RowCount);

			foreach (DataRow row in Rows)
			{
				if (idx++ >= 55) break;

				// Debug.Write($"process row| {++max}   (item count| {row.ItemArray.Length})");

				if (!validateRow(row))
				{
					// Debug.WriteLine($"| row is invalid");
					continue;
				}

				// Debug.WriteLine("");

				parseRowData(row.ItemArray);
			}

			showRowData();

		}

		private void parseRowData(object[] rowItems)
		{
			addBranch(rowItems);

			RowData rd = new RowData();

			rd.FileName = rowItems[3].ToString().Trim();
			rd.SheetNumber = rowItems[4].ToString().Trim();

			if (rd.SheetNumber.IsVoid()
				|| rd.FileName.IsVoid()) return;

			rd.RowType = PdfTreeItemType.PT_LEAF;

			rd.Discipline = null;
			rd.Headings = null;

			rd.SheetName= rowItems[5].ToString().Trim();

			rd.Options = new [] { true, true, true, true };

			rd.InPdfFile = new FilePath<FileNameAsSheetFile>(
				new string[] { PdfFolderPath.FolderPath, rd.FileName });

			rd.Valid = rd.Found;
			rd.PageNumber = pageNum++;

			rd.SheetBorder = getBorderType(rowItems[6]);

			RowData.Add(rd.SheetNumber, rd);
		}

		private SheetBorderType getBorderType(object bto)
		{
			SheetBorderType bt = SheetBorderType.ST_NA;

			string s = bto.ToString().Trim();

			if (s.IsVoid()) return bt;

			bool result = Enum.TryParse(s, out bt);

			if (!result) return SheetBorderType.ST_NA;

			return bt;
		}

		private void addBranch(object[] rowItems)
		{
			string discipline = rowItems[0]?.ToString() ?? string.Empty;

			if (discipline.IsVoid()) return;

			RowData rd = new RowData();
			rd.RowType = PdfTreeItemType.PT_BRANCH;
			string[] hdg = new string[2];

			hdg[0] = rowItems[1]?.ToString() ?? string.Empty;
			hdg[1] = rowItems[2]?.ToString() ?? string.Empty;

			rd.Discipline = discipline;
			rd.Headings = hdg;
			rd.PageNumber = pageNum;
			rd.SheetNumber = $"branch{branchIdx++:D5}";
			rd.SheetBorder = SheetBorderType.ST_NA;
			rd.Valid = true;
			RowData.Add(rd.SheetNumber, rd);
		}


		private bool validateRow(DataRow row)
		{
			string s0 = row.ItemArray[0].ToString().Trim();
			string s3 = row.ItemArray[3].ToString().Trim();
			
			if (s3.IsVoid() || s0.StartsWith("<<") || s0.StartsWith("[")) return false;

			return true;
		}

		// ****************

		private void showRowData()
		{
			foreach (KeyValuePair<string, RowData> kvp in RowData)
			{
				showRowData(kvp.Value);
			}
		}

		private void showRowData(RowData rd)
		{
			string r0 = rd.RowType.ToString();
			string p0 = rd.PageNumber.ToString("##");
			string d0 = rd.Discipline ?? "";
			string h1 = rd.Headings?[0] ?? "";
			string h2 = rd.Headings?[1] ?? "";

			string b0 = rd.SheetBorder.ToString();

			string s0 = rd.SheetNumber ?? "";

			string f1 = rd.InPdfFile?.FileName ?? "is null";
			string v1 = rd.Valid.ToString();

			Debug.WriteLine(
				$"P| {p0,3}| r| {r0,-10}| D| {d0,-15}| h1| {h1,-35}| h2| {h2,-25}| b| {b0,-15}| s| {s0,-12} f| {f1,-52}| v| {v1,-8}"
				);
		}

		private void showItems(object[] items)
		{
			if (items==null || items.Length==0) return;

			int idx = 1;

			foreach (object item in items)
			{
				string s = item.ToString();
				string status = item == null ? "is null" : s.IsVoid() ? "is empty" : "is string";

				Debug.WriteLine($"item| {idx++,-4}| {$"(status| {status})",-20} {s}");
			}

			Debug.WriteLine("");
		}
*/


	}
}
