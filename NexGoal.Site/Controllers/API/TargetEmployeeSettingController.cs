namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.TargetEmployeeSetting;
    using NexGoal.Model.TargetEmployeeSetting;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web;
    using NexGoal.Web.ApiAuthorizeAttribute;
    using NexGoal.Web.Services;
    using System.Collections.Generic;
    using System.Web.Http;

    public class TargetEmployeeSettingController : BaseController
    {
        private readonly ITargetEmployeeSettingService _targetEmployeeSettingService;
        private readonly IWebCommonService _webCommonService;

        public TargetEmployeeSettingController()
            : this(null, null)
        {
        }

        public TargetEmployeeSettingController(
            ITargetEmployeeSettingService targetEmployeeSettingService,
            IWebCommonService webCommonService)
        {
            _targetEmployeeSettingService = targetEmployeeSettingService ?? new TargetEmployeeSettingService();
            _webCommonService = webCommonService ?? new WebCommonService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Read)]
        public IList<TargetEmployeeModel> GetTargetEmployeeSetting(
            int? groupId = null,
            int? employeeId = null,
            int? targetTypeId = null,
            bool? showAll = null)
        {
            var criteria = new GetAllTargetEmployeeSettingCriteria
            {
                GroupId = groupId,
                EmployeeId = employeeId,
                TargetTypeId = targetTypeId,
                ShowAll = showAll
            };

            IList<TargetEmployeeModel> targetEmployeeSettings = _targetEmployeeSettingService.GetTargetEmployeeSetting(criteria);
            return targetEmployeeSettings;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Host)]
        public CommandResult AssignTargetTemplateToEmployee(AssignTargetTemplateToEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionManager.CurrentUserId;
            }

            CommandResult commandResult = _targetEmployeeSettingService.AssignTargetTemplateToEmployee(criteria);
            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Host)]
        public CommandResult DeleteTargetEmployeeSetting(int targetTemplateId, int employeeId)
        {
            var criteria = new DeleteTargetEmployeeSettingCriteria
            {
                TargetTemplateId = targetTemplateId,
                EmployeeId = employeeId,
                ModifiedBy = SessionManager.CurrentUserId
            };

            CommandResult commandResult = _targetEmployeeSettingService.DeleteTargetEmployeeSetting(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Read)]
        public IList<TargetDepartmentModel> GetTargetDepartmentSetting(
            int? departmentId = null,
            bool? isGroup = null,
            int? targetTypeId = null,
            bool? showAll = null)
        {
            var criteria = new GetTargetDepartmentSettingCriteria
            {
                DepartmentId = departmentId,
                IsGroup = isGroup,
                TargetTypeId = targetTypeId,
                ShowAll = showAll
            };
            IList<TargetDepartmentModel> targetDepartments = _targetEmployeeSettingService.GetTargetDepartmentSetting(criteria);
            return targetDepartments;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Host)]
        public CommandResult AssignTargetTemplateToDepartment(TargetSettingAssignModel model)
        {
            CommandResult commandResult = new CommandResult();

            if (model != null)
            {
                var criteria = new AssignTargetDepartmentSettingCriteria
                {
                    TargetTemplateId = model.TargetTemplateId,
                    Departments = model.ConvertToXml(),
                    ModifiedBy = SessionManager.CurrentUserId
                };

                commandResult = _targetEmployeeSettingService.AssignTargetTemplateToDepartment(criteria);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Host)]
        public CommandResult DeleteTargetDepartmentSetting(
            int targetTemplateId,
            int departmentId,
            bool isGroup)
        {
            var criteria = new DeleteTargetDepartmentSettingCriteria
            {
                TargetTemplateId = targetTemplateId,
                DepartmentId = departmentId,
                IsGroup = isGroup,
                ModifiedBy = SessionManager.CurrentUserId
            };

            CommandResult commandResult = _targetEmployeeSettingService.DeleteTargetDepartmentSetting(criteria);
            return commandResult;
        }

        public int GetUserPermissionOnTargetEmployeeSetting()
        {
            int permission = _webCommonService.GetPermission(EPAFunctionKey.TargetEmployeeSetting);
            return permission;
        }
    }
}