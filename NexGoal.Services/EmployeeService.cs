namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Employee;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Employee;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PARecord;

    public class EmployeeService : IEmployeeService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public EmployeeService(
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<EmployeeBaseModel>> GetEmployeesAsync(
            string keyword = null,
            int? groupId = null,
            bool? isLocal = null,
            bool? isDisabled = null)
        {
            var criteria = new GetEmployeesCriteria
            {
                Keyword = keyword,
                GroupId = groupId,
                IsLocal = isLocal,
                IsDisabled = isDisabled
            };

            return await baseService.GetDataAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<EmployeeBaseModel> GetEmployeeByIdAsync(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataEntityAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmployeeRoleModel>> GetRolesByEmployeeIdAsync(int employeeId)
        {
            var criteria = new GetRolesByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<EmployeeRoleModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditEmployeeRolesAsync(EditEmployeeRolesModel model)
        {
            var criteria = new EditEmployeeRolesCriteria
            {
                EmployeeId = model.EmployeeId,
                SelectedRoleIds = model.SelectedRoleIds,
                ModifiedBy = model.ModifiedBy
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PARecordModel>> GetPerformanceAppraisalResultAsync(int employeeId)
        {
            var criteria = new GetPerformanceAppraisalResult
            {
                EmployeeId = employeeId
            };

            var tuple = await baseService.FetchMultipleAsync<PARecordModel, PeriodBaseModel>(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            foreach (PARecordModel performanceAppraisal in tuple.Item1)
            {
                performanceAppraisal.GradeId = cryptoService.Decrypt(performanceAppraisal.GradeId);
                int.TryParse(performanceAppraisal.GradeId, out int gradeId);
                performanceAppraisal.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

                performanceAppraisal.Period = tuple.Item2.FirstOrDefault(x => x.PeriodId == performanceAppraisal.PeriodId);
            }

            return tuple.Item1;
        }

        public async Task<IEnumerable<PARecordModel>> GetMemberPerformanceAppraisalResultAsync(
            int actorId,
            int? periodId = null,
            int? employeeId = null)
        {
            var criteria = new GetMemberPerformanceAppraisalResult
            {
                ActorId = actorId,
                PeriodId = periodId,
                EmployeeId = employeeId
            };

            var tuple = await baseService.FetchMultipleAsync<PARecordModel, PeriodBaseModel>(criteria).ConfigureAwait(false);
            IEnumerable<EmployeeBaseModel> members = await commonService.GetEmployeesAsync().ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            foreach (PARecordModel performanceAppraisal in tuple.Item1)
            {
                performanceAppraisal.GradeId = cryptoService.Decrypt(performanceAppraisal.GradeId);
                int.TryParse(performanceAppraisal.GradeId, out int gradeId);
                performanceAppraisal.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

                performanceAppraisal.Employee = members.FirstOrDefault(x => x.EmployeeId == performanceAppraisal.EmployeeId);
                performanceAppraisal.Period = tuple.Item2.FirstOrDefault(x => x.PeriodId == performanceAppraisal.PeriodId);
            }

            return tuple.Item1;
        }

        public async Task<IEnumerable<EmployeeNominationModel>> GetNominationResultAsync(int employeeId)
        {
            var criteria = new GetNominationResultCriteria
            {
                EmployeeId = employeeId
            };

            var tuple = await baseService.FetchMultipleAsync<EmployeeNominationModel, NominationPeriodModel>(criteria).ConfigureAwait(false);

            foreach (EmployeeNominationModel nomination in tuple.Item1)
            {
                nomination.Period = tuple.Item2.FirstOrDefault(x => x.NominationPeriodId == nomination.NominationPeriodId);
            }

            return tuple.Item1;
        }

        public async Task<IEnumerable<EmployeeNominationModel>> GetMemberNominationResultAsync(int actorId, int? employeeId = null)
        {
            var criteria = new GetMemberNominationResultCriteria
            {
                ActorId = actorId,
                EmployeeId = employeeId
            };

            var tuple = await baseService.FetchMultipleAsync<EmployeeNominationModel, NominationPeriodModel>(criteria).ConfigureAwait(false);

            foreach (EmployeeNominationModel nomination in tuple.Item1)
            {
                nomination.Period = tuple.Item2.FirstOrDefault(x => x.NominationPeriodId == nomination.NominationPeriodId);
            }

            return tuple.Item1;
        }

        public async Task<IEnumerable<EmployeeNominationModel>> GetMemberNominationDetailsAsync(
            int actorId,
            int? nominationPeriodId = null,
            int? employeeId = null)
        {
            var criteria = new GetMemberNominationDetailsCriteria
            {
                ActorId = actorId,
                NominationPeriodId = nominationPeriodId,
                EmployeeId = employeeId
            };

            var tuple = await baseService.FetchMultipleAsync<NominationPeriodModel, EmployeeNominationModel, EmployeeNominationDetailModel>(criteria).ConfigureAwait(false);
            IEnumerable<EmployeeBaseModel> nominators = await commonService.GetEmployeesAsync().ConfigureAwait(false);

            MappingData(tuple.Item2, tuple.Item3, tuple.Item1, nominators);

            return tuple.Item2;
        }

        public async Task<IEnumerable<EmployeeBaseModel>> GetMemberAsync(int actorId, string keyword = null)
        {
            var criteria = new GetMemberByActorIdCriteria
            {
                ActorId = actorId,
                Keyword = keyword
            };

            return await baseService.GetDataAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmployeeBaseModel>> GetMemberByProcessIdAsync(int actorId,
            PerformanceProcess performanceProcess)
        {
            var criteria = new GetMemberByProcessIdCriteria
            {
                ActorId = actorId,
                ProcessId = performanceProcess
            };

            return await baseService.GetDataAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupBaseModel>> GetGroupAsync(int actorId)
        {
            var criteria = new GetGroupByActorIdCriteria
            {
                ActorId = actorId
            };

            return await baseService.GetDataAsync<GroupBaseModel>(criteria).ConfigureAwait(false);
        }

        private static void MappingData(
            IEnumerable<EmployeeNominationModel> nominations,
            IEnumerable<EmployeeNominationDetailModel> nominationDetails,
            IEnumerable<NominationPeriodModel> nominationPeriods,
            IEnumerable<EmployeeBaseModel> nominators)
        {
            foreach (EmployeeNominationDetailModel nominationDetail in nominationDetails)
            {
                nominationDetail.Nominator = nominators.FirstOrDefault(x => x.EmployeeId == nominationDetail.NominatorId);
            }

            foreach (EmployeeNominationModel nomination in nominations)
            {
                nomination.Period = nominationPeriods.FirstOrDefault(x => x.NominationPeriodId == nomination.NominationPeriodId);
                nomination.NominationDetails = nominationDetails.Where(x => x.NominationPeriodId == nomination.NominationPeriodId);
            }
        }
    }
}