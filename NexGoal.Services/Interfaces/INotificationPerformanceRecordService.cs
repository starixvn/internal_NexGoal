namespace NexGoal.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;

    public interface INotificationPerformanceRecordService
    {
        Task SendMailAsync(string performanceRecordIds, int actorId, ActionOnPerformanceRecord action);

        Task SendNotificationAsync(string performanceRecordIds, int actorId, ActionOnPerformanceRecord action);

        Task SendSkypeMessageAsync(string performanceRecordIds, int actorId, ActionOnPerformanceRecord action);
    }
}