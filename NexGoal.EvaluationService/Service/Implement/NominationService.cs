using NexGoal.Criteria;
using NexGoal.Model;
using NexGoal.Services.Implement;
using NexGoal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace NexGoal.BackgroundService
{
    public class NominationEmailService : INominationEmailService
    {
        private string startMail = "Views\\Emails\\StartNominationToAll.cshtml";
        private string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

        private readonly INominationService nominationService;
        private readonly IMailService mailService;
        
        public NominationEmailService() : this(null, null)
        {
        }

        public NominationEmailService(INominationService nominationService, IMailService mailService)
        {
            this.nominationService = nominationService ?? new NominationService();
            this.mailService = mailService ?? new MailService();
        }

        public void SendEmailToAllWhenBegining()
        {
            var criteria = new GetEOQYToSendEmailWhenBeginingCriteria();
            IEnumerable<EOQYEmailModel> nominations = nominationService.GetToSendEmail(criteria);
            foreach (var nomination in nominations)
            {
                foreach (var email in nomination.EmailNominator)
                {
                    mailService.SendMailToAllInNomination(Path.Combine(assemblyPath, startMail), nomination.Nomination, email);
                }
            }
        }

        public void SendEmailToAllWhenRemind()
        {
            var criteria = new GetEOQYToSendEmailWhenRemindCriteria();
            IEnumerable<EOQYEmailModel> nominations = nominationService.GetToSendEmail(criteria);
            foreach (var nomination in nominations)
            {
                foreach (var email in nomination.EmailNominator)
                {
                    mailService.SendMailToAllInNomination(Path.Combine(assemblyPath, startMail), nomination.Nomination, email);
                }
            }
        }
    }
}
