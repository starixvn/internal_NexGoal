namespace NexGoal.AutoService.Criterias.Nomination
{
    public class EditWhenFinishedCriteria : CommandBaseCriteria
    {
        public int? NominationPeriodId { get; set; }

        public override string GetSettingKey() => "EditWhenNominationFinished";

        public override bool IsValid() => true;
    }
}