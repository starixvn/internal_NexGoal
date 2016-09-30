using System.ComponentModel;

namespace NexGoal.Core.Enumerations
{
    public enum PAFocusType
    {
        None = 0,

        [Description("Strongly disagree")]
        StronglyDisagree = 1,

        [Description("Disagree")]
        Disagree,

        [Description("Neither agree nor disagree")]
        AgreeNorDisagree,

        [Description("Agree")]
        Agree,

        [Description("Strongly agree")]
        StronglyAgree
    }
}
