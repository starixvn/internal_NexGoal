namespace NexGoal.Services.Interfaces
{
    using NexGoal.Model.Integration;
    using System.Collections.Generic;

    public interface IIntegrationService
    {
        IEnumerable<HRMSummaryViewModel> GetHRMSummary(int employeeId, int periodId);

        IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking(int employeeId, int periodId);
    }
}
