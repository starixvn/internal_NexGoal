namespace NexGoal.Models.Probation
{
    using System.Collections.Generic;

    public class EditSelfEvaluationModel
    {
        public int EmployeeId { get; set; }

        public string Comment { get; set; }

        public int TotalScore { get; set; }

        public IEnumerable<EditEvaluationModel> Evaluations { get; set; }
    }
}