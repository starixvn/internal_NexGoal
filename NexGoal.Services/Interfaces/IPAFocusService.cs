namespace NexGoal.Services.Interfaces
{
    using NexGoal.Model;
    using System.Collections.Generic;

    public interface IPAFocusService
    {
        IEnumerable<PAFocusProcessSetting> GetPAFocusProcessSettingByPeriodId(int periodId);
    }
}