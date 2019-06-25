namespace NexGoal.Models.Probation
{
    public class EditReviewerEvaluationModel
    {
        public int EmployeeId { get; set; }

        public string Comment { get; set; }

        public bool IsQualified { get; set; }

        public int ActorId { get; set; }
    }
}