namespace NexGoal.Criteria.Common
{
    using Fanex.Data.Repository;

    public class SearchEmployeeCriteria : CriteriaBase
    {
        public string Keyword { get; set; }
        public int? EmployeeId { get; set; }
        public int? GroupId { get; set; }
        public bool? IsEnabled { get; set; }

        public override string GetSettingKey() => "Employee_Search";

        public override bool IsValid() => true;
    }
}