using NUnit.Framework;
using System;

namespace NexGoal.Criteria.Tests.Integration
{
    [TestFixture]
    public class GetHRMSummaryByEmployeeIdCriteriaTest
    {
        private static TestData[] _testData = new[] 
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

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetSettingKey_CorrectKey()
        {
            var criteria = new GetHRMSummaryByEmployeeIdCriteria();
            var expectedkey = "HRMSummary_GetByEmployeeId";

            var actualKey = criteria.GetSettingKey();

            Assert.AreEqual(expectedkey, actualKey);
        }

        [Test]
        public void IsValid_InValidParameter_ReturnFalse([ValueSource("_testData")]TestData testData)
        {
            var criteria = new GetHRMSummaryByEmployeeIdCriteria
            {
                EmployeeId = testData.EmployeeId,
                StartDate = testData.StartDate,
                EndDate = testData.EndDate
            };

            var actualResult = criteria.IsValid();

            Assert.IsFalse(actualResult);
        }

        [Test]
        public void IsValid_ValidParameter_ReturnTrue()
        {
            var criteria = new GetHRMSummaryByEmployeeIdCriteria
            {
                EmployeeId = 263,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10)
            };

            var actualResult = criteria.IsValid();

            Assert.IsTrue(actualResult);
        }
    }
}
