namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class MemberPerformanceControllerTests
    {
        private ISessionService sessionService;
        private IPerformanceRecordService performanceService;

        [Fact]
        public async Task Index_WhenCalled_ReturnAsExpectedPerformanceRecords()
        {
            // Arrange
            var expected = new List<PerformanceRecord>
            {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    Employees = new List<Employee>
                    {
                        new Employee { EmployeeId = 1, NickName = "Tim" }
                    },
                    Comment = "test"
                }
            };
            var controller = GetMemberPerformanceController();
            performanceService.GetMemberPerformanceAsync(sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await controller.Index();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<PerformanceRecord>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMemberPerformanceTimeline_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var param = new GetMemberPerformanceTimelineParam();
            var expected = new List<MemberPerformanceTimelineModel>
            {
                new MemberPerformanceTimelineModel
                {
                    EmployeeId = 1,
                    NickName = "Tim",
                    PerformanceRecords = new List<PerformanceRecord>
                    {
                        new PerformanceRecord
                        {
                            PerformanceRecordId = 1,
                            Comment = "test"
                        }
                    }
                }
            };
            var controller = GetMemberPerformanceController();
            performanceService.GetMemberPerformanceTimelineAsync(Arg.Any<GetMemberPerformanceTimelineParam>()).Returns(expected);

            // Act
            var result = await controller.GetMemberPerformanceTimeline(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<MemberPerformanceTimelineModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #region Create

        [Fact]
        public async Task Create_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetMemberPerformanceController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var result = await controller.Create(new CreatePerformanceRecordModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task Create_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetMemberPerformanceController();
            performanceService.CreatePerformanceRecordAsync(Arg.Any<CreatePerformanceRecordModel>()).Returns(expected);

            // Act
            var result = await controller.Create(new CreatePerformanceRecordModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #endregion Create

        #region Edit

        [Fact]
        public async Task Edit_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetMemberPerformanceController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var result = await controller.Edit(new EditPerformanceRecordModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task Edit_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetMemberPerformanceController();
            performanceService.EditPerformanceRecordAsync(Arg.Any<EditPerformanceRecordModel>()).Returns(expected);

            // Act
            var result = await controller.Edit(new EditPerformanceRecordModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #endregion Edit

        #region Helpers

        private MemberPerformanceController GetMemberPerformanceController()
        {
            sessionService = Helpers.FakeSessionService();
            performanceService = Substitute.For<IPerformanceRecordService>();
            var controller = new MemberPerformanceController(sessionService, performanceService);

            return controller;
        }

        #endregion Helpers
    }
}