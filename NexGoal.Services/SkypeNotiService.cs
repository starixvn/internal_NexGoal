namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services.Interfaces;
    using NexGoal.Core.SkypeMessages;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord.Notification;
    using NexGoal.Services.Interfaces;

    public class SkypeNotiService : ISkypeNotiService
    {
        private readonly IOptionsSnapshot<AppSettings> configuration;
        private readonly IChatbotService chatbotService;

        public SkypeNotiService(
            IOptionsSnapshot<AppSettings> configuration,
            IChatbotService chatbotService)
        {
            this.configuration = configuration;
            this.chatbotService = chatbotService;
        }

        public async Task SendMessageMemberToSupervisor(Employee actor,
            IEnumerable<ProcessSettingDto> recievers,
            ActionOnPerformanceRecord action)
        {
            foreach (var reciever in recievers)
            {
                var message = action == ActionOnPerformanceRecord.Create ?
                    PerformanceRecordTemplates.CreatePRMemberToSuperVisor(reciever.Nickname, actor.NickName, configuration.Value.ApplicationDomain) :
                    PerformanceRecordTemplates.EditPRMemberToSuperVisor(reciever.Nickname, actor.NickName, configuration.Value.ApplicationDomain);

                await chatbotService.SendMessageAsync(reciever.ConversationId, message).ConfigureAwait(false);
            }
        }

        public async Task SendMessageSupervisorToMember(Employee actor,
            IEnumerable<ProcessSettingDto> recievers,
            ActionOnPerformanceRecord action)
        {
            foreach (var reciever in recievers)
            {
                var message = action == ActionOnPerformanceRecord.Create ?
                    PerformanceRecordTemplates.CreatePRSupervisorToMember(reciever.Nickname, actor.NickName, configuration.Value.ApplicationDomain) :
                    PerformanceRecordTemplates.EditPRSupervisorToMember(reciever.Nickname, actor.NickName, configuration.Value.ApplicationDomain);

                await chatbotService.SendMessageAsync(reciever.ConversationId, message).ConfigureAwait(false);
            }
        }

        public async Task SendMessageSupervisorToSupervisor(Employee actor,
            IEnumerable<ProcessSettingDto> recievers,
            EmployeeDto employee,
            ActionOnPerformanceRecord action)
        {
            foreach (var reciever in recievers)
            {
                var message = action == ActionOnPerformanceRecord.Create ?
                    PerformanceRecordTemplates.CreatePRSupervisorToSupervisor(reciever.Nickname, actor.NickName, employee.Nickname, configuration.Value.ApplicationDomain) :
                    PerformanceRecordTemplates.EditPRSupervisorToSupervisor(reciever.Nickname, actor.NickName, employee.Nickname, configuration.Value.ApplicationDomain);

                await chatbotService.SendMessageAsync(reciever.ConversationId, message).ConfigureAwait(false);
            }
        }
    }
}