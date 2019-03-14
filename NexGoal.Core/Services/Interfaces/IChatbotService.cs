namespace NexGoal.Core.Services
{
    using System.Threading.Tasks;

    public interface IChatbotService
    {
        Task SendMessageAsync(string conversationId, string message);
    }
}