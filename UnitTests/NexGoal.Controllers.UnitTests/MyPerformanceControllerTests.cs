namespace NexGoal.Controllers.UnitTests
{
    using System;
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

    public class MyPerformanceControllerTests
    {
        private ISessionService sessionService;
        private IPerformanceRecordService performanceService;

        [Fact]
        public async Task Index_WhenCalled_ReturnAsExpectedPerformanceRecords()
        {
            // Arrange
            var expected = new List<PerformanceRecordModel>
            {
                new PerformanceRecordModel
                {
                    PerformanceRecordId = 1,
                    Employees = new List<EmployeeBaseModel>
                    {
                        new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
                    },
                    Comment = "test"
                }
            };
            var controller = GetMyPerformanceController();
            performanceService.GetPerformanceRecordByEmployeeAsync(sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await controller.Index();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<PerformanceRecordModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetPerformanceTimeline_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var param = new GetPerformanceTimelineParam();
            var performanceRecords = new List<PerformanceRecordModel>
            {
                new PerformanceRecordModel
                {
                    PerformanceRecordId = 1,
                    Employees = new List<EmployeeBaseModel>
                    {
                        new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
                    },
                    Comment = "test"
                }
            };
            var tuple = new Tuple<IEnumerable<PerformanceRecordModel>, int>(performanceRecords, 1);
            var controller = GetMyPerformanceController();
            performanceService.GetPerformanceTimelineAsync(Arg.Any<GetPerformanceTimelineParam>()).Returns(tuple);

            // Act
            var result = await controller.GetPerformanceTimeline(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var records = (IEnumerable<PerformanceRecordModel>)jsonResult.Value.GetType().GetProperty("performanceRecords").GetValue(jsonResult.Value);
            var totalItems = (int)jsonResult.Value.GetType().GetProperty("totalItems").GetValue(jsonResult.Value);
            Assert.Equal(performanceRecords, records);
            Assert.Equal(1, totalItems);
        }

        #region Create

        [Fact]
        public async Task Create_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetMyPerformanceController();
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
            var controller = GetMyPerformanceController();
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
            var controller = GetMyPerformanceController();
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
            var controller = GetMyPerformanceController();
            performanceService.EditPerformanceRecordAsync(Arg.Any<EditPerformanceRecordModel>()).Returns(expected);

            // Act
            var result = await controller.Edit(new EditPerformanceRecordModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #endregion Edit

        [Fact]
        public async Task Delete_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetMyPerformanceController();
            performanceService.DeletePerformanceRecordAsync(1, sessionService.EmployeeId).Returns(expected);

            // Act
            var actual = await controller.Delete(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        [Fact]
        public async Task GetById_WhenCalled_ReturnAsExpectedPerformanceRecord()
        {
            // Arrange
            var expected = new PerformanceRecordModel
            {
                PerformanceRecordId = 1,
                Employees = new List<EmployeeBaseModel>
                {
                    new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
                },
                Comment = "test"
            };
            var controller = GetMyPerformanceController();
            performanceService.GetPerformanceRecordByIdAsync(1, sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await controller.GetById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<PerformanceRecordModel>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetByIds_WhenCalled_ReturnAsExpectedPerformanceRecords()
        {
            // Arrange
            var expected = new List<PerformanceRecordModel>
            {
                new PerformanceRecordModel
                {
                    PerformanceRecordId = 1,
                    Employees = new List<EmployeeBaseModel>
                    {
                        new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
                    },
                    Comment = "test"
                }
            };
            var controller = GetMyPerformanceController();
            performanceService.GetPerformanceRecordByIdsAsync("1", sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await controller.GetByIds("1");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<PerformanceRecordModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private MyPerformanceController GetMyPerformanceController()
        {
            sessionService = FakeSessionService();
            performanceService = Substitute.For<IPerformanceRecordService>();
            var controller = new MyPerformanceController(sessionService, performanceService);

            return controller;
        }

        #endregion Helpers
    }
}