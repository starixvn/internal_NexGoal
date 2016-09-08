namespace NexGoal.Core.Enumerations
{
    using System.ComponentModel;

    public enum NominationStatus : int
    {
        [Description("None")]
        None = 0,

        [Description("Nominated")]
        Nominated = 1,

        [Description("Shorted List")]
        ShortedList = 2,

        [Description("Winner")]
        Winner = 3
    }
}