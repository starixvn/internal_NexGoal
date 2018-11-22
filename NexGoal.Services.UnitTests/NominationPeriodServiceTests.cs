namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.NominationPeriod;
    using NexGoal.Models.NominationPeriod;
    using NSubstitute;
    using Xunit;

    public class NominationPeriodServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task GetAsync_WhenCalled_ReturnAsExpectedNominationPeriods()
        {
            // Arrange
            var expected = new List<NominationPeriodModel>
            {
                new NominationPeriodModel { NominationPeriodId = 1, Year = 2018, Quarter = 1 }
            };
            var service = GetNominationPeriodService();
            baseService.GetDataAsync<NominationPeriodModel>(Arg.Any<GetNominationPeriodCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private NominationPeriodService GetNominationPeriodService()
        {
            baseService = Substitute.For<IBaseService>();
            var service = new NominationPeriodService(baseService);

            return service;
        }

        #endregion Helpers
    }
}