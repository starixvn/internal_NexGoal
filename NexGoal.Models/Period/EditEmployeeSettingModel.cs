namespace NexGoal.Models.Period
{
    using NexGoal.Core.Extensions;

    [XMLName("Row")]
    public class EditEmployeeSettingModel
    {
        public int PARecordId { get; set; }

        public int ProcessId { get; set; }

        public int EvaluatorId { get; set; }
    }
}