namespace NexGoal.BackgroundService
{
    public interface IPeriodEvalutionService
    {
        void UpdatePeriodIsEnd();

        void GeneratePAFocus();
    }
}
