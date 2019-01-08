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
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceRecord;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Resources;

    public class PerformanceRecordMailService : IPerformanceRecordMailService
    {
        private const string MemberToSupervisorEmailTemplate = @"EmailTemplates\\PCI\\{0}\\MemberToSupervisor.cshtml";
        private const string SupervisorToMemberEmailTemplate = @"EmailTemplates\\PCI\\{0}\\SupervisorToMember.cshtml";
        private const string SupervisorToSupervisorEmailTemplate = @"EmailTemplates\\PCI\\{0}\\SupervisorToSupervisor.cshtml";

        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IOptionsSnapshot<AppSettings> configuration;
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;
        private readonly IBaseMailService mailService;

        public PerformanceRecordMailService(
            IHostingEnvironment hostingEnvironment,
            IOptionsSnapshot<AppSettings> configuration,
            IBaseService baseService,
            ICommonService commonService,
            IBaseMailService mailService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
            this.baseService = baseService;
            this.commonService = commonService;
            this.mailService = mailService;
        }

        public async Task SendMailAsync(string performanceRecordIds, int actorId, ActionOnPerformanceRecord action)
        {
            string[] recordIds = performanceRecordIds.Split(",");
            Tuple<IEnumerable<PerformanceRecordModel>, IEnumerable<ProcessSettingModel>> tuple = await GetSettingToSendMailAsync(performanceRecordIds, actorId).ConfigureAwait(false);
            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync().ConfigureAwait(false);
            EmployeeBaseModel actor = await commonService.GetEmployeeByIdAsync(actorId).ConfigureAwait(false);

            foreach (string recordId in recordIds)
            {
                IEnumerable<ProcessSettingModel> receivers = tuple.Item2.Where(x => x.PerformanceRecordId.ToString() == recordId);
                if (receivers.Any(x => x.MailMode == PerformanceRecordMailMode.MemberToSupervisor))
                {
                    var receiver = receivers.Where(x => x.MailMode == PerformanceRecordMailMode.MemberToSupervisor).OrderBy(x => x.Priority).First();
                    var cc = receivers.Where(x => x.MailMode == PerformanceRecordMailMode.MemberToSupervisor && x.EvaluatorId != receiver.EvaluatorId)
                        .Select(x => x.Email);

                    await SendMailFromMemberToSupervisorAsync(actor, receiver, cc, action).ConfigureAwait(false);
                }

                if (receivers.Any(x => x.MailMode == PerformanceRecordMailMode.SupervisorToMember))
                {
                    var receiver = receivers.First(x => x.MailMode == PerformanceRecordMailMode.SupervisorToMember);

                    await SendMailFromSupervisorToMemberAsync(actor, receiver, action).ConfigureAwait(false);
                }

                if (receivers.Any(x => x.MailMode == PerformanceRecordMailMode.SupervisorToSupervisor))
                {
                    var employee = employees.First(x => x.EmployeeId.ToString() == tuple.Item1.First(y => y.PerformanceRecordId.ToString() == recordId).EmployeeIds);
                    var receiver = receivers.Where(x => x.MailMode == PerformanceRecordMailMode.SupervisorToSupervisor).OrderBy(x => x.Priority).First();
                    var cc = receivers.Where(x => x.MailMode == PerformanceRecordMailMode.SupervisorToSupervisor && x.EvaluatorId != receiver.EvaluatorId)
                        .Select(x => x.Email);

                    await SendMailFromSupervisorToSupervisorAsync(actor, employee, receiver, cc, action).ConfigureAwait(false);
                }
            }
        }

        private async Task<Tuple<IEnumerable<PerformanceRecordModel>, IEnumerable<ProcessSettingModel>>> GetSettingToSendMailAsync(string performanceRecordIds, int actorId)
        {
            var criteria = new GetSettingToSendMailCriteria
            {
                PerformanceRecordIds = performanceRecordIds,
                ActorId = actorId
            };

            return await baseService.FetchMultipleAsync<PerformanceRecordModel, ProcessSettingModel>(criteria).ConfigureAwait(false);
        }

        private async Task SendMailFromMemberToSupervisorAsync(
            EmployeeBaseModel actor,
            ProcessSettingModel receiver,
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
            EmployeeBaseModel actor,
            ProcessSettingModel receiver,
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
            EmployeeBaseModel actor,
            EmployeeBaseModel employee,
            ProcessSettingModel receiver,
            IEnumerable<string> cc,
            ActionOnPerformanceRecord action)
        {
            var emailTemplate = string.Format(SupervisorToSupervisorEmailTemplate, action == ActionOnPerformanceRecord.Create ? "Create" : "Edit");
            var subject = action == ActionOnPerformanceRecord.Create
                ? string.Format(Message.EmailSubject_CreatePCI_SupervisorToSupervisor, actor.NickName)
                : string.Format(Message.EmailSubject_EditPCI_SupervisorToSupervisor, actor.NickName);
            var filePath = Path.Combine(hostingEnvironment.WebRootPath, emailTemplate);

            var body = string.Format(
                mailService.ReadTextFile(filePath),
                receiver.Nickname,
                actor.NickName,
                employee.NickName,
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