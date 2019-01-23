namespace NexGoal.AutoService.UnitTests.Services
{
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Criterias.PA;
    using NexGoal.AutoService.Services;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NSubstitute;
    using Xunit;

    public class PAServiceTests
    {
        private IOptionsSnapshot<AppSettings> configuration;
        private IBaseService baseService;
        private IBaseMailService mailService;
        private ICryptoService cryptoService;

        [Fact]
        public void EditWhenEvaluationFinished_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetPAService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditWhenEvaluationFinishedCriteria>()).Returns(expected);

            // Act
            var actual = service.EditWhenEvaluationFinished();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EditStatus_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetPAService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditStatusCriteria>()).Returns(expected);

            // Act
            var actual = service.EditStatus();

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private PAService GetPAService()
        {
            configuration = Helpers.FakeOptionsSnapshot();
            baseService = Helpers.FakeBaseService();
            mailService = Helpers.FakeBaseMailService();
            cryptoService = Helpers.FakeCryptoService();

            return new PAService(configuration, baseService, mailService, cryptoService);
        }

        #endregion Helpers
    }
}