namespace NexGoal.BackgroundJob.Models.PAFocus
{
    using NexGoal.Core.Enumerations;

    public class ProcessSettingModel
    {
        public int PeriodId { get; set; }
        public MailMode NotificationMode { get; set; }
        public int EvaluatorId { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string ConversationId { get; set; }
    }
}