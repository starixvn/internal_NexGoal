namespace NexGoal.Model
{
    using System.Collections.Generic;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.Employee;

    public class EOQYNominationModel
    {
        public int NominationId { get; set; }

        public EOQYModel Period { get; set; }

        public EmployeeModel Nominee { get; set; }

        public EmployeeModel Nominator { get; set; }

        public List<EOQYQuestionModel> NominationDetails { get; set; }

        public int StatusId { get; set; }

        public int RoleId { get; set; }
    }
}