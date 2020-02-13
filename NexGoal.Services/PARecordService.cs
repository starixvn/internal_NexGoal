namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PARecord;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Repositories;
    using NexGoal.Resources;

    public class PARecordService : IPARecordService
    {
        private readonly IPARepository paRepository;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public PARecordService(
            IPARepository paRepository,
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService)
        {
            this.paRepository = paRepository;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<PeriodBaseModel>> GetPeriodByActorAsync(int actorId)
        {
            var criteria = new GetPeriodByActorCriteria
            {
                ActorId = actorId
            };

            return await baseService.GetDataAsync<PeriodBaseModel>(criteria);
        }

        public async Task<IEnumerable<Group>> GetGroupByActorAsync(int actorId, int periodId)
        {
            var criteria = new GetGroupByActorCriteria
            {
                ActorId = actorId,
                PeriodId = periodId
            };

            return await baseService.GetDataAsync<Group>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PerformanceAppraisalResult>> GetPerformanceAppraisalResultAsync(int employeeId)
        {
            var criteria = new GetPerformanceAppraisalResult
            {
                EmployeeId = employeeId
            };

            IEnumerable<PerformanceAppraisalResult> performanceAppraisals = await paRepository.GetPerformanceAppraisalResultAsync(criteria);
            IEnumerable<PerformanceGrade> performanceGrades = await commonService.GetPerformanceGradeAsync();

            foreach (PerformanceAppraisalResult performanceAppraisal in performanceAppraisals)
            {
                performanceAppraisal.GradeId = cryptoService.Decrypt(performanceAppraisal.GradeId);
                int.TryParse(performanceAppraisal.GradeId, out int gradeId);
                performanceAppraisal.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
            }

            return performanceAppraisals;
        }

        public async Task<IEnumerable<PerformanceAppraisalResult>> GetMemberPerformanceAppraisalResultAsync(
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

            IEnumerable<PerformanceAppraisalResult> performanceAppraisals = await paRepository.GetMemberPerformanceAppraisalResultAsync(criteria);
            IEnumerable<PerformanceGrade> performanceGrades = await commonService.GetPerformanceGradeAsync();

            foreach (PerformanceAppraisalResult performanceAppraisal in performanceAppraisals)
            {
                performanceAppraisal.GradeId = cryptoService.Decrypt(performanceAppraisal.GradeId);
                int.TryParse(performanceAppraisal.GradeId, out int gradeId);
                performanceAppraisal.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
            }

            return performanceAppraisals;
        }

        public async Task<PARecordModel> GetPARecordByIdAsync(int paRecordId, int actorId)
        {
            var criteria = new GetPARecordByIdCriteria
            {
                PARecordId = paRecordId,
                ActorId = actorId
            };
            PARecordModel performanceAppraisal = await paRepository.GetPARecordByIdAsync(criteria);

            if (performanceAppraisal != null)
            {
                IEnumerable<PerformanceGrade> performanceGrades = await commonService.GetPerformanceGradeAsync();
                MappingData(performanceAppraisal, performanceGrades);
            }

            return performanceAppraisal;
        }

        public async Task<CommandResult> QuickEditPARecordByReviewerAsync(int actorId, int paRecordId, bool isRevised)
        {
            var criteria = new QuickEditPARecordByReviewerCriteria
            {
                PARecordId = paRecordId,
                ActorId = actorId,
                IsRevised = isRevised
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PARecordGroupModel>> GetPARecordByReviewerAsync(GetPARecordParam param)
        {
            var criteria = new GetPARecordByReviewerCriteria
            {
                ActorId = param.ActorId,
                PeriodId = param.PeriodId,
                GroupIds = param.GroupIds,
                IsRevised = param.IsRevised
            };
            IEnumerable<PARecordModel> performanceAppraisals = await paRepository.GetPARecordByReviewerAsync(criteria);
            IEnumerable<PerformanceGrade> performanceGrades = await commonService.GetPerformanceGradeAsync();

            foreach (PARecordModel performanceAppraisal in performanceAppraisals)
            {
                MappingData(performanceAppraisal, performanceGrades);
            }

            var paRecords = performanceAppraisals.GroupBy(p => p.Employee.GroupName)
                .Select(r => new PARecordGroupModel
                {
                    GroupName = r.Key,
                    PerformanceAppraisals = r
                });

            return paRecords;
        }

        public async Task<CommandResult> EditPARecordByReviewerAsync(int actorId, EditPARecordByReviewerModel paRecord)
        {
            var criteria = new EditPARecordByReviewerCriteria
            {
                PARecordId = paRecord.PARecordId,
                GradeId = cryptoService.Encrypt(paRecord.GradeId),
                Comment = paRecord.Comment,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<Policy> GetPermissionOnPerformanceAppraisalAsync(int actorId, int recordId)
        {
            var criteria = new GetPermissionOnPACriteria
            {
                ActorId = actorId,
                PARecordId = recordId
            };

            return await baseService.GetDataEntityAsync<Policy>(criteria);
        }

        private void MappingData(
            PARecordModel performanceAppraisal,
            IEnumerable<PerformanceGrade> performanceGrades)
        {
            int gradeId;
            foreach (PAProcessModel process in performanceAppraisal.Processes)
            {
                process.Grade = GetGradeOfPAProcess(process, performanceGrades);
                foreach (PerformanceRecord performanceRecord in process.RelatedPerformanceRecords)
                {
                    performanceRecord.GradeId = cryptoService.Decrypt(performanceRecord.GradeId);
                    int.TryParse(performanceRecord.GradeId, out gradeId);
                    performanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
                }
            }

            performanceAppraisal.GradeId = cryptoService.Decrypt(performanceAppraisal.GradeId);
            int.TryParse(performanceAppraisal.GradeId, out gradeId);
            performanceAppraisal.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            performanceAppraisal.OriginalGradeId = cryptoService.Decrypt(performanceAppraisal.OriginalGradeId);
            int.TryParse(performanceAppraisal.OriginalGradeId, out gradeId);
            performanceAppraisal.OriginalGrade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
        }

        private string GetGradeOfPAProcess(PAProcessModel process, IEnumerable<PerformanceGrade> performanceGrades)
        {
            process.GradeId = cryptoService.Decrypt(process.GradeId);
            int.TryParse(process.GradeId, out int gradeId);
            PerformanceGrade grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            if (grade != null)
            {
                return grade.Name;
            }

            return !string.IsNullOrWhiteSpace(process.RelatedPerformanceRecordIds) ? ViewLang.NA : string.Empty;
        }
    }
}