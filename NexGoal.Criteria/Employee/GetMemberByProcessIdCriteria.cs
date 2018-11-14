namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetMemberByProcessIdCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public int ProcessId { get; set; }

        public override string GetSettingKey() => "Employee_GetMemberByProcessId";

        public override bool IsValid() => ActorId > 0 && ProcessId > 0;
    }
}