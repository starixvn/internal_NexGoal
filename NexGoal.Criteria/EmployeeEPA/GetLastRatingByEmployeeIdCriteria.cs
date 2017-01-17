namespace NexGoal.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetLastRatingByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int EpaId { get; set; }

        public int TargetTypeId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetLastEvaluation";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
