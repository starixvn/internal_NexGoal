namespace NexGoal.Services.Interfaces
{
    using NexGoal.Model.Report.EPADetail;
    using System.Collections.Generic;
    using System.IO;

    public interface IEPADetailReportService
    {
        MemoryStream Export(IEnumerable<EPADetailModel> epaDetail);

        void Render(IEnumerable<EPADetailModel> epaDetail);
    }
}
