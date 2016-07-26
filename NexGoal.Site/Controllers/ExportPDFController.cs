namespace NexGoal.Site.Controllers
{
    using NexGoal.Core.Constants;
    using NexGoal.Core.Utilities;
    using NexGoal.Criteria.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Configuration;
    using System.Web.Mvc;

    public class ExportPDFController : Controller
    {
        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly IExportPdfService _exportPdfService;

        public ExportPDFController()
            : this(null, null)
        {
        }

        public ExportPDFController(
            IEmployeeEPAService employeeEPAService,
            IExportPdfService exportPdfService)
        {
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            _exportPdfService = exportPdfService ?? new ExportPdfService();
        }

        public string RequestExportEPA(int epaId)
        {
            string target = "/EPA/Detail/" + epaId + "?exportpdf=true";
            string fileName = RequestExport(epaId, target);

            return fileName;
        }

        public string RequestExportSemiEPA(int epaId)
        {
            string target = "/EPA/SemiDetail/" + epaId + "?exportpdf=true";
            string fileName = RequestExport(epaId, target);

            return fileName;
        }

        public ActionResult DownloadEPADetail(int epaId, string fileName)
        {
            string pdfFile = Server.MapPath(Constants.ExportingFolder + fileName);
            bool isValid = FileUtilities.IsReadyWithIntervals(pdfFile, int.Parse(ConfigurationManager.AppSettings["IntervalToDownloadFile"]));
            if (!isValid)
            {
                return HttpNotFound();
            }

            byte[] bytes = FileUtilities.ReadAndDeleteFile(pdfFile);
            string newFileName = FormatPdfFileName(epaId);

            return File(bytes, "application/pdf", newFileName);
        }

        private string FormatPdfFileName(int epaId)
        {
            EPAModel epaInfo = _employeeEPAService.GetEPAInfoById(epaId);
            string code = string.IsNullOrEmpty(epaInfo.Employee.Code) ? string.Empty : "_" + epaInfo.Employee.Code;

            return string.Format("{0}.pdf", epaInfo.Employee.NickName + epaInfo.Employee.Code);
        }

        private string RequestExport(int epaId, string target)
        {
            string fileName = string.Format("EPA_{0}.pdf", DateTime.Now.ToString("HHmmss"));
            string domain = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port != 0 ? ":" + Request.Url.Port : string.Empty);
            string url = domain + target;
            string pdfFile = Server.MapPath(Constants.ExportingFolder + fileName);

            _exportPdfService.WriteEPADetailToFileAsync(epaId, pdfFile, url, Request.Cookies);

            return fileName;
        }
    }
}
