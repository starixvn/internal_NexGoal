using NexGoal.Core.Services;
using NexGoal.Services.Interfaces;
using NexGoal.Site.Controllers.API;
using NexGoal.ViewModel;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace NexGoal.Controller.Tests.API
{
    [TestFixture]
    public class IntergrationControllerTest
    {
        private IntegrationController controller;
        private IIntegrationService integrationService;
        private ISessionService sessionService;

        [SetUp]
        public void Setup()
        {
            integrationService = Substitute.For<IIntegrationService>();
            sessionService = Substitute.For<ISessionService>();
            sessionService.EmployeeId = 263;
            controller = new IntegrationController(integrationService);
            controller.SessionService = sessionService;
        }

        [Test]
        [Category("GetHRMSummary")]
        public void CheckCorrectControllerName()
        {
            /// Arrange
            var expectedControllerName = "IntegrationController";

            /// Action
            var actualControllerName = controller.GetType().Name;

            /// Assert
            Assert.AreEqual(expectedControllerName, actualControllerName);
        }

        [Test]
        [Category("GetHRMSummary")]
        public void GetHRMSummary_CheckCorrectActionName()
        {
            /// Arrange

            /// Action
            var actualAction = controller.GetType().GetMethod("GetHRMSummary");

            /// Assert
            Assert.IsNotNull(actualAction);
        }

        [Test]
        [Category("GetHRMSummary")]
        public void GetHRMSummary_Return_ListIsNotNull()
        {
            /// Arrange
            var data = new List<HRMSummaryViewModel>();
            integrationService.GetHRMSummary(sessionService.EmployeeId).Returns(data);

            /// Action
            var list = controller.GetHRMSummary(263);

            /// Assert
            Assert.IsNotNull(list as IEnumerable<HRMSummaryViewModel>);
        }

        [Test]
        [Category("GetEmployeeWorking")]
        public void GetEmployeeWorking_CheckCorrectActionName()
        {
            /// Arrange

            /// Action
            var actualAction = controller.GetType().GetMethod("GetEmployeeWorking");

            /// Assert
            Assert.IsNotNull(actualAction);
        }

        [Test]
        [Category("GetEmployeeWorking")]
        public void GetEmployeeWorking_Return_ListIsNotNull()
        {
            /// Arrange
            var data = new List<EmployeeWorkingViewModel>();
            integrationService.GetEmployeeWorking(sessionService.EmployeeId).Returns(data);

            /// Action
            var list = controller.GetEmployeeWorking(263);

            /// Assert
            Assert.IsNotNull(list as IEnumerable<EmployeeWorkingViewModel>);
        }
    }
}
