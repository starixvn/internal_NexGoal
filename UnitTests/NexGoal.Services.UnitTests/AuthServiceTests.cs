namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Auth;
    using NexGoal.Models.Auth;
    using NexGoal.Models.Common;
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

        [Fact]
        public async Task GetEmployeeInfoAsync_WhenCalled_ReturnAsExpectedEmployee()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 1, NickName = "Oliver" };
            ValueTuple<Employee, bool, int> expected = new ValueTuple<Employee, bool, int>(employee, true, 10);

            var service = GetAuthService();
            baseService.GetDataEntityAsync<Employee>(Arg.Any<GetEmployeeInfoCriteria>()).Returns(employee);

            // Act
            var actual = await service.GetEmployeeInfoAsync(1);

            // Assert
            Assert.Equal(expected.Item1, actual.Item1);
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

        [Fact]
        public async Task WriteLogLoginAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var criteria = new WriteLogLoginCriteria();
            var model = new LogLogin();
            var service = GetAuthService();
            baseService.ExecuteNonQueryAsync(Arg.Do<WriteLogLoginCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            await service.WriteLogLoginAsync(model);

            // Assert
            Assert.Equal(model.Username, criteria.Username);
            Assert.Equal(model.IP, criteria.IP);
            Assert.Equal(model.MachineName, criteria.MachineName);
            Assert.Equal(model.Platform, criteria.Platform);
        }

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