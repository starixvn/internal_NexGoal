namespace NexGoal.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetDisciplineHRMInfoAndPreviousPAGradeCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public int PARecordId { get; set; }

        public override string GetSettingKey() => "PARecord_GetDisciplineHRMInfoAndPreviousPAGrade";

        public override bool IsValid() => ActorId > 0 && PARecordId > 0;
    }
}