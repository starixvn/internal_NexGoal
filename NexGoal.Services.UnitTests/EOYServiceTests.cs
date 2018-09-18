namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EOY;
    using NexGoal.Models.EOQY;
    using NSubstitute;
    using Xunit;

    public class EOYServiceTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;

        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnAsExpectedEOQs()
        {
            // Arrange
            var expected = new List<EOQYModel>
            {
                new EOQYModel { EOQYId = 1, Year = 2018, Quarter = 1 }
            };
            var service = GetEOYService();
            baseService.GetDataAsync<EOQYModel>(Arg.Any<GetEOYCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetAllAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalled_ReturnAsExpectedEOQ()
        {
            // Arrange
            var expected = new EOQYModel { EOQYId = 1, Year = 2018, Quarter = 1 };
            var service = GetEOYService();
            baseService.GetDataEntityAsync<EOQYModel>(Arg.Any<GetEOYByIdCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetByIdAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var eoy = new CreateEOYModel
            {
                Year = 2018
            };
            var service = GetEOYService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEOYCriteria>()).Returns(expected);

            // Act
            var actual = await service.CreateAsync(eoy);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var eoq = new EditEOYModel
            {
                EOYId = 1,
                Year = 2018
            };
            var service = GetEOYService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditEOYCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditAsync(eoq);

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
            var service = GetEOYService();
            baseService.ExecuteNonQueryAsync(Arg.Any<DeleteEOYCriteria>()).Returns(expected);

            // Act
            var actual = await service.DeleteAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetNominatorSettingAsync_WhenCalled_ReturnAsExpectedEOQs()
        {
            // Arrange
            var expected = new List<NominatorSettingModel>
            {
                new NominatorSettingModel { EmployeeId = 1, NickName = "tim.trinh", RoleIds = "1,2" }
            };
            var service = GetEOYService();
            baseService.GetDataAsync<NominatorSettingModel>(Arg.Any<GetNominatorSettingCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetNominatorSettingAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditNominatorSettingAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var setting = new EditNominatorSettingModel
            {
                EOQYId = 1,
                EmployeeId = 1,
                RoleIds = "1,2"
            };
            var service = GetEOYService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditNominatorSettingCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditNominatorSettingAsync(setting);

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

        private EOYService GetEOYService()
        {
            sessionService = FakeSessionService();
            baseService = Substitute.For<IBaseService>();
            var service = new EOYService(baseService, sessionService);

            return service;
        }

        #endregion Helpers
    }
}