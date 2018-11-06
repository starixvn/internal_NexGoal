namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EmployeeNomination;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PerformanceRecord;

    public class EmployeeNominationService : IEmployeeNominationService
    {
        private readonly IBaseService baseService;

        private readonly IDynamicRepository dynamicRepository;

        private readonly IAvatarService avatarService;

        public EmployeeNominationService(IBaseService baseService, IDynamicRepository dynamicRepository, IAvatarService avatarService)
        {
            this.baseService = baseService;
            this.dynamicRepository = dynamicRepository;
            this.avatarService = avatarService;
        }

        public async Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominator(int nominatorId)
        {
            var criteria = new GetNominationPeriodsForNominatorCriteria
            {
                NominatorId = nominatorId
            };

            return await baseService.GetDataAsync<NominationPeriodModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> Create(CreateEmployeeNominationModel model)
        {
            var criteria = new CreateEmployeeNominationCriteria
            {
                NominationPeriodId = model.NominationPeriodId,
                NominatorId = model.NominatorId,
                NomineeId = model.NomineeId,
                Comment = model.Comment
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PerformanceRecordModel>> GetMemberNominationssAsync(int nominatorId)
        {
            var criteria = new GetMemberNominationCriteria
            {
                ActorId = nominatorId,
            };

            var result = await dynamicRepository.FetchMultipleAsync(criteria,
                new[] {
                    typeof(PerformanceRecordModel),
                    typeof(EmployeeBaseModel),
                    typeof(NominationModel),
                    typeof(NominationPeriodModel)
                }).ConfigureAwait(false);

            var performanceRecords = result[0].Cast<PerformanceRecordModel>();
            var employees = result[1].Cast<EmployeeBaseModel>();
            var nominations = result[2].Cast<NominationModel>();
            var nominationPeriods = result[3].Cast<NominationPeriodModel>();

            MappingData(performanceRecords, employees, nominations, nominationPeriods);

            return performanceRecords;
        }

        private void MappingData(IEnumerable<PerformanceRecordModel> performanceRecords, IEnumerable<EmployeeBaseModel> employees, IEnumerable<NominationModel> nominations,
            IEnumerable<NominationPeriodModel> nominationPeriods)
        {
            foreach (var employee in employees)
            {
                employee.Base64Image = avatarService.GetBase64Image(employee.Code);
            }

            foreach (var nomination in nominations)
            {
                nomination.Period = nominationPeriods.FirstOrDefault(x => x.NominationPeriodId == nomination.NominationPeriodId);
            }

            foreach (var record in performanceRecords)
            {
                var employeeIds = record.EmployeeIds.Split(',').Select(int.Parse);
                record.Employees = employees.Where(x => employeeIds.Any(e => e == x.EmployeeId));
                record.Creator = employees.FirstOrDefault(x => x.EmployeeId == record.CreatorId);
                record.Nomination = nominations.FirstOrDefault(x => x.NominationId == record.NominationId);
            }
        }
    }
}