namespace NexGoal.Services.Interfaces
{
    using NexGoal.ViewModel;
    using System.Collections.Generic;

    public interface IIntegrationService
    {
        IEnumerable<HRMSummaryViewModel> GetHRMSummary(int employeeId);

        IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking(int employeeId);
    }
}
