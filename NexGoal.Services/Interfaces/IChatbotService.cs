namespace NexGoal.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IChatbotService
    {
        Task SendMessage(string conversationId, string message);
    }
}