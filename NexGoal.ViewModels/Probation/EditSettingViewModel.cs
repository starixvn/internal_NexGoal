namespace NexGoal.ViewModels.Probation
{
    using System;

    public class EditSettingViewModel
    {
        public int EmployeeId { get; set; }

        public byte ProcessId { get; set; }

        public int EvaluatorId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}