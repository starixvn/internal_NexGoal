namespace NexGoal.Repositories.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;
    using NSubstitute;
    using Xunit;

    public class NominationReportRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public NominationReportRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task GetNominationAsync_WhenCalled_ReturnAsExpectedNominations()
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
            var records = new List<PerformanceRecord>
            {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    Employees = new List<Employee> { new Employee { EmployeeId = 1, NickName = "tim" } },
                    Creator = new Employee { EmployeeId = 2, NickName = "quinn" },
                    NominationPeriod = new NominationPeriodModel { NominationPeriodId = 1, Year = 2018 }
                }
            };
            var multiResultReader = Helpers.FakeMultipleResultReader();
            var criteria = Helpers.FakeCriteria();
            var repository = new NominationReportRepository();
            multiResultReader.Read(
                Arg.Any<Func<NominationModel, Employee, NominationPeriodModel, NominationModel>>(),
                "EmployeeId, NominationPeriodId").Returns(nominations);
            multiResultReader.Read(
                Arg.Any<Func<PerformanceRecord, Employee, Employee, NominationPeriodModel, PerformanceRecord>>(),
                "EmployeeId, EmployeeId, NominationPeriodId").Returns(records);
            objectDb.QueryMultipleAsync(criteria).Returns(multiResultReader);

            // Act
            var actual = await repository.GetNominationAsync(criteria);

            // Assert
            Assert.Equal(nominations, actual);
            Assert.Single(actual.First().Nominators);
            Assert.Equal(records.First(), actual.First().Nominators.First());
        }
    }
}