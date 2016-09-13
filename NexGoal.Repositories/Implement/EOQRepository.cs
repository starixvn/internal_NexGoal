namespace NexGoal.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model;
    using NexGoal.Model.Department;
    using NexGoal.Model.Enitites;
    using NexGoal.Repositories.Interfaces;

    public class EOQRepository : IEOQRepository
    {
        public IEnumerable<EOQYNominatorSettingModel> GetEOQNominatorSettingById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IEnumerable<EOQYNominatorSettingModel> nominators =
                    db.Query<EOQYNominatorSettingModel, NomineeModel, DepartmentModel, Group, JobTitle, EOQYNominatorSettingModel>(
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
    }
}