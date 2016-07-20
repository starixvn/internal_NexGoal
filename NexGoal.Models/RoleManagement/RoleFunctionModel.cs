namespace NexGoal.Model.RoleManagement
{
    using NexGoal.Core.Model;
    using System.Collections.Generic;

    public class RoleFunctionModel : FunctionModel
    {
        public RoleFunctionModel()
        {
            ChildFunctions = new List<RoleFunctionModel>();
        }

        public IEnumerable<RoleFunctionModel> ChildFunctions { get; set; }
    }
}