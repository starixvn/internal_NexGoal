namespace NexGoal.Criteria.Redmine
{
    using System;
    using Fanex.Data.Repository;

    public class GetTeamWorkloadCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Redmine_GetTeamWorkLoad";

        public override bool IsValid() => EmployeeId > 0 && ToDate > FromDate;
    }
}