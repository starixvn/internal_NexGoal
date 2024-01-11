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

    public class EmployeeNominationRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public EmployeeNominationRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task GetNominationResultAsync_WhenCalled_ReturnAsExpectedRecords()
        {
            // Arrange
            var nominations = new List<EmployeeNominationModel>
            {
                new EmployeeNominationModel { Period = new NominationPeriodModel { NominationPeriodId = 1, Quarter = 4, Year = 2018 } }
            };
            var criteria = Helpers.FakeCriteria();
            var repository = new EmployeeNominationRepository();
            objectDb.QueryAsync(Arg.Any<Func<EmployeeNominationModel, NominationPeriodModel, EmployeeNominationModel>>(),
                "NominationPeriodId",
                criteria).Returns(nominations);

            // Act
            var actual = await repository.GetNominationResultAsync(criteria);

            // Assert
            Assert.Equal(nominations, actual);
        }

        [Fact]
        public async Task GetMemberNominationAsync_WhenCalled_ReturnAsExpectedRecords()
        {
            // Arrange
            var records = new List<PerformanceRecord>
            {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    Employees = new List<Employee>{ new  Employee { EmployeeId = 1, NickName = "tim" } },
                    Creator = new Employee { EmployeeId = 2, NickName = "quinn" },
                    NominationPeriod = new NominationPeriodModel { NominationPeriodId = 1, Year = 2018 }
                }
            };
            var criteria = Helpers.FakeCriteria();
            var repository = new EmployeeNominationRepository();
            objectDb.QueryAsync(Arg.Any<Func<PerformanceRecord, Employee, Employee, NominationPeriodModel, PerformanceRecord>>(),
                "EmployeeId, EmployeeId, NominationPeriodId",
                criteria).Returns(records);

            // Act
            var actual = await repository.GetMemberNominationAsync(criteria);

            // Assert
            Assert.Equal(records, actual);
        }

        [Fact]
        public async Task GetNominationByRecordIdAsync_WhenCalled_ReturnAsExpectedRecord()
        {
            // Arrange
            var record = new PerformanceRecord
            {
                PerformanceRecordId = 1,
                Employees = new List<Employee> { new Employee { EmployeeId = 1, NickName = "tim" } },
                Creator = new Employee { EmployeeId = 2, NickName = "quinn" },
                NominationPeriod = new NominationPeriodModel { NominationPeriodId = 1, Year = 2018 }
            };
            var criteria = Helpers.FakeCriteria();
            var repository = new EmployeeNominationRepository();
            objectDb.QueryEntityAsync(Arg.Any<Func<PerformanceRecord, Employee, Employee, NominationPeriodModel, PerformanceRecord>>(),
                "EmployeeId, EmployeeId, NominationPeriodId",
                criteria).Returns(record);

            // Act
            var actual = await repository.GetNominationByRecordIdAsync(criteria);

            // Assert
            Assert.Equal(record, actual);
        }

        [Fact]
        public async Task GetNominationByPeriodIdsAsync_WhenCalled_ReturnAsExpectedNominations()
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
            var repository = new EmployeeNominationRepository();
            multiResultReader.Read(
                Arg.Any<Func<NominationModel, Employee, NominationPeriodModel, NominationModel>>(),
                "EmployeeId, NominationPeriodId").Returns(nominations);
            multiResultReader.Read(
                Arg.Any<Func<PerformanceRecord, Employee, Employee, NominationPeriodModel, PerformanceRecord>>(),
                "EmployeeId, EmployeeId, NominationPeriodId").Returns(records);
            objectDb.QueryMultipleAsync(criteria).Returns(multiResultReader);

            // Act
            var actual = await repository.GetNominationByPeriodIdsAsync(criteria);

            // Assert
            Assert.Equal(nominations, actual);
            Assert.Single(actual.First().Nominators);
            Assert.Equal(records[0], actual.First().Nominators.First());
        }
    }
}