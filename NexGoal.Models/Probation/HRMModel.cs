namespace NexGoal.Models.Probation
{
    using System.Collections.Generic;

    public class HRMModel
    {
        public ToeicModel Toeic { get; set; }

        public IEnumerable<AttendanceModel> Attendances { get; set; }
    }
}