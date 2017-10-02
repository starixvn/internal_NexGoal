namespace NexGoal.Services.Implement
{
    using Criteria.Dashboard;
    using Interfaces;
    using Model.Dashboard;
    using Repositories.Implement;
    using Repositories.Interfaces;

    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository dashboardRepository;

        public DashboardService() : this(null)
        {
        }

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            this.dashboardRepository = dashboardRepository ?? new DashboardRepository();
        }

        public ToDoListModel GetToDoList(int employeeId)
        {
            var criteria = new GetToDoListCriteria
            {
                EmployeeId = employeeId
            };
            var result = new ToDoListModel();

            if (criteria.IsValid())
            {
                result = dashboardRepository.GetToDoList(criteria);
            }

            return result;
        }
    }
}