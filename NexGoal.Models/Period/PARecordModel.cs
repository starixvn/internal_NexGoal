namespace NexGoal.Models.Period
{
    using System.Collections.Generic;
    using NexGoal.Models.UserManagement;

    public class PARecordModel
    {
        public int PARecordId { get; set; }
        public int PeriodId { get; set; }
        public bool IsIncluded { get; set; }

        public int EmployeeId { get; set; }
        public UserModel Employee { get; set; }

        public int StatusId { get; set; }
        public string Status { get; set; }

        public IEnumerable<PAProcessModel> Processes { get; set; }
    }
}