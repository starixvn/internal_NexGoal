namespace NexGoal.Models.FlowSetting
{
    public class FlowSettingModel
    {
        public int ProcessId { get; set; }
        public int Priority { get; set; }

        public int EvaluatorId { get; set; }
        public string Code { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
    }
}