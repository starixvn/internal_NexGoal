namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Redmine;
    using NexGoal.Models;
    using NexGoal.Models.Redmine;

    public class RedmineService : IRedmineService
    {
        private readonly IBaseService baseService;

        public RedmineService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<TeamWorkloadModel>> GetTeamWorkload(int EmployeeId, DateTime from, DateTime to)
        {
            var criteria = new GetTeamWorkloadCriteria
            {
                EmployeeId = EmployeeId,
                FromDate = from,
                ToDate = to
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

        private IEnumerable<TeamWorkloadModel> GenerateTeamWorkload(IEnumerable<MembersWorkloadEntityModel> rawWorkloads)
        {
            var workloads = GroupWorkloadByTeam(rawWorkloads);

            var teamWorkloads = new List<TeamWorkloadModel>();
            foreach (var workload in workloads)
            {
                var fullProjects = GetAllProjects(workload);
                var fullWorkloads = FillFullWorkload(workload, fullProjects);
                var teamWorkload = new TeamWorkloadModel
                {
                    GroupId = workload.GroupId,
                    GroupName = workload.GroupName,
                    EmployeeWorkloads = fullWorkloads
                };
                teamWorkloads.Add(teamWorkload);
            }

            return teamWorkloads;
        }

        private IEnumerable<TeamWorkloadModel> GroupWorkloadByTeam(IEnumerable<MembersWorkloadEntityModel> workloadEntries)
        {
            var teamWorkloads = workloadEntries.GroupBy(w => new { w.GroupId, w.GroupName }).Select(i => new TeamWorkloadModel
            {
                GroupId = i.Key.GroupId,
                GroupName = i.Key.GroupName,
                EmployeeWorkloads = i.GroupBy(e => new { e.EmpId, e.NickName }).Select(x => new EmployeeWorkloadModel
                {
                    EmployeeId = x.Key.EmpId,
                    NickName = x.Key.NickName,
                    Workloads = x.Select(y => new ProjectWorkloadModel
                    {
                        ProjectId = y.ProjectId,
                        Name = y.ProjectName,
                        TotalHours = y.TotalHour
                    }).ToList()
                })
            });

            return teamWorkloads;
        }

        private IEnumerable<ProjectModel> GetAllProjects(TeamWorkloadModel teamWorkload)
        {
            var workloads = new List<ProjectModel>();
            foreach (var employeeWorkload in teamWorkload.EmployeeWorkloads)
            {
                foreach (var workload in employeeWorkload.Workloads)
                {
                    workloads.Add(new ProjectModel
                    {
                        ProjectId = workload.ProjectId,
                        Name = workload.Name
                    });
                }
            }

            return workloads.GroupBy(w => w.ProjectId).Select(g => g.First());
        }

        private IEnumerable<EmployeeWorkloadModel> FillFullWorkload(TeamWorkloadModel teamWorkloads, IEnumerable<ProjectModel> projects)
        {
            IList<EmployeeWorkloadModel> data = new List<EmployeeWorkloadModel>();
            foreach (var workload in teamWorkloads.EmployeeWorkloads)
            {
                var notExistProjects = projects.Where(p => !workload.Workloads.Any(w => w.ProjectId == p.ProjectId));
                var oldWorkload = workload.Workloads;
                if (notExistProjects.Any())
                {
                    foreach (var p in notExistProjects)
                    {
                        var newProject = new ProjectWorkloadModel
                        {
                            ProjectId = p.ProjectId,
                            Name = p.Name,
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