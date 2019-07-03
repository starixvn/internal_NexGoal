namespace NexGoal.Services
{
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Probation;
    using NexGoal.Models.Common;
    using NexGoal.Models.Probation;
    using NexGoal.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ProbationReportService : IProbationReportService
    {
        private readonly IProbationRepository probationRepository;

        private readonly IBaseService baseService;

        public ProbationReportService(IProbationRepository probationRepository, IBaseService baseService)
        {
            this.probationRepository = probationRepository;
            this.baseService = baseService;
        }

        public async Task<Tuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndEmployeesAsync(int actorId)
        {
            return await probationRepository.GetGroupsAndEmployeesAsync(new GetGroupsAndEmployeesCriteria
            {
                ActorId = actorId
            });
        }

        public async Task<IEnumerable<ProbationReportModel>> GetAsync(GetProbationReportModel model)
        {
            return await baseService.GetDataAsync<ProbationReportModel>(new GetProbationReportCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeIds = model.EmployeeIds,
                IsQualified = model.IsQualified,
                IsQualifiedByEvaluator = model.IsQualifiedByEvaluator,
                ActorId = model.ActorId
            });
        }
    }
}