namespace NexGoal.Model.Integration
{
    using System;
    using NexGoal.Model.Enitites;

    public class EmployeeWorkingViewModel
    {
        public int EmployeeId { get; set; }

        public string ProjectName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Roles { get; set; }

        public float WorkingHour { get; set; }

        public string EncryptedScore { get; set; }

        public string DecryptedScore { get; set; }

        public string Comment { get; set; }

        public int ProjectManagerId { get; set; }

        public string ProjectManagerName { get; set; }
    }
}
