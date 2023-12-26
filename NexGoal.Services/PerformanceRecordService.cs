namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceRecord;
    using NexGoal.Models;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.Period;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Resources;

    public class PerformanceRecordService : IPerformanceRecordService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;
        private readonly INotificationPerformanceRecordService notificationService;
        private readonly IPerformanceRecordRepository performanceRecordRepository;
        private readonly IBackgroundJobClient backgroundJob;

        public PerformanceRecordService(
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService,
            INotificationPerformanceRecordService notificationService,
            IPerformanceRecordRepository performanceRecordRepository,
            IBackgroundJobClient backgroundJob)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
            this.notificationService = notificationService;
            this.performanceRecordRepository = performanceRecordRepository;
            this.backgroundJob = backgroundJob;
        }

        public async Task<IEnumerable<PerformanceRecord>> GetPerformanceRecordByEmployeeAsync(int employeeId)
        {
            var criteria = new GetPerformanceRecordByEmployeeCriteria
            {
                EmployeeId = employeeId
            };
            var performanceRecords = await performanceRecordRepository.GetPerformanceRecordByEmployeeAsync(criteria);
            var performanceGrades = await commonService.GetPerformanceGradeAsync();

            var currentPeriod = await commonService.GetCurrentPerformancePeriodAsync();

            foreach (var performanceRecord in performanceRecords)
            {
                MappingDataForPerformanceRecordAsync(performanceRecord, performanceGrades, employeeId, currentPeriod);
                if (performanceRecord.TypeId == (byte)PerformanceRecordType.SystemPerformanceAppraisal)
                {
                    DecryptPAGrade(performanceRecord.PASummary, performanceGrades);
                }
            }

            return performanceRecords;
        }

        public async Task<ValueTuple<IEnumerable<PerformanceRecord>, int>> GetPerformanceTimelineAsync(GetPerformanceTimelineParam param)
        {
            var criteria = new GetPerformanceTimelineCriteria
            {
                EmployeeId = param.EmployeeId,
                PageIndex = param.PageIndex,
                PageSize = param.PageSize
            };
            var performanceRecords = await performanceRecordRepository.GetPerformanceTimelineAsync(criteria);
            var performanceGrades = await commonService.GetPerformanceGradeAsync();

            var currentPeriod = await commonService.GetCurrentPerformancePeriodAsync();

            foreach (var performanceRecord in performanceRecords)
            {
                MappingDataForPerformanceRecordAsync(performanceRecord, performanceGrades, param.EmployeeId, currentPeriod);
            }

            return new (performanceRecords, criteria.TotalItems);
        }

        public async Task<IEnumerable<PerformanceRecord>> GetMemberPerformanceAsync(int actorId)
        {
            var criteria = new GetMemberPerformanceCriteria
            {
                ActorId = actorId
            };
            var performanceRecords = await performanceRecordRepository.GetMemberPerformanceAsync(criteria);
            var performanceGrades = await commonService.GetPerformanceGradeAsync();

            var currentPeriod = await commonService.GetCurrentPerformancePeriodAsync();

            foreach (var performanceRecord in performanceRecords)
            {
                MappingDataForPerformanceRecordAsync(performanceRecord, performanceGrades, actorId, currentPeriod);
            }

            return performanceRecords;
        }

        public async Task<IEnumerable<PerformanceRecord>> GetMemberPerformanceDetailAsync(int actorId, int employeeId)
        {
            var criteria = new GetMemberPerformanceDetailCriteria
            {
                ActorId = actorId,
                EmployeeId = employeeId
            };
            var performanceRecords = await performanceRecordRepository.GetMemberPerformanceDetailAsync(criteria);
            var performanceGrades = await commonService.GetPerformanceGradeAsync();

            var currentPeriod = await commonService.GetCurrentPerformancePeriodAsync();

            foreach (var performanceRecord in performanceRecords)
            {
                MappingDataForPerformanceRecordAsync(performanceRecord, performanceGrades, actorId, currentPeriod);

                if (performanceRecord.TypeId == (byte)PerformanceRecordType.SystemPerformanceAppraisal)
                {
                    DecryptPAGrade(performanceRecord.PASummary, performanceGrades);
                }
            }

            return performanceRecords;
        }

        public async Task<IEnumerable<MemberPerformanceTimelineModel>> GetMemberPerformanceTimelineAsync(GetMemberPerformanceTimelineParam param)
        {
            var criteria = new GetMemberPerformanceTimelineCriteria
            {
                ActorId = param.ActorId,
                PeriodId = param.PeriodId,
                EmployeeId = param.EmployeeId,
                PageIndex = param.PageIndex,
                PageSize = param.PageSize
            };

            var (memberPerformances, performanceRecords) = await performanceRecordRepository.GetMemberPerformanceTimelineAsync(criteria);
            var performanceGrades = await commonService.GetPerformanceGradeAsync();

            var currentPeriod = await commonService.GetCurrentPerformancePeriodAsync();

            foreach (var performanceRecord in performanceRecords)
            {
                MappingDataForPerformanceRecordAsync(performanceRecord, performanceGrades, param.ActorId, currentPeriod);
            }

            foreach (var member in memberPerformances)
            {
                member.GradeId = cryptoService.Decrypt(member.GradeId);
                _ = int.TryParse(member.GradeId, out int gradeId);
                member.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

                member.PerformanceRecords = performanceRecords.Where(t => t.Employees.First().EmployeeId == member.EmployeeId);
            }

            return memberPerformances;
        }

        public async Task<PerformanceRecord> GetPerformanceRecordByIdAsync(int performanceRecordId, int creatorId)
        {
            var criteria = new GetPerformanceRecordByIdCriteria
            {
                PerformanceRecordId = performanceRecordId,
                CreatorId = creatorId
            };

            var currentPeriod = await commonService.GetCurrentPerformancePeriodAsync();

            var performanceRecord = await performanceRecordRepository.GetPerformanceRecordByIdAsync(criteria);
            var performanceGrades = await commonService.GetPerformanceGradeAsync();
            MappingDataForPerformanceRecordAsync(performanceRecord, performanceGrades, creatorId, currentPeriod);

            return performanceRecord;
        }

        public async Task<PerformanceRecord> GetDraftPerformanceRecordByIdAsync(int performanceRecordId, int creatorId)
        {
            var criteria = new GetDraftPerformanceRecordByIdCriteria
            {
                PerformanceRecordId = performanceRecordId
            };

            var currentPeriod = await commonService.GetCurrentPerformancePeriodAsync();

            var performanceRecord = await performanceRecordRepository.GetDraftPerformanceRecordByIdAsync(criteria);
            var performanceGrades = await commonService.GetPerformanceGradeAsync();
            MappingDataForPerformanceRecordAsync(performanceRecord, performanceGrades, creatorId, currentPeriod);

            return performanceRecord;
        }

        public async Task<IEnumerable<PerformanceRecord>> GetPerformanceRecordByIdsAsync(string performanceRecordIds, int creatorId)
        {
            var criteria = new GetPerformanceRecordByIdsCriteria
            {
                PerformanceRecordIds = performanceRecordIds,
                CreatorId = creatorId
            };

            var performanceRecords = await performanceRecordRepository.GetPerformanceRecordByIdsAsync(criteria);
            var performanceGrades = await commonService.GetPerformanceGradeAsync();
            var currentPeriod = await commonService.GetCurrentPerformancePeriodAsync();

            foreach (var performanceRecord in performanceRecords)
            {
                MappingDataForPerformanceRecordAsync(performanceRecord, performanceGrades, creatorId, currentPeriod);
            }

            return performanceRecords;
        }

        public async Task<CommandResult> CreatePerformanceRecordAsync(CreatePerformanceRecordDto performanceRecord)
        {
            var criteria = new CreatePerformanceRecordCriteria
            {
                EmployeeIds = performanceRecord.EmployeeIds,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = commonService.RemoveInvalidXmlChars(performanceRecord.Comment),
                ProjectIds = performanceRecord.ProjectIds,
                TypeId = performanceRecord.TypeId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
            commandResult.ObjectIds = criteria.PerformanceRecordIds;

            if (commandResult.IsSuccessful)
            {
                backgroundJob.Enqueue(() => notificationService.SendMailAsync(criteria.PerformanceRecordIds, performanceRecord.CreatorId, ActionOnPerformanceRecord.Create));
                backgroundJob.Enqueue(() => notificationService.SendNotificationAsync(
                    criteria.PerformanceRecordIds,
                    performanceRecord.CreatorId,
                    ActionOnPerformanceRecord.Create)
                    );
                backgroundJob.Enqueue(() => notificationService.SendSkypeMessageAsync(
                    criteria.PerformanceRecordIds,
                    performanceRecord.CreatorId,
                    ActionOnPerformanceRecord.Create)
                    );
            }

            return commandResult;
        }

        public async Task<CommandResult> CreateDraftPerformanceRecordAsync(CreatePerformanceRecordDto performanceRecord)
        {
            var criteria = new CreateDraftPerformanceRecordCriteria
            {
                EmployeeIds = performanceRecord.EmployeeIds,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = commonService.RemoveInvalidXmlChars(performanceRecord.Comment),
                ProjectIds = performanceRecord.ProjectIds,
                TypeId = performanceRecord.TypeId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
            commandResult.ObjectIds = criteria.PerformanceRecordIds;

            return commandResult;
        }

        public async Task<CommandResult> EditPerformanceRecordAsync(EditPerformanceRecordDto performanceRecord)
        {
            var criteria = new EditPerformanceRecordCriteria
            {
                PerformanceRecordId = performanceRecord.PerformanceRecordId,
                EmployeeIds = performanceRecord.EmployeeIds,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = commonService.RemoveInvalidXmlChars(performanceRecord.Comment),
                ProjectIds = performanceRecord.ProjectIds,
                TypeId = performanceRecord.TypeId,
                IsFromDraft = performanceRecord.IsFromDraft
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
            commandResult.ObjectIds = criteria.PerformanceRecordIds;

            if (commandResult.IsSuccessful)
            {
                if (performanceRecord.IsFromDraft)
                {
                    backgroundJob.Enqueue(() => notificationService.SendMailAsync(criteria.PerformanceRecordIds, performanceRecord.CreatorId, ActionOnPerformanceRecord.Create));
                    backgroundJob.Enqueue(() => notificationService.SendNotificationAsync(criteria.PerformanceRecordIds,
                        performanceRecord.CreatorId,
                        ActionOnPerformanceRecord.Create)
                        );
                    backgroundJob.Enqueue(() => notificationService.SendSkypeMessageAsync(criteria.PerformanceRecordIds,
                        performanceRecord.CreatorId,
                        ActionOnPerformanceRecord.Create)
                        );
                }
                else
                {
                    backgroundJob.Enqueue(() => notificationService.SendMailAsync(criteria.PerformanceRecordIds, performanceRecord.CreatorId, ActionOnPerformanceRecord.Edit));
                    backgroundJob.Enqueue(() => notificationService.SendNotificationAsync(criteria.PerformanceRecordIds,
                        performanceRecord.CreatorId,
                        ActionOnPerformanceRecord.Edit)
                        );
                    backgroundJob.Enqueue(() => notificationService.SendSkypeMessageAsync(criteria.PerformanceRecordIds,
                        performanceRecord.CreatorId,
                        ActionOnPerformanceRecord.Edit)
                        );
                }
            }

            return commandResult;
        }

        public async Task<CommandResult> EditDraftPerformanceRecordAsync(EditPerformanceRecordDto performanceRecord)
        {
            var criteria = new EditDraftPerformanceRecordCriteria
            {
                PerformanceRecordId = performanceRecord.PerformanceRecordId,
                EmployeeIds = performanceRecord.EmployeeIds,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = commonService.RemoveInvalidXmlChars(performanceRecord.Comment),
                ProjectIds = performanceRecord.ProjectIds,
                TypeId = performanceRecord.TypeId
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> DeletePerformanceRecordAsync(int performanceRecordId, int actorId)
        {
            var criteria = new DeletePerformanceRecordCriteria
            {
                PerformanceRecordId = performanceRecordId,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        private void MappingDataForPerformanceRecordAsync(PerformanceRecord performanceRecord, IEnumerable<PerformanceGrade> performanceGrades, int actorId, PeriodModel currentPeriod)
        {
            var today = DateTime.Now;
            performanceRecord.GradeId = cryptoService.Decrypt(performanceRecord.GradeId);
            _ = int.TryParse(performanceRecord.GradeId, out int gradeId);
            performanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
            var createdDate = performanceRecord.CreatedDate ?? DateTime.MinValue;

            performanceRecord.IsAllowEditFeedback = performanceRecord.CreatedDate >= currentPeriod.DisciplineStartDate && currentPeriod.StartDate > today;

            performanceRecord.IsHost = performanceRecord.Creator.EmployeeId == actorId
                && (performanceRecord.IsDraft
                    || ((today - createdDate).Days < Constants.PerformanceCheckinDuration && currentPeriod.StartDate > today));

            if (performanceRecord.Creator.EmployeeId == Constants.NexGoalSystemId)
            {
                performanceRecord.Creator = NexGoalSystemInfo.NexGoal;
            }
        }

        private void DecryptPAGrade(PASummaryModel paSummary, IEnumerable<PerformanceGrade> performanceGrades)
        {
            paSummary.OriginalGradeId = cryptoService.Decrypt(paSummary.OriginalGradeId);
            _ = int.TryParse(paSummary.OriginalGradeId, out int gradeId);
            paSummary.OriginalGrade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            foreach (var detail in paSummary.GradeDetails)
            {
                detail.Grade = MappingGrade(detail, performanceGrades);
            }
        }

        private string MappingGrade(PAGradeDetailModel process, IEnumerable<PerformanceGrade> performanceGrades)
        {
            process.GradeId = cryptoService.Decrypt(process.GradeId);
            _ = int.TryParse(process.GradeId, out int gradeId);
            var grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            if (grade != null)
            {
                return grade.Name;
            }

            return !string.IsNullOrWhiteSpace(process.RelatedPerformanceRecordIds) ? ViewLang.NA : string.Empty;
        }
    }
}