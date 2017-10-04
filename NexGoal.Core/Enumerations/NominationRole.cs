namespace NexGoal.Core.Enumerations
{
    using System.ComponentModel;

    public enum NominationRole : int
    {
        [Description("None")]
        None = 0,

        [Description("Nominator")]
        Nominator = 1,

        [Description("Committee")]
        Committee = 2
    }
}