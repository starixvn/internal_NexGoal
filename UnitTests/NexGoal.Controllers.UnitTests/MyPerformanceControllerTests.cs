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
            var controller = GetMyPerformanceController();
            performanceService.GetPerformanceRecordByEmployeeAsync(sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await controller.Index();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<PerformanceRecord>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetPerformanceTimeline_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var param = new GetPerformanceTimelineParam();
            var performanceRecords = new List<PerformanceRecord>
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
            var ValueTuple = new ValueTuple<IEnumerable<PerformanceRecord>, int>(performanceRecords, 1);
            var controller = GetMyPerformanceController();
            performanceService.GetPerformanceTimelineAsync(Arg.Any<GetPerformanceTimelineParam>()).Returns(ValueTuple);

            // Act
            var result = await controller.GetPerformanceTimeline(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var records = (IEnumerable<PerformanceRecord>)jsonResult.Value.GetType().GetProperty("performanceRecords").GetValue(jsonResult.Value);
            var totalItems = (int)jsonResult.Value.GetType().GetProperty("totalItems").GetValue(jsonResult.Value);
            Assert.Equal(performanceRecords, records);
            Assert.Equal(1, totalItems);
        }

        [Fact]
        public async Task GetDraftById_WhenCalled_ReturnExpected()
        {
            var expected = new PerformanceRecord
            {
                PerformanceRecordId = 1,
                Employees = new List<Employee>
                    {
                        new Employee { EmployeeId = 1, NickName = "Tim" }
                    },
                Comment = "test"
            };
            var controller = GetMyPerformanceController();
            performanceService.GetDraftPerformanceRecordByIdAsync(1, 1).Returns(expected);

            // Act
            var result = await controller.GetDraftById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<PerformanceRecord>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #region Create

        [Fact]
        public async Task Create_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetMyPerformanceController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var result = await controller.Create(new CreatePerformanceRecordDto());

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
            performanceService.CreatePerformanceRecordAsync(Arg.Any<CreatePerformanceRecordDto>()).Returns(expected);

            // Act
            var result = await controller.Create(new CreatePerformanceRecordDto());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateDraft_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetMyPerformanceController();
            controller.ModelState.AddModelError("comment", "required");

            // Act
            var result = await controller.CreateDraft(new CreatePerformanceRecordDto());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task CreateDraft_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetMyPerformanceController();
            performanceService.CreateDraftPerformanceRecordAsync(Arg.Any<CreatePerformanceRecordDto>()).Returns(expected);

            // Act
            var result = await controller.CreateDraft(new CreatePerformanceRecordDto());

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
            var result = await controller.Edit(new EditPerformanceRecordDto());

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
            performanceService.EditPerformanceRecordAsync(Arg.Any<EditPerformanceRecordDto>()).Returns(expected);

            // Act
            var result = await controller.Edit(new EditPerformanceRecordDto());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditDraft_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetMyPerformanceController();
            controller.ModelState.AddModelError("comment", "required");

            // Act
            var result = await controller.EditDraft(new EditPerformanceRecordDto());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task EditDraft_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetMyPerformanceController();
            performanceService.EditDraftPerformanceRecordAsync(Arg.Any<EditPerformanceRecordDto>()).Returns(expected);

            // Act
            var result = await controller.EditDraft(new EditPerformanceRecordDto());

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
            var expected = new PerformanceRecord
            {
                PerformanceRecordId = 1,
                Employees = new List<Employee>
                {
                    new Employee { EmployeeId = 1, NickName = "Tim" }
                },
                Comment = "test"
            };
            var controller = GetMyPerformanceController();
            performanceService.GetPerformanceRecordByIdAsync(1, sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await controller.GetById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<PerformanceRecord>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetByIds_WhenCalled_ReturnAsExpectedPerformanceRecords()
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
            var controller = GetMyPerformanceController();
            performanceService.GetPerformanceRecordByIdsAsync("1", sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await controller.GetByIds("1");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<PerformanceRecord>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private MyPerformanceController GetMyPerformanceController()
        {
            sessionService = Helpers.FakeSessionService();
            performanceService = Substitute.For<IPerformanceRecordService>();
            var controller = new MyPerformanceController(sessionService, performanceService);

            return controller;
        }

        #endregion Helpers
    }
}