namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using Fanex.Data;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Criteria;

    public class SyncJobTitleCriteria : CommandBaseCriteria
    {
        private const string _JobTitleTpl = "<JobTitle Id='{0}' Name='{1}' ModifiedDate='{2}' SyncDate='{3}'></JobTitle>";

        [SpParam(Ignored = true)]
        public IList<JobTitle> JobTitles { get; set; }

        public string JobTitleXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");
                if (JobTitles != null)
                {
                    foreach (var jobTitle in JobTitles)
                    {
                        value.AppendFormat(
                            _JobTitleTpl,
                            jobTitle.Id,
                            jobTitle.Name,
                            jobTitle.ModifiedDate);
                    }
                }
                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey() => "SyncJobTitle_Import";

        public override bool IsValid() => JobTitles != null && JobTitles.Any();
    }
} 