namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EOQ;
    using NexGoal.Models.EOQY;
    using NSubstitute;
    using Xunit;

    public class EOQServiceTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;

        [Fact]
        public async Task GetAllEOQAsync_WhenCalled_ReturnAsExpectedEOQs()
        {
            // Arrange
            var expected = new List<EOQYModel>
            {
                new EOQYModel { EOQYId = 1, Year=2018, Quarter=1 }
            };
            var service = GetEOQService();
            baseService.GetDataAsync<EOQYModel>(Arg.Any<GetEOQCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetAllEOQAsync(2018, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEOQByIdAsync_WhenCalled_ReturnAsExpectedEOQ()
        {
            // Arrange
            var expected = new EOQYModel { EOQYId = 1, Year = 2018, Quarter = 1 };
            var service = GetEOQService();
            baseService.GetDataEntityAsync<EOQYModel>(Arg.Any<GetEOQByIdCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetEOQByIdAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetYearsAsync_WhenCalled_ReturnAsExpectedYears()
        {
            // Arrange
            var expected = new List<int> { 2017, 2018 };
            var service = GetEOQService();
            baseService.GetDataAsync<int>(Arg.Any<GetYearsCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetYearsAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateEOQAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var eoq = new CreateEOQModel
            {
                Year = 2018,
                Quarter = 1
            };
            var service = GetEOQService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEOQCriteria>()).Returns(expected);

            // Act
            var actual = await service.CreateEOQAsync(eoq);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditEOQAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var eoq = new EOQYModel
            {
                Year = 2018,
                Quarter = 1
            };
            var service = GetEOQService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditEOQCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditEOQAsync(eoq);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task DeleteEOQAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetEOQService();
            baseService.ExecuteNonQueryAsync(Arg.Any<DeleteEOQCriteria>()).Returns(expected);

            // Act
            var actual = await service.DeleteEOQAsync(1);

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

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private EOQService GetEOQService()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            var service = new EOQService(baseService, sessionService);

            return service;
        }

        #endregion Helpers
    }
}