namespace NexGoal.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model.Department;
    using NexGoal.Model.Enitites;
    using NexGoal.Repositories.Interfaces;

    public class DepartmentRepository : IDepartmentRepository
    {
        public List<DepartmentModel> GetDepartment(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                List<DepartmentModel> departments = db.Query<DepartmentModel, Employee, DepartmentModel>(
                   (dept, manager) =>
                   {
                       dept.Manager = manager;

                       return dept;
                   },
                   "ManagerId",
                   criteria.GetSpParams()).ToList();

                if (departments.Any())
                {
                    departments.ForEach(x => x.Childrens.AddRange(departments.Where(y => y.ParentId == x.DepartmentId)));
                }

                return departments;
            }
        }
    }
}