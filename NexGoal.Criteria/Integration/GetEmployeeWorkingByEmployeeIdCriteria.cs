using Fanex.Data.Repository;
using System;

namespace NexGoal.Criteria
{
    public class GetEmployeeWorkingByEmployeeIdCriteria : CriteriaBase
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
            return EmployeeId > 0 
                && StartDate != DateTime.MaxValue 
                && StartDate != DateTime.MinValue 
                && EndDate != DateTime.MinValue 
                && EndDate != DateTime.MaxValue 
                && EndDate >= StartDate;
        }
    }
}
