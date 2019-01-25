namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Discipline;
    using NexGoal.Models.Discipline;
    using NexGoal.Repositories;
    using NSubstitute;
    using Xunit;

    public class DisciplineServiceTests
    {
        private IBaseService baseService;
        private IDisciplineRepository disciplineRepository;

        [Fact]
        public async Task GetDisciplineAsync_WhenCalled_ReturnAsExpectedDisciplines()
        {
            // Arrange
            var param = new GetDisciplineParam();
            var expected = new List<DisciplineModel>
            {
                new DisciplineModel { DisciplineId = 1, Description = "test" }
            };
            var criteria = new GetDisciplineCriteria();
            var service = GetDisciplineService();
            disciplineRepository.GetDisciplineAsync(Arg.Do<GetDisciplineCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await service.GetDisciplineAsync(param);

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(param.FromDate, criteria.FromDate);
            Assert.Equal(param.ToDate, criteria.ToDate);
            Assert.Equal(param.GroupId, criteria.GroupId);
            Assert.Equal(param.EmployeeId, criteria.EmployeeId);
            Assert.Equal(param.IsActive, criteria.IsActive);
        }

        [Fact]
        public async Task GetDisciplineByEmployeeIdAsync_WhenCalled_ReturnAsExpectedDisciplines()
        {
            // Arrange
            var expected = new List<DisciplineModel>
            {
                new DisciplineModel { DisciplineId = 1 }
            };
            var criteria = new GetDisciplineByEmployeeIdCriteria();
            var service = GetDisciplineService();
            baseService.GetDataAsync<DisciplineModel>(Arg.Do<GetDisciplineByEmployeeIdCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await service.GetDisciplineByEmployeeIdAsync(363);

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(363, criteria.EmployeeId);
            Assert.Null(criteria.PeriodId);
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
            disciplineRepository = Substitute.For<IDisciplineRepository>();
            var service = new DisciplineService(baseService, disciplineRepository);

            return service;
        }
    }
}