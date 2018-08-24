namespace NexGoal.Models.Period
{
    using System.Collections.Generic;

    public class PARecordModel
    {
        public int PARecordId { get; set; }

        public int PeriodId { get; set; }

        public bool IsIncluded { get; set; }

        public string Group { get; set; }

        public int EmployeeId { get; set; }

        public int Code { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string JobTitle { get; set; }

        public int StatusId { get; set; }

        public string Status { get; set; }

        public IEnumerable<PAProcessModel> Processes { get; set; }
    }
}