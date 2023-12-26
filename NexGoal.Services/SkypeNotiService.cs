namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord.Notification;
    using NexGoal.Resources;

    public class SkypeNotiService : ISkypeNotiService
    {
        private readonly AppSettings appSettings;
        private readonly IChatbotService chatbotService;

        public SkypeNotiService(
            AppSettings appSettings,
            IChatbotService chatbotService)
        {
            this.appSettings = appSettings;
            this.chatbotService = chatbotService;
        }

        public async Task SendMessageMemberToSupervisorAsync(Employee actor,
            IEnumerable<ProcessSettingDto> recievers,
            ActionOnPerformanceRecord action)
        {
            foreach (var reciever in recievers)
            {
                var message = action == ActionOnPerformanceRecord.Create ?
                    string.Format(Message.SkypeMessageTemplate_PR_CreateMemberToSuperVisor, reciever.Nickname, actor.NickName, appSettings.ApplicationDomain) :
                    string.Format(Message.SkypeMessageTemplate_PR_EditMemberToSupervisor, reciever.Nickname, actor.NickName, appSettings.ApplicationDomain);

                await chatbotService.SendMessageAsync(reciever.ConversationId, message);
            }
        }

        public async Task SendMessageSupervisorToMemberAsync(Employee actor,
            IEnumerable<ProcessSettingDto> recievers,
            ActionOnPerformanceRecord action)
        {
            foreach (var reciever in recievers)
            {
                var message = action == ActionOnPerformanceRecord.Create ?
                    string.Format(Message.SkypeMessageTemplate_PR_CreateSupervisorToMember, reciever.Nickname, actor.NickName, appSettings.ApplicationDomain) :
                    string.Format(Message.SkypeMessageTemplate_PR_EditSupervisorToMember, reciever.Nickname, actor.NickName, appSettings.ApplicationDomain);

                await chatbotService.SendMessageAsync(reciever.ConversationId, message);
            }
        }

        public async Task SendMessageSupervisorToSupervisorAsync(Employee actor,
            IEnumerable<ProcessSettingDto> recievers,
            EmployeeDto employee,
            ActionOnPerformanceRecord action)
        {
            foreach (var reciever in recievers)
            {
                var message = action == ActionOnPerformanceRecord.Create ?
                    string.Format(Message.SkypeMessageTemplate_PR_CreateSupervisorToSupervisor, reciever.Nickname, actor.NickName, employee.Nickname, appSettings.ApplicationDomain) :
                    string.Format(Message.SkypeMessageTemplate_PR_EditSupervisorToSupervisor, reciever.Nickname, actor.NickName, employee.Nickname, appSettings.ApplicationDomain);

                await chatbotService.SendMessageAsync(reciever.ConversationId, message);
            }
        }
    }
}