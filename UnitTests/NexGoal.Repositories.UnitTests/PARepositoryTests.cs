namespace NexGoal.Repositories.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NSubstitute;
    using Xunit;

    public class PARepositoryTests
    {
        private readonly IObjectDb objectDb;

        public PARepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task GetPerformanceAppraisalResultAsync_WhenCalled_ReturnAsExpectedPerformanceAppraisals()
        {
            // Arrange
            var records = new List<PerformanceAppraisalResult>
            {
                new PerformanceAppraisalResult
                {
                    PARecordId = 1,
                    Period = new PeriodBaseModel { PeriodId = 1, Name = "2h 2018" }
                }
            };
            var criteria = FakeCriteria();
            var repository = new PARepository();
            objectDb.QueryAsync(Arg.Any<Func<PerformanceAppraisalResult, PeriodBaseModel, PerformanceAppraisalResult>>(),
                "PeriodId",
                criteria).Returns(records);

            // Act
            var actual = await repository.GetPerformanceAppraisalResultAsync(criteria);

            // Assert
            Assert.Equal(records, actual);
        }

        [Fact]
        public async Task GetMemberPerformanceAppraisalResultAsync_WhenCalled_ReturnAsExpectedPerformanceAppraisals()
        {
            // Arrange
            var records = new List<PerformanceAppraisalResult>
            {
                new PerformanceAppraisalResult
                {
                    PARecordId = 1,
                    Employee = new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" },
                    Period = new PeriodBaseModel { PeriodId = 1, Name = "2h 2018" }
                }
            };
            var criteria = FakeCriteria();
            var repository = new PARepository();
            objectDb.QueryAsync(Arg.Any<Func<PerformanceAppraisalResult, EmployeeBaseModel, PeriodBaseModel, PerformanceAppraisalResult>>(),
                "EmployeeId, PeriodId",
                criteria).Returns(records);

            // Act
            var actual = await repository.GetMemberPerformanceAppraisalResultAsync(criteria);

            // Assert
            Assert.Equal(records, actual);
        }

        #region Helpers

        private static ICriteria FakeCriteria()
        {
            var criteria = Substitute.For<ICriteria>();
            criteria.GetSettingKey().Returns("NexGoal_Test");

            return criteria;
        }

        #endregion Helpers
    }
}