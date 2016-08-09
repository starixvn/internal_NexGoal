using NexGoal.Core.Services;

namespace NexGoal.Model.EmployeeEPA.RatingByKPI
{
    public class RatingByKPIModel
    {
        public int EPAId { get; set; }

        public EPAEmployeeModel Employee { get; set; }

        public string TotalScoreEncode { get; set; }

        public string TotalScore { get; set; }

        public string TotalScoreOriginal
        {
            get
            {
                return TotalScore;
            }
        }

        public RatingTypeModel RatingType { get; set; }

        public bool RequiredCommentOnMaxScore { get; set; }

        public TemporaryKPIGroupModel KPIGroup { get; set; }

        public void Decode()
        {
            TotalScore = EncodeService.Decode(TotalScoreEncode);
        }
    }
}