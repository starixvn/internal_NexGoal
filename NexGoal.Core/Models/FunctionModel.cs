namespace NexGoal.Core.Models
{
    using Enumerations;

    public class FunctionModel
    {
        public int FunctionId { get; set; }
        public string FunctionKey { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public Policy Policy { get; set; }
        public bool IsActive { get; set; }
    }
}