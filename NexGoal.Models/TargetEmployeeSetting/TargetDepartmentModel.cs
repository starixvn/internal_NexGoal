namespace NexGoal.Model.TargetEmployeeSetting
{
    using NexGoal.Model.Department;
    using NexGoal.Model.TargetTemplate;
    using System.Collections.Generic;

    public class TargetDepartmentModel : DepartmentModel
    {
        public TargetDepartmentModel()
        {
            TargetTemplates = new List<TargetTemplateModel>();
        }

        public TargetTemplateModel RawTargetTemplate { get; set; }

        public IList<TargetTemplateModel> TargetTemplates { get; set; }

        public int NumberTargetTemplates
        {
            get
            {
                int numberTargetTemplates = TargetTemplates.Count;
                return numberTargetTemplates != 0 ? numberTargetTemplates : 1;
            }
        }
    }
}