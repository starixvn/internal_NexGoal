namespace NexGoal.AutoService.Criterias.PerformanceRecord
{
    public class EditStatusCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "EditPerformanceRecordStatus";

        public override bool IsValid() => true;
    }
}