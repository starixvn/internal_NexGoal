namespace NexGoal.Services.Implement
{
    using Model;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.Report;
    using NexGoal.Model.Report.Dashboard;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class ReportService : IReportService
    {
        private readonly IReportRepository reportRepository;
        private readonly IEmployeeEPARepository employeeEPARepository;
        private readonly IIntegrationService integrationService;

        public ReportService()
            : this(null, null, null)
        {
        }

        public ReportService(
            IReportRepository reportRepository,
            IEmployeeEPARepository employeeEPARepository,
            IIntegrationService integrationService)
        {
            this.reportRepository = reportRepository ?? new ReportRepository();
            this.employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
            this.integrationService = integrationService ?? new IntegrationService();
        }

        public IEnumerable<MasterReportModel> GetMaster(GetMasterReportCriteria criteria)
        {
            IEnumerable<MasterReportModel> result = new List<MasterReportModel>();
            if (criteria.IsValid())
            {
                result = this.reportRepository.GetMaster(criteria);

                var nexPMs = this.integrationService.GetEmployeeWorking(criteria.EmployeeId, criteria.PeriodId);
                foreach (var employeeFocus in result)
                {
                    var nexPMsOfEmployee = nexPMs.Where(i => i.EmployeeId == employeeFocus.PAFocus.Employee.EmployeeId);
                    employeeFocus.EmployeeWorkings.AddRange(nexPMsOfEmployee);
                    employeeFocus.Decrypt();
                }
            }

            return result;
        }

        public DashboardModel GetDashboard(GetDashboardCriteria criteria)
        {
            DashboardModel result = new DashboardModel();

            if (criteria != null && criteria.IsValid())
            {
                result = employeeEPARepository.GetDashboard(criteria);
            }

            return result;
        }

        public IEnumerable<EPADetailModel> GetEPADetailReport(GetPAReportCriteria criteria)
        {
            IEnumerable<EPADetailModel> result = new List<EPADetailModel>();

            if (criteria != null && criteria.IsValid())
            {
                result = reportRepository.GetEPADetail(criteria);
            }

            return result;
        }

        public IEnumerable<NominationViewModel> GetNomination(GetNominationReportCriteria criteria)
        {
            IEnumerable<NominationViewModel> result = new List<NominationViewModel>();

            if (criteria != null && criteria.IsValid())
            {
                IEnumerable<NominationModel> nominations = reportRepository.GetNomination(criteria);

                result = nominations.GroupBy(
                    x => new { x.Period.EOQYId, x.Nominee.EmployeeId },
                    (key, g) => new NominationViewModel
                    {
                        Period = g.First().Period,
                        Nominee = g.First().Nominee,
                        Nominations = g.Select(x => new NominationDetailViewModel
                        {
                            NominationId = x.NominationId,
                            Nominator = x.Nominator
                        }).ToList(),
                        RoleId = g.First().RoleId,
                        StatusId = g.First().StatusId
                    });
            }

            return result;
        }
    }
}