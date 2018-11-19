namespace NexGoal.AutoService.Criterias
{
    using Fanex.Data.Repository;

    public class GetRefResourceByNameCriteria : CriteriaBase
    {
        public string RefNames { get; set; }
        public bool? IsActive { get; set; }

        public override bool IsValid() => !string.IsNullOrEmpty(RefNames);

        public override string GetSettingKey() => "GetRefResourceByNames";
    }
}