namespace NexGoal.Criteria.Redmine
{
    using Fanex.Data.Repository;

    public class GetTeamWorkloadCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int PeriodId { get; set; }

        public override string GetSettingKey() => "Redmine_GetTeamWorkLoad";

        public override bool IsValid() => EmployeeId > 0 && PeriodId > 0;
    }
}