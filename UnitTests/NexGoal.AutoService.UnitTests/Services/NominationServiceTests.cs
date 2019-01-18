namespace NexGoal.AutoService.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Options;
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
                new ProcessSettingModel { NominationPeriodId = 1, Nickname = "tim", Email = "tim.trinh@nexcel.vn", NotificationMode = MailMode.Start}
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
                new ProcessSettingModel { NominationPeriodId = 1, Nickname = "tim", Email = "tim.trinh@nexcel.vn", NotificationMode = MailMode.Remind}
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

        #region Helpers

        private IOptionsSnapshot<AppSettings> FakeOptionsSnapshot()
        {
            var appSettings = new AppSettings
            {
                ApplicationDomain = "https://goal.nexcel.vn"
            };

            var config = Substitute.For<IOptionsSnapshot<AppSettings>>();
            config.Value.Returns(appSettings);

            return config;
        }

        private NominationService GetNominationService()
        {
            configuration = FakeOptionsSnapshot();
            baseService = Substitute.For<IBaseService>();
            mailService = Substitute.For<IBaseMailService>();

            return new NominationService(configuration, baseService, mailService);
        }

        #endregion Helpers
    }
}