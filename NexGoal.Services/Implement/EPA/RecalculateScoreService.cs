namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Model;
    using Core.Services;
    using Criteria.EmployeeEPA;
    using Interfaces;
    using Model.EmployeeEPA;

    public class RecalculateScoreService : BaseService, IRecalculateScoreService
    {
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly ICertificateService certificateService;

        public RecalculateScoreService()
            : this(null, null)
        {
        }

        public RecalculateScoreService(IEmployeeEPAService employeeEPAService, ICertificateService certificateService)
        {
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.certificateService = certificateService ?? new CertificateService();
        }

        public CommandResult RecalculateScore(int userId, IEnumerable<int> epaIds)
        {
            IEnumerable<StandardGradeModel> standardGrades = employeeEPAService.GetStandardGrade();
            IList<EPAModel> epaInfos = new List<EPAModel>();

            foreach (int epaId in epaIds)
            {
                EPAModel epa = employeeEPAService.GetEPAInfoById(epaId);

                RecalculateScoreForReviewer(epa);

                double finalScore = GetFinalScoreOfEvalution(epa);

                finalScore = finalScore >= 0 ? finalScore : 0;
                epa.FinalScore = certificateService.Encrypt(finalScore.ToString());
                string grade = employeeEPAService.ConvertScoreToGrade(finalScore, standardGrades);
                epa.Grade = certificateService.Encrypt(grade);

                epaInfos.Add(epa);
            }

            CommandResult commandResult = UpdateReviewerScore(userId, epaInfos);

            if (commandResult.IsSuccessful)
            {
                commandResult = UpdateFinalScore(userId, epaInfos);
            }

            return commandResult;
        }

        private void RecalculateScoreForReviewer(EPAModel epaInfo)
        {
            foreach (EPAProcessModel process in epaInfo.Process.Reviewers)
            {
                double adjustScore;
                if (double.TryParse(process.AdjustScore, out adjustScore))
                {
                    process.TotalScore = (epaInfo.Process.AverageScoreByEvaluators + adjustScore + epaInfo.ToeicPolicyScore).ToString();
                }
                else
                {
                    process.TotalScore = (epaInfo.Process.AverageScoreByEvaluators + epaInfo.ToeicPolicyScore).ToString();
                }
            }
        }

        private double GetFinalScoreOfEvalution(EPAModel epaInfo)
        {
            double finalScore = 0;

            if (epaInfo.Process.Reviewers.Any())
            {
                finalScore = epaInfo.Process.AverageScoreByReviewers;
            }
            else
            {
                finalScore = epaInfo.Process.AverageScoreByEvaluators + epaInfo.ToeicPolicyScore;
            }

            return finalScore;
        }

        private CommandResult UpdateReviewerScore(int userId, IEnumerable<EPAModel> epaInfos)
        {
            var commandResult = new CommandResult();

            var criteria = new RecalculateReviewerScoreCriteria
            {
                ModifiedBy = userId,
                EPAInfos = epaInfos
            };

            if (criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        private CommandResult UpdateFinalScore(int userId, IEnumerable<EPAModel> epaInfos)
        {
            var commandResult = new CommandResult();

            var criteria = new RecalculateFinalScoreCriteria
            {
                ModifiedBy = userId,
                EPAInfos = epaInfos
            };

            if (criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }
    }
}