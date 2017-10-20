namespace NexGoal.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeWorkingByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public override string GetSettingKey() => "NexPM_GetEmployeeWorkingByEmployeeId";

        public override bool IsValid() => EmployeeId > 0 && EndDate >= StartDate;
    }
}