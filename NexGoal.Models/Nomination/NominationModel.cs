namespace NexGoal.Model.Nomination
{
    using System.Collections.Generic;

    public class NominationModel
    {
        public int NominationId { get; set; }

        public EOQYModel Period { get; set; }

        public NomineeModel Nominee { get; set; }

        public NomineeModel Nominator { get; set; }

        public List<QuestionModel> NominationDetails { get; set; }

        public int StatusId { get; set; }

        public int RoleId { get; set; }
    }
}