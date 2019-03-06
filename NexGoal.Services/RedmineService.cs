namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Redmine;
    using NexGoal.Models.Redmine;

    public class RedmineService : IRedmineService
    {
        private readonly IBaseService baseService;

        public RedmineService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<TeamWorkloadModel>> GetTeamWorkload(int EmployeeId, int periodId)
        {
            var criteria = new GetTeamWorkloadCriteria
            {
                EmployeeId = EmployeeId,
                PeriodId = periodId
            };

            var workloads = await baseService.GetDataAsync<MembersWorkloadEntityModel>(criteria).ConfigureAwait(false);

            var teamWorkloads = GenerateTeamWorkload(workloads);

            return teamWorkloads;
        }

        public async Task<IEnumerable<ProjectWorkloadModel>> GetWorkloadAsync(int employeeId, int periodId)
        {
            var criteria = new GetWorkloadByEmployeeIdCriteria
            {
                EmployeeId = employeeId,
                PeriodId = periodId
            };

            var workload = await baseService.GetDataAsync<ProjectWorkloadModel>(criteria).ConfigureAwait(false);

            return workload.OrderByDescending(x => x.TotalHours);
        }

        private IEnumerable<TeamWorkloadModel> GenerateTeamWorkload(IEnumerable<MembersWorkloadEntityModel> rawWorkloads)
        {
            var workloads = GroupWorkloadByTeam(rawWorkloads);

            return workloads;
        }

        private IEnumerable<TeamWorkloadModel> GroupWorkloadByTeam(IEnumerable<MembersWorkloadEntityModel> workloadEntries)
        {
            var teamWorkloads = workloadEntries.GroupBy(w => new { w.GroupId, w.GroupName }).Select(i => new TeamWorkloadModel
            {
                GroupId = i.Key.GroupId,
                GroupName = i.Key.GroupName,
                EmployeeWorkloads = i.GroupBy(e => new { e.EmployeeId, e.NickName }).Select(x => new EmployeeWorkloadModel
                {
                    EmployeeId = x.Key.EmployeeId,
                    NickName = x.Key.NickName,
                    Workloads = x.GroupBy(p => new { p.ProjectId, p.ProjectName }).Select(p => new ProjectWorkloadModel
                    {
                        ProjectId = p.Key.ProjectId,
                        Name = p.Key.ProjectName,
                        TotalHours = p.Sum(h => h.TotalHour)
                    })
                })
            });

            return teamWorkloads;
        }
    }
}