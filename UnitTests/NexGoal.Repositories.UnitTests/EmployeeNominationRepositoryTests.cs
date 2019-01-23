namespace NexGoal.Repositories.UnitTests
{
    using System;
    using System.Collections.Generic;
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
            var records = new List<PerformanceRecordModel>
            {
                new PerformanceRecordModel
                {
                    PerformanceRecordId = 1,
                    Employees = new List<EmployeeBaseModel>{ new  EmployeeBaseModel { EmployeeId = 1, NickName = "tim" } },
                    Creator = new EmployeeBaseModel { EmployeeId = 2, NickName = "quinn" },
                    NominationPeriod = new NominationPeriodModel { NominationPeriodId = 1, Year = 2018 }
                }
            };
            var criteria = Helpers.FakeCriteria();
            var repository = new EmployeeNominationRepository();
            objectDb.QueryAsync(Arg.Any<Func<PerformanceRecordModel, EmployeeBaseModel, EmployeeBaseModel, NominationPeriodModel, PerformanceRecordModel>>(),
                "EmployeeId, EmployeeId, NominationPeriodId",
                criteria).Returns(records);

            // Act
            var actual = await repository.GetMemberNominationAsync(criteria);

            // Assert
            Assert.Equal(records, actual);
        }
    }
}