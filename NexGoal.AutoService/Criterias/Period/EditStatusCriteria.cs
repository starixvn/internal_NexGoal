namespace NexGoal.AutoService.Criterias.Period
{
    public class EditStatusCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "EditPAStatus";

        public override bool IsValid() => true;
    }
}