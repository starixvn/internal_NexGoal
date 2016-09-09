namespace NexGoal.Core.Enumerations
{
    using System.ComponentModel;

    public enum NominationSettingStatus : int
    {
        [Description("Not Started")]
        NotStarted = 0,

        [Description("In Progress")]
        InProgress = 1,

        [Description("Closed")]
        Closed = 2
    }
}