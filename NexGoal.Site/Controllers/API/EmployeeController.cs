namespace NexGoal.Site.Controllers.API
{
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
    using NexGoal.Web;
    using NexGoal.Web.ApiAuthorizeAttribute;
    using NexGoal.Web.Services;
    using System.Collections.Generic;
    using System.Web.Http;

    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IWebCommonService _webCommonService;

        public EmployeeController()
            : this(null, null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService,
            IWebCommonService webCommonService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _webCommonService = webCommonService ?? new WebCommonService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Read)]
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

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionManager.CurrentUserId;
                criteria.Password = Encryption.ComputeHash(HashProvider.Sha1, criteria.Password);
            }

            CommandResult commandResult = _employeeService.CreateSpecialEmployee(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionManager.CurrentUserId;
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
                var user = this._employeeService.GetEmployeeById(SessionManager.CurrentUserId);
                if (Encryption.CompareStringToHash(HashProvider.Sha1, criteria.OldPassword, user.Password))
                {
                    criteria.EmployeeId = SessionManager.CurrentUserId;
                    criteria.ModifiedBy = SessionManager.CurrentUserId;

                    commandResult = _employeeService.ChangePassword(criteria);
                }
                else
                {
                    commandResult.Message = Lang.ValidateWrongPassword;
                }
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionManager.CurrentUserId;
            }

            CommandResult commandResult = _employeeService.AssignRoleToEmployee(criteria);

            return commandResult;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult ResetPassword(int employeeId)
        {
            var criteria = new ResetPasswordCriteria()
            {
                EmployeeId = employeeId,
                NewPassword = Constants.ResetDefaultPassword,
                ModifiedBy = SessionManager.CurrentUserId
            };

            return _employeeService.ResetPassword(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult UpdateHasMonthlyEPA(UpdateHasMonthlyEPACriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionManager.CurrentUserId;
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
            int permission = _webCommonService.GetPermission(EPAFunctionKey.UserManagement);
            return permission;
        }
    }
}