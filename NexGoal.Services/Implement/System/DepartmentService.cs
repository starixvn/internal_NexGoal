namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Department;
    using NexGoal.Model.Department;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;

    public class DepartmentService : BaseService, IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService()
            : this(null)
        {
        }

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository ?? new DepartmentRepository();
        }

        public List<DepartmentModel> GetDepartment(GetDepartmentCriteria criteria)
        {
            List<DepartmentModel> departments = new List<DepartmentModel>();

            if (criteria != null && criteria.IsValid())
            {
                departments = _departmentRepository.GetDepartment(criteria);
            }

            return departments;
        }
    }
}