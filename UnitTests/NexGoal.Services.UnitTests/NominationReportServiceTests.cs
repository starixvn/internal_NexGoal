namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
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
        public async Task GetNominationPeriodsAsync_WhenCalled_ReturnExpectedResult()
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

            var result = await service.GetNominationPeriodsAsync();

            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetNominationsAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = GetService();
            baseService.FetchMultipleAsync<NominationModel, PerformanceRecordModel>(Arg.Any<GetNominationsCriteria>())
                .Returns(new Tuple<IEnumerable<NominationModel>, IEnumerable<PerformanceRecordModel>>
                (
                   new List<NominationModel>
                   {
                       new NominationModel
                       {
                           NominationId = 1,
                           NominationPeriodId = 1,
                           NomineeId = 2
                       }
                   },
                   new List<PerformanceRecordModel>
                   {
                       new PerformanceRecordModel
                       {
                           EmployeeIds = "2",
                           CreatorId = 1
                       }
                   }
                ));
            commonService.GetEmployeesAsync(isEnabled: null).Returns(new List<Employee>
            {
                new Employee
                {
                    EmployeeId = 1
                },
                new Employee
                {
                    EmployeeId = 2
                }
            });

            var result = await service.GetNominationsAsync("1", null, null, null, 1);

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