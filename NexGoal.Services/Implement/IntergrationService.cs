namespace NexGoal.Services.Implement
{
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Model.Period;
    using NexGoal.Services.Interfaces;
    using NexGoal.ViewModel;
    using System;
    using System.Collections.Generic;

    public class IntegrationService : BaseService, IIntegrationService
    {
        private IPeriodService periodService;

        public IntegrationService()
            : this(null)
        { 
        }

        public IntegrationService(IPeriodService periodService)
        {
            this.periodService = periodService ?? new PeriodService();
        }

        public IEnumerable<HRMSummaryViewModel> GetHRMSummary(int employeeId)
        {
            IEnumerable<HRMSummaryViewModel> result = new List<HRMSummaryViewModel>();
            PeriodModel period = periodService.GetInprogressSemiPeriodByEmployeeId(employeeId);
            if (period != null)
            {
                var criteria = new GetHRMSummaryByEmployeeId
                {
                    EmployeeId = employeeId,
                    StartDate = period.AttendanceStartDate.Value,
                    EndDate = period.AttendanceEndDate.Value
                };
                result = GetData<HRMSummaryViewModel>(criteria);
            }

            return result;
        }

        public IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking(int employeeId)
        {
            IEnumerable<EmployeeWorkingViewModel> result = new List<EmployeeWorkingViewModel>();
            PeriodModel period = periodService.GetInprogressSemiPeriodByEmployeeId(employeeId);
            if (period != null)
            {
                var criteria = new GetEmployeeWorkingByEmployeeId
                {
                    EmployeeId = employeeId,
                    StartDate = period.AttendanceStartDate.Value,
                    EndDate = period.AttendanceEndDate.Value
                };
                result = GetData<EmployeeWorkingViewModel>(criteria);
            }

            return result;
        }
    }
}
