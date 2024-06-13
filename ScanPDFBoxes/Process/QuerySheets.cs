#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.XMP;
using ShItextCode;
using Path = System.IO.Path;

#endregion

// user name: jeffs
// created:   6/3/2024 7:29:33 PM

namespace ScanPDFBoxes.Process
{


	public class QuerySheets
	{
		private PdfDocument doc;
		private PdfPage page;

		private PdfDocInfo docInfo;
		private PdfPageInfo pageInfo;

		public List<PdfDocInfo> Docs { get; set; }

		public void Process(string[] sheets)
		{
			string filename;
			string filePath;
			bool result;

			Docs = new List<PdfDocInfo>();

			Debug.Write("Begin query ");

			foreach (string sheet in sheets)
			{
				filename = Path.GetFileNameWithoutExtension(sheet);
				filePath = Path.GetFullPath(sheet);

				result = File.Exists(sheet);

				if (!result)
				{
					Debug.Write("x");
					continue;
				}

				Debug.Write(".");

				docInfo = new PdfDocInfo(filename, sheet);
				
				querySheet(sheet);

				Docs.Add(docInfo);
			}
		}

		private void querySheet(string file)
		{
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
		}


		public override string ToString()
		{
			return $"this is {nameof(QuerySheets)}";
		}
	}
}
