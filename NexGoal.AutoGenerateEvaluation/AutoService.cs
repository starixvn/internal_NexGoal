namespace NexGoal.AutoGenerateEvaluation
{
    using Fanex.Logging;
    using System;
    using System.Configuration;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using System.Timers;

    public partial class AutoService : ServiceBase
    {
        private readonly Timer updateCounterTimer;
        private readonly ISemiAnnualService semiAnnualService;
        private readonly IProbationService probationService;
        private readonly IPromotionService promotionService;
        private readonly IProcessService processService;
        private readonly IPeriodEvalutionService periodEvalutionService;
        private readonly IMailEPAService mailService;
        private bool _isBusy;

        public AutoService()
        {
            InitializeComponent();

            var schedulerTime = DateTime.Today.AddDays(1).AddHours(5);

            updateCounterTimer = new Timer { Interval = schedulerTime.Subtract(DateTime.Now).TotalSeconds * 1000 };
            updateCounterTimer.Elapsed += ExecuteJob;
            updateCounterTimer.Enabled = false;

            semiAnnualService = new SemiAnnualService();
            probationService = new ProbationService();
            promotionService = new PromotionService();
            processService = new ProcessService();
            periodEvalutionService = new PeriodEvalutionService();
            mailService = new MailEPAService();
        }

        protected override void OnStart(string[] args)
        {
            updateCounterTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            updateCounterTimer.Enabled = false;
        }

        private void ExecuteJob(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (Math.Abs(updateCounterTimer.Interval - (AppSetting.TimerInterval * 1000)) > 0)
                {
                    updateCounterTimer.Interval = AppSetting.TimerInterval * 1000;
                }

                if (!_isBusy)
                {
                    _isBusy = true;
                    CheckAndExecute();
                    _isBusy = false;
                }
            }
            catch (Exception ex)
            {
                _isBusy = false;
                LogService.Error(ex.InnerException.Message);
            }
        }

        private void CheckAndExecute()
        {
            semiAnnualService.CheckSemiAnnualPeriod();
            probationService.CheckProbationPeriod();
            promotionService.CheckPromotionPeriod();
            CheckSendMail();
            processService.CheckEndProcess();
            periodEvalutionService.CheckEndEvaluationPeriod();
        }

        private void CheckSendMail()
        {
            Task.Factory.StartNew(mailService.CheckSendNoticeAtBeginEmail);

            Task.Factory.StartNew(mailService.CheckSendReminderEmail);
        }
    }
}