namespace NexGoal.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model;
    using NexGoal.Model.Department;
    using NexGoal.Model.Enitites;
    using NexGoal.Repositories.Interfaces;

    public class ManageNominationRepository : IManageNominationRepository
    {
        public IEnumerable<NominatorSettingModel> GetNominatorSettingById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IEnumerable<NominatorSettingModel> nominators =
                    db.Query<NominatorSettingModel, NomineeModel, DepartmentModel, Group, JobTitle, NominatorSettingModel>(
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