using NexGoal.Core.Model;

namespace NexGoal.Services.Interfaces
{
    public interface ISessionService
    {
        UserSession CurrentUser { get; set; }
    }
}
