namespace NexGoal.Models.Auth
{
    using NexGoal.Core.Enumerations;

    public class FunctionSession
    {
        public int FunctionId { get; set; }
        public string FunctionKey { get; set; }
        public string Name { get; set; }
        public Policy Policy { get; set; }
    }
}