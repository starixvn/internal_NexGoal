namespace NexGoal.Models.PARecord
{
    using System.Collections.Generic;
    using System.Linq;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;

    public class PARecordModel
    {
        public int PARecordId { get; set; }

        public int PeriodId { get; set; }
        public PeriodBaseModel Period { get; set; }

        public int EmployeeId { get; set; }
        public EmployeeBaseModel Employee { get; set; }

        public int StatusId { get; set; }
        public string Status { get; set; }

        public string GradeId { get; set; }
        public PerformanceGradeModel Grade { get; set; }

        public string Comment { get; set; }

        public int? Toeic { get; set; }

        public bool IsPassedToeic { get; set; }

        public int? MinimumScore { get; set; }

        public bool IsEdited
        {
            get
            {
                return Grade.GradeId != CalculateAutoGeneratePAGrade();
            }
        }

        public IEnumerable<PAProcessModel> Processes { get; set; }

        private int CalculateAutoGeneratePAGrade()
        {
            var evaluatorProcess = Processes.FirstOrDefault(p => p.ProcessId == (int)PerformanceProcess.Evaluator);
            var grade = evaluatorProcess.PerformanceRecord.Grade.GradeId;
            if (IsPassedToeic)
            {
                grade++;
            }
            else
            {
                grade--;
            }
            return grade;
        }
    }
}