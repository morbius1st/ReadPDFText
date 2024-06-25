﻿#region using

using SharedCode.ShDataSupport;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using UtilityLibrary;

#endregion

// username: jeffs
// created:  12/9/2023 3:51:34 PM

namespace CommonPdfCodeShCode
{
	public enum PdfTreeItemType
	{
		PT_LEAF,
		PT_BRANCH,
		PT_NODE,
		PT_NODE_FILE,
	}

	public interface IPdfTreeItem
	{
		[DataMember(Order = 1)]
		PdfTreeItemType ItemType { get; }

		[DataMember(Order = 10)]
		string Bookmark { get; }
		
		[DataMember(Order = 10)]
		int PageNumber { get; set; }
		
		[DataMember(Order = 10)]
		int PageCount { get; }

		[DataMember(Order = 10)]
		string SheetNumber { get; set; }
		
		[DataMember(Order = 10)]
		Dictionary<string, IPdfTreeItem> ItemList { get; set; }

		bool ContainsBranch(string bookmark, out APdfTreeNode branch);
	}

	[DataContract(Namespace = "")]
	public abstract class APdfTreeNode : IPdfTreeItem
	{
		[DataMember(Order = 1)]
		public PdfTreeItemType ItemType { get; protected  set; }

		[DataMember(Order = 10)]
		public string Bookmark { get; protected set; }
		[DataMember(Order = 10)]
		public int PageNumber { get; set; }
		[DataMember(Order = 10)]
		public int PageCount { get; set; }
		[DataMember(Order = 10)]
		public string SheetNumber { get; set; }

		[DataMember(Order = 10)]
		public Dictionary<string, IPdfTreeItem> ItemList { get; set; }

		public APdfTreeNode()
		{
			ItemList = new Dictionary<string, IPdfTreeItem>();
		}

		// public APdfTreeNode(string bookmark)
		// {
		// 	ItemList = new Dictionary<string, IPdfTreeItem>();
		//
		// 	Bookmark = bookmark;
		// 	PageNumber = 0;
		// 	PageCount = 0;
		// }

		public bool AddNode(APdfTreeNode node)
		{
			return addNode(node);
		}

		public bool AddBranch(string bookmark, int pageNum, out APdfTreeNode branch)
		{
			branch = new PdfTreeBranch(bookmark);
			branch.PageNumber = pageNum;

			return addNode(branch);
		}

		private bool addNode(APdfTreeNode node)
		{
			if (ItemList.ContainsKey(node.Bookmark)) return false;

			ItemList.Add(node.Bookmark, node);

			return true;
		}

		public bool ContainsBranch(string bookmark, out APdfTreeNode branch)
		{
			IPdfTreeItem item;

			branch = null;

			if (!ItemList.TryGetValue(bookmark, out item)) return false;

			branch = (APdfTreeNode) item;

			return true;
		}

		public bool ContainsBranch(string bookmark)
		{
			// IPdfTreeItem item;
			//
			APdfTreeNode branch = null;
			//
			// if (!ItemList.TryGetValue(bookmark, out item)) return false;

			return ContainsBranch(bookmark, out branch);
		}
	}

	public abstract class APdfTreeNodeEx : APdfTreeNode
	{
		// public new string SheetNumber { get; set; }
		public string SheetName { get; set; }

		public TextAndLineSegmentData SheetNumberTals { get; set; }
		public string SheetNameExtracted { get; set; }

		public List<TextAndLineSegmentData> PageLinkData { get; set; }

		public bool SheetNumbersMisMatch { get; set; }
		public bool SheetNumberIsTemp { get; set; }
	}

	[DataContract(Namespace = "")]
	public class PdfTreeBranch : APdfTreeNode
	{
		public PdfTreeBranch(){}

		public PdfTreeBranch(string bookmark) 
		{
			Bookmark = bookmark;
			ItemType = PdfTreeItemType.PT_BRANCH;
		}

		public override string ToString()
		{
			return $"Branch| {Bookmark}";
		}
	}

	[DataContract(Namespace = "")]
	public class PdfTreeLeaf : APdfTreeNodeEx, ICloneable
	{
		public PdfTreeLeaf() { }

		public PdfTreeLeaf(string bookmark,
			IFilePath file, int pageCount,
			string shtNum, string shtName
			)
			// : base(bookmark)
		{
			ItemType = PdfTreeItemType.PT_LEAF;

			this.File = file;
			this.PageCount = pageCount;

			Bookmark = bookmark;

			ItemList = new Dictionary<string, IPdfTreeItem>();

			IgnoreList = new bool[5];
			SheetBorderType = SheetBorderType.ST_NA;

			SheetNumber = shtNum;
			SheetName = shtName;

			PageNumber = 0;
			PageCount = 0;
		}

		[IgnoreDataMember]
		public IFilePath File { get; set; }

		[DataMember(Order = 10)]
		public string FilePath 
		{
			get => File.FullFilePath;
			set { }
		}

		// public float PageRotation { get; set; }

		public bool[] IgnoreList { get; set; }

		public bool AddXrLinks => IgnoreList?[0] ?? false;
		public bool AddBanner => IgnoreList?[1] ?? false;
		public bool AddAuthorLink => IgnoreList?[2] ?? false;
		public bool AddDisclaimer => IgnoreList?[3] ?? false;
		public bool AnnotateSheet => IgnoreList?[4] ?? false;

		public SheetBorderType SheetBorderType { get; set; }
		public SheetData SheetData { get; set; }

		public PdfTreeLeaf Clone()
		{
			PdfTreeLeaf copy = new PdfTreeLeaf(Bookmark, File, PageCount, SheetNumber, SheetName);

			copy.PageNumber = PageNumber;
			copy.IgnoreList = IgnoreList;
			copy.SheetBorderType = SheetBorderType;
			copy.SheetData = SheetData;

			return copy;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public override string ToString()
		{
			return $"Leaf| {Bookmark}| {SheetNumber} | {SheetName}";
		}
	}

	[DataContract(Namespace = "")]
	public class PdfTreeNode : PdfTreeLeaf
	{
		public PdfTreeNode(){}

		public PdfTreeNode(string bookmark,
			IFilePath file,
			int pageCount) //: base(bookmark, file, pageCount, null, null)
		{
			Bookmark = bookmark;
			File = file;
			PageCount = pageCount;

			ItemType = PdfTreeItemType.PT_NODE;
			
			parseSheetInfo();
		}

		[DataMember(Order = 10)]
		public int Level { get; set; }

		public bool EstIsSheet { get; set; }

		private void parseSheetInfo()
		{
			int pos = Bookmark.IndexOf(" - ");

			if (pos == -1) return;

			EstIsSheet = true;

			SheetNumber = Bookmark.Substring(0, pos).Trim();
			SheetName = Bookmark.Substring(pos + 3).Trim();
		}

		public override string ToString()
		{
			return $"Leaf| {Bookmark}| {SheetNumber} | {SheetName}";
		}
	}

	[DataContract(Namespace = "")]
	public class PdfTreeNodeFile : PdfTreeNode
	{
		public PdfTreeNodeFile(string bookmark,
			IFilePath file, int pageCount) 
			// : base(bookmark, file, pageCount)
		{
			Bookmark = bookmark;
			File = file;
			PageCount = pageCount;

			ItemType = PdfTreeItemType.PT_NODE_FILE;
		}

		public override string ToString()
		{
			return $"Leaf| {Bookmark}| file| {File.FileName}";
		}

	}

	public class PdfNodeTree
	{
		public const string ROOTNAME = "root";

	#region private fields

		private PdfTreeBranch root;
		private APdfTreeNode currNode;

		private int currPageNumber = 1;
		private int pdfNodePageNumber = 0;

	#endregion

	#region ctor

		public PdfNodeTree()
		{
			root = new PdfTreeBranch(ROOTNAME);
		}

	#endregion

	#region public properties

		public PdfTreeBranch Root => root;

		public int CurrentPageNumber => currPageNumber;

		public APdfTreeNode CurrentNode => currNode;

		public int PdfNodePageNumber => pdfNodePageNumber;

	#endregion

	#region private properties

	#endregion

	#region public methods

		public void SetPdfNodePageNumber()
		{
			pdfNodePageNumber = currPageNumber;
		}

		public void AddToPageNumber(int numPages)
		{
			currPageNumber += numPages;
		}

		public void UpdateCurrentNode(APdfTreeNode node)
		{
			currNode = node;
		}

		/// <summary>
		/// Adds a typical tree node, assigns the page number,
		/// and sets the next page number based on the nodes page count
		/// </summary>
		/// <param name="bookmarklist"></param>
		/// <param name="node"></param>
		public void AddNode(List<string> bookmarklist, APdfTreeNode node)
		{
			FindOrAddBranch(bookmarklist);

			node.PageNumber = currPageNumber;
			currPageNumber += node.PageCount;

			currNode.AddNode(node);

			currNode = node;
		}

		/// <summary>
		/// Finds the requested node, sets currNode,
		/// and assigns the page number as the current page number
		/// </summary>
		public void FindOrAddBranch(List<string> bookmarklist)
		{
			APdfTreeNode temp;
			currNode = null;

			int level = FindBranch(bookmarklist, out temp);

			if (level > 0)
			{
				currNode = temp;
				return;
			}

			for (int i = (level + 1) * -1 ; i < bookmarklist.Count-1; i++)
			{
				temp.AddBranch(bookmarklist[i], currPageNumber, out temp);
				// temp.PageNumber = currPageNumber;
			}

			temp.AddBranch(bookmarklist[ bookmarklist.Count - 1], currPageNumber, out currNode);

			// currNode.PageNumber = currPageNumber;
		}

		/// <summary>
		/// add the provided node as a child to the last
		/// node in the list
		/// </summary>
		public bool AddOutlineNode(
			List<string> bookmarklist, PdfTreeNode node, int relativePgNum)
		{
			APdfTreeNode temp;

			// find the parent node - here, all prior 
			// nodes have been mode.  if level returns a
			// number < 0 - fail
			if (FindBranch(bookmarklist, out temp) < 0) return false;

			node.PageNumber = pdfNodePageNumber + relativePgNum;

			// showTreeNodeInfo(bookmarklist, node);

			temp.AddNode(node);

			return true;
		}

		private void showTreeNodeInfo(List<string> bookmarklist, PdfTreeNode node)
		{
			string h = String.Empty;

			foreach (string s in bookmarklist)
			{
				h += $"\\{s}";
			}


			Debug.WriteLine($"{node.ItemType,-15}| {node.Level,4} | {node.Bookmark,-32}| {node.File.FileName,-30}|  {bookmarklist.Count,-3}| {h}");
		}

		public int FindBranch(List<string> bookmarkList, out APdfTreeNode branch)
		{
			int level = 1;
			branch = null;

			APdfTreeNode temp;
			APdfTreeNode currBranch = root;

			bool result = true;

			do
			{
				// at the end, found the branch
				// return the node
				if (level == bookmarkList.Count + 1)
				{
					branch = currBranch;
					return level;
				}

				// found a matching node
				// continue to look for the next
				if (currBranch.ContainsBranch(bookmarkList[level - 1], out temp))
				{
					currBranch = temp;
					level++;
				}
				else
				{
					// matching node not found - will
					// need to add nodes from this point down
					branch = currBranch;
					level = 0 - level;
					result = false;
				}
			}
			while (result);

			return level;
		}

		public int CountElements()
		{
			int[] temp = CountNodes();

			return temp[0] + temp[1];
		}

		public int[] CountNodes()
		{
			return countNodes(root);
		}

	#endregion

	#region private methods

		// [0] = # branches / [1] = # nodes
		private int[] countNodes(PdfTreeBranch branch)
		{
			int[] result = new int[2];

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in branch.ItemList)
			{
				if (kvp.Value.ItemType == PdfTreeItemType.PT_BRANCH)
				{
					result[0]++;

					int[] temp = countNodes((PdfTreeBranch) kvp.Value);

					result[0] += temp[0];
					result[1] += temp[1];
				}
				else
				{
					result[1]++;
				}
			}

			return result;
		}

	#endregion

	#region event consuming

	#endregion

	#region event publishing

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(PdfNodeTree)}";
		}

	#endregion
	}
}