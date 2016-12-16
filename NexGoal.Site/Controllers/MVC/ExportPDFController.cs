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
        private const string ViewTemplatePath = "~/Views/ExportPDF/EpaDetail.cshtml";

        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly ISessionService sessionService;
        private readonly IRazorRenderService _razorRenderService;
        private readonly IIntegrationService interationService;
        private readonly ICommonService commonService;

        public ExportPDFController()
            : this(null, null, null, null, null)
        {
        }

        public ExportPDFController(
            IEmployeeEPAService employeeEPAService,
            ISessionService sessionService,
            IRazorRenderService _razorRenderService,
            IIntegrationService interationService,
            ICommonService commonService)
        {
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.sessionService = sessionService ?? new SessionService();
            this._razorRenderService = _razorRenderService ?? new RazorRenderService();
            this.interationService = interationService ?? new IntegrationService();
            this.commonService = commonService ?? new CommonService();
        }

        public ActionResult EpaDetail(int epaId)
        {
            int permission = _employeeEPAService.CheckEmployeePermission(epaId, sessionService.EmployeeId);

            if (permission == (int)Policy.None)
            {
                return new EmptyResult();
            }

            var epa = _employeeEPAService.GetEPAInfoById(epaId);
            var hrm = interationService.GetHRMSummary(epa.Employee.EmployeeId, epa.Period.PeriodId);
            var grades = commonService.GetStandardGrade();
            var epaInfo = new EpaDetailModel
            {
                Epa = epa,
                Hrm = hrm,
                Grades = grades,
                RootApplication = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"))
            };            

            return View(epaInfo);
        }

        public ActionResult ExportEpaDetail(int epaId)
        {
            int permission = _employeeEPAService.CheckEmployeePermission(epaId, sessionService.EmployeeId);

            if (permission == (int)Policy.None)
            {
                return new EmptyResult();
            }

            var epa = _employeeEPAService.GetEPAInfoById(epaId);
            var hrm = interationService.GetHRMSummary(epa.Employee.EmployeeId, epa.Period.PeriodId);
            var grades = commonService.GetStandardGrade();
            var epaInfo = new EpaDetailModel { Epa = epa, Hrm = hrm, Grades = grades };

            var streamContent = RenderPdfContent(epaInfo);

            return File(streamContent, @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", "x.pdf");
        }

        private byte[] RenderPdfContent(EpaDetailModel epa)
        {
            var cssFilePath = GetPhysicalFilePath(CssFilePath);
            var htmlString = _razorRenderService.RenderViewToString(this, ViewTemplatePath, epa);

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
