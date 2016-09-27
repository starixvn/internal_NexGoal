namespace NexGoal.Model
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Utilities;

    public class NominationEPAModel
    {
        public int EOQYId { get; set; }

        public int Year { get; set; }

        public string Quarter { get; set; }

        public string QuarterToString
        {
            get
            {
                return string.IsNullOrEmpty(Quarter) ? string.Empty : Quarter + "st Quarter";
            }
        }

        public int StatusId { get; set; }

        public string StatusToString
        {
            get
            {
                return Enumerations.GetDescription((NominationStatus)StatusId);
            }
        }
    }
}