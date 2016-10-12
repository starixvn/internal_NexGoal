namespace NexGoal.Criteria.AttendanceSummary
{
    using Fanex.Data.Repository;
    using System;

    public class GetOtherViolationsOfEmployeeCriteria : CriteriaBase
    {
        public string EmployeeIds { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime? WorkDate { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EmployeeIds) && StartDate <= EndDate;
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_GetOtherViolations";
        }
    }
}