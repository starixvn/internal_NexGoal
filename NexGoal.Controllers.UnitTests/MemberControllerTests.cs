namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Models.Common;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class MemberControllerTests
    {
        private ICommonService commonService;
        private ISessionService sessionService;
        private IEmployeeService employeeService;
        private IEmployeeNominationService nominationService;

        [Fact]
        public async Task Index_WhenCalled_ReturnsAsExpectedViewResult()
        {
            // Arrange
            var controller = GetMemberController();
            // Act
            nominationService.IsNominator(sessionService.EmployeeId).Returns(true);
            var actual = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<bool>(viewResult.Model);
            Assert.True(viewModel);
        }

        [Fact]
        public async Task Details_WhenNotHavePermissionOnMember_ReturnActionAccessDenied()
        {
            // Arrange
            var controller = GetMemberController();
            var employees = new List<EmployeeBaseModel>();
            // Act
            employeeService.GetMemberAsync(sessionService.EmployeeId).Returns(employees);
            nominationService.IsNominator(sessionService.EmployeeId).Returns(true);
            var actual = await controller.Details(363);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
        }

        [Fact]
        public async Task Details_WhenHavePermissionOnMember_ReturnExpectViewModel()
        {
            // Arrange
            var controller = GetMemberController();
            var employees = new List<EmployeeBaseModel>();
            var employee = new EmployeeBaseModel { EmployeeId = 363 };
            employees.Add(employee);

            // Act
            employeeService.GetMemberAsync(sessionService.EmployeeId).Returns(employees);
            nominationService.IsNominator(sessionService.EmployeeId).Returns(true);
            commonService.GetEmployeeByIdAsync(363).Returns(employee);
            var actual = await controller.Details(363);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<EmployeeBaseModel>(viewResult.Model);
            Assert.True((bool)viewResult.ViewData["IsNominator"]);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 363;

            return service;
        }

        private MemberController GetMemberController()
        {
            sessionService = FakeSessionService();
            employeeService = Substitute.For<IEmployeeService>();
            commonService = Substitute.For<ICommonService>();
            nominationService = Substitute.For<IEmployeeNominationService>();
            var controller = new MemberController(commonService, sessionService, employeeService, nominationService);

            return controller;
        }

        #endregion Helpers
    }
}