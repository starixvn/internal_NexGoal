namespace NexGoal.Criteria.EmployeeNomination
{
    public class CreateEmployeeNominationEoyCriteria : CreateEmployeeNominationCriteria
    {
        public string KpiNominations { get; set; }

        public override string GetSettingKey() => "Nomination_Insert_Eoy";
    }
}
