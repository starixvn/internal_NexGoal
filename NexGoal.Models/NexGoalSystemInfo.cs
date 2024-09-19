namespace NexGoal.Models
{
    using NexGoal.Core.Constants;
    using NexGoal.Models.Common;

    public class NexGoalSystemInfo : Employee
    {
        public override int EmployeeId { get; set; } = Constants.NexGoalSystemId;
    }
}