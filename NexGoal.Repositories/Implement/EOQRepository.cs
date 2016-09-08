namespace NexGoal.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model;
    using NexGoal.Model.Department;
    using NexGoal.Model.Employee;
    using NexGoal.Model.Enitites;
    using NexGoal.Repositories.Interfaces;

    public class EOQRepository : IEOQRepository
    {
        public IEnumerable<EOQYNominatorSettingModel> GetEOQNominatorSettingById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IEnumerable<EOQYNominatorSettingModel> nominators =
                    db.Query<EOQYNominatorSettingModel, EmployeeModel, DepartmentModel, Group, JobTitle, EOQYNominatorSettingModel>(
                        (nominator, emp, dept, gr, jb) =>
                        {
                            emp.Department = dept;
                            emp.Group = gr;
                            emp.JobTitle = jb;
                            nominator.NominatorInfo = emp;

                            return nominator;
                        },
                        "EmployeeId, DepartmentId, GroupId, JobTitleId",
                        criteria.GetSpParams());

                return nominators;
            }
        }

        public EOQYNominatorSettingModel GetCurrentEOQByNominatorId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<EOQYNominatorSettingModel, EOQYModel, EOQYNominatorSettingModel>(
                    (setting, period) =>
                    {
                        setting.Period = period;
                        return setting;
                    },
                    "EOQYId",
                    criteria.GetSpParams());
            }
        }

        public IEnumerable<EOQYNominationModel> GetNominationByNominatorId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IEnumerable<EOQYNominationModel> nominations =
                    db.Query<EOQYNominationModel, EOQYModel, EmployeeModel, DepartmentModel, Group, JobTitle, EmployeeModel, EOQYNominationModel>(
                        (nomination, period, nominee, dept, gr, jb, nominator) =>
                        {
                            nomination.Period = period;
                            nominee.Department = dept;
                            nominee.Group = gr;
                            nominee.JobTitle = jb;
                            nomination.Nominee = nominee;
                            nomination.Nominator = nominator;

                            return nomination;
                        },
                        "EOQYId, EmployeeId, DepartmentId, GroupId, JobTitleId, NominatorId",
                        criteria.GetSpParams());

                return nominations;
            }
        }

        public EOQYNominationModel GetEOQNominationById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    EOQYNominationModel nomination =
                        multi.Read<EOQYNominationModel, EOQYModel, EmployeeModel, DepartmentModel, Group, JobTitle, EmployeeModel, EOQYNominationModel>(
                            (nt, period, nominee, dept, gr, jb, nominator) =>
                            {
                                nt.Period = period;
                                nominee.Department = dept;
                                nominee.Group = gr;
                                nominee.JobTitle = jb;
                                nt.Nominee = nominee;
                                nt.Nominator = nominator;

                                return nt;
                            },
                            "EOQYId, EmployeeId, DepartmentId, GroupId, JobTitleId, NominatorId").FirstOrDefault();

                    nomination.NominationDetails = multi.Read<EOQYQuestionModel>().ToList();

                    return nomination;
                }
            }
        }
    }
}