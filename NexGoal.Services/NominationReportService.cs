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

            var criteriaNew = new GetNominationsNewCriteria
            {
                ActorId = actorId,
                NominationPeriodIds = nominationPeriodIds,
                GroupIds = groupIds,
                EmployeeIds = employeeIds,
                StatusId = statusId
            };

            var index = 0;
            var nominations = await repository.GetNominationByPeriodIdsAsync(criteria);
            var nominationsNew = await repository.GetNominationByPeriodIdsAsync(criteriaNew);

            foreach (NominationModel nomination in nominations)
            {
                var nominatorNew = nominationsNew.ElementAt(index).Nominators;
                var selected = nomination.Nominators.Where(p => !nominatorNew.Any(pn => pn.PerformanceRecordId == p.PerformanceRecordId));

                nominatorNew = EmployeeNominationService.GroupKPIPerformances(nominatorNew);
                nominationsNew.ElementAt(index).Nominators = nominatorNew.Concat(selected).OrderByDescending(x => x.CreatedDate);

                index++;
            }

            return nominationsNew.OrderByDescending(x => x.StatusId).ThenBy(x => x.Nominee.NickName);
        }
    }
}