namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models;
    using NexGoal.Models.Function;
    using NexGoal.Services;
    using NexGoal.ViewModels.Function;
    using NSubstitute;
    using Xunit;

    public class RoleControllerTests
    {
        private ICommonService commonService;
        private ISessionService sessionService;
        private IFunctionService functionService;

        [Fact]
        public async Task Index_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var roleId = 2;
            var functions = new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1, Policy = Policy.Host }
            };
            var refResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel { ResourceId = 1, Value = "SA" }
            };
            var selectList = new SelectList(refResources, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value));
            var controller = GetRoleController();

            functionService.GetTreeFunctionsAsync().Returns(functions);
            functionService.GetFunctionByRoleIdAsync(roleId).Returns(functions);
            commonService.GetSelectList(Arg.Any<IEnumerable<ReferenceResourceModel>>(), nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value))
                .Returns(selectList);
            sessionService.HasPermissionOnFunction(Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);

            // Act
            var actual = await controller.Index(roleId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<AssignPermissionViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(roleId, viewModel.RoleId);
            Assert.Equal(functions, viewModel.Functions);
            Assert.Equal(selectList, viewModel.Roles);
            Assert.True(viewModel.IsHost);
        }

        [Fact]
        public async Task AssignPermission_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var roleId = 1;
            var expected = new CommandResult
            {
                Code = 0
            };
            var permissions = new List<RolePermissionModel>
            {
                new RolePermissionModel { FunctionId = 1, Policy = Policy.Host }
            };
            var controller = GetRoleController();
            functionService.AssignFunctionToRoleAsync(roleId, permissions).Returns(expected);

            // Act
            var actual = await controller.AssignPermission(roleId, permissions);

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

        private ICommonService FakeCommonService()
        {
            var service = Substitute.For<ICommonService>();

            return service;
        }

        private IFunctionService FakeFunctionService()
        {
            var service = Substitute.For<IFunctionService>();

            return service;
        }

        private RoleController GetRoleController()
        {
            commonService = FakeCommonService();
            sessionService = FakeSessionService();
            functionService = FakeFunctionService();
            var controller = new RoleController(sessionService, commonService, functionService);

            return controller;
        }

        #endregion Helpers
    }
}