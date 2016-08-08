using Fanex.Data;
using Fanex.Data.Repository;
using NexGoal.Core.Model;
using System.Collections.Generic;
using System.Linq;

namespace NexGoal.Core.Repositories
{
    public class AuthenticationRepository : IIAuthenticationRepository
    {
        public UserSession GetEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    UserSession userSession = null;
                    var employeeIds = multi.Read<int>();
                    if (employeeIds.Any())
                    {
                        IEnumerable<FunctionModel> functions = multi.Read<FunctionModel>();
                        userSession = new UserSession
                        {
                            EmployeeId = employeeIds.First(),
                            Functions = functions
                        };
                    }

                    return userSession;
                }
            }
        }
    }
}
