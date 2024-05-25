#region + Using Directives

using CommonPdfCodeShCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharedCode.ShDataSupport.ExcelSupport;
using SharedCode.ShDataSupport.Process;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   3/30/2024 1:24:34 PM

namespace SharedCode.ShDataSupport.ScheduleListSupport
{
	public class ScheduleListManager
	{
		public Dictionary<string, RowData> RowData { get; set; }

		private ExcelManager xlMgr;
		private ValidateFilesInFolder validate;

		private int pageNum;
		private int branchIdx = 0;

		public ScheduleListManager(ExcelManager xlMgr)
		{
			this.xlMgr = xlMgr;
		}

		public int RowCount => xlMgr.Rows.Count;

		public ExcelManager XlMgr { get; set; }

		public FilePath<FileNameSimple> PdfFolderPath { get; set; }

		public bool ReadSchedule(FilePath<FileNameSimple> filePath,
			FilePath<FileNameSimple> pdfFolderPath,
			ValidateFilesInFolder validate
			)
		{
			this.validate = validate;
			PdfFolderPath = pdfFolderPath;

			return xlMgr.ReadSchedule(filePath);
		}

		public void ParseRows()
		{
			xlMgr.GetRows();

			DataRowCollection rowx = xlMgr.Rows;

			object[] items;
			int idx = 0;
			pageNum = 1;

			RowData = new Dictionary<string, RowData>(RowCount);

			foreach (DataRow row in xlMgr.Rows)
			{
				// if (idx++ >= ReadPDFText.maxFilesToCombine) break;
				if (idx++ >= 50) break;

				// Debug.Write($"process row| {++max}   (item count| {row.ItemArray.Length})");

				if (!validateRow(row))
				{
					// Debug.WriteLine($"| row is invalid");
					continue;
				}

				// Debug.WriteLine("");

				parseRowData(row.ItemArray);
			}

			// todo un-comment when needed
			// showRowData();
		}

		private void parseRowData(object[] rowItems)
		{
			// this adds branch nodes as needed
			addBranch(rowItems);

			RowData rd = new RowData();

			// the below adds a typical lead node
			rd.FileName = rowItems[3].ToString().Trim();
			rd.SheetNumber = rowItems[4].ToString().Trim();

			if (rd.SheetNumber.IsVoid()
				|| rd.FileName.IsVoid()) return;

			
			if (RowData.ContainsKey(rd.SheetNumber))
			{
				validate.Duplicates.Add(rd.FileName);
				return;
			}

			rd.RowType = PdfTreeItemType.PT_LEAF;
			rd.Headings = new List<string>();
			rd.SheetName = rowItems[5].ToString().Trim();
			// rd.IgnoreList = new [] { true, true, true, true };
			rd.IgnoreList = getIgnoreList(rowItems);
			rd.InPdfFile = new FilePath<FileNameAsSheetFile>(
				new string[] { PdfFolderPath.FolderPath, rd.FileName });

			rd.Valid = rd.Found;
			rd.PageNum = pageNum++;
			rd.SheetBorder = getBorderType(rowItems[6]);

			RowData.Add(rd.SheetNumber, rd);
		}

		private bool[] getIgnoreList(object[] rowItems)
		{
			bool[] ignoreBools = new bool[5];
			string s;

			int idx = 7;

			for (var i = 0; i < ignoreBools.Length; i++)
			{
				s = rowItems[idx++].ToString();

				if (s.IsVoid() || 
					!bool.TryParse(s, out ignoreBools[i]))
				{
					ignoreBools[i] = true;
				}
			}

			return ignoreBools;
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
			rd.Headings = new List<string>();

			rd.Headings.Add(discipline);
			rd.Headings.Add(rowItems[1]?.ToString() ?? string.Empty);
			rd.Headings.Add(rowItems[2]?.ToString() ?? string.Empty);

			rd.PageNum = pageNum;
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
			int max = rd.Headings.Count;
			string[] h = new string[3];

			for (int i = 0; i < 3; i++)
			{
				if (i < max)
				{
					h[i] = rd.Headings?[i] ?? "";
				}
				else
				{
					h[i] = "";
				}
			}


			string r0 = rd.RowType.ToString();
			string p0 = rd.PageNum.ToString("##");

			string b0 = rd.SheetBorder.ToString();

			string s0 = rd.SheetNumber ?? "";

			string f1 = rd.InPdfFile?.FileName ?? "is null";
			string v1 = rd.Valid.ToString();

			Debug.WriteLine(
				$"P| {p0,3}| r| {r0,-10}| D| {h[0],-15}| h1| {h[1],-35}| h2| {h[2],-25}| b| {b0,-15}| s| {s0,-12} f| {f1,-52}| v| {v1,-8}"
				);
		}

		private void showItems(object[] items)
		{
			if (items == null || items.Length == 0) return;

			int idx = 1;

			foreach (object item in items)
			{
				string s = item.ToString();
				string status = item == null ? "is null" : s.IsVoid() ? "is empty" : "is string";

				Debug.WriteLine($"item| {idx++,-4}| {$"(status| {status})",-20} {s}");
			}

			Debug.WriteLine("");
		}


		public override string ToString()
		{
			return $"this is {nameof(ScheduleListManager)}";
		}
	}
}