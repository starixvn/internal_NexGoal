namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Auth;
    using NexGoal.Models.Auth;
    using NSubstitute;
    using Xunit;

    public class AuthServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task GetEmployeeByDomainUsernameAsync_WhenCalled_ReturnAsExpectedEmployee()
        {
            // Arrange
            var expected = new EmployeeSession { EmployeeId = 1, NickName = "Tim" };
            var service = GetAuthService();
            baseService.GetDataEntityAsync<EmployeeSession>(Arg.Any<GetEmployeeByDomainUsernameCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetEmployeeByDomainUsernameAsync("tim");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetFunctionByEmployeeIdAsync_WhenCalled_ReturnAsExpectedFunctions()
        {
            // Arrange
            var expected = new List<FunctionSession> {
                new FunctionSession { FunctionId = 1, FunctionKey = "Configuration", Name = "Configuration" }
            };
            var service = GetAuthService();
            baseService.GetDataAsync<FunctionSession>(Arg.Any<GetFunctionByEmployeeIdCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetFunctionByEmployeeIdAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region GetMenuByEmployeeIdAsync

        [Fact]
        public async Task GetMenuByEmployeeIdAsync_HasNotChildMenu_ReturnAsExpectedMenus()
        {
            // Arrange
            var expected = new List<MenuItemSession> {
                new MenuItemSession { MenuId = 1, Name = "Dasboard" },
                new MenuItemSession { MenuId = 2, Name = "Member" }
            };
            var service = GetAuthService();
            baseService.GetDataAsync<MenuItemSession>(Arg.Any<GetMenuByEmployeeIdCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetMenuByEmployeeIdAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMenuByEmployeeIdAsync_HasChildMenu_ReturnAsExpectedMenus()
        {
            // Arrange
            var menus = new List<MenuItemSession> {
                new MenuItemSession { MenuId = 1, Name = "Admin Center" },
                new MenuItemSession { MenuId = 2, Name = "Employee", ParentId = 1 }
            };
            var service = GetAuthService();
            baseService.GetDataAsync<MenuItemSession>(Arg.Any<GetMenuByEmployeeIdCriteria>()).Returns(menus);

            // Act
            var actual = await service.GetMenuByEmployeeIdAsync(1);

            // Assert
            Assert.Single(actual);
            Assert.Equal(menus.First().MenuId, actual.First().MenuId);
            Assert.Equal(menus.First().Name, actual.First().Name);
        }

        #endregion GetMenuByEmployeeIdAsync

        #region Helpers

        private AuthService GetAuthService()
        {
            baseService = Substitute.For<IBaseService>();
            var service = new AuthService(baseService);

            return service;
        }

        #endregion Helpers
    }
}