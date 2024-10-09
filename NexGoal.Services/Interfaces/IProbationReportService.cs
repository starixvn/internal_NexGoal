namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Common;
    using NexGoal.Models.Probation;

    public interface IProbationReportService
    {
        Task<(IEnumerable<Group>, IEnumerable<Employee>)> GetGroupsAndEmployeesAsync(int actorId);

        Task<IEnumerable<ProbationReportModel>> GetAsync(GetProbationReportModel model);

        Task<byte[]> ExportExcelAsync(GetProbationReportModel model);

        Task<PdfModel> ExportPdfAsync(int employeeId, int actorId);

        Task<(ProbationReportModel, IEnumerable<KPIGroupModel>, HRMModel)> GetByEmployeeIdAsync(int employeeId, int actorId);
    }
}