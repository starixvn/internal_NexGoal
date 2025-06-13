namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Criteria;
    using System.Text;
    using System.Linq;

    public class SyncTOEICCriteria : CommandBaseCriteria
    {
        private const string _TOEICTpl = "<TOEIC EmpId='{0}' MinimumScore='{1}' " +
            "Listening='{2}' Reading='{3}' LRExamDate='{4}' InputDate='{5}' ModifiedDate='{6}'></TOEIC>";

        [SpParam(Ignored = true)]
        public IList<TOEIC> TOEICs { get; set; }

        public string TOEICXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");
                if (TOEICs != null)
                {
                    foreach (var toeic in TOEICs)
                    {
                        value.AppendFormat(
                            _TOEICTpl,
                            toeic.EmpId,
                            toeic.MinimumScore,
                            toeic.Listening,
                            toeic.Reading,
                            toeic.LRExamDate?.ToString("yyyy-MM-dd"),
                            toeic.InputDate?.ToString("yyyy-MM-dd"),
                            toeic.ModifiedDate?.ToString("yyyy-MM-dd"));
                    }
                }
                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey() => "SyncTOEIC_Import";

        public override bool IsValid() => TOEICs != null && TOEICs.Any();
    }
} 