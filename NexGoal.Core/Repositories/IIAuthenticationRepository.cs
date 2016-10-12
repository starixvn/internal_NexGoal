using Fanex.Data.Repository;
using NexGoal.Core.Model;

namespace NexGoal.Core.Repositories
{
    public interface IIAuthenticationRepository
    {
        UserSession GetEmployee(ICriteria criteria);
    }
}
