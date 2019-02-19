namespace NexGoal.Core.Enumerations
{
    using System.ComponentModel;

    public enum PerformanceProcess
    {
        [Description("Human Resource")]
        HumanResource = 1,

        [Description("Self")]
        Self,

        [Description("Project Manager")]
        ProjectManager,

        [Description("Evaluator")]
        Evaluator,

        [Description("Reviewer")]
        Reviewer
    }
}