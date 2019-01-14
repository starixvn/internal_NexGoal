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
            var controller = GetContronller();

            var actual = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task GetSetting_WhenCalled_ReturnsExpectedJsonResult()
        {
            var controller = GetContronller();
            pmSettingService.GetSettingAsync(null, 1).Returns(new List<MemberModel>
            {
                new MemberModel
                {
                    EmployeeId = 1,
                    ProjectManagerId = 2
                }
            });
            var actual = await controller.GetSetting(null, 1);

            var jsonResult = Assert.IsType<JsonResult>(actual);
            var value = Assert.IsAssignableFrom<IEnumerable<MemberModel>>(jsonResult.Value);

            Assert.NotEmpty(value);
        }

        [Fact]
        public async Task AddMembers_ValidData_ReturnsExpectedCommandResult()
        {
            var controller = GetContronller();
            pmSettingService.AddMembers(2, "3", sessionService.EmployeeId).Returns(new CommandResult
            {
                Code = 0
            });
            var actual = await controller.AddMembers(2, "3");

            var jsonResult = Assert.IsType<JsonResult>(actual);
            var value = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);

            Assert.True(value.IsSuccessful);
        }

        [Fact]
        public async Task RemoveMember_ValidData_ReturnsExpectedCommandResult()
        {
            var controller = GetContronller();
            pmSettingService.RemoveMember(2, 3, sessionService.EmployeeId).Returns(new CommandResult
            {
                Code = 0
            });
            var actual = await controller.RemoveMember(2, 3);

            var jsonResult = Assert.IsType<JsonResult>(actual);
            var value = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);

            Assert.True(value.IsSuccessful);
        }

        [Fact]
        public async Task GetProjectManagers_WhenCalled_ReturnsExpectedJsonResult()
        {
            var controller = GetContronller();
            pmSettingService.GetProjectManagersAsync().Returns(new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel
                {
                    EmployeeId = 2,
                }
            });
            var actual = await controller.GetProjectManagers();

            var jsonResult = Assert.IsType<JsonResult>(actual);
            var value = Assert.IsAssignableFrom<IEnumerable<EmployeeBaseModel>>(jsonResult.Value);

            Assert.NotEmpty(value);
        }

        private PMSettingController GetContronller()
        {
            pmSettingService = Substitute.For<IPMSettingService>();
            sessionService = Substitute.For<ISessionService>();
            sessionService.EmployeeId = 1;

            var controller = new PMSettingController(pmSettingService, sessionService);

            return controller;
        }
    }
}