namespace NexGoal.Tools
{
    public class PAFocusModel
    {
        public int PAFocusId { get; set; }

        public string EncryptedContribution { get; set; }

        public int? Contribution { get; set; }

        public string EncryptedBonusGiven { get; set; }

        public int? BonusGiven { get; set; }

        public bool? IsReadyPromotion { get; set; }

        public string EncryptedIsReadyPromotion { get; set; }

        public bool? IsPoorPerformance { get; set; }

        public string EncryptedIsPoorPerformance { get; set; }        
    }
}
