namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EmployeeNomination;
    using NexGoal.Criteria.NominationReport;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;
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
            Assert.Equal(1, result.Count());
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