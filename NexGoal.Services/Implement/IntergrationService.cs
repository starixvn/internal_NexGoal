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
            throw new NotImplementedException();
        }

        internal IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking(int employeeId)
        {
            throw new NotImplementedException();
        }
    }
}
