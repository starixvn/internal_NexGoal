namespace NexGoal.Criteria.SyncData
{
    using Fanex.Data.Repository;

    public class GetFlowToSyncCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SyncData_Flow";

        public override bool IsValid() => true;
    }
}