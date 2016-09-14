namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.TargetEmployeeSetting;
    using NexGoal.Model.TargetEmployeeSetting;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class TargetEmployeeSettingController : BaseController
    {
        private readonly ITargetEmployeeSettingService _targetEmployeeSettingService;

        public TargetEmployeeSettingController()
            : this(null)
        {
        }

        public TargetEmployeeSettingController(
            ITargetEmployeeSettingService targetEmployeeSettingService)
        {
            _targetEmployeeSettingService = targetEmployeeSettingService ?? new TargetEmployeeSettingService();
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Read)]
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

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpPut]
        public CommandResult AssignTargetTemplateToEmployee(AssignTargetTemplateToEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = _targetEmployeeSettingService.AssignTargetTemplateToEmployee(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpDelete]
        public CommandResult DeleteTargetEmployeeSetting(int targetTemplateId, int employeeId)
        {
            var criteria = new DeleteTargetEmployeeSettingCriteria
            {
                TargetTemplateId = targetTemplateId,
                EmployeeId = employeeId,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = _targetEmployeeSettingService.DeleteTargetEmployeeSetting(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Read)]
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

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpPut]
        public CommandResult AssignTargetTemplateToDepartment(TargetSettingAssignModel model)
        {
            CommandResult commandResult = new CommandResult();

            if (model != null)
            {
                var criteria = new AssignTargetDepartmentSettingCriteria
                {
                    TargetTemplateId = model.TargetTemplateId,
                    Departments = model.ConvertToXml(),
                    ModifiedBy = SessionService.EmployeeId
                };

                commandResult = _targetEmployeeSettingService.AssignTargetTemplateToDepartment(criteria);
            }

            return commandResult;
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpDelete]
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
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = _targetEmployeeSettingService.DeleteTargetDepartmentSetting(criteria);
            return commandResult;
        }

        public int GetUserPermissionOnTargetEmployeeSetting()
        {
            return SessionService.CurrentUser.GetPermission(FunctionKey.EPASetting);
        }
    }
}