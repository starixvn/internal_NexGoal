namespace NexGoal.Core.Enumerations
{
    using System.ComponentModel;

    public enum PerformanceProcess
    {
        [Description("Human Resource")]
        HumanResource = 1,

        [Description("Self")]
        Self = 2,

        [Description("Project Manager")]
        ProjectManager = 3,

        [Description("Evaluator")]
        Evaluator = 4,

        [Description("Reviewer")]
        Reviewer = 5
    }
}