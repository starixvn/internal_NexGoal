namespace NexGoal.BackgroundService
{
    public interface INominationEmailService
    {
        void SendEmail();

        void SendEmailToAllWhenBegining();

        void SendEmailToNominatorWhenRemind();

        void SendEmailToManagementWhenEnd();
    }
}
