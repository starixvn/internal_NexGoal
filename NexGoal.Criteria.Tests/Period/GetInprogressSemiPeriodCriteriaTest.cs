using NexGoal.Criteria.Period;
using NUnit.Framework;

namespace NexGoal.Criteria.Tests.Period
{
    [TestFixture]
    public class GetInprogressSemiPeriodCriteriaTest
    {
        [Test]
        public void GetSettingKey_CorrectKey()
        {
            var criteria = new GetInprogressSemiPeriodCriteria();
            var expectedkey = "Period_GetInprogressSemiPeriod";

            var actualKey = criteria.GetSettingKey();

            Assert.AreEqual(expectedkey, actualKey);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [Test]
        public void IsValid_InValidEmployeeId_ReturnFalse(int employeeId)
        {
            var criteria = new GetInprogressSemiPeriodCriteria { EmployeeId = employeeId };

            var actualResult = criteria.IsValid();

            Assert.IsFalse(actualResult);
        }

        [Test]
        public void IsValid_ValidEmployeeId_ReturnTrue()
        {
            var criteria = new GetInprogressSemiPeriodCriteria { EmployeeId = 263 };

            var actualResult = criteria.IsValid();

            Assert.IsTrue(actualResult);
        }
    }
}
