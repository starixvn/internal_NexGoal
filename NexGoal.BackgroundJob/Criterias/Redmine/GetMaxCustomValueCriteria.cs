namespace NexGoal.BackgroundJob.Criterias.Redmine
{
    using Fanex.Data.Repository;

    public class GetMaxCustomValueCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "CustomValues_GetMaxId";

        public override bool IsValid() => true;
    }
}