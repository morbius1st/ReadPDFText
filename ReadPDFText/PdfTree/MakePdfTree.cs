#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonPdfCodeShCode;
using SharedCode.ShDataSupport.ScheduleListSupport;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   3/30/2024 3:48:21 PM

namespace SharedCode.ShDataSupport.PdfTree
{
	public class MakePdfTree
	{
		private ScheduleListManager schMgr;
		private PdfNodeTree tree;

		public PdfNodeTree Tree
		{
			get => tree;
			set => tree = value;
		}

		private Dictionary<SheetBorderType, SheetData> shtData;

		public MakePdfTree(ScheduleListManager schMgr, Dictionary<SheetBorderType, SheetData> shtData)
		{
			this.schMgr = schMgr;
			this.shtData=shtData;
		}

		private int currDepth = 0;

		public bool MakeTree()
		{
			if (schMgr == null) { return false; }

			int j = 0;
			RowData rd;

			tree = new PdfNodeTree();

			APdfTreeNode root =tree.Root;
			APdfTreeNode branch = root;

			foreach (KeyValuePair<string, RowData> kvp in schMgr.RowData)
			{
				// Debug.Write($"process| {kvp.Value.SheetBookMark}| curr branch| {branch?.Bookmark ?? "none"}");

				// if (currDepth == -1)
				// {
				// 	currDepth = 0;
				//
				// 	if (kvp.Value.RowType == PdfTreeItemType.PT_BRANCH)
				// 	{
				// 		root.AddBranch(kvp.Value.SheetBookMark, out branch);
				// 	}
				// 	else
				// 	{
				// 		root.AddBranch("", out branch);
				// 	}
				// }

				if (kvp.Value.RowType == PdfTreeItemType.PT_BRANCH)
				{
					addBranch(kvp.Value, root, out branch);
				}
				else
				{
					addLeaf(kvp.Value, branch);
				}

				// Debug.Write("\n");
				
			}

			return true;
		}

		private void addBranch(RowData rd, APdfTreeNode root, out APdfTreeNode result)
		{
			result = null;
			APdfTreeNode currBranch = root;

			foreach (string hdg in rd.Headings)
			{
				// Debug.Write($"checking branch| {hdg}");

				if (hdg.Trim().IsVoid()) break;

				if (currBranch.ContainsBranch(hdg, out result))
				{
					currBranch = result;
				}
				else
				{
					// Debug.Write($"adding branch| {hdg}");
					currBranch.AddBranch(hdg, rd.PageNum, out result);
					currBranch = result;
				}

				// Debug.Write("\n");
			}
		}

		private void addLeaf(RowData rd, APdfTreeNode branch)
		{
			// string fileString;
			// string bookmark;

			FilePath<FileNameSimple> filePath;
			PdfTreeLeaf leaf;


			// fileString = string.Concat(fileLocation, "\\", sd.String1);
			// filePath = new FilePath<FileNameSimple>(fileString);

			filePath = new FilePath<FileNameSimple>(rd.InPdfFile.FullFilePath);


			leaf = new PdfTreeLeaf(rd.SheetBookMark, filePath, 1, rd.SheetNumber, rd.SheetName);
			leaf.IgnoreList = rd.IgnoreList;
			leaf.SheetBorderType = rd.SheetBorder;
			leaf.PageNumber = rd.PageNum;
			leaf.PageCount = rd.PageCount;
			leaf.SheetNumber = rd.SheetNumber;
			leaf.SheetName = rd.SheetName;

			leaf.SheetData = shtData[rd.SheetBorder];
			
			branch.AddNode(leaf);
		}

		public override string ToString()
		{
			return $"this is {nameof(MakePdfTree)}";
		}
	}
}
