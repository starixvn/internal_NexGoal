namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Discipline;
    using NexGoal.Models.Discipline;
    using NSubstitute;
    using Xunit;

    public class DisciplineServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task GetDisciplines_WhenCalled_ReturnExpectedResult()
        {
            var expectedDisciplines = new List<DisciplineModel>
            {
                new DisciplineModel
                {
                    DisciplineId = 1
                }
            };

            var service = GetDisciplineService();
            baseService.GetDataAsync<DisciplineModel>(Arg.Any<GetDisciplinesCriteria>()).Returns(expectedDisciplines);

            var result = await service.GetDisciplines(DateTime.Today, DateTime.Today.AddMonths(1), null, string.Empty, null);

            Assert.Equal(expectedDisciplines.Count, result.Count());
        }

        [Fact]
        public async Task EditStatus_ValidData_ReturnExpectedResult()
        {
            var service = GetDisciplineService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateDisciplineStatusCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.EditStatus(1, true, 1);

            Assert.True(result.IsSuccessful);
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private DisciplineService GetDisciplineService()
        {
            baseService = FakeBaseService();
            var service = new DisciplineService(baseService);

            return service;
        }
    }
}