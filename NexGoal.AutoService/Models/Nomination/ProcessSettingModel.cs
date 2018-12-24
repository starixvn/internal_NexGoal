namespace NexGoal.AutoService.Models.Nomination
{
    using NexGoal.Core.Enumerations;

    public class ProcessSettingModel
    {
        public int NominationPeriodId { get; set; }
        public MailMode MailMode { get; set; }
        public int NominatorId { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
    }
}