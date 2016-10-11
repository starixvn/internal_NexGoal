namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class GetHRMSummaryOnProbationCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "HRMSummary_GetOnProbation";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
