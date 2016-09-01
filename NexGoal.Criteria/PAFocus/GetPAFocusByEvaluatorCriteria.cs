namespace NexGoal.Criteria.PAFocus
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Constants;
    using NexGoal.Model.PAFocus;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class GetPAFocusByEvaluatorCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public string PeriodIds { get; set; }

        public string GroupIds { get; set; }

        public int? EmployeeId { get; set; }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }

        public override string GetSettingKey()
        {
            return "PAFocus_GetEvaluator";
        }
    }
}