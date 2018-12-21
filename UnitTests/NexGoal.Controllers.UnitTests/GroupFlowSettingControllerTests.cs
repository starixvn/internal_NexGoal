namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.FlowSetting;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class GroupFlowSettingControllerTests
    {
        private ISessionService sessionService;
        private IGroupFlowSettingService flowSettingService;

        [Fact]
        public async Task GetGroupHaveSetting_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var groups = new List<GroupBaseModel>
            {
                new GroupBaseModel { GroupId = 1, Code = "NEX", Name = "Nexus" }
            };
            var controller = GetGroupFlowSettingController();
            flowSettingService.GetGroupHaveSettingAsync().Returns(groups);

            // Act
            var actual = await controller.GetGroupHaveSetting();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<GroupBaseModel>>(jsonResult.Value);
            Assert.Equal(groups, result);
        }

        [Fact]
        public async Task GetGroupFlowSetting_WhenCalled_ReturnAsExpectedFlowSettings()
        {
            // Arrange
            var flowSettings = new List<GroupFlowSettingModel>
            {
                new GroupFlowSettingModel
                {
                    GroupId = 1,
                    Code = "NEX",
                    Name = "Nexus",
                    Evaluator = new FlowSettingModel { EmployeeId = 2, NickName = "Quinn" }
                }
            };
            var controller = GetGroupFlowSettingController();
            flowSettingService.GetGroupFlowSettingAsync().Returns(flowSettings);

            // Act
            var actual = await controller.GetGroupFlowSetting();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<GroupFlowSettingModel>>(jsonResult.Value);
            Assert.Equal(flowSettings, result);
        }

        [Fact]
        public async Task GetById_WhenCalled_ReturnAsExpectedFlowSetting()
        {
            // Arrange
            var flowSetting = new GroupFlowSettingModel
            {
                GroupId = 1,
                Code = "NEX",
                Name = "Nexus",
                Evaluator = new FlowSettingModel { EmployeeId = 2, NickName = "Quinn" }
            };
            var controller = GetGroupFlowSettingController();
            flowSettingService.GetByIdAsync(1).Returns(flowSetting);

            // Act
            var actual = await controller.GetById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<GroupFlowSettingModel>(jsonResult.Value);
            Assert.Equal(flowSetting, result);
        }

        [Fact]
        public async Task Edit_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var flowSettings = new List<FlowSettingModel>();
            var expected = new CommandResult();
            var controller = GetGroupFlowSettingController();
            flowSettingService.EditAsync(sessionService.EmployeeId, 1, flowSettings).Returns(expected);

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
            var controller = GetGroupFlowSettingController();
            flowSettingService.DeleteAsync(sessionService.EmployeeId, 1).Returns(expected);

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

        private GroupFlowSettingController GetGroupFlowSettingController()
        {
            sessionService = FakeSessionService();
            flowSettingService = Substitute.For<IGroupFlowSettingService>();
            var controller = new GroupFlowSettingController(sessionService, flowSettingService);

            return controller;
        }

        #endregion Helpers
    }
}