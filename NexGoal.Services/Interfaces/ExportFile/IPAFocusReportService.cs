using NexGoal.Model.PAFocus;
using System.Collections.Generic;
using System.IO;

namespace NexGoal.Services.Interfaces
{
    public interface IPAFocusReportService
    {
        MemoryStream ExportPAFocus(IEnumerable<PAFocusModel> focuses, int periodId);
    }
}
