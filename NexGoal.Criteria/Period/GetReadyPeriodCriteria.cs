namespace NexGoal.Criteria.Period
{
    using Fanex.Data.Repository;

    public class GetReadyPeriodCriteria : CriteriaBase
    {
        public int? TargetTypeId { get; set; }
        
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_GetReadyPeriod";
        }
    }
}