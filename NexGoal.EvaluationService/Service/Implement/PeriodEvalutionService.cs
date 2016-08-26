using System.Collections.Generic;
using System.Linq;
using NexGoal.Core.Enumerations;
using NexGoal.Core.Model;
using NexGoal.Core.Services;
using NexGoal.Criteria.EmployeeEPA;
using NexGoal.Criteria.Period.GenerationEvaluation;
using NexGoal.Model.EmployeeEPA;
using NexGoal.Model.Period;
using NexGoal.Services.Implement;
using NexGoal.Services.Interfaces;

namespace NexGoal.AutoGenerateEvaluation
{
    public class PeriodEvalutionService : BaseService, IPeriodEvalutionService
    {
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly ICertificateService certificateService;

        public PeriodEvalutionService()
            : this(null, null, null, null)
        {
        }

        public PeriodEvalutionService(
            IPeriodService periodService,
            IAutoMailService autoMailService,
            IEmployeeEPAService employeeEPAService,
            ICertificateService certificateService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.certificateService = certificateService ?? new CertificateService();
        }

        public void UpdatePeriodIsEnd()
        {
            var getAllEndedEvaluationCriteria = new GetEPAEndEvaluationPeriodCriteria();
            IEnumerable<EPAModel> epas = employeeEPAService.GetEPAEndEvaluationPeriod(getAllEndedEvaluationCriteria);
            foreach (EPAModel epa in epas)
            {
                UpdateFinalScore(epa);
                SendMail(epa);
            }
        }

        private void SendMail(EPAModel epa)
        {
            int periods = 0;
            if (AppSetting.SendMailEndPeriod)
            {
                float averagePerformanceScore = GetFinalScoreOfEvalution(epa);
                autoMailService.SendEndEvaluationMailToSelf(epa, averagePerformanceScore, epa.DisciplineScore);
                if (epa.Period != null)
                {
                    if (epa.Period.TargetTypeId == (int)TargetType.Probation
                        || epa.Period.TargetTypeId == (int)TargetType.Promotion)
                    {
                        autoMailService.SendEndEvaluationMailToHumanResourceByEmployee(epa);
                    }
                    else if (epa.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                    {
                        periods = epa.Period.PeriodId;
                    }
                }

                int lastestProcessOfPeriod = CheckLastestProcessOfPeriod(periods);
                if (lastestProcessOfPeriod == (int)ProcessType.HumanResource)
                {
                    SendEmailToAllRelatedEmployee(periods, lastestProcessOfPeriod);
                }
            }
        }

        private void UpdateFinalScore(EPAModel epa)
        {
            var periods = new List<int>();

            IEnumerable<StandardGradeModel> standardGrades = employeeEPAService.GetStandardGrade();

            float averagePerformanceScore = GetFinalScoreOfEvalution(epa);

            float finalScore = averagePerformanceScore + epa.DisciplineScore;
            finalScore = finalScore >= 0 ? finalScore : 0;

            epa.Grade = employeeEPAService.ConvertScoreToGrade(finalScore, standardGrades);

            EncodeAndUpdateFinalScore(averagePerformanceScore, epa.DisciplineScore, epa.EPAId, epa.Grade);
        }

        private int CheckLastestProcessOfPeriod(int periodId)
        {
            int lastestProcess = 0;
            var criteria = new CheckLastestProcessOfPeriodCriteria
            {
                PeriodId = periodId
            };

            if (criteria != null && criteria.IsValid())
            {
                lastestProcess = GetDataEntity<int>(criteria);
            }

            return lastestProcess;
        }

        private void SendEmailToAllRelatedEmployee(
            int periodId,
            int? processId)
        {
            PeriodModel period = periodService.GetPeriodById(periodId);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, processId);

            autoMailService.SendEndEvaluationMailToHumanResourceByPeriod(period, employeeSettings);
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, int? processId)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                ProcessId = processId
            };
            employeeSettings = periodService.GetEmployeeSettingByPeriodId(criteria);

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
            string finalScoreEncode = certificateService.Encrypt(finalScore.ToString()),
                gradeEncode = certificateService.Encrypt(grade);

            var criteria = new UpdateFinalScoreEndEvaluationCriteria
            {
                EvalutionId = evaluationId,
                FinalScore = finalScoreEncode,
                DisciplineScore = disciplineScore,
                Grade = gradeEncode
            };

            UpdateFinalScore(criteria);
        }

        private CommandResult UpdateFinalScore(UpdateFinalScoreEndEvaluationCriteria criteria)
        {
            var comandResult = new CommandResult();
            if (criteria != null && criteria.IsValid())
            {
                comandResult = Repository.Update(criteria);
            }

            return comandResult;
        }
    }
}