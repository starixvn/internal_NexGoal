namespace NexGoal.Models.EmployeeNomination
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;

    public class EmployeeNominationModel : EmployeeBaseModel
    {
        [JsonIgnore]
        public int NominationPeriodId { get; set; }
        public NominationPeriodModel Period { get; set; }

        public IEnumerable<EmployeeNominationDetailModel> NominationDetails { get; set; }

        public NominationStatus StatusId { get; set; }
    }
}