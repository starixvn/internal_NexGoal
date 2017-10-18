namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Model.Integration;

    public interface IIntegrationService
    {
        IEnumerable<HRMSummaryViewModel> GetHRMSummary(int employeeId, int periodId);

        IEnumerable<HRMSummaryViewModel> GetHRMSummaryOnProbation(int employeeId);

        IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking(int employeeId, int periodId);

        IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking(int? employeeId, int periodId);
    }
}