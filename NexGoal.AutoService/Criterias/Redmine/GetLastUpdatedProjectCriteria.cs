namespace NexGoal.AutoService.Criterias.Redmine
{
    using Fanex.Data.Repository;

    public class GetLastUpdatedProjectCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Projects_GetLastUpdated";

        public override bool IsValid() => true;
    }
}