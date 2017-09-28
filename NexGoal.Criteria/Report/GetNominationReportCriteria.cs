namespace NexGoal.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetNominationReportCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public string Query { get; set; }

        public string GroupIds { get; set; }

        public int? StatusId { get; set; }

        public override string GetSettingKey()
        {
            return "Report_Nomination";
        }

        public override bool IsValid()
        {
            return EOQYId > 0;
        }
    }
}