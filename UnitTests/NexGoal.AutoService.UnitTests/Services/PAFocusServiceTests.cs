namespace NexGoal.AutoService.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Criterias;
    using NexGoal.AutoService.Criterias.PAFocus;
    using NexGoal.AutoService.Models.PAFocus;
    using NexGoal.AutoService.Models.Period;
    using NexGoal.AutoService.Services;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NSubstitute;
    using Xunit;

    public class PAFocusServiceTests
    {
        private IOptionsSnapshot<AppSettings> configuration;
        private IBaseService baseService;
        private IBaseMailService mailService;
        private IChatbotService chatbotService;

        [Fact]
        public async Task EditWhenFinished_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var periodIds = new List<int> { 1, 2 };
            var expected = new CommandResult
            {
                Code = 0
            };
            var criteria = new GenerateCriteria();
            var service = GetPAFocusService();
            baseService.GetDataAsync<int>(Arg.Any<GetPeriodStartedCriteria>()).Returns(periodIds);
            baseService.ExecuteNonQueryAsync(Arg.Do<GenerateCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await service.GenerateAsync();

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal("1,2", criteria.PeriodIds);
        }

        #region SendMail

        [Fact]
        public async Task SendMail_Start_ExpectedSendMail()
        {
            // Arrange
            var periods = new List<PeriodModel>
            {
                new PeriodModel { PeriodId = 1, Name = "2h 2018" }
            };
            var settings = new List<ProcessSettingModel>
            {
                new ProcessSettingModel { PeriodId = 1, Nickname = "tim", Email = "tim.trinh@nexcel.vn", NotificationMode = MailMode.Start }
            };
            var mailModel = new MailModel();
            var mailSubject = string.Format("The PA Focus for the {0} has begun!", periods[0].Name);
            var mailMody = "The PA Focus has begun!";
            var receiverEmail = "tim.trinh@nexcel.vn";
            var tuple = new Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>>(periods, settings);
            var service = GetPAFocusService();
            baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(tuple);
            mailService.ReadTextFile(Arg.Any<string>()).Returns(mailMody);
            await mailService.SendMailAsync(Arg.Do<MailModel>(x => mailModel = x));

            // Act
            await service.SendMailAsync();

            // Assert
            await mailService.Received(1).SendMailAsync(Arg.Any<MailModel>());
            Assert.Equal(mailModel.Subject, mailSubject);
            Assert.Equal(mailModel.Body, mailMody);
            Assert.Equal(mailModel.ReceiverEmail, receiverEmail);
        }

        [Fact]
        public async Task SendMail_Remind_ExpectedSendMail()
        {
            // Arrange
            var periods = new List<PeriodModel>
            {
                new PeriodModel { PeriodId = 1, Name = "2h 2018" }
            };
            var settings = new List<ProcessSettingModel>
            {
                new ProcessSettingModel { PeriodId = 1, Nickname = "tim", Email = "tim.trinh@nexcel.vn", NotificationMode = MailMode.Remind }
            };
            var mailModel = new MailModel();
            var mailSubject = string.Format("The PA Focus for the {0} is almost over!", periods[0].Name);
            var mailMody = "The PA Focus is going to be closed";
            var receiverEmail = "tim.trinh@nexcel.vn";
            var tuple = new Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>>(periods, settings);
            var service = GetPAFocusService();
            baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(tuple);
            mailService.ReadTextFile(Arg.Any<string>()).Returns(mailMody);
            await mailService.SendMailAsync(Arg.Do<MailModel>(x => mailModel = x));

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
            var tuple = new Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>>
                (new List<PeriodModel>(), new List<ProcessSettingModel>());
            var service = GetPAFocusService();
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
            var endDate = new DateTime(2018, 12, 20);
            var periods = new List<PeriodModel>
            {
                new PeriodModel { PeriodId = 1, Name = "1h 2018", FocusEndDate = endDate },
                new PeriodModel { PeriodId = 2, Name = "2h 2018", FocusEndDate = endDate }
            };
            var settings = new List<ProcessSettingModel>
            {
                new ProcessSettingModel { PeriodId = 1, EvaluatorId = 285, Nickname = "tim", Email = "tim.trinh@nexcel.vn", NotificationMode = MailMode.Start },
                new ProcessSettingModel { PeriodId = 2, EvaluatorId = 285, Nickname = "tim", Email = "tim.trinh@nexcel.vn", NotificationMode = MailMode.Remind }
            };
            var notificationXml = "<Row EmployeeId=\"285\"" +
                " Content=\"The PA Focus for the 1h 2018 is IN PROGRESS and will end at 23:59 20 Dec 2018, please set the objectives for your members.\"" +
                " StatusId=\"0\" ActionTypeId=\"7\" RecordId=\"\" />";
            var notification1Xml = "<Row EmployeeId=\"285\"" +
                " Content=\"The PA Focus for the 2h 2018 is going to be CLOSED at 23:59 20 Dec 2018, please review the objectives for all of your members.\"" +
                " StatusId=\"0\" ActionTypeId=\"7\" RecordId=\"\" />";

            var criteria = new CreateNotificationCriteria();

            var tuple = new Tuple<IEnumerable<PeriodModel>, IEnumerable<ProcessSettingModel>>(periods, settings);
            var service = GetPAFocusService();
            baseService.FetchMultipleAsync<PeriodModel, ProcessSettingModel>(Arg.Any<GetSettingToSendNotificationCriteria>()).Returns(tuple);
            await baseService.ExecuteNonQueryAsync(Arg.Do<CreateNotificationCriteria>(x => criteria = x));
            mailService.FormatDate(endDate).Returns("20 Dec 2018");

            // Act
            await service.SendNotificationAsync();

            // Assert
            await baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<CreateNotificationCriteria>());
            Assert.Contains(notificationXml, criteria.Notifications);
            Assert.Contains(notification1Xml, criteria.Notifications);
        }

        #endregion SendNotification

        #region Helpers

        private PAFocusService GetPAFocusService()
        {
            IHostingEnvironment hostingEnvironment = Helpers.FakeHostingEnvironment();
            configuration = Helpers.FakeOptionsSnapshot();
            baseService = Helpers.FakeBaseService();
            mailService = Helpers.FakeBaseMailService();
            chatbotService = Helpers.FakeChatbotService();

            return new PAFocusService(hostingEnvironment, configuration, baseService, mailService, chatbotService);
        }

        #endregion Helpers
    }
}