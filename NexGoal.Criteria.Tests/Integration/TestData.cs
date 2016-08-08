using System;

namespace NexGoal.Criteria.Tests.Integration
{
    public class TestData
    {
        public TestData(int id, DateTime start, DateTime end)
        {
            EmployeeId = id;
            StartDate = start;
            EndDate = end;
        }

        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
