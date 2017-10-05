namespace NexGoal.Model.Period
{
    using System;
    using System.Collections.Generic;
    using NexGoal.Model.Department;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.TargetTemplate;

    [Serializable]
    public class PeriodEmployeeSettingModel
    {
        public PeriodEmployeeSettingModel()
        {
            Group = new Group();
            HumanResources = new List<PeriodFlowModel>();
            SelfEvaluators = new List<PeriodFlowModel>();
            Evaluators = new List<PeriodFlowModel>();
            Reviewers = new List<PeriodFlowModel>();
        }

        public int PeriodId { get; set; }

        public string Name { get; set; }

        public Employee Employee { get; set; }

        public DepartmentModel Department { get; set; }

        public Group Group { get; set; }

        public JobTitle JobTitle { get; set; }

        public IList<PeriodFlowModel> HumanResources { get; set; }

        public IList<PeriodFlowModel> SelfEvaluators { get; set; }

        public IList<PeriodFlowModel> Evaluators { get; set; }

        public IList<PeriodFlowModel> Reviewers { get; set; }

        public bool IsIncluded { get; set; }

        public TargetTemplateModel TargetTemplate { get; set; }

        public int NumberEvaluators { get; set; }
    }
}