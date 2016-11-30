using NexGoal.Core.Enumerations;
using NexGoal.Criteria;
using NexGoal.Model;
using NexGoal.Services.Implement;
using NexGoal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NexGoal.BackgroundService
{
    public class NominationEmailService : INominationEmailService
    {
        private readonly INominationService nominationService;
        private readonly IMailService mailService;

        private string startMailToNominator = "Views\\Emails\\StartNominationToNominator.cshtml";
        private string startMailToManagement = "Views\\Emails\\StartNominationToManagement.cshtml";
        private string endMailToManagement = "Views\\Emails\\EndNominationToManagement.cshtml";
        private string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

        public NominationEmailService() : this(null, null)
        {
        }

        public NominationEmailService(INominationService nominationService, IMailService mailService)
        {
            this.nominationService = nominationService ?? new NominationService();
            this.mailService = mailService ?? new MailService();
        }

        public void SendEmail()
        {
            SendEmailToAllWhenBegining();
            SendEmailToNominatorWhenRemind();
            SendEmailToManagementWhenEnd();
        }

        public void SendEmailToAllWhenBegining()
        {
            var criteria = new GetEOQYToSendEmailWhenBeginingCriteria();
            IEnumerable<EOQYEmailModel> nominations = nominationService.GetToSendEmail(criteria);
            foreach (var nomination in nominations)
            {
                foreach (var email in nomination.EmailNominator)
                {
                    if (email.RoleId == (int)NominationRole.Nominator)
                    {
                        mailService.SendStartMailToNominator(Path.Combine(assemblyPath, startMailToNominator), nomination.Nomination, email);
                    }
                    else
                    {
                        mailService.SendStartMailToManagement(Path.Combine(assemblyPath, startMailToManagement), nomination.Nomination, email);
                    }
                }
            }
        }

        public void SendEmailToNominatorWhenRemind()
        {
            var criteria = new GetEOQYToSendEmailWhenRemindCriteria();
            IEnumerable<EOQYEmailModel> nominations = nominationService.GetToSendEmail(criteria);
            foreach (var nomination in nominations)
            {
                foreach (var email in nomination.EmailNominator.Where(i => i.RoleId == (int)NominationRole.Nominator))
                {
                    mailService.SendRemindMailToNominator(Path.Combine(assemblyPath, startMailToNominator), nomination.Nomination, email);
                }
            }
        }

        public void SendEmailToManagementWhenEnd()
        {
            var criteria = new GetEOQYToSendEmailWhenEndCriteria();
            IEnumerable<EOQYEmailModel> nominations = nominationService.GetToSendEmail(criteria);
            foreach (var nomination in nominations)
            {
                foreach (var email in nomination.EmailNominator.Where(i => i.RoleId == (int)NominationRole.NominationBoard))
                {
                    mailService.SendEndMailToManagement(Path.Combine(assemblyPath, endMailToManagement), nomination.Nomination, email);
                }
            }
        }
    }
}
