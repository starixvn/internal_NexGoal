namespace NexGoal.Model.TargetTemplate
{
    using NexGoal.Model.KPI;
    using NexGoal.Model.KPIGroup;
    using System.Collections.Generic;

    public class TargetTemplateKPIGroupModel : KPIGroupModel
    {
        public bool IsSpreadKPIOriginal
        {
            get
            {
                return IsSpreadKPI;
            }
        }

        public IEnumerable<KPIModel> KPIs { get; set; }
    }
}