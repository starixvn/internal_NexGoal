namespace NexGoal.Criteria.EmployeeNomination
{
    public class EditEmployeeNominationEoyCriteria : EditEmployeeNominationCriteria
    {
        public string KpiNominations { get; set; }

        public override string GetSettingKey() => "Nomination_Update_Eoy";
    }
}
