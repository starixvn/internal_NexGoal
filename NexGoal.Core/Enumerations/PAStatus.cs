namespace NexGoal.Core.Enumerations
{
    using System.ComponentModel;

    public enum PAStatus
    {
        [Description("Open")]
        Open = 0,

        [Description("Closed")]
        Closed = 1,

        [Description("Rejected")]
        Rejected = 2
    }
}