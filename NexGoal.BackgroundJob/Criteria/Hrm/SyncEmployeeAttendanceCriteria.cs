namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Criteria;
    using System.Text;
    using System.Linq;

    public class SyncEmployeeAttendanceCriteria : CommandBaseCriteria
    {
        private const string _AttendanceTpl = "<EmployeeAttendance Id='{0}' EmpId='{1}' AttendanceDate='{2}' InFixed='{3}' OutFixed='{4}' " +
            "LeaveTypeId='{5}' TotalWorkHours='{6}' LeaveDay='{7}' LeaveTypeId2='{8}' LateIn='{9}' EarlyOut='{10}' Absence='{11}' InsertDate='{12}'></EmployeeAttendance>";

        [SpParam(Ignored = true)]
        public IList<EmployeeAttendance> Attendances { get; set; }

        public string AttendanceXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");
                if (Attendances != null)
                {
                    foreach (var attendance in Attendances)
                    {
                        value.AppendFormat(
                            _AttendanceTpl,
                            attendance.Id,
                            attendance.EmpId,
                            attendance.AttendanceDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            attendance.InFixed?.ToString("yyyy-MM-dd HH:mm:ss"),
                            attendance.OutFixed?.ToString("yyyy-MM-dd HH:mm:ss"),
                            attendance.LeaveTypeId,
                            attendance.TotalWorkHours,
                            attendance.LeaveDay,
                            attendance.LeaveTypeId2,
                            attendance.LateIn,
                            attendance.EarlyOut,
                            attendance.Absence,
                            attendance.InsertDate);
                    }
                }
                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey() => "SyncEmployeeAttendance_Import";

        public override bool IsValid() => Attendances != null && Attendances.Any();
    }
} 