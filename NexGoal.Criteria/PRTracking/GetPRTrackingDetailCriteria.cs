namespace NexGoal.Criteria.PRTracking
{
    using Fanex.Data.Repository;

    public class GetPRTrackingDetailCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int ProcessId { get; set; }

        public override string GetSettingKey() => "PRTracking_GetDetail";

        public override bool IsValid() => ProcessId > 0 && EmployeeId > 0;
    }
}