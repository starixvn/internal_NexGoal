namespace NexGoal.Models.Probation
{
    using System.Collections.Generic;

    public class ProbationerModel
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public IEnumerable<ProbationSettingModel> Settings { get; set; }
    }
}