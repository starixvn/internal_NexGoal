namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Criteria;
    using System.Text;
    using System.Linq;

    public class SyncGroupCriteria : CommandBaseCriteria
    {
        private const string _GroupTpl = "<Group GroupId='{0}' Code='{1}' Name='{2}' DepartmentId='{3}' Rank='{4}' IsDeleted='{5}' ParentId='{6}' ModifiedDate='{7}'></Group>";

        [SpParam(Ignored = true)]
        public IList<Group> Groups { get; set; }

        public string GroupXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");
                if (Groups != null)
                {
                    foreach (var group in Groups)
                    {
                        value.AppendFormat(
                            _GroupTpl,
                            group.GroupId,
                            group.Code,
                            group.Name,
                            group.DepartmentId,
                            group.Rank,
                            group.IsDeleted,
                            group.ParentId,
                            group.ModifiedDate);
                    }
                }
                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey() => "SyncGroup_Import";

        public override bool IsValid() => Groups != null && Groups.Any();
    }
}