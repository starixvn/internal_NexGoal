namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Employee;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PARecord;

    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeBaseModel>> GetEmployeesAsync(string keyword = null,
            int? groupId = null,
            bool? isLocal = null,
            bool? isDisabled = null);

        Task<EmployeeBaseModel> GetEmployeeByIdAsync(int employeeId);

        Task<IEnumerable<EmployeeRoleModel>> GetRolesByEmployeeIdAsync(int employeeId);

        Task<CommandResult> EditEmployeeRolesAsync(EditEmployeeRolesModel model);

        Task<IEnumerable<PARecordModel>> GetPerformanceAppraisalResultAsync(int employeeId);

        Task<IEnumerable<PARecordModel>> GetMemberPerformanceAppraisalResultAsync(
            int actorId,
            int? periodId = null,
            int? employeeId = null);

        Task<IEnumerable<EmployeeNominationModel>> GetNominationResultAsync(int employeeId);

        Task<IEnumerable<EmployeeNominationModel>> GetMemberNominationResultAsync(int actorId, int? employeeId = null);

        Task<IEnumerable<EmployeeNominationModel>> GetMemberNominationDetailsAsync(
            int actorId,
            int? nominationPeriodId = null,
            int? employeeId = null);

        Task<IEnumerable<EmployeeBaseModel>> GetMemberAsync(int actorId, string keyword = null);
    }
}