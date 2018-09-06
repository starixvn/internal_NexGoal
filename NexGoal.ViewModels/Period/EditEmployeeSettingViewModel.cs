namespace NexGoal.ViewModels.Period
{
    using System;

    public class EditEmployeeSettingViewModel
    {
        public int PARecordId { get; set; }

        public int ProcessId { get; set; }

        public int? EvaluatorId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}