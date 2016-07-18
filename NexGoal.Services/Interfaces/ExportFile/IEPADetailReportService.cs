namespace NexGoal.Services.Interfaces
{
    using NexGoal.Model.Report.EPADetail;
    using System.IO;

    public interface IEPADetailReportService
    {
        MemoryStream Export(EPADetailModel epaDetail);

        void Render(EPADetailModel epaDetail);
    }
}
