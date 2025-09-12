namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Criteria;

    public class SyncWorkingTimeRequestCriteria : CommandBaseCriteria
    {
        private const string _WorkingTimeRequestTpl = "<WorkingTimeRequest Id='{0}' EmpId='{1}' RequestType='{2}' " +
            "Status='{3}' FromDate='{4}' ToDate='{5}' TotalDays='{6}' ModifiedDate='{7}' />";

        [SpParam(Ignored = true)]
        public IList<WorkingTimeRequest> WorkingTimeRequests { get; set; }

        public string WorkingTimeRequestXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");
                if (WorkingTimeRequests != null)
                {
                    foreach (var request in WorkingTimeRequests)
                    {
                        value.AppendFormat(
                            _WorkingTimeRequestTpl,
                            request.Id,
                            request.EmpId,
                            request.RequestType,
                            request.Status,
                            request.FromDate,
                            request.ToDate,
                            request.TotalDays,
                            request.ModifiedDate);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey() => "SyncWorkingTimeRequest_Import";

        public override bool IsValid() => WorkingTimeRequests != null && WorkingTimeRequests.Any();
    }
}