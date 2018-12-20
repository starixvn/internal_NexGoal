namespace NexGoal.Criteria.NominationReport
{
    using Fanex.Data.Repository;

    public class GetNominationPeriodsCriteria : CriteriaBase
    {
        public bool IsQuarter { get; set; }

        public override string GetSettingKey() => "NominationReport_GetPeriods";

        public override bool IsValid() => true;
    }
}