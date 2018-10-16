namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PAFocus;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;
    using NSubstitute;
    using Xunit;

    public class PAFocusServiceTests
    {
        private IBaseService baseService;
        private ICryptoService cryptoService;
        private ICommonService commonService;

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
            var expected = new List<GroupBaseModel>
            {
                new GroupBaseModel { GroupId = 1, Code = "NEX", Name = "Nexus" }
            };
            var service = GetPAFocusService();
            baseService.GetDataAsync<GroupBaseModel>(Arg.Any<GetGroupCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetGroupAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMemberAsync_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, Code = "NEX", NickName = "Nexus" }
            };
            var service = GetPAFocusService();
            baseService.GetDataAsync<EmployeeBaseModel>(Arg.Any<GetMemberCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetMemberAsync(1);

            // Assert
            Assert.Equal(expected, actual);
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
            var service = new PAFocusService(baseService, cryptoService, commonService);

            return service;
        }

        #endregion Helpers
    }
}