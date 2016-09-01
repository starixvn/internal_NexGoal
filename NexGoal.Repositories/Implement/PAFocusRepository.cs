namespace NexGoal.Repositories.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model.Employee;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.PAFocus;
    using NexGoal.Repositories.Interfaces;
    using System.Collections.Generic;

    public class PAFocusRepository : IPAFocusRepository
    {
        public IEnumerable<PAFocusModel> GetPAFocus(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<PAFocusModel, EmployeeModel, JobTitle, Group, PAFocusModel>(
                    (att, emp, title, gr) =>
                    {
                        att.Employee = emp;
                        att.Employee.JobTitle = title;
                        att.Employee.Group = gr;
                        return att;
                    },
                    "EmployeeId, JobTitleId, GroupId",
                    criteria.GetSpParams());
            }
        }
    }
}