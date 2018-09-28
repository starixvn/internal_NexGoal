namespace NexGoal.Criteria.Dashboard
{
    using Fanex.Data.Repository;

    public class GetUserWorkLoadCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "User_GetWorkLoad";

        public override bool IsValid() => EmployeeId > 0;
    }
}