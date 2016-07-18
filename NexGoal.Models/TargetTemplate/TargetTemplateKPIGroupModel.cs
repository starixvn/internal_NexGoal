namespace NexGoal.Model.TargetTemplate
{
    using System.Collections.Generic;
    using NexGoal.Model.KPI;
    using NexGoal.Model.KPIGroup;

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