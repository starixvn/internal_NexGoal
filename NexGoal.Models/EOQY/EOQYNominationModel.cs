namespace NexGoal.Model
{
    using System.Collections.Generic;

    public class EOQYNominationModel
    {
        public int NominationId { get; set; }

        public EOQYModel Period { get; set; }

        public NomineeModel Nominee { get; set; }

        public NomineeModel Nominator { get; set; }

        public List<EOQYQuestionModel> NominationDetails { get; set; }

        public int StatusId { get; set; }

        public int RoleId { get; set; }
    }
}