namespace NexGoal.Services
{
    using System;
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
    using NexGoal.Repositories;

    public class PARecordService : IPARecordService
    {
        private readonly IPARepository paRepository;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;
        private readonly IAvatarService avatarService;

        public PARecordService(
            IPARepository paRepository,
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService,
            IAvatarService avatarService)
        {
            this.paRepository = paRepository;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
            this.avatarService = avatarService;
        }

        public async Task<IEnumerable<PeriodBaseModel>> GetPeriodByActorAsync(int actorId)
        {
            var criteria = new GetPeriodByActorCriteria
            {
                ActorId = actorId
            };

            return await baseService.GetDataAsync<PeriodBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupBaseModel>> GetGroupByActorAsync(int actorId, int periodId)
        {
            var criteria = new GetGroupByActorCriteria
            {
                ActorId = actorId,
                PeriodId = periodId
            };

            return await baseService.GetDataAsync<GroupBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PerformanceAppraisalResult>> GetPerformanceAppraisalResultAsync(int employeeId)
        {
            var criteria = new GetPerformanceAppraisalResult
            {
                EmployeeId = employeeId
            };

            IEnumerable<PerformanceAppraisalResult> performanceAppraisals = await paRepository.GetPerformanceAppraisalResultAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

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

            IEnumerable<PerformanceAppraisalResult> performanceAppraisals = await paRepository.GetMemberPerformanceAppraisalResultAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

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
            PARecordModel performanceAppraisal = await paRepository.GetPARecordByIdAsync(criteria).ConfigureAwait(false);

            if (performanceAppraisal != null)
            {
                IEnumerable<ProjectModel> projects = await commonService.GetProjectsAsync().ConfigureAwait(false);
                IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
                MappingData(performanceAppraisal, performanceGrades, projects);
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

        public async Task<IEnumerable<PARecordModel>> GetPARecordByReviewerAsync(GetPARecordParam param)
        {
            var criteria = new GetPARecordByReviewerCriteria
            {
                ActorId = param.ActorId,
                PeriodId = param.PeriodId,
                GroupIds = param.GroupIds
            };
            IEnumerable<PARecordModel> performanceAppraisals = await paRepository.GetPARecordByReviewerAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            MappingData(performanceAppraisals, performanceGrades);

            return performanceAppraisals;
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

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<Policy> GetPermissionOnPerformanceAppraisalAsync(int actorId, int recordId)
        {
            var criteria = new GetPermissionOnPACriteria
            {
                ActorId = actorId,
                PARecordId = recordId
            };

            return await baseService.GetDataEntityAsync<Policy>(criteria).ConfigureAwait(false);
        }

        private void MappingData(
            IEnumerable<PARecordModel> performanceAppraisals,
            IEnumerable<PerformanceGradeModel> performanceGrades)
        {
            foreach (PARecordModel performanceAppraisal in performanceAppraisals)
            {
                performanceAppraisal.GradeId = cryptoService.Decrypt(performanceAppraisal.GradeId);
                int.TryParse(performanceAppraisal.GradeId, out int gradeId);
                performanceAppraisal.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

                performanceAppraisal.OriginalGradeId = cryptoService.Decrypt(performanceAppraisal.OriginalGradeId);
                int.TryParse(performanceAppraisal.OriginalGradeId, out gradeId);
                performanceAppraisal.OriginalGrade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

                foreach (PAProcessModel process in performanceAppraisal.Processes)
                {
                    if (process.PerformanceRecord != null)
                    {
                        process.PerformanceRecord.GradeId = cryptoService.Decrypt(process.PerformanceRecord.GradeId);
                        int.TryParse(process.PerformanceRecord.GradeId, out gradeId);
                        process.PerformanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
                    }
                }
            }
        }

        private void MappingData(
            PARecordModel performanceAppraisal,
            IEnumerable<PerformanceGradeModel> performanceGrades,
            IEnumerable<ProjectModel> projects)
        {
            int gradeId;
            foreach (PAProcessModel process in performanceAppraisal.Processes)
            {
                process.Evaluator.Avatar = avatarService.GetFilePath(process.Evaluator.Code);
                if (process.PerformanceRecord != null)
                {
                    process.PerformanceRecord.Creator.Avatar = avatarService.GetFilePath(process.PerformanceRecord.Creator.Code);

                    string[] projectIds = !string.IsNullOrWhiteSpace(process.PerformanceRecord.ProjectIds) ? process.PerformanceRecord.ProjectIds.Split(",") : new string[] { };
                    process.PerformanceRecord.Projects = projects.Where(x => projectIds.Contains(x.ProjectId.ToString()));

                    process.PerformanceRecord.GradeId = cryptoService.Decrypt(process.PerformanceRecord.GradeId);
                    int.TryParse(process.PerformanceRecord.GradeId, out gradeId);
                    process.PerformanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
                }
            }

            performanceAppraisal.GradeId = cryptoService.Decrypt(performanceAppraisal.GradeId);
            int.TryParse(performanceAppraisal.GradeId, out gradeId);
            performanceAppraisal.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            performanceAppraisal.OriginalGradeId = cryptoService.Decrypt(performanceAppraisal.OriginalGradeId);
            int.TryParse(performanceAppraisal.OriginalGradeId, out gradeId);
            performanceAppraisal.OriginalGrade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
        }

        public async Task<string> GetPreviousPAGrade(int currentPeriodId, int actorId, int employeeId)
        {
            var criteria = new GetLastPAGradeCriteria
            {
                CurrentPeriodId = currentPeriodId,
                ActorId = actorId,
                EmployeeId = employeeId
            };

            return await baseService.GetDataEntityAsync<string>(criteria).ConfigureAwait(false);
        }
    }
}