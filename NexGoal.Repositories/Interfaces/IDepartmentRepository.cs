namespace NexGoal.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Model.Department;
    using System.Collections.Generic;

    public interface IDepartmentRepository
    {
        List<DepartmentModel> GetDepartment(ICriteria criteria);
    }
}