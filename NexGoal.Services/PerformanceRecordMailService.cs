namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceRecord;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Resources;

    public class PerformanceRecordMailService : IPerformanceRecordMailService
    {
        private const string CreateEmailTemplate = @"EmailTemplates\\PCI\\Create.cshtml";
        private const string EditEmailTemplate = @"EmailTemplates\\PCI\\Edit.cshtml";

        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;
        private readonly IBaseMailService mailService;

        public PerformanceRecordMailService(
            IHostingEnvironment hostingEnvironment,
            IBaseService baseService,
            ICommonService commonService,
            IBaseMailService mailService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.baseService = baseService;
            this.commonService = commonService;
            this.mailService = mailService;
        }

        public async Task SendMailAsync(string performanceRecordIds, int actorId, PerformanceRecordMailMode mailMode)
        {
            IEnumerable<ProcessSettingModel> mailInfos = await GetSettingToSendMailAsync(performanceRecordIds, actorId).ConfigureAwait(false);
            EmployeeBaseModel actor = await commonService.GetEmployeeByIdAsync(actorId).ConfigureAwait(false);
            IEnumerable<ProcessSettingModel> receivers = mailInfos.Where(x => x.ProcessId == (int)PerformanceProcess.Self || x.ProcessId == (int)PerformanceProcess.Evaluator)
                .GroupBy(x => x.PerformanceRecordId).Select(x => x.OrderBy(y => y.ProcessId)).Select(x => x.First());

            var emailTemplate = mailMode == PerformanceRecordMailMode.Create ? CreateEmailTemplate : EditEmailTemplate;
            var subject = mailMode == PerformanceRecordMailMode.Create ? ViewLang.CreatePerformanceCheckin : ViewLang.EditPerformanceCheckin;
            var filePath = Path.Combine(hostingEnvironment.ContentRootPath, emailTemplate);

            foreach (ProcessSettingModel receiver in receivers)
            {
                var body = string.Format(mailService.ReadTextFile(filePath), receiver.Nickname, actor.NickName);
                var cc = mailInfos.Where(x => x.PerformanceRecordId == receiver.PerformanceRecordId && x.ProcessId != receiver.ProcessId)
                    .Select(x => x.Email).Distinct();

                var mailModel = new MailModel
                {
                    Subject = subject,
                    ReceiverEmail = receiver.Email,
                    Body = body,
                    Cc = cc
                };

                await mailService.SendMailAsync(mailModel).ConfigureAwait(false);
            }
        }

        private async Task<IEnumerable<ProcessSettingModel>> GetSettingToSendMailAsync(string performanceRecordIds, int actorId)
        {
            var criteria = new GetSettingToSendMailCriteria
            {
                PerformanceRecordIds = performanceRecordIds,
                ActorId = actorId
            };

            return await baseService.GetDataAsync<ProcessSettingModel>(criteria).ConfigureAwait(false);
        }
    }
}