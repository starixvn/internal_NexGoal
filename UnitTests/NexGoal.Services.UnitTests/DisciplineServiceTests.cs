namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
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
        public async Task GetDisciplineAsync_WhenCalled_ReturnAsExpectedDisciplines()
        {
            // Arrange
            var param = new GetDisciplineParam();
            var expected = new List<DisciplineModel>
            {
                new DisciplineModel
                {
                    DisciplineId = 1
                }
            };
            var service = GetDisciplineService();
            baseService.GetDataAsync<DisciplineModel>(Arg.Any<GetDisciplineCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetDisciplineAsync(param);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditStatusAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var service = GetDisciplineService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateDisciplineStatusCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditStatusAsync(1, true, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var service = GetDisciplineService();
            baseService.ExecuteNonQueryAsync(Arg.Any<AddDisciplineCriteria>()).Returns(expected);

            // Act
            var actual = await service.CreateAsync(new AddDisciplineModel());

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var service = GetDisciplineService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditDisciplineCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditAsync(new EditDisciplineModel());

            // Assert
            Assert.Equal(expected, actual);
        }

        private DisciplineService GetDisciplineService()
        {
            baseService = Substitute.For<IBaseService>();
            var service = new DisciplineService(baseService);

            return service;
        }
    }
}