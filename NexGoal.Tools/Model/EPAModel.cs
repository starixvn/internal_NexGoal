namespace NexGoal.Tools
{
    using System.Collections.Generic;

    public class EPAModel
    {
        public EPAModel()
        {
            EPAProcesses = new List<EPAProcessModel>();
        }

        public int EPAId { get; set; }

        public string FinalScore { get; set; }

        public string Grade { get; set; }

        public bool HasDisciplineScore { get; set; }

        public float DisciplineScore { get; set; }

        public int StatusId { get; set; }

        public IList<EPAProcessModel> EPAProcesses { get; set; }
    }
}