namespace NexGoal.Criteria.Period.GenerationEvaluation
{
    using Fanex.Data.Repository;

    public class GetPeriodHasEndedProcessCriteria : CriteriaBase
    {
        public int? PeriodId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_GetPeriodHasEndedProcess";
        }
    }
}