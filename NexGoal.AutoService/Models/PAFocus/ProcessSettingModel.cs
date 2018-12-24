namespace NexGoal.AutoService.Models.PAFocus
{
    using NexGoal.Core.Enumerations;

    public class ProcessSettingModel
    {
        public int PeriodId { get; set; }
        public MailMode MailMode { get; set; }
        public int ProcessId { get; set; }
        public int EvaluatorId { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
    }
}