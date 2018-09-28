namespace NexGoal.Models.EmployeeNomination
{
    using Newtonsoft.Json;
    using NexGoal.Models.Common;

    public class EmployeeNominationDetailModel
    {
        [JsonIgnore]
        public int NominationPeriodId { get; set; }

        [JsonIgnore]
        public int NominatorId { get; set; }
        public EmployeeBaseModel Nominator { get; set; }

        public string Comment { get; set; }
    }
}