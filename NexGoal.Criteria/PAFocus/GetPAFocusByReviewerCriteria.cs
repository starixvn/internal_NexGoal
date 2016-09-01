namespace NexGoal.Criteria.PAFocus
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Constants;
    using NexGoal.Model.PAFocus;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class GetPAFocusByReviewerCriteria : CriteriaBase
    {
        public int ReviewerId { get; set; }

        public string PeriodIds { get; set; }

        public string GroupIds { get; set; }

        public int? EmployeeId { get; set; }

        public override bool IsValid()
        {
            return ReviewerId > 0;
        }

        public override string GetSettingKey()
        {
            return "PAFocus_GetReviewer";
        }
    }
}