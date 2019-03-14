namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Options;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceRecord;
    using NexGoal.Models.Common;
    using NexGoal.Models.Notification;
    using NexGoal.Models.PerformanceRecord.Notification;
    using NexGoal.Resources;

    public class NotificationPerformanceRecordService : INotificationPerformanceRecordService
    {
        private const string MemberToSupervisorEmailTemplate = @"EmailTemplates\\PCI\\{0}\\MemberToSupervisor.cshtml";
        private const string SupervisorToMemberEmailTemplate = @"EmailTemplates\\PCI\\{0}\\SupervisorToMember.cshtml";
        private const string SupervisorToSupervisorEmailTemplate = @"EmailTemplates\\PCI\\{0}\\SupervisorToSupervisor.cshtml";

        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IOptionsSnapshot<AppSettings> configuration;
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;
        private readonly IBaseMailService mailService;
        private readonly ISkypeNotiService skypeNotiService;

        public NotificationPerformanceRecordService(
            IHostingEnvironment hostingEnvironment,
            IOptionsSnapshot<AppSettings> configuration,
            IBaseService baseService,
            ICommonService commonService,
            IBaseMailService mailService,
            ISkypeNotiService skypeNotiService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
            this.baseService = baseService;
            this.commonService = commonService;
            this.mailService = mailService;
            this.skypeNotiService = skypeNotiService;
        }

        public async Task SendMailAsync(string performanceRecordIds, int actorId, ActionOnPerformanceRecord action)
        {
            IEnumerable<int> recordIds = performanceRecordIds.Split(",").Select(x => int.Parse(x));
            Tuple<IEnumerable<EmployeeDto>, IEnumerable<ProcessSettingDto>> tuple =
                await GetSettingToSendNotificationAsync(performanceRecordIds, actorId, ReceiveNotificationType.Email).ConfigureAwait(false);
            Employee actor = await commonService.GetEmployeeByIdAsync(actorId).ConfigureAwait(false);

            foreach (int recordId in recordIds)
            {
                IEnumerable<ProcessSettingDto> receivers = tuple.Item2.Where(x => x.PerformanceRecordId == recordId);
                if (receivers.Any(x => x.NotificationMode == PerformanceRecordMailMode.MemberToSupervisor))
                {
                    var receiver = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.MemberToSupervisor).OrderBy(x => x.Priority).First();
                    var cc = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.MemberToSupervisor && x.EvaluatorId != receiver.EvaluatorId)
                        .Select(x => x.Email);

                    await SendMailFromMemberToSupervisorAsync(actor, receiver, cc, action).ConfigureAwait(false);
                }

                if (receivers.Any(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToMember))
                {
                    var receiver = receivers.First(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToMember);

                    await SendMailFromSupervisorToMemberAsync(actor, receiver, action).ConfigureAwait(false);
                }

                if (receivers.Any(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToSupervisor))
                {
                    EmployeeDto employee = tuple.Item1.First(y => y.PerformanceRecordId == recordId);
                    var receiver = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToSupervisor).OrderBy(x => x.Priority).First();
                    var cc = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToSupervisor && x.EvaluatorId != receiver.EvaluatorId)
                        .Select(x => x.Email);

                    await SendMailFromSupervisorToSupervisorAsync(actor, employee, receiver, cc, action).ConfigureAwait(false);
                }
            }
        }

        public async Task SendNotificationAsync(string performanceRecordIds, int actorId, ActionOnPerformanceRecord action)
        {
            var notifications = new List<CreateNotificationModel>();
            IEnumerable<int> recordIds = performanceRecordIds.Split(",").Select(x => int.Parse(x));
            Tuple<IEnumerable<EmployeeDto>, IEnumerable<ProcessSettingDto>> tuple =
                await GetSettingToSendNotificationAsync(performanceRecordIds, actorId, ReceiveNotificationType.Notification).ConfigureAwait(false);
            Employee actor = await commonService.GetEmployeeByIdAsync(actorId).ConfigureAwait(false);

            foreach (int recordId in recordIds)
            {
                EmployeeDto employee = tuple.Item1.First(y => y.PerformanceRecordId == recordId);
                IEnumerable<ProcessSettingDto> receivers = tuple.Item2.Where(x => x.PerformanceRecordId == recordId);

                notifications.AddRange(receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.MemberToSupervisor)
                    .Select(x => new CreateNotificationModel
                    {
                        EmployeeId = x.EvaluatorId,
                        Content = action == ActionOnPerformanceRecord.Create
                            ? string.Format(Message.NotificationTemplate_CreatePCI_MemberToSupervisor, actor.NickName)
                            : string.Format(Message.NotificationTemplate_EditPCI_MemberToSupervisor, actor.NickName),
                        StatusId = (int)NotificationStatus.Unread,
                        ActionTypeId = (int)NotificationActionType.MemberPageWithSpecificPerformanceCheckin,
                        RecordId = recordId
                    }));

                notifications.AddRange(receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToMember)
                    .Select(x => new CreateNotificationModel
                    {
                        EmployeeId = x.EvaluatorId,
                        Content = action == ActionOnPerformanceRecord.Create
                            ? string.Format(Message.NotificationTemplate_CreatePCI_SupervisorToMember, actor.NickName)
                            : string.Format(Message.NotificationTemplate_EditPCI_SupervisorToMember, actor.NickName),
                        StatusId = (int)NotificationStatus.Unread,
                        ActionTypeId = (int)NotificationActionType.MyPageWithSpecificPerformanceCheckin,
                        RecordId = recordId
                    }));

                notifications.AddRange(receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToSupervisor)
                    .Select(x => new CreateNotificationModel
                    {
                        EmployeeId = x.EvaluatorId,
                        Content = action == ActionOnPerformanceRecord.Create
                            ? string.Format(Message.NotificationTemplate_CreatePCI_SupervisorToSupervisor, actor.NickName, employee.Nickname)
                            : string.Format(Message.NotificationTemplate_EditPCI_SupervisorToSupervisor, actor.NickName, employee.Nickname),
                        StatusId = (int)NotificationStatus.Unread,
                        ActionTypeId = (int)NotificationActionType.MemberPageWithSpecificPerformanceCheckin,
                        RecordId = recordId
                    }));
            }

            if (notifications.Any())
            {
                var criteria = new CreateNotificationCriteria
                {
                    ActorId = actorId,
                    Notifications = notifications.ToXML().ToString()
                };

                await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            }
        }

        public async Task SendSkypeMessageAsync(string performanceRecordIds, int actorId, ActionOnPerformanceRecord action)
        {
            IEnumerable<int> recordIds = performanceRecordIds.Split(",").Select(x => int.Parse(x));
            Tuple<IEnumerable<EmployeeDto>, IEnumerable<ProcessSettingDto>> tuple =
                await GetSettingToSendNotificationAsync(performanceRecordIds, actorId, ReceiveNotificationType.SkypeMessage)
                    .ConfigureAwait(false);
            Employee actor = await commonService.GetEmployeeByIdAsync(actorId).ConfigureAwait(false);

            foreach (int recordId in recordIds)
            {
                IEnumerable<ProcessSettingDto> receivers = tuple.Item2.Where(x => x.PerformanceRecordId == recordId);
                if (receivers.Any(x => x.NotificationMode == PerformanceRecordMailMode.MemberToSupervisor))
                {
                    var conversations = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.MemberToSupervisor).OrderBy(x => x.Priority);

                    await skypeNotiService.SendMessageMemberToSupervisorAsync(actor, conversations, action).ConfigureAwait(false);
                }

                if (receivers.Any(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToMember))
                {
                    var conversations = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToMember);

                    await skypeNotiService.SendMessageSupervisorToMemberAsync(actor, conversations, action).ConfigureAwait(false);
                }

                if (receivers.Any(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToSupervisor))
                {
                    EmployeeDto employee = tuple.Item1.First(y => y.PerformanceRecordId == recordId);
                    var conversations = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToSupervisor);

                    await skypeNotiService.SendMessageSupervisorToSupervisorAsync(actor, conversations, employee, action).ConfigureAwait(false);
                }
            }
        }

        private async Task<Tuple<IEnumerable<EmployeeDto>, IEnumerable<ProcessSettingDto>>> GetSettingToSendNotificationAsync(
            string performanceRecordIds,
            int actorId,
            ReceiveNotificationType receiveType)
        {
            var criteria = new GetSettingToSendNotificationCriteria
            {
                PerformanceRecordIds = performanceRecordIds,
                ActorId = actorId,
                ReceiveType = (int)receiveType
            };

            return await baseService.FetchMultipleAsync<EmployeeDto, ProcessSettingDto>(criteria).ConfigureAwait(false);
        }

        private async Task SendMailFromMemberToSupervisorAsync(
            Employee actor,
            ProcessSettingDto receiver,
            IEnumerable<string> cc,
            ActionOnPerformanceRecord action)
        {
            var emailTemplate = string.Format(MemberToSupervisorEmailTemplate, action == ActionOnPerformanceRecord.Create ? "Create" : "Edit");
            var subject = action == ActionOnPerformanceRecord.Create
                ? string.Format(Message.EmailSubject_CreatePCI_MemberToSupervisor, actor.NickName)
                : string.Format(Message.EmailSubject_EditPCI_MemberToSupervisor, actor.NickName);
            var filePath = Path.Combine(hostingEnvironment.WebRootPath, emailTemplate);

            var body = string.Format(
                mailService.ReadTextFile(filePath),
                receiver.Nickname,
                actor.NickName,
                configuration.Value.ApplicationDomain);

            var mailModel = new MailModel
            {
                Subject = subject,
                ReceiverEmail = receiver.Email,
                Cc = cc,
                Body = body
            };

            await mailService.SendMailAsync(mailModel).ConfigureAwait(false);
        }

        private async Task SendMailFromSupervisorToMemberAsync(
            Employee actor,
            ProcessSettingDto receiver,
            ActionOnPerformanceRecord action)
        {
            var emailTemplate = string.Format(SupervisorToMemberEmailTemplate, action == ActionOnPerformanceRecord.Create ? "Create" : "Edit");
            var subject = action == ActionOnPerformanceRecord.Create
                ? string.Format(Message.EmailSubject_CreatePCI_SupervisorToMember, actor.NickName)
                : string.Format(Message.EmailSubject_EditPCI_SupervisorToMember, actor.NickName);
            var filePath = Path.Combine(hostingEnvironment.WebRootPath, emailTemplate);

            var body = string.Format(
                mailService.ReadTextFile(filePath),
                receiver.Nickname,
                actor.NickName,
                configuration.Value.ApplicationDomain);

            var mailModel = new MailModel
            {
                Subject = subject,
                ReceiverEmail = receiver.Email,
                Body = body
            };

            await mailService.SendMailAsync(mailModel).ConfigureAwait(false);
        }

        private async Task SendMailFromSupervisorToSupervisorAsync(
            Employee actor,
            EmployeeDto employee,
            ProcessSettingDto receiver,
            IEnumerable<string> cc,
            ActionOnPerformanceRecord action)
        {
            var emailTemplate = string.Format(SupervisorToSupervisorEmailTemplate, action == ActionOnPerformanceRecord.Create ? "Create" : "Edit");
            var subject = action == ActionOnPerformanceRecord.Create
                ? string.Format(Message.EmailSubject_CreatePCI_SupervisorToSupervisor, actor.NickName, employee.Nickname)
                : string.Format(Message.EmailSubject_EditPCI_SupervisorToSupervisor, actor.NickName, employee.Nickname);
            var filePath = Path.Combine(hostingEnvironment.WebRootPath, emailTemplate);

            var body = string.Format(
                mailService.ReadTextFile(filePath),
                receiver.Nickname,
                actor.NickName,
                employee.Nickname,
                configuration.Value.ApplicationDomain);

            var mailModel = new MailModel
            {
                Subject = subject,
                ReceiverEmail = receiver.Email,
                Cc = cc,
                Body = body
            };

            await mailService.SendMailAsync(mailModel).ConfigureAwait(false);
        }
    }
}