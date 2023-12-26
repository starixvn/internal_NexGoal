namespace NexGoal.AutoService.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.AutoService.Criterias;
    using NexGoal.AutoService.Criterias.PA;
    using NexGoal.AutoService.Models.PA;
    using NexGoal.AutoService.Models.Period;
    using NexGoal.AutoService.Services;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Resources;
    using NSubstitute;
    using Xunit;

    public class PAServiceTests
    {
        private AppSettings appSettings;
        private IBaseService baseService;
        private IBaseMailService mailService;
        private ICryptoService cryptoService;
        private IChatbotService chatbotService;

        [Fact]
        public async Task EditWhenEvaluationFinished_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetPAService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditWhenEvaluationFinishedCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditWhenEvaluationFinishedAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditStatus_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetPAService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditStatusCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditStatusAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        #region SendMail

        public static IEnumerable<object[]> SendMail_TestCase()
        {
            yield return new object[]
            {
                PerformanceProcess.Self,
                MailMode.RemindStartEvaluation,
                "The Performance Appraisal for the 2h 2018 is coming!",
                "The Performance Appraisal for the 2h 2018 will begin on 20 Dec 2018."
            };
            yield return new object[]
            {
                PerformanceProcess.Reviewer,
                MailMode.Start,
                "The Performance Appraisal for the 2h 2018 has begun!",
                "The Performance Appraisal for the 2h 2018 is in progress."
            };
            yield return new object[]
            {
                PerformanceProcess.Reviewer,
                MailMode.Remind,
                "The Performance Appraisal for the 2h 2018 is almost over!",
                "The Performance Appraisal for the 2h 2018 is going to be closed on 31 Dec 2018."
            };
            yield return new object[]
            {
                PerformanceProcess.Self,
                MailMode.EndEvaluation,
                "The Performance Appraisal for the 2h 2018 is complete!",
                "The Performance Appraisal for the 2h 2018 is now complete."
            };
        }

        [Theory, MemberData(nameof(SendMail_TestCase))]
        public async Task SendMail_WhenCalled_ExpectedSendMail(
            PerformanceProcess processId,
            MailMode mailMode,
            string mailSubject,
            string mailMody)
        {
            // Arrange
            var startDate = new DateTime(2018, 12, 20);
            var endDate = new DateTime(2018, 12, 31);
            var periods = new List<PeriodModel>
            {
                new PeriodModel { PeriodId = 1, Name = "2h 2018", StartDate = startDate, EndDate = endDate }
            };
            var settings = new List<ProcessSettingModel>
            {
                new ProcessSettingModel { PeriodId = 1, ProcessId = processId, EvaluatorId = 285, Nickname = "tim", Email = "tim.trinh@nexcel.vn", NotificationMode = mailMode }
            };
            var mailModel = new MailModel();
            var receiverEmail = "tim.trinh@nexcel.vn";
            var service = GetPAService();
            baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns((periods, settings));
            mailService.ReadTextFile(Arg.Any<string>()).Returns(mailMody);
            await mailService.SendMailAsync(Arg.Do<MailModel>(x => mailModel = x));
            mailService.FormatDate(startDate).Returns("20 Dec 2018");
            mailService.FormatDate(endDate).Returns("31 Dec 2018");

            // Act
            await service.SendMailAsync();

            // Assert
            await mailService.Received(1).SendMailAsync(Arg.Any<MailModel>());
            Assert.Equal(mailModel.Subject, mailSubject);
            Assert.Equal(mailModel.Body, mailMody);
            Assert.Equal(mailModel.ReceiverEmail, receiverEmail);
        }

        #endregion SendMail

        #region SendNotification

        [Fact]
        public async Task SendNotification_DoNotHaveData_NotSendNotification()
        {
            // Arrange
            var tuple = (new List<PeriodModel>(), new List<ProcessSettingModel>());
            var service = GetPAService();
            baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(tuple);

            // Act
            await service.SendNotificationAsync();

            // Assert
            await baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<CreateNotificationCriteria>());
        }

        [Fact]
        public async Task SendNotification_HasData_ExpectedSendNotification()
        {
            // Arrange
            var startDate = new DateTime(2018, 12, 20);
            var endDate = new DateTime(2018, 12, 31);
            var periods = new List<PeriodModel>
            {
                new PeriodModel { PeriodId = 1, Name = "2h 2018", StartDate = startDate, EndDate = endDate }
            };
            var settings = new List<ProcessSettingModel>
            {
                new ProcessSettingModel { PeriodId = 1, ProcessId = PerformanceProcess.Self, EvaluatorId = 285, Nickname = "tim", NotificationMode = MailMode.RemindStartEvaluation },
                new ProcessSettingModel { PeriodId = 1, ProcessId = PerformanceProcess.Evaluator, EvaluatorId = 125, Nickname = "quinn", NotificationMode = MailMode.RemindStartEvaluation },
                new ProcessSettingModel { PeriodId = 1, EvaluatorId = 285, Nickname = "tim", NotificationMode = MailMode.Start, EndDate = endDate },
                new ProcessSettingModel { PeriodId = 1, EvaluatorId = 285, Nickname = "tim", NotificationMode = MailMode.Remind, EndDate = endDate },
                new ProcessSettingModel { PeriodId = 1, EvaluatorId = 285, Nickname = "tim", NotificationMode = MailMode.EndEvaluation, PerformanceRecordId = 1 }
            };
            var notificationXml = "<Row EmployeeId=\"285\"" +
                $" Content=\"{string.Format(Message.NotificationTemplate_PA_RemindStartEvaluationToMember, "20 Dec 2018").Replace("&", "&amp;")}\"" +
                " StatusId=\"0\" ActionTypeId=\"1\" RecordId=\"\" />";
            var notification1Xml = "<Row EmployeeId=\"125\"" +
                $" Content=\"{string.Format(Message.NotificationTemplate_PA_RemindStartEvaluationToSupervisor, "20 Dec 2018").Replace("&", "&amp;")}\"" +
                " StatusId=\"0\" ActionTypeId=\"1\" RecordId=\"\" />";
            var notification2Xml = "<Row EmployeeId=\"285\"" +
                " Content=\"The Performance Appraisal for the 2h 2018 is IN PROGRESS and will end at 23:59 31 Dec 2018, please revise and submit the PA for your members.\"" +
                " StatusId=\"0\" ActionTypeId=\"6\" RecordId=\"\" />";
            var notification3Xml = "<Row EmployeeId=\"285\"" +
                " Content=\"The Performance Appraisal for the 2h 2018 is going to be CLOSED at 23:59 31 Dec 2018, please make sure to revise and submit the PA for all of your members.\"" +
                " StatusId=\"0\" ActionTypeId=\"6\" RecordId=\"\" />";
            var notification4Xml = "<Row EmployeeId=\"285\"" +
                " Content=\"The Performance Appraisal for the 2h 2018 is COMPLETE!\"" +
                " StatusId=\"0\" ActionTypeId=\"2\" RecordId=\"1\" />";

            var criteria = new CreateNotificationCriteria();

            var service = GetPAService();
            baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns((periods, settings));
            await baseService.ExecuteNonQueryAsync(Arg.Do<CreateNotificationCriteria>(x => criteria = x));
            mailService.FormatDate(startDate).Returns("20 Dec 2018");
            mailService.FormatDate(endDate).Returns("31 Dec 2018");

            // Act
            await service.SendNotificationAsync();

            // Assert
            await baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<CreateNotificationCriteria>());
            Assert.Contains(notificationXml, criteria.Notifications);
            Assert.Contains(notification1Xml, criteria.Notifications);
            Assert.Contains(notification2Xml, criteria.Notifications);
            Assert.Contains(notification3Xml, criteria.Notifications);
            Assert.Contains(notification4Xml, criteria.Notifications);
        }

        #endregion SendNotification

        #region Helpers

        private PAService GetPAService()
        {
            IWebHostEnvironment hostingEnvironment = Helpers.FakeHostingEnvironment();
            appSettings = Helpers.FakeAppSettings();
            baseService = Helpers.FakeBaseService();
            mailService = Helpers.FakeBaseMailService();
            cryptoService = Helpers.FakeCryptoService();
            chatbotService = Helpers.FakeChatbotService();

            return new PAService(hostingEnvironment, appSettings, baseService, mailService, cryptoService, chatbotService);
        }

        #endregion Helpers
    }
}