namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.NominationReport;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Repositories;
    using NSubstitute;
    using Xunit;

    public class NominationReportServiceTests
    {
        private IBaseService baseService;
        private IEmployeeNominationRepository repository;

        [Fact]
        public async Task GetNominationPeriodAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var nominationPeriods = new List<NominationPeriodModel>
            {
                new NominationPeriodModel
                {
                    NominationPeriodId = 1,
                    Year = 2019
                }
            };
            var service = GetService();
            baseService.GetDataAsync<NominationPeriodModel>(Arg.Any<GetNominationPeriodsCriteria>()).Returns(nominationPeriods);

            // Act
            var actual = await service.GetNominationPeriodAsync();

            // Assert
            Assert.Equal(nominationPeriods, actual);
        }

        [Fact]
        public async Task GetNominationAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var nominations = new List<NominationModel>
            {
                new NominationModel
                {
                    NominationId = 1,
                    Period = new NominationPeriodModel { NominationPeriodId = 1, Year = 2019 },
                    Nominee = new Employee { EmployeeId = 1, NickName = "tim" }
                }
            };
            var service = GetService();
            repository.GetNominationByPeriodIdsAsync(Arg.Any<GetNominationsCriteria>()).Returns(nominations);

            // Act
            var actual = await service.GetNominationAsync(1, "1", null, null, null);

            // Assert
            Assert.Equal(nominations, actual);
        }

        private NominationReportService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            repository = Substitute.For<IEmployeeNominationRepository>();

            var service = new NominationReportService(baseService, repository);

            return service;
        }
    }
}