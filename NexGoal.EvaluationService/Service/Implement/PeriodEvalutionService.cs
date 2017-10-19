namespace NexGoal.BackgroundService
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Period;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;

    public class PeriodEvalutionService : BaseService, IPeriodEvalutionService
    {
        private readonly IPAFocusService focusService;
        private readonly IAutoMailService autoMailService;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly ICertificateService certificateService;

        public PeriodEvalutionService()
            : this(null, null, null, null)
        {
        }

        public PeriodEvalutionService(
            IAutoMailService autoMailService,
            IEmployeeEPAService employeeEPAService,
            ICertificateService certificateService,
            IPAFocusService focusService)
        {
            this.autoMailService = autoMailService ?? new AutoMailService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.certificateService = certificateService ?? new CertificateService();
            this.focusService = focusService ?? new PAFocusService();
        }

        public void UpdatePeriodIsEnd()
        {
            var criteria = new GetEPAEndEvaluationPeriodCriteria();
            IEnumerable<EPAModel> epaInfos = employeeEPAService.GetEPAEndEvaluationPeriod(criteria);
            foreach (EPAModel epaInfo in epaInfos)
            {
                UpdateFinalScore(epaInfo);
                SendMail(epaInfo);
            }
        }

        public void GeneratePAFocus()
        {
            var criteria = new GetPeriodToGeneratePAFocusCriteria();
            IEnumerable<PeriodModel> periods = GetData<PeriodModel>(criteria);
            foreach (PeriodModel period in periods)
            {
                focusService.GeneratePAFocus(period.PeriodId);
            }
        }

        private static double GetFinalScoreOfEvalution(EPAModel epaInfo)
        {
            double finalScore = epaInfo.Process.Reviewers.Any()
                    ? epaInfo.Process.AverageScoreByReviewers
                    : epaInfo.Process.AverageScoreByEvaluators + epaInfo.ToeicPolicyScore;

            return finalScore;
        }

        private void SendMail(EPAModel epa)
        {
            if (AppSetting.SendMailEndPeriod)
            {
                autoMailService.SendEndEvaluationMailToSelf(epa);
                if (epa.Period != null && epa.Period.TargetTypeId == (int)TargetType.Probation)
                {
                    autoMailService.SendEndEvaluationMailToHRByEmployee(epa);
                }
            }
        }

        private void UpdateFinalScore(EPAModel epaInfo)
        {
            IEnumerable<StandardGradeModel> standardGrades = employeeEPAService.GetStandardGrade();

            double averagePerformanceScore = GetFinalScoreOfEvalution(epaInfo);
            double finalScore = averagePerformanceScore + epaInfo.DisciplineScore;
            finalScore = finalScore >= 0 ? finalScore : 0;

            epaInfo.Grade = employeeEPAService.ConvertScoreToGrade(finalScore, standardGrades);

            EncodeAndUpdateFinalScore(averagePerformanceScore, epaInfo.DisciplineScore, epaInfo.EPAId, epaInfo.Grade, epaInfo.ToeicPolicyScore);
        }

        private void EncodeAndUpdateFinalScore(
            double performanceScore,
            double disciplineScore,
            int evaluationId,
            string grade,
            int policyToeicScore)
        {
            double finalScore = performanceScore + disciplineScore;
            string finalScoreEncode = certificateService.Encrypt(finalScore.ToString(CultureInfo.InvariantCulture));
            string gradeEncode = certificateService.Encrypt(grade);

            var criteria = new UpdateFinalScoreEndEvaluationCriteria
            {
                EvaluationId = evaluationId,
                FinalScore = finalScoreEncode,
                DisciplineScore = disciplineScore,
                Grade = gradeEncode,
                ToeicPolicyScore = policyToeicScore
            };

            UpdateFinalScore(criteria);
        }

        private void UpdateFinalScore(UpdateFinalScoreEndEvaluationCriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                Repository.ExecuteNonQuery(criteria);
            }
        }
    }
}