namespace NexGoal.Models.Probation
{
    using System.Collections.Generic;

    public class EditEvaluatorEvaluationModel
    {
        public int EmployeeId { get; set; }

        public string Comment { get; set; }

        public bool IsQualifiedByEvaluator { get; set; }

        public int TotalScore { get; set; }

        public int ActorId { get; set; }

        public IEnumerable<EditEvaluationModel> Evaluations { get; set; }
    }
}