namespace NexGoal.Repositories.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using NexGoal.Models.Common;
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
        public async Task GetMemberNominationAsync_WhenCalled_ReturnAsExpectedRecords()
        {
            // Arrange
            var pafcuses = new List<PerformanceRecordModel>
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
                criteria).Returns(pafcuses);

            // Act
            var actual = await repository.GetMemberNominationAsync(criteria);

            // Assert
            Assert.Equal(pafcuses, actual);
        }
    }
}