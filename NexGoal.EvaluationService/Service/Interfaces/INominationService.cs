namespace NexGoal.BackgroundService
{
    public interface INominationEmailService
    {
        void SendEmailToAllWhenBegining();

        void SendEmailToAllWhenRemind();
    }
}
