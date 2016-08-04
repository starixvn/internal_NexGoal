namespace NexGoal.Services.Implement
{
    using NexGoal.Services.Interfaces;
    using NexGoal.ViewModel;
    using System;
    using System.Collections.Generic;

    public class IntergrationService : IIntegrationService
    {
        public IEnumerable<HRMSummaryViewModel> GetHRMSummary(int employeeId)
        {
            return new List<HRMSummaryViewModel>();
        }

        public IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking(int employeeId)
        {
            return new List<EmployeeWorkingViewModel>();
        }
    }
}
