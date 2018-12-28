namespace NexGoal.AutoService.Models.PA
{
    using System;
    using NexGoal.Core.Enumerations;

    public class ProcessSettingModel
    {
        public int PeriodId { get; set; }
        public MailMode MailMode { get; set; }
        public int EvaluatorId { get; set; }
        public int ProcessId { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}