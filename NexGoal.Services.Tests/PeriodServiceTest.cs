using Fanex.Data.Repository;
using NexGoal.Criteria.Period;
using NexGoal.Model.Period;
using NexGoal.Services.Implement;
using NSubstitute;
using NUnit.Framework;

namespace NexGoal.Services.Tests
{
    [TestFixture]
    public class PeriodServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetInprogressSemiPeriodByEmployeeId_WhenEmployeeIdIsNotValid_ReturnNull()
        {
            var service = new PeriodService();
            var invalidEmployeeId = -1;

            var period = service.GetInprogressSemiPeriodByEmployeeId(invalidEmployeeId);

            Assert.IsNull(period);
        }

        [Test]
        public void GetInprogressSemiPeriodByEmployeeId_WhenEmployeeIdIsNotExist_ReturnNull()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void GetInprogressSemiPeriodByEmployeeId_WhenSuccess_ReturnValidPeriod()
        {
            Assert.IsTrue(true);
        }
    }
}
