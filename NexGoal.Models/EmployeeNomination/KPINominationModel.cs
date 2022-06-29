namespace NexGoal.Models.EmployeeNomination
{
    using System;
    using NexGoal.Core.Extensions;

    [Serializable]
    [XMLName("Row")]

    public class KPINominationModel
    {
        public int KpiGroupId { get; set; }

        public int KpiId { get; set; }

        public int Rate { get; set; }

        public string Comment { get; set; }
    }
}
