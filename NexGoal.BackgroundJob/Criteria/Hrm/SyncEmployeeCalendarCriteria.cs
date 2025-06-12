namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Criteria;
    using System.Text;
    using System.Linq;

    public class SyncEmployeeCalendarCriteria : CommandBaseCriteria
    {
        private const string _CalendarTpl = "<EmployeeCalendar Id='{0}' EmpId='{1}' WorkDate='{2}' " +
            "IsOffDay='{3}' IsHoliday='{4}' IsLeaveDay='{5}' LeaveDay='{6}' ModifiedDate='{7}'></EmployeeCalendar>";

        [SpParam(Ignored = true)]
        public IList<EmployeeCalendar> Calendars { get; set; }

        public string CalendarXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");
                if (Calendars != null)
                {
                    foreach (var calendar in Calendars)
                    {
                        value.AppendFormat(
                            _CalendarTpl,
                            calendar.Id,
                            calendar.EmpId,
                            calendar.WorkDate,
                            calendar.IsOffDay,
                            calendar.IsHoliday,
                            calendar.IsLeaveDay,
                            calendar.LeaveDay,
                            calendar.ModifiedDate);
                    }
                }
                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey() => "SyncEmployeeCalendar_Import";

        public override bool IsValid() => Calendars != null && Calendars.Any();
    }
} 