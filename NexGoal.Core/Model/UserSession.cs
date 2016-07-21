namespace NexGoal.Core.Model
{
    using NexGoal.Core.Enumerations;
    using System.Collections.Generic;
    using System.Linq;

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
            int policy = (int)Policy.None;

            if (HasPermission(functionKey, Policy.Host))
            {
                policy = (int)Policy.Host;
            }
            else if (HasPermission(functionKey, Policy.Read))
            {
                policy = (int)Policy.Read;
            }

            return policy;
        }

        public bool HasPermission(string functionKey, Policy policy)
        {
            var function = Functions.Where(i => i.FunctionKey.Equals(functionKey));
            if (function.Any())
            {
                switch (policy)
                {
                    case Policy.Host:
                        return function.First().IsHost;
                    default:
                        return function.First().IsRead;
                }
            }

            return false;
        }
    }
}