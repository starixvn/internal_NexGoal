namespace NexGoal.Model
{
    using System.Collections.Generic;
    using NexGoal.Model.Employee;

    public class NominationModel
    {
        public int NominationId { get; set; }

        public EOQYModel Period { get; set; }

        public EmployeeModel Nominee { get; set; }

        public EmployeeModel Nominator { get; set; }

        public List<NominationDetailModel> NominationDetails { get; set; }

        public int StatusId { get; set; }
    }
}