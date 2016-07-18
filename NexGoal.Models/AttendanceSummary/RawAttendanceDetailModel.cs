namespace NexGoal.Model.AttendanceSummary
{
    using System.Collections.Generic;
    using NexGoal.Model.Employee;

    public class RawAttendanceDetailModel
    {
        public RawAttendanceDetailModel()
        {
            Employees = new List<EmployeeModel>();
            Violations = new List<AttendanceDetailModel>();
        }

        public IEnumerable<EmployeeModel> Employees { get; set; }

        public IEnumerable<AttendanceDetailModel> Violations { get; set; }

        public IEnumerable<AttendanceDetailModel> CommentViolations { get; set; }
    }
}