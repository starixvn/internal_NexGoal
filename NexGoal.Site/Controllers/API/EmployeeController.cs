namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Core.Security;
    using NexGoal.Criteria.Employee;
    using NexGoal.Model.Employee;
    using NexGoal.Model.Enitites;
    using NexGoal.Resources;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

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

        /// [ApiAuthorize(EPAFunctionKey.UserManagement, Policy.Read)]
        public IEnumerable<EmployeeModel> GetAllEmployee(
            string query = "",
            int? groupId = null,
            bool? isLocal = null,
            bool? isSpecialUser = null,
            bool? isDisabledUser = null,
            bool? hasMonthlyEPA = null)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = query,
                GroupId = groupId,
                IsLocal = isLocal,
                IsSpecialUser = isSpecialUser,
                IsDisabledUser = isDisabledUser,
                HasMonthlyEPA = hasMonthlyEPA
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

        /// [ApiAuthorize(EPAFunctionKey.UserManagement, Policy.Host)]
        [HttpPost]
        public CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeId;
                criteria.Password = Encryption.ComputeHash(HashProvider.Sha1, criteria.Password);
            }

            CommandResult commandResult = _employeeService.CreateSpecialEmployee(criteria);
            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.UserManagement, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeId;
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

        [HttpPut]
        public CommandResult ChangePassword(ChangePasswordCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            if (criteria != null)
            {
                var user = this._employeeService.GetEmployeeById(CurrentUser.EmployeeId);
                if (Encryption.CompareStringToHash(HashProvider.Sha1, criteria.OldPassword, user.Password))
                {
                    criteria.EmployeeId = CurrentUser.EmployeeId;
                    criteria.ModifiedBy = CurrentUser.EmployeeId;

                    commandResult = _employeeService.ChangePassword(criteria);
                }
                else
                {
                    commandResult.Message = Lang.ValidateWrongPassword;
                }
            }

            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.UserManagement, Policy.Host)]
        [HttpPut]
        public CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeId;
            }

            CommandResult commandResult = _employeeService.AssignRoleToEmployee(criteria);

            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.UserManagement, Policy.Host)]
        [HttpGet]
        public CommandResult ResetPassword(int employeeId)
        {
            var criteria = new ResetPasswordCriteria()
            {
                EmployeeId = employeeId,
                NewPassword = Constants.ResetDefaultPassword,
                ModifiedBy = CurrentUser.EmployeeId
            };

            return _employeeService.ResetPassword(criteria);
        }

        /// [ApiAuthorize(EPAFunctionKey.UserManagement, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateHasMonthlyEPA(UpdateHasMonthlyEPACriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeId;
            }

            CommandResult commandResult = _employeeService.UpdateHasMonthlyEPA(criteria);

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
            var permission = Policy.None;
            var isRead = CurrentUser.HasPermission(EPAFunctionKey.UserManagement, Policy.Read);
            var isHost = CurrentUser.HasPermission(EPAFunctionKey.UserManagement, Policy.Host);
            if (isRead)
            {
                permission = Policy.Read;
            }
            else if (isHost)
            {
                permission = Policy.Host;
            }

            return (int)permission;
        }
    }
}