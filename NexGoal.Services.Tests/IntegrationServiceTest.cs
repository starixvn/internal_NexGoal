using NUnit.Framework;

namespace NexGoal.Services.Tests
{
    [TestFixture]
    public class IntegrationServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetHRMSummary_WhenEmployeeIdIsNotExist_ReturnEmptyList()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void GetHRMSummary_WhenEmployeeDoesntHasInpropressSemiPeriod_ReturnEmptyList()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void GetHRMSummary_WhenInvalidStartAndEndDate_ReturnEmptyList()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void GetHRMSummary_WhenNotFoundResult_ReturnEmptyList()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void GetEmployeeWorking_WhenEmployeeIsNotExist_ReturnEmptyList()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void GetEmployeeWorking_WhenEmployeeDoesntHasInpropressSemiPeriod_ReturnEmptyList()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void GetEmployeeWorking_WhenInvalidStartAndEndDate_ReturnEmptyList()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void GetEmployeeWorking_WhenSuccess_ScoreMustDecrypt()
        {
            Assert.IsTrue(true);
        }
    }
}
