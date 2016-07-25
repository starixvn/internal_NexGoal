using NexGoal.Core.Enumerations;
using NexGoal.Core.Model;
using NexGoal.Core.Services;
using NexGoal.Criteria.EmployeeEPA;
using NexGoal.Model.EmployeeEPA;
using NexGoal.Model.Period;
using NexGoal.Services.Implement;
using NexGoal.Services.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NexGoal.AutoGenerateEvaluation
{
    public class ProcessService : IProcessService
    {
         private readonly IEvaluationService evaluationService;
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;
        private readonly IEmployeeEPAService employeeEPAService;

        public ProcessService()
            : this(null, null, null, null)
        {
        }

        public ProcessService(
            IEvaluationService evaluationService,
            IPeriodService periodService,
            IAutoMailService autoMailService,
            IEmployeeEPAService employeeEPAService)
        {
            this.evaluationService = evaluationService ?? new EvaluationService();
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public void CheckEndProcess()
        {
            var checkEndProcessCriteria = new CheckEndProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> evaluationProcesses = evaluationService.CheckEndProcess(checkEndProcessCriteria);

            if (evaluationProcesses != null && evaluationProcesses.Any())
            {
                var updateEndProcessCriteria = new UpdateIsEndProcessCriteria()
                {
                    EvaluationProcesses = evaluationProcesses
                };

                CommandResult updateIsEndProcess = evaluationService.UpdateEndProcess(updateEndProcessCriteria);

                IList<AutoEmployeePeriodModel> semiEvaluationProcesses = evaluationProcesses
                    .Where(evaluationProcess => evaluationProcess.ProcessId == (int)ProcessType.Evaluator
                        && evaluationProcess.TargetTypeId == (int)TargetType.SemiAnnual
                        && evaluationProcess.HasMonthlyEPA).ToList();

                foreach (AutoEmployeePeriodModel evaluationProcess in semiEvaluationProcesses)
                {
                    var getEPAInfoCriteria = new GetEPAInfoByIdCriteria
                    {
                        EPAId = evaluationProcess.EPAId
                    };
                    EPAModel epaInfo = employeeEPAService.GetEPAInfoById(getEPAInfoCriteria);

                    var updateTotalScoreOfSemiHasMonthlyCriteria = new UpdateTotalScoreOfSemiHasMonthlyCriteria
                    {
                        EPAId = evaluationProcess.EPAId,
                        MonthlyEPA = epaInfo.MonthlyEPA
                    };
                    CommandResult commandResult = evaluationService.UpdateTotalScoreOfSemiHasMonthly(updateTotalScoreOfSemiHasMonthlyCriteria);
                }

                foreach (AutoEmployeePeriodModel evaluationProcess in evaluationProcesses.Where(evaluationProcess => evaluationProcess.ProcessId >= (int)ProcessType.Reviewer))
                {
                    if (string.IsNullOrEmpty(evaluationProcess.TotalScore))
                    {
                        var getEvaluationInfoCriteria = new GetEPAInfoByIdCriteria
                        {
                            EPAId = evaluationProcess.EPAId
                        };
                        EPAModel evaluationInfo = employeeEPAService.GetEPAInfoById(getEvaluationInfoCriteria);
                        float averageScore = GetAverageScorePreProcess(evaluationInfo, evaluationProcess.ProcessId);

                        var updateTotalScoreCriteria = new UpdateTotalScoreOfProcessCriteria
                        {
                            EPAId = evaluationProcess.EPAId,
                            ProcessId = evaluationProcess.ProcessId,
                            AverageScore = EncodeService.Encode(averageScore.ToString())
                        };
                        CommandResult commandResult = evaluationService.UpdateTotalScoreOfProcess(updateTotalScoreCriteria);
                    }
                }
            }
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
