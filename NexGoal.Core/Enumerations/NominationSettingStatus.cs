namespace NexGoal.Core.Enumerations
{
    using System.ComponentModel;

    public enum NominationSettingStatus
    {
        [Description("Not Ready")]
        NotReady = 0,

        [Description("In Progress")]
        InProgress = 1,

        Closed = 2
    }
}