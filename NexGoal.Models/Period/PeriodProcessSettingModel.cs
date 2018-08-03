namespace NexGoal.Models.Period
{
    public class PeriodProcessSettingModel
    {
        public int ProcessId { get; set; }

        public int PeriodId { get; set; }

        public string ProcessName { get; set; }

        public int Duration { get; set; }

        public bool NoticeAtBegin { get; set; }

        public int RemindDay { get; set; }

        public int? FocusDay { get; set; }
    }
}