namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.FlowManagement;
    using NexGoal.Model.Department;
    using NexGoal.Model.Employee;
    using NexGoal.Model.FlowManagement;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    public class FlowManagementController : BaseController
    {
        private readonly IFlowManagementService _flowManagementService;

        public FlowManagementController()
            : this(null)
        {
        }

        public FlowManagementController(
            IFlowManagementService flowManagementService)
        {
            _flowManagementService = flowManagementService ?? new FlowManagementService();
        }

        #region Flow Template

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Read)]
        [HttpGet]
        public IEnumerable<FlowTemplateModel> GetFlowTemplate(
            int? targetTypeId = null,
            bool? isActive = null)
        {
            var criteria = new GetFlowTemplateCriteria()
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            return _flowManagementService.GetFlowTemplate(criteria);
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Read)]
        [HttpGet]
        public FlowTemplateModel GetFlowTemplateById(int flowTemplateId)
        {
            var criteria = new GetFlowTemplateByIdCriteria()
            {
                FlowTemplateId = flowTemplateId
            };

            return _flowManagementService.GetFlowTemplateById(criteria);
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpPost]
        public CommandResult CreateFlowTemplate(CreateFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeId;
            }

            return _flowManagementService.CreateFlowTemplate(criteria);
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateFlowTemplate(UpdateFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeId;
            }

            return _flowManagementService.UpdateFlowTemplate(criteria);
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateStatusFlowTemplate(UpdateStatusFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeId;
            }

            return _flowManagementService.UpdateStatusFlowTemplate(criteria);
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpDelete]
        public CommandResult DeleteFlowTemplate(int flowTemplateId)
        {
            var criteria = new DeleteFlowTemplateCriteria()
            {
                FlowTemplateId = flowTemplateId,
                ModifiedBy = CurrentUser.EmployeeId
            };

            return _flowManagementService.DeleteFlowTemplate(criteria);
        }

        #endregion Flow Template

        #region Department Flow Setting

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Read)]
        [HttpGet]
        public IEnumerable<DepartmentFlowModel> GetDepartmentFlow(int flowTemplateId)
        {
            var criteria = new GetDepartmentFlowCriteria()
            {
                FlowTemplateId = flowTemplateId
            };

            return _flowManagementService.GetDepartmentFlow(criteria);
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Read)]
        [HttpGet]
        public IEnumerable<FlowModel> GetDepartmentFlowById(
            int departmentId,
            int flowTemplateId)
        {
            var criteria = new GetDepartmentFlowByIdCriteria()
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = Math.Abs(departmentId),
                IsGroup = departmentId <= 0
            };

            return _flowManagementService.GetDepartmentFlowById(criteria);
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpPost]
        public CommandResult CreateDepartmentFlow(InsertFlowModel model)
        {
            CommandResult commandResult = new CommandResult();

            if (model != null)
            {
                var criteria = new CreateDepartmentFlowCriteria()
                {
                    DepartmentId = Math.Abs(model.DepartmentId),
                    FlowTemplateId = model.FlowTemplateId,
                    IsGroup = model.DepartmentId <= 0,
                    ModifiedBy = CurrentUser.EmployeeId,
                    FlowDetails = model.FlowDetails
                };

                commandResult = _flowManagementService.CreateDepartmentFlow(criteria);
            }

            return commandResult;
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpDelete]
        public CommandResult DeleteDepartmentFlow(
            int departmentId,
            int flowTemplateId)
        {
            var criteria = new DeleteDepartmentFlowCriteria()
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = Math.Abs(departmentId),
                IsGroup = departmentId <= 0,
                ModifiedBy = CurrentUser.EmployeeId
            };

            return _flowManagementService.DeleteDepartmentFlow(criteria);
        }

        [HttpGet]
        public IList<DepartmentModel> GetDepartmentHasSetting(int flowTemplateId)
        {
            var criteria = new GetDepartmentSectionHasSettingCriteria
            {
                FlowTemplateId = flowTemplateId
            };

            IList<DepartmentModel> departments = _flowManagementService.GetDepartmentHasSetting(criteria);
            return departments;
        }

        #endregion Department Flow Setting

        #region Personal Flow Setting

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Read)]
        [HttpGet]
        public IEnumerable<PersonalFlowModel> GetPersonalFlow(int flowTemplateId)
        {
            var criteria = new GetPersonalFlowCriteria()
            {
                FlowTemplateId = flowTemplateId
            };

            return _flowManagementService.GetPersonalFlow(criteria);
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Read)]
        [HttpGet]
        public IEnumerable<FlowModel> GetPersonalFlowById(
            int employeeId,
            int flowTemplateId)
        {
            var criteria = new GetPersonalFlowByIdCriteria()
            {
                EmployeeId = employeeId,
                FlowTemplateId = flowTemplateId
            };

            return _flowManagementService.GetPersonalFlowById(criteria);
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpPost]
        public CommandResult CreatePersonalFlow(InsertFlowModel model)
        {
            CommandResult commandResult = new CommandResult();

            if (model != null)
            {
                var criteria = new CreatePersonalFlowCriteria()
                {
                    EmployeeId = model.Selfs.Employees[0].EmployeeId,
                    FlowTemplateId = model.FlowTemplateId,
                    FlowDetails = model.FlowDetails,
                    ModifiedBy = CurrentUser.EmployeeId
                };

                commandResult = _flowManagementService.CreatePersonalFlow(criteria);
            }

            return commandResult;
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpDelete]
        public CommandResult DeletePersonalFlow(
            int employeeId,
            int flowTemplateId)
        {
            var criteria = new DeletePersonalFlowCriteria()
            {
                EmployeeId = employeeId,
                FlowTemplateId = flowTemplateId,
                ModifiedBy = CurrentUser.EmployeeId
            };

            return _flowManagementService.DeletePersonalFlow(criteria);
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Read)]
        [HttpGet]
        public EmployeeFlowSettingModel GetEmployeeFlowSetting(
            int flowTemplateId,
            int? departmentId = null,
            int? groupId = null)
        {
            IEnumerable<EmployeeModel> employees = new List<EmployeeModel>();
            var result = new EmployeeFlowSettingModel();

            var criteria = new GetEmployeeFlowSettingCriteria()
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = departmentId,
                GroupId = groupId
            };

            employees = _flowManagementService.GetEmployeeFlowSetting(criteria);

            if (employees != null)
            {
                result.DefaultFlowSettings = employees.Where(x => x.IsExistsSetting == false);
                result.PersonalFlowSettings = employees.Where(x => x.IsExistsSetting == true);
            }

            return result;
        }

        #endregion Personal Flow Setting

        public int GetUserPermissionOnFlowManagement()
        {
            return CurrentUser.GetPermission(FunctionKey.EPASetting);
        }
    }
}