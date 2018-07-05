namespace NexGoal.ViewModels.Function
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NexGoal.Models.Function;

    public class AssignPermissionViewModel
    {
        public int RoleId { get; set; }
        public IEnumerable<FunctionModel> Functions { get; set; } = new List<FunctionModel>();
        public SelectList Roles { get; set; }
        public bool IsHost { get; set; }
    }
}