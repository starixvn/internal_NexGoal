namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceRecord;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord.Notification;
    using NSubstitute;
    using Xunit;

    public class NotificationPerformanceRecordServiceTests
    {
        private IWebHostEnvironment hostingEnvironment;
        private AppSettings appSettings;
        private IBaseService baseService;
        private ICommonService commonService;
        private IBaseMailService mailService;
        private ISkypeNotiService skypeNotiService;

        #region SendMailAsync

        public static IEnumerable<object[]> MemberCreateOrEditRecord_TestCase()
        {
            yield return new object[]
            {
                ActionOnPerformanceRecord.Create,
                "tim has created a Performance Record!",
                "tim has created a Performance Record!"
            };
            yield return new object[]
            {
                ActionOnPerformanceRecord.Edit,
                "tim has edited his/her Performance Record!",
                "tim has edited his/her Performance Record!"
            };
        }

        [Theory, MemberData(nameof(MemberCreateOrEditRecord_TestCase))]
        public async Task SendMailAsync_MemberCreateOrEditRecord_ExpectedSendMail(
            ActionOnPerformanceRecord action,
            string mailSubject,
            string mailMody)
        {
            // Arrange
            var employees = new List<EmployeeDto>
            {
                new EmployeeDto { PerformanceRecordId = 1, EmployeeId = 1, Nickname = "tim" }
            };
            var settings = new List<ProcessSettingDto>
            {
                new ProcessSettingDto { PerformanceRecordId = 1, EvaluatorId = 1, Nickname = "quinn", Email = "quinn.vo@nexcel.vn",
                    Priority = 1, NotificationMode = PerformanceRecordMailMode.MemberToSupervisor }
            };
            var actor = new Employee { EmployeeId = 1, NickName = "tim" };
            var mailModel = new MailModel();
            var service = GetService();
            commonService.GetEmployeeByIdAsync(1).Returns(actor);
            baseService.FetchMultipleAsync<EmployeeDto, ProcessSettingDto>(Arg.Any<GetSettingToSendNotificationCriteria>())
                .Returns((employees, settings));
            mailService.ReadTextFile(Arg.Any<string>()).Returns(mailMody);
            await mailService.SendMailAsync(Arg.Do<MailModel>(x => mailModel = x));

            // Act
            await service.SendMailAsync("1", 1, action);

            // Assert
            await mailService.Received(1).SendMailAsync(Arg.Any<MailModel>());
            Assert.Equal(mailModel.Subject, mailSubject);
            Assert.Equal(mailModel.Body, mailMody);
        }

        #endregion SendMailAsync

        #region Helpers

        private NotificationPerformanceRecordService GetService()
        {
            hostingEnvironment = Helpers.FakeHostingEnvironment();
            appSettings = Helpers.FakeAppSettings();
            baseService = Helpers.FakeBaseService();
            commonService = Helpers.FakeCommonService();
            mailService = Helpers.FakeBaseMailService();
            skypeNotiService = Helpers.FakeSkypeNotiService();

            return new NotificationPerformanceRecordService(
                hostingEnvironment,
                appSettings,
                baseService,
                commonService,
                mailService,
                skypeNotiService);
        }

        #endregion Helpers
    }
}