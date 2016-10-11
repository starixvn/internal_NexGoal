namespace NexGoal.Criteria.PAFocus
{
    using Fanex.Data.Repository;

    public class GetPAFocusProcessIdCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public int PeriodId { get; set; }        

        public override bool IsValid()
        {
            return EvaluatorId > 0 && PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "PAFocus_GetProcessId";
        }
    }
}