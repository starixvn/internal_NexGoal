namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.NominationReport;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Repositories;

    public class NominationReportService : INominationReportService
    {
        private readonly IBaseService baseService;
        private readonly INominationReportRepository reportRepository;

        public NominationReportService(
            IBaseService baseService,
            INominationReportRepository reportRepository)
        {
            this.baseService = baseService;
            this.reportRepository = reportRepository;
        }

        public async Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodAsync()
        {
            var criteria = new GetNominationPeriodsCriteria();

            return await baseService.GetDataAsync<NominationPeriodModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<NominationModel>> GetNominationAsync(
            int actorId,
            string nominationPeriodIds,
            string groupIds,
            string employeeIds,
            int? statusId)
        {
            var criteria = new GetNominationsCriteria
            {
                ActorId = actorId,
                NominationPeriodIds = nominationPeriodIds,
                GroupIds = groupIds,
                EmployeeIds = employeeIds,
                StatusId = statusId
            };

            IEnumerable<NominationModel> nominations = await reportRepository.GetNominationAsync(criteria).ConfigureAwait(false);

            return nominations.OrderByDescending(x => x.StatusId).ThenBy(x => x.Nominee.NickName);
        }
    }
}