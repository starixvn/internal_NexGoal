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
            var service = GetPMSettingService();
            baseService.GetDataAsync<MemberModel>(Arg.Any<GetPMSettingCriteria>()).Returns(new List<MemberModel>
            {
                new MemberModel
                {
                    EmployeeId = 1,
                    ProjectManagerId = 2
                }
            });

            var result = await service.GetSettingAsync("2", null);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task AddMembers_ValidData_ReturnExpectedCommandResult()
        {
            var service = GetPMSettingService();
            baseService.ExecuteNonQueryAsync(Arg.Any<AddMemberCriteria>()).Returns(
                new CommandResult
                {
                    Code = 0
                }
            );

            var result = await service.AddMembers(1, "2,3,4", 5);

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task RemoveMember_ValidData_ReturnExpectedCommandResult()
        {
            var service = GetPMSettingService();
            baseService.ExecuteNonQueryAsync(Arg.Any<RemoveMemberCriteria>()).Returns(
                new CommandResult
                {
                    Code = 0
                }
            );

            var result = await service.RemoveMember(1, 2, 5);

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task GetProjectManagerAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = GetPMSettingService();
            baseService.GetDataAsync<Employee>(Arg.Any<GetProjectManagersCriteria>()).Returns(new List<Employee>
            {
                new Employee
                {
                    EmployeeId = 1
                }
            });

            var result = await service.GetProjectManagerAsync();

            Assert.NotEmpty(result);
        }

        private PMSettingService GetPMSettingService()
        {
            baseService = Substitute.For<IBaseService>();
            var service = new PMSettingService(baseService);

            return service;
        }
    }
}