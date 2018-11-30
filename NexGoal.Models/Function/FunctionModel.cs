namespace NexGoal.Models.Function
{
    using NexGoal.Core.Enumerations;

    public class FunctionModel
    {
        public int FunctionId { get; set; }
        public string FunctionKey { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public Policy? OnlyPolicy { get; set; }
        public int DisplayOrder { get; set; }
        public Policy Policy { get; set; }
        public bool IsActive { get; set; }
    }
}