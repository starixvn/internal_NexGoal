namespace NexGoal.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;

    public interface IPerformanceRecordMailService
    {
        Task SendMailAsync(string performanceRecordIds, int actorId, ActionOnPerformanceRecord action);
    }
}