namespace NexGoal.ViewModel
{
    using System;

    public class EmployeeWorkingViewModel
    {
        public int EmployeeId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectManager { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string RoleName { get; set; }

        public float WorkingHour { get; set; }

        public float Score { get; set; }

        public string Comment { get; set; }
    }
}
