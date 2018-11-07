namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class GetNominationByRecordIdCriteria : CriteriaBase
    {
        public int RecordId { get; set; }

        public override string GetSettingKey() => "Nomination_GetByRecordId";

        public override bool IsValid() => RecordId > 0;
    }
}