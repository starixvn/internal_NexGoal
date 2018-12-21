namespace NexGoal.Controllers.UnitTests
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class FlowControllerTests
    {
        private ISessionService sessionService;
        private ICommonService commonService;

        [Fact]
        public void Index_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var controller = GetFlowController();
            sessionService.HasPermissionOnFunction(FunctionKey.EvaluationFlow, Policy.Host).Returns(true);

            // Act
            var actual = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
            Assert.True((bool)viewResult.ViewData["IsHost"]);
        }

        [Fact]
        public async Task GetEmployeeById_WhenCalled_ReturnAsExpectedEmployee()
        {
            // Arrange
            var employee = new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" };
            var controller = GetFlowController();
            commonService.GetEmployeeByIdAsync(1).Returns(employee);

            // Act
            var actual = await controller.GetEmployeeById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<EmployeeBaseModel>(jsonResult.Value);
            Assert.Equal(employee, result);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private FlowController GetFlowController()
        {
            sessionService = FakeSessionService();
            commonService = Substitute.For<ICommonService>();
            var controller = new FlowController(sessionService, commonService);

            return controller;
        }

        #endregion Helpers
    }
}