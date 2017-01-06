namespace NexGoal.BackgroundService
{
    using System;
    using Fanex.Logging;

    public class NexGoalService : INexGoalService
    {
        private readonly ISemiAnnualService semiAnnualService;
        private readonly IProbationService probationService;
        private readonly IProcessService processService;
        private readonly IPeriodEvalutionService periodEvalutionService;
        private readonly IMailEPAService mailService;
        private readonly INominationEmailService nominationService;

        public NexGoalService()
            : this(null, null, null, null, null, null)
        {
        }

        public NexGoalService(
            ISemiAnnualService semiAnnualService,
            IProbationService probationService,
            IProcessService processService,
            IPeriodEvalutionService periodEvalutionService,
            IMailEPAService mailService,
            INominationEmailService nominationService)
        {
            this.semiAnnualService = semiAnnualService ?? new SemiAnnualService();
            this.probationService = probationService ?? new ProbationService();
            this.processService = processService ?? new ProcessService();
            this.periodEvalutionService = periodEvalutionService ?? new PeriodEvalutionService();
            this.mailService = mailService ?? new MailEPAService();
            this.nominationService = nominationService ?? new NominationEmailService();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Reviewed")]
        public void Run()
        {
            try
            {
                semiAnnualService.GeneratedEPA();
                probationService.GeneratedEPA();
                processService.UpdateInformationAtEndProccess();
                periodEvalutionService.UpdatePeriodIsEnd();
                periodEvalutionService.GeneratePAFocus();
                nominationService.SendEmail();
                CheckSendMail();
            }
            catch (Exception ex)
            {
                LogService.Error(ex.InnerException.Message, ex);
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