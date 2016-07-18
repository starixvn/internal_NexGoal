namespace NexGoal.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Model.TargetEmployeeSetting;

    public interface ITargetEmployeeSettingRepository
    {
        TargetEmployeeSettingModel GetTargetEmployeeSetting(ICriteria criteria);

        TargetDepartmentSettingModel GetTargetDepartmentSetting(ICriteria criteria);
    }
}