namespace NexGoal.Controllers.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.Common;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class PRTrackingControllerTests
    {
        private IPRTrackingService prTrackingService;
        private ISessionService sessionService;

        [Fact]
        public void Index_WhenCalled_ReturnsExpectedViewResult()
        {
            var controller = GetContronller();

            var actual = controller.Index();

            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public async Task GetGroupsAndMembersAsync_WhenCalled_ReturnsExpectedJsonResult()
        {
            var expected = new ValueTuple<IEnumerable<Group>, IEnumerable<Employee>>(
                    new List<Group> { new Group { GroupId = 1, Name = "MGT" } },
                    new List<Employee> { new Employee { EmployeeId = 1, GroupId = 1 } });

            var controller = GetContronller();
            prTrackingService.GetGroupsAndMembersAsync(sessionService.EmployeeId)
                .Returns(expected);

            var result = await controller.GetGroupsAndMembers();

            Assert.IsType<JsonResult>(result);
        }

        private PRTrackingController GetContronller()
        {
            prTrackingService = Substitute.For<IPRTrackingService>();
            sessionService = Helpers.FakeSessionService();

            var controller = new PRTrackingController(sessionService, prTrackingService);

            return controller;
        }
    }
}