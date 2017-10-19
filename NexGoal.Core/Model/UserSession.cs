namespace NexGoal.Core.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using NexGoal.Core.Enumerations;

    public class UserSession
    {
        public UserSession()
        {
            Functions = new List<FunctionModel>();
        }

        public int EmployeeId { get; set; }

        public IEnumerable<FunctionModel> Functions { get; set; }

        public int GetPermission(string functionKey)
        {
            int policy;

            if (HasPermission(functionKey, Policy.Host))
            {
                policy = (int)Policy.Host;
            }
            else if (HasPermission(functionKey, Policy.Read))
            {
                policy = (int)Policy.Read;
            }
            else
            {
                policy = (int)Policy.None;
            }

            return policy;
        }

        public bool HasPermission(string functionKey, Policy policy)
        {
            var function = Functions.Where(i => i.FunctionKey.Equals(functionKey));
            if (function.Any())
            {
                return policy == Policy.Host ? function.First().IsHost : function.First().IsRead;
            }

            return false;
        }
    }
}