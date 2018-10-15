namespace NexGoal.Services
{
    using System.Collections.Generic;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;

    public interface IExportPAFocusService
    {
        byte[] Export(PeriodBaseModel period, IEnumerable<PAFocusModel> paFocuses);
    }
}