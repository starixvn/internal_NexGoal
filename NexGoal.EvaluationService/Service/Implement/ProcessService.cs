namespace NexGoal.BackgroundService
{
    using System.Collections.Generic;
    using System.Linq;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Period;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;

    public class ProcessService : BaseService, IProcessService
    {
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly ICertificateService certificateService;

        public ProcessService()
            : this(null, null)
        {
        }

        public ProcessService(
            IEmployeeEPAService employeeEPAService,
            ICertificateService certificateService)
        {
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.certificateService = certificateService ?? new CertificateService();
        }

        public void UpdateInformationAtEndProccess()
        {
            IEnumerable<AutoEmployeePeriodModel> proccess = GetProcessNeedToClose();
            CloseProcess(proccess);
            var processNeedToCaculateScore = proccess.Where(evaluationProcess => evaluationProcess.ProcessId >= (int)ProcessType.Reviewer);
            CaculatorScore(processNeedToCaculateScore);
        }

        private void CaculatorScore(IEnumerable<AutoEmployeePeriodModel> processes)
        {
            foreach (AutoEmployeePeriodModel process in processes)
            {
                if (string.IsNullOrEmpty(process.TotalScore))
                {
                    EPAModel epaInfo = employeeEPAService.GetEPAInfoById(process.EPAId);
                    double averageScore = epaInfo.Process.AverageScoreByEvaluators + epaInfo.ToeicPolicyScore;

                    UpdateTotalScoreOfProcess(process, averageScore);
                }
            }
        }

        private void UpdateTotalScoreOfProcess(AutoEmployeePeriodModel process, double score)
        {
            var criteria = new UpdateTotalScoreOfProcessCriteria
            {
                EPAId = process.EPAId,
                ProcessId = process.ProcessId,
                AverageScore = certificateService.Encrypt(score.ToString())
            };

            if (criteria.IsValid())
            {
                Repository.ExecuteNonQuery(criteria);
            }
        }

        private void CloseProcess(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            var criteria = new UpdateIsEndProcessCriteria()
            {
                EvaluationProcesses = evaluationProcesses
            };

            if (criteria.IsValid())
            {
                Repository.ExecuteNonQuery(criteria);
            }
        }

        private IEnumerable<AutoEmployeePeriodModel> GetProcessNeedToClose()
        {
            var criteria = new CheckEndProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> evaluationProcesses = new List<AutoEmployeePeriodModel>();

            if (criteria.IsValid())
            {
                evaluationProcesses = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return evaluationProcesses;
        }
    }
}