namespace NexGoal.Repositories.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using NexGoal.Models.Common;
    using NexGoal.Models.Discipline;
    using NSubstitute;
    using Xunit;

    public class DisciplineRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public DisciplineRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task GetDisciplineAsync_WhenCalled_ReturnAsExpectedDisciplines()
        {
            // Arrange
            var disciplines = new List<DisciplineModel>
            {
                new DisciplineModel
                {
                    DisciplineId = 1,
                    Description = "test",
                    Employee = new EmployeeBaseModel { EmployeeId = 1, NickName = "tim" },
                    Creator =  new EmployeeBaseModel { EmployeeId = 2, NickName = "quinn" }
                }
            };
            var criteria = Helpers.FakeCriteria();
            var repository = new DisciplineRepository();
            objectDb.QueryAsync(Arg.Any<Func<DisciplineModel, EmployeeBaseModel, EmployeeBaseModel, DisciplineModel>>(),
                "EmployeeId, EmployeeId",
                criteria).Returns(disciplines);

            // Act
            var actual = await repository.GetDisciplineAsync(criteria);

            // Assert
            Assert.Equal(disciplines, actual);
        }
    }
}