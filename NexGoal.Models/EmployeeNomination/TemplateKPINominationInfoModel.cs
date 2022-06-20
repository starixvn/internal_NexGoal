namespace NexGoal.Models.EmployeeNomination
{
    using System.Collections.Generic;

    public class TemplateKPINominationInfoModel
    {
        public int KPIGroupId { get; set; }

        public string Name { get; set; }

        public IEnumerable<KPIModel> KPIs { get; set; }
    }
}
