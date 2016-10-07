using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexGoal.BackgroundService
{
    public interface IMailEPAService
    {
        void CheckSendNoticeAtBeginEmail();

        void CheckSendReminderEmail();

        void CheckSendNoticeAtEndPeriod();
    }
}
