using Fanex.Data.Repository;
using NexGoal.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexGoal.Core.Repositories
{
    public interface IIAuthenticationRepository
    {
        UserSession GetEmployee(ICriteria criteria);
    }
}
