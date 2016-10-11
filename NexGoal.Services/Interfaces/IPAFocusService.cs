namespace NexGoal.Services.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.PAFocus;
    using NexGoal.Model;
    using NexGoal.Model.PAFocus;
    using System.Collections.Generic;

    public interface IPAFocusService
    {
        IEnumerable<int> GetProcessId(int periodId, int evaluatorId);

        IEnumerable<FocusProcessSettingModel> GetPAFocusProcessSettingByPeriodId(int periodId);

        CommandResult UpdateProcessSetting(UpdatePAFocusProcessSetingCriteria criteria);

        CommandResult UpdatePAFocus(int modifiedBy, IEnumerable<PAFocusModel> evaluations);

        IEnumerable<PAFocusModel> GetPAFocus(ICriteria criteria);

        IEnumerable<PAFocusProcessModel> GetPAFocusProcess(int periodId);

        CommandResult GeneratePAFocus(int periodId);
    }
}