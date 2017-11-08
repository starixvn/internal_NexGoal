namespace NexGoal.Criteria.Tests.Integration
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class GetEmployeeWorkingByEmployeeIdCriteriaTest
    {
        private static TestData[] testData = new[]
            {
                new TestData(0, DateTime.Now, DateTime.Now.AddDays(1)),
                new TestData(-1, DateTime.Now, DateTime.Now.AddDays(1)),
                new TestData(263, DateTime.Now, DateTime.Now.AddDays(-1))
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
            var actual = criteria.IsValid();

            Assert.IsFalse(actual);
        }
    }
}