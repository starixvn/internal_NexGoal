namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using NexGoal.ViewModel;
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

        public IEnumerable<HRMSummaryViewModel> GetHRMSummary()
        {
            IEnumerable<HRMSummaryViewModel> result = integrationService.GetHRMSummary(SessionService.EmployeeId);
            result = DummyHRMSummary();

            return result;
        }

        public IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking()
        {
            IEnumerable<EmployeeWorkingViewModel> result = integrationService.GetEmployeeWorking(SessionService.EmployeeId);
            result = DummyEmployeeWorking();

            return result;
        }

        private List<HRMSummaryViewModel> DummyHRMSummary()
        {
            var result = new List<HRMSummaryViewModel>();

            var item1 = new HRMSummaryViewModel
            {
                EmployeeId = SessionService.EmployeeId,
                Month = 11,
                Year = 2015,
                NumberMissingCheckInOut = 3,
                NumberLateIn = 2,
                NumberEarlyOut = 2,
                NumberUnpaidLeave = 2,
                AverageWorkingHours = 8.35f,
                NumberOvertime = 1,
                TOEICScore = 780
            };
            var item2 = new HRMSummaryViewModel
            {
                EmployeeId = SessionService.EmployeeId,
                Month = 12,
                Year = 2015,
                NumberMissingCheckInOut = 4,
                NumberLateIn = 5,
                NumberEarlyOut = 5,
                NumberUnpaidLeave = 1,
                NumberUnplannedAnnualLeave = 7,
                AverageWorkingHours = 8.35f,
                NumberOvertime = 1,
                TOEICScore = 780
            };

            result.Add(item1);
            result.Add(item2);

            return result;
        }

        private IEnumerable<EmployeeWorkingViewModel> DummyEmployeeWorking()
        {
            var result = new List<EmployeeWorkingViewModel>();

            var item1 = new EmployeeWorkingViewModel
            {
                EmployeeId = SessionService.EmployeeId,
                ProjectName = "Nap Accounting Report",
                ProjectManager = "Ronny",
                StartDate = new DateTime(2015, 11, 1),
                EndDate = new DateTime(2015, 4, 30),
                Roles = "Business Analysis",
                WorkingHour = 141.25f,
                EncryptedScore = "7DFmRMOnPkY=",
                DecryptedScore = "4.3",
                Comment = "ployee the system shows all step by step of the evalution. There are 5 steps: Admin, Self, Evaluator, Reviewer, Final Score and together"
            };

            var item2 = new EmployeeWorkingViewModel
            {
                EmployeeId = SessionService.EmployeeId,
                ProjectName = "Keno Integration",
                ProjectManager = "Richard",
                StartDate = new DateTime(2015, 11, 1),
                EndDate = new DateTime(2015, 4, 30),
                Roles = "Tester",
                WorkingHour = 141.25f,
                EncryptedScore = "C3p+IKnKg/I=",
                DecryptedScore = "4.3",
                Comment = "ployee the system shows all step by step of the evalution. There are 5 steps: Admin, Self, Evaluator, Reviewer, Final Score and together "
            };

            result.Add(item1);
            result.Add(item2);

            return result;
        }
    }
}