namespace NexGoal.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Employee;
    using NexGoal.Model.Employee;
    using NexGoal.Model.Enitites;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;

    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController()
            : this(null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService)
        {
            _employeeService = employeeService ?? new EmployeeService();
        }

        public IEnumerable<EmployeeModel> GetAllEmployee(
            string query = "",
            int? groupId = null,
            bool? isLocal = null,
            bool? isSpecialUser = null,
            bool? isDisabledUser = null)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = query,
                GroupId = groupId,
                IsLocal = isLocal,
                IsSpecialUser = isSpecialUser,
                IsDisabledUser = isDisabledUser
            };

            IEnumerable<EmployeeModel> employees = _employeeService.GetAllEmployee(criteria);

            return employees;
        }

        public IEnumerable<Group> GetAllGroup()
        {
            var criteria = new GetAllGroupCriteria();
            IEnumerable<Group> groups = _employeeService.GetAllGroup(criteria);

            return groups;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        [HttpPut]
        public CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = _employeeService.UpdateSpecialEmployee(criteria);
            return commandResult;
        }

        public IEnumerable<JobTitle> GetAllJobTitle()
        {
            var criteria = new GetAllJobTitleCriteria();
            IEnumerable<JobTitle> jobTitles = _employeeService.GetAllJobTitle(criteria);
            return jobTitles;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        [HttpPut]
        public CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = _employeeService.AssignRoleToEmployee(criteria);

            return commandResult;
        }

        [HttpGet]
        public string GetDefaultPassword()
        {
            string defaultPassword = Constants.ResetDefaultPassword;

            return defaultPassword;
        }

        public int GetUserPermissionOnEmployee()
        {
            return SessionService.CurrentUser.GetPermission(FunctionKey.UserManagement);
        }
    }
}