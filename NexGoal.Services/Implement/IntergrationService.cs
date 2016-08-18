namespace NexGoal.Services.Implement
{
    using IBCCrypt;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Model.Integration;
    using NexGoal.Model.Period;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Collections.Generic;

    public class IntegrationService : BaseService, IIntegrationService
    {
        private const string KeyCryptography = "NexEPA08062014";
        private IPeriodService periodService;
        private IIntegrationRepository integrationRepository;

        public IntegrationService()
            : this(null, null)
        {
        }

        public IntegrationService(IPeriodService periodService, IIntegrationRepository integrationRepository)
        {
            this.periodService = periodService ?? new PeriodService();
            this.integrationRepository = integrationRepository ?? new IntegrationRepository();
        }

        public IEnumerable<HRMSummaryViewModel> GetHRMSummary(int employeeId)
        {
            IEnumerable<HRMSummaryViewModel> result = new List<HRMSummaryViewModel>();
            PeriodModel period = periodService.GetInprogressSemiPeriodByEmployeeId(employeeId);
            if (period != null)
            {
                var criteria = new GetHRMSummaryByEmployeeIdCriteria
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
                var criteria = new GetEmployeeWorkingByEmployeeIdCriteria
                {
                    EmployeeId = employeeId,
                    StartDate = period.AttendanceStartDate.Value,
                    EndDate = period.AttendanceEndDate.Value
                };
                result = this.integrationRepository.GetEmployeeWorking(criteria);
                foreach (var item in result)
                {
                    item.DecryptedScore = DecodeNexPM(item.EncryptedScore);
                    item.DecryptedScore = PercentToStarNumber(item.DecryptedScore);
                }
            }

            return result;
        }

        private string DecodeNexPM(string cipherText)
        {
            return string.IsNullOrWhiteSpace(cipherText) ? string.Empty : IBCCrypt.Decrypt(cipherText, KeyCryptography);
        }

        private string PercentToStarNumber(string percent)
        {
            decimal starNumber = Math.Round(ConvertDecimal(percent) / (2 * 10), 1);
            string result = string.Empty;

            if (starNumber > 0)
            {
                result = starNumber.ToString();
            }

            return result;
        }

        private decimal ConvertDecimal(object value)
        {
            decimal result = 0;

            if (value != null)
            {
                bool isSuccessful = decimal.TryParse(value.ToString(), out result);

                if (!isSuccessful)
                {
                    result = 0;
                }
            }

            return result;
        }
    }
}
