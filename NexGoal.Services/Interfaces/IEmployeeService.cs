namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Employee;
    using NexGoal.Models.PARecord;

    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeBaseModel>> GetEmployeesAsync(string keyword, int? groupId, bool? isLocal, bool isDisabled);

        Task<EmployeeBaseModel> GetEmployeeByIdAsync(int employeeId);

        Task<IEnumerable<EmployeeRoleModel>> GetRolesByEmployeeIdAsync(int employeeId);

        Task<CommandResult> EditEmployeeRolesAsync(EditEmployeeRolesModel model);

        Task<IEnumerable<EmployeeWorkLoadModel>> GetWorkLoadAsync(int employeeId);

        Task<IEnumerable<PARecordModel>> GetPerformanceAppraisalResultAsync(int employeeId);
    }
}