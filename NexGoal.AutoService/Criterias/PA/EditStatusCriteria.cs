namespace NexGoal.AutoService.Criterias.PA
{
    public class EditStatusCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "EditPAStatus";

        public override bool IsValid() => true;
    }
}