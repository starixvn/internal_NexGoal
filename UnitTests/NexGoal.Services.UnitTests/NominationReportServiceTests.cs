namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.NominationReport;
    using NexGoal.Models.NominationPeriod;
    using NSubstitute;
    using Xunit;

    public class NominationReportServiceTests
    {
        private IBaseService baseService;
        private ICommonService commonService;

        [Fact]
        public async Task GetNominationPeriodsForNominatorAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = GetService();
            baseService.GetDataAsync<NominationPeriodModel>(Arg.Any<GetNominationPeriodsCriteria>())
                .Returns(new List<NominationPeriodModel>
                {
                    new NominationPeriodModel
                    {
                        NominationPeriodId = 1,
                        Year = 2028
                    }
                });

            var result = await service.GetNominationPeriodsForNominatorAsync(false);

            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        private NominationReportService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            commonService = Substitute.For<ICommonService>();

            var service = new NominationReportService(baseService, commonService);

            return service;
        }
    }
}