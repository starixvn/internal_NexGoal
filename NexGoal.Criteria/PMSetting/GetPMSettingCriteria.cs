namespace NexGoal.Criteria.PMSetting
{
    using Fanex.Data.Repository;

    public class GetPMSettingCriteria : CriteriaBase
    {
        public string PMIds { get; set; }

        public int? EmployeeId { get; set; }

        public override string GetSettingKey() => "PMSetting_Get";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(PMIds);
    }
}