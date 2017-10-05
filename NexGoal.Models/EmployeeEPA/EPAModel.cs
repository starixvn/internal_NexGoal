namespace NexGoal.Model.EmployeeEPA
{
    using System.Collections.Generic;
    using System.Linq;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Model.ScoreRatingType;

    public class EPAModel
    {
        public EPAModel()
        {
            KPIGroups = new List<EPAKPIGroupModel>();
            Process = new EvaluationProcessModel();
        }

        public int EPAId { get; set; }

        public int EPAParentId { get; set; }

        public EPAEmployeeModel Employee { get; set; }

        public EPAPeriodModel Period { get; set; }

        public EPAStatusModel Status { get; set; }

        public ScoreRatingTypeModel ScoreRatingType { get; set; }

        public RatingTypeModel RatingType { get; set; }

        public bool RequiredCommentOnMaxScore { get; set; }

        public string FinalScoreEncode { get; set; }

        public string FinalScore { get; set; }

        public int MaxFinalScore
        {
            get
            {
                return Constants.MaxFinalScore;
            }
        }

        public string GradeEncode { get; set; }

        public string Grade { get; set; }

        public bool HasDisciplineScore { get; set; }

        public float DisciplineScore { get; set; }

        public float SystemDisciplineScore { get; set; }

        public float MaxDisciplineScore { get; set; }

        public bool HasMaxScoreOnKPI { get; set; }

        public bool HasOtherViolation { get; set; }

        public int ToeicPolicyScore { get; set; }

        public EvaluationProcessModel Process { get; set; }

        public IList<EPAKPIGroupModel> KPIGroups { get; set; }

        #region Last score

        public string LastFinalScoreEncode { get; set; }

        public string LastFinalScore { get; set; }

        public string LastGradeEncode { get; set; }

        public string LastGrade { get; set; }

        #endregion Last score

        public int Rowspan
        {
            get
            {
                List<int> numbers = new List<int>
                    {
                        Process.Evaluators.Count,
                        Process.Reviewers.Count
                    };

                return numbers.Max();
            }
        }

        public bool IsEndProcessBySelf { get; set; }

        public bool IsMonthly
        {
            get
            {
                return Period != null && Period.TargetTypeId == (int)TargetType.Monthly;
            }
        }

        public bool IsAllowedToEdit
        {
            get
            {
                bool isAllowedToView = Process.CurrentProcess == null
                    || Process.CurrentProcess.IsEndProcess
                    || Status.StatusId == (int)EPAStatus.Closed
                    || Period.IsEndPeriod;

                return !isAllowedToView;
            }
        }

        public void Decrypt()
        {
            ICertificateService certificateService = new CertificateService();

            LastFinalScore = certificateService.Decrypt(LastFinalScoreEncode);
            LastGrade = certificateService.Decrypt(LastGradeEncode);
            FinalScore = certificateService.Decrypt(FinalScoreEncode);
            Grade = certificateService.Decrypt(GradeEncode);
            Process.Self.Decrypt();
            DecodeForList(Process.Evaluators);
            DecodeForList(Process.Reviewers);
            DecodeForGroup();
            foreach (EPAKPIModel kpi in KPIGroups.SelectMany(x => x.KPIs))
            {
                kpi.EvaluationBySelf.Decrypt();
                foreach (EvaluationModel evaluation in kpi.EvaluationByEvaluators)
                {
                    evaluation.Decrypt();
                }
            }
        }

        private void DecodeForList(IList<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes)
            {
                process.Decrypt();
            }
        }

        private void DecodeForGroup()
        {
            foreach (var group in KPIGroups)
            {
                group.Decrypt();
            }
        }
    }
}