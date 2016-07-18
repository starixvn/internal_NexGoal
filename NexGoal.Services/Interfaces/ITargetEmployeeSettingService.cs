namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.TargetEmployeeSetting;
    using NexGoal.Model.TargetEmployeeSetting;

    public interface ITargetEmployeeSettingService
    {
        IList<TargetEmployeeModel> GetTargetEmployeeSetting(GetAllTargetEmployeeSettingCriteria criteria);

        CommandResult AssignTargetTemplateToEmployee(AssignTargetTemplateToEmployeeCriteria criteria);

        CommandResult DeleteTargetEmployeeSetting(DeleteTargetEmployeeSettingCriteria criteria);

        IList<TargetDepartmentModel> GetTargetDepartmentSetting(GetTargetDepartmentSettingCriteria criteria);

        CommandResult AssignTargetTemplateToDepartment(AssignTargetDepartmentSettingCriteria criteria);

        CommandResult DeleteTargetDepartmentSetting(DeleteTargetDepartmentSettingCriteria criteria);
    }
}