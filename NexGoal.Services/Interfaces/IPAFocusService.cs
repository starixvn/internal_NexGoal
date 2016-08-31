namespace NexGoal.Services.Interfaces
{
    using NexGoal.Core.Model;
    using NexGoal.Model;
    using System.Collections.Generic;

    public interface IPAFocusService
    {
        IEnumerable<PAFocusProcessSetting> GetPAFocusProcessSettingByPeriodId(int periodId);

        CommandResult UpdateProcessSetting(int periodId, IEnumerable<PAFocusProcessSetting> process);
    }
}