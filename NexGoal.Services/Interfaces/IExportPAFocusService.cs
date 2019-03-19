namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;

    public interface IExportPAFocusService
    {
        Task<byte[]> ExportAsync(PeriodBaseModel period, IEnumerable<PAFocusModel> paFocuses);
    }
}