namespace NexGoal.BackgroundJob.Criterias.Redmine
{
    using Fanex.Data.Repository;

    public class GetCustomValueByMinIdCriteria : CriteriaBase
    {
        public int MinCustomValueId { get; set; }

        public override string GetSettingKey() => "Sync_CustomValues_GetByMinId";

        public override bool IsValid() => true;
    }
}