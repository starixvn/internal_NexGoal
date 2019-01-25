namespace NexGoal.Criteria.PARecord
{
    using Fanex.Data.Repository;

    public class GetDisciplineByRecordIdCriteria : CriteriaBase
    {
        public int PARecordId { get; set; }

        public override string GetSettingKey() => "PARecord_GetDisciplineById";

        public override bool IsValid() => PARecordId > 0;
    }
}