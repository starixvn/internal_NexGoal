namespace NexGoal.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAToUpdateFinalScoreCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public override string GetSettingKey() => "EPA_GetEPAToUpdateFinalScore";

        public override bool IsValid() => PeriodId > 0;
    }
}