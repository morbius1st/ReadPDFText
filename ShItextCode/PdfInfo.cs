#region + Using Directives
using iText.Kernel.Pdf;
using iText.Kernel.XMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;

#endregion

// user name: jeffs
// created:   6/3/2024 10:29:41 PM

namespace ShItextCode
{
	
	public class PdfDocInfo
	{
		public PdfDocInfo(string name, string path) 
		{
			Name = name;
			Path = path;

			PageInfo = new Dictionary<int, PdfPageInfo>();
		}

		public string Name { get; set; }
		public string Path { get; set; }

		public string Subject { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
		public string Author { get; set; }
		public string Creator { get; set; }
		public string Producer { get; set; }
		public string Publisher { get; set; }

		public string CreationData {get; set; }
		public string ModificationData { get; set; }

		public int NumberOfPages { get; set; }

		public Dictionary<int, PdfPageInfo> PageInfo { get; set; }

		public void GetDocumentInfo(PdfDocument doc)
		{
			NumberOfPages = doc.GetNumberOfPages();

			PdfDocumentInfo d = doc.GetDocumentInfo();

			Subject = d.GetSubject();
			Title = d.GetTitle();
			Type = d.GetMoreInfo(PdfConst.Type);
			Author = d.GetAuthor();
			Creator = d.GetCreator();
			Producer = d.GetProducer();

			CreationData = d.GetMoreInfo(PdfConst.CreateDate);
			ModificationData = d.GetMoreInfo(PdfConst.ModifyDate);

			Publisher = d.GetMoreInfo(PdfConst.Publisher);
			Description = d.GetMoreInfo(PdfConst.Description);

			string t;
			t = d.GetMoreInfo(PdfConst.Contributor);
			t = d.GetMoreInfo(PdfConst.CreatorTool);
			t = d.GetMoreInfo(PdfConst.Rights);

		}
	}

	public struct PdfPageInfo
	{
		public Rectangle PageSize { get; set; }
		public Rectangle PageSizeWithRotation { get; set; }
		public float PageRotation { get; set; }

		public PdfPageInfo(PdfPage page)
		{
			PageSize = page.GetPageSize();
			PageSizeWithRotation = page.GetPageSizeWithRotation();
			PageRotation = page.GetRotation();
		}
	}

}
