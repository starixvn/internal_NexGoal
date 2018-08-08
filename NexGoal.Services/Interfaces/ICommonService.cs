namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NexGoal.Models;
    using NexGoal.Models.UserManagement;

    public interface ICommonService
    {
        Task<IEnumerable<ReferenceResourceModel>> GetReferenceResourcesByNamesAsync(string[] refNames, bool? isActive = true);

        SelectList GetSelectList<T>(IEnumerable<T> items, string dataValueField, string dataTextField);

        Task<IEnumerable<UserModel>> GetEmployeesAsync(
            string keyword = null,
            int? groupId = null,
            bool? isEnabled = true);

        Task<UserModel> GetEmployeeByIdAsync(int employeeId);

        Task<IEnumerable<UserGroupModel>> GetGroupsAsync();
    }
}