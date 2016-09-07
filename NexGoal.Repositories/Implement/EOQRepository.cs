namespace NexGoal.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model;
    using NexGoal.Model.Department;
    using NexGoal.Model.Employee;
    using NexGoal.Model.Enitites;
    using NexGoal.Repositories.Interfaces;

    public class EOQRepository : IEOQRepository
    {
        public IEnumerable<EOQNominatorSettingModel> GetEOQNominatorSettingById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EOQNominatorSettingModel> nominators =
                        multi.Read<EOQNominatorSettingModel, EmployeeModel, DepartmentModel, Group, JobTitle, EOQNominatorSettingModel>(
                           (nominator, emp, dept, gr, jb) =>
                           {
                               emp.Department = dept;
                               emp.Group = gr;
                               emp.JobTitle = jb;
                               nominator.NominatorInfo = emp;

                               return nominator;
                           },
                           "EmployeeId, DepartmentId, GroupId, JobTitleId");

                    return nominators;
                }
            }
        }
    }
}