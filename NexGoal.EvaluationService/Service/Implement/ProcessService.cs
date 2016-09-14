using System.Collections.Generic;
using System.Linq;
using NexGoal.Core.Enumerations;
using NexGoal.Core.Model;
using NexGoal.Core.Services;
using NexGoal.Model.EmployeeEPA;
using NexGoal.Model.Period;
using NexGoal.Services.Implement;
using NexGoal.Services.Interfaces;

namespace NexGoal.BackgroundService
{
    public class ProcessService : BaseService, IProcessService
    {
        private readonly IAutoMailService autoMailService;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly ICertificateService certificateService;

        public ProcessService()
            : this(null, null, null)
        {
        }

        public ProcessService(
            IAutoMailService autoMailService,
            IEmployeeEPAService employeeEPAService,
            ICertificateService certificateService)
        {
            this.autoMailService = autoMailService ?? new AutoMailService();
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

        private void CaculatorScore(IEnumerable<AutoEmployeePeriodModel> process)
        {
            foreach (AutoEmployeePeriodModel evaluationProcess in process)
            {
                if (string.IsNullOrEmpty(evaluationProcess.TotalScore))
                {
                    EPAModel evaluationInfo = employeeEPAService.GetEPAInfoById(evaluationProcess.EPAId);
                    float averageScore = GetAverageScorePreProcess(evaluationInfo, evaluationProcess.ProcessId);

                    UpdateTotalScoreOfProcess(evaluationProcess, averageScore);
                }
            }
        }

        private CommandResult UpdateTotalScoreOfProcess(AutoEmployeePeriodModel process, float score)
        {
            var commandResult = new CommandResult();
            var criteria = new UpdateTotalScoreOfProcessCriteria
            {
                EPAId = process.EPAId,
                ProcessId = process.ProcessId,
                AverageScore = certificateService.Encrypt(score.ToString())
            };
            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        private CommandResult CloseProcess(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            var comandResult = new CommandResult();
            var criteria = new UpdateIsEndProcessCriteria()
            {
                EvaluationProcesses = evaluationProcesses
            };

            if (criteria != null && criteria.IsValid())
            {
                comandResult = Repository.Update(criteria);
            }

            return comandResult;
        }

        private IEnumerable<AutoEmployeePeriodModel> GetProcessNeedToClose()
        {
            var criteria = new CheckEndProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> evaluationProcesses = new List<AutoEmployeePeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                evaluationProcesses = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return evaluationProcesses;
        }

        private float GetAverageScorePreProcess(EPAModel evaluationInfo, int processId)
        {
            float averageScore = 0;

            switch (processId)
            {
                case (int)ProcessType.Reviewer:
                    averageScore = evaluationInfo.Process.AverageScoreByEvaluators;
                    break;

                case (int)ProcessType.Management:
                    averageScore = evaluationInfo.Process.Reviewers.Any() ? evaluationInfo.Process.AverageScoreByReviewers : evaluationInfo.Process.AverageScoreByEvaluators;
                    break;
            }

            return averageScore;
        }
    }
}