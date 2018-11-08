namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Redmine;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class MemberControllerTests
    {
        private ICommonService commonService;
        private ISessionService sessionService;
        private IEmployeeService employeeService;
        private IEmployeeNominationService nominationService;
        private IRedmineService redmineService;

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

        [Fact]
        public async Task GetMemberPA_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var controller = GetMemberController();
            var expected = new List<PARecordModel>
            {
                new PARecordModel { PARecordId = 1, Employee = new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" } }
            };

            // Act
            employeeService.GetMemberPerformanceAppraisalResultAsync(sessionService.EmployeeId, null, null).Returns(expected);
            var result = await controller.GetMemberPerformanceAppraisalResult(null, null);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<PARecordModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMemberNominationsResult_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var controller = GetMemberController();
            var expected = new List<EmployeeNominationModel>
            {
                new EmployeeNominationModel
                {
                    NominationPeriodId = 1, StatusId = NominationStatus.Nominated
                }
            };

            // Act
            employeeService.GetMemberNominationResultAsync(sessionService.EmployeeId, 363)
                .Returns(expected);
            var result = await controller.GetMemberNominationResult(363);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<EmployeeNominationModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMemberNominationDetails_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var controller = GetMemberController();
            var expected = new List<EmployeeNominationModel>
            {
                new EmployeeNominationModel
                {
                    NominationPeriodId = 1, StatusId = NominationStatus.Nominated
                }
            };

            // Act
            employeeService.GetMemberNominationDetailsAsync(sessionService.EmployeeId, 2, 363)
                .Returns(expected);
            var result = await controller.GetMemberNominationDetails(2, 363);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<EmployeeNominationModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMembersWorkload_WhenCalled_ReturnAsExpectedWorkloads()
        {
            // Arrange
            var expected = new List<TeamWorkloadModel>
            {
                new TeamWorkloadModel { GroupId = 1, GroupName = "Nexus" }
            };
            var controller = GetMemberController();
            redmineService.GetTeamWorkload(sessionService.EmployeeId, 1).Returns(expected);

            // Act
            var result = await controller.GetMembersWorkload(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<TeamWorkloadModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
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
            redmineService = Substitute.For<IRedmineService>();
            var controller = new MemberController(commonService, sessionService, employeeService, nominationService, redmineService);

            return controller;
        }

        #endregion Helpers
    }
}