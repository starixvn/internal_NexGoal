namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Function;
    using NexGoal.Models.Function;
    using NSubstitute;
    using Xunit;

    public class FunctionServiceTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;

        [Fact]
        public async Task GetFunctionByRoleIdAsync_WhenCalled_ReturnAsExpectedFunctions()
        {
            // Arrange
            var expected = GetFunctionForTest();
            var functionService = GetFunctionService();
            baseService.GetDataAsync<FunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>()).Returns(expected);

            // Act
            var actual = await functionService.GetFunctionByRoleIdAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetTreeFunctions_WhenCalled_ReturnAsExpectedTreeFunctions()
        {
            // Arrange
            var expected = GetFunctionForTest();
            var functionService = GetFunctionService();
            baseService.GetDataAsync<FunctionModel>(Arg.Any<GetAllFunctionCriteria>()).Returns(expected);

            // Act
            var actual = await functionService.GetTreeFunctionsAsync(true);

            // Assert
            Assert.Single(actual);
            Assert.Single(actual.First().Childs);
            Assert.Equal(expected.First(), actual.First());
            Assert.Equal(expected[1], actual.First().Childs.First());
        }

        [Fact]
        public async Task AssignFunctionToRoleAsync_WhenCalled_ReturnAsExpectedCommandResult()
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
            var functionService = GetFunctionService();
            baseService.ExecuteNonQueryAsync(Arg.Any<AssignFunctionToRoleCriteria>()).Returns(expected);

            // Act
            var actual = await functionService.AssignFunctionToRoleAsync(roleId, permissions);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private List<FunctionModel> GetFunctionForTest()
        {
            return new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1, FunctionKey = "test", Name = "test" },
                new FunctionModel { FunctionId = 2, ParentId = 1, FunctionKey = "test", Name = "test" }
            };
        }

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private FunctionService GetFunctionService()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            var service = new FunctionService(baseService, sessionService);

            return service;
        }

        #endregion Helpers
    }
}