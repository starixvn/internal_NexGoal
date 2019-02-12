namespace NexGoal.Services
{
    using System.Threading.Tasks;

    public interface IMasterReportService
    {
        Task<byte[]> ExportAsync(int periodId, string groupIds, string employeeIds);
    }
}