namespace NexGoal.Services.Interfaces
{
    using NexGoal.Criteria.Department;
    using NexGoal.Model.Department;
    using System.Collections.Generic;

    public interface IDepartmentService
    {
        List<DepartmentModel> GetDepartment(GetDepartmentCriteria criteria);
    }
}