namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PMSetting;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class PMSettingControllerTests
    {
        private ISessionService sessionService;
        private IPMSettingService pmSettingService;

        [Fact]
        public void Index_WhenCalled_ReturnsExpectedViewResult()
        {
            // Arrange
            var controller = GetContronller();

            // Act
            var actual = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task GetSetting_WhenCalled_ReturnsExpectedJsonResult()
        {
            // Arrange
            var members = new List<MemberModel>
            {
                new MemberModel { EmployeeId = 1, NickName = "elton", ProjectManagerId = 2 }
            };
            var controller = GetContronller();
            pmSettingService.GetSettingAsync(null, 1).Returns(members);

            // Act
            var result = await controller.GetSetting(null, 1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<MemberModel>>(jsonResult.Value);
            Assert.Equal(members, actual);
        }

        [Fact]
        public async Task AddMembers_ValidData_ReturnsExpectedCommandResult()
        {
            // Arrange
            var commandResult = new CommandResult();
            var controller = GetContronller();
            pmSettingService.AddMemberAsync(2, "3", sessionService.EmployeeId).Returns(commandResult);

            // Act
            var result = await controller.AddMembers(2, "3");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(commandResult, actual);
        }

        [Fact]
        public async Task RemoveMember_ValidData_ReturnsExpectedCommandResult()
        {
            // Arrange
            var commandResult = new CommandResult();
            var controller = GetContronller();
            pmSettingService.RemoveMemberAsync(2, 3, sessionService.EmployeeId).Returns(commandResult);

            // Act
            var result = await controller.RemoveMember(2, 3);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(commandResult, actual);
        }

        [Fact]
        public async Task GetProjectManagers_WhenCalled_ReturnsExpectedJsonResult()
        {
            // Arrange
            var projectManagers = new List<Employee>
            {
                new Employee { EmployeeId = 2, NickName = "richard" }
            };
            var controller = GetContronller();
            pmSettingService.GetProjectManagerAsync().Returns(projectManagers);

            // Act
            var result = await controller.GetProjectManagers();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<Employee>>(jsonResult.Value);
            Assert.Equal(projectManagers, actual);
        }

        private PMSettingController GetContronller()
        {
            pmSettingService = Substitute.For<IPMSettingService>();
            sessionService = Helpers.FakeSessionService();

            var controller = new PMSettingController(pmSettingService, sessionService);

            return controller;
        }
    }
}