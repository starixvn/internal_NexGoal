namespace NexGoal.Models.Probation
{
    using NexGoal.Core.Extensions;

    [XMLName("Row")]
    public class EditSelfEvaluationModel
    {
        public int EmployeeId { get; set; }

        public int KPIId { get; set; }

        public int SelfPoint { get; set; }
    }
}