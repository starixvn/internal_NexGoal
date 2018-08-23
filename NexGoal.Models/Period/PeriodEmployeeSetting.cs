namespace NexGoal.Models.Period
{
    using System.Collections.Generic;

    public class PeriodEmployeeSettingModel
    {
        public int SettingId { get; set; }

        public int PeriodId { get; set; }

        public bool IsIncluded { get; set; }

        public string Group { get; set; }

        public int EmployeeId { get; set; }

        public int Code { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string JobTitle { get; set; }

        public IEnumerable<PeriodFlowSettingModel> FlowSettings { get; set; }
    }
}