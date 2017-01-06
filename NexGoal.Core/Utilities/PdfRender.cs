using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NexGoal.Core.Utilities
{
    public static class PdfRender
    {
        private const int A4Width = 595;

        private const int A4Height = 842;

        public static byte[] ContentRender(string cssFilePath, string htmlText)
        {
            MemoryStream stream = null;
            Document pdfDocument = null;
            try
            {
                using (stream = new MemoryStream())
                {
                    pdfDocument = new Document(new Rectangle(A4Width, A4Height), 20, 20, 40, 40);
                    pdfDocument.SetPageSize(PageSize.A4);
                    var pdfWriter = PdfWriter.GetInstance(pdfDocument, stream);
                    var worker = XMLWorkerHelper.GetInstance();
                    var font = new UnicodeFontFactory();

                    pdfDocument.Open();

                    using (var htmlTextReader = new MemoryStream(Encoding.Unicode.GetBytes(htmlText)))
                    {
                        using (var cssStream = new FileStream(cssFilePath, FileMode.Open, FileAccess.Read))
                        {
                            worker.ParseXHtml(pdfWriter, pdfDocument, htmlTextReader, cssStream, Encoding.Unicode, font);
                        }
                    }

                    pdfDocument.Close();
                    pdfDocument = null;

                    return stream.ToArray();
                }
            }
            finally
            {
                if (pdfDocument != null)
                {
                    pdfDocument.Dispose();
                }
            }
        }
    }
}
