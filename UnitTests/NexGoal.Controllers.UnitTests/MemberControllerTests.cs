namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Models.Common;
    using NexGoal.Models.Employee;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
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
        private INominationPeriodService nominationPeriodService;
        private IEmployeeNominationService nominationService;
        private IRedmineService redmineService;

        [Fact]
        public async Task Index_WhenCalled_ReturnsAsExpectedViewResult()
        {
            // Arrange
            var employeeRoles = new List<EmployeeRoleModel>();
            var controller = GetMemberController();
            employeeService.GetRolesByEmployeeIdAsync(sessionService.EmployeeId).Returns(employeeRoles);

            // Act
            var actual = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.False((bool)viewResult.ViewData["IsReviewer"]);
        }

        [Fact]
        public async Task Details_WhenNotHavePermissionOnMember_ReturnActionAccessDenied()
        {
            // Arrange
            var controller = GetMemberController();
            var employees = new List<EmployeeBaseModel>();
            employeeService.GetMemberAsync(sessionService.EmployeeId).Returns(employees);
            nominationService.IsNominator(sessionService.EmployeeId).Returns(true);

            // Act
            var actual = await controller.Details(363);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
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
            Assert.IsType<EmployeeBaseModel>(viewResult.Model);
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
        public async Task GetNominationPeriod_WhenCalled_ReturnAsExpectedNominationPeriods()
        {
            // Arrange
            var expected = new List<NominationPeriodModel>
            {
                new NominationPeriodModel { NominationPeriodId = 1, Year = 2018, Quarter = 1 }
            };
            var controller = GetMemberController();
            nominationPeriodService.GetAsync().Returns(expected);

            // Act
            var result = await controller.GetNominationPeriod();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<NominationPeriodModel>>(jsonResult.Value);
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

        [Fact]
        public async Task GetRoleAccordingToMember_WhenCalled_ReturnAsExpectedRole()
        {
            // Arrange
            var expected = 1;
            var controller = GetMemberController();
            employeeService.GetRoleAccordingToMemberAsync(sessionService.EmployeeId, 1).Returns(expected);

            // Act
            var result = await controller.GetRoleAccordingToMember(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<int>(jsonResult.Value);
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
            nominationPeriodService = Substitute.For<INominationPeriodService>();
            nominationService = Substitute.For<IEmployeeNominationService>();
            redmineService = Substitute.For<IRedmineService>();
            var controller = new MemberController(sessionService, commonService, employeeService, nominationPeriodService, nominationService, redmineService);

            return controller;
        }

        #endregion Helpers
    }
}