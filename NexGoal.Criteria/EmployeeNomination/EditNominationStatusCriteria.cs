namespace NexGoal.Criteria.EmployeeNomination
{
    public class EditNominationStatusCriteria : CommandBaseCriteria
    {
        public int NominationId { get; set; }

        public int StatusId { get; set; }

        public int UpdatedById { get; set; }

        public override string GetSettingKey() => "Nomination_UpdateStatus";

        public override bool IsValid() => NominationId > 0 && StatusId > 0 && UpdatedById > 0;
    }
}