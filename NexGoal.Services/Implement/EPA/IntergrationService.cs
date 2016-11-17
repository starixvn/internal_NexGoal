namespace NexGoal.Services.Implement
{
    using Core.Constants;
    using Core.Enumerations;
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
        private ISessionService sessionService;

        public IntegrationService()
            : this(null, null, null)
        {
        }

        public IntegrationService(IPeriodService periodService, IIntegrationRepository integrationRepository, ISessionService sessionService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.integrationRepository = integrationRepository ?? new IntegrationRepository();
            this.sessionService = sessionService ?? new SessionService();
        }

        public IEnumerable<HRMSummaryViewModel> GetHRMSummary(int employeeId, int periodId)
        {
            IEnumerable<HRMSummaryViewModel> result = new List<HRMSummaryViewModel>();
            PeriodModel period = periodService.GetPeriodById(periodId);
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

        public IEnumerable<HRMSummaryViewModel> GetHRMSummaryOnProbation(int employeeId)
        {
            IEnumerable<HRMSummaryViewModel> result = new List<HRMSummaryViewModel>();

            var criteria = new GetHRMSummaryOnProbationCriteria
            {
                EmployeeId = employeeId
            };
            result = GetData<HRMSummaryViewModel>(criteria);

            return result;
        }

        public IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking(int employeeId, int periodId)
        {
            IEnumerable<EmployeeWorkingViewModel> result = new List<EmployeeWorkingViewModel>();
            var policyToView = CheckPermissionToViewNexPM(employeeId);
            if (policyToView != Policy.None)
            {
                PeriodModel period = periodService.GetPeriodById(periodId);
                if (period != null)
                {
                    var criteria = new GetEmployeeWorkingByEmployeeIdCriteria
                    {
                        EmployeeId = employeeId,
                        StartDate = period.AttendanceStartDate.Value,
                        EndDate = period.AttendanceEndDate.Value
                    };
                    result = this.integrationRepository.GetEmployeeWorking(criteria);
                    Decrypt(result);
                }
            }            

            return result;
        }

        public IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking(int? employeeId, int periodId)
        {
            var criteria = new GetEmployeeWorkingByPeriodIdCriteria
            {
                EmployeeId = employeeId,
                PeriodId = periodId
            };
            var result = this.integrationRepository.GetEmployeeWorking(criteria);
            Decrypt(result);

            return result;
        }

        private void Decrypt(IEnumerable<EmployeeWorkingViewModel> nexPMs)
        {
            foreach (var item in nexPMs)
            {
                item.DecryptedScore = DecodeNexPM(item.EncryptedScore);
                item.DecryptedScore = PercentToStarNumber(item.DecryptedScore);
            }
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

        private Policy CheckPermissionToViewNexPM(int employeeId)
        {
            if (employeeId != sessionService.EmployeeId)
            {
                var policy = sessionService.CurrentUser.GetPermission(FunctionKey.Evaluator) + sessionService.CurrentUser.GetPermission(FunctionKey.Reviewer);

                return policy > (int)Policy.None ? Policy.Read : Policy.None;
            }

            return Policy.Read;
        }
    }
}
