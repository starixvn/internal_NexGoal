namespace NexGoal.Services
{
    using System;
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

        public async Task<IList<EmployeeWorkloadModel>> GetTeamWorkload(int EmployeeId, DateTime from, DateTime to)
        {
            var criteria = new GetTeamWorkloadCriteria
            {
                EmployeeId = EmployeeId,
                FromDate = from,
                ToDate = to
            };

            var workloadEntries = await baseService.GetDataAsync<MembersWorkloadEntityModel>(criteria).ConfigureAwait(false);

            IList<EmployeeWorkloadModel> data = FillFullWorkload(workloadEntries);

            return data;
        }

        public async Task<IEnumerable<ProjectWorkloadModel>> GetWorkloadAsync(int employeeId)
        {
            var criteria = new GetWorkloadByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<ProjectWorkloadModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ProjectWorkloadModel>> GetWorkloadForPMAsync(int employeeId)
        {
            var criteria = new GetWorkloadForPMCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<ProjectWorkloadModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ProjectWorkloadModel>> GetWorkloadForLeaderAsync(int employeeId)
        {
            var criteria = new GetWorkloadForLeaderCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<ProjectWorkloadModel>(criteria).ConfigureAwait(false);
        }

        private IList<EmployeeWorkloadModel> FillFullWorkload(IEnumerable<MembersWorkloadEntityModel> workloadEntries)
        {
            var workloads = workloadEntries
                    .GroupBy(w => new { w.EmpId, w.NickName })
                    .Select(x => new EmployeeWorkloadModel
                    {
                        EmployeeId = x.Key.EmpId,
                        NickName = x.Key.NickName,
                        Workloads = x.Select(y => new ProjectWorkloadModel
                        {
                            ProjectId = y.ProjectId,
                            Name = y.ProjectName,
                            TotalHours = y.TotalHour
                        }).ToList()
                    });

            var allProjects = workloadEntries.GroupBy(p => new { p.ProjectId, p.ProjectName }).Select(x => x.Key).ToList();

            IList<EmployeeWorkloadModel> data = new List<EmployeeWorkloadModel>();
            foreach (var workload in workloads)
            {
                var notExistProjects = allProjects.Where(p => !workload.Workloads.Any(w => w.ProjectId == p.ProjectId));
                var oldWorkload = workload.Workloads;
                if (notExistProjects.Any())
                {
                    foreach (var p in notExistProjects)
                    {
                        var newProject = new ProjectWorkloadModel
                        {
                            ProjectId = p.ProjectId,
                            Name = p.ProjectName,
                            TotalHours = 0
                        };
                        oldWorkload.Add(newProject);
                    }
                }
                workload.Workloads = oldWorkload.OrderBy(x => x.ProjectId).ToList();
                data.Add(workload);
            }

            return data.ToList();
        }
    }
}