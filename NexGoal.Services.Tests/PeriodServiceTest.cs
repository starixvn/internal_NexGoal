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
            Assert.IsTrue(true);
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
