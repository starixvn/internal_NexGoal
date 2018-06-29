namespace NexGoal.Core.Models
{
    using System.Collections.Generic;
    using Enumerations;

    public class FunctionModel
    {
        public int FunctionId { get; set; }
        public string FunctionKey { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int Level { get; set; }
        public Policy Policy { get; set; }
        public bool IsActive { get; set; }

        public IList<FunctionModel> Childs { get; set; } = new List<FunctionModel>();
    }
}