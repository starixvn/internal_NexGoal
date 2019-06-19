namespace NexGoal.Models.Probation
{
    using NexGoal.Core.Extensions;

    [XMLName("Row")]
    public class EditEvaluationModel
    {
        public int EmployeeId { get; set; }

        public int KPIId { get; set; }

        public decimal Point { get; set; }
    }
}