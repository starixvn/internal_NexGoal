namespace NexGoal.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NexGoal.Model.Department;

    public interface IDepartmentRepository
    {
        List<DepartmentModel> GetDepartment(ICriteria criteria);
    }
}