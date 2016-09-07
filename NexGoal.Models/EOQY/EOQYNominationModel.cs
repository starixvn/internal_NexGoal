namespace NexGoal.Model
{
    using System.Collections.Generic;
    using NexGoal.Model.Employee;

    public class EOQYNominationModel
    {
        public EmployeeModel Nominee { get; set; }

        public List<NominationModel> Nominations { get; set; }

        public EOQYModel Period { get; set; }

        public int StatusId { get; set; }
    }
}