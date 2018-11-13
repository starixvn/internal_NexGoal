namespace NexGoal.AutoService.Criterias.Nomination
{
    public class EditWhenFinishedCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "EditWhenNominationFinished";

        public override bool IsValid() => true;
    }
}