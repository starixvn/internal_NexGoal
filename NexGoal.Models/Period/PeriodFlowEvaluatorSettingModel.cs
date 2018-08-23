namespace NexGoal.Models.Period
{
    public class PeriodFlowEvaluatorSettingModel
    {
        public int FlowSettingId { get; set; }

        public int EvaluatorId { get; set; }

        public string Evaluator { get; set; }

        public bool IsEvaluated { get; set; }
    }
}