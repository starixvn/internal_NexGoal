using NexGoal.Core.Model;

namespace NexGoal.Core.Services.Interfaces
{
    public interface ISessionService
    {
        UserSession CurrentUser { get; set; }
    }
}
