using Fanex.Data.Repository;
using System;

namespace NexGoal.Criteria
{
    public class GetEmployeeWorkingByEmployeeId : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public override string GetSettingKey()
        {
            return "NexPM_GetEmployeeWorkingByEmployeeId";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
