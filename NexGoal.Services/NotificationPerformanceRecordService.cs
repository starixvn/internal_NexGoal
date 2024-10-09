namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceRecord;
    using NexGoal.Models.Common;
    using NexGoal.Models.Notification;
    using NexGoal.Models.PerformanceRecord.Notification;
    using NexGoal.Resources;

    public class NotificationPerformanceRecordService(
        IWebHostEnvironment hostingEnvironment,
        AppSettings appSettings,
        IBaseService baseService,
        ICommonService commonService,
        IBaseMailService mailService,
        ISkypeNotiService skypeNotiService) : INotificationPerformanceRecordService
    {
        private const string MemberToSupervisorEmailTemplate = "EmailTemplates/PCI/{0}/MemberToSupervisor.txt";
        private const string SupervisorToMemberEmailTemplate = "EmailTemplates/PCI/{0}/SupervisorToMember.txt";
        private const string SupervisorToSupervisorEmailTemplate = "EmailTemplates/PCI/{0}/SupervisorToSupervisor.txt";

        public async Task SendMailAsync(string performanceRecordIds, int actorId, ActionOnPerformanceRecord action)
        {
            var recordIds = performanceRecordIds.Split(",").Select(x => int.Parse(x)).ToList();
            var (employees, processSettings) = await GetSettingToSendNotificationAsync(performanceRecordIds, actorId, ReceiveNotificationType.Email);
            var actor = await commonService.GetEmployeeByIdAsync(actorId);

            foreach (int recordId in recordIds)
            {
                var receivers = processSettings.Where(x => x.PerformanceRecordId == recordId).ToList();
                if (receivers.Exists(x => x.NotificationMode == PerformanceRecordMailMode.MemberToSupervisor))
                {
                    var receiver = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.MemberToSupervisor).OrderBy(x => x.Priority).First();
                    var cc = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.MemberToSupervisor && x.EvaluatorId != receiver.EvaluatorId)
                        .Select(x => x.Email);

                    await SendMailFromMemberToSupervisorAsync(actor, receiver, cc, action);
                }

                if (receivers.Exists(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToMember))
                {
                    var receiver = receivers.First(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToMember);

                    await SendMailFromSupervisorToMemberAsync(actor, receiver, action);
                }

                if (receivers.Exists(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToSupervisor))
                {
                    var employee = employees.First(y => y.PerformanceRecordId == recordId);
                    var receiver = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToSupervisor).OrderBy(x => x.Priority).First();
                    var cc = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToSupervisor && x.EvaluatorId != receiver.EvaluatorId)
                        .Select(x => x.Email);

                    await SendMailFromSupervisorToSupervisorAsync(actor, employee, receiver, cc, action);
                }
            }
        }

        public async Task SendNotificationAsync(string performanceRecordIds, int actorId, ActionOnPerformanceRecord action)
        {
            var notifications = new List<CreateNotificationModel>();
            var recordIds = performanceRecordIds.Split(",").Select(x => int.Parse(x)).ToList();
            var (employees, processSettings) = await GetSettingToSendNotificationAsync(performanceRecordIds, actorId, ReceiveNotificationType.Notification);
            var actor = await commonService.GetEmployeeByIdAsync(actorId);

            foreach (int recordId in recordIds)
            {
                var employee = employees.First(y => y.PerformanceRecordId == recordId);
                var receivers = processSettings.Where(x => x.PerformanceRecordId == recordId).ToList();

                notifications.AddRange(receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.MemberToSupervisor)
                    .Select(x => new CreateNotificationModel
                    {
                        EmployeeId = x.EvaluatorId,
                        Content = action == ActionOnPerformanceRecord.Create
                            ? string.Format(Message.NotificationTemplate_CreatePCI_MemberToSupervisor, actor.NickName)
                            : string.Format(Message.NotificationTemplate_EditPCI_MemberToSupervisor, actor.NickName),
                        StatusId = (byte)NotificationStatus.Unread,
                        ActionTypeId = (byte)NotificationActionType.MemberPageWithSpecificPerformanceCheckin,
                        RecordId = recordId
                    }));

                notifications.AddRange(receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToMember)
                    .Select(x => new CreateNotificationModel
                    {
                        EmployeeId = x.EvaluatorId,
                        Content = action == ActionOnPerformanceRecord.Create
                            ? string.Format(Message.NotificationTemplate_CreatePCI_SupervisorToMember, actor.NickName)
                            : string.Format(Message.NotificationTemplate_EditPCI_SupervisorToMember, actor.NickName),
                        StatusId = (byte)NotificationStatus.Unread,
                        ActionTypeId = (byte)NotificationActionType.MyPageWithSpecificPerformanceCheckin,
                        RecordId = recordId
                    }));

                notifications.AddRange(receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToSupervisor)
                    .Select(x => new CreateNotificationModel
                    {
                        EmployeeId = x.EvaluatorId,
                        Content = action == ActionOnPerformanceRecord.Create
                            ? string.Format(Message.NotificationTemplate_CreatePCI_SupervisorToSupervisor, actor.NickName, employee.Nickname)
                            : string.Format(Message.NotificationTemplate_EditPCI_SupervisorToSupervisor, actor.NickName, employee.Nickname),
                        StatusId = (byte)NotificationStatus.Unread,
                        ActionTypeId = (byte)NotificationActionType.MemberPageWithSpecificPerformanceCheckin,
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

                await baseService.ExecuteNonQueryAsync(criteria);
            }
        }

        public async Task SendSkypeMessageAsync(string performanceRecordIds, int actorId, ActionOnPerformanceRecord action)
        {
            var recordIds = performanceRecordIds.Split(",").Select(x => int.Parse(x)).ToList();
            var (employees, processSettings) = await GetSettingToSendNotificationAsync(performanceRecordIds, actorId, ReceiveNotificationType.SkypeMessage);
            var actor = await commonService.GetEmployeeByIdAsync(actorId);

            foreach (int recordId in recordIds)
            {
                var receivers = processSettings.Where(x => x.PerformanceRecordId == recordId).ToList();
                if (receivers.Exists(x => x.NotificationMode == PerformanceRecordMailMode.MemberToSupervisor))
                {
                    var conversations = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.MemberToSupervisor).OrderBy(x => x.Priority);

                    await skypeNotiService.SendMessageMemberToSupervisorAsync(actor, conversations, action);
                }

                if (receivers.Exists(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToMember))
                {
                    var conversations = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToMember);

                    await skypeNotiService.SendMessageSupervisorToMemberAsync(actor, conversations, action);
                }

                if (receivers.Exists(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToSupervisor))
                {
                    var employee = employees.First(y => y.PerformanceRecordId == recordId);
                    var conversations = receivers.Where(x => x.NotificationMode == PerformanceRecordMailMode.SupervisorToSupervisor);

                    await skypeNotiService.SendMessageSupervisorToSupervisorAsync(actor, conversations, employee, action);
                }
            }
        }

        private async Task<(IEnumerable<EmployeeDto>, IEnumerable<ProcessSettingDto>)> GetSettingToSendNotificationAsync(
            string performanceRecordIds,
            int actorId,
            ReceiveNotificationType receiveType)
        {
            var criteria = new GetSettingToSendNotificationCriteria
            {
                PerformanceRecordIds = performanceRecordIds,
                ActorId = actorId,
                ReceiveType = (byte)receiveType
            };

            return await baseService.FetchMultipleAsync<EmployeeDto, ProcessSettingDto>(criteria);
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
            var filePath = Path.Combine(hostingEnvironment.ContentRootPath, emailTemplate);

            var body = string.Format(
                mailService.ReadTextFile(filePath),
                receiver.Nickname,
                actor.NickName,
                $"{appSettings.ApplicationDomain}/member?recordId={receiver.PerformanceRecordId}");

            var mailModel = new MailModel
            {
                Subject = subject,
                ReceiverEmail = receiver.Email,
                Cc = cc,
                Body = body
            };

            await mailService.SendMailAsync(mailModel);
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
            var filePath = Path.Combine(hostingEnvironment.ContentRootPath, emailTemplate);

            var body = string.Format(
                mailService.ReadTextFile(filePath),
                receiver.Nickname,
                actor.NickName,
                $"{appSettings.ApplicationDomain}?recordId={receiver.PerformanceRecordId}");

            var mailModel = new MailModel
            {
                Subject = subject,
                ReceiverEmail = receiver.Email,
                Body = body
            };

            await mailService.SendMailAsync(mailModel);
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
            var filePath = Path.Combine(hostingEnvironment.ContentRootPath, emailTemplate);

            var body = string.Format(
                mailService.ReadTextFile(filePath),
                receiver.Nickname,
                actor.NickName,
                employee.Nickname,
                $"{appSettings.ApplicationDomain}/member?recordId={receiver.PerformanceRecordId}");

            var mailModel = new MailModel
            {
                Subject = subject,
                ReceiverEmail = receiver.Email,
                Cc = cc,
                Body = body
            };

            await mailService.SendMailAsync(mailModel);
        }
    }
}