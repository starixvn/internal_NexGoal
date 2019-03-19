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

        public async Task<IEnumerable<TeamWorkloadModel>> GetTeamWorkloadAsync(int EmployeeId, int periodId)
        {
            var criteria = new GetTeamWorkloadCriteria
            {
                EmployeeId = EmployeeId,
                PeriodId = periodId
            };

            var workloads = await baseService.GetDataAsync<MembersWorkloadEntityModel>(criteria).ConfigureAwait(false);

            return GenerateTeamWorkload(workloads);
        }

        public async Task<IEnumerable<ProjectWorkloadModel>> GetWorkloadAsync(int employeeId, int periodId)
        {
            var criteria = new GetWorkloadByEmployeeIdCriteria
            {
                EmployeeId = employeeId,
                PeriodId = periodId
            };

            var workloads = await baseService.GetDataAsync<ProjectWorkloadModel>(criteria).ConfigureAwait(false);

            return GenerateWorkload(workloads);
        }

        private IEnumerable<ProjectWorkloadModel> GenerateWorkload(IEnumerable<ProjectWorkloadModel> projectWorkloads)
        {
            var workloads = projectWorkloads.GroupBy(x => x.Name).Select(y => new ProjectWorkloadModel
            {
                ProjectId = y.Min(pid => pid.ProjectId),
                Name = y.Key,
                TotalHours = y.Sum(t => t.TotalHours)
            });

            return workloads.OrderByDescending(x => x.TotalHours);
        }

        private IEnumerable<TeamWorkloadModel> GenerateTeamWorkload(IEnumerable<MembersWorkloadEntityModel> rawWorkloads)
        {
            var teamWorkloads = rawWorkloads.GroupBy(w => new { w.GroupId, w.GroupName }).Select(i => new TeamWorkloadModel
            {
                GroupId = i.Key.GroupId,
                GroupName = i.Key.GroupName,
                EmployeeWorkloads = i.GroupBy(e => new { e.EmployeeId, e.NickName }).Select(x => new EmployeeWorkloadModel
                {
                    EmployeeId = x.Key.EmployeeId,
                    NickName = x.Key.NickName,
                    Workloads = x.Where(p => p.ProjectId > 0).GroupBy(p => p.ProjectName).Select(p => new ProjectWorkloadModel
                    {
                        ProjectId = p.Min(pid => pid.ProjectId),
                        Name = p.Key,
                        TotalHours = p.Sum(h => h.TotalHour)
                    })
                })
            });

            return teamWorkloads;
        }
    }
}