namespace NexGoal.Model.AttendanceSummary
{
    using NexGoal.Model.Employee;
    using System.Collections.Generic;

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