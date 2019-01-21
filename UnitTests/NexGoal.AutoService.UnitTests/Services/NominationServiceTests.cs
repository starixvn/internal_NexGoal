namespace NexGoal.AutoService.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Criterias;
    using NexGoal.AutoService.Criterias.Nomination;
    using NexGoal.AutoService.Models.Nomination;
    using NexGoal.AutoService.Services;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NSubstitute;
    using Xunit;

    public class NominationServiceTests
    {
        private IOptionsSnapshot<AppSettings> configuration;
        private IBaseService baseService;
        private IBaseMailService mailService;

        [Fact]
        public void EditWhenFinished_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetNominationService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditWhenFinishedCriteria>()).Returns(expected);

            // Act
            var actual = service.EditWhenFinished();

            // Assert
            Assert.Equal(expected, actual);
        }

        #region SendMail

        [Fact]
        public void SendMail_Start_ExpectedSendMail()
        {
            // Arrange

            var periods = new List<NominationPeriodModel>
            {
                new NominationPeriodModel { NominationPeriodId = 1, Year = 2018 }
            };
            var settings = new List<ProcessSettingModel>
            {
                new ProcessSettingModel { NominationPeriodId = 1, Nickname = "tim", Email = "tim.trinh@nexcel.vn", NotificationMode = MailMode.Start }
            };
            var mailModel = new MailModel();
            var mailSubject = string.Format("The Nomination {0} process has begun!", periods[0].Title);
            var mailMody = "The Nomination is in progress";
            var receiverEmail = "tim.trinh@nexcel.vn";
            var tuple = new Tuple<IEnumerable<NominationPeriodModel>, IEnumerable<ProcessSettingModel>>(periods, settings);
            var service = GetNominationService();
            baseService.FetchMultipleAsync<NominationPeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(tuple);
            mailService.ReadTextFile(Arg.Any<string>()).Returns(mailMody);
            mailService.SendMailAsync(Arg.Do<MailModel>(x => mailModel = x));

            // Act
            service.SendMail();

            // Assert
            mailService.Received(1).SendMailAsync(Arg.Any<MailModel>());
            Assert.Equal(mailModel.Subject, mailSubject);
            Assert.Equal(mailModel.Body, mailMody);
            Assert.Equal(mailModel.ReceiverEmail, receiverEmail);
        }

        [Fact]
        public void SendMail_Remind_ExpectedSendMail()
        {
            // Arrange
            var periods = new List<NominationPeriodModel>
            {
                new NominationPeriodModel { NominationPeriodId = 1, Year = 2018 }
            };
            var settings = new List<ProcessSettingModel>
            {
                new ProcessSettingModel { NominationPeriodId = 1, Nickname = "tim", Email = "tim.trinh@nexcel.vn", NotificationMode = MailMode.Remind }
            };
            var mailModel = new MailModel();
            var mailSubject = string.Format("The Nomination {0} process is almost over!", periods[0].Title);
            var mailMody = "The Nomination is going to be closed";
            var receiverEmail = "tim.trinh@nexcel.vn";
            var tuple = new Tuple<IEnumerable<NominationPeriodModel>, IEnumerable<ProcessSettingModel>>(periods, settings);
            var service = GetNominationService();
            baseService.FetchMultipleAsync<NominationPeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(tuple);
            mailService.ReadTextFile(Arg.Any<string>()).Returns(mailMody);
            mailService.SendMailAsync(Arg.Do<MailModel>(x => mailModel = x));

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
        public void SendNotification_NominationIsNotBeginOrRemind_NotSendNotification()
        {
            // Arrange
            var tuple = new Tuple<IEnumerable<NominationPeriodModel>, IEnumerable<ProcessSettingModel>>
                (new List<NominationPeriodModel>(), new List<ProcessSettingModel>());
            var service = GetNominationService();
            baseService.FetchMultipleAsync<NominationPeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(tuple);

            // Act
            service.SendNotification();

            // Assert
            baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<CreateNotificationCriteria>());
        }

        [Fact]
        public void SendNotification_NominationIsBeginOrRemind_ExpectedSendNotification()
        {
            // Arrange
            var endDate = new DateTime(2018, 12, 20);
            var periods = new List<NominationPeriodModel>
            {
                new NominationPeriodModel { NominationPeriodId = 1, Quarter = 4, Year = 2018, EndDateForNominator = endDate },
                new NominationPeriodModel { NominationPeriodId = 2, Year = 2018, EndDateForNominator = endDate }
            };
            var settings = new List<ProcessSettingModel>
            {
                new ProcessSettingModel { NominationPeriodId = 1, NominatorId = 285, Nickname = "tim", Email = "tim.trinh@nexcel.vn", NotificationMode = MailMode.Start },
                new ProcessSettingModel { NominationPeriodId = 2, NominatorId = 285, Nickname = "tim", Email = "tim.trinh@nexcel.vn", NotificationMode = MailMode.Remind }
            };
            var notificationXml = "<Row EmployeeId=\"285\"" +
                " Content=\"The Nomination process for the Employee of the Quarter - IV 2018 is IN PROGRESS, please submit the vote for your nominee.\"" +
                " StatusId=\"0\" ActionTypeId=\"5\" RecordId=\"\" />";
            var notification1Xml = "<Row EmployeeId=\"285\"" +
                " Content=\"The Nomination process for the Employee of the Year - 2018 is going to be CLOSED on 20 Dec 2018, please submit the vote for your nominee.\"" +
                " StatusId=\"0\" ActionTypeId=\"5\" RecordId=\"\" />";

            var criteria = new CreateNotificationCriteria();

            var tuple = new Tuple<IEnumerable<NominationPeriodModel>, IEnumerable<ProcessSettingModel>>(periods, settings);
            var service = GetNominationService();
            baseService.FetchMultipleAsync<NominationPeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(tuple);
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateNotificationCriteria>(x => criteria = x));
            mailService.FormatEndDate(endDate).Returns("20 Dec 2018");

            // Act
            service.SendNotification();

            // Assert
            baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<CreateNotificationCriteria>());
            Assert.Contains(notificationXml, criteria.Notifications);
            Assert.Contains(notification1Xml, criteria.Notifications);
        }

        #endregion SendNotification

        #region Helpers

        private NominationService GetNominationService()
        {
            configuration = Helpers.FakeOptionsSnapshot();
            baseService = Helpers.FakeBaseService();
            mailService = Helpers.FakeBaseMailService();

            return new NominationService(configuration, baseService, mailService);
        }

        #endregion Helpers
    }
}