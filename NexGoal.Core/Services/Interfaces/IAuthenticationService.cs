using NexGoal.Core.Criteria;
using NexGoal.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexGoal.Core.Services
{
    public interface IAuthenticationService
    {
        UserSession GetEmployeeByUsername(string domainName);
    }
}
