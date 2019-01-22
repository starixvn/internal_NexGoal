namespace NexGoal.Criteria.PARecord
{
    using Fanex.Data.Repository;

    public class GetLastPAGradeCriteria : CriteriaBase
    {
        public int CurrentPeriodId { get; set; }

        public int EmployeeId { get; set; }

        public int ActorId { get; set; }

        public override string GetSettingKey() => "PARecord_GetPreviousPAGrade";

        public override bool IsValid() => CurrentPeriodId > 0 && ActorId > 0 && EmployeeId > 0;
    }
}