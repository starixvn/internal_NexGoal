using Fanex.Data.Repository;
using System;

namespace NexGoal.Criteria
{
    public class GetEmployeeWorkingByPeriodIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int? EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "NexPM_GetEmployeeWorkingByPeriodId";
        }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }
    }
}
