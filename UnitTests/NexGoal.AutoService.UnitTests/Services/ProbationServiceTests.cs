namespace NexGoal.AutoService.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
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
        public void SendMail_Remind_ExpectedSendMail()
        {
            // Arrange
            var endDate = new DateTime(2018, 12, 20);
            var receiverEmail = "tim.trinh@nexcel.vn";
            var settings = new List<ProcessSettingModel>
            {
                new ProcessSettingModel { ProcessId = PerformanceProcess.HumanResource, EvaluatorId = 204, Nickname = "albert", Email = receiverEmail, EndDate = endDate },
                new ProcessSettingModel { ProcessId = PerformanceProcess.Self, EvaluatorId = 285, Nickname = "tim", Email = receiverEmail, EndDate = endDate },
                new ProcessSettingModel { ProcessId = PerformanceProcess.Evaluator, EvaluatorId = 125, Nickname = "quinn", Email = receiverEmail, EndDate = endDate }
            };
            var mailModel = new MailModel();
            var mailSubject = "The Probation process is almost over!";
            var mailMody = "The Probation process is going to be closed.";
            var service = GetProbationService();
            baseService.GetDataAsync<ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(settings);
            mailService.ReadTextFile(Arg.Any<string>()).Returns(mailMody);
            mailService.SendMailAsync(Arg.Do<MailModel>(x => mailModel = x));
            mailService.FormatEndDate(endDate).Returns("20 Dec 2018");

            // Act
            service.SendMail();

            // Assert
            mailService.Received(3).SendMailAsync(Arg.Any<MailModel>());
            Assert.Equal(mailModel.Subject, mailSubject);
            Assert.Equal(mailModel.Body, mailMody);
            Assert.Equal(mailModel.ReceiverEmail, receiverEmail);
        }

        #endregion SendMail

        #region SendNotification

        [Fact]
        public void SendNotification_DoNotHaveMemberInProbation_NotSendNotification()
        {
            // Arrange
            var settings = new List<ProcessSettingModel>();
            var service = GetProbationService();
            baseService.GetDataAsync<ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(settings);

            // Act
            service.SendNotification();

            // Assert
            baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<CreateNotificationCriteria>());
        }

        [Fact]
        public void SendNotification_Remind_ExpectedSendNotification()
        {
            // Arrange
            var endDate = new DateTime(2018, 12, 20);
            var settings = new List<ProcessSettingModel>
            {
                new ProcessSettingModel { ProcessId = PerformanceProcess.HumanResource, EvaluatorId = 204, Nickname = "albert", EndDate = endDate },
                new ProcessSettingModel { ProcessId = PerformanceProcess.Self, EvaluatorId = 285, Nickname = "tim", EndDate = endDate },
                new ProcessSettingModel { ProcessId = PerformanceProcess.Evaluator, EvaluatorId = 125, Nickname = "quinn", EndDate = endDate }
            };
            var notificationXml = "<Row EmployeeId=\"285\"" +
                " Content=\"The Probation process is going to be CLOSED on 20 Dec 2018, please create a Performance Record (with a detailed comment, selected grade, and a &quot;Probation&quot; tag) for yourself.\"" +
                " StatusId=\"0\" ActionTypeId=\"1\" RecordId=\"\" />";
            var notification1Xml = "<Row EmployeeId=\"125\"" +
                " Content=\"The Probation process is going to be CLOSED on 20 Dec 2018, please create a Performance Record (with a detailed comment, selected grade, and a &quot;Probation&quot; tag) for your member(s).\"" +
                " StatusId=\"0\" ActionTypeId=\"3\" RecordId=\"\" />";
            var notification2Xml = "The Probation process is going to be CLOSED on 20 Dec 2018, please follow up on the progress and make a reminder for related people (if needed).";

            var criteria = new CreateNotificationCriteria();

            var service = GetProbationService();
            baseService.GetDataAsync<ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(settings);
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateNotificationCriteria>(x => criteria = x));
            mailService.FormatEndDate(endDate).Returns("20 Dec 2018");

            // Act
            service.SendNotification();

            // Assert
            baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<CreateNotificationCriteria>());
            Assert.Contains(notificationXml, criteria.Notifications);
            Assert.Contains(notification1Xml, criteria.Notifications);
            Assert.Contains(notification2Xml, criteria.Notifications);
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