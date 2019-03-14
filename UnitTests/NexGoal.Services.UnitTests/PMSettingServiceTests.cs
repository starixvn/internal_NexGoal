namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PMSetting;
    using NexGoal.Models.Common;
    using NexGoal.Models.PMSetting;
    using NSubstitute;
    using Xunit;

    public class PMSettingServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task GetSettingAsync_ValidData_ReturnExpectedResult()
        {
            // Arrange
            var members = new List<MemberModel>
            {
                new MemberModel
                {
                    EmployeeId = 1,
                    ProjectManagerId = 2
                }
            };
            var criteria = new GetPMSettingCriteria();
            var service = GetPMSettingService();
            baseService.GetDataAsync<MemberModel>(Arg.Do<GetPMSettingCriteria>(x => criteria = x)).Returns(members);

            // Act
            var actual = await service.GetSettingAsync("2", null);

            // Assert
            Assert.Equal(members, actual);
            Assert.Equal("2", criteria.ProjectManagerIds);
            Assert.Null(criteria.EmployeeId);
        }

        [Fact]
        public async Task AddMemberAsync_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var commandResult = new CommandResult();
            var criteria = new AddMemberCriteria();
            var service = GetPMSettingService();
            baseService.ExecuteNonQueryAsync(Arg.Do<AddMemberCriteria>(x => criteria = x)).Returns(commandResult);

            // Act
            var actual = await service.AddMemberAsync(1, "2,3,4", 5);

            // Assert
            Assert.Equal(commandResult, actual);
            Assert.Equal(1, criteria.ProjectManagerId);
            Assert.Equal("2,3,4", criteria.MemberIds);
            Assert.Equal(5, criteria.ActorId);
        }

        [Fact]
        public async Task RemoveMemberAsync_ValidData_ReturnExpectedCommandResult()
        {
            // Arrange
            var commandResult = new CommandResult();
            var criteria = new RemoveMemberCriteria();
            var service = GetPMSettingService();
            baseService.ExecuteNonQueryAsync(Arg.Do<RemoveMemberCriteria>(x => criteria = x)).Returns(commandResult);

            // Act
            var actual = await service.RemoveMemberAsync(1, 2, 5);

            // Assert
            Assert.Equal(commandResult, actual);
            Assert.Equal(1, criteria.ProjectManagerId);
            Assert.Equal(2, criteria.MemberId);
            Assert.Equal(5, criteria.ActorId);
        }

        [Fact]
        public async Task GetProjectManagerAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var projectManagers = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = 1
                }
            };
            var service = GetPMSettingService();
            baseService.GetDataAsync<Employee>(Arg.Any<GetProjectManagersCriteria>()).Returns(projectManagers);

            // Act
            var actual = await service.GetProjectManagerAsync();

            // Assert
            Assert.Equal(projectManagers, actual);
        }

        private PMSettingService GetPMSettingService()
        {
            baseService = Substitute.For<IBaseService>();
            var service = new PMSettingService(baseService);

            return service;
        }
    }
}