namespace NexGoal.BackgroundJob.Criterias.Nomination
{
    using NexGoal.Core.Criteria;

    public class EditWhenFinishedCriteria : CommandBaseCriteria
    {
        public int? NominationPeriodId { get; set; }

        public override string GetSettingKey() => "Auto_Nomination_UpdateWhenFinished";

        public override bool IsValid() => true;
    }
}