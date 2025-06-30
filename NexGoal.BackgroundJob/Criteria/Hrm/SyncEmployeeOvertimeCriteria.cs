namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Criteria;
    using System.Text;
    using System.Linq;

    public class SyncEmployeeOvertimeCriteria : CommandBaseCriteria
    {
        private const string _OvertimeTpl = "<EmployeeOvertime OvertimeId='{0}' EmployeeId='{1}' " +
            "Date='{2}' Hour='{3}' ModifiedDate='{4}'></EmployeeOvertime>";

        [SpParam(Ignored = true)]
        public IList<EmployeeOvertime> Overtimes { get; set; }

        public string OvertimeXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");
                if (Overtimes != null)
                {
                    foreach (var overtime in Overtimes)
                    {
                        value.AppendFormat(
                            _OvertimeTpl,
                            overtime.OvertimeId,
                            overtime.EmployeeId,
                            overtime.Date.ToString("yyyy-MM-dd"),
                            overtime.Hour.ToString("0.0"),
                            overtime.ModifiedDate);
                    }
                }
                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey() => "SyncEmployeeOvertime_Import";

        public override bool IsValid() => Overtimes != null && Overtimes.Any();
    }
} 