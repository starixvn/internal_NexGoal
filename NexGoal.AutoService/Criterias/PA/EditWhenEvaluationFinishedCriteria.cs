namespace NexGoal.AutoService.Criterias.PA
{
    public class EditWhenEvaluationFinishedCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "EditWhenEvaluationFinished";

        public override bool IsValid() => true;
    }
}