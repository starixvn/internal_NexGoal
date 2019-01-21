namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Services;
    using NexGoal.ViewModels.EOQY;
    using NexGoal.ViewModels.NominationPeriod;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class NominationPeriodControllerTests
    {
        private ISessionService sessionService;
        private INominationPeriodService nominationPeriodService;

        [Fact]
        public void Index_WhenCalled_ReturnsAsExpectedViewResult()
        {
            // Arrange
            var activeTab = NominationPeriodController.NavTabs.EOQ;
            var controller = GetNominationPeriodController();
            sessionService.HasPermissionOnFunction(Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);

            // Act
            var actual = controller.Index(activeTab);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<NominationManagementViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Equal((int)activeTab, viewModel.ActiveTab);
            Assert.True(viewModel.IsHost);
        }

        [Fact]
        public async Task Delete_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetNominationPeriodController();
            nominationPeriodService.DeleteAsync(1, sessionService.EmployeeId).Returns(expected);

            // Act
            var actual = await controller.Delete(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        [Fact]
        public async Task GetRoleSetting_WhenCalled_ReturnAsExpectedSettings()
        {
            // Arrange
            var param = new GetRoleSettingParam();
            var settings = new List<RoleSettingModel>
            {
                new RoleSettingModel { EmployeeId = 1, NickName = "tim.trinh", RoleIds = "1,2" }
            };
            var controller = GetNominationPeriodController();
            nominationPeriodService.GetRoleSettingAsync(param).Returns(settings);

            // Act
            var actual = await controller.GetRoleSetting(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<RoleSettingViewModel>>(jsonResult.Value);
            Assert.Single(result);
        }

        [Fact]
        public async Task EditNominatorSetting_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var setting = new EditRoleSettingModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetNominationPeriodController();
            nominationPeriodService.EditRoleSettingAsync(setting, sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await controller.EditRoleSetting(setting);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private NominationPeriodController GetNominationPeriodController()
        {
            sessionService = Helpers.FakeSessionService();
            nominationPeriodService = Substitute.For<INominationPeriodService>();
            var controller = new NominationPeriodController(sessionService, nominationPeriodService);

            return controller;
        }

        #endregion Helpers
    }
}