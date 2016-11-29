using System.Collections.Generic;

namespace NexGoal.Model
{
    public class EOQYEmailModel
    {
        public EOQYEmailModel()
        {
            EmailNominator = new List<EmailNominatorModel>();
        }

        public EOQYModel Nomination { get; set; }

        public IEnumerable<EmailNominatorModel> EmailNominator { get; set; }
    }
}
