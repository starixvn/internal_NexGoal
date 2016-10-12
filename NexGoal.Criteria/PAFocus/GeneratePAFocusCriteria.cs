namespace NexGoal.Criteria.PAFocus
{
    using Fanex.Data.Repository;

    public class GeneratePAFocusCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "PAFocus_Generate";
        }
    }
}