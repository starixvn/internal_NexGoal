namespace NexGoal.AutoService.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Criterias;
    using NexGoal.AutoService.Criterias.PA;
    using NexGoal.AutoService.Models.PA;
    using NexGoal.AutoService.Models.Period;
    using NexGoal.AutoService.Services;
    using NexGoal.Core.Enumerations;
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
        private IChatbotService chatbotService;

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
        public void SendMail_WhenCalled_ExpectedSendMail(
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
            var tuple = new Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>>(periods, settings);
            var service = GetPAService();
            baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(tuple);
            mailService.ReadTextFile(Arg.Any<string>()).Returns(mailMody);
            mailService.SendMailAsync(Arg.Do<MailModel>(x => mailModel = x));
            mailService.FormatStartDate(startDate).Returns("20 Dec 2018");
            mailService.FormatEndDate(endDate).Returns("31 Dec 2018");

            // Act
            service.SendMail();

            // Assert
            mailService.Received(1).SendMailAsync(Arg.Any<MailModel>());
            Assert.Equal(mailModel.Subject, mailSubject);
            Assert.Equal(mailModel.Body, mailMody);
            Assert.Equal(mailModel.ReceiverEmail, receiverEmail);
        }

        #endregion SendMail

        #region SendNotification

        [Fact]
        public void SendNotification_DoNotHaveData_NotSendNotification()
        {
            // Arrange
            var tuple = new Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>>(new List<PeriodModel>(), new List<ProcessSettingModel>());
            var service = GetPAService();
            baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(tuple);

            // Act
            service.SendNotification();

            // Assert
            baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<CreateNotificationCriteria>());
        }

        [Fact]
        public void SendNotification_HasData_ExpectedSendNotification()
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
                new ProcessSettingModel { PeriodId = 1, EvaluatorId = 285, Nickname = "tim", NotificationMode = MailMode.Start },
                new ProcessSettingModel { PeriodId = 1, EvaluatorId = 285, Nickname = "tim", NotificationMode = MailMode.Remind, EndDate = endDate },
                new ProcessSettingModel { PeriodId = 1, EvaluatorId = 285, Nickname = "tim", NotificationMode = MailMode.EndEvaluation, PerformanceRecordId = 1 }
            };
            var notificationXml = "<Row EmployeeId=\"285\"" +
                " Content=\"The Performance Appraisal for the 2h 2018 will begin on 20 Dec 2018, you are recommended to create a neat &amp; clean Performance Record (with a detailed comment &amp; selected grade) for a better evaluation result.\"" +
                " StatusId=\"0\" ActionTypeId=\"1\" RecordId=\"\" />";
            var notification1Xml = "<Row EmployeeId=\"125\"" +
                " Content=\"The Performance Appraisal for the 2h 2018 will begin on 20 Dec 2018, you are recommended to create a neat &amp; clean Performance Record for yourself and your members (with a detailed comment &amp; selected grade) for a better evaluation result.\"" +
                " StatusId=\"0\" ActionTypeId=\"1\" RecordId=\"\" />";
            var notification2Xml = "<Row EmployeeId=\"285\"" +
                " Content=\"The Performance Appraisal for the 2h 2018 is IN PROGRESS, please revise and submit the PA for your members.\"" +
                " StatusId=\"0\" ActionTypeId=\"6\" RecordId=\"\" />";
            var notification3Xml = "<Row EmployeeId=\"285\"" +
                " Content=\"The Performance Appraisal for the 2h 2018 is going to be CLOSED on 31 Dec 2018, please make sure to revise and submit the PA for all of your members.\"" +
                " StatusId=\"0\" ActionTypeId=\"6\" RecordId=\"\" />";
            var notification4Xml = "<Row EmployeeId=\"285\"" +
                " Content=\"The Performance Appraisal for the 2h 2018 is COMPLETE!\"" +
                " StatusId=\"0\" ActionTypeId=\"2\" RecordId=\"1\" />";

            var criteria = new CreateNotificationCriteria();

            var tuple = new Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>>(periods, settings);
            var service = GetPAService();
            baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(tuple);
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateNotificationCriteria>(x => criteria = x));
            mailService.FormatStartDate(startDate).Returns("20 Dec 2018");
            mailService.FormatEndDate(endDate).Returns("31 Dec 2018");

            // Act
            service.SendNotification();

            // Assert
            baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<CreateNotificationCriteria>());
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
            IHostingEnvironment hostingEnvironment = Helpers.FakeHostingEnvironment();
            configuration = Helpers.FakeOptionsSnapshot();
            baseService = Helpers.FakeBaseService();
            mailService = Helpers.FakeBaseMailService();
            cryptoService = Helpers.FakeCryptoService();
            chatbotService = Helpers.FakeChatbotService();

            return new PAService(hostingEnvironment, configuration, baseService, mailService, cryptoService, chatbotService);
        }

        #endregion Helpers
    }
}