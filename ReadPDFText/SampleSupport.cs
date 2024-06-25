#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CommonPdfCodeShCode;
using UtilityLibrary;

using static SharedCode.ShDataSupport.SheetBorderType;
using static CommonPdfCodeShCode.PdfTreeItemType;

#endregion

// user name: jeffs
// created:   2/11/2024 11:17:27 PM

namespace SharedCode.ShDataSupport
{
	public struct SampleData
	{
		public PdfTreeItemType Type { get; set; }
		public string String1 { get; set; }
		public string String2 { get; set; }
		public string String3 { get; set; }
		public SheetBorderType BorderBorderType { get; set; }
		// 0 = xref link | 1 = banner | 2 = author | 3 disclaimer
		public bool[] IgnoreFlags { get; set; }
		public int PageNum { get; set; }
		public int PageCount { get; set; }

		public SampleData( PdfTreeItemType type, int pageNum, int pageCount, string string1, string string2, string string3,
			SheetBorderType borderBorderType, bool[] ignoreFlags)
		{
			Type = type;
			String1 = string1;
			String2 = string2;
			String3 = string3;
			BorderBorderType = borderBorderType;
			IgnoreFlags = ignoreFlags;
			PageNum = pageNum;
			PageCount = pageCount;
		}

		public string SheetNumber => String2;
		public string SheetName => String3;
		public string SheetBookMark => $"{SheetNumber} - {SheetName}";

		public string FirstBranchBookMark => String1;
		public string SecondBranchBookMark => String2;

		public string NodeFileName => String3;

		public string NodeBookmark => String1;


	}

	public class SampleSupport
	{
		private static int tempShtNumIdx = 1;
		private const string TEMP_SHEET_NAME = "Un-named Sheet";

		private static Dictionary<SheetBorderType, SheetData> shtData;

		static SampleSupport()
		{
			// shtData = SheetConfig.SheetConfigData;
			shtData = ReadPDFText.ShtData;

			// processSampleList();
			// processSampleList2();
		}

		public static string fileLocation { get; set; } = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\PDF SOLUTIONS\_Samples\Coliseum";

		public static PdfNodeTree tree { get; set; }


		public static PdfNodeTree MakePdfTree()
		{
			return MakePdfTreeEx(fileNameListShort);
		}

		public static PdfNodeTree MakePdfTree2()
		{
			return MakePdfTreeEx(fileNameListLong);
		}

		public static PdfNodeTree MakePdfTreeEx(List<SampleData> sampleList)
		{
			SampleData sd;

			tree = new PdfNodeTree();

			APdfTreeNode root =tree.Root;
			APdfTreeNode branch = null;

			int j = 0;

			for (int i = 0; i < sampleList.Count; i++)
			{
				sd = sampleList[i];


				if (sd.Type == PT_BRANCH)
				{
					addBranch(sd, root, branch, out branch);
				}
				else
				if (sd.Type == PT_LEAF)
				{
					
					// Debug.WriteLine($"adding leaf  | {sd.SheetBookMark}");

					addLeaf(sd, branch);
				}
				else
				if (sd.Type == PT_NODE_FILE)
				{
					// Debug.WriteLine($"adding node_f| {sd.NodeBookmark}");

					addNodeFile(sd, branch, out branch);
				}
				else
				{
					// Debug.WriteLine($"adding node  | {sd.NodeBookmark}");

					addNode(sd, branch);

				}
			}

			return tree;
		}

		private static void addBranch(SampleData sd, APdfTreeNode root, APdfTreeNode currBranch, out APdfTreeNode result)
		{
			string topBranchName = sd.FirstBranchBookMark;
			string nextBranchName = sd.SecondBranchBookMark;

			result = null;

			// Debug.Write($"adding branch|");

			// case 1 - begining - initial branch is null - just add to root
			// case 2 - currBranch == topBranchname - just add second to currBranch
			// case 3 - currBranch != topBranchName - add topBranchName to root - then add second if applies

			if (currBranch!=null && !currBranch.Bookmark.Equals(topBranchName))
			{
				if (!root.ContainsBranch(topBranchName, out currBranch))
				{
					currBranch = null;
				}
			}

			if (currBranch==null)
			{
				// Debug.Write($"| {topBranchName}");

				root.AddBranch(topBranchName, sd.PageNum, out currBranch);
				// currBranch.PageNumber = sd.PageNum;
				result = currBranch;
			} 


			if (!nextBranchName.IsVoid())
			{
				// Debug.Write($"| {nextBranchName}");

				currBranch.AddBranch(nextBranchName, sd.PageNum, out result);
				// result.PageNumber = sd.PageNum;
			}

			// Debug.WriteLine(" ");

		}

		private static void addLeaf(SampleData sd, APdfTreeNode branch)
		{
			FilePath<FileNameSimple> filePath;
			string fileString;
			string bookmark;
			PdfTreeLeaf leaf;

			fileString = string.Concat(fileLocation, "\\", sd.String1);
			filePath = new FilePath<FileNameSimple>(fileString);
			leaf = new PdfTreeLeaf(sd.SheetBookMark, filePath, 1, sd.SheetNumber, sd.SheetName);
			leaf.IgnoreList = sd.IgnoreFlags;
			leaf.SheetBorderType = sd.BorderBorderType;
			leaf.PageNumber = sd.PageNum;
			leaf.PageCount = sd.PageCount;
			leaf.SheetNumber = sd.SheetNumber;
			leaf.SheetName = sd.SheetName;

			leaf.SheetData = shtData[sd.BorderBorderType];
			
			branch.AddNode(leaf);
		}

		private static SheetBorderType nodeFileType;

		private static void addNodeFile(SampleData sd, APdfTreeNode branch, out APdfTreeNode result)
		{
			result = branch;

			if (!branch.Bookmark.Equals(sd.FirstBranchBookMark))
			{
				result = tree.Root;
			}

			string fileString = string.Concat(fileLocation, "\\", sd.NodeFileName);
			FilePath<FileNameSimple> filePath = new FilePath<FileNameSimple>(fileString);

			PdfTreeNodeFile nodeFile = new PdfTreeNodeFile(sd.FirstBranchBookMark, filePath, 3);
			nodeFile.IgnoreList = sd.IgnoreFlags;
			nodeFile.SheetBorderType = sd.BorderBorderType;
			nodeFile.Level = 1;
			nodeFile.PageNumber = sd.PageNum;
			nodeFile.PageCount = sd.PageCount;
			nodeFile.SheetNumber = sd.SheetNumber;

			nodeFileType = sd.BorderBorderType;

			result.AddNode(nodeFile);

			tree.Root.ContainsBranch(sd.FirstBranchBookMark, out result);

		}

		private static void addNode(SampleData sd, APdfTreeNode branch)
		{
			PdfTreeNode node = new PdfTreeNode(sd.SheetBookMark, null, 0);
			node.IgnoreList = sd.IgnoreFlags;
			node.SheetBorderType = nodeFileType;
			node.PageNumber = sd.PageNum;
			node.PageCount = sd.PageCount;

			node.SheetNumber = sd.SheetNumber;

			if ( sd.SheetNumber.IsVoid())
			{
				node.SheetNumber = $"ZZ-{tempShtNumIdx:D5}";
				node.SheetNumberIsTemp = true;
			}

			node.SheetName = sd.SheetName;

			if (node.SheetName.IsVoid())
			{
				node.SheetName = TEMP_SHEET_NAME;
			}

			node.SheetData = shtData[nodeFileType];

			branch.AddNode(node);
		}

		private static void processSampleList()
		{
			int pageNumber = 1;
			SampleData sd;

			for (var i = 0; i < fileNameListShort.Count; i++)
			{
				// Debug.Write(".");

				sd = fileNameListShort[i];

				sd.PageNum = pageNumber;

				fileNameListShort[i] = sd;

				if (sd.Type == PT_NODE_FILE) continue;

				pageNumber += sd.PageCount;
			}

			// Debug.WriteLine("process sample short list done");

			// showSampleData();
		}

		private static void processSampleList2()
		{
			int pageNumber = 1;
			SampleData sd;

			for (var i = 0; i < fileNameListLong.Count; i++)
			{
				// Debug.Write(".");

				sd = fileNameListLong[i];

				sd.PageNum = pageNumber;

				fileNameListLong[i] = sd;

				if (sd.Type == PT_NODE_FILE) continue;

				pageNumber += sd.PageCount;
			}

			// Debug.WriteLine("process sample long list done");

			// showSampleData();
		}
		
		// 0 = xref link | 1 = banner | 2 = author | 3 disclaimer
		public static List<SampleData> fileNameListShort { get; } = new List<SampleData>
		{
			//                                                                                                                                                                     flags
			//                                                                                                                                                                     xrefs | ban'r | author | disclaimer
			new SampleData(PT_BRANCH       ,0, 0,       "General"                                                                         , "", ""                                             , ST_NA      , null),                                    
			new SampleData(PT_BRANCH       ,0, 0,       "General"                                                                         , "Cover Sheet", ""                                  , ST_NA      , null),									
			// new SampleData(PT_LEAF         ,0 ,1,       "C1.0-1 - EROSION CONTROL PLAN.pdf"                                               , "C1.0-1" , "EROSION CONTROL PLAN"                  , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0, 1,       "CS - COVER SHEET.pdf"                                                            , "CS", "COVER SHEET"                                , ST_AO_36X48, new bool[] {false, true, true, true}),	
			new SampleData(PT_BRANCH       ,0, 0,       "General"                                                                         , "Title Sheets", ""                                 , ST_NA      , null),									
			new SampleData(PT_LEAF         ,0, 1,       "T1.0-0 - TITLE SHEET.pdf"                                                        , "T1.0-0", "TITLE SHEET"                            , ST_AO_36X48, new bool[] {false, true, true, true}),	
			new SampleData(PT_LEAF         ,0, 1,       "T2.0 - GENERAL CONTRACTOR'S NOTES.pdf"                                           , "T2.0", "GENERAL CONTRACTOR'S NOTES"               , ST_AO_36X48, new bool[] {false, true, true, true}),	
			new SampleData(PT_LEAF         ,0, 1,       "T2.1 - GENERAL CONTRACTORS NOTES.pdf"                                            , "T2.1", "GENERAL CONTRACTORS NOTES"                , ST_AO_36X48, new bool[] {false, true, true, true}),	
			new SampleData(PT_BRANCH       ,0, 0,       "General"                                                                         , "Accessibility", ""                                , ST_NA      , null),									
			new SampleData(PT_LEAF         ,0, 1,       "T3.2 - TYPICAL BUILDING REQUIREMENTS.pdf"                                        , "T3.2", "TYPICAL BUILDING REQUIREMENTS"            , ST_AO_36X48, new bool[] {false, true, true, true}),	
			new SampleData(PT_LEAF         ,0, 1,       "T3.3 - TYPICAL BUILDING REQUIREMENTS.pdf"                                        , "T3.3", "TYPICAL BUILDING REQUIREMENTS"            , ST_AO_36X48, new bool[] {false, true, true, true}),	
			new SampleData(PT_NODE_FILE    ,0, 3,       "Green Sheets"                                                                    , "", "T5 Series.pdf"                                , ST_AO_36X48, new bool[] {false, true, true, true}),	
			new SampleData(PT_NODE         ,0, 1,       "CALGREEN CHECKLIST"                                                              , "", ""                                             , ST_NA, null),											
			new SampleData(PT_NODE         ,0, 1,       "T5.1 - CALGREEN CHECKLIST"                                                       , "T5.1", "CALGREEN CHECKLIST"                       , ST_NA, null),											
			new SampleData(PT_NODE         ,0, 1,       "T5.2 - CALGREEN CHECKLIST"                                                       , "T5.2", "CALGREEN CHECKLIST"                       , ST_NA, null),											
			new SampleData(PT_BRANCH       ,0, 0,       "Odd Sized Sheets"                                                                , "", ""                                             , ST_NA      , null),									
			new SampleData(PT_LEAF         ,0, 1,       "Cx0A - Preface Sheet1.pdf"                                                       , "C0A", "Preface1 Sheet"                            , ST_CS_11X17, new bool[] {false, true, true, true}),	
			new SampleData(PT_LEAF         ,0, 1,       "Cx0B - Preface Sheet2.pdf"                                                       , "C0B", "Preface2 Sheet"                            , ST_CS_11X17, new bool[] {false, true, true, true}),	
//			new SampleData(PT_LEAF         ,0, 1,       "Cx01 - Cover Sheet.pdf"                                                          , "Cx01", "Cover Sheet"                              , ST_CS_11X17, new bool[] {false, false, false, false}),	
//			new SampleData(PT_LEAF         ,0, 1,       "Cx02 - Demolition Plan.pdf"                                                      , "C02", "Demolition Plan"                           , ST_CS_11X17, new bool[] {false, false, false, false}),	
//			new SampleData(PT_LEAF         ,0, 1,       "Cx03 - Site Plan.pdf"                                                            , "C03", "Site Plan"                                 , ST_CS_11X17, new bool[] {false, false, false, false}),	
			new SampleData(PT_LEAF         ,0, 1,       "Cx04 - Details.pdf"                                                              , "C04", "Details"                                   , ST_CS_11X17, new bool[] {true, true, true, true}),	
			new SampleData(PT_BRANCH       ,0 ,0,       "Civil"                                                                           , "" , ""                                            , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "C0.0-1 - TITLE SHEET.pdf"                                                        , "C0.0-1" , "TITLE SHEET"                           , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C1.0-1 - EROSION CONTROL PLAN.pdf"                                               , "C1.0-1" , "EROSION CONTROL PLAN"                  , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C2.0-1 - ROUGH GRADING PLAN.pdf"                                                 , "C2.0-1" , "ROUGH GRADING PLAN"                    , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C2.1-1 - ROUGH GRADING SECTIONS.pdf"                                             , "C2.1-1" , "ROUGH GRADING SECTIONS"                , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C2.2-1 - PRECISE GRADING PLAN - SOUTH.pdf"                                       , "C2.2-1" , "PRECISE GRADING PLAN - SOUTH"          , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C2.2-2 - PRECISE GRADING PLAN - NORTH.pdf"                                       , "C2.2-2" , "PRECISE GRADING PLAN - NORTH"          , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C3.0-1 - UTILITY PLAN.pdf"                                                       , "C3.0-1" , "UTILITY PLAN"                          , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C3.1-1 - LOW IMPACT DEVELOPMENT PLAN - SOUTH.pdf"                                , "C3.1-1" , "LOW IMPACT DEVELOPMENT PLAN - SOUTH"   , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C4.0-1 - DETAILS.pdf"                                                            , "C4.0-1" , "DETAILS"                               , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0, 0,       "Architectural Sheets"                                                            , "", ""                                             , ST_NA      , null),									
			new SampleData(PT_BRANCH       ,0, 0,       "Architectural Sheets"                                                            , "Floor Plans", ""                                  , ST_NA      , null),									
			new SampleData(PT_LEAF         ,0, 1,       "A2.2-G - LEVEL G - OVERALL FLOOR PLAN.pdf"                                       , "A2.2-G", "LEVEL G - OVERALL FLOOR PLAN"           , ST_AO_36X48, new bool[] {true, true, true, true}),		
			new SampleData(PT_LEAF         ,0, 1,       "A2.2-GN - LEVEL G - FLOOR PLAN - NORTH.pdf"                                      , "A2.2-GN", "LEVEL G - FLOOR PLAN - NORTH"          , ST_AO_36X48, new bool[] {true, true, true, true}),		
			new SampleData(PT_LEAF         ,0, 1,       "A2.2-GS - LEVEL G - FLOOR PLAN - SOUTH.pdf"                                      , "A2.2-GS", "LEVEL G - FLOOR PLAN - SOUTH"          , ST_AO_36X48, new bool[] {true, true, true, true}),		
			new SampleData(PT_BRANCH       ,0, 0,       "Architectural Sheets"                                                            , "VT Sheets", ""                                    , ST_NA      , null),									
			new SampleData(PT_LEAF         ,0, 1,       "A5.0-0 - STAIR AND RAILING DETAILS.pdf"                                          , "A5.0-0", "STAIR AND RAILING DETAILS"              , ST_AO_36X48, new bool[] {true, true, true, true}),		
			new SampleData(PT_LEAF         ,0, 1,       "A5.0-1 - STAIR A07 - ENLARGED FLOOR PLANS.pdf"                                   , "A5.0-1", "STAIR A07 - ENLARGED FLOOR PLANS"       , ST_AO_36X48, new bool[] {true, true, true, true}),		
			new SampleData(PT_LEAF         ,0, 1,       "A5.0-2 - STAIR A11 & A12 -ENLARGED FLOOR PLANS.pdf"                              , "A5.0-2", "STAIR A11 & A12 -ENLARGED FLOOR"        , ST_AO_36X48, new bool[] {true, true, true, true}),		
			new SampleData(PT_LEAF         ,0, 1,       "A5.0-3 - STAIR F01 - ENLARGED FLOOR PLANS.pdf"                                   , "A5.0-3", "STAIR F01 - ENLARGED FLOOR PLANS"       , ST_AO_36X48, new bool[] {true, true, true, true}),		
			new SampleData(PT_LEAF         ,0, 1,       "A5.0-4 - STAIR F14 - ENLARGED FLOOR PLANS.pdf"                                   , "A5.0-4", "STAIR F14 - ENLARGED FLOOR PLANS"       , ST_AO_36X48, new bool[] {true, true, true, true}),		
			new SampleData(PT_LEAF         ,0, 1,       "A5.0-5 - STAIR J01 & L06 - ENLARGED FLOOR PLANS.pdf"                             , "A5.0-5", "STAIR J01 & L06 - ENLARGED FLOOR PLANS" , ST_AO_36X48, new bool[] {true, true, true, true}),		
			new SampleData(PT_LEAF         ,0, 1,       "A5.0-6 - STAIR L10 - ENLARGED FLOOR PLANS.pdf"                                   , "A5.0-6", "STAIR L10 - ENLARGED FLOOR PLANS"       , ST_AO_36X48, new bool[] {true, true, true, true}),
		};

		public static List<SampleData> fileNameListLong { get; } = new List<SampleData>
		{
			new SampleData(PT_BRANCH       ,0 ,0,       "General"                                                                         , "" , ""    , ST_NA, null),
			new SampleData(PT_BRANCH       ,0 ,0,       "General"                                                                         , "Cover Sheet" , "" , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "CS - COVER SHEET.pdf"                                                            , "CS" , "COVER SHEET" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "General"                                                                         , "Title Sheets" , "" , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "T1.0-0 - TITLE SHEET.pdf"                                                        , "T1.0-0" , "TITLE SHEET" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T2.0 - GENERAL CONTRACTOR'S NOTES.pdf"											  , "T2.0" , "GENERAL CONTRACTOR'S NOTES" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T2.1 - GENERAL CONTRACTORS NOTES.pdf"											  , "T2.1" , "GENERAL CONTRACTORS NOTES" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T3.0 - TYPICAL BUILDING REQUIREMENTS.pdf"                                        , "T3.0" , "TYPICAL BUILDING REQUIREMENTS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T3.1 - TYPICAL BUILDING REQUIREMENTS.pdf"                                        , "T3.1" , "TYPICAL BUILDING REQUIREMENTS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T3.2 - TYPICAL BUILDING REQUIREMENTS.pdf"                                        , "T3.2" , "TYPICAL BUILDING REQUIREMENTS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T3.3 - TYPICAL BUILDING REQUIREMENTS.pdf"                                        , "T3.3" , "TYPICAL BUILDING REQUIREMENTS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T3.4 - TYPICAL BUILDING REQUIREMENTS.pdf"                                        , "T3.4" , "TYPICAL BUILDING REQUIREMENTS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T4.0 - ADA ACCESSIBILITY REQUIREMENTS NOTE.pdf"                                  , "T4.0" , "ADA ACCESSIBILITY REQUIREMENTS NOTE" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T4.1 - ADA ACCESSIBILITY REQUIREMENTS NOTE.pdf"                                  , "T4.1" , "ADA ACCESSIBILITY REQUIREMENTS NOTE" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T4.2 - LADBS INFORMATION BULLETINS.pdf"                                          , "T4.2" , "LADBS INFORMATION BULLETINS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T4.3 - LADBS INFORMATION BULLETINS.pdf"                                          , "T4.3" , "LADBS INFORMATION BULLETINS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T4.4 - LADBS INFORMATION BULLETINS.pdf"                                          , "T4.4" , "LADBS INFORMATION BULLETINS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T4.5 - LADBS INFORMATION BULLETINS.pdf"                                          , "T4.5" , "LADBS INFORMATION BULLETINS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T4.6 - LADBS INFORMATION BULLETINS.pdf"                                          , "T4.6" , "LADBS INFORMATION BULLETINS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T5.0 - CALGREEN CHECKLIST.pdf"                                                   , "T5.0" , "CALGREEN CHECKLIST" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T5.1 - CALGREEN CHECKLIST.pdf"                                                   , "T5.1" , "CALGREEN CHECKLIST" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T5.2 - CALGREEN CHECKLIST.pdf"                                                   , "T5.2" , "CALGREEN CHECKLIST" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T5.3-1 - GREEN CODE.pdf"                                                         , "T5.3-1" , "GREEN CODE" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "T5.3-2 - GREEN CODE.pdf"                                                         , "T5.3-2" , "GREEN CODE" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Civil"                                                                           , "" , ""      , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "C0.0-1 - TITLE SHEET.pdf"                                                        , "C0.0-1" , "TITLE SHEET" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C1.0-1 - EROSION CONTROL PLAN.pdf"                                               , "C1.0-1" , "EROSION CONTROL PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C2.0-1 - ROUGH GRADING PLAN.pdf"                                                 , "C2.0-1" , "ROUGH GRADING PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C2.1-1 - ROUGH GRADING SECTIONS.pdf"                                             , "C2.1-1" , "ROUGH GRADING SECTIONS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C2.2-1 - PRECISE GRADING PLAN - SOUTH.pdf"                                       , "C2.2-1" , "PRECISE GRADING PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C2.2-2 - PRECISE GRADING PLAN - NORTH.pdf"                                       , "C2.2-2" , "PRECISE GRADING PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C3.0-1 - UTILITY PLAN.pdf"                                                       , "C3.0-1" , "UTILITY PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C3.1-1 - LOW IMPACT DEVELOPMENT PLAN - SOUTH.pdf"                                , "C3.1-1" , "LOW IMPACT DEVELOPMENT PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "C4.0-1 - DETAILS.pdf"                                                            , "C4.0-1" , "DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Landscape"                                                                       , "" , ""      , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "L0.0 - COVER SHEET & GENERAL NOTES.pdf"                                          , "L0.0" , "COVER SHEET & GENERAL NOTES" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L1.10 - HARDSCAPE PLAN -LEVEL 1.pdf"                                             , "L1.10" , "HARDSCAPE PLAN -LEVEL 1" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L1.11 - ENLARGED HARDSCAPE PLAN -LEVEL 1 SOUTH.pdf"                              , "L1.11" , "ENLARGED HARDSCAPE PLAN -LEVEL 1 SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L1.12 - ENLARGED HARDSCAPE PLAN -LEVEL 1 NORTH.pdf"                              , "L1.12" , "ENLARGED HARDSCAPE PLAN -LEVEL 1 NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L1.20 - HARDSCAPE PLAN -LEVEL 2.pdf"                                             , "L1.20" , "HARDSCAPE PLAN -LEVEL 2" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L1.21 - ENLARGED HARDSCAPE PLAN -LEVEL 2 SOUTH.pdf"                              , "L1.21" , "ENLARGED HARDSCAPE PLAN -LEVEL 2 SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L1.22 - ENLARGED HARDSCAPE PLAN -LEVEL 1 NORTH.pdf"                              , "L1.22" , "ENLARGED HARDSCAPE PLAN -LEVEL 1 NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L1.50 - HARDSCAPE PLAN -LEVEL 5.pdf"                                             , "L1.50" , "HARDSCAPE PLAN -LEVEL 5" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L1.51 - ENLARGED HARDSCAPE PLAN -LEVEL 5 SOUTH.pdf"                              , "L1.51" , "ENLARGED HARDSCAPE PLAN -LEVEL 5 SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L1.52 - ENLARGED HARDSCAPE PLAN -LEVEL 5 NORTH.pdf"                              , "L1.52" , "ENLARGED HARDSCAPE PLAN -LEVEL 5 NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L3.00 - COURTYARD SECTIONS.pdf"                                                  , "L3.00" , "COURTYARD SECTIONS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L3.01 - ROOFTOP SECTIONS.pdf"                                                    , "L3.01" , "ROOFTOP SECTIONS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L5.00 - DETAILS.pdf"                                                             , "L5.00" , "DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L8.00 - PLANTING SCHEDULE.pdf"                                                   , "L8.00" , "PLANTING SCHEDULE" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L8.11 - ENLARGED PLANTING PLAN -LEVEL 1 SOUTH.pdf"                               , "L8.11" , "ENLARGED PLANTING PLAN -LEVEL 1 SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L8.12 - ENLARGED PLANTING PLAN -LEVEL 1 NORTH.pdf"                               , "L8.12" , "ENLARGED PLANTING PLAN -LEVEL 1 NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L8.20 - PLANTING PLAN -LEVEL 2.pdf"                                              , "L8.20" , "PLANTING PLAN -LEVEL 2" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L8.21 - ENLARGED PLANTING PLAN -LEVEL 2 SOUTH.pdf"                               , "L8.21" , "ENLARGED PLANTING PLAN -LEVEL 2 SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L8.22 - ENLARGED PLANTING PLAN -LEVEL 2 NORTH.pdf"                               , "L8.22" , "ENLARGED PLANTING PLAN -LEVEL 2 NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L8.50 - PLANTING PLAN -LEVEL 5.pdf"                                              , "L8.50" , "PLANTING PLAN -LEVEL 5" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L8.51 - ENLARGED PLANTING PLAN -LEVEL 5 SOUTH.pdf"                               , "L8.51" , "ENLARGED PLANTING PLAN -LEVEL 5 SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L8.52 - ENLARGED PLANTING PLAN -LEVEL 5 NORTH.pdf"                               , "L8.52" , "ENLARGED PLANTING PLAN -LEVEL 5 NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "L9.00 - PLANTING DETAILS.pdf"                                                    , "L9.00" , "PLANTING DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Life-Safety" , "" , ""                                                                     , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-G - LEVEL G - OVERALL EGRESS PLAN.pdf"                                     , "LS2.2-G" , "LEVEL G - OVERALL EGRESS PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-M - LEVEL M - OVERALL EGRESS PLAN.pdf"                                     , "LS2.2-M" , "LEVEL M - OVERALL EGRESS PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-P1 - LEVEL P1 - OVERALL EGRESS PLAN.pdf"                                   , "LS2.2-P1" , "LEVEL P1 - OVERALL EGRESS PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-P2 - LEVEL 2 - OVERALL EGRESS PLAN.pdf"                                    , "LS2.2-P2" , "LEVEL 2 - OVERALL EGRESS PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-P3 - LEVEL 3 - OVERALL EGRESS PLAN.pdf"                                    , "LS2.2-P3" , "LEVEL 3 - OVERALL EGRESS PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-P4 - LEVEL 4 - OVERALL EGRESS PLAN.pdf"                                    , "LS2.2-P4" , "LEVEL 4 - OVERALL EGRESS PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-P5 - LEVEL 5 - OVERALL EGRESS PLAN.pdf"                                    , "LS2.2-P5" , "LEVEL 5 - OVERALL EGRESS PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-R1 - LEVEL R1 - EGRESS FLOOR PLAN.pdf"                                     , "LS2.2-R1" , "LEVEL R1 - EGRESS FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-R2 - LEVEL R2 - EGRESS FLOOR PLAN.pdf"                                     , "LS2.2-R2" , "LEVEL R2 - EGRESS FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-R3 - LEVEL R3 - EGRESS FLOOR PLAN.pdf"                                     , "LS2.2-R3" , "LEVEL R3 - EGRESS FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-R4 - LEVEL R4 - EGRESS FLOOR PLAN.pdf"                                     , "LS2.2-R4" , "LEVEL R4 - EGRESS FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-R5 - LEVEL R5 - EGRESS FLOOR PLAN.pdf"                                     , "LS2.2-R5" , "LEVEL R5 - EGRESS FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS2.2-R6 - LEVEL R6 - EGRESS ROOF PLAN.pdf"                                      , "LS2.2-R6" , "LEVEL R6 - EGRESS ROOF PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "LS3.0-1 - FIREWALL AREA PLAN (R1-R5).pdf"                                        , "LS3.0-1"  , "FIREWALL AREA PLAN (R1-R5)" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Architectural" , ""   , ""                                                                    , ST_NA, null),
			new SampleData(PT_BRANCH       ,0 ,0,       "Architectural", "Assembly Details" , ""                                                       , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "A0.1-1 - EXTERIOR WALL ASSEMBLY TYPES.pdf"                                       , "A0.1-1" , "EXTERIOR WALL ASSEMBLY TYPES" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A0.2-1 - INTERIOR  WALL ASSEMBLY TYPES.pdf"                                      , "A0.2-1" , "INTERIOR  WALL ASSEMBLY TYPES" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A0.3-1 - FLOOR ASSEMBLY TYPES.pdf"                                               , "A0.3-1" , "FLOOR ASSEMBLY TYPES" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Architectural" , "Site" , ""                                                                , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "A1.0-0 - OVERALL SITE VICINITY PLAN.pdf"                                         , "A1.0-0" , "OVERALL SITE VICINITY PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A1.1-0 - SITE PLAN.pdf"                                                          , "A1.1-0" , "SITE PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A1.1-1 - ENLARGED PLAN EAST DRIVEWAY.pdf"                                        , "A1.1-1" , "ENLARGED PLAN EAST DRIVEWAY" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A1.1-2 - WEST DRIVEWAY.pdf"                                                      , "A1.1-2" , "WEST DRIVEWAY" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Architectural" , "Plans" , ""                                                               , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-G - LEVEL G - OVERALL FLOOR PLAN.pdf"                                       , "A2.2-G" , "LEVEL G - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-GN - LEVEL G - FLOOR PLAN - NORTH.pdf"                                      , "A2.2-GN" , "LEVEL G - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-GS - LEVEL G - FLOOR PLAN - SOUTH.pdf"                                      , "A2.2-GS" , "LEVEL G - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-M - LEVEL M - OVERALL FLOOR PLAN.pdf"                                       , "A2.2-M" , "LEVEL M - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-MN - LEVEL M - FLOOR PLAN - NORTH.pdf"                                      , "A2.2-MN" , "LEVEL M - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-MS - LEVEL M - FLOOR PLAN - SOUTH.pdf"                                      , "A2.2-MS" , "LEVEL M - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P1 - LEVEL P1 - OVERALL FLOOR PLAN.pdf"                                     , "A2.2-P1" , "LEVEL P1 - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P1N - LEVEL P1 - FLOOR PLAN - NORTH.pdf"                                    , "A2.2-P1N" , "LEVEL P1 - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P1S - LEVEL P1 - FLOOR PLAN - SOUTH.pdf"                                    , "A2.2-P1S" , "LEVEL P1 - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P2 - LEVEL 2 - OVERALL FLOOR PLAN.pdf"                                      , "A2.2-P2" , "LEVEL 2 - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P2N - LEVEL P2 - FLOOR PLAN - NORTH.pdf"                                    , "A2.2-P2N" , "LEVEL P2 - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P2S - LEVEL P2 - FLOOR PLAN - SOUTH.pdf"                                    , "A2.2-P2S" , "LEVEL P2 - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P3 - LEVEL 3 - OVERALL FLOOR PLAN.pdf"                                      , "A2.2-P3" , "LEVEL 3 - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P3N - LEVEL P3 - FLOOR PLAN - NORTH.pdf"                                    , "A2.2-P3N" , "LEVEL P3 - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P3S - LEVEL P3 - FLOOR PLAN - SOUTH.pdf"                                    , "A2.2-P3S" , "LEVEL P3 - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P4 - LEVEL 4 - OVERALL FLOOR PLAN.pdf"                                      , "A2.2-P4" , "LEVEL 4 - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P4N - LEVEL P4 - FLOOR PLAN - NORTH.pdf"                                    , "A2.2-P4N" , "LEVEL P4 - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P4S - LEVEL P4 - FLOOR PLAN - SOUTH.pdf"                                    , "A2.2-P4S" , "LEVEL P4 - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P5 - LEVEL 5 - OVERALL FLOOR PLAN.pdf"                                      , "A2.2-P5" , "LEVEL 5 - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P5N - LEVEL P5 - FLOOR PLAN - NORTH.pdf"                                    , "A2.2-P5N" , "LEVEL P5 - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-P5S - LEVEL P5 - FLOOR PLAN - SOUTH.pdf"                                    , "A2.2-P5S" , "LEVEL P5 - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R1 - LEVEL R1 - OVERALL FLOOR PLAN.pdf"                                     , "A2.2-R1" , "LEVEL R1 - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R1N - LEVEL R1 - FLOOR PLAN - NORTH.pdf"                                    , "A2.2-R1N" , "LEVEL R1 - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R1S - LEVEL R1 - FLOOR PLAN - SOUTH.pdf"                                    , "A2.2-R1S" , "LEVEL R1 - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R2 - LEVEL R2 - OVERALL FLOOR PLAN.pdf"                                     , "A2.2-R2" , "LEVEL R2 - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R2N - LEVEL R2 - FLOOR PLAN - NORTH.pdf"                                    , "A2.2-R2N" , "LEVEL R2 - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R2S - LEVEL R2 - FLOOR PLAN - SOUTH.pdf"                                    , "A2.2-R2S" , "LEVEL R2 - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R3 - LEVEL R3 - OVERALL FLOOR PLAN.pdf"                                     , "A2.2-R3" , "LEVEL R3 - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R3N - LEVEL R3 - FLOOR PLAN - NORTH.pdf"                                    , "A2.2-R3N" , "LEVEL R3 - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R3S - LEVEL R3 - FLOOR PLAN - SOUTH.pdf"                                    , "A2.2-R3S" , "LEVEL R3 - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R4 - LEVEL R4 - OVERALL FLOOR PLAN.pdf"                                     , "A2.2-R4" , "LEVEL R4 - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R4N - LEVEL R4 - FLOOR PLAN - NORTH.pdf"                                    , "A2.2-R4N" , "LEVEL R4 - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R4S - LEVEL R4 - FLOOR PLAN - SOUTH.pdf"                                    , "A2.2-R4S" , "LEVEL R4 - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R5 - LEVEL R5 - OVERALL FLOOR PLAN.pdf"                                     , "A2.2-R5" , "LEVEL R5 - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R5N - LEVEL R5 - FLOOR PLAN - NORTH.pdf"                                    , "A2.2-R5N" , "LEVEL R5 - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R5S - LEVEL R5 - FLOOR PLAN - SOUTH.pdf"                                    , "A2.2-R5S" , "LEVEL R5 - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R6 - LEVEL R6 - OVERALL FLOOR PLAN.pdf"                                     , "A2.2-R6" , "LEVEL R6 - OVERALL FLOOR PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R6N - LEVEL R6 - FLOOR PLAN - NORTH.pdf"                                    , "A2.2-R6N" , "LEVEL R6 - FLOOR PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A2.2-R6S - LEVEL R6 - FLOOR PLAN - SOUTH.pdf"                                    , "A2.2-R6S" , "LEVEL R6 - FLOOR PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Architectural" , "Elevations and Sections" , ""                                             , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "A3.0-1 - SOUTH ELEVATION.pdf"                                                    , "A3.0-1" , "SOUTH ELEVATION" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A3.0-2 - EAST ELEVATION.pdf"                                                     , "A3.0-2" , "EAST ELEVATION" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A3.0-3 - NORTH ELEVATION.pdf"                                                    , "A3.0-3" , "NORTH ELEVATION" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A3.0-4 - WEST ELEVATION.pdf"                                                     , "A3.0-4" , "WEST ELEVATION" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A3.2-1 - BUILDING SECTION A NORTH TO SOUTH.pdf"                                  , "A3.2-1" , "BUILDING SECTION A NORTH TO SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A3.2-2 - BUILDING SECTION B EAST TO WEST.pdf"                                    , "A3.2-2" , "BUILDING SECTION B EAST TO WEST" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A3.2-3 - BUILDING SECTION C EAST TO WEST.pdf"                                    , "A3.2-3" , "BUILDING SECTION C EAST TO WEST" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Architectural" , "Vertical Circulation" , ""                                                , ST_NA, null),
			new SampleData(PT_BRANCH       ,0 ,0,       "Architectural" , "Vertical Circulation" , "Stairs"                                          , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.0-0 - STAIR AND RAILING DETAILS.pdf"                                          , "A5.0-0" , "STAIR AND RAILING DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.0-1 - STAIR A07 - ENLARGED FLOOR PLANS.pdf"                                   , "A5.0-1" , "STAIR A07 - ENLARGED FLOOR PLANS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.0-2 - STAIR A11 & A12 -ENLARGED FLOOR PLANS.pdf"                              , "A5.0-2" , "STAIR A11 & A12 -ENLARGED FLOOR PLANS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.0-3 - STAIR F01 - ENLARGED FLOOR PLANS.pdf"                                   , "A5.0-3" , "STAIR F01 - ENLARGED FLOOR PLANS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.0-4 - STAIR F14 - ENLARGED FLOOR PLANS.pdf"                                   , "A5.0-4" , "STAIR F14 - ENLARGED FLOOR PLANS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.0-5 - STAIR J01 & L06 - ENLARGED FLOOR PLANS.pdf"                             , "A5.0-5" , "STAIR J01 & L06 - ENLARGED FLOOR PLANS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.0-6 - STAIR L10 - ENLARGED FLOOR PLANS.pdf"                                   , "A5.0-6" , "STAIR L10 - ENLARGED FLOOR PLANS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.0-7 - STAIR L14 & G15 -ENLARGED FLOOR PLANS.pdf"                              , "A5.0-7" , "STAIR L14 & G15 -ENLARGED FLOOR PLANS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.0-8 - STAIRS A05, A08, A09, E02, L02, & L08 - ENLARGED FLOOR PLANS.pdf"       , "A5.0-8" , "STAIRS A05, A08, A09, E02, L02, & L08 - ENLARGED FLOOR PLANS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Architectural" , "Vertical Circulation" , "Elevators"                                       , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.3-0 - DEFERRED ELEVATOR DETAILS.pdf"                                          , "A5.3-0" , "DEFERRED ELEVATOR DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Architectural" , "Vertical Circulation" , ""                                                , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.7-1 - PEDESTRIAN RAMPS L11 AND L12.pdf"                                       , "A5.7-1" , "PEDESTRIAN RAMPS L11 AND L12" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.8-1 - VEHICLE RAMP AB01 & AB-09.pdf"                                          , "A5.8-1" , "VEHICLE RAMP AB01 & AB-09" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.8-2 - VEHICLE RAMP AB09.pdf"                                                  , "A5.8-2" , "VEHICLE RAMP AB09" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.8-3 - VEHICLE RAMP AB13.pdf"                                                  , "A5.8-3" , "VEHICLE RAMP AB13" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.8-4 - VEHICLE RAMP M01.pdf"                                                   , "A5.8-4" , "VEHICLE RAMP M01" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A5.8-5 - VEHICLE RAMP L15.pdf"                                                   , "A5.8-5" , "VEHICLE RAMP L15" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Architectural" , "Schedules" , ""                                                           , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "A6.1-1 - DOOR SCHEDULE.pdf"                                                      , "A6.1-1" , "DOOR SCHEDULE" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A6.1-2 - DOOR SCHEDULE, DOOR TYPES & NOTES.pdf" , "A6.1-2"                       , "DOOR SCHEDULE, DOOR TYPES & NOTES" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A6.3-1 - WINDOW DETAILS.pdf"                                                     , "A6.3-1" , "WINDOW DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A6.3-2 - STOREFRONT & CANOPY DETAILS.pdf"                                        , "A6.3-2" , "STOREFRONT & CANOPY DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Architectural", "Details" , ""                                                              , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "A7.1-1 - INTERIOR DETAILS.pdf"                                                   , "A7.1-1" , "INTERIOR DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A7.3-1 - FLAT ROOF DETAILS.pdf"                                                  , "A7.3-1" , "FLAT ROOF DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A7.3-2 - FLAT ROOF DETAILS.pdf"                                                  , "A7.3-2" , "FLAT ROOF DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A8.0-1 - ASSEMBLY DETAILS.pdf"                                                   , "A8.0-1" , "ASSEMBLY DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "A8.0-2 - WINDOW DETAILS.pdf"                                                     , "A8.0-2" , "WINDOW DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Shoring" , "" , ""                                                                          , ST_NA, null),
			new SampleData(PT_BRANCH       ,0 ,0,       "Shoring" , "Plans" , ""                                                                     , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "SH-1 - OVER ALL SHORING PLAN.pdf"                                                , "SH-1" , "OVER ALL SHORING PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "SH-1A - SHORING PLAN - NORTH.pdf"                                                , "SH-1A" , "SHORING PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "SH-1B - SHORING PLAN - SOUTH.pdf"                                                , "SH-1B" , "SHORING PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "SH-2 - GENERAL NOTES, DESIGN CRITERIA & SCHEDULE.pdf"                            , "SH-2" , "GENERAL NOTES, DESIGN CRITERIA & SCHEDULE" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "SH-2.1 - SCHEDULE.pdf"                                                           , "SH-2.1" , "SCHEDULE" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "SH-3 - TYPICAL DETAILS.pdf"                                                      , "SH-3" , "TYPICAL DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Shoring"                                                                         , "Elevations and Sections" , "" , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "SH-4 - ELEVATIONS AND SECTION.pdf"                                               , "SH-4" , "ELEVATIONS AND SECTION" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "SH-5 - ELEVATIONS AND SECTION.pdf"                                               , "SH-5" , "ELEVATIONS AND SECTION" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "SH-6 - ELEVATIONS AND SECTION.pdf"                                               , "SH-6" , "ELEVATIONS AND SECTION" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "SH-7 - ELEVATIONS AND SECTION.pdf"                                               , "SH-7" , "ELEVATIONS AND SECTION" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Structural"                                                                      , "" , ""        , ST_NA, null),
			new SampleData(PT_BRANCH       ,0 ,0,       "Structural"                                                                      , "General Notes" , "" , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "S0.0-1 - GENERAL NOTES.pdf"                                                      , "S0.0-1" , "GENERAL NOTES" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S0.0-2 - GENERAL NOTES.pdf"                                                      , "S0.0-2" , "GENERAL NOTES" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Structural"                                                                      , "Typical Details" , "" , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "S0.1-1 - TYPICAL DETAILS CONCRETE.pdf"                                           , "S0.1-1" , "TYPICAL DETAILS CONCRETE" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S0.1-2 - TYPICAL DETAILS CONCRETE.pdf"                                           , "S0.1-2" , "TYPICAL DETAILS CONCRETE" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S0.2-1 - TYPICAL DETAILS CMU.pdf"                                                , "S0.2-1" , "TYPICAL DETAILS CMU" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S0.3-1 - TYPICAL DETAILS STEEL.pdf"                                              , "S0.3-1" , "TYPICAL DETAILS STEEL" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S0.3-2 - TYPICAL DETAILS STEEL.pdf"                                              , "S0.3-2" , "TYPICAL DETAILS STEEL" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S0.4-1 - TYPICAL DETAILS ANCHOR IN CONC & CMU.pdf"                               , "S0.4-1" , "TYPICAL DETAILS ANCHOR IN CONC & CMU" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S0.5-1 - TYPICAL DETAILS POST TENSION.pdf"                                       , "S0.5-1" , "TYPICAL DETAILS POST TENSION" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S0.5-2 - TYPICAL DETAILS POST TENSION.pdf"                                       , "S0.5-2" , "TYPICAL DETAILS POST TENSION" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Structural"                                                                      , "Plans" , ""   , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.1-G - LEVEL G - LOAD MAP.pdf"                                                 , "S2.1-G" , "LEVEL G - LOAD MAP" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.1-M - LEVEL M - LOAD MAP.pdf"                                                 , "S2.1-M" , "LEVEL M - LOAD MAP" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.1-P1 - LEVEL P1 - LOAD MAP.pdf"                                               , "S2.1-P1" , "LEVEL P1 - LOAD MAP" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.1-P2 - LEVEL P2 - LOAD MAP.pdf"                                               , "S2.1-P2" , "LEVEL P2 - LOAD MAP" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.1-P3 - LEVEL P3 - LOAD MAP.pdf"                                               , "S2.1-P3" , "LEVEL P3 - LOAD MAP" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.1-P4 - LEVEL P4 - LOAD MAP.pdf"                                               , "S2.1-P4" , "LEVEL P4 - LOAD MAP" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.1-P5 - LEVEL P5 - LOAD MAP.pdf"                                               , "S2.1-P5" , "LEVEL P5 - LOAD MAP" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.1-R - LEVEL R - LOAD MAP.pdf"                                                 , "S2.1-R" , "LEVEL R - LOAD MAP" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2 -P2S - LEVEL P2 FRAMING PLAN - SOUTH.pdf"                                   , "S2.2 -P2S" , "LEVEL P2 FRAMING PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-GN - LEVEL G FRAMING PLAN - NORTH.pdf"                                      , "S2.2-GN" , "LEVEL G FRAMING PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-GNA - LEVEL G ADDITIONAL REINFORCING PLAN (N-S) - NORTH.pdf"                , "S2.2-GNA" , "LEVEL G ADDITIONAL REINFORCING PLAN (N-S) - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-GNB - LEVEL G ADDITIONAL REINFORCING PLAN (E-W) - NORTH.pdf"                , "S2.2-GNB" , "LEVEL G ADDITIONAL REINFORCING PLAN (E-W) - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-GNC - LEVEL G POST TENSION PLAN -NORTH.pdf"                                 , "S2.2-GNC" , "LEVEL G POST TENSION PLAN -NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-GS - LEVEL G FRAMING PLAN - SOUTH.pdf"                                      , "S2.2-GS" , "LEVEL G FRAMING PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-GSA - LEVEL G ADDITIONAL REINFORCING PLAN (N-S) - SOUTH.pdf"                , "S2.2-GSA" , "LEVEL G ADDITIONAL REINFORCING PLAN (N-S) - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-GSB - LEVEL G ADDITIONAL REINFORCING PLAN (E-W) - SOUTH.pdf"                , "S2.2-GSB" , "LEVEL G ADDITIONAL REINFORCING PLAN (E-W) - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-GSC - LEVEL G POST TENSION PLAN -SOUTH.pdf"                                 , "S2.2-GSC" , "LEVEL G POST TENSION PLAN -SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-MN - LEVEL M FRAMING  PLAN - NORTH.pdf"                                     , "S2.2-MN" , "LEVEL M FRAMING  PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-MS - LEVEL M  FRAMING  PLAN - SOUTH.pdf"                                    , "S2.2-MS" , "LEVEL M  FRAMING  PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P1N - LEVEL P1 FRAMING PLAN - NORTH.pdf"                                    , "S2.2-P1N" , "LEVEL P1 FRAMING PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P1NA - LEVEL P1 ADDITIONAL NORTH-SOUTH REINFORCING PLAN - NORTH.pdf"        , "S2.2-P1NA" , "LEVEL P1 ADDITIONAL NORTH-SOUTH REINFORCING PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P1NB - LEVEL P1 ADDITIONAL EAST-WEST REINFORCING PLAN - NORTH.pdf"          , "S2.2-P1NB" , "LEVEL P1 ADDITIONAL EAST-WEST REINFORCING PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P1S - LEVEL P1 FRAMING PLAN - SOUTH.pdf"                                    , "S2.2-P1S" , "LEVEL P1 FRAMING PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P1SA - LEVEL P1 ADDITIONAL NORTH-SOUTH REINFORCING PLAN - SOUTH.pdf"        , "S2.2-P1SA" , "LEVEL P1 ADDITIONAL NORTH-SOUTH REINFORCING PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P1SB - LEVEL P1 ADDITIONAL EAST-WEST REINFORCING PLAN - SOUTH.pdf"          , "S2.2-P1SB" , "LEVEL P1 ADDITIONAL EAST-WEST REINFORCING PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P2N - LEVEL P2 FRAMING PLAN - NORTH.pdf"                                    , "S2.2-P2N" , "LEVEL P2 FRAMING PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P2NA - LEVEL P2 ADDITIONAL NORTH-SOUTH REINFORCING PLAN - NORTH.pdf"        , "S2.2-P2NA" , "LEVEL P2 ADDITIONAL NORTH-SOUTH REINFORCING PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P2NB - LEVEL P2 ADDITIONAL EAST-WEST REINFORCING PLAN - NORTH.pdf"          , "S2.2-P2NB" , "LEVEL P2 ADDITIONAL EAST-WEST REINFORCING PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P2SA - LEVEL P2 ADDITIONAL NORTH-SOUTH REINFORCING PLAN - SOUTH.pdf"        , "S2.2-P2SA" , "LEVEL P2 ADDITIONAL NORTH-SOUTH REINFORCING PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P2SB - LEVEL P2 ADDITIONAL EAST-WEST REINFORCING PLAN - SOUTH.pdf"          , "S2.2-P2SB" , "LEVEL P2 ADDITIONAL EAST-WEST REINFORCING PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P3N - LEVEL P3 FRAMING PLAN - NORTH.pdf"                                    , "S2.2-P3N" , "LEVEL P3 FRAMING PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P3NA - LEVEL P3 ADDITIONAL REINF. PLAN (NORTH-SOUTH) -NORTH.pdf"            , "S2.2-P3NA" , "LEVEL P3 ADDITIONAL REINF. PLAN (NORTH-SOUTH) -NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P3NB - LEVEL P3 ADDITIONAL REINF. PLAN (EAST-WEST) - NORTH.pdf"             , "S2.2-P3NB" , "LEVEL P3 ADDITIONAL REINF. PLAN (EAST-WEST) - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P3S - LEVEL P3 FRAMING PLAN - SOUTH.pdf"                                    , "S2.2-P3S" , "LEVEL P3 FRAMING PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P3SA - LEVEL P3 ADDITIONAL REINF. PLAN (NORTH-SOUTH) -SOUTH.pdf"            , "S2.2-P3SA" , "LEVEL P3 ADDITIONAL REINF. PLAN (NORTH-SOUTH) -SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P3SB - LEVEL P3 ADDITIONAL REINF. PLAN (EAST-WEST) - SOUTH.pdf"             , "S2.2-P3SB" , "LEVEL P3 ADDITIONAL REINF. PLAN (EAST-WEST) - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P4N - LEVEL P4 FRAMING PLAN - NORTH.pdf"                                    , "S2.2-P4N" , "LEVEL P4 FRAMING PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P4NA - LEVEL P4 ADDITIONAL REINF. PLAN (EAST-WEST) - NORTH.pdf"             , "S2.2-P4NA" , "LEVEL P4 ADDITIONAL REINF. PLAN (EAST-WEST) - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P4NB - LEVEL P4 ADDITIONAL REINF. PLAN (E-W)-NORTH.pdf"                     , "S2.2-P4NB" , "LEVEL P4 ADDITIONAL REINF. PLAN (E-W)-NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P4S - LEVEL P4 FRAMING PLAN - SOUTH.pdf"                                    , "S2.2-P4S" , "LEVEL P4 FRAMING PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P4SA - LEVEL P4 ADDITIONAL REINF. PLAN (EAST-WEST) - SOUTH.pdf"             , "S2.2-P4SA" , "LEVEL P4 ADDITIONAL REINF. PLAN (EAST-WEST) - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P4SB - LEVEL P4 ADDITIONAL REINF. PLAN (E-W)- SOUTH.pdf"                    , "S2.2-P4SB" , "LEVEL P4 ADDITIONAL REINF. PLAN (E-W)- SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P5N - LEVEL P5 FOUANDATION PLAN - NORTH.pdf"                                , "S2.2-P5N" , "LEVEL P5 FOUANDATION PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-P5S - LEVEL P5 FOUNDATION PLAN - SOUTH.pdf"                                 , "S2.2-P5S" , "LEVEL P5 FOUNDATION PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-RNA - LEVEL R ADDITIONAL REINFORCING PLAN (N-S) - NORTH.pdf"                , "S2.2-RNA" , "LEVEL R ADDITIONAL REINFORCING PLAN (N-S) - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-RNB - LEVEL R ADDITIONAL REINFORCING PLAN (E-W) - NORTH.pdf"                , "S2.2-RNB" , "LEVEL R ADDITIONAL REINFORCING PLAN (E-W) - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-RNC - LEVEL R - POST TENSIONING PLAN - NORTH.pdf"                           , "S2.2-RNC" , "LEVEL R - POST TENSIONING PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-RSA - LEVEL R ADDITIONAL REINFORCING PLAN (N-S) - SOUTH.pdf"                , "S2.2-RSA" , "LEVEL R ADDITIONAL REINFORCING PLAN (N-S) - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-RSB - LEVEL R ADDITIONAL REINFORCING PLAN (E-W) - SOUTH.pdf"                , "S2.2-RSB" , "LEVEL R ADDITIONAL REINFORCING PLAN (E-W) - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.2-RSC - LEVEL R - POST TENSIONING PLAN - SOUTH.pdf"                           , "S2.2-RSC" , "LEVEL R - POST TENSIONING PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.3-RN - LEVEL R FRAMING  PLAN - NORTH.pdf"                                     , "S2.3-RN" , "LEVEL R FRAMING  PLAN - NORTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S2.3-RS - LEVEL R1 FRAMING  PLAN - SOUTH.pdf"                                    , "S2.3-RS" , "LEVEL R1 FRAMING  PLAN - SOUTH" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Structural"                                                                      , "Column Schedules" , "" , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "S3.0-1 - CONC. COLUMNS SCHEDULE AND DETAILS.pdf"                                 , "S3.0-1" , "CONC. COLUMNS SCHEDULE AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S3.0-2 - CONC. COLUMN  DETAILS.pdf"                                              , "S3.0-2" , "CONC. COLUMN  DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Structural"                                                                      , "Basement Sections and Details" , "" , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "S3.1-1 - BASEMENT WALL SECTIONS AND DETAILS.pdf"                                 , "S3.1-1" , "BASEMENT WALL SECTIONS AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S3.1-2 - BASEMENT WALL SECTIONS AND DETAILS.pdf"                                 , "S3.1-2" , "BASEMENT WALL SECTIONS AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S3.1-3 - BASEMENT WALL SECTIONS AND DETAILS.pdf"                                 , "S3.1-3" , "BASEMENT WALL SECTIONS AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S3.2-1 - MAT SLAB SECTIONS AND DETAILS.pdf"                                      , "S3.2-1" , "MAT SLAB SECTIONS AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S3.3-1 - CONC. SHEAR WALL SCHEDULE AND DETAILS.pdf"                              , "S3.3-1" , "CONC. SHEAR WALL SCHEDULE AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "Structural"                                                                      , "Conc. Slab Sections and Details" , "" , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "S4.1-1 - CONC SLAB SECTIONS AND DETAILS.pdf"                                     , "S4.1-1" , "CONC SLAB SECTIONS AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S4.1-2 - CONC SLAB SECTIONS AND DETAILS.pdf"                                     , "S4.1-2" , "CONC SLAB SECTIONS AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S4.1-3 - CONC SLAB SECTIONS AND DETAILS.pdf"                                     , "S4.1-3" , "CONC SLAB SECTIONS AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S4.2-1 - CONC. BEAM SCHEDULE AND DETAILS.pdf"                                    , "S4.2-1" , "CONC. BEAM SCHEDULE AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S4.2-2 - CONC. BEAM SCHEDULE AND DETAILS.pdf"                                    , "S4.2-2" , "CONC. BEAM SCHEDULE AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S4.2-3 - CONC. BEAM SECTION AND DETAILS.pdf"                                     , "S4.2-3" , "CONC. BEAM SECTION AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S4.3-1 - PT CONC. BEAM SCHEDULE AND DETAILS.pdf"                                 , "S4.3-1" , "PT CONC. BEAM SCHEDULE AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "S4.3-2 - PT SECTIONS AND DETAILS.pdf"                                            , "S4.3-2" , "PT SECTIONS AND DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_BRANCH       ,0 ,0,       "EBM"                                                                             , "" , ""        , ST_NA, null),
			new SampleData(PT_LEAF         ,0 ,1,       "EBM0.0 - GENERAL NOTES.pdf"                                                      , "EBM0.0" , "GENERAL NOTES" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "EBM2.2-RF - COMPOSITE ROOF PLAN.pdf"                                             , "EBM2.2-RF" , "COMPOSITE ROOF PLAN" , ST_AO_36X48, new bool[] {true, true, true, true}),
			new SampleData(PT_LEAF         ,0 ,1,       "EBM9.0 - DETAILS.pdf"                                                            , "EBM9.0" , "DETAILS" , ST_AO_36X48, new bool[] {true, true, true, true}),

		};

		public static List<string> fileNameList { get; } = new List<string>()
		{
			"Cx01 - Cover Sheet.pdf",
			"Cx02 - Demolition Plan.pdf",
			"Cx03 - Site Plan.pdf",
			"2.2-P1NA - LEVEL P1 ADDITIONAL NORTH-SOUTH REINFORCING PLAN - NORTH.pdf",
			"2.2-P1NB - LEVEL P1 ADDITIONAL EAST-WEST REINFORCING PLAN - NORTH.pdf",
			"2.2-P1SA - LEVEL P1 ADDITIONAL NORTH-SOUTH REINFORCING PLAN - SOUTH.pdf",
			"2.2-P1SB - LEVEL P1 ADDITIONAL EAST-WEST REINFORCING PLAN - SOUTH.pdf",
			"2.2-P2NA - LEVEL P2 ADDITIONAL NORTH-SOUTH REINFORCING PLAN - NORTH.pdf",
			"2.2-P2NB - LEVEL P2 ADDITIONAL EAST-WEST REINFORCING PLAN - NORTH.pdf",
			"2.2-P2SA - LEVEL P2 ADDITIONAL NORTH-SOUTH REINFORCING PLAN - SOUTH.pdf",
			"2.2-P2SB - LEVEL P2 ADDITIONAL EAST-WEST REINFORCING PLAN - SOUTH.pdf",
			"2.2-P3NA - LEVEL P3 ADDITIONAL REINF. PLAN (NORTH-SOUTH) -NORTH.pdf",
			"2.2-P3NB - LEVEL P3 ADDITIONAL REINF. PLAN (EAST-WEST) - NORTH.pdf",
			"2.2-P3SA - LEVEL P3 ADDITIONAL REINF. PLAN (NORTH-SOUTH) -SOUTH.pdf",
			"2.2-P3SB - LEVEL P3 ADDITIONAL REINF. PLAN (EAST-WEST) - SOUTH.pdf",
			"2.2-P4NB - LEVEL P4 ADDITIONAL REINF. PLAN (E-W)-NORTH.pdf",
			"2.2-P4SA - LEVEL P4 ADDITIONAL REINF. PLAN (EAST-WEST) - SOUTH.pdf",
			"2.2-P4SB - LEVEL P4 ADDITIONAL REINF. PLAN (E-W)- SOUTH.pdf",
			"A0.1-1 - EXTERIOR WALL ASSEMBLY TYPES.pdf",
			"A0.2-1 - INTERIOR  WALL ASSEMBLY TYPES.pdf",
			"A0.3-1 - FLOOR ASSEMBLY TYPES.pdf",
			"A1.0-0 - OVERALL SITE VICINITY PLAN.pdf",
			"A1.1-0 - SITE PLAN.pdf",
			"A1.1-1 - ENLARGED PLAN EAST DRIVEWAY.pdf",
			"A1.1-2 - WEST DRIVEWAY.pdf",
			"A2.2-G - LEVEL G - OVERALL FLOOR PLAN.pdf",
			"A2.2-GN - LEVEL G - FLOOR PLAN - NORTH.pdf",
			"A2.2-GS - LEVEL G - FLOOR PLAN - SOUTH.pdf",
			"A2.2-M - LEVEL M - OVERALL FLOOR PLAN.pdf",
			"A2.2-MN - LEVEL M - FLOOR PLAN - NORTH.pdf",
			"A2.2-MS - LEVEL M - FLOOR PLAN - SOUTH.pdf",
			"A2.2-P1 - LEVEL P1 - OVERALL FLOOR PLAN.pdf",
			"A2.2-P1N - LEVEL P1 - FLOOR PLAN - NORTH.pdf",
			"A2.2-P1S - LEVEL P1 - FLOOR PLAN - SOUTH.pdf",
			"A2.2-P2 - LEVEL 2 - OVERALL FLOOR PLAN.pdf",
			"A2.2-P2N - LEVEL P2 - FLOOR PLAN - NORTH.pdf",
			"A2.2-P2S - LEVEL P2 - FLOOR PLAN - SOUTH.pdf",
			"A2.2-P3 - LEVEL 3 - OVERALL FLOOR PLAN.pdf",
			"A2.2-P3N - LEVEL P3 - FLOOR PLAN - NORTH.pdf",
			"A2.2-P3S - LEVEL P3 - FLOOR PLAN - SOUTH.pdf",
			"A2.2-P4 - LEVEL 4 - OVERALL FLOOR PLAN.pdf",
			"A2.2-P4N - LEVEL P4 - FLOOR PLAN - NORTH.pdf",
			"A2.2-P4S - LEVEL P4 - FLOOR PLAN - SOUTH.pdf",
			"A2.2-P5 - LEVEL 5 - OVERALL FLOOR PLAN.pdf",
			"A2.2-P5N - LEVEL P5 - FLOOR PLAN - NORTH.pdf",
			"A2.2-P5S - LEVEL P5 - FLOOR PLAN - SOUTH.pdf",
			"A2.2-R1 - LEVEL R1 - OVERALL FLOOR PLAN.pdf",
			"A2.2-R1N - LEVEL R1 - FLOOR PLAN - NORTH.pdf",
			"A2.2-R1S - LEVEL R1 - FLOOR PLAN - SOUTH.pdf",
			"A2.2-R2 - LEVEL R2 - OVERALL FLOOR PLAN.pdf",
			"A2.2-R2N - LEVEL R2 - FLOOR PLAN - NORTH.pdf",
			"A2.2-R2S - LEVEL R2 - FLOOR PLAN - SOUTH.pdf",
			"A2.2-R3 - LEVEL R3 - OVERALL FLOOR PLAN.pdf",
			"A2.2-R3N - LEVEL R3 - FLOOR PLAN - NORTH.pdf",
			"A2.2-R3S - LEVEL R3 - FLOOR PLAN - SOUTH.pdf",
			"A2.2-R4 - LEVEL R4 - OVERALL FLOOR PLAN.pdf",
			"A2.2-R4N - LEVEL R4 - FLOOR PLAN - NORTH.pdf",
			"A2.2-R4S - LEVEL R4 - FLOOR PLAN - SOUTH.pdf",
			"A2.2-R5 - LEVEL R5 - OVERALL FLOOR PLAN.pdf",
			"A2.2-R5N - LEVEL R5 - FLOOR PLAN - NORTH.pdf",
			"A2.2-R5S - LEVEL R5 - FLOOR PLAN - SOUTH.pdf",
			"A2.2-R6 - LEVEL R6 - OVERALL FLOOR PLAN.pdf",
			"A2.2-R6N - LEVEL R6 - FLOOR PLAN - NORTH.pdf",
			"A2.2-R6S - LEVEL R6 - FLOOR PLAN - SOUTH.pdf",
			"A3.0-1 - SOUTH ELEVATION.pdf",
			"A3.0-2 - EAST ELEVATION.pdf",
			"A3.0-3 - NORTH ELEVATION.pdf",
			"A3.0-4 - WEST ELEVATION.pdf",
			"A3.2-1 - BUILDING SECTION A NORTH TO SOUTH.pdf",
			"A3.2-2 - BUILDING SECTION B EAST TO WEST.pdf",
			"A3.2-3 - BUILDING SECTION C EAST TO WEST.pdf",
			"A5.0-0 - STAIR AND RAILING DETAILS.pdf",
			"A5.0-1 - STAIR A07 - ENLARGED FLOOR PLANS.pdf",
			"A5.0-2 - STAIR A11 & A12 -ENLARGED FLOOR PLANS.pdf",
			"A5.0-3 - STAIR F01 - ENLARGED FLOOR PLANS.pdf",
			"A5.0-4 - STAIR F14 - ENLARGED FLOOR PLANS.pdf",
			"A5.0-5 - STAIR J01 & L06  -ENLARGED FLOOR PLANS.pdf",
			"A5.0-6 - STAIR L10 - ENLARGED FLOOR PLANS.pdf",
			"A5.0-7 - STAIR L14 & G15 -ENLARGED FLOOR PLANS.pdf",
			"A5.0-8 - STAIRS A05, A08, A09, E02, L02, & L08 - ENLARGED FLOOR PLANS.pdf",
			"A5.3-0 - DEFERRED ELEVATOR DETAILS.pdf",
			"A5.7-1 - PEDESTRIAN RAMPS L11 AND L12.pdf",
			"A5.8-1 - VEHICLE RAMP AB01 & AB-09.pdf",
			"A5.8-2 - VEHICLE RAMP AB09.pdf",
			"A5.8-3 - VEHICLE RAMP AB13.pdf",
			"A5.8-4 - VEHICLE RAMP M01.pdf",
			"A5.8-5 - VEHICLE RAMP L15.pdf",
			"A6.1-1 - DOOR SCHEDULE.pdf",
			"A6.1-1 - INTERIOR DETAILS.pdf",
			"A6.1-2 - DOOR SCHEDULE, DOOR TYPES & NOTES.pdf",
			"A6.3-1 - WINDOW DETAILS.pdf",
			"A6.3-2 - STOREFRONT & CANOPY DETAILS.pdf",
			"A7.3-1 - FLAT ROOF DETAILS.pdf",
			"A7.3-2 - FLAT ROOF DETAILS.pdf",
			"A8.0-1 - ASSEMBLY DETAILS.pdf",
			"A8.0-2 - WINDOW DETAILS.pdf",
			"C0.0-1 - TITLE SHEET.pdf",
			"C1.0-1 - EROSION CONTROL PLAN.pdf",
			"C2.0-1 - ROUGH GRADING PLAN.pdf",
			"C2.1-1 - ROUGH GRADING SECTIONS.pdf",
			"C2.2-1 - PRECISE GRADING PLAN - SOUTH.pdf",
			"C2.2-2 - PRECISE GRADING PLAN - NORTH.pdf",
			"C3.0-1 - UTILITY PLAN.pdf",
			"C3.1-1 - LOW IMPACT DEVELOPMENT PLAN - SOUTH.pdf",
			"C4.0-1 - DETAILS.pdf",
			"CS - COVER SHEET.pdf",
			"EBM0.0 - GENERAL NOTES.pdf",
			"EBM2.2-RF - COMPOSITE ROOF PLAN.pdf",
			"EBM9.0 - DETAILS.pdf",
			"L0.0 - COVER SHEET & GENERAL NOTES.pdf",
			"L1.10 - HARDSCAPE PLAN -LEVEL 1.pdf",
			"L1.11 - ENLARGED HARDSCAPE PLAN -LEVEL 1 SOUTH.pdf",
			"L1.12 - ENLARGED HARDSCAPE PLAN -LEVEL 1 NORTH.pdf",
			"L1.20 - HARDSCAPE PLAN -LEVEL 2.pdf",
			"L1.21 - ENLARGED HARDSCAPE PLAN -LEVEL 2 SOUTH.pdf",
			"L1.22 - ENLARGED HARDSCAPE PLAN -LEVEL 1 NORTH.pdf",
			"L1.50 - HARDSCAPE PLAN -LEVEL 5.pdf",
			"L1.51 - ENLARGED HARDSCAPE PLAN -LEVEL 5 SOUTH.pdf",
			"L1.52 - ENLARGED HARDSCAPE PLAN -LEVEL 5 NORTH.pdf",
			"L3.00 - COURTYARD SECTIONS.pdf",
			"L3.01 - ROOFTOP SECTIONS.pdf",
			"L5.00 - DETAILS.pdf",
			"L8.00 - PLANTING SCHEDULE.pdf",
			"L8.11 - ENLARGED PLANTING PLAN -LEVEL 1 SOUTH.pdf",
			"L8.12 - ENLARGED PLANTING PLAN -LEVEL 1 NORTH.pdf",
			"L8.20 - PLANTING PLAN -LEVEL 2.pdf",
			"L8.21 - ENLARGED PLANTING PLAN -LEVEL 2 SOUTH.pdf",
			"L8.22 - ENLARGED PLANTING PLAN -LEVEL 2 NORTH.pdf",
			"L8.50 - PLANTING PLAN -LEVEL 5.pdf",
			"L8.51 - ENLARGED PLANTING PLAN -LEVEL 5 SOUTH.pdf",
			"L8.52 - ENLARGED PLANTING PLAN -LEVEL 5 NORTH.pdf",
			"L9.00 - PLANTING DETAILS.pdf",
			"LS2.2-G - LEVEL G - OVERALL EGRESS PLAN.pdf",
			"LS2.2-M - LEVEL M - OVERALL EGRESS PLAN.pdf",
			"LS2.2-P1 - LEVEL P1 - OVERALL EGRESS PLAN.pdf",
			"LS2.2-P2 - LEVEL 2 - OVERALL EGRESS PLAN.pdf",
			"LS2.2-P3 - LEVEL 3 - OVERALL EGRESS PLAN.pdf",
			"LS2.2-P4 - LEVEL 4 - OVERALL EGRESS PLAN.pdf",
			"LS2.2-P5 - LEVEL 5 - OVERALL EGRESS PLAN.pdf",
			"LS2.2-R1 - LEVEL R1 - EGRESS FLOOR PLAN.pdf",
			"LS2.2-R2 - LEVEL R2 - EGRESS FLOOR PLAN.pdf",
			"LS2.2-R3 - LEVEL R3 - EGRESS FLOOR PLAN.pdf",
			"LS2.2-R4 - LEVEL R4 - EGRESS FLOOR PLAN.pdf",
			"LS2.2-R5 - LEVEL R5 - EGRESS FLOOR PLAN.pdf",
			"LS2.2-R6 - LEVEL R6 - EGRESS ROOF PLAN.pdf",
			"LS3.0-1 - FIREWALL AREA PLAN (R1-R5).pdf",
			"S0.0-1 - GENERAL NOTES.pdf",
			"S0.0-2 - GENERAL NOTES.pdf",
			"S0.1-1 - TYPICAL DETAILS CONCRETE.pdf",
			"S0.1-2 - TYPICAL DETAILS CONCRETE.pdf",
			"S0.2-1 - TYPICAL DETAILS CMU.pdf",
			"S0.3-1 - TYPICAL DETAILS STEEL.pdf",
			"S0.3-2 - TYPICAL DETAILS STEEL.pdf",
			"S0.4-1 - TYPICAL DETAILS ANCHOR IN CONC & CMU.pdf",
			"S0.5-1 - TYPICAL DETAILS POST TENSION.pdf",
			"S0.5-2 - TYPICAL DETAILS POST TENSION.pdf",
			"S2.1-G - LEVEL G - LOAD MAP.pdf",
			"S2.1-M - LEVEL M - LOAD MAP.pdf",
			"S2.1-P1 - LEVEL P1 - LOAD MAP.pdf",
			"S2.1-P2 - LEVEL P2 - LOAD MAP.pdf",
			"S2.1-P3 - LEVEL P3 - LOAD MAP.pdf",
			"S2.1-P4 - LEVEL P4 - LOAD MAP.pdf",
			"S2.1-P5 - LEVEL P5 - LOAD MAP.pdf",
			"S2.1-R - LEVEL R - LOAD MAP.pdf",
			"S2.2 -P2S - LEVEL P2 FRAMING PLAN - SOUTH.pdf",
			"S2.2-GN - LEVEL G FRAMING PLAN - NORTH.pdf",
			"S2.2-GNA - LEVEL G ADDITIONAL REINFORCING PLAN (N-S) - NORTH.pdf",
			"S2.2-GNB - LEVEL G ADDITIONAL REINFORCING PLAN (E-W) - NORTH.pdf",
			"S2.2-GNC - LEVEL G POST TENSION PLAN -NORTH.pdf",
			"S2.2-GS - LEVEL G FRAMING PLAN - SOUTH.pdf",
			"S2.2-GSA - LEVEL G ADDITIONAL REINFORCING PLAN (N-S) - SOUTH.pdf",
			"S2.2-GSB - LEVEL G ADDITIONAL REINFORCING PLAN (E-W) - SOUTH.pdf",
			"S2.2-GSC - LEVEL G POST TENSION PLAN -SOUTH.pdf",
			"S2.2-MN - LEVEL M FRAMING  PLAN - NORTH.pdf",
			"S2.2-MS - LEVEL M  FRAMING  PLAN - SOUTH.pdf",
			"S2.2-P1N - LEVEL P1 FRAMING PLAN - NORTH.pdf",
			"S2.2-P1S - LEVEL P1 FRAMING PLAN - SOUTH.pdf",
			"S2.2-P2N - LEVEL P2 FRAMING PLAN - NORTH.pdf",
			"S2.2-P3N - LEVEL P3 FRAMING PLAN - NORTH.pdf",
			"S2.2-P3S - LEVEL P3 FRAMING PLAN - SOUTH.pdf",
			"S2.2-P4N - LEVEL P4 FRAMING PLAN - NORTH.pdf",
			"S2.2-P4NA - LEVEL P4 ADDITIONAL REINF. PLAN (EAST-WEST) - NORTH.pdf",
			"S2.2-P4S - LEVEL P4 FRAMING PLAN - SOUTH.pdf",
			"S2.2-P5N - LEVEL P5 FOUANDATION PLAN - NORTH.pdf",
			"S2.2-P5S - LEVEL P5 FOUNDATION PLAN - SOUTH.pdf",
			"S2.2-RNA - LEVEL R ADDITIONAL REINFORCING PLAN (N-S) - NORTH.pdf",
			"S2.2-RNB - LEVEL R ADDITIONAL REINFORCING PLAN (E-W) - NORTH.pdf",
			"S2.2-RNC - LEVEL R - POST TENSIONING PLAN - NORTH.pdf",
			"S2.2-RSA - LEVEL R ADDITIONAL REINFORCING PLAN (N-S) - SOUTH.pdf",
			"S2.2-RSB - LEVEL R ADDITIONAL REINFORCING PLAN (E-W) - SOUTH.pdf",
			"S2.2-RSC - LEVEL R - POST TENSIONING PLAN - SOUTH.pdf",
			"S2.3-RN - LEVEL R FRAMING  PLAN - NORTH.pdf",
			"S2.3-RS - LEVEL R1 FRAMING  PLAN - SOUTH.pdf",
			"S3.0-1 - CONC. COLUMNS SCHEDULE AND DETAILS.pdf",
			"S3.0-2 - CONC. COLUMN  DETAILS.pdf",
			"S3.1-1 - BASEMENT WALL SECTIONS AND DETAILS.pdf",
			"S3.1-2 - BASEMENT WALL SECTIONS AND DETAILS.pdf",
			"S3.1-3 - BASEMENT WALL SECTIONS AND DETAILS.pdf",
			"S3.2-1 - MAT SLAB SECTIONS AND DETAILS.pdf",
			"S3.3-1 - CONC. SHEAR WALL SCHEDULE AND DETAILS.pdf",
			"S4.1-1 - CONC SLAB SECTIONS AND DETAILS.pdf",
			"S4.1-2 - CONC SLAB SECTIONS AND DETAILS.pdf",
			"S4.1-3 - CONC SLAB SECTIONS AND DETAILS.pdf",
			"S4.2-1 - CONC. BEAM SCHEDULE AND DETAILS.pdf",
			"S4.2-2 - CONC. BEAM SCHEDULE AND DETAILS.pdf",
			"S4.2-3 - CONC. BEAM SECTION AND DETAILS.pdf",
			"S4.3-1 - PT CONC. BEAM SCHEDULE AND DETAILS.pdf",
			"S4.3-2 - PT SECTIONS AND DETAILS.pdf",
			"SH-1 - OVER ALL SHORING PLAN.pdf",
			"SH-1A - SHORING PLAN - NORTH.pdf",
			"SH-1B - SHORING PLAN - SOUTH.pdf",
			"SH-2 - GENERAL NOTES, DESIGN CRITERIA & SCHEDULE.pdf",
			"SH-2.1 - SCHEDULE.pdf",
			"SH-3 - TYPICAL DETAILS.pdf",
			"SH-4 - ELEVATIONS AND SECTION.pdf",
			"SH-5 - ELEVATIONS AND SECTION.pdf",
			"SH-6 - ELEVATIONS AND SECTION.pdf",
			"SH-7 - ELEVATIONS AND SECTION.pdf",
			"T1.0-0 - TITLE SHEET.pdf",
			"T2.0 - GENERAL CONTRACTOR'S NOTES GENERAL CONSTRUCTION AND DEMOLITION NOTES.pdf",
			"T2.1 - GENERAL CONTRACTORS NOTES GENERAL CONSTRUCTION AND DEMOLITION NOTES.pdf",
			"T3.0 - TYPICAL BUILDING REQUIREMENTS.pdf",
			"T3.1 - TYPICAL BUILDING REQUIREMENTS.pdf",
			"T3.2 - TYPICAL BUILDING REQUIREMENTS.pdf",
			"T3.3 - TYPICAL BUILDING REQUIREMENTS.pdf",
			"T3.4 - TYPICAL BUILDING REQUIREMENTS.pdf",
			"T4.0 - ADA ACCESSIBILITY REQUIREMENTS NOTE.pdf",
			"T4.1 - ADA ACCESSIBILITY REQUIREMENTS NOTE.pdf",
			"T4.2 - LADBS INFORMATION BULLETINS.pdf",
			"T4.3 - LADBS INFORMATION BULLETINS.pdf",
			"T4.4 - LADBS INFORMATION BULLETINS.pdf",
			"T4.5 - LADBS INFORMATION BULLETINS.pdf",
			"T4.6 - LADBS INFORMATION BULLETINS.pdf",
			"T5.0 - CALGREEN CHECKLIST.pdf",
			"T5.1 - CALGREEN CHECKLIST.pdf",
			"T5.2 - CALGREEN CHECKLIST.pdf",
			"T5.3-1 - GREEN CODE.pdf",
			"T5.3-2 - GREEN CODE.pdf"
		};

		public override string ToString()
		{
			return $"this is {nameof(SampleSupport)}";
		}


		private static int level = 0;
		private static int margMulti = 2;


		public static void showSampleDataShort()
		{
			showSampleData(fileNameListShort);
		}

		public static void showSampleDataLong()
		{
			showSampleData(fileNameListLong);
		}



		private static void showSampleData(List<SampleData> sampleData)
		{
			Debug.WriteLine("\n\n*** show sample data ***\n");

			foreach (SampleData sd in sampleData)
			{
				string pn = sd.PageNum == 0 ? "" : $"pg| {sd.PageNum,3}";
				string pc = sd.PageCount == 0 ? "" : $"pg cnt| {sd.PageCount,3}";

				Debug.Write($"type| {sd.Type,-14}| {pn,-7} | {pc,-12} | {sd.String1,-55}| {sd.String2,-14}| {sd.String3,-42}| {sd.BorderBorderType,-13}");

				if (sd.IgnoreFlags != null)
				{
					Debug.Write($"| h-links?| {sd.IgnoreFlags[0]}| banner?| {sd.IgnoreFlags[1]}| author?| {sd.IgnoreFlags[2]}| disclaimer?| {sd.IgnoreFlags[3]}");

				}

				Debug.Write("\n");

			}

			Debug.WriteLine("*** show sample data done ***\n");
		}

		public static void showPdfNodeTree(PdfNodeTree tree)
		{
			level = 0 ;

			sb = new StringBuilder();

			Debug.WriteLine("\n\n*** show pdf node tree A ***");
			sb.Append("\n\n*** show pdf node tree B ***\n");

			showPdfTreeNodes(tree.Root);

			sb.Append("*** show pdf node tree done B ***\n");
			Debug.WriteLine("*** show pdf node tree A done ***");

			Debug.WriteLine(sb.ToString());
		}

		private static StringBuilder sb = new StringBuilder();

		// when showing typical nodes
		private static void showPdfTreeNodes(APdfTreeNode node)
		{
			if (!node.Bookmark.Equals(PdfNodeTree.ROOTNAME))
			{
				showPdfTreeBranch(node);
			}

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in node.ItemList)
			{
				if (kvp.Value.ItemType == PdfTreeItemType.PT_BRANCH)
				{
					level++;

					showPdfTreeNodes((APdfTreeNode) kvp.Value);

					level--;
				} 
				else 
				if (kvp.Value.ItemType == PdfTreeItemType.PT_NODE)
				{
					showPdfNodeLeaf((PdfTreeNode) kvp.Value);
				} 
				else 
				if (kvp.Value.ItemType == PdfTreeItemType.PT_NODE_FILE)
				{
					level++;
					showPdfNodes2((PdfTreeNode) kvp.Value, 1);
					level--;
				} 
				else  // node leaf
				{
					showPdfNodeLeaf((APdfTreeNode) kvp.Value);
				}
			}
		}

		// private void showPdfTreeItem(APdfTreeNode node)
		// {
		// 	string n1 = node.ItemList.ToString();
		// 	string n2 = node.Bookmark.ToString();
		// 	string n3 = node.PageCount.ToString();
		// 	string n4 = node.ItemType.ToString();
		// 	string n6 = node.PageNumber.ToString();
		//
		// }


		private static void showPdfTreeBranch(APdfTreeNode node)
		{
			string t = "B";

			string preface =
			$"level| {level,-3}| ->  {node.PageNumber,-3} {"  ".Repeat((level - 1) * margMulti)}{node.Bookmark}";
			Debug.WriteLine($"{t,-3}{preface}");

			sb.Append($"A type| {node.ItemType,-14}| ->  {node.PageNumber,-3}| list count| {node.ItemList.Count,2}|                 bkmrk| {node.Bookmark}\n");
		}



		private static void showPdfNodeLeaf(APdfTreeNode node)
		{
			string h;
			string t = "?";
			string f = String.Empty;
			string preface = $"       |    | pg| {node.PageNumber,-3} {" ".Repeat((level + 2) * margMulti)}{node.Bookmark}";

			if (node.ItemType == PdfTreeItemType.PT_LEAF)
			{
				t = "L";
				f = $"| file| {( (PdfTreeLeaf) node).File.FileName}";
			} 
			else if (node.ItemType == PdfTreeItemType.PT_NODE || node.ItemType == PdfTreeItemType.PT_NODE_FILE)
			{
				t = "N";
			}

			Debug.WriteLine($"{t}{preface,-90}| pg cnt| {node.PageCount,-3}{f}");

			sb.Append($"B type| {node.ItemType,-14}| pg| {node.PageNumber,-3}| list count| {node.ItemList.Count,2}| pg count| {node.PageCount,2}|   bkmrk| {node.Bookmark}\n");

		}



		// specific when showing bookmarks from a compiled pdf file
		// shows the whole "branch"

		private static void showPdfNodes2(PdfTreeNode node, int level)
		{
			showPdfNode2(node, level);

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in node.ItemList)
			{
				showPdfNodes2((PdfTreeNode) kvp.Value, level + 1);
			}
		}

		private static void showPdfNode2(PdfTreeNode node, int level)
		{
			if (node.ItemType == PdfTreeItemType.PT_NODE)
			{
				showPdfNodeItem2(node, level);
			} 
			else 
			if (node.ItemType == PdfTreeItemType.PT_NODE_FILE)
			{
				showPdfNodeFile2(node, level);
			}

		}

		private static void showPdfNodeItem2(PdfTreeNode node, int level)
		{
			string t = "N";
			// string f = node.File.FileName;

			string b = node.Bookmark;
			string b1 = "bkmk only";
			string sht = "";

			if (node.EstIsSheet)
			{
				b1 = "* est pg bkmk";
				sht = $"({node.SheetNumber} - {node.SheetName})";
			}
			
			string preface =
				$"     |    | pg| {node.PageNumber,-3}{"  ".Repeat((level-1) * margMulti)}{node.Bookmark}";

			Debug.WriteLine($"{t,-3}{preface}");

			sb.Append($"E type| {node.ItemType,-14}| pg| {node.PageNumber,-3}| list count| {node.ItemList.Count,2}| {b1, -14}| bkmrk| {b}  {sht}\n");

		}

		private static void showPdfNodeFile2(PdfTreeNode node, int level)
		{
			string t = "F";
			string f = node.File.FileName;
			
			string preface =
				$"level| {node.Level,-3}| ->  {node.PageNumber,-3} {"  ".Repeat((level-1) * margMulti)}{node.Bookmark}";

			Debug.WriteLine($"{t,-3}{preface,-87} | pg cnt| {node.PageCount,-3}| file| {f}");

			sb.Append($"F type| {node.ItemType,-14}| ->  {node.PageNumber,-3}| list count| {node.ItemList.Count,2}| pg count| {node.PageCount,2}| bkmrk| {node.Bookmark}\n");

		}

		
		public static void ShowPdfNodeTreeInOrder()
		{
			Debug.WriteLine("\n\n*** show pdf node tree in order ***\n");

			level = 0;
			showPdfNodeTreeInOrder(tree.Root);

			Debug.WriteLine("\n*** show pdf node tree in order done ***\n");
		}

		public static void showPdfNodeTreeInOrder(APdfTreeNode node)
		{
			if (node.ItemType == PT_LEAF || node.ItemType == PT_NODE) return;

			Debug.Print($"{node.Bookmark}");
			string margin;

			level++;

			margin = " ".Repeat(level * 2);


			foreach (KeyValuePair<string, IPdfTreeItem> kvp in node.ItemList)
			{
				Debug.Print($"{margin}{kvp.Value.Bookmark}");
			}

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in node.ItemList)
			{
				showPdfNodeTreeInOrder((APdfTreeNode) kvp.Value);
			}

			level--;
		}




		/* not applicable
		private static void showPdfNodes(PdfTreeNode node)
		{
			showPdfNode(node);

			foreach (KeyValuePair<string, IPdfTreeItem> kvp in node.ItemList)
			{
				showPdfNodes((PdfTreeNode) kvp.Value);
			}
		}

		private static void showPdfNode(PdfTreeNode node)
		{
			if (node.ItemType == PdfTreeItemType.PT_NODE)
			{
				showPdfNodeItem(node);
			} 
			else 
			if (node.ItemType == PdfTreeItemType.PT_NODE_FILE)
			{
				showPdfNodeFile(node);
			}
		}

		private static void showPdfNodeFile(PdfTreeNode node)
		{
			string t = "F";
			string f = node.File.FileName;
			
			string preface =
				$"level| {node.Level,-3}| pg| {node.PageNumber,-3}{"  ".Repeat((node.Level-1) * margMulti)}{node.Bookmark}";

			Debug.WriteLine($"{t,-3}{preface,-75} | file| {f}");
			sb.Append($"type| {node.ItemType,-10}| pg num| {node.PageNumber,4}| list count| {node.ItemList.Count,4}| pg count  | {node.PageCount,4}| bkmrk| {node.Bookmark}\n");
		}

		private static void showPdfNodeItem(PdfTreeNode node)
		{
			string t = "N";
			string f = node.File.FileName;
			
			string preface =
				$"level| {node.Level,-3}| pg| {node.PageNumber,-3}{"  ".Repeat((node.Level-1) * margMulti)}{node.Bookmark}";

			Debug.WriteLine($"{t,-3}{preface}");
			sb.Append($"type| {node.ItemType,-10}| pg num| {node.PageNumber,4}| list count| {node.ItemList.Count,4}| pg count  | {node.PageCount,4}| bkmrk| {node.Bookmark}\n");
		}
		*/




	}
}