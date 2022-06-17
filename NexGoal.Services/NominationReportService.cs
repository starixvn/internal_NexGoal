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
        private readonly IEmployeeNominationRepository repository;

        public NominationReportService(
            IBaseService baseService,
            IEmployeeNominationRepository repository)
        {
            this.baseService = baseService;
            this.repository = repository;
        }

        public async Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodAsync()
        {
            var criteria = new GetNominationPeriodsCriteria();

            return await baseService.GetDataAsync<NominationPeriodModel>(criteria);
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

            IEnumerable<NominationModel> nominations = await repository.GetNominationByPeriodIdsAsync(criteria);

            return nominations.OrderByDescending(x => x.StatusId).ThenBy(x => x.Nominee.NickName);
        }
    }
}