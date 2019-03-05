namespace NexGoal.AutoService.Criterias.PA
{
    using NexGoal.Core.Criteria;

    public class EditStatusCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "Auto_PA_UpdateStatus";

        public override bool IsValid() => true;
    }
}