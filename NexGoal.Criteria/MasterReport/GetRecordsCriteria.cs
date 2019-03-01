namespace NexGoal.Criteria.MasterReport
{
    using System;
    using Fanex.Data.Repository;

    public class GetRecordsCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public string GroupIds { get; set; }

        public string EmployeeIds { get; set; }

        public int ActorId { get; set; }

        public override string GetSettingKey() => "MasterReport_Get";

        public override bool IsValid() => PeriodId > 0 && ActorId > 0;
    }
}