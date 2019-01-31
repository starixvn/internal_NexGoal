namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceRecord;
    using NexGoal.Models;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Repositories.Interfaces;

    public class PerformanceRecordService : IPerformanceRecordService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;
        private readonly INotificationPerformanceRecordService notificationService;
        private readonly IPerformanceRecordRepository performanceRecordRepository;

        public PerformanceRecordService(
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService,
            INotificationPerformanceRecordService notificationService,
            IPerformanceRecordRepository performanceRecordRepository)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
            this.notificationService = notificationService;
            this.performanceRecordRepository = performanceRecordRepository;
        }

        public async Task<IEnumerable<PerformanceRecord>> GetPerformanceRecordByEmployeeAsync(int employeeId)
        {
            var criteria = new GetPerformanceRecordByEmployeeCriteria
            {
                EmployeeId = employeeId
            };
            var tuple = await performanceRecordRepository.GetPerformanceRecordByEmployeeAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            foreach (var performanceRecord in tuple.Item1)
            {
                performanceRecord.Projects = tuple.Item2.Where(t => t.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                MappingDataForPerformanceRecord(performanceRecord, performanceGrades, employeeId);
            }

            return tuple.Item1;
        }

        public async Task<Tuple<IEnumerable<PerformanceRecord>, int>> GetPerformanceTimelineAsync(GetPerformanceTimelineParam param)
        {
            var criteria = new GetPerformanceTimelineCriteria
            {
                EmployeeId = param.EmployeeId,
                PageIndex = param.PageIndex,
                PageSize = param.PageSize
            };
            var performanceRecords = await performanceRecordRepository.GetPerformanceTimelineAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            foreach (var performanceRecord in performanceRecords)
            {
                MappingDataForPerformanceRecord(performanceRecord, performanceGrades, param.EmployeeId);
            }

            return new Tuple<IEnumerable<PerformanceRecord>, int>(performanceRecords, criteria.TotalItems);
        }

        public async Task<IEnumerable<PerformanceRecord>> GetMemberPerformanceAsync(int actorId)
        {
            var criteria = new GetMemberPerformanceCriteria
            {
                ActorId = actorId
            };
            var tuple = await performanceRecordRepository.GetMemberPerformanceAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            foreach (var performanceRecord in tuple.Item1)
            {
                performanceRecord.Employees = tuple.Item2.Where(t => t.LinkObjectId == performanceRecord.PerformanceRecordId);
                performanceRecord.Projects = tuple.Item3.Where(t => t.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                MappingDataForPerformanceRecord(performanceRecord, performanceGrades, actorId);
            }

            return tuple.Item1;
        }

        public async Task<IEnumerable<PerformanceRecord>> GetMemberPerformanceDetailAsync(int actorId, int employeeId)
        {
            var criteria = new GetMemberPerformanceDetailCriteria
            {
                ActorId = actorId,
                EmployeeId = employeeId
            };

            var tuple = await performanceRecordRepository.GetMemberPerformanceDetailAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            foreach (var performanceRecord in tuple.Item1)
            {
                performanceRecord.Employees = tuple.Item2.Where(t => t.LinkObjectId == performanceRecord.PerformanceRecordId);
                performanceRecord.Projects = tuple.Item3.Where(t => t.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                MappingDataForPerformanceRecord(performanceRecord, performanceGrades, actorId);
            }

            return tuple.Item1;
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

            var tuple = await performanceRecordRepository.GetMemberPerformanceTimelineAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            foreach (var performanceRecord in tuple.Item2)
            {
                MappingDataForPerformanceRecord(performanceRecord, performanceGrades, param.ActorId);
            }

            foreach (var member in tuple.Item1)
            {
                member.PerformanceRecords = tuple.Item2.Where(t => t.Employees.FirstOrDefault().EmployeeId == member.EmployeeId);
            }

            return tuple.Item1;
        }

        public async Task<PerformanceRecord> GetPerformanceRecordByIdAsync(int performanceRecordId, int creatorId)
        {
            var criteria = new GetPerformanceRecordByIdCriteria
            {
                PerformanceRecordId = performanceRecordId,
                CreatorId = creatorId
            };

            var performanceRecord = await performanceRecordRepository.GetPerformanceRecordByIdAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            MappingDataForPerformanceRecord(performanceRecord, performanceGrades, creatorId);

            return performanceRecord;
        }

        public async Task<PerformanceRecord> GetDraftPerformanceRecordByIdAsync(int performanceRecordId, int creatorId)
        {
            var criteria = new GetDraftPerformanceRecordByIdCriteria
            {
                PerformanceRecordId = performanceRecordId
            };

            var performanceRecord = await performanceRecordRepository.GetDraftPerformanceRecordByIdAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            MappingDataForPerformanceRecord(performanceRecord, performanceGrades, creatorId);

            return performanceRecord;
        }

        public async Task<IEnumerable<PerformanceRecord>> GetPerformanceRecordByIdsAsync(string performanceRecordIds, int creatorId)
        {
            var criteria = new GetPerformanceRecordByIdsCriteria
            {
                PerformanceRecordIds = performanceRecordIds,
                CreatorId = creatorId
            };

            var tuple = await performanceRecordRepository.GetPerformanceRecordByIdsAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            foreach (var performanceRecord in tuple.Item1)
            {
                performanceRecord.Projects = tuple.Item2.Where(t => t.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                MappingDataForPerformanceRecord(performanceRecord, performanceGrades, creatorId);
            }

            return tuple.Item1;
        }

        public async Task<CommandResult> CreatePerformanceRecordAsync(CreatePerformanceRecordModel performanceRecord)
        {
            var criteria = new CreatePerformanceRecordCriteria
            {
                EmployeeIds = performanceRecord.EmployeeIds,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = performanceRecord.Comment,
                ProjectIds = performanceRecord.ProjectIds,
                TypeId = performanceRecord.TypeId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            commandResult.ObjectIds = criteria.PerformanceRecordIds;

            if (commandResult.IsSuccessful)
            {
                await notificationService.SendMailAsync(criteria.PerformanceRecordIds, performanceRecord.CreatorId, ActionOnPerformanceRecord.Create).ConfigureAwait(false);
                await notificationService.SendNotificationAsync(criteria.PerformanceRecordIds, performanceRecord.CreatorId, ActionOnPerformanceRecord.Create).ConfigureAwait(false);
            }

            return commandResult;
        }

        public async Task<CommandResult> CreateDraftPerformanceRecordAsync(CreatePerformanceRecordModel performanceRecord)
        {
            var criteria = new CreateDraftPerformanceRecordCriteria
            {
                EmployeeIds = performanceRecord.EmployeeIds,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = performanceRecord.Comment,
                ProjectIds = performanceRecord.ProjectIds,
                TypeId = performanceRecord.TypeId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            commandResult.ObjectIds = criteria.PerformanceRecordIds;

            return commandResult;
        }

        public async Task<CommandResult> EditPerformanceRecordAsync(EditPerformanceRecordModel performanceRecord)
        {
            var criteria = new EditPerformanceRecordCriteria
            {
                PerformanceRecordId = performanceRecord.PerformanceRecordId,
                EmployeeIds = performanceRecord.EmployeeIds,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = performanceRecord.Comment,
                ProjectIds = performanceRecord.ProjectIds,
                TypeId = performanceRecord.TypeId,
                IsFromDraft = performanceRecord.IsFromDraft
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            commandResult.ObjectIds = criteria.PerformanceRecordIds;

            if (commandResult.IsSuccessful)
            {
                if (performanceRecord.IsFromDraft)
                {
                    await notificationService.SendMailAsync(criteria.PerformanceRecordIds, performanceRecord.CreatorId, ActionOnPerformanceRecord.Create).ConfigureAwait(false);
                    await notificationService.SendNotificationAsync(criteria.PerformanceRecordIds, performanceRecord.CreatorId, ActionOnPerformanceRecord.Create).ConfigureAwait(false);
                }
                else
                {
                    await notificationService.SendMailAsync(criteria.PerformanceRecordIds, performanceRecord.CreatorId, ActionOnPerformanceRecord.Edit).ConfigureAwait(false);
                    await notificationService.SendNotificationAsync(criteria.PerformanceRecordIds, performanceRecord.CreatorId, ActionOnPerformanceRecord.Edit).ConfigureAwait(false);
                }
            }

            return commandResult;
        }

        public async Task<CommandResult> EditDraftPerformanceRecordAsync(EditPerformanceRecordModel performanceRecord)
        {
            var criteria = new EditDraftPerformanceRecordCriteria
            {
                PerformanceRecordId = performanceRecord.PerformanceRecordId,
                EmployeeIds = performanceRecord.EmployeeIds,
                CreatorId = performanceRecord.CreatorId,
                GradeId = cryptoService.Encrypt(performanceRecord.GradeId),
                Comment = performanceRecord.Comment,
                ProjectIds = performanceRecord.ProjectIds,
                TypeId = performanceRecord.TypeId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            return commandResult;
        }

        public async Task<CommandResult> DeletePerformanceRecordAsync(int performanceRecordId, int actorId)
        {
            var criteria = new DeletePerformanceRecordCriteria
            {
                PerformanceRecordId = performanceRecordId,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        private void MappingDataForPerformanceRecord(PerformanceRecord performanceRecord, IEnumerable<PerformanceGradeModel> performanceGrades, int actorId)
        {
            performanceRecord.GradeId = cryptoService.Decrypt(performanceRecord.GradeId);
            int.TryParse(performanceRecord.GradeId, out int gradeId);
            performanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            performanceRecord.IsHost = performanceRecord.Creator.EmployeeId == actorId
                    && (performanceRecord.IsDraft || (!performanceRecord.IsDraft && (DateTime.Now - performanceRecord.CreatedDate).Days < 7));

            if (performanceRecord.Creator.EmployeeId == Constants.NexGoalSystemId)
            {
                performanceRecord.Creator = NexGoalSystemInfo.NexGoal;
            }
        }
    }
}