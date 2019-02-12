namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PAFocus;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;
    using NexGoal.Repositories;
    using NSubstitute;
    using Xunit;

    public class PAFocusServiceTests
    {
        private IBaseService baseService;
        private ICryptoService cryptoService;
        private ICommonService commonService;
        private IPAFocusRepository focusRepository;

        [Fact]
        public async Task GetPeriodAsync_WhenCalled_ReturnAsExpectedPeriods()
        {
            // Arrange
            var expected = new List<PeriodBaseModel>
            {
                new PeriodBaseModel { PeriodId = 1, Name = "1H 2018" }
            };
            var service = GetPAFocusService();
            baseService.GetDataAsync<PeriodBaseModel>(Arg.Any<GetPeriodCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetPeriodAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetGroupAsync_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var expected = new List<Group>
            {
                new Group { GroupId = 1, Code = "NEX", Name = "Nexus" }
            };
            var service = GetPAFocusService();
            baseService.GetDataAsync<Group>(Arg.Any<GetGroupCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetGroupAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMemberAsync_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var expected = new List<Employee>
            {
                new Employee { EmployeeId = 1, Code = "NEX", NickName = "Nexus" }
            };
            var service = GetPAFocusService();
            baseService.GetDataAsync<Employee>(Arg.Any<GetMemberCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetMemberAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetPAFocusAsync_WhenCalled_ReturnAsExpectedPAFocuses()
        {
            // Arrange
            var plainText = "1";
            var focuses = new List<PAFocusModel>
            {
                new PAFocusModel { PAFocusId = 1, Employee = new Employee { EmployeeId = 1 } }
            };
            var performanceGrades = new List<PerformanceGradeModel>
            {
                new PerformanceGradeModel { GradeId = 1, Name = "E+" }
            };
            var service = GetPAFocusService();
            focusRepository.GetPAFocusAsync(Arg.Any<GetPAFocusCriteria>()).Returns(focuses);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(plainText);

            // Act
            var actual = await service.GetPAFocusAsync(new GetPAFocusParam());

            // Assert
            Assert.Single(actual);
            Assert.Equal(performanceGrades[0], actual.First().Grade);
            Assert.Equal(plainText, actual.First().IsReadyPromotion);
            Assert.Equal(plainText, actual.First().Contribution);
            Assert.Equal(plainText, actual.First().BonusGiven);
        }

        [Fact]
        public async Task EditPAFocusAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var paFocuses = new List<EditPAFocusModel>
            {
                new EditPAFocusModel { PAFocusId  = 1, BonusGiven = "1", Contribution = "1" }
            };
            var service = GetPAFocusService();
            cryptoService.Encrypt(Arg.Any<string>()).Returns("encrypted");
            baseService.ExecuteNonQueryAsync(Arg.Any<EditPAFocusCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditPAFocusAsync(1, paFocuses);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private PAFocusService GetPAFocusService()
        {
            baseService = Substitute.For<IBaseService>();
            cryptoService = Substitute.For<ICryptoService>();
            commonService = Substitute.For<ICommonService>();
            focusRepository = Substitute.For<IPAFocusRepository>();
            var service = new PAFocusService(baseService, cryptoService, commonService, focusRepository);

            return service;
        }

        #endregion Helpers
    }
}