namespace NexGoal.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Model.Authentication;
    using NexGoal.Model.Employee;

    public interface IEmployeeRepository
    {
        RawEmployeeModel GetAllEmployee(ICriteria criteria);

        UserSession GetEmployee(ICriteria criteria);

        EmployeeModel GetEmployeeById(ICriteria criteria);
    }
}