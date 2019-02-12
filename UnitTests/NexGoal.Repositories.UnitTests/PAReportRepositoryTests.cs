namespace NexGoal.Repositories.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;
    using NSubstitute;
    using Xunit;

    public class PAReportRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public PAReportRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task GetPAReportAsync_WhenCalled_ReturnAsExpectedPerformanceAppraisals()
        {
            // Arrange
            var performanceAppraisals = new List<PARecordModel>
            {
                new PARecordModel
                {
                    PARecordId = 1,
                    Employee = new Employee { EmployeeId = 1, NickName = "tim" },
                    Period = new PeriodBaseModel { PeriodId = 1, Name = "2h 2018" }
                },
                new PARecordModel
                {
                    PARecordId = 2,
                    Employee = new Employee { EmployeeId = 2, NickName = "quinn" },
                    Period = new PeriodBaseModel { PeriodId = 1, Name = "2h 2018" }
                }
            };
            var processes = new List<PAProcessModel>
            {
                new PAProcessModel { PARecordId = 1, ProcessId = (int)PerformanceProcess.Self, Evaluator = new Employee { EmployeeId = 1, NickName = "tim" } },
                new PAProcessModel { PARecordId = 1, ProcessId = (int)PerformanceProcess.Evaluator, Evaluator = new Employee { EmployeeId = 2, NickName = "quinn" } },
                new PAProcessModel { PARecordId = 1, ProcessId = (int)PerformanceProcess.Reviewer, Evaluator = new Employee { EmployeeId = 3, NickName = "anders" } },
                new PAProcessModel { PARecordId = 2, ProcessId = (int)PerformanceProcess.Self, Evaluator = new Employee { EmployeeId = 2, NickName = "quinn" } },
                new PAProcessModel { PARecordId = 2, ProcessId = (int)PerformanceProcess.Evaluator, Evaluator = new Employee { EmployeeId = 3, NickName = "anders" } },
                new PAProcessModel { PARecordId = 2, ProcessId = (int)PerformanceProcess.Reviewer, Evaluator = new Employee { EmployeeId = 4, NickName = "phuong" } }
            };
            var multiResultReader = Helpers.FakeMultipleResultReader();
            var criteria = Helpers.FakeCriteria();
            var repository = new PAReportRepository();
            multiResultReader.Read(
                Arg.Any<Func<PARecordModel, Employee, PeriodBaseModel, PARecordModel>>(),
                "EmployeeId, PeriodId").Returns(performanceAppraisals);
            multiResultReader.Read(
                Arg.Any<Func<PAProcessModel, Employee, PerformanceRecord, PAProcessModel>>(),
                "EvaluatorId, PerformanceRecordId").Returns(processes);
            objectDb.QueryMultipleAsync(criteria).Returns(multiResultReader);

            // Act
            var actual = await repository.GetPAReportAsync(criteria);

            // Assert
            Assert.Equal(performanceAppraisals, actual);
            Assert.Equal(performanceAppraisals.First().Processes, processes.Where(x => x.PARecordId == performanceAppraisals.First().PARecordId));
            Assert.Equal(performanceAppraisals[1].Processes, processes.Where(x => x.PARecordId == performanceAppraisals[1].PARecordId));
        }
    }
}