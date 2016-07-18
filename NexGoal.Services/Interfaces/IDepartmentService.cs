namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Criteria.Department;
    using NexGoal.Model.Department;

    public interface IDepartmentService
    {
        List<DepartmentModel> GetDepartment(GetDepartmentCriteria criteria);
    }
}