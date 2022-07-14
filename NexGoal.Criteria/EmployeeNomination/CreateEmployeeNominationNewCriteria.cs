namespace NexGoal.Criteria.EmployeeNomination
{
    public class CreateEmployeeNominationNewCriteria : CreateEmployeeNominationCriteria
    {
        public string KpiNominations { get; set; }

        public override string GetSettingKey() => "Nomination_Insert_New";
    }
}
