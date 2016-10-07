namespace NexGoal.Criteria.Period.GenerationEvaluation
{
    using Fanex.Data.Repository;

    public class GetEmployeeSettingOfHRCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_GetEmployeeSettingToSendMailHR";
        }
    }
}