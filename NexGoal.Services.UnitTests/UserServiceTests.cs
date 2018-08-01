namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.UserManagement;
    using NexGoal.Models.UserManagement;
    using NSubstitute;
    using Xunit;

    public class UserServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task GetUsers_WhenCalled_ReturnExpectedUsers()
        {
            var expectedUsers = new List<UserModel>
            {
                new UserModel
                {
                    EmployeeId = 1,
                    FullName = "Eha",
                    NickName ="Hea"
                }
            };

            var service = GetUserService();
            baseService.GetDataAsync<UserModel>(Arg.Any<GetUsersCriteria>()).Returns(expectedUsers);

            var result = await service.GetUsers(null, null, null, false);

            Assert.Equal(expectedUsers.Count, result.Count());
        }

        [Fact]
        public async Task GetUser_WhenCalled_ReturnExpectedUser()
        {
            var expectedUser = new UserModel
            {
                EmployeeId = 1,
                FullName = "Eha",
                NickName = "Hea"
            };

            var service = GetUserService();
            baseService.GetDataEntityAsync<UserModel>(Arg.Any<GetUserCriteria>()).Returns(expectedUser);

            var result = await service.GetUser(1);

            Assert.Equal(expectedUser.EmployeeId, result.EmployeeId);
            Assert.Equal(expectedUser.FullName, result.FullName);
            Assert.Equal(expectedUser.NickName, result.NickName);
        }

        [Fact]
        public async Task GetRolesForUser_WhenCalled_ReturnExpectedResults()
        {
            var expectedRoles = new List<UserRoleModel>
            {
                new UserRoleModel
                {
                    RoleId = 1,
                    Role= "HR",
                    HasRole = true
                },
                new UserRoleModel
                {
                    RoleId = 2,
                    Role= "SA",
                    HasRole = false
                }
            };

            var service = GetUserService();
            baseService.GetDataAsync<UserRoleModel>(Arg.Any<GetRolesForUserCriteria>()).Returns(expectedRoles);

            var result = await service.GetRolesForUser(1);

            Assert.Equal(expectedRoles.Count, result.Count());
        }

        [Fact]
        public async Task EditUserRoles_ValidModel_ReturnExpectedCommandResult()
        {
            var expectedCommandResult = new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };

            var service = GetUserService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditUserRolesCriteria>()).Returns(expectedCommandResult);

            var result = await service.EditUserRoles(new EditUserRolesModel { EmployeeId = 1, SelectedRoleIds = "1,2,3", ModifiedBy = 2 });

            Assert.Equal(expectedCommandResult.Code, result.Code);
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private UserService GetUserService()
        {
            baseService = FakeBaseService();
            var service = new UserService(baseService);

            return service;
        }
    }
}