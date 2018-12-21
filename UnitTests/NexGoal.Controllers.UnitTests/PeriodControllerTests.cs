namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;
    using NexGoal.Services;
    using NexGoal.ViewModels.Period;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class PeriodControllerTests
    {
        private ISessionService sessionService;
        private IPeriodService periodService;

        [Fact]
        public void Index_WhenCalled_ReturnsAsExpectedViewResult()
        {
            // Arrange
            var controller = GetPeriodController();
            sessionService.HasPermissionOnFunction(Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);

            // Act
            var actual = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<bool>(viewResult.Model);
            Assert.True(viewModel);
        }

        [Fact]
        public async Task GetPeriods_WhenCalled_ReturnAsExpectedPeriods()
        {
            // Arrange
            var expected = new List<PeriodModel>
            {
                new PeriodModel
                {
                    PeriodId = 1,
                    TargetTypeId = 1
                }
            };
            var controller = GetPeriodController();
            periodService.GetPeriodAsync().Returns(expected);

            // Act
            var result = await controller.GetPeriods();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<PeriodModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetPeriodProcessSetting_WhenCalled_ReturnAsExpectedProcessSettings()
        {
            // Arrange
            var expected = new List<PeriodProcessSettingModel>
            {
                new PeriodProcessSettingModel
                {
                    PeriodId = 1,
                    ProcessId = 1
                }
            };
            var controller = GetPeriodController();
            periodService.GetPeriodProcessSettingAsync(1).Returns(expected);

            // Act
            var result = await controller.GetPeriodProcessSetting(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<PeriodProcessSettingModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetById_WhenCalled_ReturnAsExpectedPeriod()
        {
            // Arrange
            var expected = new PeriodModel
            {
                PeriodId = 1,
                TargetTypeId = 1
            };
            var controller = GetPeriodController();
            periodService.GetByIdAsync(1).Returns(expected);

            // Act
            var result = await controller.GetById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<PeriodModel>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #region Create

        [Fact]
        public async Task Create_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetPeriodController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var result = await controller.Create(new CreatePeriodViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task Create_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetPeriodController();
            periodService.CreateAsync(sessionService.EmployeeId, Arg.Any<CreatePeriodModel>()).Returns(expected);

            // Act
            var result = await controller.Create(new CreatePeriodViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #endregion Create

        #region Edit

        [Fact]
        public async Task Edit_PeriodIsNotExist_ReturnRedirectToPageNotFound()
        {
            // Arrange
            PeriodModel eoq = null;
            var controller = GetPeriodController();
            periodService.GetByIdAsync(1).Returns(eoq);

            // Act
            var actual = await controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task Edit_PeriodIsExist_ReturnAsExpectedViewResult()
        {
            // Arrange
            var period = new PeriodModel { PeriodId = 1, Name = "2h 2018" };
            var controller = GetPeriodController();
            periodService.GetByIdAsync(1).Returns(period);

            // Act
            var actual = await controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(1, viewResult.Model);
        }

        [Fact]
        public async Task EditPost_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetPeriodController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var actual = await controller.Edit(new EditPeriodViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task EditPost_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var viewModel = new EditPeriodViewModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetPeriodController();
            periodService.EditAsync(sessionService.EmployeeId, Arg.Any<EditPeriodModel>()).Returns(expected);

            // Act
            var result = await controller.Edit(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #endregion Edit

        [Fact]
        public async Task EditStatus_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetPeriodController();
            periodService.EditStatusAsync(sessionService.EmployeeId, 1, true).Returns(expected);

            // Act
            var result = await controller.EditStatus(1, true);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditProcessSetting_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var settings = new List<EditProcessSettingModel>
            {
                new EditProcessSettingModel { PeriodId = 1, ProcessId = 1, Duration = 3 }
            };
            var expected = new CommandResult();
            var controller = GetPeriodController();
            periodService.EditProcessSettingAsync(sessionService.EmployeeId, 1, settings).Returns(expected);

            // Act
            var result = await controller.EditProcessSetting(1, settings);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEmployeeSettings_WhenCalled_ReturnAsExpectedSettings()
        {
            // Arrange
            var expected = new List<PARecordModel>
                {
                    new PARecordModel
                    {
                        PARecordId = 1,
                        EmployeeId = 1,
                        PeriodId = 1,
                        StatusId = 1
                    }
                };
            var controller = GetPeriodController();
            periodService.GetEmployeeSettingAsync(1).Returns(expected);

            // Act
            var result = await controller.GetEmployeeSettings(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<PARecordModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateEmployeeSettings_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetPeriodController();
            periodService.CreateEmployeeSettingAsync(sessionService.EmployeeId, 1, "1,2").Returns(expected);

            // Act
            var result = await controller.CreateEmployeeSettings(1, "1,2");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditEmployeeSetting_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var recordIds = new[] { 1, 2 };
            var processes = new List<EditEmployeeSettingModel>();
            var expected = new CommandResult();
            var controller = GetPeriodController();
            periodService.EditEmployeeSettingAsync(sessionService.EmployeeId, 1, recordIds, processes).Returns(expected);

            // Act
            var result = await controller.EditEmployeeSetting(1, recordIds, processes);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditEmployeeSettingStatus_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetPeriodController();
            periodService.EditEmployeeSettingStatusAsync(sessionService.EmployeeId, 1, true).Returns(expected);

            // Act
            var result = await controller.EditEmployeeSettingStatus(1, true);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEmployeeSettingById_WhenCalled_ReturnAsExpectedSettings()
        {
            // Arrange
            var expected = new PARecordModel
            {
                PARecordId = 1,
                EmployeeId = 1,
                PeriodId = 1,
                StatusId = 1
            };
            var controller = GetPeriodController();
            periodService.GetEmployeeSettingByIdAsync(1).Returns(expected);

            // Act
            var result = await controller.GetEmployeeSettingById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<PARecordModel>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GeneratePA_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetPeriodController();
            periodService.GeneratePAAsync(1, "1,2", sessionService.EmployeeId).Returns(expected);

            // Act
            var actual = await controller.GeneratePA(1, "1,2");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        [Fact]
        public async Task Delete_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetPeriodController();
            periodService.DeleteAsync(sessionService.EmployeeId, 1).Returns(expected);

            // Act
            var actual = await controller.Delete(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private PeriodController GetPeriodController()
        {
            sessionService = FakeSessionService();
            periodService = Substitute.For<IPeriodService>();
            var controller = new PeriodController(sessionService, periodService);

            return controller;
        }

        #endregion Helpers
    }
}