namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Model.Integration;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Collections.Generic;

    public class IntegrationController : BaseController
    {
        private readonly IIntegrationService integrationService;

        public IntegrationController()
            : this(null)
        {
        }

        public IntegrationController(IIntegrationService integrationService)
        {
            this.integrationService = integrationService ?? new IntegrationService();
        }

        public IEnumerable<HRMSummaryViewModel> GetHRMSummary(int employeeId, int periodId)
        {
            IEnumerable<HRMSummaryViewModel> result = integrationService.GetHRMSummary(employeeId, periodId);

            return result;
        }

        public IEnumerable<HRMSummaryViewModel> GetHRMSummaryOnProbation(int employeeId)
        {
            IEnumerable<HRMSummaryViewModel> result = integrationService.GetHRMSummaryOnProbation(employeeId);

            return result;
        }

        public IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking(int employeeId, int periodId)
        {
            IEnumerable<EmployeeWorkingViewModel> result = integrationService.GetEmployeeWorking(employeeId, periodId);

            return result;
        }
    }
}