namespace NexGoal.Criteria.PAFocus
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Constants;
    using NexGoal.Model.PAFocus;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class GeneratePAFocusCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "PAFocus_Generate";
        }
    }
}