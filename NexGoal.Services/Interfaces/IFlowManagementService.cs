namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Criteria.FlowManagement;
    using NexGoal.Model.Department;
    using NexGoal.Model.Employee;
    using NexGoal.Model.FlowManagement;

    public interface IFlowManagementService
    {
        #region Flow Template

        IEnumerable<FlowTemplateModel> GetFlowTemplate(GetFlowTemplateCriteria criteria);

        FlowTemplateModel GetFlowTemplateById(GetFlowTemplateByIdCriteria criteria);

        #endregion Flow Template

        #region Department Flow Setting

        IEnumerable<DepartmentFlowModel> GetDepartmentFlow(GetDepartmentFlowCriteria criteria);

        IEnumerable<FlowModel> GetDepartmentFlowById(GetDepartmentFlowByIdCriteria criteria);

        IList<DepartmentModel> GetDepartmentHasSetting(GetDepartmentSectionHasSettingCriteria criteria);

        #endregion Department Flow Setting

        #region Employee Flow Setting

        IEnumerable<PersonalFlowModel> GetPersonalFlow(GetPersonalFlowCriteria criteria);

        IEnumerable<FlowModel> GetPersonalFlowById(GetPersonalFlowByIdCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeFlowSetting(GetEmployeeFlowSettingCriteria criteria);

        #endregion Employee Flow Setting
    }
}