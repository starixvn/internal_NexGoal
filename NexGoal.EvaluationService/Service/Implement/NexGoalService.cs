using Fanex.Logging;
using System;
using System.Threading.Tasks;

namespace NexGoal.BackgroundService
{
    public class NexGoalService : INexGoalService
    {
        private readonly ISemiAnnualService semiAnnualService;
        private readonly IProbationService probationService;
        private readonly IPromotionService promotionService;
        private readonly IProcessService processService;
        private readonly IPeriodEvalutionService periodEvalutionService;
        private readonly IMailEPAService mailService;

        public NexGoalService()
            : this(null, null, null, null, null, null)
        {
        }

        public NexGoalService(
            ISemiAnnualService semiAnnualService,
            IProbationService probationService,
            IPromotionService promotionService,
            IProcessService processService,
            IPeriodEvalutionService periodEvalutionService,
            IMailEPAService mailService)
        {
            this.semiAnnualService = semiAnnualService ?? new SemiAnnualService();
            this.probationService = probationService ?? new ProbationService();
            this.promotionService = promotionService ?? new PromotionService();
            this.processService = processService ?? new ProcessService();
            this.periodEvalutionService = periodEvalutionService ?? new PeriodEvalutionService();
            this.mailService = mailService ?? new MailEPAService();
        }

        public void Run()
        {
            try
            {
                semiAnnualService.GeneratedEPA();
                probationService.GeneratedEPA();
                promotionService.GeneratedEPA();
                processService.UpdateInformationAtEndProccess();
                periodEvalutionService.UpdatePeriodIsEnd();
                periodEvalutionService.GeneratePAFocus();
                CheckSendMail();
            }
            catch (Exception ex)
            {
                LogService.Error("Error: " + ex.InnerException.Message, ex);
            }
        }

        private void CheckSendMail()
        {
            mailService.CheckSendNoticeAtBeginEmail();
            mailService.CheckSendReminderEmail();
            mailService.CheckSendNoticeAtEndPeriod();
        }
    }
}
