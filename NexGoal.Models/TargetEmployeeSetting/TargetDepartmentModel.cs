namespace NexGoal.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using NexGoal.Model.Department;
    using NexGoal.Model.TargetTemplate;

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