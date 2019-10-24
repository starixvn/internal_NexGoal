namespace NexGoal.AutoService.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Criterias;
    using NexGoal.AutoService.Criterias.Probation;
    using NexGoal.AutoService.Models.PA;
    using NexGoal.AutoService.Services;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NSubstitute;
    using Xunit;

    public class ProbationServiceTests
    {
        private IOptionsSnapshot<AppSettings> configuration;
        private IBaseService baseService;
        private IBaseMailService mailService;
        private IChatbotService chatbotService;

        #region SendMail

        [Fact]
        public async Task SendMail_Remind_ExpectedSendMail()
        {
            // Arrange
            var endDate = new DateTime(2018, 12, 20);
            var receiverEmail = "tim.trinh@nexcel.vn";
            var settings = new List<ProbationProcessSettingModel>
            {
                new ProbationProcessSettingModel { ProcessId = PerformanceProcess.HumanResource, EvaluatorId = 204, Nickname = "albert", Email = receiverEmail, EndDate = endDate },
                new ProbationProcessSettingModel { ProcessId = PerformanceProcess.Self, EvaluatorId = 285, Nickname = "tim", Email = receiverEmail, EndDate = endDate },
                new ProbationProcessSettingModel { ProcessId = PerformanceProcess.Evaluator, EvaluatorId = 125, Nickname = "quinn", Email = receiverEmail, EndDate = endDate }
            };
            var mailModel = new MailModel();
            var mailSubject = "The Probation evaluation process is almost over!";
            var mailMody = "The Probation process is going to be closed.";
            var service = GetProbationService();
            baseService.GetDataAsync<ProbationProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(settings);
            mailService.ReadTextFile(Arg.Any<string>()).Returns(mailMody);
            await mailService.SendMailAsync(Arg.Do<MailModel>(x => mailModel = x));
            mailService.FormatDate(endDate).Returns("20 Dec 2018");

            // Act
            await service.SendMailAsync();

            // Assert
            await mailService.Received(3).SendMailAsync(Arg.Any<MailModel>());
            Assert.Equal(mailSubject, mailModel.Subject);
            Assert.Equal(mailModel.Body, mailMody);
            Assert.Equal(mailModel.ReceiverEmail, receiverEmail);
        }

        #endregion SendMail

        #region SendNotification

        [Fact]
        public async Task SendNotification_DoNotHaveMemberInProbation_NotSendNotification()
        {
            // Arrange
            var settings = new List<ProbationProcessSettingModel>();
            var service = GetProbationService();
            baseService.GetDataAsync<ProbationProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(settings);

            // Act
            await service.SendNotificationAsync();

            // Assert
            await baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<CreateNotificationCriteria>());
        }

        [Fact]
        public async Task SendNotification_Remind_ExpectedSendNotification()
        {
            // Arrange
            var endDate = new DateTime(2018, 12, 20);
            var settings = new List<ProbationProcessSettingModel>
            {
                new ProbationProcessSettingModel { ProcessId = PerformanceProcess.HumanResource, EvaluatorId = 204, Nickname = "albert", EndDate = endDate, ProbationerNickname="tim" },
                new ProbationProcessSettingModel { ProcessId = PerformanceProcess.Self, EvaluatorId = 285, Nickname = "tim", EndDate = endDate, ProbationerNickname="tim" },
                new ProbationProcessSettingModel { ProcessId = PerformanceProcess.Evaluator, EvaluatorId = 125, Nickname = "quinn", EndDate = endDate, ProbationerNickname="tim" }
            };

            var criteria = new CreateNotificationCriteria();

            var service = GetProbationService();
            baseService.GetDataAsync<ProbationProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(settings);
            await baseService.ExecuteNonQueryAsync(Arg.Do<CreateNotificationCriteria>(x => criteria = x));
            mailService.FormatDate(endDate).Returns("20 Dec 2018");

            // Act
            await service.SendNotificationAsync();

            // Assert
            await baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<CreateNotificationCriteria>());
        }

        #endregion SendNotification

        #region Helpers

        private ProbationService GetProbationService()
        {
            IHostingEnvironment hostingEnvironment = Helpers.FakeHostingEnvironment();
            configuration = Helpers.FakeOptionsSnapshot();
            baseService = Helpers.FakeBaseService();
            mailService = Helpers.FakeBaseMailService();
            chatbotService = Helpers.FakeChatbotService();

            return new ProbationService(hostingEnvironment, configuration, baseService, mailService, chatbotService);
        }

        #endregion Helpers
    }
}