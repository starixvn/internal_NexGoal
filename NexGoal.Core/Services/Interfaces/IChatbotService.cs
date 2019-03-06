namespace NexGoal.Core.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IChatbotService
    {
        Task SendMessageAsync(string conversationId, string message);
    }
}