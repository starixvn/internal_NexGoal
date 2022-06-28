namespace NexGoal.Models.EmployeeNomination
{
    using System.Collections.Generic;

    public class TemplateKPINominationInfoModel
    {
        public int KpiGroupId { get; set; }

        public string Name { get; set; }

        public int Rate { get; set; }

        public IEnumerable<KPIModel> Kpis { get; set; }
    }
}
