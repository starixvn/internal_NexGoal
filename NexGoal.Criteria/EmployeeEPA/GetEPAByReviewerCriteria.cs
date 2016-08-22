namespace NexGoal.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAByReviewerCriteria : CriteriaBase
    {
        public int ReviewerId { get; set; }

        public string EmployeeIds { get; set; }

        public string GroupIds { get; set; }

        public string PeriodIds { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetByReviewer";
        }

        public override bool IsValid()
        {
            return ReviewerId > 0;
        }
    }
}