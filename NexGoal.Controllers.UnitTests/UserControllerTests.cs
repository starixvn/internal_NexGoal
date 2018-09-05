namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.UserManagement;
    using NexGoal.Services;
    using NexGoal.ViewModels.UserManagement;
    using NSubstitute;
    using Xunit;

    public class UserControllerTests
    {
        private IUserService userService;
        private ISessionService sessionService;

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewResult()
        {
            var controller = GetUserController();
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            var result = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<bool>(viewResult.Model);
            Assert.True(viewModel);
        }

        [Fact]
        public async Task GetUsers_WhenCalled_ReturnExpectedData()
        {
            var expectedResult = new List<UserModel>
            {
                new UserModel
                {
                    EmployeeId = 1,
                    Code = "123",
                }
            };

            var controller = GetUserController();
            userService.GetUsers(string.Empty, null, null, false).Returns(expectedResult);

            var result = await controller.GetUsers(string.Empty, null, null, false);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.IsAssignableFrom<IEnumerable<UserModel>>(jsonResult.Value);
        }

        [Fact]
        public async Task GetUser_WhenCalled_ReturnExpectedData()
        {
            var expectedResult = new UserModel
            {
                EmployeeId = 1,
                Code = "123",
            };

            var controller = GetUserController();
            userService.GetUser(1).Returns(expectedResult);

            var result = await controller.GetUser(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            var user = Assert.IsAssignableFrom<UserModel>(jsonResult.Value);
            Assert.Equal(expectedResult.EmployeeId, user.EmployeeId);
            Assert.Equal(expectedResult.Code, user.Code);
        }

        [Fact]
        public async Task GetRolesForUser_WhenCalled_ReturnExpectedData()
        {
            var expectedResult = new List<UserRoleModel>
            {
                new UserRoleModel
                {
                    RoleId = 1,
                    HasRole = true
                }
            };

            var controller = GetUserController();
            userService.GetRolesForUser(1).Returns(expectedResult);

            var result = await controller.GetRolesForUser(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            var roles = Assert.IsAssignableFrom<IEnumerable<UserRoleModel>>(jsonResult.Value);
            Assert.Equal(expectedResult.Count, roles.Count());
        }

        [Fact]
        public async Task EditUserRoles_ValidModel_ReturnExpectedCommandResult()
        {
            var controller = GetUserController();
            sessionService.EmployeeId = 1;
            userService.EditUserRoles(Arg.Any<EditUserRolesModel>()).Returns(new CommandResult
            {
                Code = 0
            });

            var result = await controller.EditUserRoles(new EditUserRolesViewModel
            {
                EmployeeId = 1,
                SelectedRoleIds = "1,2"
            });

            var jsonResult = Assert.IsType<JsonResult>(result);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.True(commandResult.IsSuccessful);
        }

        private UserController GetUserController()
        {
            userService = Substitute.For<IUserService>();
            sessionService = Substitute.For<ISessionService>();

            return new UserController(userService, sessionService);
        }
    }
}