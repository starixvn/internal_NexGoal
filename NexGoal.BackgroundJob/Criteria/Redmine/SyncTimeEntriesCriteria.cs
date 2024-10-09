namespace NexGoal.BackgroundJob.Criteria.Redmine
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models.Redmine;

    public class SyncTimeEntriesCriteria : CriteriaBase
    {
        private const string _TimeEntriesTpl = "<TimeEntries Id='{0}' ProjectId='{1}' UserId='{2}' IssueId='{3}' Hours='{4}' SpentOn='{5}' UpdatedOn='{6}'></TimeEntries>";

        [SpParam(Ignored = true)]
        public IList<TimeEntries> TimeEntries { get; set; }

        public string TimeEntriesXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (TimeEntries != null && TimeEntries.Any())
                {
                    foreach (var timeEntries in TimeEntries)
                    {
                        value.AppendFormat(
                            _TimeEntriesTpl,
                            timeEntries.Id,
                            timeEntries.ProjectId,
                            timeEntries.UserId,
                            timeEntries.IssueId,
                            timeEntries.Hours,
                            timeEntries.SpentOn,
                            timeEntries.UpdatedOn);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey() => "TimeEntries_Sync";

        public override bool IsValid() => TimeEntries != null && TimeEntries.Any();
    }
}