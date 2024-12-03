#region using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DebugCode;
using iText.Kernel.Pdf;
using ShItextCode;
using ShTempCode.DebugCode;
using UtilityLibrary;

#endregion

// username: jeffs
// created:  7/9/2024 11:47:05 PM

namespace CreatePDFElements.Support
{
	public class InvestigateManager
	{
		private PdfDocument doc;
		private PdfPage page;

		private PdfDocInfo docInfo;
		private PdfPageInfo pageInfo;

		public List<PdfDocInfo> Docs { get; set; }

		public string[] GetSheets(int idx)
		{
			return Directory.GetFiles(Program.Folders[idx], "*.pdf");
		}

		public void Process(string idx)
		{
			DM.DbxLineEx(0, "start", 1);

			string filename;
			string filePath;
			bool result;

			Docs = new List<PdfDocInfo>();

			Debug.Write("Begin query ");

			foreach (string sheet in GetSheets(1))
			{
				addSheet(sheet);
			}

			showResults(idx);

			DM.DbxLineEx(0, "end", 0, -1);
		}

		public void Process(string sheet, string idx)
		{
			DM.DbxLineEx(0, "start", 1);

			Docs = new List<PdfDocInfo>();

			addSheet(sheet);

			showResults(idx);

			DM.DbxLineEx(0, "end", 0, -1);
		}

		private void showResults(string idx)
		{
			if (idx.Equals("S"))
			{
				PdfShowInfo.ShowPdfInfoBasic(Docs);
			}
			else
			if (idx.Equals("L"))
			{
				PdfShowInfo.ShowPdfInfoEx(Docs);
			}
		}

		private void addSheet(string sheet)
		{
			string filename;
			string filePath;
			bool result;

			filename = Path.GetFileNameWithoutExtension(sheet);
			filePath = Path.GetFullPath(sheet);

			result = File.Exists(sheet);

			if (!result)
			{
				Debug.Write("x");
				return;
			}

			Debug.Write(".");

			docInfo = new PdfDocInfo(filename, sheet);
				
			querySheet(sheet);

			Docs.Add(docInfo);
		}

		private void querySheet(string file)
		{
			DM.DbxLineEx(0, "start", 1);

			bool result = true;
			
			doc = new PdfDocument(new PdfReader(file));

			docInfo.GetDocumentInfo(doc);
			
			for (int i = 1; i < docInfo.NumberOfPages+1; i++)
			{
				page = doc.GetPage(i);

				pageInfo = new PdfPageInfo(page);

				docInfo.PageInfo.Add(i,pageInfo);
			}

			doc.Close();

			DM.DbxLineEx(0, "end", 0, -1);
		}

	}
}
