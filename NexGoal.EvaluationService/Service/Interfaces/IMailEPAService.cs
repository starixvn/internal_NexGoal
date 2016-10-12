namespace NexGoal.BackgroundService
{
    public interface IMailEPAService
    {
        void CheckSendNoticeAtBeginEmail();

        void CheckSendReminderEmail();

        void CheckSendNoticeAtEndPeriod();
    }
}
