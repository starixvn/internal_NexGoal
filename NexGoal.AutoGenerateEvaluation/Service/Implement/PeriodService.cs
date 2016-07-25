using NexGoal.Core.Enumerations;
using NexGoal.Core.Model;
using NexGoal.Core.Services;
using NexGoal.Criteria.EmployeeEPA;
using NexGoal.Criteria.Period;
using NexGoal.Criteria.Period.GenerationEvaluation;
using NexGoal.Model.EmployeeEPA;
using NexGoal.Model.Period;
using NexGoal.Services.Implement;
using NexGoal.Services.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NexGoal.AutoGenerateEvaluation
{
    public class PeriodEvalutionService : IPeriodEvalutionService
    {
        private readonly IEvaluationService evaluationService;
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;
        private readonly IEmployeeEPAService employeeEPAService;

        public PeriodEvalutionService()
            : this(null, null, null, null)
        {
        }

        public PeriodEvalutionService(
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

        public void CheckEndEvaluationPeriod()
        {
            var getAllEndedEvaluationCriteria = new GetEPAEndEvaluationPeriodCriteria();
            IEnumerable<EPAModel> allEvaluations = employeeEPAService.GetEPAEndEvaluationPeriod(getAllEndedEvaluationCriteria);
            if (allEvaluations != null && allEvaluations.Any())
            {
                UpdateFinalScoreAndSendMail(allEvaluations);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void UpdateFinalScoreAndSendMail(IEnumerable<EPAModel> evaluationInfos)
        {
            IList<int> periods = new List<int>();
            bool hasSendMailEndEvaluation = AppSetting.SendMailEndPeriod;

            var criteria = new GetStandardGradeCriteria();
            IEnumerable<StandardGradeModel> standardGrades = employeeEPAService.GetStandardGrade(criteria);

            foreach (EPAModel evaluationInfo in evaluationInfos)
            {
                float averagePerformanceScore = GetFinalScoreOfEvalution(evaluationInfo);

                float finalScore = averagePerformanceScore + evaluationInfo.DisciplineScore;
                finalScore = finalScore >= 0 ? finalScore : 0;

                evaluationInfo.Grade = employeeEPAService.ConvertScoreToGrade(finalScore, standardGrades);

                EncodeAndUpdateFinalScore(averagePerformanceScore, evaluationInfo.DisciplineScore, evaluationInfo.EPAId, evaluationInfo.Grade);

                if (AppSetting.SendMailEndPeriod)
                {
                    SendEndEvaluationMailToSelf(evaluationInfo, averagePerformanceScore, evaluationInfo.DisciplineScore);

                    if (evaluationInfo.Period != null)
                    {
                        if (evaluationInfo.Period.TargetTypeId == (int)TargetType.Probation
                            || evaluationInfo.Period.TargetTypeId == (int)TargetType.Promotion)
                        {
                            autoMailService.SendEndEvaluationMailToHumanResourceByEmployee(evaluationInfo);
                        }
                        else if ((evaluationInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual || evaluationInfo.Period.TargetTypeId == (int)TargetType.Monthly)
                            && !periods.Contains(evaluationInfo.Period.PeriodId))
                        {
                            periods.Add(evaluationInfo.Period.PeriodId);
                        }
                    }
                }
            }

            if (AppSetting.SendMailEndPeriod && periods.Any())
            {
                foreach (int periodId in periods)
                {
                    var checkLastestProcessCriteria = new CheckLastestProcessOfPeriodCriteria
                    {
                        PeriodId = periodId
                    };
                    int lastestProcessOfPeriod = evaluationService.CheckLastestProcessOfPeriod(checkLastestProcessCriteria);
                    if (lastestProcessOfPeriod == 1)
                    {
                        GetAndSendEmailToAllRelatedEmployee(periodId, string.Empty, (int)ProcessType.HumanResource);
                    }
                }
            }
        }

        private void GetAndSendEmailToAllRelatedEmployee(
            int periodId,
            string employeeIds,
            int? processId)
        {
            PeriodModel period = GetPeriodInformation(periodId);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);

            autoMailService.SendEndEvaluationMailToHumanResourceByPeriod(period, employeeSettings);
        }

        private PeriodModel GetPeriodInformation(int periodId)
        {
            var period = new PeriodModel();

            var criteria = new GetPeriodByIdCriteria
            {
                PeriodId = periodId
            };
            period = periodService.GetPeriodById(criteria);

            return period;
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds, int? processId)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            var getEmployeeSettingCriteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds,
                ProcessId = processId
            };
            employeeSettings = periodService.GetEmployeeSettingByPeriodId(getEmployeeSettingCriteria);

            return employeeSettings;
        }

        private float GetFinalScoreOfEvalution(EPAModel evaluationInfo)
        {
            float finalScore = 0;

            if (evaluationInfo.Process.Managements.Any())
            {
                finalScore = evaluationInfo.Process.AverageScoreByManagements;
            }
            else if (evaluationInfo.Process.Reviewers.Any())
            {
                finalScore = evaluationInfo.Process.AverageScoreByReviewers;
            }
            else if (evaluationInfo.Process.Evaluators.Any())
            {
                finalScore = evaluationInfo.Process.AverageScoreByEvaluators;
            }

            return finalScore;
        }

        private void EncodeAndUpdateFinalScore(float performanceScore, float disciplineScore, int evaluationId, string grade)
        {
            float finalScore = performanceScore + disciplineScore;
            string finalScoreEncode = EncodeService.Encode(finalScore.ToString());

            string gradeEncode = EncodeService.Encode(grade);

            var criteria = new UpdateFinalScoreEndEvaluationCriteria();
            criteria.EvaluationScores = criteria.ConvertToXml(evaluationId, finalScoreEncode, disciplineScore, gradeEncode);

            CommandResult commandResult = evaluationService.UpdateFinalScoreEndEvaluation(criteria);
        }

        private void SendEndEvaluationMailToSelf(EPAModel evaluationInfo, float averagePerformanceScore, float disciplineScore)
        {
            autoMailService.SendEndEvaluationMailToSelf(evaluationInfo, averagePerformanceScore, disciplineScore);
        }
    }
}
