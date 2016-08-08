using NUnit.Framework;
using System;

namespace NexGoal.Criteria.Tests.Integration
{
    [TestFixture]
    public class GetEmployeeWorkingByEmployeeIdCriteriaTest
    {
        private static TestData[] testData = new[] 
            {
                new TestData(0, DateTime.Now, DateTime.Now.AddDays(1)),
                new TestData(-1, DateTime.Now, DateTime.Now.AddDays(1)),
                new TestData(263, DateTime.MinValue, DateTime.Now.AddDays(1)),
                new TestData(263, DateTime.MaxValue, DateTime.Now.AddDays(1)),
                new TestData(263, DateTime.Now, DateTime.MinValue),
                new TestData(263, DateTime.Now, DateTime.MaxValue),
                new TestData(263, DateTime.Now, DateTime.Now.AddDays(-1)),
                new TestData(0, DateTime.MinValue, DateTime.MinValue)
            };

        [Test]
        public void GetSettingKey_CorrectKey()
        {
            var criteria = new GetEmployeeWorkingByEmployeeIdCriteria();
            var expectedKey = "NexPM_GetEmployeeWorkingByEmployeeId";

            var actualkey = criteria.GetSettingKey();

            Assert.AreEqual(expectedKey, actualkey);
        }

        [Test]
        public void IsValid_InValidParameter_ReturnFalse([ValueSource("testData")]TestData testData)
        {
            var criteria = new GetEmployeeWorkingByEmployeeIdCriteria
            {
                EmployeeId = testData.EmployeeId,
                StartDate = testData.StartDate,
                EndDate = testData.EndDate
            };
            var a = DateTime.Now;
            var actual = criteria.IsValid();

            Assert.IsFalse(actual);
        }
    }
}
