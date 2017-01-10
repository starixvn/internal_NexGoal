namespace NexGoal.Site.Controllers
{
    using Core.Enumerations;
    using Core.Services;
    using Core.Utilities;
    using Model.EmployeeEPA;
    using Model.Export;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using Services.Interfaces.EPA;
    using Services.Interfaces.ExportFile;
    using System.Web.Hosting;
    using System.Web.Mvc;

    public class ExportPDFController : Controller
    {
        private const string FileNameFormat = "{0}-{1}_{2}.pdf";
        private const string CssFilePath = @"~\Content\Site\ExportPDF\EpaDetail.css";
        private const string ViewTemplatePath = "~/Views/ExportPDF/ProbationDetail.cshtml";

        private readonly IEmployeeEPAService employeeEPAService;
        private readonly ISessionService sessionService;
        private readonly IRazorRenderService razorRenderService;
        private readonly IIntegrationService interationService;
        private readonly ICommonService commonService;

        public ExportPDFController()
            : this(null, null, null, null, null)
        {
        }

        public ExportPDFController(
            IEmployeeEPAService employeeEPAService,
            ISessionService sessionService,
            IRazorRenderService razorRenderService,
            IIntegrationService interationService,
            ICommonService commonService)
        {
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.sessionService = sessionService ?? new SessionService();
            this.razorRenderService = razorRenderService ?? new RazorRenderService();
            this.interationService = interationService ?? new IntegrationService();
            this.commonService = commonService ?? new CommonService();
        }

        public ActionResult ProbationDetail(int epaId)
        {
            int permission = employeeEPAService.CheckEmployeePermission(epaId, sessionService.EmployeeId);

            if (permission == (int)Policy.None)
            {
                return new EmptyResult();
            }

            var epa = employeeEPAService.GetEPAInfoById(epaId);
            var hrm = interationService.GetHRMSummaryOnProbation(epa.Employee.EmployeeId);
            var grades = commonService.GetStandardGrade();
            var epaInfo = new EpaDetailModel { Epa = epa, Hrm = hrm, Grades = grades };
            var fileName = string.Format(FileNameFormat, epa.Employee.Code, epa.Employee.NickName, epa.Period.StartDate.Value.ToString("MMM"));

            var streamContent = RenderPdfContent(epaInfo);

            return File(streamContent, @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", fileName);
        }

        private byte[] RenderPdfContent(EpaDetailModel epa)
        {
            var cssFilePath = GetPhysicalFilePath(CssFilePath);
            var htmlString = razorRenderService.RenderViewToString(this, ViewTemplatePath, epa);

            return RenderPdfContent(cssFilePath, htmlString);
        }

        private string GetPhysicalFilePath(string virtualFilePath)
        {
            return HostingEnvironment.MapPath(virtualFilePath);
        }

        private byte[] RenderPdfContent(string cssFilePath, string htmlString)
        {
            return PdfRender.ContentRender(cssFilePath, htmlString);
        }
    }
}
