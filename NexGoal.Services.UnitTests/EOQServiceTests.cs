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
        public async Task GetAllAsync_WhenCalled_ReturnAsExpectedEOQs()
        {
            // Arrange
            var expected = new List<EOQYModel>
            {
                new EOQYModel { EOQYId = 1, Year = 2018, Quarter = 1 }
            };
            var service = GetEOQService();
            baseService.GetDataAsync<EOQYModel>(Arg.Any<GetEOQCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetAllAsync(2018, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalled_ReturnAsExpectedEOQ()
        {
            // Arrange
            var expected = new EOQYModel { EOQYId = 1, Year = 2018, Quarter = 1 };
            var service = GetEOQService();
            baseService.GetDataEntityAsync<EOQYModel>(Arg.Any<GetEOQByIdCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetByIdAsync(1);

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
        public async Task CreateAsync_WhenCalled_ReturnAsExpectedCommandResult()
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
            var actual = await service.CreateAsync(eoq);

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
            var eoq = new EditEOQModel
            {
                EOQId = 1,
                Year = 2018,
                Quarter = 1
            };
            var service = GetEOQService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditEOQCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditAsync(eoq);

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

        private EOQService GetEOQService()
        {
            sessionService = FakeSessionService();
            baseService = Substitute.For<IBaseService>();
            var service = new EOQService(baseService, sessionService);

            return service;
        }

        #endregion Helpers
    }
}