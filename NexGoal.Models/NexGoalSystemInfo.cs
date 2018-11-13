namespace NexGoal.Models
{
    using NexGoal.Core.Constants;
    using NexGoal.Models.Common;

    public static class NexGoalSystemInfo
    {
        public static EmployeeBaseModel NexGoal => new EmployeeBaseModel
        {
            EmployeeId = Constants.NexGoalSystemId,
            NickName = "NexGoal",
            FullName = "NexGoal",
            Avatar = "/dist/avatars/nexgoal.jpg"
        };
    }
}