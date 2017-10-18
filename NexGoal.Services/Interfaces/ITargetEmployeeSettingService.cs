namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Criteria.TargetEmployeeSetting;
    using NexGoal.Model.TargetEmployeeSetting;

    public interface ITargetEmployeeSettingService
    {
        IList<TargetEmployeeModel> GetTargetEmployeeSetting(GetAllTargetEmployeeSettingCriteria criteria);

        IList<TargetDepartmentModel> GetTargetDepartmentSetting(GetTargetDepartmentSettingCriteria criteria);
    }
}