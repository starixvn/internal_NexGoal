namespace NexGoal.Criteria.EmployeeNomination
{
    public class EditEmployeeNominationNewCriteria : EditEmployeeNominationCriteria
    {
        public string KpiNominations { get; set; }

        public override string GetSettingKey() => "Nomination_Update_New";
    }
}
