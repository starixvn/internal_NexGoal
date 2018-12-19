namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.FlowSetting;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class EmployeeFlowSettingControllerTests
    {
        private IEmployeeFlowSettingService flowSettingService;

        [Fact]
        public async Task GetEmployeeHaveSetting_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var employees = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
            };
            var controller = GetEmployeeFlowSettingController();
            flowSettingService.GetEmployeeHaveSettingAsync(1).Returns(employees);

            // Act
            var actual = await controller.GetEmployeeHaveSetting(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<EmployeeBaseModel>>(jsonResult.Value);
            Assert.Equal(employees, result);
        }

        [Fact]
        public async Task GetEmployeeUnsetting_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var employees = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
            };
            var controller = GetEmployeeFlowSettingController();
            flowSettingService.GetEmployeeUnsettingAsync(1).Returns(employees);

            // Act
            var actual = await controller.GetEmployeeUnsetting(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<EmployeeBaseModel>>(jsonResult.Value);
            Assert.Equal(employees, result);
        }

        [Fact]
        public async Task GetEmployeeFlowSetting_WhenCalled_ReturnAsExpectedFlowSettings()
        {
            // Arrange
            var flowSettings = new List<EmployeeFlowSettingModel>
            {
                new EmployeeFlowSettingModel
                {
                    EmployeeId = 1,
                    NickName = "Tim",
                    Evaluator = new FlowSettingModel { EmployeeId = 2, NickName = "Quinn" }
                }
            };
            var controller = GetEmployeeFlowSettingController();
            flowSettingService.GetEmployeeFlowSettingAsync().Returns(flowSettings);

            // Act
            var actual = await controller.GetEmployeeFlowSetting();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<EmployeeFlowSettingModel>>(jsonResult.Value);
            Assert.Equal(flowSettings, result);
        }

        [Fact]
        public async Task GetById_WhenCalled_ReturnAsExpectedFlowSetting()
        {
            // Arrange
            var flowSetting = new EmployeeFlowSettingModel
            {
                EmployeeId = 1,
                NickName = "Tim",
                Evaluator = new FlowSettingModel { EmployeeId = 2, NickName = "Quinn" }
            };
            var controller = GetEmployeeFlowSettingController();
            flowSettingService.GetByIdAsync(1).Returns(flowSetting);

            // Act
            var actual = await controller.GetById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<EmployeeFlowSettingModel>(jsonResult.Value);
            Assert.Equal(flowSetting, result);
        }

        [Fact]
        public async Task Edit_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var flowSettings = new List<FlowSettingModel>();
            var expected = new CommandResult();
            var controller = GetEmployeeFlowSettingController();
            flowSettingService.EditAsync(1, flowSettings).Returns(expected);

            // Act
            var actual = await controller.Edit(1, flowSettings);

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
            var controller = GetEmployeeFlowSettingController();
            flowSettingService.DeleteAsync(1).Returns(expected);

            // Act
            var actual = await controller.Delete(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        #region Helpers

        private EmployeeFlowSettingController GetEmployeeFlowSettingController()
        {
            flowSettingService = Substitute.For<IEmployeeFlowSettingService>();
            var controller = new EmployeeFlowSettingController(flowSettingService);

            return controller;
        }

        #endregion Helpers
    }
}