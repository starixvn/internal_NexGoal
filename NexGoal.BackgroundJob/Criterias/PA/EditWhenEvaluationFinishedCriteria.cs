namespace NexGoal.BackgroundJob.Criterias.PA
{
    using NexGoal.Core.Criteria;

    public class EditWhenEvaluationFinishedCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "Auto_PA_UpdateWhenEvaluationFinished";

        public override bool IsValid() => true;
    }
}