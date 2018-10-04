namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeesCriteria : CriteriaBase
    {
        public string Keyword { get; set; }
        public int? GroupId { get; set; }
        public bool? IsLocal { get; set; }
        public bool? IsDisabled { get; set; }
        public override string GetSettingKey() => "Employee_GetAll";

        public override bool IsValid() => true;
    }
}