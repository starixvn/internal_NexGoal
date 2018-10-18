namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.FlowSetting;
    using NexGoal.Models.Common;
    using NexGoal.Models.FlowSetting;
    using NSubstitute;
    using Xunit;

    public class GroupFlowSettingServiceTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;
        private IAvatarService avatarService;

        [Fact]
        public async Task GetGroupHaveSettingAsync_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var expected = new List<GroupBaseModel>
            {
                new GroupBaseModel { GroupId = 1, Code = "NEX" }
            };
            var service = GetGroupFlowSettingService();
            baseService.GetDataAsync<GroupBaseModel>(Arg.Any<GetGroupHaveSettingCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetGroupHaveSettingAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetGroupFlowSettingAsync_WhenCalled_ReturnAsExectedFlowSettings()
        {
            // Arrange
            var groupFlowSettings = new List<GroupFlowSettingModel>
            {
                new GroupFlowSettingModel { GroupId = 1, Code = "NEX" }
            };
            var flowSettings = new List<FlowSettingModel>
            {
                new FlowSettingModel { ProcessId = 1, GroupId = 1, EvaluatorId = 1, NickName = "Maria" },
                new FlowSettingModel { ProcessId = 4, GroupId = 1, EvaluatorId = 2, NickName = "Quinn" },
                new FlowSettingModel { ProcessId = 5, GroupId = 1, EvaluatorId = 3, NickName = "Anders" }
            };
            var tuple = new Tuple<IEnumerable<GroupFlowSettingModel>, IEnumerable<FlowSettingModel>>(groupFlowSettings, flowSettings);
            var service = GetGroupFlowSettingService();
            baseService.FetchMultipleAsync<GroupFlowSettingModel, FlowSettingModel>(Arg.Any<GetGroupFlowSettingCriteria>()).Returns(tuple);

            // Act
            var actual = await service.GetGroupFlowSettingAsync(1);

            // Assert
            Assert.Single(actual);
            Assert.Equal(1, actual.First().GroupId);
            Assert.Equal("NEX", actual.First().Code);

            Assert.Single(actual.First().HumanResources);
            Assert.Equal(flowSettings[0], actual.First().HumanResources.First());
            Assert.Equal(flowSettings[1], actual.First().Evaluator);
            Assert.Equal(flowSettings[2], actual.First().Reviewer);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalled_ReturnAsExpectedFlowTemplate()
        {
            // Arrange
            var flowSettings = new List<FlowSettingModel>
            {
                new FlowSettingModel { ProcessId = 1, EvaluatorId = 1, NickName = "Maria" },
                new FlowSettingModel { ProcessId = 4, EvaluatorId = 2, NickName = "Quinn" },
                new FlowSettingModel { ProcessId = 5, EvaluatorId = 3, NickName = "Anders" }
            };
            var service = GetGroupFlowSettingService();
            baseService.GetDataAsync<FlowSettingModel>(Arg.Any<GetGroupFlowSettingByIdCriteria>()).Returns(flowSettings);

            // Act
            var actual = await service.GetByIdAsync(1, 1);

            // Assert
            Assert.Equal(1, actual.GroupId);
            Assert.Single(actual.HumanResources);
            Assert.NotNull(actual.Evaluator);
            Assert.NotNull(actual.Reviewer);
        }

        [Fact]
        public async Task EditAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var settings = new List<FlowSettingModel>
            {
                new FlowSettingModel { ProcessId = 1, EvaluatorId = 1 }
            };
            var service = GetGroupFlowSettingService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditGroupFlowSettingCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditAsync(1, 1, settings);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task DeleteAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetGroupFlowSettingService();
            baseService.ExecuteNonQueryAsync(Arg.Any<DeleteGroupFlowSettingCriteria>()).Returns(expected);

            // Act
            var actual = await service.DeleteAsync(1, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private GroupFlowSettingService GetGroupFlowSettingService()
        {
            sessionService = FakeSessionService();
            baseService = Substitute.For<IBaseService>();
            avatarService = Substitute.For<IAvatarService>();
            var service = new GroupFlowSettingService(sessionService, baseService, avatarService);

            return service;
        }

        #endregion Helpers
    }
}