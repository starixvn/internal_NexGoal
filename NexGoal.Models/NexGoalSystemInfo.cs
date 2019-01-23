namespace NexGoal.Models
{
    using NexGoal.Core.Constants;
    using NexGoal.Models.Common;

    public static class NexGoalSystemInfo
    {
        public static Employee NexGoal => new Employee
        {
            EmployeeId = Constants.NexGoalSystemId,
            NickName = "NexGoal",
            FullName = "NexGoal"
        };
    }
}