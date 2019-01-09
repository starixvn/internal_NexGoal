namespace NexGoal.Criteria.PMSetting
{
    using Fanex.Data.Repository;

    public class GetProjectManagersCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "PMSetting_GetProjectManagers";

        public override bool IsValid() => true;
    }
}